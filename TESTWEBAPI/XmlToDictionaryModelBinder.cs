using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

public class XmlToDictionaryModelBinder : IModelBinder
{
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        var request = bindingContext.HttpContext.Request;
        if (!request.ContentType.StartsWith("application/xml", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        using (var reader = new StreamReader(request.Body))
        {
            var xmlString = await reader.ReadToEndAsync();
            var doc = XDocument.Parse(xmlString);
            var dictionary = new Dictionary<string, object>();
            foreach (var element in doc.Descendants("{http://schemas.microsoft.com/2003/10/Serialization/Arrays}KeyValueOfstringanyType"))
            {
                var key = element.Element("{http://schemas.microsoft.com/2003/10/Serialization/Arrays}Key")?.Value;
                var valueElement = element.Element("{http://schemas.microsoft.com/2003/10/Serialization/Arrays}Value");
                var type = valueElement?.Attribute(XName.Get("type", "http://www.w3.org/2001/XMLSchema-instance"))?.Value;

                if (key != null && valueElement != null)
                {
                    dictionary[key] = ParseXmlValue(valueElement);
                }
            }
            Console.WriteLine(JsonConvert.SerializeObject(dictionary));
            //foreach (var element in xml.Root.Elements("KeyValueOfstringanyType"))
            //{
            //    var key = element.Element("Key").Value;
            //    var valueElement = element.Element("Value");
            //    var value = ParseXmlValue(valueElement);
            //    dictionary.Add(key, value);
            //}

            bindingContext.Result = ModelBindingResult.Success(dictionary);
        }
    }

    private object ParseXmlValue(XElement valueElement)
    {
        var typeAttribute = valueElement.Attribute(XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance") + "type");
        if (typeAttribute != null)
        {
            var type = typeAttribute.Value.Split(':').Last(); // Get the type without the namespace prefix
            switch (type)
            {
                case "int":
                    return int.Parse(valueElement.Value);
                case "float":
                    return float.Parse(valueElement.Value);
                case "float[]":
                    
                    //var type = typeAttribute.Value;
                    //switch (type)
                    //{
                    //    case "d3p1:int":
                    //        return int.Parse(valueElement.Value);
                    //    case "d3p1:float":
                    //        return float.Parse(valueElement.Value);
                    //    case "d3p1:float[]":

                    return  valueElement.Descendants().Where(e => e.Name.LocalName == "float").Select(e => float.Parse(e.Value)).ToArray();
                    
            }
        }
        return valueElement.Value;
    }
}
