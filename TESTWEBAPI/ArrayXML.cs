using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization;
using System.Xml.Serialization;

[XmlRoot("ArrayOfKeyValueOfstringanyType")]
public class KeyValueArray
{
    [XmlElement("KeyValueOfstringanyType")]
    public List<KeyValue> KeyValues { get; set; }
}

public class KeyValue
{
    [XmlElement("Key")]
    public string Key { get; set; }

    [XmlElement("Value")]
    public Value Value { get; set; }
}

public class Value
{
    [XmlElement("int", IsNullable = true)]
    public int? IntValue { get; set; }

    [XmlArray("ArrayOfFloat")]
    [XmlArrayItem("float")]
    public List<float> FloatArray { get; set; }
}

[DataContract]
[KnownType(typeof(float))]
[KnownType(typeof(Dictionary<string,object>))]
[KnownType(typeof(string))]
[KnownType(typeof(object[]))]
public class ComplexType
{
    [DataMember]
    public Dictionary<string,object> Data { get; set; }

}

