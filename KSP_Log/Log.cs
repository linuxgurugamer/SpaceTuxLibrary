///
/// Define Log as a internal or public static in a single class
/// Make sure it is initialized before first use
/// Use the "using static" directive to remove the need to reference the entire class path:
/// 
/// 
/// using static MyNameSpace.MyClass;
/// 
/// [KSPAddon(KSPAddon.Startup.Instantly, true)]
/// public class InitLog : MonoBehaviour
/// {
///     protected void Awake()
///     {
///         Log = new KSP_Log.Log("DangIt"
/// #if DEBUG
///                 , KSP_Log.Log.LEVEL.INFO
/// #endif
///                 );
///     }
/// }
/// 
/// ===========================
/// 
/// using KSP_Log;
/// namespace MyNameSpace
/// {
///     public class MyClass
///     {
///         public static KSP_Log.Log Log;
///     }
/// }
/// ===========================
/// 
/// using static MyNameSpace.MyClass;
/// Class SecondClass
/// {
///     void Start()
///     {
///         Log.Info("Start");
///         
using System;

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
        /// Initialize the class and set the level
        /// </summary>
        /// <param name="title"></param>
        /// <param name="level"></param>
        public Log(string title, LEVEL level)
        {
            setTitle(title);
            SetLevel(level);
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
        public LEVEL level = LEVEL.ERROR;


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
            this.level = level;
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
            return this.level == level;
        }

        /// <summary>
        /// Returns true if the specified level is greaterthan or equal the the log level
        /// </summary>
        /// <param name="level"></param>
        /// <returns>True if logable</returns>
        public bool IsLogable(LEVEL level)
        {
            return level <= this.level;
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
        /// Logs at a DETAIL level
        /// </summary>
        /// <param name="msg"></param>
        public void Debug(string msg) => Detail(msg);

        /// <summary>
        /// Logs at an INFO level.  If not compiled in DEBUG mode, this is compiled away by the compiler and does not log anything
        /// </summary>
        /// <param name="msg"></param>
        //[ConditionalAttribute("DEBUG")]
        public void Info(String msg)
        {
            if (IsLogable(LEVEL.INFO))
            {
                UnityEngine.Debug.Log(PREFIX + msg);
            }
        }

        /// <summary>
        /// Logs at an info level, this variant allows message formatting
        /// </summary>
        /// <param name="messageOrFormat"></param>
        /// <param name="args"></param>
        public  void Info(object messageOrFormat, params object[] args)
        {
            Info(GetLogMessage(messageOrFormat, args));
        }

        /// <summary>
        /// Logs at an error level, this variant allows message formatting
        /// </summary>
        /// <param name="messageOrFormat"></param>
        /// <param name="args"></param>
        public void Error(object messageOrFormat, params object[] args)
        {
            Error(GetLogMessage(messageOrFormat, args));
        }

        /// <summary>
        /// Logs at an warn level, this variant allows message formatting
        /// </summary>
        /// <param name="messageOrFormat"></param>
        /// <param name="args"></param>
        public void Warn(object messageOrFormat, params object[] args)
        {
            Warn(GetLogMessage(messageOrFormat, args));
        }


        /// <summary>
        /// Logs at any level.  If not compiled in DEBUG mode, this is compiled away by the compiler and does not log anything
        /// </summary>
        /// <param name="msg"></param>
        //[ConditionalAttribute("DEBUG")]
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
        /// Logs exception
        /// </summary>
        /// <param name="exception"></param>
        public void Exception(Exception exception)
        {
            Error("exception caught: " + exception.GetType() + ": " + exception.Message);
        }

        public void Error(Exception exception) => Exception(exception);

        /// <summary>
        /// Log exception
        /// </summary>
        /// <param name="name"></param>
        /// <param name="exception"></param>
        public void Exception(string name, Exception exception)
        {
            Error(name + " exception caught: " + exception.GetType() + ": " + exception.Message);
        }

        private static string GetLogMessage(object messageOrFormat, object[] args)
        {
            string message = messageOrFormat.ToString();
            if (args != null && args.Length > 0)
            {
                message = String.Format(message, args);
            }
            return String.Format("[BetterLoadSaveGame] {0}", message);
        }

    }
}
