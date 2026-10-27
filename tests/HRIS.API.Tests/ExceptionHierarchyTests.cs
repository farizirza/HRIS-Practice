using HRIS.API.Common.Exceptions;

namespace HRIS.API.Tests;

public class ExceptionHierarchyTests
{
    [Fact]
    public void DuplicateEmployeeException_IsAConflict_WithStatus409AndItsOwnErrorCode()
    {
        // Arrange + Act
        var exception = new DuplicateEmployeeException("Employee number 'EMP-0001' is already used.");

        // Assert
        Assert.IsAssignableFrom<ConflictException>(exception);
        Assert.Equal(409, exception.StatusCode);
        Assert.Equal("EMPLOYEE_DUPLICATE", exception.ErrorCode);
    }
}