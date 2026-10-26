using HRIS.API.Data.Repositories;
using HRIS.API.Models;
using MediatR;

namespace HRIS.API.Features.Positions.Commands.CreatePosition;

public record CreatePositionCommand(CreatePositionRequest Request) : IRequest<Guid>;

public class CreatePositionCommandHandler(IUnitOfWork uow) : IRequestHandler<CreatePositionCommand, Guid>
{
    public async Task<Guid> Handle(CreatePositionCommand command, CancellationToken ct)
    {
        var position = command.Request.ToEntity();

        await uow.Positions.AddAsync(position, ct);
        await uow.SaveChangesAsync(ct);
        return position.Id;
    }
}