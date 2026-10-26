using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.API.Controllers;

public record AnnotatedRequest(
    [Required, StringLength(100)] string FullName,
    [Range(1, 10)] int Level);

[ApiController]
[Route("api/annotation-lab")]
public class AnnotationLabController : ControllerBase
{
    [HttpPost]
    public IActionResult Post(AnnotatedRequest request) => Ok(request);
}