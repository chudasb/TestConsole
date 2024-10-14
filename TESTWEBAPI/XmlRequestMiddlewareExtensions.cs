using Microsoft.AspNetCore.Builder;

public static class XmlRequestMiddlewareExtensions
{
    public static IApplicationBuilder UseXmlRequestMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<XmlRequestMiddleware>();
    }
}
