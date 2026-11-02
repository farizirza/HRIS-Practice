using HRIS.API.Common.Exceptions;
using HRIS.API.Common.Security;
using HRIS.API.Data;
using HRIS.API.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Features.Employees.Queries.GetMyProfile;

public record GetMyProfileQuery : IRequest<EmployeeResponse>;

public class GetMyProfileQueryHandler(HrisDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetMyProfileQuery, EmployeeResponse>
{
    public async Task<EmployeeResponse> Handle(GetMyProfileQuery query, CancellationToken ct)
    {
        var employeeId = currentUser.EmployeeId ?? throw new NotFoundException("Employee", "current user");
        return await db.Employees
                   .AsNoTracking()
                   .Where(e => e.Id == employeeId)
                   .Select(e => e.ToResponse(
                       e.Department.Name,
                       e.Position.Title,
                       e.Manager != null ? e.Manager.FullName : null))
                   .FirstOrDefaultAsync(ct)
               ?? throw new NotFoundException("Employee", employeeId);
    }
}