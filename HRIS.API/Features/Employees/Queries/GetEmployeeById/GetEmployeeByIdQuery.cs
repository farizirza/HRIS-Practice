using HRIS.API.Common.Exceptions;
using HRIS.API.Common.Security;
using HRIS.API.Data;
using HRIS.API.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Features.Employees.Queries.GetEmployeeById;

public record GetEmployeeByIdQuery(Guid Id) : IRequest<EmployeeResponse>;

public class GetEmployeeByIdQueryHandler(HrisDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetEmployeeByIdQuery, EmployeeResponse>
{
    public async Task<EmployeeResponse> Handle(GetEmployeeByIdQuery query, CancellationToken ct)
    {
        var row = await db.Employees.AsNoTracking()
                      .Where(e => e.Id == query.Id)
                      .Select(e => new
                      {
                          Response = e.ToResponse(e.Department.Name, e.Position.Title, e.Manager != null ? e.Manager.FullName : null),
                          // ToResponse (Module 4) builds the response; ManagerId travels beside it for the ownership check
                          e.ManagerId
                      })
                      .FirstOrDefaultAsync(ct)
                  ?? throw new NotFoundException("Employee", query.Id);

        EmployeeAccess.EnsureCanView(currentUser, row.Response.Id, row.ManagerId);   // AFTER loading: we need the owner of the record
        return row.Response;
    }
}