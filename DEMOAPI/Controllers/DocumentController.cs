using EmployeeApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentConversionService _conversionService;

        public DocumentController(IDocumentConversionService conversionService)
        {
            _conversionService = conversionService;
        }

        // POST api/Document/convert
        // Accepts markdown as plain text body or as an uploaded .md file
        [HttpPost("convert")]
        public IActionResult ConvertToDocx([FromBody] ConvertRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.MarkdownContent))
            {
                return BadRequest(new { message = "Markdown content is required." });
            }

            var docxBytes = _conversionService.ConvertMarkdownToDocx(request.MarkdownContent);
            var fileName = string.IsNullOrWhiteSpace(request.FileName)
                ? "document.docx"
                : $"{Path.GetFileNameWithoutExtension(request.FileName)}.docx";

            return File(docxBytes,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                fileName);
        }

        // POST api/Document/convert-file
        // Accepts an uploaded .md file
        [HttpPost("convert-file")]
        public async Task<IActionResult> ConvertFileToDocx(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "A .md file must be uploaded." });
            }

            if (!file.FileName.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = "Only .md files are supported." });
            }

            using var reader = new StreamReader(file.OpenReadStream());
            var markdownContent = await reader.ReadToEndAsync();

            var docxBytes = _conversionService.ConvertMarkdownToDocx(markdownContent);
            var fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}.docx";

            return File(docxBytes,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                fileName);
        }
    }

    public class ConvertRequest
    {
        public string? MarkdownContent { get; set; }
        public string? FileName { get; set; }
    }
}
