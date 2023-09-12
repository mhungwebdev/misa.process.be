using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.PROCESS.COMMON;
using MISA.PROCESS.DAL.Interfaces;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace MISA.PROCESS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VNWorkController : ControllerBase
    {
        private IConfiguration _configuraion;
        private IConfigurationSection _vnworkConfig;
        private IBaseRepository<VNWorkTokenInfo> _baseRepository;

        public VNWorkController(IConfiguration configuraion, IBaseRepository<VNWorkTokenInfo> baseRepository)
        {
            _configuraion = configuraion;
            _vnworkConfig = _configuraion.GetSection("VNWork");
            _baseRepository = baseRepository;
        }

        [HttpGet("author")]
        public string GetUrlAuthorize()
        {
            var uri = _vnworkConfig.GetValue<string>("Author_Uri");
            var clientID = _vnworkConfig.GetValue<string>("ClientID");
            var clientSecret = _vnworkConfig.GetValue<string>("ClientSecret");
            var redirectUri = _vnworkConfig.GetValue<string>("Redirect_Uri_Short_Term");
            return $"{uri}?clientID={clientID}&clientSecret={clientSecret}&scope=jobpost&state=12345678&response_type=code&redirect_uri=${redirectUri}";
        }

        [HttpGet("author-short-term")]
        public async System.Threading.Tasks.Task<IActionResult> ReceivedShortTerm(string state, string code, string error)
        {
            if (string.IsNullOrWhiteSpace(error) && !string.IsNullOrWhiteSpace(code))
            {
                var httpClient = new HttpClient();
                var uri = _vnworkConfig.GetValue<string>("Author_Uri");
                var clientID = _vnworkConfig.GetValue<string>("ClientID");
                var clientSecret = _vnworkConfig.GetValue<string>("ClientSecret");
                var redirectUri = _vnworkConfig.GetValue<string>("Redirect_Uri_Short_Term");
                var respon = await httpClient.GetAsync($"{uri}?code=${code}&clientID={clientID}&clientSecret={clientSecret}&grant_type=authorization_code&redirect_uri=${redirectUri}");

                if (respon.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var data = await respon.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(data))
                    {
                        var longTerm = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);

                        if (longTerm != null && longTerm.Count > 0)
                        {
                            using (var connection = _baseRepository.GetConnection())
                            {
                                var commandInsert = "INSERT INTO vnw_token_info (TenantID,AuthorInfo) VALUES (@TenantID,@AuthorInfo);";
                                var param = new Dictionary<string, object>() {
                                    {"@TenantID", state },
                                    {"@AuthorInfo", longTerm.ToString() }
                                };

                                await connection.ExecuteAsync(commandInsert, param, commandType: System.Data.CommandType.Text);
                            }
                        }
                    }
                }
            }

            return Ok("Ok babe");
        }

        [HttpGet("GetCVs")]
        public async Task<IEnumerable<object>> GetCVs()
        {
            var result = new List<object>();
            var vnworkInfos = new List<VNWorkTokenInfo>();
            using (var connection = _baseRepository.GetConnection())
            {
                var commandGet = "SELECT * FROM vnw_token_info;";

                vnworkInfos = (List<VNWorkTokenInfo>)await connection.QueryAsync<VNWorkTokenInfo>(commandGet, commandType: System.Data.CommandType.Text);
            }

            if (vnworkInfos != null && vnworkInfos.Count > 0)
            {
                foreach (var vnworkInfo in vnworkInfos)
                {
                    var httpClient = new HttpClient();
                    var info = JsonConvert.DeserializeObject<Dictionary<string, object>>(vnworkInfo.AuthorInfo);
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bareer {info.GetValueOrDefault("access_token").ToString()}");

                    var responseJobs = await httpClient.GetAsync($"https://api.vietnamworks.com/api/rest/v1/jobs/online.json");
                    if (responseJobs != null && responseJobs.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var responseJobsString = await responseJobs.Content.ReadAsStringAsync();
                        var responseJobsObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJobsString);
                        if (responseJobsObject != null)
                        {
                            var jobs = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJobsObject["jobs"].ToString())["data"].ToString());
                            if (jobs != null && jobs.Count > 0)
                            {
                                foreach (var job in jobs)
                                {
                                    var responDetail = await httpClient.GetAsync($"https://api.vietnamworks.com/api/rest/v1/applications/{job["id"]}?page=1&lang=1");
                                    if (responDetail.StatusCode == System.Net.HttpStatusCode.OK)
                                    {
                                        var responDetailString = await responDetail.Content.ReadAsStringAsync();
                                        var responObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(responDetailString);
                                        var data = JsonConvert.DeserializeObject<List<object>>(JsonConvert.DeserializeObject<Dictionary<string, object>>(responObject["data"].ToString())["items"].ToString());
                                        result.AddRange(data);
                                    }
                                }
                            }
                        }

                    }
                }
            }

            return result;
        }
    }
}
