namespace HRIS.API.Services;

public interface IOperation
{
    Guid OperationId { get; set; }
}

public interface ITransientOperation : IOperation {}
public interface IScopedOperation : IOperation {}
public interface ISingletonOperation : IOperation {}

public class Operation : ITransientOperation, IScopedOperation, ISingletonOperation
{
    public Guid OperationId { get; set; } = Guid.NewGuid();
}