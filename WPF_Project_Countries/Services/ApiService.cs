namespace WPF_Project_Countries.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Net;
    using System.IO;
    using System.Drawing.Imaging;
    using Svg;
    using Newtonsoft.Json;
    using Library.Models;

    //TODO: Proper error handling in the catch blocks when downloading and converting flags
    //TODO: If the default.svg link no longer exists, the program will crash

    public class ApiService
    {
        /// <summary>
        /// Fetches data from the API through a JSON deserializer
        /// </summary>
        /// <param name="urlBase"></param>
        /// <param name="controller"></param>
        /// <returns>Response object</returns>
        public async Task<Response> GetCountries(string urlBase, string controller)
        {
            try
            {
                var client = new HttpClient
                {
                    BaseAddress = new Uri(urlBase)
                };

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

                var countries = JsonConvert.DeserializeObject<List<Country>>(result, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                if (!Directory.Exists("Flags"))
                {
                    Directory.CreateDirectory("Flags");
                }

                DownloadFlags(countries);

                await ConvertFlags(countries);

                return new Response
                {
                    IsSuccess = true,
                    Result = countries
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// Converts flags from svg format to bmp
        /// </summary>
        /// <param name="countries"></param>
        private async Task ConvertFlags(List<Country> countries)
        {
            foreach (var country in countries)
            {
                await Task.Run(() =>
                {
                    try
                    {

                        var svgDocument = SvgDocument.Open($@"Flags\{country.Name}.svg");
                        using (var smallBitmap = svgDocument.Draw())
                        {
                            var width = smallBitmap.Width;
                            var height = smallBitmap.Height;

                            using (var bitmap = svgDocument.Draw(width, height))
                            {
                                bitmap.Save($@"Flags\{country.Name}.bmp", ImageFormat.Bmp);
                            }
                        }
                    }
                    catch
                    {

                    }
                });
            }

            await Task.Run(()=> {
                try
                {
                    var defaultFlag = SvgDocument.Open($@"Flags\Default.svg");
                    using (var smallBitmap = defaultFlag.Draw())
                    {
                        var width = smallBitmap.Width;
                        var height = smallBitmap.Height;

                        using (var bitmap = defaultFlag.Draw(width, height))
                        {
                            bitmap.Save($@"Flags\Default.bmp", ImageFormat.Bmp);
                        }
                    }
                }
                catch
                {

                }
            });
        }

        /// <summary>
        /// Download flags from the API in svg format
        /// </summary>
        /// <param name="countries"></param>
        private void DownloadFlags(List<Country> countries)
        {
            foreach (var country in countries)
            {
                try
                {
                    using (var flag = new WebClient())
                    {
                        flag.DownloadFileAsync(country.Flag, $@"Flags\{country.Name}.svg");
                    }
                }
                catch
                {

                }
            }

            try
            {
                var noFlag = new WebClient();

                noFlag.DownloadFile("https://upload.wikimedia.org/wikipedia/commons/b/b0/No_flag.svg", $@"Flags\Default.svg");

                noFlag.Dispose();
            }
            catch
            {

            }
        }
    }
}
