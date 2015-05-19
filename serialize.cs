using System;
using System.IO;
using System.Xml.Serialization;

class serialize
{
    public static Object DeserializeObject(String serializedstring, Type objecttype)
    {
        Object obj;
        XmlSerializer xmlSerializer = new XmlSerializer(objecttype);
        using (StringReader stringReader = new StringReader(serializedstring))
        {
            obj = xmlSerializer.Deserialize(stringReader);
        }
        return obj;
    }

    public static String SerializeObject(Object o)
    {
        String str;
        XmlSerializer xmlSerializer = new XmlSerializer(o.GetType());
        using (StringWriter stringWriter = new StringWriter())
        {
            try
            {
                xmlSerializer.Serialize(stringWriter, o);
                str = stringWriter.ToString();
            }
            catch (Exception exception)
            {
                str = null;
            }
        }
        return str;
    }
}