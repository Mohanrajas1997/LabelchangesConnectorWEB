using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Security.Policy;

namespace FlexicodeConnectors.Helper
{
    public class ConnectorApi
    {

		private readonly IConfiguration _configuration;
		string api_url = "";
		public ConnectorApi(IConfiguration configuration)
		{
			_configuration = configuration;
			api_url = _configuration.GetConnectionString("Apiurl");

		}
		public HttpClient Initial()
        {
            var client = new HttpClient();
			//client.BaseAddress = new Uri("http://localhost:5786/");
			//client.BaseAddress = new Uri("http://140.238.247.210/ConnectorApi/");
			client.BaseAddress = new Uri(api_url);
			//client.BaseAddress = new Uri("http://localhost/ConnectorApi/");
			return client;
        }
    }
}
