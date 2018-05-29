using Akka.Actor;
using Akka.Configuration;
using NLog;
using NLog.Config;
using NLog.Targets;
using Topshelf;
using Topshelf.HostConfigurators;

namespace ConnelHooley.DirectoryBackupService.Shared
{
    internal static class Logging
    {
        static Logging()
        {
            var config = new LoggingConfiguration();
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, LogLevel.Fatal, new ConsoleTarget
            {
                Name = "console",
                Layout = "${longdate} | ${level:uppercase=true} | ${message:withException=true:exceptionSeparator= | }",
            }));
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, LogLevel.Fatal, new FileTarget
            {
                Name = "file",
                Layout = "${longdate} | ${level:uppercase=true} | ${logger} | ${message:withException=true:exceptionSeparator= | }",
                FileName = "log.txt"
            }));
            LogManager.Configuration = config;
        }
        
        public static void AddToTopShelf(HostConfigurator x) => 
            x.UseNLog(LogManager.LogFactory);

        public static Config GetAkkaLoggingConfig() => 
            ConfigurationFactory.ParseString(@"
                akka {
                    loggers = [""Akka.Logger.NLog.NLogLogger, Akka.Logger.NLog""])
                    loglevel = ""DEBUG""
                }");

        public static Akka.Event.ILoggingAdapter GetAkkaLogger(IActorContext context) =>
            Akka.Event.Logging.GetLogger(context);
    }
}
