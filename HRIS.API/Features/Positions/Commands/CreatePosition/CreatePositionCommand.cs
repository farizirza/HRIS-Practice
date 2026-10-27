using HRIS.API.Common.Exceptions;
using HRIS.API.Data;
using HRIS.API.Data.Repositories;
using HRIS.API.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Features.Positions.Commands.CreatePosition;

public record CreatePositionCommand(CreatePositionRequest Request) : IRequest<Guid>;

public class CreatePositionCommandHandler(IUnitOfWork uow) : IRequestHandler<CreatePositionCommand, Guid>
{
    public async Task<Guid> Handle(CreatePositionCommand command, CancellationToken ct)
    {
        var trimmedTitle = command.Request.Title.Trim();

        if (await uow.Positions.TitleExistsAsync(trimmedTitle, ct))
        {
            throw new DuplicatePositionException($"Position title '{trimmedTitle}' is already used.");
        }

        var position = command.Request.ToEntity();

        await uow.Positions.AddAsync(position, ct);

        try
        {
            await uow.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.IsUniqueConstraintViolation())
        {
            throw new DuplicatePositionException("Position title is already used.", ex);
        }

        return position.Id;
    }
}