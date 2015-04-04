namespace SnyderIS.sCore.Exi.Kiosk
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Gtk;


    internal static class Program
    {
        public static bool Recycle = false;

        [STAThread]
        private static int Main(string[] args)
        {

            int retVal = 0;
          
            using (var application = new KioskAppImpl())
            {
                retVal = application.Run(args);              
            }

            if (Program.Recycle)
            {
                return 999;
            }

            return retVal;
        }
    }
}
