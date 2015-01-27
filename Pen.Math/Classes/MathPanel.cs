using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using micautLib;
using System.Threading;
using System.Windows;

namespace Pen.Math
{
    /// <summary>
    /// Gives you a control on the Math Input Panel of Windows    
    /// Also send the recognized equations to Word, for more manipulation
    /// </summary>    
    class MathPanel
    {   
        public MathInputControl mathPanel;
        // Top and Buttom
        private int panelHeight = 300;        
        private int panelTop = -30;
        private int panelBottom;
        // width and Right
        private int panelWidth = 345;
        private int panelLeft = 468;
        private int panelRight;
        // sliding speed of the panel
        private int speed = 2;

        public MathPanel()
        {
            panelRight = panelLeft + panelWidth;
            panelBottom = panelTop + panelHeight;

            // initialize math panel
            mathPanel = new MathInputControl();            
            // set some properties
            mathPanel.EnableExtendedButtons(false);
            mathPanel.EnableAutoGrow(false);
            mathPanel.SetCaptionText("Math Input");            
            mathPanel.Insert += new _IMathInputControlEvents_InsertEventHandler(mathPanel_Insert);
            mathPanel.Close += new _IMathInputControlEvents_CloseEventHandler(mathPanel_Close);
            mathPanel.SetPosition(panelLeft, panelTop - panelHeight, panelRight, panelTop);
            mathPanel.Show();
        }
       
        // show math input panel with a slide effect
        public void MathPanelShow()
        {
            Thread mathPanelShowThread = new Thread(() => ShowThread());
            mathPanelShowThread.Start();
        }

        // show math input panel with a slide effect
        public void HideMathPanel()
        {
            Thread mathPanelHideThread = new Thread(() => HideThread());
            mathPanelHideThread.Start();
        }

        // method of mathPanelShowThread
        private void ShowThread()
        {
            int top = panelTop - panelHeight;
            int bottom = panelTop;
            mathPanel.SetPosition(panelLeft, top, panelRight, bottom);
            mathPanel.Show();
            while(top < panelTop)
            {
                //Thread.Sleep(speed);
                top += 10;
                bottom = panelHeight + top;
                mathPanel.SetPosition(panelLeft, top, panelRight, bottom);
            }
        }

        // method of mathPanelHideThread
        private void HideThread()
        {
            int top = panelTop;
            int bottom = panelTop;
            mathPanel.SetPosition(panelLeft, top, panelRight, bottom);

            while (top > panelTop - panelHeight)
            {
                Thread.Sleep(speed);
                top -= 10;
                bottom = panelHeight + top;
                mathPanel.SetPosition(panelLeft, top, panelRight, bottom);                
            }
            // in fact, hiding math input panel is so important so as to return focus back
            // to the main window/application which called this input panel
            mathPanel.Hide();
        }

        // insert/return recognized equation when user press insert button        
        private void mathPanel_Insert(string RecoResult)
        {
            // call Office Word            
            HideMathPanel();
            Clipboard.SetText(RecoResult);
            // clear panel
            //mathPanel.Clear();
        }

        // hide math panel if user clicked cancel button
        private void mathPanel_Close()
        {
            HideMathPanel();
        }
                
        // dispose
        public void Dispose()
        {
            if (mathPanel != null)
                mathPanel.Hide();
        }
    }
}
