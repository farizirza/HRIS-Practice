namespace HRIS.API.Models;

public record CreatePositionRequest(
    string Title, 
    int Level);

public record PositionResponse(
    Guid Id, 
    string Title, 
    int Level);