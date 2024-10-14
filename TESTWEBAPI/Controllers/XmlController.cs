using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TESTWEBAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class XmlController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] KeyValueArray keyValueArray)
        {
            if (keyValueArray == null || keyValueArray.KeyValues == null)
            {
                return BadRequest("Invalid XML payload.");
            }

            // Process the data as needed
            return Ok(keyValueArray);
        }

        [HttpPost("Simple")]
        public IActionResult Post1([FromBody] IDictionary<string, object>? payload = null)
        {
            if (payload == null && HttpContext.Items.ContainsKey("XmlPayload"))
            {
                payload = HttpContext.Items["XmlPayload"] as IDictionary<string, object>;
            }

            //if (payload == null)
            //{
            //    return BadRequest("Invalid XML payload.");
            //}

            // Process the data as needed
            return Ok(payload);

        }

        [HttpPost("raw")]
        public IActionResult PostXml([FromBody] string xmlData)
        {
            if (string.IsNullOrWhiteSpace(xmlData))
            {
                return BadRequest("Invalid XML payload.");
            }

            KeyValueArray keyValueArray;
            var serializer = new XmlSerializer(typeof(KeyValueArray));

            using (var reader = new StringReader(xmlData))
            {
                keyValueArray = (KeyValueArray)serializer.Deserialize(reader);
            }

            // Process the data as needed
            return Ok(keyValueArray);
        }

        [HttpPost("Complex")]
        public IActionResult ParseXMl([FromBody] ComplexType payload)
        {
            if (payload == null || payload == null)
            {
                return BadRequest("Invalid XML payload.");
            }

            // Process the data as needed
            return Ok(payload);
        }

    }
}
