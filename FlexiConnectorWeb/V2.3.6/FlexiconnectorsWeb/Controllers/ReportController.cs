using FlexicodeConnectors.Helper;
using FlexicodeConnectors.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlexicodeConnectors.Controllers
{
    public class ReportController : Controller
    {
        private readonly IConfiguration _configuration;

        public ReportController(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectorApi _api = new ConnectorApi(configuration);

        }

        public IActionResult PipelineDatasetInfo()
        {
            return View();
        }

        public IActionResult ImportedFileInfo()
        {
            return View();
        }


        [HttpGet]
        public async Task<JsonResult> GellPipelinedatasetInfo(string pipeline_code, string dataset_code)
        {

            //List<DatasetModel> datasetlist = new List<DatasetModel>();
            var results = "";
            var response = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Report/PPLDSmappedReport?pipeline_code=" + pipeline_code + "&dataset_code=" + dataset_code);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                response = JsonConvert.DeserializeObject(results).ToString();
                //datasetlist = (List<DatasetModel>)JsonConvert.DeserializeObject<List<DatasetModel>>(results);
            }
            return Json(response);
        }


        [HttpGet]
        public async Task<JsonResult> GellImportedFileInfo(string from_date, string to_date)
        {

            //List<DatasetModel> datasetlist = new List<DatasetModel>();
            var results = "";
            var response = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Report/FileimportReport?from_date=" + from_date + "&to_date=" + to_date);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                response = JsonConvert.DeserializeObject(results).ToString();
                //datasetlist = (List<DatasetModel>)JsonConvert.DeserializeObject<List<DatasetModel>>(results);
            }
            return Json(response);
        }
    }
}
