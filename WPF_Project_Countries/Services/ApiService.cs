namespace WPF_Project_Countries.Services
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Net;
    using System.IO;
    using System.Drawing.Imaging;
    using Library.Models;
    using Svg;
    using Newtonsoft.Json;

    #endregion

    public class ApiService
    {
        #region Variables

        private readonly DialogService dialogService = new DialogService();

        #endregion

        #region Methods

        /// <summary>
        /// Fetches data from the API through a JSON deserializer
        /// </summary>
        /// <param name="urlBase"></param>
        /// <param name="controller"></param>
        /// <returns>Response object</returns>
        public async Task<Response> GetCountries(string urlBase, string controller, IProgress<ProgressReport> progress)
        {
            try
            {
                HttpClient client = new HttpClient
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

                DownloadFlags(countries);

                await ConvertFlags(countries, progress);

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
        /// Converts flags from svg format to bitmap
        /// </summary>
        /// <param name="countries"></param>
        private async Task ConvertFlags(List<Country> countries, IProgress<ProgressReport> progress)
        {
            ProgressReport report = new ProgressReport();
            int i = 1;

            await Task.Run(() => {
                foreach (var country in countries)
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
                    catch(Exception e)
                    {
                        dialogService.ShowMessage("Error", $"An error occurred trying to convert {country.Name}'s flag\n{e.Message}");
                    }

                    report.CompletePercentage = Convert.ToByte((i * 100) / countries.Count);
                    progress.Report(report);
                    i++;
                }
            });

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
                catch(Exception e)
                {
                    dialogService.ShowMessage("Error", $"An error occurred trying to convert the default flag\n{e.Message}");
                }
            });
        }

        /// <summary>
        /// Downloads flags from the API in svg format
        /// </summary>
        /// <param name="countries"></param>
        private void DownloadFlags(List<Country> countries)
        {
            if (!Directory.Exists("Flags"))
            {
                Directory.CreateDirectory("Flags");
            }

            foreach (var country in countries)
            {
                try
                {
                    using (var flag = new WebClient())
                    {
                        flag.DownloadFileAsync(country.Flag, $@"Flags\{country.Name}.svg");
                    }
                }
                catch(Exception e)
                {
                    dialogService.ShowMessage("Error", e.Message);
                }
            }

            try
            {
                var noFlag = new WebClient();

                noFlag.DownloadFile("https://upload.wikimedia.org/wikipedia/commons/b/b0/No_flag.svg", $@"Flags\Default.svg");

                noFlag.Dispose();
            }
            catch(Exception e)
            {
                dialogService.ShowMessage("Error", $"Couldn't fetch the default flag\nmost likely the link no longer exists\n{e.Message}");
            }
        }

        #endregion
    }
}