using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanProjects.Operations
{
  internal static class MiscOps
    {
        /// <summary>
        /// Adding to Log
        /// </summary>
        /// <param name="header"></param>
        /// <param name="txt"></param>
        internal static void AddToLog(string header, string txt)
        {

            var hdrtxt = "";
            try
            {
                hdrtxt = StringResources.ResourceManager.GetString(header);
            }
            catch
            {
                hdrtxt = header;
            }

            var lgtext = string.Format("{0}:{1}", hdrtxt, txt);
            Global.Log += Environment.NewLine + lgtext;
        }

        /// <summary>
        /// Logs exception
        /// </summary>
        /// <param name="ex"></param>
        internal static void LogException(Exception ex)
        {
            AddToLog("Error", ex.Message);
        }
    }
}
