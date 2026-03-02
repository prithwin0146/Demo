namespace EmployeeApi.Services
{
    public interface IDocumentConversionService
    {
        byte[] ConvertMarkdownToDocx(string markdownContent);
    }
}
