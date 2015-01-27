using System;
using System.Collections.Generic;
using Pen.Service.App;
using System.Windows.Forms;

namespace Pen.Service.App
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());            
            //Application.Run(new Form1());

            //try
            //{            
            //Console.SetWindowSize(50, 20);
            //Console.Title = "Pen.Service.App";

            //Console.WriteLine("***** Pen.Service.App *****");
            //Console.WriteLine("Try starting Multitouch driver...");
            ////tracker = new Tracker();          
            ////tracker.StartProcessing();
           
            //Console.WriteLine("Multitouch driver is running.\nPress ENTER to stop and exit.");
            //Console.ReadLine();
            ////tracker.StopProcessing();
            ////tracker.Dispose();
            //Console.WriteLine("Stopping service...");
            //Console.WriteLine("Service stopped.");           
            //}
            //catch (Exception e)
            //{
            //    Console.ForegroundColor = ConsoleColor.Red;
            //    Console.WriteLine(e.Message);
            //    Console.ResetColor();
            //    Console.ReadLine();             
            //}                       
        }
    }
}
