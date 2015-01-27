using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Pen.Service.App
{
    public partial class Form1 : Form
    {
        Timer timer1 = new Timer();

        public Form1()
        {
            InitializeComponent();
            timer1.Tick += new EventHandler((object sender, EventArgs e) =>
                {
                    if (this.Opacity < 1)
                        // fadeIn
                        for (double i = 0.25; i < 1.05; i += 0.05)
                        {
                            System.Threading.Thread.Sleep(10);
                            this.Opacity = i;                            
                            Application.DoEvents();                            
                        }
                    else
                        // fadeOut
                        for (double i = 1; i > 0.2; i -= 0.05)
                        {
                            System.Threading.Thread.Sleep(20);
                            this.Opacity = i;                            
                            Application.DoEvents();                            
                        }

                    timer1.Stop();
                });
            timer1.Interval = 3000;
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            //dataSetLessonFiles.Tables["LessonFile"]
            timer1.Stop();
            timer1.Start();            
        }
    }
}
