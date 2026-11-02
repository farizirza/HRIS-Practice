using HRIS.API.Common.Exceptions;
using HRIS.API.Data;
using HRIS.API.Data.Repositories;
using HRIS.API.Entities;
using HRIS.API.Extensions;
using HRIS.API.Models;
using HRIS.API.Services.Email;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Features.Employees.Commands.CreateEmployee;

public record CreateEmployeeCommand(CreateEmployeeRequest Request) : IRequest<Guid>;

public class CreateEmployeeCommandHandler(
    IUnitOfWork uow,
    TimeProvider timeProvider,                      // unchanged since Module 6: sets CreatedAt
    IEmailService emailService,
    IEmailTemplateRenderer templateRenderer,
    IConfiguration configuration,
    ILogger<CreateEmployeeCommandHandler> logger) : IRequestHandler<CreateEmployeeCommand, Guid>
{
    public async Task<Guid> Handle(CreateEmployeeCommand command, CancellationToken ct)
    {
        // Module 7: normalization after validation, before the checks and the save.
        var request = command.Request with { Email = command.Request.Email.NormalizeEmail() };

        if (await uow.Employees.EmployeeNumberExistsAsync(request.EmployeeNumber, ct))
        {
            throw new DuplicateEmployeeException($"Employee number '{request.EmployeeNumber}' is already used.");
        }

        if (await uow.Employees.EmailExistsAsync(request.Email, ct))
        {
            throw new DuplicateEmployeeException("The email is already used by another employee.");   // no email in the message: messages are logged
        }

        var employee = request.ToEntity(timeProvider.GetUtcNow(), EmployeeStatus.Active);
        await uow.Employees.AddAsync(employee, ct);

        try
        {
            await uow.SaveChangesAsync(ct);
        }
        // Only a unique-index violation is translated; any other DbUpdateException travels up unchanged.
        catch (DbUpdateException ex) when (ex.IsUniqueConstraintViolation())
        {
            throw new DuplicateEmployeeException("Employee number or email is already used.", ex);
        }

        logger.LogInformation("Employee {EmployeeId} created", employee.Id);

        // Module 9: the employee is already saved. An email problem must NOT turn a successful create into an error.
        try
        {
            var activationUrl = $"{configuration["App:FrontendUrl"]}/activate";
            var html = await templateRenderer.RenderAsync("WelcomeEmployee", new Dictionary<string, string>
            {
                ["FullName"] = employee.FullName,
                ["EmployeeNumber"] = employee.EmployeeNumber,
                ["ActivationUrl"] = activationUrl
            }, ct);

            await emailService.SendEmailAsync(new MailRequest
            {
                ToEmail = employee.Email,
                Subject = $"Welcome to the team, {employee.FullName}",
                HtmlBody = html,
                TextBody = $"Welcome, {employee.FullName}. Your employee number is {employee.EmployeeNumber}. Activate your account: {activationUrl}"
            }, ct);
        }
        catch (EmailDeliveryException ex)
        {
            // Message templates in logger calls; the employee id identifies the record, the email address is not logged.
            logger.LogWarning("Welcome email for employee {EmployeeId} was not delivered: {Reason} ({Cause})",
                employee.Id, ex.Message, ex.InnerException?.GetType().Name);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Welcome email for employee {EmployeeId} failed unexpectedly", employee.Id);
        }

        return employee.Id;
    }
}