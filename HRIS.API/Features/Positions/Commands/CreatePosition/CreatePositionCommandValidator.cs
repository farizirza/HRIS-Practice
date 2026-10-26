using FluentValidation;

namespace HRIS.API.Features.Positions.Commands.CreatePosition;

public class CreatePositionCommandValidator : AbstractValidator<CreatePositionCommand>
{
    public CreatePositionCommandValidator()
    {
        RuleFor(x => x.Request.Title).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Request.Level).InclusiveBetween(1, 10);   // 1 = staff ... 10 = executive
    }
}