using System;

namespace NancyAPI.Core
{
    public class NancyAPICoreExeption : Exception
    {
        public NancyAPICoreExeption(string message) : base(message) { }
    }
}
