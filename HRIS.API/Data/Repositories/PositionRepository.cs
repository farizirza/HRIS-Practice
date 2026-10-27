using HRIS.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Data.Repositories;

public class PositionRepository(HrisDbContext context) : Repository<Position>(context), IPositionRepository
{
    public async Task<bool> TitleExistsAsync(string title, CancellationToken ct = default)
        => await Context.Positions.AnyAsync(p => p.Title == title, ct);
}