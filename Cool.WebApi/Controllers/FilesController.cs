using Cool.Application.Dtos.Files;
using Cool.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cool.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class FilesController : ControllerBase
{
    private readonly IFileService _fileService;

    public FilesController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost]
    public async Task<IActionResult> AddNewFile(CreateFileModel newFileModel)
    {
        bool result = await _fileService.AddFile(newFileModel);
        return result ? Ok() : BadRequest();
    }
}
