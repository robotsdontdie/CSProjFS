// Copied from https://github.com/microsoft/ProjFS-Managed-API and provided under the same license.

using CommandLine;
using Serilog;
using System;
using System.IO;

namespace SimpleProviderManaged
{
    public class Program
    {
        private enum ReturnCode
        {
            Success = 0,
            InvalidArguments = 1,
            GeneralException = 2,
        }
        
        public static int Main(string[] args)
        {
            try
            {
                var parser = new Parser(with =>
                    {
                        with.AutoHelp = true;
                        with.AutoVersion = true;
                        with.EnableDashDash = true;
                        with.CaseSensitive = false;
                        with.CaseInsensitiveEnumValues = true;
                        with.HelpWriter = Console.Out;
                    });
                    
                var parserResult = parser
                    .ParseArguments<ProviderOptions>(args)
                    .WithParsed((ProviderOptions options) =>
                    {
                        // We want verbose logging so we can see all our callback invocations.
                        var logConfig = new LoggerConfiguration()
                           .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss::ffff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                           .WriteTo.File("SimpleProviderManaged-.log", rollingInterval: RollingInterval.Day);

                        if (options.Verbose)
                        {
                            logConfig = logConfig.MinimumLevel.Verbose();
                        }

                        Log.Logger = logConfig.CreateLogger();

                        Log.Information("Start");
                        Run(options);
                    });

                Log.Information("Exit successfully");
                return (int) ReturnCode.Success;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Unexpected exception: {ex}");
                return (int)ReturnCode.GeneralException;
            }
        }

        private static void Run(ProviderOptions options)
        {
            SimpleProvider provider;
            try
            {
                //Directory.Delete(options.VirtRoot, true);
                provider = new SimpleProvider(options);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Failed to create SimpleProvider.");
                throw;
            }

            Log.Information("Starting provider");

            if (!provider.StartVirtualization())
            {
                Log.Error("Could not start provider.");
                Environment.Exit(1);
            }

            Console.WriteLine("Provider is running.  Press Enter to exit.");
            Console.ReadLine();
        }
    }
}
