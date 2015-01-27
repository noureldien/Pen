using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Pen.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //System.Diagnostics.Process.Start(Environment.CurrentDirectory + "\\Pen.Driver.exe");
            
            //Console.SetWindowPosition(500, 500);
            //Console.SetBufferSize(15, 15);
            //Console.WriteLine(Console.WindowTop.ToString());           
            //Console.WriteLine(Console.WindowWidth.ToString());

            //Console.SetWindowSize(17, 1);
            //Console.SetBufferSize(17, 1);
            //Console.Title = "S";
            //Console.SetWindowPosition(100, 100);
                                   
            //Console.SetWindowPosition(0, 0);
            //Console.MoveBufferArea(0, 0, 15, 1, 0, 0);
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            //Application.Run(new ControlForm());
            //Application.Run(new VirtualCursor());
        }
    }
}
