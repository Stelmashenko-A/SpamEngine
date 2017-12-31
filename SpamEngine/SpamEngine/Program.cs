using System;
using Quartz.Logging;
using SpamEngine.Builders;

namespace SpamEngine
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());
            var spamEngineBuilder = new SpamEngineBuilder();
            spamEngineBuilder.Build().GetAwaiter().GetResult();

            Console.WriteLine("Press any key to close the application");
            Console.ReadKey();
        }

      
    }
}