using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace QuickBooks.Data.Helper
{
    public static class Extra
    {
        public static string ParseToXmlString(this object obj)
        {
            try
            {
                XmlSerializer serializer = XmlSerializer.FromTypes(new[] { obj.GetType() })[0];
                StringWriter textWriter = new StringWriter();
                serializer.Serialize(textWriter, obj);
                return textWriter.ToString();
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public static object ParseToObject(this string xml, Type objectType)
        {
            object returnobject;
            XmlSerializer serializer = XmlSerializer.FromTypes(new Type[] { objectType })[0];
            using (TextReader reader = new StringReader(xml))
            {
                returnobject = serializer.Deserialize(reader);
            }

            return returnobject;
        }

        public static XmlDocument ParseToXml(this object obj)
        {
            XmlSerializer serializer = XmlSerializer.FromTypes(new[] { obj.GetType() })[0];
            XmlDocument xd = null;
            using (MemoryStream memStm = new MemoryStream())
            {
                serializer.Serialize(memStm, obj);

                memStm.Position = 0;

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace = true;

                using (var xtr = XmlReader.Create(memStm, settings))
                {
                    xd = new XmlDocument();
                    xd.Load(xtr);
                }
            }

            return xd;
        }

    }
}
