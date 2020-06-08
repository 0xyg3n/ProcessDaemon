//Author 0xyg3n
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ProcessDaemon
{


    #region Config
    public static class Config
    {
        //default payload for config creation calc.exe

        public static string PayLoad = Properties.Settings.Default.PayLoad; // define payload from user settings
        public static bool WillRun = Properties.Settings.Default.WillRun; // define state from user settings

        public static void SetIsRunningTrue() 
        {
            WillRun = Properties.Settings.Default.WillRun = true; //this will make sure that the payload will run once
            Properties.Settings.Default.Save(); //apply the changes to the config
        }
        public static void SetIsRunningFalse()
        {
            WillRun = Properties.Settings.Default.WillRun = false; //if this is set to false the payload will run
            Properties.Settings.Default.Save(); //apply the changes to the config
        }

    }
    #endregion
  
    #region Core
    public class Program
    {
        //this is for the window to be hidden just dont touch
        #region hidewindow
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0; // hide window
        const int SW_SHOW = 5; // show window
        #endregion

        static async Task Main()
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_SHOW); //hide application
            
            //method for process scanning
            void PScan()
            {
                try
                {
                    Process[] pname = Process.GetProcessesByName("WINWORD"); //target process
                    if (pname.Length == 0)
                    {
                        //if targeted process is not running
                        Config.SetIsRunningFalse();
                    }
                    else // if targeted process is running
                    {
                        if (!Config.WillRun) // if willrun = false
                        {
                            Process.Start(Config.PayLoad); // run the configured payload
                            Config.SetIsRunningTrue(); // set the state to true in order for it not to spam the payload 
                        }
                    }
                }
                catch { }
            }

            //run the task async in order to avoid the high cpu usage
            async Task IFloop()
            {
                try
                {
                    while (true) // infinite loop
                    {
                        await Task.Delay(5100); // loop the code with delay for low cpu usage.
                        PScan(); // call the process scan func

                        //Note that if the payload runs once and you close the targeted exe
                        //you need to wait 5100ms in order for it to set itself false again for it to be able to run the payload again.
                    }
                }
                catch { }
            }

            try
            {
                await IFloop(); //scan the processes infinitely.
            }
            catch { }
        }
    }
    #endregion
}