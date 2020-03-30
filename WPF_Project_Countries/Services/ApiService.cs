namespace WPF_Project_Countries.Services
{
    using Classes;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class ApiService
    {
        public async Task<Response> GetCountries(string urlBase, string controller)
        {
            try
            {
                var client = new HttpClient();

                client.BaseAddress = new Uri(urlBase);

                var response = await client.GetAsync(controller);

                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result
                    };
                }

                var countries = JsonConvert.DeserializeObject<List<Country>>(result);

                return new Response
                {
                    IsSuccess = true,
                    Result = countries
                };
            }
            catch(Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
