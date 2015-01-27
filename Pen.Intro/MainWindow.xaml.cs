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

using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Media.Animation;
using Telerik.Windows.Controls;

namespace Pen.Intro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Show Help Icon
        private const uint WS_EX_CONTEXTHELP = 0x00000400;
        private const uint WS_MINIMIZEBOX = 0x00020000;
        private const uint WS_MAXIMIZEBOX = 0x00010000;
        private const int GWL_STYLE = -16;
        private const int GWL_EXSTYLE = -20;
        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOZORDER = 0x0004;
        private const int SWP_FRAMECHANGED = 0x0020;
        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_CONTEXTHELP = 0xF180;

        [DllImport("user32.dll")]
        private static extern uint GetWindowLong(IntPtr hwnd, int index);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, uint newStyle);
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height, uint flags);

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            uint styles = GetWindowLong(hwnd, GWL_STYLE);
            styles &= 0xFFFFFFFF ^ (WS_MINIMIZEBOX | WS_MAXIMIZEBOX);
            SetWindowLong(hwnd, GWL_STYLE, styles);
            styles = GetWindowLong(hwnd, GWL_EXSTYLE);
            styles |= WS_EX_CONTEXTHELP;
            SetWindowLong(hwnd, GWL_EXSTYLE, styles);
            SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
            ((HwndSource)PresentationSource.FromVisual(this)).AddHook(HelpHook);
        }

        private IntPtr HelpHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_SYSCOMMAND && ((int)wParam & 0xFFF0) == SC_CONTEXTHELP)
            {
                MessageBox.Show("help");
                handled = true;
            }
            return IntPtr.Zero;
        }
        #endregion

        /// <summary>
        /// Reference to the canvases of radTileViewItems
        /// </summary>
        Canvas[] canvasTileItem = new Canvas[4];

        // Constructor
        public MainWindow()
        {
            InitializeComponent();            
        }            

        /// <summary>
        /// Window loaded event handler. Required changes in window loading.
        /// </summary>        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // cnovert this window to a glassy window
            GlassyWindow.LoadGlassyWindow(this);
            this.Left = SystemParameters.VirtualScreenWidth - 12 - this.Width;
            this.Top = 12;
            
            radTileViewItem1.TileState = Telerik.Windows.Controls.TileViewItemState.Minimized;
            canvasTile2.Opacity = 0;
            canvasTile3.Opacity = 0;
            canvasTile4.Opacity = 0;

            // subscribe to stateChanged event handler
            radTileViewItem1.TileStateChanged += radTileViewItem_TileStateChanged;
            radTileViewItem2.TileStateChanged += radTileViewItem_TileStateChanged;
            radTileViewItem3.TileStateChanged += radTileViewItem_TileStateChanged;
            radTileViewItem4.TileStateChanged += radTileViewItem_TileStateChanged;

            // add canvases of radTitleViewItems to the array/list
            canvasTileItem[0] = canvasTile1;
            canvasTileItem[1] = canvasTile2;
            canvasTileItem[2] = canvasTile3;
            canvasTileItem[3] = canvasTile4;
        }

        /// <summary>
        /// Window closing event handler.
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        /// <summary>
        /// Tile State changed. Show the content of maximized tile and hide that of minimized one.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radTileViewItem_TileStateChanged(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            RadTileViewItem item = (RadTileViewItem)sender;
            int tileID = Convert.ToInt32(item.Name.Substring(15, 1)) - 1;

            if (((RadTileViewItem)sender).TileState == TileViewItemState.Maximized)
            {
                canvasTileItem[tileID].Visibility = System.Windows.Visibility.Visible;
                AnimateOpacity(canvasTileItem[tileID], true);
            }
            else
            {
                AnimateOpacity(canvasTileItem[tileID], false);
                canvasTileItem[tileID].Visibility = System.Windows.Visibility.Collapsed;
            }
        }       

        /// <summary>
        /// Animate opacity of content of radTileViewItem according to its state.
        /// </summary>       
        private void AnimateOpacity(Canvas item, bool isMaximized)
        {
            DoubleAnimation animation = new DoubleAnimation();
            EasingFunctionBase easing = new QuadraticEase();
            easing.EasingMode = EasingMode.EaseInOut;

            animation.From = isMaximized ? 0 : 1;
            animation.To = isMaximized ? 1 : 0;
            animation.By = 1;
            animation.EasingFunction = easing;
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(250));

            Storyboard storyBoard = new Storyboard();
            Storyboard.SetTargetName(item, item.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.OpacityProperty));
            storyBoard.Children.Add(animation);
            storyBoard.Begin(item);            
        }

        /// <summary>
        /// Event handler for Start button click. Start the appropriate Pen application.
        /// </summary>        
        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int buttonID = Convert.ToInt32(button.Name.Substring(6, 1));

            OpenApplication(buttonID);
            //OpenApplication_(buttonID);           
        }

        /// <summary>
        /// Open the appropriate Pen application according to the send number. 
        /// Applications are opened in a separate process.
        /// </summary>
        private void OpenApplication(int number)
        {
            // get the relative directory(path) of the application
            System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(Environment.CurrentDirectory);
            string path = directory.Parent.Parent.Parent.FullName;

            switch (number)
            {
                // Pen Math
                case 1:
                    path += "\\Pen.Math\\bin\\Debug\\Pen.Math.exe";
                    break;

                // Pen Map
                case 2:
                    path += "\\Pen.Map\\bin\\Debug\\Pen.Map.exe";
                    break;

                // Pen Language
                case 3:
                    path += "\\Pen.Language\\bin\\Debug\\Pen.Language.exe";
                    break;
                
                // Pen Map Tool (teacher tool to build lesson)
                case 4:
                    path += "\\Pen.Map.Tool\\bin\\Debug\\Pen.Map.exe";
                    break;

                // Pen Service (Used to calibrate camera and adjust settings of image processing)
                case 5:
                    path += "\\Pen.Service\\bin\\Debug\\Pen.Service.exe";
                    break;

                // Pen Touch Service with single TouchDriver (which means that no ID)
                case 6:
                    path += "\\Pen.Service.App\\bin\\Debug\\Pen.Service.App.exe";
                    break;

                // Pen Touch Service with many TouchDrivers (which means that there is ID)
                case 7:
                    path += "\\Pen.Service.App\\bin\\Debug\\Pen.Service.App.exe";
                    break;

                // Multimice service (use mice as input provided)
                case 8:
                    path += "\\Multimice\\Multitouch.Service.Console.exe";
                    break;

                default:
                    break;
            }
            // start the application in a separate process
            System.Diagnostics.Process.Start(path);
        }
        
        /// <summary>
        /// Open the appropriate Pen application according to the send number.
        /// Applications are opened in the same process in a new thread.
        /// </summary>
        private void OpenApplication_(int number)
        {
            //System.Windows.Window window = null;
            
            //switch (number)
            //{
            //    case 1:
            //        window = new Pen.Math.MainWindow();
            //        break;
            //    case 2:
            //        window = new Pen.Map.MainWindow();
            //        break;
            //    case 3:
            //        window = new Pen.Language.MainWindow();
            //        break;
            //    default:
            //        break;
            //}

            //System.Threading.Thread thread = new System.Threading.Thread(() =>
            //{
                //Pen.Math.MainWindow w = new Pen.Math.MainWindow();
                //Pen.Math.MainWindow w = new Pen.Math.MainWindow();
                //w.Owner = this;
                //w.Show();

                //Application application = new Application();
                //application.Run(window);

                //Pen.Intro.App app = new Pen.Intro.App();
                //app.InitializeComponent();
                //app.Run();
            //});

            //System.Threading.Thread thread = new System.Threading.Thread(() =>
            //    {
            //        System.Diagnostics.Process.Start(path);
            //    });
            //thread.Start();
         
        }

        /// <summary>
        /// Maximize the tileViewItem when clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radTileViewItem_MouseUp(object sender, MouseButtonEventArgs e)
        {           
            RadTileViewItem item = (RadTileViewItem)sender;
            if (item.TileState != TileViewItemState.Maximized)
                item.TileState = TileViewItemState.Maximized;            
        }       
    }
}
