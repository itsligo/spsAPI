using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using spsServerAPI.Models;
using System.Web.Http.Cors;
using System.Threading.Tasks;
using System.IO;
using Excel;
using System.Web;

namespace spsServerAPI.Controllers
{
    [RoutePrefix("api/Import")]
    [AllowAnonymous]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class importController : ApiController
    {
        Model db = new Model();

       [Route("excel")]
       [HttpPost]
       public async Task<IEnumerable<string>> Post()
        {
           // must be uploaded as a multi-part form with Header
            // Content-Disposition application/vnd.ms-excel
           // form Data
           // size original
           // Content File FileName

            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Invalid Request!"));
            }

            string fullPath = HttpContext.Current.Server.MapPath("~/Uploaded");
            var streamProvider = new CustomMultipartFormDataStreamProvider(fullPath);

            try
            {
                await Request.Content.ReadAsMultipartAsync(streamProvider);
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            var fileInfo = streamProvider.FileData.Select(i =>
            {
                var info = new FileInfo(i.LocalFileName);
                importFile(info);
                return "Uploaded " + info.FullName + " (" + info.Length + ")";
            });
            
            return fileInfo;
        }

       public void importFile(FileInfo toUpload)
       {
           FileStream stream = File.Open(toUpload.FullName, FileMode.Open, FileAccess.Read);
          // //using ExcelDataReader to open .xlsx file
           //Stream stream = toUpload.OpenRead();
           if (stream != null)
           {
               IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
               excelReader.IsFirstRowAsColumnNames = true;
               DataSet result = excelReader.AsDataSet();
               DataRowCollection rows = result.Tables[0].Rows;
               foreach (DataRow row in rows)
               {
                   db.PlacementProviders.Add(
                       new PlacementProvider
                       {
                           City = row.ItemArray[4].ToString(),
                           County = row.ItemArray[5].ToString(),
                           ProviderName = row.ItemArray[6].ToString(),
                           //Supervisor = row.ItemArray[7].ToString(),
                           //SupervisorContactNumber = row.ItemArray[8].ToString(),
                       });
               }
           }
       }
    }

    public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        public CustomMultipartFormDataStreamProvider(string path) : base(path) { }

        public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
        {
            string fileName;
            if (!string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName))
            {
                var ext = Path.GetExtension(headers.ContentDisposition.FileName.Replace("\"", string.Empty));
                fileName = "Uploaded" + ext;
            }
            else
            {
                fileName = "Uploaded" + ".xslx";
            }
            return fileName;
        }
    }

}