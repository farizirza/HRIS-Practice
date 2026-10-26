using HRIS.API.Data;
using HRIS.API.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Features.Positions.Queries.GetPositions;

public record GetPositionsQuery : IRequest<IReadOnlyCollection<PositionResponse>>;

public class GetPositionsQueryHandler(HrisDbContext db)
    : IRequestHandler<GetPositionsQuery, IReadOnlyCollection<PositionResponse>>
{
    public async Task<IReadOnlyCollection<PositionResponse>> Handle(GetPositionsQuery query, CancellationToken ct)
        => await db.Positions
            .AsNoTracking()
            .OrderByDescending(p => p.Level)
            .ThenBy(p => p.Title)
            .Select(p => p.ToResponse())
            .ToListAsync(ct);
}