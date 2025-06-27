using log4net.Config;
using log4net.Repository;
using log4net;
using System;
using System.Diagnostics;
using System.IO;

namespace EasyTemplate.Desktop.Wpf.Common
{
    public class Log
    {
        private static ILog log { get; set; }
        private static ILog GetLog()
        {
            if (log is null)
            {
                ILoggerRepository repository = LogManager.CreateRepository("LogRepository");
                XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));
                log = LogManager.GetLogger("LogRepository", typeof(Log));
            }
            return log;
        }

        public static void Debug(string message)
        {
            var frame = new StackFrame(1);
            var body = $"[{DateTime.Now.ToString("O")}] - {frame.GetMethod().DeclaringType.FullName} ：{message}";
            GetLog().Debug(body);
        }

        public static void Info(string message)
        {
            var frame = new StackFrame(1);
            var body = $"[{DateTime.Now.ToString("O")}] - {frame.GetMethod().DeclaringType.FullName} ：{message}";
            GetLog().Info(body);
        }

        public static void Warn(string message)
        {
            var frame = new StackFrame(1);
            var body = $"[{DateTime.Now.ToString("O")}] - {frame.GetMethod().DeclaringType.FullName} ：{message}";
            GetLog().Warn(body);
        }

        public static void Error(string message)
        {
            var frame = new StackFrame(1);
            var body = $"[{DateTime.Now.ToString("O")}] - {frame.GetMethod().DeclaringType.FullName} ：{message}";
            GetLog().Error(body);
        }

    }
}
