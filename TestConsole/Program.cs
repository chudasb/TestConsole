namespace TestConsole
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    namespace PostApiCallExample
    {
        class Program
        {
            static async Task Main(string[] args)
            {
                IDictionary<string, object> payload1 = new Dictionary<string, object>();

                // Add an array of doubles to the payload
                double[] values = { 1.0, 2.0, 3.0 };
                payload1.Add("array", values);

                // Serialize the payload to XML
                string xml = SerializePayloadToXml(payload1);

                
                string arrayaxml = @"<ArrayOfKeyValueOfstringanyType xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://schemas.microsoft.com/2003/10/Serialization/Arrays"">
  <KeyValueOfstringanyType>
    <Key>int_num</Key>
    <Value i:type=""d3p1:int"">1</Value>
  </KeyValueOfstringanyType>
  <KeyValueOfstringanyType>
    <Key>float_num</Key>
    <Value i:type=""d3p1:float[]"">
      <ArrayOfFloat>
        <float>0.33333334</float>
        <float>0.66666667</float>
        <float>1.0</float>
      </ArrayOfFloat>
    </Value>
  </KeyValueOfstringanyType>
</ArrayOfKeyValueOfstringanyType>";
             

                var result = DeserializeXmlToDictionary(arrayaxml);

                foreach (var kvp in result)
                {
                    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                }
                // Print the XML
                Console.WriteLine(xml);


                var url = "https://compliance.dev.otpp.com/edb-webapi/cdm/COMP.CDM_TRD_AUTH_MAIN.BULK_INSERT_ALL_PERSON";
                var payload = new
                {
                    SESSION_ID = 226868,
                    OTPP_PERSON_ID = new[] { -4 },
                    FIRST_NAME = new[] { "~Not Provided~" },
                    MIDDLE_NAME = new string[] { null },
                    LAST_NAME = new[] { "~Not Provided~" },
                    FULL_NAME = new[] { "~Not Provided~ ~Not Provided~" },
                    AD_ACCOUNT_ID = new string[] { null },
                    JOB_TITLE_NAME = new string[] { null },
                    DIVISION_NAME = new string[] { null },
                    EMAIL_ADDRESS = new string[] { null },
                    LOCATION_COUNTRY_CODE = new[] { "~NA~" },
                    LOCATION_COUNTRY_NAME = new[] { "~Not Applicable~" },
                    DEPARTMENT_NAME = new string[] { null },
                    SUB_DEPARTMENT_NAME = new string[] { null },
                    DATE_HIRED = new[] { "1990-12-31T00:00:00" },
                    LEAVE_FLAG = new[] { "N" },
                    CONTINGENT_WORKER_TYPE = new[] { "~Not Applicable~" },
                    MANAGER_OTPP_PERSON_ID = new[] { 0 },
                    TAXOS_GROUP = new string[] { null },
                    COST_CENTER_CODE = new[] { "~NP~" },
                    COST_CENTER_NAME = new[] { "~Not Provided~" },
                    COMPANY_NAME = new string[] { null }
                };

                using var client = new HttpClient();
                var response = await client.PostAsJsonAsync(url, payload);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Request successful.");
                }
                else
                {
                    Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                }
            }
            public static Dictionary<string, object> DeserializeXmlToDictionary(string xml)
            {
                var dictionary = new Dictionary<string, object>();
                XDocument doc = XDocument.Parse(xml);

                foreach (var keyValue in doc.Descendants(XName.Get("KeyValueOfstringanyType", "http://schemas.microsoft.com/2003/10/Serialization/Arrays")))
                {
                  
                    var key = keyValue.Element("Key")?.Value;
                    var valueElement = keyValue.Element("Value");

                    if (valueElement != null)
                    {
                        // Handle int
                        if (valueElement.Attribute(XName.Get("type", "http://www.w3.org/2001/XMLSchema-instance"))?.Value == "d3p1:int")
                        {
                            dictionary[key] = int.Parse(valueElement.Value);
                        }
                        // Handle float array
                        else if (valueElement.Attribute(XName.Get("{http://www.w3.org/2001/XMLSchema-instance}type"))?.Value == "d3p1:float[]")
                        {
                            var floatArray = new List<float>();
                            foreach (var floatElement in valueElement.Element("ArrayOfFloat").Elements("float"))
                            {
                                floatArray.Add(float.Parse(floatElement.Value));
                            }
                            dictionary[key] = floatArray;
                        }
                    }
                }

                return dictionary;
               
            }
            public static string SerializePayloadToXml(IDictionary<string, object> payload)
            {
                var knownTypes = new List<Type> { typeof(double[]) };
                DataContractSerializer serializer = new DataContractSerializer(payload.GetType(), knownTypes);

                // Create a StringWriter to hold the XML output
                using (StringWriter stringWriter = new StringWriter())
                using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter))
                {
                    // Serialize the payload to XML
                    serializer.WriteObject(xmlWriter, payload);
                    xmlWriter.Flush();

                    // Return the XML as a string
                    return stringWriter.ToString();
                }
            }
        }
    }
}
