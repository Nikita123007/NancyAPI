using System;

namespace NancyAPI
{
    public class NancyAPIExeption : Exception
    {
        public NancyAPIExeption(string message) : base(message) { }
    }
}
