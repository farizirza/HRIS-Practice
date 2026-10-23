namespace HRIS.API.Services;

public class OperationReport(
    ITransientOperation transient,
    IScopedOperation scoped,
    ISingletonOperation singleton)
{
    public ITransientOperation Transient { get; } = transient;
    public IScopedOperation Scoped { get; } = scoped;
    public ISingletonOperation Singleton { get; } = singleton;
}