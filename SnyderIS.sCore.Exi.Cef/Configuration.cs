using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xilium.CefGlue.Wrapper;
using Xilium.CefGlue;


namespace SnyderIS.sCore.Exi.Cef
{
    public class Configuration
    {

        public static string BrowseUrl
        {
            get
            {
                return System.Configuration.ConfigurationSettings.AppSettings["BrowserUrl"];
            }
        }

        public static string MessageUrl
        {
            get
            {
                return System.Configuration.ConfigurationSettings.AppSettings["MessageUrl"];
            }
        }

        public static int? MessageBoxYOverride
        {
            get
            {
                if (System.Configuration.ConfigurationSettings.AppSettings["MessageBoxYOverride"] != null)
                {
                    return int.Parse(System.Configuration.ConfigurationSettings.AppSettings["MessageBoxYOverride"]);
                }

                return null;
            }
        }

        public static CefLogSeverity CefLogSeverity
        {
            get
            {
                if (System.Configuration.ConfigurationSettings.AppSettings["CefLogSeverity"] != null)
                {
                    return (CefLogSeverity)System.Enum.Parse(typeof(CefLogSeverity), System.Configuration.ConfigurationSettings.AppSettings["CefLogSeverity"]);
                }
                else
                {
                    return CefLogSeverity.Default;
                }
            }
        }

        public static string CefLogPath
        {
            get
            {
                string path = string.Empty;

                if (System.Configuration.ConfigurationSettings.AppSettings["CefLogPath"] != null)
                {
                    path = System.Configuration.ConfigurationSettings.AppSettings["CefLogPath"];
                }

                return string.Concat(path, DateTime.Today.ToString("MM-dd-yyyy-"), "cef.log");
            }
        }

        public static double MessageOpacity
        {
            get
            {
                if (System.Configuration.ConfigurationSettings.AppSettings["MessageOpacity"] != null)
                {
                    return Convert.ToDouble(System.Configuration.ConfigurationSettings.AppSettings["MessageOpacity"]);
                }
                else
                {
                    return .5;
                }
            }
        }

        public static int MessageSpeed
        {
            get
            {
                if (System.Configuration.ConfigurationSettings.AppSettings["MessageSpeed"] != null)
                {
                    return Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["MessageSpeed"]);
                }
                else
                {
                    return 3;
                }
            }
        }

        public static int HeartBeatSeconds
        {
            get
            {
                if (System.Configuration.ConfigurationSettings.AppSettings["HeartBeatSeconds"] != null)
                {
                    return Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["HeartBeatSeconds"]);
                }
                else
                {
                    return 0;
                }
            }
        }

        public static string ShellLogPath
        {
            get
            {
                string path = string.Empty;

                if (System.Configuration.ConfigurationSettings.AppSettings["ShellLogPath"] != null)
                {
                    path = System.Configuration.ConfigurationSettings.AppSettings["ShellLogPath"];
                }

                return string.Concat(path, DateTime.Today.ToString("MM-dd-yyyy-"), "shell.log");
            }
        }

    }
}
