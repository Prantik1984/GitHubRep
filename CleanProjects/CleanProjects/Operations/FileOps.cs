using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanProjects.Operations
{
    /// <summary>
    /// Performs the various file operations
    /// </summary>
  internal static class FileOps
    {

        /// <summary>
        /// checks if a file exists
        /// </summary>
        /// <returns></returns>
        internal static bool CheckIfFileExists(string fl)
        {
            if (Path.IsPathRooted(fl))
            {
                if (File.Exists(fl))
                {
                    return true;
                }
            }
            return false;
        }
      /// <summary>
      /// Cleans a sln file of source control
      /// </summary>
      /// <param name="fl"></param>
      /// <returns></returns>
      internal static bool CleanSlnFl(string fl)
      {
          MiscOps.AddToLog("RemBindSln", fl);
          try
          {
              var strlns = File.ReadAllLines(fl,Encoding.UTF8);
              var tempstrlns = new List<string>();
              for(var i=0;i<strlns.Length;i++)
              {
                  if(!strlns[i].Trim().StartsWith("Scc"))
                  {
                      if(!strlns[i].Trim().StartsWith("GlobalSection")
                            &&
                          !strlns[i].Trim().StartsWith("EndGlobalSection")
                          )
                      {
                          tempstrlns.Add(strlns[i]);
                      }
                      else
                      {
                          if(strlns[i].Trim().StartsWith("GlobalSection"))
                          {
                              if(!strlns[i+1].Trim().StartsWith("Scc"))
                              {
                                  tempstrlns.Add(strlns[i]);
                              }
                          }
                          if(strlns[i].Trim().StartsWith("EndGlobalSection"))
                          {
                              if(!strlns[i-1].Trim().StartsWith("Scc"))
                              {
                                  tempstrlns.Add(strlns[i]);
                              }
                          }
                      }
                  }

              }


              
              File.WriteAllLines(fl,tempstrlns.ToArray(),Encoding.UTF8);
          }
          catch(Exception ex)
          {
              MiscOps.LogException(ex);
              return false;
          }
          return true;
      }
      /// <summary>
      /// Deletes file
      /// </summary>
      /// <param name="fl"></param>
      /// <returns></returns>
     internal static bool DeleteFile(string fl)
      {
          MiscOps.AddToLog("DelFl",fl);
         if(!RemoveReadonly(fl))
         {
             return false;
         }
         try
         {
             File.Delete(fl);
         }
         catch(Exception ex)
         {
             MiscOps.LogException(ex);
             return false;
         }
         return true;
      }
      /// <summary>
      /// removes the readonly from files
      /// </summary>
      /// <param name="fl"></param>
      internal static bool RemoveReadonly(string fl)
      {
          try
          {
              MiscOps.AddToLog("RemReadOnly", fl);
              FileAttributes attributes = File.GetAttributes(fl);
              File.SetAttributes(fl, attributes & ~FileAttributes.ReadOnly);
              return true;
          }
          catch (Exception ex)
          {
              MiscOps.LogException(ex);
              return false;
          }

      }
    }
}
