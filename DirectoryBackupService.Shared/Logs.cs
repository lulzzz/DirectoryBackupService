using Akka.Actor;
using Akka.Configuration;
using Akka.Event;
using NLog;
using NLog.Config;
using NLog.Targets;
using Topshelf;
using Topshelf.HostConfigurators;

namespace ConnelHooley.DirectoryBackupService.Shared
{
    internal static class Logs
    {
        private static readonly LogFactory LogFactory;

        static Logs()
        {
            var config = new LoggingConfiguration();
            config.LoggingRules.Add(new LoggingRule("*", NLog.LogLevel.Trace, NLog.LogLevel.Fatal, new ConsoleTarget
            {
                Name = "console",
                Layout = "${longdate} | ${level:uppercase=true} | ${message:withException=true:exceptionSeparator= | }",
            }));
            config.LoggingRules.Add(new LoggingRule("*", NLog.LogLevel.Trace, NLog.LogLevel.Fatal, new FileTarget
            {
                Name = "file",
                Layout = "${longdate} | ${level:uppercase=true} | ${logger} | ${message:withException=true:exceptionSeparator= | }",
                FileName = "log.txt"
            }));
            LogManager.Configuration = config;
            LogFactory = LogManager.LogFactory;
        }
        
        public static void AddToTopShelf(HostConfigurator x) => 
            x.UseNLog(LogFactory);

        public static Config GetAkkaConfig() => 
            ConfigurationFactory.ParseString(@"
                akka {
                    loggers = [""Akka.Logger.NLog.NLogLogger, Akka.Logger.NLog""])
                    loglevel = ""DEBUG""
                }");

        public static ILoggingAdapter GetAkkaLogger(IActorContext context) => 
            Akka.Event.Logging.GetLogger(context);
    }
}
