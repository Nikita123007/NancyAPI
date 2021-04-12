using NancyAPI.Core.Models;
using NancyAPI.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace NancyAPI.Core.Services
{
    public class ArticlesSourceService : IArticlesSourceService
    {
        public async Task<List<ArticleSource>> GetData(string section = null)
        {
            var content = await GetContent(section ?? Config.HomeSection);
            if (TryParse<DataResponse>(content, out var dataResponse))
            {
                return dataResponse.Results;
            }
            throw new NancyAPICoreExeption("Invalid data format");
        }

        private async Task<string> GetContent(string section)
        {
            try
            {
                var request = WebRequest.Create(GetUrl(section));
                using (var stream = request.GetResponse().GetResponseStream())
                using (var streamReader = new StreamReader(stream))
                {
                    return await streamReader.ReadToEndAsync();
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
                {
                    throw new NancyAPICoreExeption(errorResponse.Fault.FaultString);
                }
                else
                {
                    throw new NancyAPICoreExeption(errorContent);
                }
            }
        }

        private string GetUrl(string section) => string.Format(Config.UrlTemplate, section, Config.Key);

        private bool TryParse<T>(string content, out T data)
        {
            try
            {
                data = Converter.FromJson<T>(content);
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
