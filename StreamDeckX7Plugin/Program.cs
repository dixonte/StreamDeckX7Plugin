using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Settings.Configuration;
using StreamDeckLib;
using StreamDeckX7Plugin.SBX7;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace StreamDeckX7Plugin
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            SBClient.Singleton.Enabled = true;

            using (var config = StreamDeckLib.Config.ConfigurationBuilder.BuildDefaultConfiguration(args))
            {
                await ConnectionManager.Initialize(args, config.LoggerFactory)
                                                             .RegisterAllActions(typeof(Program).Assembly)
                                                             .StartAsync();
            }

            SBClient.Singleton.Enabled = false;
        }
    }
}
