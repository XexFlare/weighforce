using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace WeighForce.Services
{
    public class CrmLock
    {
        [JsonPropertyName("locked")]
        public bool? Locked { get; set; }
        [JsonPropertyName("unlocked")]
        public bool? Unlocked { get; set; }
    }
    public class CrmService
    {
        public HttpClient Client { get; }
        public string IDAddress;
        private readonly string _installId;
        public DateTime startTime;
        public DateTime syncTime;
        public CrmService(HttpClient client, IConfiguration configuration)
        {
            IDAddress = configuration.GetValue("Sync:IDAddress", "http://127.0.0.1");
            _installId = configuration.GetValue("InstallID", "Kanengo");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.BaseAddress = new Uri(configuration.GetValue("Sync:CRMAddress", "http://127.0.0.1:1337"));
            Client = client;
            startTime = DateTime.Now;
            syncTime = DateTime.Now;
        }

        public async Task<bool> GetLock()
        {
            try
            {
                var res = await Client.GetStringAsync($"/lock?id={_installId}");
                var _lock = JsonSerializer.Deserialize<CrmLock>(res);
                return _lock.Locked ?? true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> CancelLock()
        {
            try
            {
                var res = await Client.GetStringAsync($"/unlock?id={_installId}");
                var _lock = JsonSerializer.Deserialize<CrmLock>(res);
                return _lock.Unlocked ?? true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<T> Get<T>(string endpoint, DateTime lastUpdate, int limit = -1, long id = -1) where T : class
        {
            try
            {
                if (id != -1)
                {
                    var response = await Client.GetAsync($"/{endpoint}/{id}");
                    response.EnsureSuccessStatusCode();
                    using var responseStream = await response.Content.ReadAsStreamAsync();
                    return await JsonSerializer.DeserializeAsync
                        <T>(responseStream);
                }
                else
                {
                    var response = await Client.GetAsync($"/{endpoint}?sync_time_gt={startTime.ToUniversalTime():o}&sync_time_lt={lastUpdate.ToUniversalTime():o}&_limit={limit}");
                    response.EnsureSuccessStatusCode();
                    using var responseStream = await response.Content.ReadAsStreamAsync();
                    return await JsonSerializer.DeserializeAsync
                        <T>(responseStream);
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(endpoint);
                throw new Exception($"CRM Connection Error: GET {endpoint}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception("Error");
            }
        }

        public async Task DownloadFile(string path, string writePath)
        {
            HttpClient downloadClient = new HttpClient();
            downloadClient.DefaultRequestHeaders.Add("Accept", "application/json");
            downloadClient.BaseAddress = new Uri(IDAddress);
            var response = await downloadClient.GetAsync(path);
            using var stream = await response.Content.ReadAsStreamAsync();
            var fileInfo = new FileInfo(writePath);
            using var fileStream = fileInfo.OpenWrite();
            await stream.CopyToAsync(fileStream);
        }
        public async Task<T> Post<T>(dynamic updates, string endpoint, DateTime? lastUpdate = null, string param = null) where T : class
        {
            try
            {
                var data = new StringContent(JsonSerializer.Serialize(updates), Encoding.UTF8, "application/json");
                // Console.WriteLine(JsonSerializer.Serialize(item));
                HttpResponseMessage response;
                if (lastUpdate != null)
                    response = await Client.PostAsync($"/{endpoint}?sync_time={syncTime.ToUniversalTime():o}&sync_time_gt={startTime.ToUniversalTime():o}&sync_time_lt={lastUpdate?.ToUniversalTime():o}&_limit=-1&{param}", data);
                else
                    response = await Client.PostAsync($"/{endpoint}&{param}", data);
                response.EnsureSuccessStatusCode();
                using var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync
                    <T>(responseStream);
            }
            catch (HttpRequestException e)
            {
                // var obj = JsonSerializer.Serialize(updates);
                Console.WriteLine(e);
                throw new Exception($"CRM Connection Error: POST {endpoint}");
            }
            catch (Exception e)
            {
                // var obj = JsonSerializer.Serialize(updates);
                Console.WriteLine(e);
                throw new Exception($"CRM Connection Error: POST {endpoint}");
            }
        }
        public async Task<T> Put<T>(dynamic item, long id, string endpoint) where T : class
        {
            try
            {
                var data = new StringContent(JsonSerializer.Serialize(item), Encoding.UTF8, "application/json");
                var response = await Client.PutAsync($"/{endpoint}/{id}", data);
                response.EnsureSuccessStatusCode();
                using var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync
                    <T>(responseStream);
            }
            catch (HttpRequestException)
            {
                var obj = JsonSerializer.Serialize(item);
                throw new Exception($"CRM Connection Error: PUT {endpoint} {obj}");
            }
            catch (Exception)
            {
                throw new Exception("Error");
            }
        }
    }
}