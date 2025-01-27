using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CRUtils
{
    public static class CRUtils
    {
        public static void CreateXsdFile(DataSet ds, string path, string name)
        {
            string nameFile = name.EndsWith("xsd") ? name : $"{name}.xsd";
            var dir = System.IO.Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~" +path), nameFile);
            using (var sw = new System.IO.StreamWriter(dir, false))
            {
                ds.WriteXmlSchema(sw);
            }
        }

        public static string XmlFromDataset(DataSet ds, bool removeCharset = true)
        {
            XmlDocument docSchema = new XmlDocument();
            docSchema.LoadXml(ds.GetXmlSchema());

            if (removeCharset)
            {
                var declarations = docSchema.ChildNodes.OfType<XmlNode>()
                              .Where(x => x.NodeType == XmlNodeType.XmlDeclaration)
                              .ToList();
                declarations.ForEach(x => docSchema.RemoveChild(x));
            }

            return docSchema.OuterXml + ds.GetXml();
        }

        public static DataSet DataSetFromXml(string xml)
        {
            try
            {
                System.IO.StringReader xmlSR = new System.IO.StringReader(xml);
                DataSet dt = new DataSet();
                dt.ReadXml(xmlSR);
                return dt;
            }
            catch (Exception e)
            {
                throw new Exception("Impossibile leggere il DataSet");
            }
        }

      
    }
}
