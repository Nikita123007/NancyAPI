﻿using NancyAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace NancyAPI.Services
{
    public class ArticlesSourceService
    {
        public List<ArticleSource> GetData(string section = null)
        {
            var content = GetContent(section ?? Config.HomeSection);
            if (TryParse<DataResponse>(content, out var dataResponse))
            {
                if (dataResponse.Fault == null)
                {
                    return dataResponse.Results;
                }
                throw new NancyAPIExeption(dataResponse.Fault.FaultString);
            }
            throw new NancyAPIExeption("Invalid data format");
        }

        private string GetContent(string section)
        {
            try
            {
                var request = WebRequest.Create(GetUrl(section));
                using (var stream = request.GetResponse().GetResponseStream())
                using (var streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                string errorContent;
                using (var streamReader = new StreamReader(ex.Response.GetResponseStream()))
                {
                    errorContent = streamReader.ReadToEnd();
                }
                if (TryParse<ErrorResponse>(errorContent, out var errorResponse))
                    throw new NancyAPIExeption(errorResponse.Fault.FaultString);
                else
                    throw new NancyAPIExeption(errorContent);
            }
        }

        private string GetUrl(string section) => string.Format(Config.UrlTemplate, section, Config.Key);

        private bool TryParse<T>(string content, out T data)
        {
            try
            {
                data = JsonConvert.DeserializeObject<T>(content);
                return true;
            }
            catch (Exception)
            {
                data = default(T);
                return false;
            }
        }
    }
}
