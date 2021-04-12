using Microsoft.Extensions.Logging;
using System;

namespace NancyAPI.Utils
{
    public static class Logger
    {
        private static ILogger m_Logger { get; set; } 

        public static void Configure(ILogger logger)
        {
            m_Logger = logger;
        }

        public static void Log(string message)
        {
            m_Logger.LogError(message);
        }

        public static void Log(Exception ex)
        {
            m_Logger.LogError(ex, ex.Message);
        }
    }
}
