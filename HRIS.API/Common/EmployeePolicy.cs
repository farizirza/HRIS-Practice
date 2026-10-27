namespace HRIS.API.Common;

public class EmployeePolicy(TimeProvider timeProvider)
{
    public const int ProbationMonths = 3;
    public const int MaxFutureHireDays = 90;
    
    public DateOnly Today => DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
    
    public bool IsOnProbation(DateOnly hireDate) 
        => Today < hireDate.AddMonths(ProbationMonths);
}