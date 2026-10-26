using HRIS.API.Entities;
using HRIS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Data.Repositories;

public class PositionRepository(HrisDbContext context) : Repository<Position>(context), IPositionRepository
{
    public async Task<IReadOnlyCollection<PositionResponse>> GetResponsesAsync(CancellationToken ct = default)
        => await Context.Positions
            .AsNoTracking()
            .OrderByDescending(p => p.Level)
            .ThenBy(p => p.Title)
            .Select(p => p.ToResponse())
            .ToListAsync(ct);
}