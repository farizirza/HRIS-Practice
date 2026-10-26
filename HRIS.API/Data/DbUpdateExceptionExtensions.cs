// Data/DbUpdateExceptionExtensions.cs
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Data;

// Module 7: a handler may only "translate" a database error it understands.
// Everything else must keep travelling up (unchanged) to GlobalExceptionHandler.
public static class DbUpdateExceptionExtensions
{
    // SQL Server error numbers: 2601 = duplicate key in a unique index, 2627 = unique / primary key constraint.
    private const int UniqueIndexViolation = 2601;
    private const int UniqueConstraintViolation = 2627;

    /// <summary>True when the save failed because a unique index rejected the row (a race between two requests).</summary>
    public static bool IsUniqueConstraintViolation(this DbUpdateException exception)
        => exception.InnerException is SqlException { Number: UniqueIndexViolation or UniqueConstraintViolation };
}