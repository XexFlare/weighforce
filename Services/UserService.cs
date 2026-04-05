using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WeighForce.Dtos;

namespace WeighForce.Services
{
    public class UserService
    {
        public HttpClient Client { get; }

        public UserService(HttpClient client, IConfiguration configuration)
        {
            client.BaseAddress = new Uri(configuration.GetValue<string>("Sync:IDAddress", "https://localhost:5001"));
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            // client.DefaultRequestHeaders.Add("Authorization", "Bearer eyJhbGciOiJSUzI1NiIsImtpZCI6IkRldmVsb3BtZW50IiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE2MTI4MjY5NTksImV4cCI6MTYxMjgzMDU1OSwiaXNzIjoiaHR0cHM6Ly93ZWlnaGZvcmNlLmZlcnQuYXBwIiwiYXVkIjoiU2NhbGVVc0FQSSIsImNsaWVudF9pZCI6IlNjYWxlVXMiLCJzdWIiOiJmNDE4YTUyMy02NDU5LTQ4ZGYtYmZmYS05NmIwOTNiOGNlN2YiLCJhdXRoX3RpbWUiOjE2MTI4MjY5NTcsImlkcCI6ImxvY2FsIiwic2NvcGUiOlsib3BlbmlkIiwicHJvZmlsZSIsIlNjYWxlVXNBUEkiXSwiYW1yIjpbInB3ZCJdfQ.w5DNBMFQMD5ybd3s7-wq_lbSRZMFniRZVIPhIZNSPX-GAfUyWArxMi0MfatjWp8_meDjAhs-ZMZqdQ6CAu3WqHnS6gorQ_TyoPh_0EF2AjAGGjzGALPiZU--ObEB7MgDGtOA9rV4HD67UALq4iUNf7xpeMO3UFrRiA7X0t26-tn8_lk-eG9tyXyDJbE8KxV_D54cKJgEFs_ooebB8LmiQnoU5vYx8_39FxPUgDRzn_mBsPVBCNsH7EeGIlOg5ZrV07e4TeUqxpkMigUByJy4G0a4PAQrdsGxk7m0lpFjcqTGDCylJhJ6-YCjTRBOZ5r523rSDLX14L-BMoEr4_cUrw");
            Client = client;
        }

        public async Task<IDSyncDto> GetUsers()
        {
            try
            {
                var response = await Client.GetAsync("/id");
                response.EnsureSuccessStatusCode();
                using var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<IDSyncDto>(responseStream);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.Message);
                throw new Exception($"Auth Server Connection Error");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception("Error");
            }
        }
    }
}