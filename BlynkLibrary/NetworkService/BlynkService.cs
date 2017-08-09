﻿using BlynkLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;
using Windows.Networking.Connectivity;

namespace BlynkLibrary.NetworkService
{
    public class BlynkService
    {
        private const string Blynk_api_String = "http://blynk-cloud.com/{0}/";

        private static Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
        private static Windows.Web.Http.Headers.HttpRequestHeaderCollection headers = httpClient.DefaultRequestHeaders;
        private static Windows.Web.Http.HttpResponseMessage httpResponse = new Windows.Web.Http.HttpResponseMessage();

        public async static Task<Project> Login(string auth)
        {
            string httpResponseBody = "";
            try
            {
                string uriString = String.Format(Blynk_api_String + "project",auth);
                httpResponse = await httpClient.GetAsync(new Uri(uriString));
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
                Project proj = JsonConvert.DeserializeObject<Project>(httpResponseBody);
                return proj;
            }
            catch (Exception e)
            {
                var error = new Project();
                error.id = 0;
                if(httpResponseBody == "Invalid token.")
                {
                    error.name = "Error: " + httpResponseBody;
                }
                else
                {
                    error.name = "Error: " + e.HResult.ToString("X") + " Message: " + e.Message;
                }
                return error;
            }
        }

        public static async Task<bool> IsInternet()
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            bool internet = connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            return internet;
        }
    }
}
