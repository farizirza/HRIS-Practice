namespace HRIS.API.Common.Security;

public static class Policies
{
    public const string HrOnly = "HrOnly";                          // Admin or HR (role-based rule, written as a named policy)
    public const string HasEmployeeProfile = "HasEmployeeProfile"; // claim-based rule: the account is linked to an employee record
}

public static class HrisClaimTypes
{
    public const string EmployeeId = "employee_id";
}