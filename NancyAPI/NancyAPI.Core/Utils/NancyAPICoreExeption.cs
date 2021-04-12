using System;

namespace NancyAPI.Core.Utils
{
    public class NancyAPICoreExeption : Exception
    {
        public NancyAPICoreExeption(string message) : base(message) { }
    }
}
