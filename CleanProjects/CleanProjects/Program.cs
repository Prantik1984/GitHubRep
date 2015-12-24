using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CleanProjects.Operations;
namespace CleanProjects
{
    class Program
    {
        

        /// <summary>
        /// Methods of Execution 
        /// Source in GitHub1
        /// </summary>
        /// <param name="args"></param>
        #region Methods
            static void Main(string[] args)
            {
                ReadArgs(args);
                if(string.IsNullOrEmpty(Global.ParseDir))
                {
                    Global.ParseDir =new DirectoryInfo( Assembly.GetExecutingAssembly().Location).Parent.FullName;
                }
                if(string.IsNullOrEmpty(Global.LogFile))
                {
                    Global.LogFile = Path.Combine(Global.ParseDir,"cleanerlog.log");
                }
                if(DirectoryOps.CleanBinObj())
                {
                    #region remreadonly
                    if (DirectoryOps.RemoveReadOnlyFromDir())
                    {
                        #region delsrcfls
                        if (DirectoryOps.DeleteSourceFiles())
                        {
                            #region slnsrc
                            if (DirectoryOps.RemoveSlnSourceBinding())
                            {
                                #region prjsrc
                                if (DirectoryOps.RemoveProjectSourceBinding())
                                {
                                    MiscOps.AddToLog("Success", "");
                                }
                                else
                                {
                                    MiscOps.AddToLog("Fail", "RemoveProjectSourceBinding");
                                }
                                #endregion

                            }
                            else
                            {
                                MiscOps.AddToLog("Fail", "RemoveProjectSourceBinding");
                            }
                            #endregion
                        }
                        else
                        {
                            MiscOps.AddToLog("Fail", "DelSrcFls");
                        }
                        #endregion
                    }
                    else
                    {
                        MiscOps.AddToLog("Fail", "RemoveReadOnly");
                    } 
                    #endregion
                }
                else
                {
                    MiscOps.AddToLog("Fail", "CleanBinobj");
                }
                File.WriteAllText(Global.LogFile,Global.Log,Encoding.UTF8);
            }
        /// <summary>
        /// Reading command Line Arguments
        /// </summary>
            static void ReadArgs(string[] args)
            {
                foreach(var arg in args)
                {
                    if(arg.StartsWith("-d"))
                    {
                        Global.ParseDir = arg.Substring(2).Trim();
                    }

                    if(arg.StartsWith("-l"))
                    {
                        Global.LogFile = arg.Substring(2).Trim();
                    }
                }
            } 
       #endregion
    }
}
