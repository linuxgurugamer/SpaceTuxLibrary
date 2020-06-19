using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace KSP_Log

{
    /// <summary>
    /// Logging class
    /// </summary>
    public class Log
    {
        /// <summary>
        /// Log level
        /// </summary>
        public enum LEVEL
        {
            OFF = 0,
            ERROR = 1,
            WARNING = 2,
            INFO = 3,
            DETAIL = 4,
            TRACE = 5
        };
        string PREFIX = "";

        /// <summary>
        /// Used to initialize the class. 
        /// </summary>
        /// <param name="title">Title to be displayed in the log file as the prefix to a line</param>
        public Log(string title)
        {
            setTitle(title);
        }

        /// <summary>
        /// Sets the title
        /// </summary>
        /// <param name="title">Title to be displayed in the log file as the prefix to a line</param>
        public void setTitle(string title)
        {
            PREFIX = title + ": ";
        }

        /// <summary>
        /// Current log level
        /// </summary>
        public static LEVEL level = LEVEL.INFO;


        /// <summary>
        /// Returns the current log level
        /// </summary>
        /// <returns>LEVEL</returns>
        public LEVEL GetLevel()
        {
            return level;
        }

        /// <summary>
        /// Sets the current log level
        /// </summary>
        /// <param name="level"></param>
        public void SetLevel(LEVEL level)
        {
            UnityEngine.Debug.Log("log level " + level);
            Log.level = level;
        }

        /// <summary>
        /// Returns the current log level
        /// </summary>
        /// <returns>Current loglevel</returns>
        public LEVEL GetLogLevel()
        {
            return level;
        }

        private bool IsLevel(LEVEL level)
        {
            return Log.level == level;
        }

        /// <summary>
        /// Returns true if the specified level is greaterthan or equal the the log level
        /// </summary>
        /// <param name="level"></param>
        /// <returns>True if logable</returns>
        public bool IsLogable(LEVEL level)
        {
            return Log.level <= level;
        }

        /// <summary>
        /// Logs at a TRACE level
        /// </summary>
        /// <param name="msg"></param>
        public void Trace(String msg)
        {
            if (IsLogable(LEVEL.TRACE))
            {
                UnityEngine.Debug.Log(PREFIX + msg);
            }
        }

        /// <summary>
        /// Logs at a DETAIL level
        /// </summary>
        /// <param name="msg"></param>
        public void Detail(String msg)
        {
            if (IsLogable(LEVEL.DETAIL))
            {
                UnityEngine.Debug.Log(PREFIX + msg);
            }
        }

        /// <summary>
        /// Logs at an INFO level.  If not compiled in DEBUG mode, this is compiled away by the compiler and does not log anything
        /// </summary>
        /// <param name="msg"></param>
        [ConditionalAttribute("DEBUG")]
        public void Info(String msg)
        {

            if (IsLogable(LEVEL.INFO))

            {
                UnityEngine.Debug.Log(PREFIX + msg);
            }
        }

        /// <summary>
        /// Logs at any level.  If not compiled in DEBUG mode, this is compiled away by the compiler and does not log anything
        /// </summary>
        /// <param name="msg"></param>
        [ConditionalAttribute("DEBUG")]
        public void Test(String msg)
        {
            //if (IsLogable(LEVEL.INFO))

            {
                UnityEngine.Debug.LogWarning(PREFIX + "TEST:" + msg);
            }
        }

        /// <summary>
        /// Logs at a WARNING level
        /// </summary>
        /// <param name="msg"></param>
        public void Warning(String msg)
        {
            if (IsLogable(LEVEL.WARNING))
            {
                UnityEngine.Debug.LogWarning(PREFIX + msg);
            }
        }

        /// <summary>
        /// Logs at an ERROR level
        /// </summary>
        /// <param name="msg"></param>
        public void Error(String msg)
        {
            if (IsLogable(LEVEL.ERROR))
            {
                UnityEngine.Debug.LogError(PREFIX + msg);
            }
        }

        /// <summary>
        /// Logs exceptiopn
        /// </summary>
        /// <param name="exception"></param>
        public void Exception(Exception exception)
        {
            Error("exception caught: " + exception.GetType() + ": " + exception.Message);
        }

    }
}
