using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Fluent;
using LogLevel = NLog.LogLevel;

namespace eCommerce.Common
{

    public enum LoggingLevel
    {
        Info,
        Error
    }
    
    public class Logger
    {
        private static Logger _logger = new Logger("Log.log");
        
        private NLog.Logger _nlogger;
        private Semaphore _numberOfTasks;
        private ConcurrentQueue<Tuple<LoggingLevel, string>> _loggingQueue;

        Logger(string loggerFileName)
        {
            var config = new NLog.Config.LoggingConfiguration();
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = loggerFileName };
            var testLogfile = new NLog.Targets.FileTarget("testslog") { FileName = "TestsLog.log" };

            config.AddRule(LogLevel.Info, LogLevel.Error, logfile);
            config.AddRule(LogLevel.Info, LogLevel.Error, testLogfile);

            NLog.LogManager.Configuration = config;
            _nlogger = NLog.LogManager.GetCurrentClassLogger();

            _numberOfTasks = new Semaphore(0, 50);
            _loggingQueue = new ConcurrentQueue<Tuple<LoggingLevel, string>>();
            
            // Run the logging task
            new Task(() => LoggingTask()).Start();
        }

        public static void SetLoggerForTests()
        {
            _logger._nlogger = NLog.LogManager.GetLogger("testslog");
        }

        public static Logger GetLogger()
        {
            return _logger;
        }

        public void Log(LoggingLevel level, string message)
        {
            _loggingQueue.Enqueue(new Tuple<LoggingLevel, string>(level, message));
            _numberOfTasks.Release();
        }

        private void LoggingTask()
        {
            LogLevel currentLogLevel = LogLevel.Info;
            while (true)
            {
                _numberOfTasks.WaitOne();
                _loggingQueue.TryDequeue(out var loggingInfo);
                switch (loggingInfo.Item1)
                {
                    case LoggingLevel.Info:
                        currentLogLevel = LogLevel.Info;
                        break;
                    case LoggingLevel.Error:
                        currentLogLevel = LogLevel.Error;
                        break;
                }
                _nlogger.Log(currentLogLevel, loggingInfo.Item2);
                Console.WriteLine("log");
            }
        }
    }
}