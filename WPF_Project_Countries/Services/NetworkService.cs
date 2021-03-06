﻿namespace WPF_Project_Countries.Services
{
    #region Usings

    using System.Net;
    using Library.Models;

    #endregion

    public class NetworkService
    {
        #region Methods (alphabetical order)

        /// <summary>
        /// Checks if the computer has internet connection
        /// </summary>
        /// <returns>Response object</returns>
        public Response CheckConnection()
        {
            var client = new WebClient();

            try
            {
                using (client.OpenRead("https://clients3.google.com/generate_204"))
                {
                    return new Response
                    {
                        IsSuccess = true,
                        Message = "Internet connection successful"
                    };
                }
            }
            catch
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = "Internet connection failed"
                };
            }
        }

        #endregion
    }
}