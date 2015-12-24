using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
namespace CleanProjects.Operations
{
    /// <summary>
    /// Performs the various directory operations
    /// </summary>
  internal static  class DirectoryOps
  {
      #region methods

      /// <summary>
      /// Checks if a directory exists
      /// </summary>
      /// <param name="dir"></param>
      /// <returns></returns>
      internal static bool CheckIfDirExists(string dir)
      {
          MiscOps.AddToLog("ChkdirExist",dir);
          try
          {
              if (Path.IsPathRooted(dir))
              {
                  if (Directory.Exists(dir))
                  {
                      MiscOps.AddToLog("DirExists",true.ToString());
                      return true;
                  }
                  else
                  {
                      MiscOps.AddToLog("DirNotExist",dir);
                      return false;
                  }
              }
              else
              {
                  MiscOps.AddToLog("PthNtRooted",dir);
                  return false;
              }
          }
          catch (Exception ex)
          {
              MiscOps.LogException(ex);

          }
          return false;
      }
       /// <summary>
       /// Deletes Bin/obj
       /// </summary>
       /// <param name="dir"></param>
       /// <returns></returns>
       internal static bool CleanBinObj()
        {
           if(!CheckIfDirExists(Global.ParseDir))
           {
               return false;
           }
            var dirstodelete = new List<string>();
           try
           {
                foreach(var d in Directory.GetDirectories(Global.ParseDir,"*.*",SearchOption.AllDirectories)
                .Where(dnm=>new DirectoryInfo(dnm).Name=="bin"
                ||
                new DirectoryInfo(dnm).Name == "obj"
                )
                )
                {
                    dirstodelete.Add(d);
                }
                foreach (var d in dirstodelete)
                {
                    if (!DeleteDirectory(d))
                    {
                        return false;
                    }
                }
           }
           catch(Exception ex)
           {
               MiscOps.LogException(ex);
               return false;
           }
           
          
          
            return true;
        }

       /// <summary>
       /// Deleting directories
       /// </summary>
       /// <param name="dir"></param>
       internal static bool DeleteDirectory(string dir)
       {
           MiscOps.AddToLog("DelDir", dir);
           try
           {
               if (!RenameDirectoryForDelete(dir))
               {
                   return false;
               }
               Directory.Delete(dir, true);

           }
           catch (Exception ex)
           {
               MiscOps.AddToLog("Error", ex.Message);
               return false;
           }
           return true;
       }

       /// <summary>
       /// renaming directories for deletion
       /// </summary>
       /// <param name="dir"></param>
       static bool RenameDirectoryForDelete(string dir)
       {
           var cnt = 0;
           foreach (var fl in Directory.GetFiles(dir, "*.*", SearchOption.TopDirectoryOnly))
           {
               if (!FileOps.RemoveReadonly(fl))
               {
                   return false;
               }
               try
               {
                   MiscOps.AddToLog("MoveFl", fl);
                   File.Move(fl, Path.Combine(dir, cnt.ToString() + ".fl"));
               }
               catch (Exception ex)
               {
                   MiscOps.LogException(ex);
                   return false;
               }
               cnt++;
           }
           cnt = 0;
           foreach (var d in Directory.GetDirectories(dir, "*.*", SearchOption.TopDirectoryOnly))
           {
               try
               {
                   MiscOps.AddToLog("MoveDir", dir);
                   Directory.Move(d, Path.Combine(dir, cnt.ToString()));
               }
               catch (Exception ex)
               {
                   MiscOps.LogException(ex);
                   return false;
               }

               cnt++;
           }
           foreach (var d in Directory.GetDirectories(dir, "*.*", SearchOption.TopDirectoryOnly))
           {
               if (!RenameDirectoryForDelete(d))
               {
                   return false;
               }
           }
           return true;
       }
       
      /// <summary>
       /// removes readonlyfrom files in directory
       /// </summary>
       /// <returns></returns>
       internal static bool RemoveReadOnlyFromDir()
       {
           MiscOps.AddToLog("RemReadOnly", Global.ParseDir);
           if(!CheckIfDirExists(Global.ParseDir))
           {
               return false;
           }
           foreach(var fl in Directory.GetFiles(Global.ParseDir,"*.*",SearchOption.AllDirectories))
           {
               if (!FileOps.RemoveReadonly(fl))
               {
                   return false;
               }
           }
           return true;
       }
       
      /// <summary>
      /// deleting source files
      /// </summary>
      /// <returns></returns>
       internal static bool DeleteSourceFiles()
       {
           MiscOps.AddToLog("RemSourceFls", Global.ParseDir);
           if (!CheckIfDirExists(Global.ParseDir))
           {
               return false;
           }
           List<string> srcfls = new List<string>();
           foreach(var fl in Directory.GetFiles(Global.ParseDir,"*.*",SearchOption.AllDirectories)
               .Where(f=>new FileInfo(f).Extension==".suo"
                       ||
                      new FileInfo(f).Extension==".vssscc"
                       ||
                      new FileInfo(f).Extension==".user"
                       ||
                      new FileInfo(f).Extension==".vspscc"
               ) 
               )
           {
               srcfls.Add(fl);
           }
           foreach(var fl in srcfls)
           {
               if (!FileOps.DeleteFile(fl))
               {
                   return false;
               }
               
           }
           return true;
       }
       
      /// <summary>
      /// Removes source bindings from solution files
      /// </summary>
      /// <returns></returns>
       internal static bool RemoveSlnSourceBinding()
       {
           MiscOps.AddToLog("RemBindSln", Global.ParseDir);
           if (!CheckIfDirExists(Global.ParseDir))
           {
               return false;
           }
           List<string> slnfls = new List<string>();
           foreach(var fl in Directory.GetFiles(Global.ParseDir,"*.sln",SearchOption.AllDirectories))
           {
               slnfls.Add(fl);
           }
           foreach(var fl in slnfls)
           {
               if(!FileOps.CleanSlnFl(fl))
               {
                   return false;
               }
               
           }
           return true;
       }

       /// <summary>
       /// Removes source bindings from project files
       /// </summary>
       /// <returns></returns>
       internal static bool RemoveProjectSourceBinding()
       {
           MiscOps.AddToLog("RemBindPrj", Global.ParseDir);
           if(!CheckIfDirExists(Global.ParseDir))
           {
               return false;
           }
           
           var prjfls=new List<string>();
           foreach(var fl in Directory.GetFiles(Global.ParseDir,"*.*proj",SearchOption.AllDirectories))
           {
               prjfls.Add(fl);
           }
           try
           {
               foreach (var fl in prjfls)
               {
                   MiscOps.AddToLog("RemBindPrj", fl);
                   var doc = XDocument.Parse(File.ReadAllText(fl, Encoding.UTF8));
                   var nmpc = doc.Root.GetDefaultNamespace();
                   var nds = doc.Root.Descendants().Where(nd => nd.Value == "SAK");
                   List<string> ndnms = new List<string>();
                   if (nds.Any())
                   {
                       foreach (var nd in nds)
                       {
                           var nm = nd.Name.LocalName;
                           ndnms.Add(nm);

                       }
                       foreach (var ndnm in ndnms)
                       {
                           var t = doc.Root.Descendants(nmpc.GetName(ndnm));
                           t.Remove();

                       }

                   }
                   doc.Save(fl);
               }
           }
           catch(Exception ex)
           {
               MiscOps.LogException(ex);
               return false;
           }
         
           return true;
       }
      #endregion
  }
}
