using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;

namespace DotnetCoreConsoleAppSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("program started");

            ILoggerFactory loggerFactory = new LoggerFactory();
            loggerFactory.AddNLog().AddDebug();
            loggerFactory.ConfigureNLog("nlog.config"); 

            ILogger logger = loggerFactory.CreateLogger<Program>();
            logger.LogInformation("the program can log");

            logger.LogDebug(new EventId(0), new ArithmeticException("it's very complicated exception"), "thrown an exception elegantly");

            Console.WriteLine("program end");
            Console.ReadLine();
        }
    }
}