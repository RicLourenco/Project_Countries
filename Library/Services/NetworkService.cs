﻿namespace Library.Services
{
    using System.Net;
    using Models;

    public class NetworkService
    {
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
    }
}