using HRIS.API.Entities;

namespace HRIS.API.Models;

public static class PositionMappings
{
    public static PositionResponse ToResponse(this Position position) => 
        new(position.Id, 
            position.Title, 
            position.Level);
    
    public static Position ToEntity(this CreatePositionRequest request) => 
        new(Guid.CreateVersion7(), 
            request.Title.Trim(), 
            request.Level);
}