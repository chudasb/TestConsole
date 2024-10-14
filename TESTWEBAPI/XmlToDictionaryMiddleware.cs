using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

public class XmlToDictionaryMiddleware
{
    private readonly RequestDelegate _next;

    public XmlToDictionaryMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.ContentType != null && context.Request.ContentType.StartsWith("application/xml"))
        {
            var j =  new
            {
                SESSION_ID = 1,
                Persion_id_array = new double?[] { 0, 1, -1, 0.1, 1.1, null }
            };
            string js = JsonConvert.SerializeObject(j);
           
            context.Request.EnableBuffering();
            using (var reader = new StreamReader(context.Request.Body, leaveOpen: true))
            {
                var xmlData = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                var dictionary = new Dictionary<string, object>();
                var doc = XDocument.Parse(xmlData);
               // RemoveAllNamespaces(doc);
              //  var json = JsonConvert.SerializeXNode(doc, Formatting.Indented, omitRootObject: true);

                foreach (var element in doc.Descendants("{http://schemas.microsoft.com/2003/10/Serialization/Arrays}KeyValueOfstringanyType"))
                {
                    var key = element.Element("{http://schemas.microsoft.com/2003/10/Serialization/Arrays}Key")?.Value;
                    var valueElement = element.Element("{http://schemas.microsoft.com/2003/10/Serialization/Arrays}Value");
                    var type = valueElement?.Attribute(XName.Get("type", "http://www.w3.org/2001/XMLSchema-instance"))?.Value;

                    if (key != null && valueElement != null)
                    {
                        dictionary[key] = ParseValueElement(valueElement, type);
                    }
                }
                Console.WriteLine(JsonConvert.SerializeObject(dictionary));
                // Convert the dictionary back to XML
                //var newDoc = new XDocument(
                //    new XElement("Root",
                //        dictionary.Select(kv =>
                //            new XElement("KeyValueOfstringanyType",
                //                new XElement("Key", kv.Key),
                //                new XElement("Value", kv.Value)
                //            )
                //        )
                //    )
                //);

                //var xmlBytes = Encoding.UTF8.GetBytes(newDoc.ToString());
                //// Replace the request body with the new XML content
                //context.Request.Body = new MemoryStream(xmlBytes);
                //context.Request.ContentType = "application/xml";
                //context.Request.ContentLength = xmlBytes.Length;
                ///============
                // Convert the dictionary to JSON
                //var json = JsonConvert.SerializeObject(dictionary);
                //var jsonBytes = Encoding.UTF8.GetBytes(json);

                //context.Request.Body =  new MemoryStream(jsonBytes);
                //// Replace the request body with the new JSON content
                //context.Request.Body = new MemoryStream(jsonBytes);
                //context.Request.ContentType = "application/json";
                //context.Request.ContentLength = jsonBytes.Length;

                context.Items["XmlPayload"] = dictionary;
            }
        }

        await _next(context);
    }

    private object ParseValueElement(XElement valueElement, string type)
    {
        switch (type)
        {
            case "d3p1:int":
                return int.Parse(valueElement.Value);
            case "d3p1:float":
                return float.Parse(valueElement.Value);
            case "d3p1:double":
                return double.Parse(valueElement.Value);
            case "d3p1:decimal":
                return decimal.Parse(valueElement.Value);
            case "d3p1:string":
                return valueElement.Value;
            case "d3p1:dateTime":
                return DateTime.Parse(valueElement.Value);
            case "d3p1:float[]":
                var aa = valueElement.Descendants().Where(e => e.Name.LocalName == "float").Select(e => float.Parse(e.Value));

                return valueElement.Descendants().Where(e => e.Name.LocalName == "float").Select(e => float.Parse(e.Value)).ToArray();
                var arrayOfFloatElement = valueElement.Element("{http://schemas.microsoft.com/2003/10/Serialization/Arrays}ArrayOfFloat");
                if (arrayOfFloatElement != null)
                {
                     arrayOfFloatElement.Elements("{http://schemas.microsoft.com/2003/10/Serialization/Arrays}float")
                                              .Select(e => float.Parse(e.Value))
                                              .ToArray();
                    
                    valueElement.Descendants("float").Select(e => float.Parse(e.Value)).ToArray();
                }
                return arrayOfFloatElement;               // return valueElement.Elements().Select(e => (e.Value)).ToArray();

            //return valueElement.Descendants("float").Select(e => float.Parse(e.Value)).ToArray();
            default:
                return valueElement.Value;
        }
    }

    private void RemoveAllNamespaces(XDocument doc)
    {
        foreach (var element in doc.Descendants())
        {
            element.Name = element.Name.LocalName;
            element.ReplaceAttributes(element.Attributes().Where(attr => !attr.IsNamespaceDeclaration));
        }
    }
}
