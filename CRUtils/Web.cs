using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CRUtils
{
    public static class Web
    {
        public static async Task<XmlDocument> ReadRequestXmlAsync(HttpRequestMessage message)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                var body = await message.Content.ReadAsStringAsync();
                doc.LoadXml(body);
            }
            catch (Exception e)
            {
                throw new Exception("Formato Request non valida!");
            }

            return doc;
        }
    }
}
