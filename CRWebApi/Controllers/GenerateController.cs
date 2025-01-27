using CRUtils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.UI.WebControls;
using System.Xml;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace CRWebApi.Controllers
{
    [RoutePrefix("api/Generate")]
    public class GenerateController : ApiController
    {
        // GET: Generate
        [HttpPost]
        public async Task<HttpResponseMessage> PDF(
            HttpRequestMessage message)
        {
            try
            {
                //Read Content
                var doc = await CRUtils.Web.ReadRequestXmlAsync(message);

                //Check Error
                CheckError(doc);

                //Read Dataset
                var dataset = doc.GetElementsByTagName("ds");
                var dt = CRUtils.CRUtils.DataSetFromXml(dataset.Item(0).OuterXml);

                //Print With CR
                var modello = doc.GetElementsByTagName("modello")?.Item(0)?.InnerText ?? "";
                var returnType = doc.GetElementsByTagName("returnType")?.Item(0)?.InnerText ?? "";


                //Return PDF // Excel // Ecc
                Tuple<ExportFormatType, string> returnedType = GetReturnedType(returnType);

                var rd = new ReportDocument();
                //TODO modificare da dove caricare i file o creare regole differenti rispetto l'url
                rd.Load(Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/public/Reports"), $"{modello}"));
                rd.SetDataSource(dt);

                var result = new HttpResponseMessage(HttpStatusCode.OK);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (var stream = rd.ExportToStream(returnedType.Item1))
                    {
                        stream.CopyTo(ms);
                    }

                    result.Content = new ByteArrayContent(ms.ToArray());
                }

                result.Content.Headers.ContentDisposition =
                    new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = $"pdf.{returnedType.Item2}"
                    };
                result.Content.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue(returnedType.Item2);

                return result;
            }
            catch (Exception e)
            {
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.BadRequest, Content = new StringContent(e.Message, Encoding.UTF8, "application/xml") };
            }

        }

        private static Tuple<ExportFormatType, string> GetReturnedType(string returnType)
        {
            Tuple<ExportFormatType, string> returnedType;
            switch (returnType)
            {
                case "pdf":
                    returnedType = new Tuple<ExportFormatType, string>(ExportFormatType.PortableDocFormat, "application/pdf");
                    break;
                case "xls":
                    returnedType =
                        new Tuple<ExportFormatType, string>(ExportFormatType.ExcelWorkbook, $"application/vnd.ms-excel");
                    //application/vnd.ms-excel (xls)
                    //application/vnd.openxmlformats-officedocument.spreadsheetml.sheet (xlsx)
                    break;
                //case "xlsx":
                //break;
                case "html":
                    returnedType =
                        new Tuple<ExportFormatType, string>(ExportFormatType.HTML40, $"text/html");
                    break;
                case "doc":
                    returnedType =
                        new Tuple<ExportFormatType, string>(ExportFormatType.WordForWindows, $"application/msword");
                    break;
                case "rtf":
                    returnedType = new Tuple<ExportFormatType, string>(ExportFormatType.RichText, "application/rtf");
                    break;
                default:
                    returnedType = new Tuple<ExportFormatType, string>(ExportFormatType.CrystalReport, "application/x-rpt");
                    break;
            }

            return returnedType;
        }


        private void CheckError(XmlDocument doc)
        {
            var dataset = doc.GetElementsByTagName("ds");
            var modello = doc.GetElementsByTagName("modello")?.Item(0)?.InnerText ?? "";
            var returnType = doc.GetElementsByTagName("returnType")?.Item(0)?.InnerText ?? "";
            var outputName = doc.GetElementsByTagName("outputName")?.Item(0)?.InnerText ?? "";

            string error = "";
            if (dataset.Count == 0)
            {
                error += "il nodo 'ds' non è presente nel modello!\n";
            }


            if (string.IsNullOrEmpty(modello))
            {
                error += "'modello'  non valido nel modello!";
            }

            if (string.IsNullOrEmpty(returnType))
            {
                error += "'returnType' non valido nel modello!";
            }

            if (string.IsNullOrEmpty(outputName))
            {
                error += "'outputName' non valido nel modello!";
            }

            if (!string.IsNullOrEmpty(error)) { throw new Exception(error); }
        }


        //TEST PER LA GENERAZIONE DI UN MODELLO DI TEST 
        [HttpGet]
        public HttpResponseMessage GetDatasetSchema()
        {
            //DsName 
            string dataSetName = $"dataSetTest_{DateTime.Now:yyyyMMddHHmm}";
            //Temp Table
            DataSet ds = new DataSet() {DataSetName = dataSetName };
            var dt = new DataTable();
            dt.Columns.Add("nome");
            dt.Columns.Add("cognome");
            for (int i = 0; i < 10; i++)
            {
                var row = dt.NewRow();
                row[0] = "a";
                row[1] = $"b{i}";
                dt.Rows.Add(row);
            }

            ds.Tables.Add(dt);

            //Create Model
            XmlDocument doc = new XmlDocument();
            XmlNode newElem = doc.CreateNode("element", "ModelGenerate", "");

            //CHILD
            //modello
            XmlNode modello = doc.CreateNode("element", "modello", "");
            modello.InnerText = "CrystalReport1.rpt";
            newElem.AppendChild(modello);

            //returnType
            XmlNode returnType = doc.CreateNode("element", "returnType", "");
            returnType.InnerText = "pdf";
            newElem.AppendChild(returnType);

            //returnType
            XmlNode outputName = doc.CreateNode("element", "outputName", "");
            outputName.InnerText = "pdfFile";
            newElem.AppendChild(outputName);


            //Schema
            var docSchema = CRUtils.CRUtils.XmlFromDataset(ds);
            XmlNode dsDataEle = doc.CreateNode("element", "ds", "");
            dsDataEle.InnerXml = docSchema;
            newElem.AppendChild(dsDataEle);

            doc.AppendChild(newElem);

            return new HttpResponseMessage() { Content = new StringContent(string.Concat(doc.OuterXml), Encoding.UTF8, "application/xml") };
        }

        [Route("xsd")]
        [HttpGet]
        public HttpResponseMessage CreateXsd()
        {
            //DataSetName 
            string dataSetName = $"dataSetTest_{DateTime.Now:yyyyMMddHHmm}";
            //Temp Table
            DataSet ds = new DataSet() { DataSetName = dataSetName };
            var dt = new DataTable();
            dt.Columns.Add("nome");
            dt.Columns.Add("cognome");
            for (int i = 0; i < 10; i++)
            {
                var row = dt.NewRow();
                row[0] = "a";
                row[1] = $"b{i}";
                dt.Rows.Add(row);
            }
            ds.Tables.Add(dt);

            CRUtils.CRUtils.CreateXsdFile(ds, "/public/DataSet", dataSetName);

            return new HttpResponseMessage() { Content = new StringContent("", Encoding.UTF8, "application/xml") };
        }


    }

}