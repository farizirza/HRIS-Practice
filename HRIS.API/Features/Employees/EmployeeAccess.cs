using HRIS.API.Common.Exceptions;
using HRIS.API.Common.Security;

namespace HRIS.API.Features.Employees;

// Resource-based authorization: a role cannot answer "is this record YOURS?". The check needs the loaded record.
public static class EmployeeAccess
{
    /// <summary>HR/Admin see everyone; an employee sees themself; a manager sees their direct reports.</summary>
    public static void EnsureCanView(ICurrentUser currentUser, Guid employeeId, Guid? employeeManagerId)
    {
        if (currentUser.IsHrOrAdmin)
            return;

        var me = currentUser.EmployeeId;
        if (me is not null && (me == employeeId || me == employeeManagerId))
            return;

        throw new ForbiddenAccessException("You can only view your own profile or your direct reports.");   // mapped to 403 by DomainExceptionHandler (Module 7)
    }
}