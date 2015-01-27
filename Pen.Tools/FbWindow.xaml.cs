using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Diagnostics;
using System.Threading;
using System.Windows.Ink;

namespace Pen.Tools
{
	/// <summary>
	/// Interaction logic for Window1.xaml
    /// This Window helps the teacher to post on facebook. He can write a comment on a certain
    /// student, gives a rate for him and send all of this as a post on this student' wall 
    /// on facebook. This is done with the help of Pen Facebook Application.
	/// </summary>
    public partial class FbWindow : Window
    {
        public FbWindow()
        {            
            InitializeComponent();            
        }

        // load some setting when form is loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // cnovert this window to a glassy window
            GlassyWindow.LoadGlassyWindow(this);
            InkCanvasSettings();
            inkCanvas1.StrokeCollected += new InkCanvasStrokeCollectedEventHandler(inkCanvas1_StrokeCollected);
            inkAnalyzer.ResultsUpdated += new ResultsUpdatedEventHandler(inkAnalyzer_ResultsUpdated);
        }

        // dispose all objects
        private void windowMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (inkAnalyzer.IsAnalyzing)
                inkAnalyzer.Abort();
            inkAnalyzer.Dispose();
        }

        // defalut drawing seetings of inkCanvas1
        private void InkCanvasSettings()
        {
            //inkCanvas1.DefaultDrawingAttributes.Color = Colors.Orange;
            inkCanvas1.DefaultDrawingAttributes.Width = inkCanvas1.DefaultDrawingAttributes.Height = 2;
            inkCanvas1.DefaultDrawingAttributes.IgnorePressure = true;
            inkCanvas1.DefaultDrawingAttributes.Color = Colors.White;
            //inkCanvas1.DefaultDrawingAttributes.
        }
        
        #region show and hide window with animation
        public void ShowWindow()
        {
            this.Visibility = System.Windows.Visibility.Visible;
            AnimateWindowOpacity(0, 1);
        }

        public void HideWindow()
        {
            AnimateWindowOpacity(1, 0);
        }

        private void AnimateWindowOpacity(int from, int to)
        {
            DoubleAnimation animation = new DoubleAnimation();
            EasingFunctionBase easing = new SineEase();
            easing.EasingMode = EasingMode.EaseIn;

            animation.From = from;
            animation.To = to;
            animation.By = 1;
            //animation.AccelerationRatio = 0;
            //animation.DecelerationRatio = 1;
            animation.EasingFunction = easing;
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(1000));

            Storyboard storyBoard = new Storyboard();
            Storyboard.SetTargetName(animation, this.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Window.OpacityProperty));
            storyBoard.Children.Add(animation);
            if (from > to)
                storyBoard.Completed += new EventHandler(storyBoard_Completed);
            storyBoard.Begin(this);
        }

        void storyBoard_Completed(object sender, EventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Collapsed;
        }
        #endregion
        
        #region hand writing recognition by inkAnalyser
        InkAnalyzer inkAnalyzer = new InkAnalyzer();                 
        // analyze
        void inkCanvas1_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            // background analyzing           
            inkAnalyzer.AddStroke(e.Stroke);
            inkAnalyzer.BackgroundAnalyze();

            // foreground Analyzing
            //analysisStatus = inkAnalyzer.Analyze();
            //if (analysisStatus.Successful == true)
            //    textBox1.Text = inkAnalyzer.GetRecognizedString();
        }

        private void inkAnalyzer_ResultsUpdated(object sender, ResultsUpdatedEventArgs e)
        {   
            textBlock1.Text = inkAnalyzer.GetRecognizedString();
        }                                   
        #endregion        

        // send to facebook
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(textBlock1.Text))
            {
                MessageBox.Show("Please Write a text post.", "Notify", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                return;
            }
            try
            {
                int ratingValue = (int)(rating1.Value*5);
                FacebookAPI.PostOnWall(textBlock1.Text, "Nour El-Dien",ratingValue);
            }
            catch (Exception exp)
            {
                MessageBox.Show("Whoa! no Internet Connection found.", "Notify", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
            }
            
        }
    }   
}

// Trash Code
#if false 
        
        //DllImport using System.Runtime.InteropServices;
        //Left Button - Mouse Down
        private const int WM_LBUTTONDOWN = 0x0201;
        //Left Button - Mouse Up
        private const int WM_LBUTTONUP = 0x0202;
        // escape button
        private const int VK_ESCAPE = 27;
        /// <summary>
        /// The SendMessage function sends the specified message to a window or windows. The function calls the window procedure for the specified window and does not return until the window procedure has processed the message. The PostMessage function, in contrast, posts a message to a thread’s message queue and returns immediately.
        /// </summary>
        ///<param name="hWnd"></param>Identifies the window whose window procedure will receive the message. If this parameter is HWND_BROADCAST, the message is sent to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows; but the message is not sent to child windows.
        ///<param name="wMsg"></param>Specifies the message to be sent.
        ///<param name="wParam"></param>Specifies additional message-specific information.
        ///<param name="lParam"></param>Specifies additional message-specific information. 
        ///<returns>The return value specifies the result of the message processing and depends on the message sent. </returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        Timer timer;
        private void SendEscape()
        {
            WindowInteropHelper windowInteropHelper = new WindowInteropHelper(this);
            IntPtr myHwnd = windowInteropHelper.Handle;

            //long lngResult = SendMessage(btn1.Handle, WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
            SendMessage(myHwnd, WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);

            MessageBox.Show("Lost focus");
            timer = new Timer((object obj) =>
            {
                //KeyEventArgs eInsertBack = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Escape);
                //eInsertBack.RoutedEvent = UIElement.KeyDownEvent;
                //InputManager.Current.ProcessInput(eInsertBack);                
                //internal const ushort VK_ESCAPE = 27;
                //SendEscape();
                MessageBox.Show("fuck");

            }, null, 1000, Timeout.Infinite);
        }
#endif