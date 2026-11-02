using HRIS.API.Common.Exceptions;
using HRIS.API.Common.Security;
using HRIS.API.Data;
using HRIS.API.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Features.Employees.Queries.GetDirectReports;

public record GetDirectReportsQuery(Guid ManagerId) : IRequest<IReadOnlyCollection<EmployeeResponse>>;

public class GetDirectReportsQueryHandler(HrisDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetDirectReportsQuery, IReadOnlyCollection<EmployeeResponse>>
{
    public async Task<IReadOnlyCollection<EmployeeResponse>> Handle(GetDirectReportsQuery query, CancellationToken ct)
    {
        // Ownership check: Hanya HR/Admin ATAU manager yang bersangkutan yang boleh melihat direct reports miliknya
        if (!currentUser.IsHrOrAdmin && currentUser.EmployeeId != query.ManagerId)
        {
            throw new ForbiddenAccessException("You can only view your own direct reports.");
        }

        var managerExists = await db.Employees.AnyAsync(e => e.Id == query.ManagerId, ct);
        if (!managerExists)
        {
            throw new NotFoundException("Employee", query.ManagerId);
        }

        var directReports = await db.Employees.AsNoTracking()
            .Where(e => e.ManagerId == query.ManagerId)
            .OrderBy(e => e.FullName)
            .Select(e => e.ToResponse(e.Department.Name, e.Position.Title, e.Manager != null ? e.Manager.FullName : null))
            .ToListAsync(ct);

        return directReports;
    }
}
