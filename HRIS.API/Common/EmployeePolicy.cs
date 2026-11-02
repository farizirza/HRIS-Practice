using HRIS.API.Common.Exceptions;
using HRIS.API.Entities;

namespace HRIS.API.Common;

public class EmployeePolicy(TimeProvider timeProvider)
{
    public const int ProbationMonths = 3;
    public const int MaxFutureHireDays = 90;

    public DateOnly Today => DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

    /// <summary>An employee is on probation for the first 3 months after the hire date.</summary>
    public bool IsOnProbation(DateOnly hireDate) => Today < hireDate.AddMonths(ProbationMonths);

    /// <summary>Completed months of service (0 before the hire date).</summary>
    public int GetTenureMonths(DateOnly hireDate)
    {
        if (Today < hireDate) return 0;
        var months = (Today.Year - hireDate.Year) * 12 + Today.Month - hireDate.Month;
        if (Today.Day < hireDate.Day) months--;   // the current month is not completed yet
        return months;
    }

    /// <summary>Hire dates may be planned ahead, but not more than 90 days.</summary>
    public bool IsValidHireDate(DateOnly hireDate) => hireDate <= Today.AddDays(MaxFutureHireDays);

    /// <summary>Rules for "manager can manage employee"; throws InvalidReportingLineException (422).</summary>
    public void EnsureCanManage(Employee manager, int managerLevel, Employee employee, int employeeLevel)
    {
        if (manager.Id == employee.Id)
            throw new InvalidReportingLineException("An employee cannot be their own manager.");
        if (manager.Status != EmployeeStatus.Active)
            throw new InvalidReportingLineException("A terminated employee cannot be assigned as a manager.");
        if (IsOnProbation(manager.HireDate))
            throw new InvalidReportingLineException("An employee on probation cannot be assigned as a manager.");
        if (managerLevel <= employeeLevel)
            throw new InvalidReportingLineException("The manager's position level must be higher than the employee's.");
    }
}
