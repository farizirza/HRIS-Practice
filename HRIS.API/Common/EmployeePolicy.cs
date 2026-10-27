namespace HRIS.API.Common;

public class EmployeePolicy(TimeProvider timeProvider)
{
    public const int ProbationMonths = 3;
    public const int MaxFutureHireDays = 90;
    
    public DateOnly Today => DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
    
    public bool IsOnProbation(DateOnly hireDate) 
        => Today < hireDate.AddMonths(ProbationMonths);
    
    public int GetTenureMonths(DateOnly hireDate)
    {
        if (Today < hireDate) return 0;
        var months = (Today.Year - hireDate.Year) * 12 + Today.Month - hireDate.Month;
        if (Today.Day < hireDate.Day) months--;
        return Math.Max(0, months);
    }
}