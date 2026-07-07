using ClosedXML.Excel;
using FlexicodeConnectors.Helper;
using FlexicodeConnectors.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DataTable = System.Data.DataTable;


namespace FlexicodeConnectors.Controllers
{
    public class PipelineController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        // Specify the folder path within your project where you want to store the file.
        //string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads") ;

        string folderPath = "";
        string macroFolderPath = "";
        string hostingfor = "";
        string _slash = "";

        public PipelineController(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            hostingfor = _configuration.GetConnectionString("HostingFor");
            if (hostingfor.Trim() == "Linux")
            {
                _slash = "/";
            }
            else
            {
                _slash = "\\";
            }

            folderPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads") + _slash;//_configuration.GetConnectionString("Uploadpath"); //WINDOWS Server
            macroFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "macros") + _slash;//_configuration.GetConnectionString("Uploadpath"); //WINDOWS Server
            ConnectorApi _api = new ConnectorApi(configuration);

        }

        //byte[] fileBytes;
        public IActionResult PipelineList()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PipelineNew(int id)

        {
            //folderPath = _configuration.GetConnectionString("Uploadpath");

            //Connector Drop Down Binding
            List<ConnectionDto> lovconn = new List<ConnectionDto>();
            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Pipeline/GetConnectorsForDropdown");
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                lovconn = JsonConvert.DeserializeObject<List<ConnectionDto>>(results);
            }
            ViewBag.Connectors = lovconn;
            ViewBag.pipeline_id = id;
            ViewBag.fileuploaddir = folderPath;

            //Label Data Info Binding
            List<PipelineModel> objpplinfo = new List<PipelineModel>();

            using (client = _api.Initial())
            {
                res = await client.GetAsync($"Pipeline/Getpipelineinfo?pipelinegid={id}");
                if (res.IsSuccessStatusCode)
                {
                    results = await res.Content.ReadAsStringAsync(); // Use async method
                    objpplinfo = JsonConvert.DeserializeObject<List<PipelineModel>>(results);
                }
            }

            if (objpplinfo.Count > 0)
            {
                // Set ViewData properties based on the retrieved data.
                ViewData["infopplname"] = objpplinfo[0].pipeline_name;
                ViewData["infoconname"] = objpplinfo[0].connection_name;
                ViewData["infosrctbl"] = objpplinfo[0].table_view_query_desc;
            }

            Connection masterData = new Connection();
            // var results = "";
            client = new HttpClient();
            client = _api.Initial();
            HttpResponseMessage res1 = await client.GetAsync($"DataProcessings/GetMasterDatawithCode?parentCode={"DAP"}");
            var connectionModel = new Connection();
            if (res1.IsSuccessStatusCode)
            {
                results = res1.Content.ReadAsStringAsync().Result;
                //masterData.masterDatas = JsonConvert.DeserializeObject<List<Masters>>(results);
                connectionModel = new Connection
                {
                    masterDatas = JsonConvert.DeserializeObject<List<Masters>>(results)
                        .Select(c => new SelectListItem
                        {
                            Value = c.master_code.ToString(),
                            Text = c.master_name
                        })
                        .ToList()
                };

            }
            return View(connectionModel);
        }
        [HttpGet]
        public async Task<JsonResult> Readpipeline(int id)
        {
            //folderPath = _configuration.GetConnectionString("Uploadpath");

            PipelineModel objpipeline = new PipelineModel();
            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Pipeline/GetPipeline?id=" + id);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                objpipeline = JsonConvert.DeserializeObject<PipelineModel>(results);
                objpipeline.file_path = folderPath;
                string ext = "";
                if (objpipeline.source_file_name != "" && objpipeline.source_file_name != null)
                {
                    string[] parts = objpipeline.source_file_name.Split('.');
                    ext = "." + parts.Last();

                }

                HttpContext.Session.SetString("session_pplcodefilename", objpipeline.pipeline_code + ext);
            }

            return Json(objpipeline);
        }

        [HttpGet]
        public async Task<JsonResult> GetsrcDbType(string Connection_code)
        {
            Connection objres = new Connection();
            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Pipeline/GetSourcedbType?connection_code=" + Connection_code);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                objres = JsonConvert.DeserializeObject<Connection>(results);
            }

            return Json(objres);
        }

        [HttpGet]
        public async Task<JsonResult> GellAllPipelines(string run_type, string pipeline_status)
        {
            try
            {
                List<PipelineModel> pipelines = new List<PipelineModel>();
                var results = "";
                ConnectorApi _api = new ConnectorApi(_configuration);
                HttpClient client = _api.Initial();
                HttpResponseMessage res = await client.GetAsync("Pipeline/GetPipelines?runtype=" + run_type + "&pipeline_status=" + pipeline_status);
                if (res.IsSuccessStatusCode)
                {
                    results = res.Content.ReadAsStringAsync().Result;
                    pipelines = (List<PipelineModel>)JsonConvert.DeserializeObject<List<PipelineModel>>(results);
                }
                return Json(pipelines);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpGet]
        public async Task<JsonResult> Getconndblist(string connection_code)
        {
            List<DatabaseInfo> dbinfo = new List<DatabaseInfo>();

            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Pipeline/GetDatabaseNames?connection_code=" + connection_code);
            HttpContext.Session.SetString("session_sheetname", "");
            HttpContext.Session.SetString("session_filename", "");

            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                dbinfo = JsonConvert.DeserializeObject<List<DatabaseInfo>>(results);
            }
            return Json(dbinfo);
        }

        [HttpPost]
        public async Task<JsonResult> UploadFile(string pipeline_code, IFormFile file)
        {
            //folderPath = _configuration.GetConnectionString("Uploadpath");

            string errormsg = "";
            var filePath = "";
            try
            {
                if (file != null && file.Length > 0)
                {
                    errormsg = folderPath;

                    //Trace Error
                    //var errorLog = new ErrorLog
                    //{
                    //    in_errorlog_pipeline_code = pipeline_code,
                    //    in_errorlog_scheduler_gid = 0,
                    //    in_errorlog_type = "folderPath - Method Name : UploadFile",
                    //    in_errorlog_exception = errormsg,
                    //    in_created_by = ""
                    //};
                    //IsErrorlog(errorLog);

                    // Create the folder if it doesn't exist.
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string[] parts = file.FileName.Split('.');
                    string extension = "." + parts.Last();
                    filePath = folderPath + file.FileName;
                    string dummy_file_name = pipeline_code + extension;

                    HttpContext.Session.SetString("session_folderpath", folderPath);
                    HttpContext.Session.SetString("session_filename", file.FileName);
                    HttpContext.Session.SetString("session_dummy_filename", dummy_file_name);

                    // Generate a unique file name to avoid overwriting existing files.
                    string uniqueFileName = Path.Combine(folderPath, dummy_file_name);
                    errormsg = uniqueFileName;

                    // Save the uploaded file to the specified path.
                    using (var fileStream = new FileStream(uniqueFileName, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    HttpContext.Session.SetString("session_uniqueFileName", uniqueFileName);

                    return Json(new { success = true, message = "File uploaded successfully!", path = uniqueFileName });

                }
                else
                {
                    //Trace Error
                    //var errorLog = new ErrorLog
                    //{
                    //    in_errorlog_pipeline_code = pipeline_code,
                    //    in_errorlog_scheduler_gid = 0,
                    //    in_errorlog_type = "Else - Method Name : UploadFile",
                    //    in_errorlog_exception = folderPath,
                    //    in_created_by = ""
                    //};
                    //IsErrorlog(errorLog);
                    return Json(new { success = false, message = "No file selected." });
                }
            }
            catch (Exception ex)
            {
                //Trace Error
                var errorLog = new ErrorLog
                {
                    in_errorlog_pipeline_code = pipeline_code,
                    in_errorlog_scheduler_gid = 0,
                    in_errorlog_type = "Catch - Method Name : UploadFile",
                    in_errorlog_exception = ex.Message,
                    in_created_by = ""
                };
                IsErrorlog(errorLog);

                return Json(new { success = false, message = "No file selected." });
            }
        }

        [HttpPost]
        public byte[] ReadExcelFileToByteArray(IFormFile file)
        {
            try
            {
                //folderPath = _configuration.GetConnectionString("Uploadpath");

                string filePath = folderPath + file.FileName;
                // Check the file extension to determine the format.
                string fileExtension = Path.GetExtension(filePath);

                if (string.Equals(fileExtension, ".xls", StringComparison.OrdinalIgnoreCase))
                {
                    // Read .xls (Excel 97-2003) file using NPOI
                    using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        var workbook = new HSSFWorkbook(fileStream);
                        if (workbook != null)
                        {
                            // Assuming you want to read the first sheet (index 0).
                            var sheet = workbook.GetSheetAt(0);
                            if (sheet != null)
                            {
                                // Convert the workbook to a byte array.
                                using (var memoryStream = new MemoryStream())
                                {
                                    workbook.Write(memoryStream);
                                    return memoryStream.ToArray();
                                }
                            }
                        }
                    }
                }
                else if (string.Equals(fileExtension, ".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    ExcelPackage.LicenseContext = LicenseContext.Commercial;
                    // Read .xlsx (Excel 2007 and later) file using EPPlus
                    using (var package = new ExcelPackage(new FileInfo(filePath)))
                    {
                        var workbook = package.Workbook;
                        if (workbook != null)
                        {
                            // Assuming you want to read the first worksheet (index 1).
                            var worksheet = workbook.Worksheets[0];
                            if (worksheet != null)
                            {
                                // Convert the Excel package to a byte array.
                                return package.GetAsByteArray();
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Unsupported file format.");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions here.
                Console.WriteLine("Error: " + ex.Message);
            }

            return null; // Return null if something goes wrong or the format is unsupported.
        }

        [HttpPost]
        public byte[] FileToByteArray(IFormFile file)
        {
            // Read the Excel file into a byte array
            //string filePath = folderPath + "\\" + file.FileName;
            //byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            byte[] fileBytes = null;
            if (file != null && file.Length > 0)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    // Copy the file's binary data directly into the MemoryStream
                    file.CopyTo(ms);

                    // Get the byte array from the MemoryStream
                    fileBytes = ms.ToArray();

                }
            }
            return fileBytes;

        }

        [HttpPost]
        public byte[] FileToBinaryByteArray(IFormFile file)
        {
            try
            {
                string fileExtension = Path.GetExtension(file.FileName);

                using (Stream fs = file.OpenReadStream())
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        byte[] bytes = br.ReadBytes((int)fs.Length);
                        return bytes; // Return the byte array
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions here
                Console.WriteLine("Error: " + ex.Message);
                return null; // Return null or handle the error appropriately
            }
        }

        public byte[] ConvertFiletobyte(IFormFile inputfile)
        {
            byte[] file;
            string filePath = folderPath + inputfile.FileName;
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new BinaryReader(stream))
                {
                    file = reader.ReadBytes((int)stream.Length);
                }
            }
            return file;
        }

        [HttpPost]
        public void DeleteFile(string filePath)
        {
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    ViewBag.Message = $"File at path '{filePath}' has been deleted.";
                }
                else
                {
                    ViewBag.Message = $"File at path '{filePath}' does not exist.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error deleting file: {ex.Message}";
            }

        }

        [HttpGet]
        public async Task<JsonResult> GetExcelsheetlist(string path, string password)
        {
            List<DatabaseInfo> dbinfo = new List<DatabaseInfo>();

            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Pipeline/GetExcelSheetNames?filePath=" + path + "&password=" + password);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                dbinfo = JsonConvert.DeserializeObject<List<DatabaseInfo>>(results);
            }

            return Json(dbinfo);
        }
        [HttpPost]
        public async Task<bool> ValidateFile_bool(string file_Paths, string sheetName, string filename)
        {
            bool flag = false;
            var dummyfile_name = "";
            var file_extension = "";
            try
            {
                file_extension = Path.GetExtension(file_Paths);
                dummyfile_name = filename;//HttpContext.Session.GetString("session_dummy_filename");
                if (file_Paths == null)
                {
                    file_Paths = folderPath + filename;//HttpContext.Session.GetString("session_pplcodefilename");
                    file_extension = Path.GetExtension(file_Paths);

                }
                else
                {
                    file_Paths = folderPath + dummyfile_name;
                }

                HttpContext.Session.SetString("session_sheetname", sheetName);

                var results = "";
                ConnectorApi _api = new ConnectorApi(_configuration);
                HttpClient client = _api.Initial();

                var requestData = new
                {
                    file_Paths,
                    sheetName
                };

                var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                HttpResponseMessage res = await client.PostAsync("Pipeline/ValidateFile_sheet", content);

                if (res.IsSuccessStatusCode)
                {
                    results = await res.Content.ReadAsStringAsync();
                    bool.TryParse(results, out flag);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
            }

            return flag;
        }
        public bool ValidateFile_old(string filePath, string sheetName)
        {
            var flag = false;
            var dummyfile_name = "";
            var file_extension = "";
            try
            {
                file_extension = Path.GetExtension(filePath);
                dummyfile_name = HttpContext.Session.GetString("session_dummy_filename");
                if (filePath == null)
                {
                    filePath = folderPath + HttpContext.Session.GetString("session_pplcodefilename");
                    file_extension = Path.GetExtension(filePath);

                }
                else
                {
                    filePath = folderPath + dummyfile_name;
                }

                HttpContext.Session.SetString("session_sheetname", sheetName);

                //if (file_extension == ".xls")
                //{
                //    using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                //    {
                //        HSSFWorkbook workbook = new HSSFWorkbook(fileStream);
                //        HSSFSheet sheet = (HSSFSheet)workbook.GetSheet(sheetName);

                //        if (sheet != null && sheet.GetRow(0) != null)
                //        {
                //            flag = true;
                //            Console.WriteLine($"The sheet '{sheetName}' has at least one row of data.");
                //        }
                //        else
                //        {
                //            Console.WriteLine($"The sheet '{sheetName}' does not contain any rows of data.");
                //        }
                //    }
                //}
                //else if (file_extension == ".xlsx")
                //{
                //    using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                //    {
                //        XSSFWorkbook workbook = new XSSFWorkbook(fileStream);
                //        XSSFSheet sheet = (XSSFSheet)workbook.GetSheet(sheetName);

                //        if (sheet != null && sheet.GetRow(0) != null)
                //        {
                //            flag = true;
                //            Console.WriteLine($"The sheet '{sheetName}' has at least one row of data.");
                //        }
                //        else
                //        {
                //            Console.WriteLine($"The sheet '{sheetName}' does not contain any rows of data.");
                //        }
                //    }
                //}
                //else
                //{
                //    Console.WriteLine("Unsupported file format.");
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
            }

            return flag;
        }

        [HttpPost]
        public async Task<JsonResult> ValidateFile(string file_Paths, string sheetName, string filename)
        {
            var dummyfile_name = "";
            var file_extension = "";
            var result = new ValidationResult1 { IsValid = false, Message = "Unknown error occurred." };

            try
            {
                file_extension = Path.GetExtension(file_Paths);
                dummyfile_name = filename;

                if (string.IsNullOrEmpty(file_Paths))
                {
                    file_Paths = folderPath + filename;
                    file_extension = Path.GetExtension(file_Paths);
                }
                else
                {
                    file_Paths = folderPath + dummyfile_name;
                }

                HttpContext.Session.SetString("session_sheetname", sheetName);

                ConnectorApi _api = new ConnectorApi(_configuration);
                HttpClient client = _api.Initial();

                var requestData = new
                {
                    file_Paths,
                    sheetName
                };

                var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                HttpResponseMessage res = await client.PostAsync("Pipeline/ValidateFile_sheet", content);

                if (res.IsSuccessStatusCode)
                {
                    var results = await res.Content.ReadAsStringAsync();

                    // Deserialize the JSON coming from ValidateFile_sheet
                    result = JsonConvert.DeserializeObject<ValidationResult1>(results);
                }
                else
                {
                    result.Message = "Failed to validate file. API returned error: " + res.StatusCode;
                }
            }
            catch (Exception e)
            {
                result.Message = $"An error occurred: {e.Message}";
            }

            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> Gettablelist(string connection_code, string database_name)
        {
            List<TableAndView> tblview = new List<TableAndView>();

            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Pipeline/GetTables?connection_code="
              + connection_code + "&databasename=" + database_name);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                tblview = JsonConvert.DeserializeObject<List<TableAndView>>(results);
            }
            return Json(tblview);
        }

        [HttpGet]
        public async Task<JsonResult> Getviewlist(string connection_code, string database_name)
        {
            List<TableAndView> tblview = new List<TableAndView>();

            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Pipeline/GetViews?connection_code="
              + connection_code + "&databasename=" + database_name);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                tblview = JsonConvert.DeserializeObject<List<TableAndView>>(results);
            }
            return Json(tblview);
        }

        [HttpGet]
        public async Task<JsonResult> GetSourceTableFielddrp(string conn_code, string dbname, string srctable)
        {
            List<SourcetblFields> tblfields = new List<SourcetblFields>();

            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Pipeline/GetSourcetblFields?connection_code="
                + conn_code + "&databasename=" + dbname + "&sourcetable=" + srctable);
            if (res.IsSuccessStatusCode)
            {

                results = res.Content.ReadAsStringAsync().Result;
                tblfields = JsonConvert.DeserializeObject<List<SourcetblFields>>(results);
            }
            return Json(tblfields);
        }

        [HttpGet]
        public async Task<JsonResult> GetSourceExcelFielddrp(string pipeline_code, string path, string sheetname, string user_code, string dataset_code, string datasetinsert)
        {
            List<DatabaseInfo> dbinfo = new List<DatabaseInfo>();

            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Pipeline/ReadFirstRowFromExcel?pipelinecode=" + pipeline_code + "&filePath="
                + path + "&sheetName=" + sheetname + "&user_code=" + user_code + "&datasetcode=" + dataset_code + "&datasetinsert=" + datasetinsert);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                dbinfo = JsonConvert.DeserializeObject<List<DatabaseInfo>>(results);
            }
            return Json(dbinfo);
        }

        [HttpGet]
        public async Task<JsonResult> GetSourcetvqFielddrp(string conn_code, string pipeline_code, string db_name, string src_table,
            string table_view_query_type, string user_code, string dataset_code, string datasetInsert)
        {
            List<DatabaseInfo> dbinfo = new List<DatabaseInfo>();

            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Pipeline/ReadTablecolFromSource?connection_code=" + conn_code + "&pipelinecode=" + pipeline_code + "&databasename="
                + db_name + "&sourcetable=" + src_table + "&tvq_type=" + table_view_query_type + "&user_code=" + user_code + "&dataset_code=" + dataset_code + "&datasetInsert=" + datasetInsert);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                dbinfo = JsonConvert.DeserializeObject<List<DatabaseInfo>>(results);
            }
            return Json(dbinfo);
        }

        [HttpGet]
        public async Task<JsonResult> GetSrcExpressionDrpdown(string pipeline_code, string source_type)
        {
            List<SrcExpression> dbinfo = new List<SrcExpression>();

            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Pipeline/GetSourceFieldDropdown?pipeline_code="
                + pipeline_code + "&source_type=" + source_type);
            if (res.IsSuccessStatusCode)
            {

                results = res.Content.ReadAsStringAsync().Result;
                dbinfo = JsonConvert.DeserializeObject<List<SrcExpression>>(results);
            }
            return Json(dbinfo);
        }

        [HttpGet]
        public async Task<JsonResult> GetKeyFielddrp(string conn_code, string dbname, string table_name)
        {
            List<SourcetblFields> tblfields = new List<SourcetblFields>();

            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Pipeline/GetKeyFields?connection_code="
                + conn_code + "&databasename=" + dbname + "&tablename=" + table_name);
            if (res.IsSuccessStatusCode)
            {

                results = res.Content.ReadAsStringAsync().Result;
                tblfields = JsonConvert.DeserializeObject<List<SourcetblFields>>(results);
            }
            return Json(tblfields);
        }

        [HttpGet]
        public async Task<JsonResult> GetTrgKeyFielddrp(string table_name, string pipeline_code)
        {
            List<TargettblKeyFields> tblfields = new List<TargettblKeyFields>();

            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Pipeline/GetTargetKeyFields?tablename=" + table_name + "&pipeline_code=" + pipeline_code);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                tblfields = JsonConvert.DeserializeObject<List<TargettblKeyFields>>(results);
            }
            return Json(tblfields);
        }

        [HttpGet]
        public async Task<JsonResult> GetTargetTableFielddrp(string dataset_code)
        {
            List<TargettblFields> tblfields = new List<TargettblFields>();

            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Pipeline/GetTargettblFields?dataset_code="
                + dataset_code);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                tblfields = JsonConvert.DeserializeObject<List<TargettblFields>>(results);
            }
            return Json(tblfields);
        }

        [HttpGet]
        public async Task<JsonResult> GetTargetTablelist()
        {
            List<TargetTable> trgtbl = new List<TargetTable>();

            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Pipeline/GetTargettable");
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                trgtbl = JsonConvert.DeserializeObject<List<TargetTable>>(results);
            }
            return Json(trgtbl);
        }

        [HttpGet]
        public async Task<JsonResult> GetTableFieldlist(string conn_code, string dbname, string srctable, string trgtable)
        {
            List<TableFields> tblfields = new List<TableFields>();

            var results = "";
            results.DefaultIfEmpty();
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Pipeline/GetFieldNames?connection_code=" + conn_code + "&databasename="
                                      + dbname + "&sourcetable=" + srctable + "&targettable=" + trgtable);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                tblfields = JsonConvert.DeserializeObject<List<TableFields>>(results);
            }
            return Json(tblfields);
        }

        [HttpPost]
        public async Task<JsonResult> SourceToTargetDataPush([FromBody] SourceToTargetPushdata srcTotrgdatapush)
        {
            List<TableFields> tblfields = new List<TableFields>();

            //var results = "";
            // Create a dictionary with your parameters
            //var parameters = new Dictionary<string, string>
            //{
            //    { "connection_code", srcTotrgdatapush.conn_code },
            //    { "databasename", dbname },
            //    { "sourcetable", srctable},
            //    { "source_field_columns", srcfieldname },
            //    { "targettable", trgtable },
            //    { "defaultvalue", defaultval },
            //    { "upload_mode", upload_mode },
            //    { "primary_key", primary_key }
            //};
            //var content = new FormUrlEncodedContent(srcTotrgdatapush);
            //HttpClient client = _api.Initial();
            //HttpResponseMessage res = await client.PostAsync("Pipeline/SourcetoTargetPush", content);
            var results = "";
            var json = JsonConvert.SerializeObject(srcTotrgdatapush);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);

            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("Pipeline/SourcetoTargetPush", stringContent);

            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }
            return Json(results);
        }

        [HttpPost]
        public async Task<JsonResult> Addnewpipeline([FromBody] PipelineModel objppl)
        {
            var results = "";
            var json = JsonConvert.SerializeObject(objppl);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("Pipeline/AddPipeline", stringContent);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }

            return Json(results);
        }

        [HttpPost]
        public async Task<JsonResult> Updateexistingpipeline([FromBody] PipelineModel objupdatepipeline)
        {
            if (objupdatepipeline.source_file_name == "" || objupdatepipeline.source_file_name == null)
            {
                objupdatepipeline.source_file_name = HttpContext.Session.GetString("session_filename");
            }
            if (objupdatepipeline.sheet_name == "" || objupdatepipeline.sheet_name == null)
            {
                objupdatepipeline.sheet_name = HttpContext.Session.GetString("session_sheetname");
            }
            //objupdatepipeline.sheet_name = objupdatepipeline.sheet_name != "" && HttpContext.Session.GetString("session_sheetname") != null ? HttpContext.Session.GetString("session_sheetname") : objupdatepipeline.sheet_name;
            //objupdatepipeline.source_file_name = HttpContext.Session.GetString("session_filename") != "" && HttpContext.Session.GetString("session_filename") != null ? HttpContext.Session.GetString("session_filename") : objupdatepipeline.source_file_name;

            var results = "";
            var json = JsonConvert.SerializeObject(objupdatepipeline);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("Pipeline/UpdatePipeline", stringContent);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }
            return Json(results);
        }

        [HttpPost]
        public async Task<JsonResult> Addnewpplfieldmap([FromBody] pplFieldMapping objpplmap)
        {
            var results = "";
            var json = JsonConvert.SerializeObject(objpplmap);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("Pipeline/AddpplFieldMapping", stringContent);
            if (res.IsSuccessStatusCode)
            {
                results = await res.Content.ReadAsStringAsync();
            }

            return Json(results);
        }

        [HttpPost]
        public async Task<JsonResult> Addnewpplfieldmap_old([FromBody] pplFieldMapping objpplmap)
        {
            var results = "";
            var json = JsonConvert.SerializeObject(objpplmap);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("Pipeline/AddpplFieldMapping", stringContent);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }

            return Json(results);
        }

        [HttpGet]
        public async Task<JsonResult> Readpplfieldmapping(string pipeline_code)
        {
            List<pplFieldMapping> objpplfield = new List<pplFieldMapping>();
            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Pipeline/GetpplFieldMapping?pipelinecode=" + pipeline_code);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                objpplfield = JsonConvert.DeserializeObject<List<pplFieldMapping>>(results);
            }

            return Json(objpplfield);
        }

        [HttpGet]
        public async Task<JsonResult> Readpplfieldmap(string pipeline_code, string dataset, string source)
        {
            List<pplFieldMapping> objpplfield = new List<pplFieldMapping>();
            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Pipeline/GetpplFieldMappingList?pipelinecode=" + pipeline_code + "&dataset_code=" + dataset
                + "&source_name=" + source);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                objpplfield = JsonConvert.DeserializeObject<List<pplFieldMapping>>(results);
            }

            return Json(objpplfield);
        }

        [HttpPost]
        public async Task<JsonResult> pplfieldmapupdatelist([FromBody] List<DatasetfieldModel> objpplmapupdlist)
        //public async Task<JsonResult> pplfieldmapupdatelist([FromBody] List<pplFieldMapping> objpplmapupdlist)
        {
            var results = "";
            var json = JsonConvert.SerializeObject(objpplmapupdlist);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("Pipeline/UpdatepplFieldMapping", stringContent);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }

            return Json(results);
        }

        [HttpDelete]
        public async Task<JsonResult> Deleteexistingpplfieldmap(int id)
        {
            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.DeleteAsync("Pipeline/Deletepplfieldmap?id=" + id);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }

            return Json(results);
        }

        [HttpGet]
        public async Task<JsonResult> GetAllexpressions(string pipelinecode, string dataset_code)
        {
            List<pplSourceExpression> pplsrcexp = new List<pplSourceExpression>();
            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Pipeline/GetallExpressionsList?pipeline_code=" + pipelinecode + "&dataset_code=" + dataset_code);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                pplsrcexp = (List<pplSourceExpression>)JsonConvert.DeserializeObject<List<pplSourceExpression>>(results);
            }

            return Json(pplsrcexp);
        }

        [HttpPost]
        public async Task<JsonResult> Addnewpplsourcefield([FromBody] pplSourceExpression objpplsrcexp)
        {
            var results = "";
            var json = JsonConvert.SerializeObject(objpplsrcexp);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("Pipeline/Addpplsourcefield", stringContent);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }

            return Json(results);
        }

        [HttpPost]
        public async Task<JsonResult> Updateexistingpplsourcefield([FromBody] pplSourceExpression objpplsrcexp)
        {
            var results = "";
            var json = JsonConvert.SerializeObject(objpplsrcexp);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("Pipeline/Updatepplsourcefield", stringContent);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }

            return Json(results);
        }

        [HttpPost]
        public async Task<JsonResult> Deleteexistingpplsourcefield(int id)
        {
            var results = "";
            var json = JsonConvert.SerializeObject(id);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();

            HttpResponseMessage res = await client.PostAsync("Pipeline/Deletepplsourcefield?pplsourcefield_gid=" + id, stringContent);

            if (res.IsSuccessStatusCode)
            {
                results = await res.Content.ReadAsStringAsync();
            }

            return Json(results);
        }

        [HttpPost]
        public async Task<JsonResult> Addnewpplcondition([FromBody] pplCondition objpplcond)
        {
            var results = "";
            var json = JsonConvert.SerializeObject(objpplcond);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("Pipeline/AddpplCondition", stringContent);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }

            return Json(results);
        }

        [HttpGet]
        public async Task<JsonResult> ReadpipelineCondition(string pipelinecode, string condition_type, string datasetcode)
        {

            List<pplCondition> objpplcon = new List<pplCondition>();
            DataTable dt = new DataTable();
            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Pipeline/GetpplCondition?pipelinecode=" + pipelinecode + "&condition_type=" + condition_type + "&datasetcode=" + datasetcode);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                objpplcon = (List<pplCondition>)JsonConvert.DeserializeObject<List<pplCondition>>(results);

            }

            return Json(objpplcon);
        }

        [HttpPost]
        public async Task<JsonResult> Updateexistingpplcondition([FromBody] pplCondition objupdatepplcond)
        {
            var results = "";
            var json = JsonConvert.SerializeObject(objupdatepplcond);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("Pipeline/Updatepplcondition", stringContent);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }

            return Json(results);
        }

        [HttpPost]
        public async Task<JsonResult> Deleteexistingpplcondition(int id)
        {
            var results = "";
            var json = JsonConvert.SerializeObject(id);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();

            HttpResponseMessage res = await client.PostAsync("Pipeline/Deletepplcondition?pplcondition_gid=" + id, stringContent);

            if (res.IsSuccessStatusCode)
            {
                results = await res.Content.ReadAsStringAsync();
            }

            return Json(results);
        }

        [HttpGet]
        public async Task<JsonResult> GetSrcUpdatetimestampFielddrp(string conn_code, string dbname, string srctable)
        {
            List<SourcetblFields> tblfields = new List<SourcetblFields>();

            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Pipeline/GetSrcUpdattimestampFields?connection_code="
                + conn_code + "&databasename=" + dbname + "&sourcetable=" + srctable);
            if (res.IsSuccessStatusCode)
            {

                results = res.Content.ReadAsStringAsync().Result;
                tblfields = JsonConvert.DeserializeObject<List<SourcetblFields>>(results);
            }
            return Json(tblfields);
        }

        [HttpPost]
        public async Task<JsonResult> Addnewpplfinalization([FromBody] pplFinalization objpplfinalization)
        {
            var results = "";
            var json = JsonConvert.SerializeObject(objpplfinalization);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("Pipeline/Addpplfinalization", stringContent);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }

            return Json(results);
        }

        [HttpPost]
        public async Task<JsonResult> Updateexistingpplfinalization([FromBody] pplFinalization objupdatepplfinz)
        {
            var results = "";
            var json = JsonConvert.SerializeObject(objupdatepplfinz);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("Pipeline/Updatepplfinalization", stringContent);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }

            return Json(results);
        }

        [HttpGet]
        public async Task<JsonResult> ReadpipelineFinalization(string pipelinecode, string dataset_code)
        {
            List<pplFinalization> objpplfinz = new List<pplFinalization>();
            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Pipeline/GetpplFinalization?pipelinecode=" + pipelinecode + "&dataset_code=" + dataset_code);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                objpplfinz = JsonConvert.DeserializeObject<List<pplFinalization>>(results);
            }

            return Json(objpplfinz);
        }

        [HttpGet]
        public async Task<string> SetPPlStatus(string type, string dataset_code, string pipeline_code)
        {
            var results = "";
            try
            {
                ConnectorApi _api = new ConnectorApi(_configuration);
                HttpClient client = _api.Initial();
                HttpResponseMessage res = await client.GetAsync("Pipeline/SetPipelineStatus?validation_type=" + type +
                    "&dscode=" + dataset_code + "&ppl_code=" + pipeline_code); // Replace pipelinecode with a valid value
                if (res.IsSuccessStatusCode)
                {
                    results = await res.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                results = ex.Message;
            }

            return results;
        }

        [HttpPost]
        public async Task<JsonResult> Deleteexistingpipeline(string pipeline_code)
        {
            // folderPath = _configuration.GetConnectionString("Uploadpath");

            HttpContext.Session.SetString("session_folderpath", "");
            HttpContext.Session.SetString("session_filename", "");
            HttpContext.Session.SetString("session_dummy_filename", "");
            HttpContext.Session.SetString("session_sheetname", "");

            var results = "";
            var fileext = "";
            var json = JsonConvert.SerializeObject(pipeline_code);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();

            try
            {
                HttpResponseMessage res = await client.PostAsync("Pipeline/DeletePipeline?pipeline_code=" + pipeline_code, stringContent);

                if (res.IsSuccessStatusCode)
                {
                    results = await res.Content.ReadAsStringAsync();
                    fileext = HttpContext.Session.GetString("fileextension");
                    DeleteFile(folderPath + pipeline_code + fileext);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here
            }

            return Json(results);
        }

        [HttpGet]
        public async Task<bool> IsCheckQuery(string conn_code, string dbname, string extractionquery)
        {
            bool result = false; // Initialize with a default value

            try
            {
                ConnectorApi _api = new ConnectorApi(_configuration);
                HttpClient client = _api.Initial();
                HttpResponseMessage res = await client.GetAsync("Pipeline/IsQueryValid?conn_code=" + conn_code +
                    "&db_name=" + dbname + "&query=" + extractionquery); // Replace pipelinecode with a valid value
                if (res.IsSuccessStatusCode)
                {
                    string responseContent = await res.Content.ReadAsStringAsync();
                    if (bool.TryParse(responseContent, out result))
                    {
                        result = result;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }

        [HttpGet]
        public async Task<string> IsCheckQueryValidation_old(string pipeline_code, string dataset_code, string query, string queryfor)
        {
            var results = "";
            try
            {
                ConnectorApi _api = new ConnectorApi(_configuration);
                HttpClient client = _api.Initial();
                HttpResponseMessage res = await client.GetAsync("Pipeline/IsCheckQueryValid?pipeline_code=" + pipeline_code +
                  "&dataset_code=" + dataset_code + "&query=" + query + "&query_for=" + queryfor); // Replace pipelinecode with a valid value
                if (res.IsSuccessStatusCode)
                {
                    results = await res.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                results = ex.Message;
            }

            return results;
        }

        [HttpGet]
        public async Task<string> IsCheckQueryValidation(string pipeline_code, string dataset_code, string query, string queryfor)
        {
            var results = "";
            CheckQueryModel Chkquery = new CheckQueryModel();


            try
            {
                Chkquery.pipeline_code = pipeline_code;
                Chkquery.dataset_code = dataset_code;
                Chkquery.query = query;
                Chkquery.query_for = queryfor;

                var json = JsonConvert.SerializeObject(Chkquery);
                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                ConnectorApi _api = new ConnectorApi(_configuration);
                HttpClient client = _api.Initial();
                HttpResponseMessage res = await client.PostAsync("Pipeline/IsCheckQueryValid", stringContent); // Replace pipelinecode with a valid value
                if (res.IsSuccessStatusCode)
                {
                    results = await res.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                results = ex.Message;
            }

            return results;
        }

        [HttpGet]
        public async Task<string> ExcelCheckQuery(string fileLocation, string query, string queryfor)
        {
            string fileExtension = "";
            string curr_date = string.Format(CultureInfo.InvariantCulture.DateTimeFormat, "#{0}#", DateTime.Today); // $CURDATE$
            string curr_datetime = string.Format(CultureInfo.InvariantCulture.DateTimeFormat, "#{0}#", DateTime.Now); // $CURDATETIME$

            // Replace date & DateTime
            query = query.Replace("$CURDATE$", curr_date);
            query = query.Replace("$CURDATETIME$", curr_datetime);

            if (queryfor != "Expression" && !string.IsNullOrEmpty(query))
            {
                query = "and " + query;
            }

            string[] parts = fileLocation.Split('.');
            fileExtension = "." + parts.Last();
            string result = "";
            DataSet ds = new DataSet();
            try
            {
                FileInfo file = new FileInfo(fileLocation);

                using (var workbook = new XLWorkbook(fileLocation))
                {
                    var worksheet = workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        return null;
                    }

                    DataTable dtexcel = new DataTable();
                    int totalRows = worksheet.LastRowUsed().RowNumber();
                    int totalCols = worksheet.LastColumnUsed().ColumnNumber();
                    for (int col = 1; col <= totalCols; col++)
                    {
                        dtexcel.Columns.Add(new DataColumn(worksheet.Cell(1, col).GetValue<string>()));
                    }

                    for (int row = 2; row <= totalRows; row++)
                    {
                        DataRow newRow = dtexcel.NewRow();
                        for (int col = 1; col <= totalCols; col++)
                        {
                            string val = worksheet.Cell(row, col).GetValue<string>(); // Retrieve cell value
                            newRow[col - 1] = val ?? ""; // Set cell value as empty if it's null
                        }
                        dtexcel.Rows.Add(newRow);
                    }

                    if (queryfor != "Expression")
                    {
                        DataView view = new DataView(dtexcel);
                        view.RowFilter = "1=2 " + query;
                        dtexcel = view.ToTable();
                    }

                    ds.Tables.Add(dtexcel.Copy());

                    result = "Success";
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        [HttpGet]
        public async Task<string> ExcelCheckQuery_OLEDB(string fileLocation, string query, string queryfor)
        {
            string fileExtension = "";
            if (queryfor != "Expression" && query != null && query != "")
            {
                query = "and " + query;
            }

            string[] parts = fileLocation.Split('.');
            fileExtension = "." + parts.Last();
            string result = "";
            DataSet ds = new DataSet();
            try
            {
                string excelConnectionString = "";//"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=PathToExcelFileWithExtension;Extended Properties='Excel 8.0;HDR=YES'";

                if (fileExtension == ".xls")
                {
                    excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"";
                }
                else if (fileExtension == ".xlsx")
                {
                    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
                }
                OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);//connection for excel.
                excelConnection.Open();//excel connection open.
                DataTable dtexcel = new DataTable();//datatable object.
                dtexcel = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if (dtexcel == null)
                {
                    return null;
                }
                String[] excelSheets = new String[dtexcel.Rows.Count];
                int t = 0;
                //excel data saves in temp file here.
                foreach (DataRow row in dtexcel.Rows)
                {
                    excelSheets[t] = row["TABLE_NAME"].ToString();
                    t++;
                }
                OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);//connection for excel.
                int excel = excelSheets.Count();
                string query1 = "";
                if (queryfor != "Expression")
                {
                    query1 = string.Format("Select * from [{0}]", excelSheets[0]);
                    query1 = query1 + " Where 1=2 " + query;
                }
                else
                {
                    query1 = string.Format("Select " + query + " from [{0}]", excelSheets[0]);
                }
                using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query1, excelConnection1))
                {
                    dataAdapter.Fill(ds);
                    result = "Success";
                }
                excelConnection.Close();
            }

            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;

        }

        [HttpGet]
        public async Task<string> ExcelCheckQueryfromDT(string fileLocation, string query)
        {
            string results = ""; // Initialize with a default value

            try
            {
                // Create a DataTable to store the data
                DataTable dataTable = new DataTable();

                using (var workbook = new XLWorkbook(fileLocation))
                {
                    var worksheet = workbook.Worksheets.FirstOrDefault(); // Assuming the data is on the first worksheet
                    if (worksheet == null)
                    {
                        return "Worksheet not found";
                    }
                    // Add columns to the DataTable based on the Excel file's header row
                    foreach (var cell in worksheet.Row(1).Cells())
                    {
                        dataTable.Columns.Add(cell.GetValue<string>());
                    }

                    // Add data rows to the DataTable
                    for (int row = 2; row <= worksheet.LastRowUsed().RowNumber(); row++)
                    {
                        var dataRow = dataTable.NewRow();
                        for (int col = 1; col <= worksheet.LastColumnUsed().ColumnNumber(); col++)
                        {
                            dataRow[col - 1] = worksheet.Cell(row, col).GetValue<string>();
                        }
                        dataTable.Rows.Add(dataRow);
                    }
                }

                dataTable.Select(query);

                results = "Success";
            }
            catch (Exception ex)
            {
                results = ex.Message;
            }

            return results;
        }
        #region Dataprocessing start
        [HttpPost]
        public async Task<JsonResult> getMasterData(String parentCode = "", String dependCode = "")
        {


            var results = "";
            var fileext = "";
            //var masterDatas = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("DataProcessings/GetMasterDatawithCode?parentCode=" + parentCode);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                var masterDatas = JsonConvert.DeserializeObject<List<Masters>>(results)
                        .Select(c => new
                        {
                            Value = c.master_code,
                            Text = c.master_name
                        })
                        .ToList();

                return Json(masterDatas);
            }

            return Json("");
        }

        [HttpPost]
        public async Task<JsonResult> getMasterDatawithCode(String parentCode = "", String dependCode = "")
        {


            var results = "";
            var fileext = "";
            //var masterDatas = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("DataProcessings/GetMasterDatawithCode?parentCode=" + parentCode);
            var connectionModel = new Connection();
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                //var masterDatas = JsonConvert.DeserializeObject<List<Masters>>(results)
                //		.Select(c => new
                //		{
                //			Value = c.master_code,
                //			Text = c.master_name
                //		})
                //		.ToList();
                connectionModel = new Connection
                {
                    masterDatas = JsonConvert.DeserializeObject<List<Masters>>(results)
                        .Select(c => new SelectListItem
                        {
                            Value = c.master_code.ToString(),
                            Text = c.master_name
                        })
                        .ToList()
                };

                //return Json(masterDatas);
            }

            return Json(connectionModel);
        }

        [HttpPost]
        public async Task<JsonResult> getMappingFieldwithCode(String pipelineCode = "", string dataset_code = "")
        {


            var results = "";
            var fileext = "";
            //var masterDatas = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("DataProcessings/GetFieldMapping?pipelineCode=" + pipelineCode
                + "&dataset_code=" + dataset_code);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                var masterDatas = JsonConvert.DeserializeObject<List<FieldMapping>>(results)
                        .Select(c => new
                        {
                            Value = c.pplfieldmapping_gid,
                            Text = c.ppl_field_name
                        })
                        .ToList();

                return Json(masterDatas);
            }

            return Json("");
        }
        [HttpPost]
        public async Task<IActionResult> setDataprocessing(DataProcessing dataprocessing)
        {
            var results = "";
            try
            {
                var json = JsonConvert.SerializeObject(dataprocessing);
                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                ConnectorApi _api = new ConnectorApi(_configuration);
                HttpClient client = _api.Initial();
                HttpResponseMessage res = await client.PostAsync("DataProcessings/setDataprocessing", stringContent);
                if (res.IsSuccessStatusCode)
                {
                    results = res.Content.ReadAsStringAsync().Result;
                }

                return Json(results);
            }
            catch (Exception ex)
            {
                return Json("Error.");
            }

        }

        [HttpPost]
        public async Task<IActionResult> getDataprocessing(string pipelineCode = "", Int64 headerGid = 0)
        {
            var results = "";
            try
            {
                ConnectorApi _api = new ConnectorApi(_configuration);
                HttpClient client = _api.Initial();
                HttpResponseMessage res = await client.GetAsync("DataProcessings/GetDataprocessingList?pipelinecode=" + pipelineCode + "&headerGid=" + headerGid);
                if (res.IsSuccessStatusCode)
                {
                    results = res.Content.ReadAsStringAsync().Result;
                }

                return Json(results);
            }
            catch (Exception ex)
            {
                return Json("Error.");
            }

        }

        [HttpPost]
        public async Task<IActionResult> setDataprocessingheader(DataProcessingHeader dataprocessingheader)
        {
            var results = "";
            try
            {
                var json = JsonConvert.SerializeObject(dataprocessingheader);
                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                ConnectorApi _api = new ConnectorApi(_configuration);
                HttpClient client = _api.Initial();
                HttpResponseMessage res = await client.PostAsync("DataProcessings/setDataprocessingHeader", stringContent);
                if (res.IsSuccessStatusCode)
                {
                    results = res.Content.ReadAsStringAsync().Result;
                }

                return Json(results);
            }
            catch (Exception ex)
            {
                return Json("Error.");
            }

        }

        [HttpPost]
        public async Task<IActionResult> getDataprocessingheader(string pipelineCode = "", string datasetCode = "")
        {
            var results = "";
            try
            {
                ConnectorApi _api = new ConnectorApi(_configuration);
                HttpClient client = _api.Initial();
                HttpResponseMessage res = await client.GetAsync("DataProcessings/GetDataprocessingHeaderList?pipelineCode=" + pipelineCode + "&datasetCode=" + datasetCode);
                if (res.IsSuccessStatusCode)
                {
                    results = res.Content.ReadAsStringAsync().Result;
                }

                return Json(results);
            }
            catch (Exception ex)
            {
                return Json("Error.");
            }

        }
        [HttpGet]
        public async Task<IActionResult> GetfieldConfig()
        {
            var results = "";
            try
            {
                ConnectorApi _api = new ConnectorApi(_configuration);
                HttpClient client = _api.Initial();
                HttpResponseMessage res = await client.GetAsync("DataProcessings/GetfieldConfig");
                if (res.IsSuccessStatusCode)
                {
                    results = res.Content.ReadAsStringAsync().Result;
                }
                return Json(results);
            }
            catch (Exception ex)
            {
                return Json("Error.");
            }
        }

        [HttpPost]
        public async Task<JsonResult> pipeline_clone([FromBody] pipelineclone objpplclone)
        {
            var results = "";
            var json = JsonConvert.SerializeObject(objpplclone);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("Pipeline/Pipeline_Cloning", stringContent);

            if (res.IsSuccessStatusCode)
            {
                results = await res.Content.ReadAsStringAsync();
            }

            return Json(results);
        }

        [HttpPost]
        public async void IsErrorlog([FromBody] ErrorLog objerrorlog)
        {
            var results = "";
            var json = JsonConvert.SerializeObject(objerrorlog);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("Pipeline/Errorlog", stringContent);

            if (res.IsSuccessStatusCode)
            {
                results = await res.Content.ReadAsStringAsync();
            }
        }
        #endregion

        [HttpGet]
        public async Task<IActionResult> GetIncrementalRecord_old(string pipelinecode)
        {
            //var results = "";
            //try
            //{
            //    ConnectorApi _api = new ConnectorApi(_configuration);
            //    HttpClient client = _api.Initial();
            //    HttpResponseMessage res = await client.GetAsync("Pipeline/GetIncrementalRecord?pipelinecode=" + pipelinecode);
            //    if (res.IsSuccessStatusCode)
            //    {
            //        results = res.Content.ReadAsStringAsync().Result;
            //    }
            //    return Json(results);
            //}
            var results = "";

            try
            {
                IncrementalRecord objres = new IncrementalRecord();
                ConnectorApi _api = new ConnectorApi(_configuration);
                HttpClient client = _api.Initial();
                HttpResponseMessage res = await client.GetAsync("Pipeline/GetIncrementalRecord?pipelinecode=" + pipelinecode);

                if (res.IsSuccessStatusCode)
                {
                    results = res.Content.ReadAsStringAsync().Result;
                    objres = JsonConvert.DeserializeObject<IncrementalRecord>(results);
                }

                return Json(objres);
            }
            catch (Exception ex)
            {
                return Json("Error.");
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetIncrementalRecord(string pipelinecode)
        {
            List<IncrementalRecord> pipelines = new List<IncrementalRecord>();
            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Pipeline/GetIncrementalRecord?pipelinecode=" + pipelinecode);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                pipelines = (List<IncrementalRecord>)JsonConvert.DeserializeObject<List<IncrementalRecord>>(results);
            }

            return Json(pipelines);
        }

        [HttpPost]
        public async Task<JsonResult> SaveIncrementalRecord([FromBody] saveIncrementalRedordModel objsaveincremental)
        {
            var results = "";
            try
            {
                var json = JsonConvert.SerializeObject(objsaveincremental);
                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                ConnectorApi _api = new ConnectorApi(_configuration);
                HttpClient client = _api.Initial();
                HttpResponseMessage res = await client.PostAsync("Pipeline/setIncrementalRecord", stringContent);
                if (res.IsSuccessStatusCode)
                {
                    results = res.Content.ReadAsStringAsync().Result;
                }

                return Json(results);
            }
            catch (Exception ex)
            {
                return Json("Error.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> TemplateDownload1(string pipelinecode, string fileExtension)
        {
            try
            {
                // Define the folder path where the files are stored
                string fileName = pipelinecode + fileExtension; // Assuming the file extension is .csv; adjust as needed
                string filePath = Path.Combine(folderPath, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return Json(new { success = false, message = "File not found." });
                }

                // Return the file path or indicate success
                return Json(new
                {
                    success = true,
                    filePath = Url.Action("TemplateDownload", "Pipeline", new { pipelinecode, fileExtension })
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }

        [HttpPost]
        public async Task<JsonResult> Addnewpplcsvheader([FromBody] pplCsvHeader objpplcsvheader)
        {
            var results = "";
            var json = JsonConvert.SerializeObject(objpplcsvheader);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("Pipeline/Addpplcsvheader", stringContent);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }

            return Json(results);
        }

        [HttpPost]
        public async Task<JsonResult> Updateexistingpplcsvheader([FromBody] pplCsvHeader objpplcsvheader)
        {
            var results = "";
            var json = JsonConvert.SerializeObject(objpplcsvheader);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("Pipeline/Updatepplcsvheader", stringContent);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }

            return Json(results);
        }

        [HttpGet]
        public async Task<JsonResult> ReadpipelineCsvHeader(string pipelinecode)
        {
            List<pplCsvHeader> objpplfinz = new List<pplCsvHeader>();
            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Pipeline/GetpplCsvHeader?pipelinecode=" + pipelinecode);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                objpplfinz = JsonConvert.DeserializeObject<List<pplCsvHeader>>(results);
            }

            return Json(objpplfinz);
        }

        [HttpPost]
        public async Task<JsonResult> Addnewpplcsvsourcefield([FromBody] pplSourceCsv objpplcsvsrcfield)
        {
            var results = "";
            var json = JsonConvert.SerializeObject(objpplcsvsrcfield);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("Pipeline/Addcsvsourcefield", stringContent);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }

            return Json(results);
        }

        [HttpPost]
        public async Task<JsonResult> Updateexistingpplcsvsrcfield([FromBody] pplSourceCsv objpplcsvsrcfield)
        {
            var results = "";
            var json = JsonConvert.SerializeObject(objpplcsvsrcfield);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("Pipeline/Updatecsvsourcefield", stringContent);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }

            return Json(results);
        }

        [HttpGet]
        public async Task<JsonResult> GetAllCsvSource(string pipelinecode)
        {
            List<pplSourceCsv> pplsrccsv = new List<pplSourceCsv>();
            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Pipeline/GetallCsvsourcefiledList?pipeline_code=" + pipelinecode);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                pplsrccsv = (List<pplSourceCsv>)JsonConvert.DeserializeObject<List<pplSourceCsv>>(results);
            }

            return Json(pplsrccsv);
        }

        [HttpGet]
        public IActionResult TemplateDownload(string pipelinecode, string fileExtension, string sourceFileName)
        {
            try
            {
                var getfolderPath = "";
                if (fileExtension == ".xlsm")
                {
                    getfolderPath = macroFolderPath;
                }
                else
                {
                    getfolderPath = folderPath;
                }
                // Define the folder path where files are stored
                string fileName = pipelinecode + fileExtension; // Assuming the file extension is .csv; adjust as needed
                string filePath = Path.Combine(getfolderPath, fileName);

                // Check if the file exists
                if (!System.IO.File.Exists(filePath))
                {
                    return Ok("File not found.");
                }

                // Read the file into a stream
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                var originalFileName = sourceFileName + fileExtension;
                // Return the file for download
                return File(fileBytes, "application/octet-stream", originalFileName);
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<JsonResult> getQCDMasterData(string parentCode, string depend_code)
        {
            var results = "";
            var fileext = "";
            //var masterDatas = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Pipeline/GetQCDMasterwithParentCode?parent_master_syscode=" + parentCode + "&depend_master_syscode=" + depend_code);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                var masterDatas = JsonConvert.DeserializeObject<List<ConnectionDto>>(results)
                        .Select(c => new
                        {
                            Id = c.Id,
                            Name = c.Name
                        })
                        .ToList();

                return Json(masterDatas);
            }

            return Json("");
        }


        // Hema
        [HttpPost]
        public async Task<JsonResult> Addnewpplapiheader([FromBody] pplApiHeader objpplapiheader)
        {
            var results = "";
            var json = JsonConvert.SerializeObject(objpplapiheader);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("Pipeline/Addpplapiheader", stringContent);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }
            return Json(results);
        }
        [HttpPost]
        public async Task<JsonResult> Updateexistingpplapiheader([FromBody] pplApiHeader objpplapiheader)
        {
            var results = "";
            var json = JsonConvert.SerializeObject(objpplapiheader);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("Pipeline/Updatepplapiheader", stringContent);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }
            return Json(results);
        }
        [HttpGet]
        public async Task<JsonResult> ReadpipelineApiHeader(string pipelinecode)
        {
            List<pplApiHeader> objpplfinz = new List<pplApiHeader>();
            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Pipeline/GetpplApiHeader?pipelinecode=" + pipelinecode);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                objpplfinz = JsonConvert.DeserializeObject<List<pplApiHeader>>(results);
            }
            return Json(objpplfinz);
        }
        //Addnewpplapisourcefield
        [HttpPost]
        public async Task<JsonResult> Addnewpplapisourcefield([FromBody] pplSourceCsv objpplcsvsrcfield)
        {
            var results = "";
            var json = JsonConvert.SerializeObject(objpplcsvsrcfield);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("Pipeline/Addapisourcefield", stringContent);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }
            return Json(results);
        }
        //Updateexistingpplapisrcfield
        [HttpPost]
        public async Task<JsonResult> Updateexistingpplapisrcfield([FromBody] pplSourceCsv objpplcsvsrcfield)
        {
            var results = "";
            var json = JsonConvert.SerializeObject(objpplcsvsrcfield);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("Pipeline/Updateapisourcefield", stringContent);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }
            return Json(results);
        }
        //GetAllApiSource
        [HttpGet]
        public async Task<JsonResult> GetAllApiSource(string pipelinecode)
        {
            List<pplSourceCsv> pplsrccsv = new List<pplSourceCsv>();
            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Pipeline/GetallApisourcefiledList?pipeline_code=" + pipelinecode);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                pplsrccsv = (List<pplSourceCsv>)JsonConvert.DeserializeObject<List<pplSourceCsv>>(results);
            }
            return Json(pplsrccsv);
        }

        //getAPIResponse
        [HttpGet]
        public async Task<IActionResult> GetAPIResponse(string url)
        {
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            try
            {
                HttpResponseMessage res = await client.GetAsync(url);
                if (res.IsSuccessStatusCode)
                {
                    var results = await res.Content.ReadAsStringAsync();
                    return Content(results, "application/json");
                }
                else
                {
                    return BadRequest(new { error = "API call failed", statusCode = res.StatusCode });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        static JObject GenerateSchema(JObject obj)
        {
            JObject schema = new JObject();

            foreach (var property in obj.Properties())
            {
                if (property.Value is JObject nestedObject)
                {
                    schema[property.Name] = GenerateSchema(nestedObject);
                }
                else if (property.Value is JArray nestedArray && nestedArray.Count > 0 && nestedArray[0] is JObject firstElement)
                {
                    schema[property.Name] = new JArray(GenerateSchema(firstElement));
                }
                else
                {
                    schema[property.Name] = JValue.CreateNull();
                }
            }

            return schema;
        }


        [HttpGet]
        public async Task<JsonResult> getExcelResponse(string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var sheetsData = new Dictionary<string, List<Dictionary<string, object>>>();

                foreach (var sheet in package.Workbook.Worksheets)
                {
                    var sheetJson = ConvertSheetToJson(sheet);
                    if (sheetJson != null)
                    {
                        sheetsData[sheet.Name] = new List<Dictionary<string, object>> { sheetJson };
                    }
                }

                var serializedData = JsonConvert.SerializeObject(sheetsData, Newtonsoft.Json.Formatting.Indented);
                return Json(serializedData);
            }
        }

        static Dictionary<string, object> ConvertSheetToJson(ExcelWorksheet worksheet)
        {
            int colCount = worksheet.Dimension?.End.Column ?? 0;
            if (colCount == 0) return null;

            var jsonObject = new Dictionary<string, object>();

            for (int col = 1; col <= colCount; col++)
            {
                string columnName = worksheet.Cells[1, col].Text.Trim(); // Read header
                if (!string.IsNullOrEmpty(columnName))
                {
                    jsonObject[columnName] = null; // Assign null values
                }
            }

            return jsonObject;
        }

        [HttpPost]
        public async Task<JsonResult> Addnewppldatasetheader([FromBody] ppldatasetHeader objppldatasetheader)
        {
            var results = "";
            var json = JsonConvert.SerializeObject(objppldatasetheader);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("Pipeline/Adddatasetheader", stringContent);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }
            return Json(results);
        }

        [HttpGet]
        public async Task<JsonResult> GetDataSet(string pipeline_code, string source_fields, string datasetType)
        {

            List<DatasetModel> datasetlist = new List<DatasetModel>();
            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Dataset/GetDataSet?pipeline_code=" + pipeline_code + "&source_fields=" + source_fields + "&datasetType=" + datasetType);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                datasetlist = (List<DatasetModel>)JsonConvert.DeserializeObject<List<DatasetModel>>(results);
            }
            return Json(datasetlist);
        }

        [HttpGet]
        public async Task<JsonResult> GetDataSetField(string pipeline_code, string datset_code)
        {
            try
            {
                List<DatasetfieldModel> datasetlist = new List<DatasetfieldModel>();
                var results = "";
                ConnectorApi _api = new ConnectorApi(_configuration);
                HttpClient client = _api.Initial();
                HttpResponseMessage res = await client.GetAsync("Dataset/GetDataSetField?pipeline_code=" + pipeline_code + "&datset_code=" + datset_code);
                if (res.IsSuccessStatusCode)
                {
                    results = res.Content.ReadAsStringAsync().Result;
                    datasetlist = (List<DatasetfieldModel>)JsonConvert.DeserializeObject<List<DatasetfieldModel>>(results);
                }
                return Json(datasetlist);
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        [HttpPost]
        public async Task<JsonResult> datasetheader([FromBody] datasetHeader objdatasetheader)
        {
            var results = "";
            var json = JsonConvert.SerializeObject(objdatasetheader);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            client.DefaultRequestHeaders.Add("user_code", objdatasetheader.in_action_by);
            client.DefaultRequestHeaders.Add("lang_code", "en");
            client.DefaultRequestHeaders.Add("role_code", "admin");
            HttpResponseMessage res = await client.PostAsync("Dataset/DatasetHeader", stringContent);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }
            return Json(results);
        }

        [HttpGet]
        public async Task<JsonResult> GetSourceFiels(string pipeline_code, string datset_code)
        {
            List<sourcefieldModel> sourcefieldlist = new List<sourcefieldModel>();
            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Pipeline/GetSourceFiels?pipeline_code=" + pipeline_code + "&dataset_code=" + datset_code);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                sourcefieldlist = (List<sourcefieldModel>)JsonConvert.DeserializeObject<List<sourcefieldModel>>(results);
            }
            return Json(sourcefieldlist);
        }

        [HttpPost]
        public async Task<JsonResult> datasetdetail([FromBody] datasetDetail objdatasetdetail)
        {
            var results = "";
            var json = JsonConvert.SerializeObject(objdatasetdetail);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("Dataset/DatasetDetail", stringContent);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }
            return Json(results);
        }

        [HttpPost]
        public async Task<JsonResult> Deletepplfieldmap(int id)
        {
            var results = "";
            var json = JsonConvert.SerializeObject("");
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("Pipeline/Deletepplfieldmap?id=" + id, stringContent);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }
            return Json(results);
        }


        [HttpGet]
        public async Task<JsonResult> Getmysqlcolheader(string conn_code, string pipeline_code, string db_name, string src_table,
        string table_view_query_type, string user_code)
        {
            List<DatabaseInfo> dbinfo = new List<DatabaseInfo>();
            var results = "";
            try
            {
                ConnectorApi _api = new ConnectorApi(_configuration);
                HttpClient client = _api.Initial();
                HttpResponseMessage res = await client.GetAsync("Pipeline/GetMyslcolFromSource?connection_code=" + conn_code + "&pipelinecode=" + pipeline_code + "&databasename="
                    + db_name + "&sourcetable=" + src_table + "&tvq_type=" + table_view_query_type + "&user_code=" + user_code);

                if (res.IsSuccessStatusCode)
                {
                    results = res.Content.ReadAsStringAsync().Result;
                }
                return Json(results);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        public async Task<JsonResult> UploadMacroFile(string pipeline_code, IFormFile file)
        {
            string errormsg = "";
            var filePath = "";
            try
            {
                if (file != null && file.Length > 0)
                {
                    errormsg = macroFolderPath;
                    if (!Directory.Exists(macroFolderPath))
                    {
                        Directory.CreateDirectory(macroFolderPath);
                    }

                    string[] parts = file.FileName.Split('.');
                    string extension = "." + parts.Last();
                    filePath = folderPath + file.FileName;
                    string dummy_file_name = pipeline_code + extension;

                    //HttpContext.Session.SetString("session_folderpath", folderPath);
                    //HttpContext.Session.SetString("session_filename", file.FileName);
                    //HttpContext.Session.SetString("session_dummy_filename", dummy_file_name);
                    string uniqueFileName = Path.Combine(macroFolderPath, dummy_file_name);
                    errormsg = uniqueFileName;

                    using (var fileStream = new FileStream(uniqueFileName, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    //  HttpContext.Session.SetString("session_uniqueMacroFileName", uniqueFileName);

                    return Json(new { success = true, message = "File uploaded successfully!", path = uniqueFileName });

                }
                else
                {
                    return Json(new { success = false, message = "No file selected." });
                }
            }
            catch (Exception ex)
            {
                var errorLog = new ErrorLog
                {
                    in_errorlog_pipeline_code = pipeline_code,
                    in_errorlog_scheduler_gid = 0,
                    in_errorlog_type = "Catch - Method Name : UploadMacroFile",
                    in_errorlog_exception = ex.Message,
                    in_created_by = ""
                };
                IsErrorlog(errorLog);

                return Json(new { success = false, message = "No file selected." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GeneratePdf(string in_pipeline_code, string in_dataset_code)
        {
            try
            {
                var report = new { in_pipeline_code };
                string json = JsonConvert.SerializeObject(report);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                ConnectorApi _api = new ConnectorApi(_configuration);
                HttpClient client = _api.Initial();
                client.Timeout = Timeout.InfiniteTimeSpan;

                HttpResponseMessage response = await client.GetAsync("Pipeline/GeneratePdf?pipeline_code=" + in_pipeline_code + "&dataset_code=" + in_dataset_code);
                if (response.IsSuccessStatusCode)
                {
                    byte[] bytes = await response.Content.ReadAsByteArrayAsync();
                    if (in_dataset_code != "" && in_dataset_code != null)
                    {
                        return File(bytes, "application/pdf", $"{in_dataset_code}.pdf");
                    }
                    else
                    {
                        return File(bytes, "application/pdf", $"{in_pipeline_code}.pdf");
                    }
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Failed to generate PDF");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<JsonResult> CloneDataset([FromBody] CloneDatasetModel objCloneDataset)
        {
            try
            {
                var results = "";
                var json = JsonConvert.SerializeObject(objCloneDataset);
                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                ConnectorApi _api = new ConnectorApi(_configuration);
                HttpClient client = _api.Initial();
                HttpResponseMessage res = await client.PostAsync("Dataset/ClonePipelineDataset", stringContent);
                if (res.IsSuccessStatusCode)
                {
                    results = res.Content.ReadAsStringAsync().Result;
                }
                return Json(results);
            }
            catch (Exception ex)
            {
                return Json(ex);
            }


        }

        //---------------------Pandiaraj support doc add 25-08-2025-------------------------//
        [HttpPost]
        public async Task<IActionResult> UploadSupportFiles(List<IFormFile> supportfileUploader, string pipeline_code,
            string created_by, string action, string gridDataJson)
        {
            List<SupportingDoc> supportingdocList = new List<SupportingDoc>();
            if (!string.IsNullOrEmpty(gridDataJson))
            {
                supportingdocList = JsonConvert.DeserializeObject<List<SupportingDoc>>(gridDataJson);
            }

            var outMessages = new List<string>();
            string folderPath = Path.Combine("wwwroot", "SupportingDocs");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            List<DataTable> dbresults = new List<DataTable>();
            var results = "";

            var json = JsonConvert.SerializeObject(supportingdocList);
            var stringContent = new StringContent(gridDataJson, UnicodeEncoding.UTF8, "application/json");
            //var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("Pipeline/supportingDoc", stringContent);
            if (res.IsSuccessStatusCode)
            {
                results = await res.Content.ReadAsStringAsync();
                var parsed = JsonConvert.DeserializeObject<List<List<SupportingDoc>>>(results);
                int count = parsed.Count;

                for (int i = 0; i < supportfileUploader.Count; i++)
                {
                    var file = supportfileUploader[i];
                    var table = parsed[i];
                    var innerList = parsed[i];
                    string originalName = Path.GetFileName(file.FileName);
                    string extension = Path.GetExtension(originalName);
                    if (innerList != null && innerList.Count > 0)
                    {
                        int uploadgid = parsed
                        .SelectMany(x => x)
                        .Where(doc => doc.supportingdoc_name == originalName)
                        .Select(doc => doc.supportingdoc_gid)
                        .FirstOrDefault();
                        string renamedFile = $"{pipeline_code}-{uploadgid}{extension}";
                        string savedPath = Path.Combine(folderPath, renamedFile);
                        using (var stream = new FileStream(savedPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                    }

                }

                outMessages = parsed.SelectMany(x => x)
                                        .Select(y => y.out_msg)
                                        .ToList();

            }

            return Json(new { success = true, message = "Uploaded", data = outMessages });
        }

        [HttpGet]
        public async Task<JsonResult> GetSupportDocuments(string pipeline_code)
        {
            List<SupportingDoc> sourcefieldlist = new List<SupportingDoc>();
            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Pipeline/GetSupportDocs?pipeline_code=" + pipeline_code);
            if (res.IsSuccessStatusCode)
            {
                string resultJson = res.Content.ReadAsStringAsync().Result;
                sourcefieldlist = (List<SupportingDoc>)JsonConvert.DeserializeObject<List<SupportingDoc>>(resultJson);
            }
            return Json(sourcefieldlist);
        }

        [HttpGet]
        public IActionResult SupportDocsdownload(string dbfileName, string filename)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dbfileName))
                    return BadRequest("Filename is required");

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "SupportingDocs", dbfileName);

                if (!System.IO.File.Exists(filePath))
                    return NotFound("File not found.");

                var mimeType = "application/octet-stream"; // Or use a MIME type resolver
                var fileBytes = System.IO.File.ReadAllBytes(filePath);

                // If user supplied a custom download name, use it; otherwise use original
                var outputFileName = string.IsNullOrEmpty(filename) ? dbfileName : filename;

                return File(fileBytes, mimeType, outputFileName);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }
        [HttpPost]
        public async Task<JsonResult> deleteSupportDocs(int id)
        {
            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            var jsonContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(id), System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage res = await client.PostAsync("Pipeline/deleteSupportDocs", jsonContent);

            {
                results = res.Content.ReadAsStringAsync().Result;
            }

            return Json(results);
        }

        //---------------------Pandiaraj support doc add 25-08-2025-------------------------//



        [HttpPost]
        public async Task<IActionResult> Addpplapisrcfield(string pipeline_code, string dataset_code, string user_code, string datasetinsert, [FromBody] Dictionary<string, object> resultSetStructure)
        {
            var results = "";
            var json = JsonConvert.SerializeObject(resultSetStructure);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("Pipeline/AddNewpplapisrcfield?pipeline_code=" + pipeline_code + "&dataset_code=" + dataset_code + "&user_code=" + user_code + "&datasetinsert=" + datasetinsert, stringContent);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }
            return Json(results);
        }

        [HttpPost]
        public JsonResult ValidateJson([FromBody] JsonInputModel input)
        {
            if (string.IsNullOrWhiteSpace(input.JsonData))
            {
                return Json(new { isValid = false, message = "JSON is empty." });
            }

            try
            {
                using (JsonDocument.Parse(input.JsonData))
                {
                    return Json(new { isValid = true, message = "Valid JSON." });
                }
            }
            catch (System.Text.Json.JsonException ex)
            {
                return Json(new { isValid = false, message = "Invalid JSON: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CallExternalApi([FromBody] ProxyRequest request)
        {
            if (request == null)
                return BadRequest("Request body missing or invalid JSON.");

            if (string.IsNullOrWhiteSpace(request.Url))
                return BadRequest("Url is required.");

            if (!Uri.TryCreate(request.Url, UriKind.Absolute, out var uri))
                return BadRequest("Invalid URL. Must be absolute (e.g. http://localhost:4984/login).");

            // Determine method dynamically
            var method = new HttpMethod(
                string.IsNullOrWhiteSpace(request.Method) ? "GET" : request.Method.ToUpperInvariant()
            );

            var httpRequest = new HttpRequestMessage(method, uri);

            // --- Attach body automatically for methods that support content ---
            if (method == HttpMethod.Post || method == HttpMethod.Put ||
                method.Method.Equals("PATCH", StringComparison.OrdinalIgnoreCase))
            {
                string contentString;

                if (request.Body.ValueKind == JsonValueKind.String)
                {
                    // Handle stringified JSON
                    contentString = request.Body.GetString() ?? "";
                }
                else if (request.Body.ValueKind != JsonValueKind.Undefined &&
                         request.Body.ValueKind != JsonValueKind.Null)
                {
                    // Convert JSON object to string
                    contentString = System.Text.Json.JsonSerializer.Serialize(request.Body);
                }
                else
                {
                    contentString = "";
                }

                var contentType = "application/json";
                if (request.Headers != null && request.Headers.TryGetValue("Content-Type", out var inCt))
                    contentType = inCt;

                httpRequest.Content = new StringContent(contentString, Encoding.UTF8, contentType);
            }

            // --- Add headers ---
            if (request.Headers != null)
            {
                foreach (var kv in request.Headers)
                {
                    // Try add to request headers first
                    if (!httpRequest.Headers.TryAddWithoutValidation(kv.Key, kv.Value))
                    {
                        // fallback to content headers if applicable
                        if (httpRequest.Content == null)
                            httpRequest.Content = new StringContent("");
                        httpRequest.Content.Headers.TryAddWithoutValidation(kv.Key, kv.Value);
                    }
                }
            }

            // --- Send request ---
            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(httpRequest);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status502BadGateway, $"Error contacting remote server: {ex.Message}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseContentType = response.Content.Headers.ContentType?.ToString() ?? "application/json";

            if (response.IsSuccessStatusCode)
            {
                return Content(responseContent, responseContentType, Encoding.UTF8);
            }

            return StatusCode((int)response.StatusCode, new
            {
                error = "API call failed",
                statusCode = response.StatusCode,
                content = responseContent
            });
        }

        [HttpPost]
        public async Task<JsonResult> pplapinode(string pipeline_code, string dataset_code, string user_code, [FromBody] List<apinodeModel> objapinode)
        {
            var results = "";
            var json = JsonConvert.SerializeObject(objapinode);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            // HttpResponseMessage res = await client.PostAsync("Pipeline/pplapinode", stringContent);
            HttpResponseMessage res = await client.PostAsync("Pipeline/pplapinode?pipeline_code=" + pipeline_code + "&dataset_code=" + dataset_code + "&user_code=" + user_code, stringContent);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }

            return Json(results);
        }

        [HttpPost]
        public IActionResult ValidateApiQuery([FromBody] apifilterCondition request)
        {
            if (request == null)
                return BadRequest("Invalid payload");

            var result = ValidateApiQuery(request.JsonData, request.Query);

            return Ok(new
            {
                isValid = result.IsValid,
                message = result.Message
            });
        }

        [HttpPost]
        public static (bool IsValid, string Message) ValidateApiQuery(string JsonData, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return (true, "Valid (empty query)");

            HashSet<string> jsonKeys = GetAllJsonKeys(JsonData);
            HashSet<string> queryColumns = ExtractColumnsFromQuery(query);

            foreach (var col in queryColumns)
            {
                if (!jsonKeys.Contains(col))
                {
                    return (false, $"Invalid column: {col}");
                }
            }

            return (true, "Query Validated Successfully..!");
        }

        public static HashSet<string> ExtractColumnsFromQuery(string query)
        {
            // Remove quoted values ('x' or "x")
            query = Regex.Replace(query, @"'[^']*'|""[^""]*""", "");

            // Remove operators & keywords
            query = Regex.Replace(query,
                @"\b(AND|OR|IN|NOT|IS|NULL|LIKE|BETWEEN)\b",
                "",
                RegexOptions.IgnoreCase);

            // Remove numbers
            query = Regex.Replace(query, @"\b\d+(\.\d+)?\b", "");

            // Remove operators and symbols EXCEPT []
            query = Regex.Replace(query, @"[=<>!(),]", " ");

            var columns = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);

            foreach (Match match in Regex.Matches(query, @"\[(.*?)\]|\b[a-zA-Z_][a-zA-Z0-9_]*\b"))
            {
                string column = match.Groups[1].Success
                    ? match.Groups[1].Value   // [status] → status
                    : match.Value;

                columns.Add(column.Trim());
            }

            return columns;
        }

        public static HashSet<string> GetAllJsonKeys(string jsonData)
        {
            var keys = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);

            void Traverse(JToken token)
            {
                if (token is JObject obj)
                {
                    foreach (var prop in obj.Properties())
                    {
                        keys.Add(prop.Name);
                        Traverse(prop.Value);
                    }
                }
                else if (token is JArray arr)
                {
                    foreach (var item in arr)
                        Traverse(item);
                }
            }

            Traverse(JToken.Parse(jsonData));
            return keys;
        }

        //getAllDatasetFields

        [HttpPost]
        public async Task<JsonResult> getAllDatasetFields([FromBody] getAllDatasetFieldsModel objgetAllDatasetFields)
        {
            try
            {
                var results = "";
                var json = JsonConvert.SerializeObject(objgetAllDatasetFields);
                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                ConnectorApi _api = new ConnectorApi(_configuration);
                HttpClient client = _api.Initial();
                HttpResponseMessage res = await client.PostAsync("Dataset/getAllDatasetFields", stringContent);
                if (res.IsSuccessStatusCode)
                {
                    results = res.Content.ReadAsStringAsync().Result;
                }
                return Json(results);
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }
        //parentchildrelation
        public async Task<JsonResult> parentchildrelation([FromBody] List<parentchildRelation> objparentchildRelation)
        {
            var results = "";
            var json = JsonConvert.SerializeObject(objparentchildRelation);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("Pipeline/Parentchildrelation", stringContent);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }
            return Json(results);
        }

        // GetparentchildRelationlist
        [HttpGet]
        public async Task<JsonResult> GetparentchildRelationlist(string pipeline_code, string dataset_code, string child_ds_code = "")
        {
            List<parentchildRelation> parentChildRelationlist = new List<parentchildRelation>();
            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Dataset/GetparentchildRelationlist?pipeline_code=" + pipeline_code + "&dataset_code=" + dataset_code + "&child_ds_code=" + child_ds_code);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }
            return Json(results);
            //if (res.IsSuccessStatusCode)
            //{
            //    results = res.Content.ReadAsStringAsync().Result;
            //    parentChildRelationlist = (List<parentchildRelation>)JsonConvert.DeserializeObject<List<parentchildRelation>>(results);
            //}
            //return Json(parentChildRelationlist);
        }

        [HttpGet]
        public async Task<JsonResult> Deleteexistingchild_ds(string pipeline_code, string parent_ds_code, string child_ds_code = "")
        {
            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.DeleteAsync("Pipeline/Deleteexistingchild_ds?pipeline_code=" + pipeline_code + "&parent_ds_code=" + parent_ds_code + "&child_ds_code=" + child_ds_code);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }
            return Json(results);
        }

    }
}
