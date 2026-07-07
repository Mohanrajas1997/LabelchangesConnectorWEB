using FlexicodeConnectors.Helper;
using FlexicodeConnectors.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FlexicodeConnectors.Controllers
{
    public class ConnectorsController : Controller
    {

        private readonly IConfiguration _configuration;
        string api_url = "";
        public ConnectorsController(IConfiguration configuration)
        {
            _configuration = configuration;
            api_url = _configuration.GetConnectionString("Apiurl");
        }
        public async Task<IActionResult> ConnectorsList()
        {
            return View();

        }

        public async Task<JsonResult> GellAllConnectors()
        {
            List<Connection> connections = new List<Connection>();
            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Connector/GetConnectors");
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                connections = JsonConvert.DeserializeObject<List<Connection>>(results);
            }

            return Json(connections);
        }
        public IActionResult ConnectorDetails(int id)
        {
            ViewBag.conn_id = id;
            return View();
        }

        public async Task<JsonResult> Readconnector(int id)
        {
            Connection connections = new Connection();
            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Connector/GetConnector?id=" + id);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                connections = JsonConvert.DeserializeObject<Connection>(results);
            }

            return Json(connections);
        }
        [HttpPost]
        public async Task<JsonResult> Addnewconnector([FromBody] Connection objconn)
        {
            var results = "";
            var json = JsonConvert.SerializeObject(objconn);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("Connector/AddConnector", stringContent);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }

            return Json(results);
        }

        [HttpPost]
        public async Task<JsonResult> Updateexistingconnector([FromBody] Connection objupdateconn)
        {
            var results = "";
            var json = JsonConvert.SerializeObject(objupdateconn);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("Connector/UpdateConnector", stringContent);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }

            return Json(results);
        }

        [HttpPost]
        public async Task<JsonResult> Deleteexistingconnector(int id)
        {
            //var results = "";
            //HttpClient client = _api.Initial();
            //HttpResponseMessage res = await client.DeleteAsync("Connector/DeleteConnector?id=" + id);
            //if (res.IsSuccessStatusCode)
            //{
            //    results = res.Content.ReadAsStringAsync().Result;
            //}

            var results = "";
            var json = JsonConvert.SerializeObject(id);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();

            try
            {
                HttpResponseMessage res = await client.PostAsync("Connector/DeleteConnector?id=" + id, stringContent);

                if (res.IsSuccessStatusCode)
                {
                    results = res.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here
            }


            return Json(results);
        }

        [HttpPost]
        public async Task<JsonResult> EstablishConn([FromBody] Connection objconn)
        {
            var results = "";
            var json = JsonConvert.SerializeObject(objconn);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("Connector/Establishconnection", stringContent);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }

            return Json(results);
        }
        public IActionResult viewName()
        {
            try
            {

                List<Model1> objcat_3st = new List<Model1>();
                Model1 obj = new Model1();
                DataTable dt = new DataTable();
                DataRow dr = dt.NewRow();
                dt.Columns.Add("id");
                dt.Columns.Add("Name");

                dr = dt.NewRow();
                dr["id"] = 1;
                dr["Name"] = "view1";
                dr = dt.NewRow();
                dr["id"] = 2;
                dr["Name"] = "view2";
                dt.Rows.Add(dr);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    obj = new Model1();
                    obj.id = Convert.ToInt32(dt.Rows[i]["id"].ToString());
                    obj.viewname = dt.Rows[i]["name"].ToString();
                    objcat_3st.Add(obj);
                }

                return Json(objcat_3st);

            }
            catch (Exception e)
            {
                return Json(e.Message);
            }
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> AddapiauthToken([FromBody] apiAuthToken objconn)
        {
            var results = "";
            var json = JsonConvert.SerializeObject(objconn);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("Connector/AddapiauthToken", stringContent);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }
            return Json(results);
        }

        [HttpPost]
        public async Task<JsonResult> UpdateapiauthToken([FromBody] apiAuthToken objupdateconn)
        {
            var results = "";
            var json = JsonConvert.SerializeObject(objupdateconn);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.PostAsync("Connector/UpdateapiauthToken", stringContent);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
            }
            return Json(results);
        }

        [HttpGet]
        public async Task<JsonResult> GetApiAuthToken(string connection_code)
        {
            apiAuthToken apiauthtoken = new apiAuthToken();
            var results = "";
            ConnectorApi _api = new ConnectorApi(_configuration);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("Connector/GetApiAuthToken?connection_code=" + connection_code);
            if (res.IsSuccessStatusCode)
            {
                results = res.Content.ReadAsStringAsync().Result;
                apiauthtoken = JsonConvert.DeserializeObject<apiAuthToken>(results);
            }
            return Json(apiauthtoken);
        }

    }
}
