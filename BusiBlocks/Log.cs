using System;
using log4net;

namespace BusiBlocks
{
    /// <summary>
    /// Log helper class using log4net.
    /// Configure log4net to enable logging before using these methods.
    /// </summary>
    public static class Log
    {
        public static void Error(Type context, string message)
        {
            ILog log = LogManager.GetLogger(context);
            log.Error(message);
        }

        public static void Error(Type context, string message, Exception exception)
        {
            ILog log = LogManager.GetLogger(context);
            log.Error(message, exception);
        }

        public static void Warning(Type context, string message, Exception exception)
        {
            ILog log = LogManager.GetLogger(context);
            log.Warn(message, exception);
        }

        public static void Debug(Type context, string message)
        {
            ILog log = LogManager.GetLogger(context);
            log.Debug(message);
        }
    }
}