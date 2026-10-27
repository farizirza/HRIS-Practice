namespace HRIS.API.Common.Exceptions;

// One exception hierarchy for the whole course (Module 7).
// Derive from Exception through HrisException, not from ApplicationException.
// Give each exception the constructors it needs, and pass the original as innerException when you wrap a lower-level error.

/// <summary>Base class for expected errors in HRIS. Any other exception stays a plain exception and becomes a 500.</summary>
public abstract class HrisException : Exception
{
    protected HrisException() { }
    protected HrisException(string message) : base(message) { }
    protected HrisException(string message, Exception innerException) : base(message, innerException) { }

    /// <summary>HTTP status code returned to the client.</summary>
    public abstract int StatusCode { get; }

    /// <summary>Short, stable ProblemDetails title.</summary>
    public abstract string Title { get; }

    /// <summary>Machine-readable code for client applications (ProblemDetails extension "errorCode").</summary>
    public virtual string ErrorCode => "HRIS_ERROR";
}

// ---------------------------------------------------------------- Categories (one per HTTP status)

/// <summary>401: the request has no valid authenticated user.</summary>
public class UnauthorizedException : HrisException
{
    public UnauthorizedException() : base("Authentication is required.") { }
    public UnauthorizedException(string message) : base(message) { }
    public UnauthorizedException(string message, Exception innerException) : base(message, innerException) { }

    public override int StatusCode => StatusCodes.Status401Unauthorized;
    public override string Title => "Unauthorized";
    public override string ErrorCode => "UNAUTHENTICATED";
}

/// <summary>403: authenticated, but not allowed to use this resource.</summary>
public class ForbiddenAccessException : HrisException
{
    public ForbiddenAccessException() : base("You do not have access to this resource.") { }
    public ForbiddenAccessException(string message) : base(message) { }
    public ForbiddenAccessException(string message, Exception innerException) : base(message, innerException) { }

    public override int StatusCode => StatusCodes.Status403Forbidden;
    public override string Title => "Forbidden";
    public override string ErrorCode => "FORBIDDEN";
}

/// <summary>404: the resource does not exist.</summary>
public class NotFoundException : HrisException
{
    public NotFoundException() : base("The requested resource was not found.") { }
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string message, Exception innerException) : base(message, innerException) { }

    /// <summary>Convenience constructor: <c>new NotFoundException("Employee", id)</c>.</summary>
    public NotFoundException(string resource, object key) : base($"{resource} with id '{key}' was not found.")
    {
        Resource = resource;
        Key = key;
    }

    public string? Resource { get; }
    public object? Key { get; }

    public override int StatusCode => StatusCodes.Status404NotFound;
    public override string Title => "Resource not found";
    public override string ErrorCode => "NOT_FOUND";
}

/// <summary>409: the request conflicts with the current state (duplicate, already done, still in use).</summary>
public class ConflictException : HrisException
{
    public ConflictException() : base("The request conflicts with the current state of the resource.") { }
    public ConflictException(string message) : base(message) { }
    public ConflictException(string message, Exception innerException) : base(message, innerException) { }

    public override int StatusCode => StatusCodes.Status409Conflict;
    public override string Title => "Conflict";
    public override string ErrorCode => "CONFLICT";
}

/// <summary>422: the request is well-formed, but a business rule rejects it.</summary>
public class BusinessRuleException : HrisException
{
    public BusinessRuleException() : base("A business rule was violated.") { }
    public BusinessRuleException(string message) : base(message) { }
    public BusinessRuleException(string message, Exception innerException) : base(message, innerException) { }

    public override int StatusCode => StatusCodes.Status422UnprocessableEntity;
    public override string Title => "Business rule violated";
    public override string ErrorCode => "BUSINESS_RULE_VIOLATION";
}

// ---------------------------------------------------------------- Specific HRIS exceptions

/// <summary>409: employee number or work email already exists.</summary>
public class DuplicateEmployeeException : ConflictException
{
    public DuplicateEmployeeException(string message) : base(message) { }
    public DuplicateEmployeeException(string message, Exception innerException) : base(message, innerException) { }

    public override string ErrorCode => "EMPLOYEE_DUPLICATE";
}

/// <summary>409: a department with this code already exists.</summary>
public class DuplicateDepartmentException : ConflictException
{
    public DuplicateDepartmentException(string message) : base(message) { }
    public DuplicateDepartmentException(string message, Exception innerException) : base(message, innerException) { }

    public override string ErrorCode => "DEPARTMENT_DUPLICATE";
}

/// <summary>409: a position with this title already exists.</summary>
public class DuplicatePositionException : ConflictException
{
    public DuplicatePositionException(string message) : base(message) { }
    public DuplicatePositionException(string message, Exception innerException) : base(message, innerException) { }

    public override string ErrorCode => "POSITION_DUPLICATE";
}

/// <summary>409: the employee was terminated before.</summary>
public class EmployeeAlreadyTerminatedException : ConflictException
{
    public EmployeeAlreadyTerminatedException(Guid employeeId)
        : base($"Employee '{employeeId}' is already terminated.")
    {
        EmployeeId = employeeId;
    }

    public Guid EmployeeId { get; }
    public override string ErrorCode => "EMPLOYEE_ALREADY_TERMINATED";
}

/// <summary>422: a manager or position change would break the reporting line. Used by EmployeePolicy in Module 8.</summary>
public class InvalidReportingLineException : BusinessRuleException
{
    public InvalidReportingLineException(string message) : base(message) { }
    public InvalidReportingLineException(string message, Exception innerException) : base(message, innerException) { }

    public override string ErrorCode => "INVALID_REPORTING_LINE";
}