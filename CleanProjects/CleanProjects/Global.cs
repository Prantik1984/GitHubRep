using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanProjects
{
    /// <summary>
    /// Holds the global variables
    /// </summary>
  internal static class Global
    {
      internal static string Log = "";//Log text
      internal static string ParseDir { get; set; }//Directory to parse
      internal static string LogFile { get; set; } //Log file 
    }
}
