using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

public class XmlRequestMiddleware
{
    private readonly RequestDelegate _next;

    public XmlRequestMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.ContentType == "application/xml")
        {
            // Custom logic for XML requests
            context.Request.EnableBuffering();
            var buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];
            await context.Request.Body.ReadAsync(buffer, 0, buffer.Length);
            var requestBody = System.Text.Encoding.UTF8.GetString(buffer);
            context.Request.Body.Position = 0;

            // Parse XML
            var xml = XDocument.Parse(requestBody);
            // Perform custom logic with the XML data
            // For example, log the XML content
            System.Console.WriteLine(xml.ToString());
        }

        // Call the next middleware in the pipeline
        await _next(context);
    }
}
