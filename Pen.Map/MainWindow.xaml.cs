using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Media;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

using Microsoft.MapPoint.Rendering3D;
using Microsoft.MapPoint.Rendering3D.Atmospherics;
using Microsoft.MapPoint.Rendering3D.Cameras;
using Microsoft.MapPoint.Rendering3D.Control;
using Microsoft.MapPoint.Rendering3D.Utility;

using Effects = System.Windows.Media.Effects;
using Telerik.Windows.Controls;
using System.Data;
using System.Data.OleDb;
using System.Windows.Controls.Primitives;

namespace Pen.Map
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // map control and some of its variables
        private GlobeControl map;
        private double pitch = -90;
                
        /// <summary>
        /// Required to number the titles with arabic numbers.
        /// </summary>
        private char[] ArabicNums;

        // constructor
        public MainWindow()
        {
            InitializeComponent();
            FullScreen();
            this.VisualTextHintingMode = TextHintingMode.Animated;

            // load appropriate UI language according to saved settings
            if (!Properties.Settings.Default.PenMapIsEnglish)
                ConvertToArabic();
        }

        // main window loaded
        private void windowMain_Loaded(object sender, RoutedEventArgs e)
        {
            // default drawing settings of map inkCanvas
            SetDefaultDrawingSettings();
            inkCanvasMap.DefaultDrawingAttributes.IgnorePressure = true;  
            inkCanvasContent.DefaultDrawingAttributes.IgnorePressure = true;
            // start the watch
            StartTime();
            // intialize the map                 
            MapInitialize();                      
            // this is required to perform smooth scrolling of the Lesson scrollViewer
            StartManipulationInertia();
            // build dataset of lesson content
            BuildDataSet();
            // default mode on inkCanvasLEsson is the Pen mode
            toggleButton1.IsChecked = true;

            ArabicNums = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            //ArabicNums = new char[] { '٠', '١', '٢', '٣', '٤', '٥', '٦', '٧', '٨', '٩' };

            radColorPicker1.SelectedColor = Color.FromArgb(255, 0, 180, 255);
            radColorPicker1.SelectedColorChanged += new EventHandler(radColorPicker1_SelectedColorChanged);

            
            // clear stakcPanel of Lesson Content from its elements before
            // filling it with new ones
            labelLesson.Content = "";
            stackPanelContent.Children.Clear();
        }

        #region Toggle UI Language
        /// <summary>
        /// Toggle UI language of this application (between English and Arabic).
        /// </summary>        
        private void ToggleLanguage()
        {
            Properties.Settings.Default.PenMapIsEnglish = !Properties.Settings.Default.PenMapIsEnglish;
            Properties.Settings.Default.Save();
            this.Hide();
            this.ShowInTaskbar = false;
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        /// <summary>
        /// change the window to arabic [flow direction = right-to-left]
        /// </summary> 
        private void ConvertToArabic()
        {
            this.FlowDirection = System.Windows.FlowDirection.RightToLeft;
            this.textBlockButton1.Text = "قلم";
            this.textBlockButton2.Text = "ممحاة";
            this.textBlockButton3.Text = "تحريك";
            this.textBlockButton4.Text = "فرشاة";
            this.textBlockButton5.Text = "شارك";
            this.textBlockButton6.Text = "أدوات";
            this.textBlockButton7.Text = "دبوس";
            this.textBlockButton9.Text = "فتح";
          
            Style style = (Style)TryFindResource("TextButtonArabicStyle");
            this.textBlockButton1.Style = style;
            this.textBlockButton2.Style = style;
            this.textBlockButton3.Style = style;
            this.textBlockButton4.Style = style;
            this.textBlockButton5.Style = style;
            this.textBlockButton6.Style = style;
            this.textBlockButton7.Style = style;            
            this.textBlockButton9.Style = style;

            this.labelLesson.Style = (Style)TryFindResource("LabelLessonNameStyle");
        }

        /// <summary>
        /// Takes Enlgish number and returns Arabic translation.
        /// </summary>        
        private string GetArabicNumber(int id)
        {
            string value = string.Empty;
            if (id < 10)
                value = ArabicNums[id].ToString();
            else if (id < 100)
                value = ArabicNums[id / 10].ToString() + ArabicNums[id % 10].ToString();
            else if (id < 1000)
                throw new Exception("Number sent is to big. Number must be less than 1000.");
            return value;
        }        
        #endregion

        #region Main window initalizer + handlers, map initializer

        // close this application if Esc key ie pressed
        private void windowMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Application.Current.Shutdown();  
        }

        // safely closing and disposing all objects
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MapDispose();
        }

        // close button pressed
        private void buttonClose_TouchDown(object sender, TouchEventArgs e)
        {            
            Application.Current.Shutdown();
        }
       
        /// <summary>
        /// full screen application 
        /// </summary>
        private void FullScreen()
        {
            this.WindowStyle = System.Windows.WindowStyle.None;
            this.WindowState = System.Windows.WindowState.Maximized;
        }

        /// <summary>
        /// show and update the time in the captionText of the windows
        /// </summary>
        private void StartTime()
        {
            string time = DateTime.Now.ToShortTimeString();
            textCaption.Text = "Pen Map |  " + time.Remove(time.Length - 3);

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(1);
            timer.Tick += new EventHandler((object sender, EventArgs e) =>
            {
                time = DateTime.Now.ToShortTimeString();
                textCaption.Text = "Pen Map |  " + time.Remove(time.Length - 3);
                //AnimateBackground();
            });
            timer.Start();
        }
         #endregion

        #region Create animation for the application background
        int animationPosition = 0;

        /// <summary>
        /// Animate background images. It contains several animation positions
        /// to toggle betwwen them every amount of time
        /// </summary>
        private void AnimateBackground()
        {
            // this method is called every period acording to SrartTime() method above.

            int moveFrom = 0, moveTo = 0, rotateFrom = 0, rotateTo = 0;

            // incerement animation position. We have different 6 positions
            animationPosition = animationPosition < 5 ? ++animationPosition : 1;

            switch (animationPosition)
            {
                case 1:
                    moveFrom = 0;
                    rotateFrom = 0;
                    moveTo = -300;
                    rotateTo = 275;
                    break;

                case 2:
                    moveFrom = -300;
                    rotateFrom = 275;
                    moveTo = -100;
                    rotateTo = 340;
                    break;

                case 3:
                    moveFrom = -100;
                    rotateFrom = 340;
                    moveTo = 0;
                    rotateTo = 187;
                    break;

                case 4:
                    moveFrom = 0;
                    rotateFrom = 187;
                    moveTo = 0;
                    rotateTo = 90;
                    break;

                case 5:
                    moveFrom = 0;
                    rotateFrom = 90;
                    moveTo = 0;
                    rotateTo = 0;
                    break;

                default:
                    break;
            }

            AnimateXaxisTransition(moveFrom, moveTo);
            AnimateRotation(rotateFrom, rotateTo);
        }

        /// <summary>
        /// Animate transition of background image in the X-axis direction
        /// </summary>        
        private void AnimateXaxisTransition(int moveFrom, int moveTo)
        {
            DoubleAnimation animation = new DoubleAnimation();
            EasingFunctionBase easing = new QuinticEase();
            easing.EasingMode = EasingMode.EaseOut;

            animation.AutoReverse = false;
            animation.From = moveFrom;
            animation.To = moveTo;
            animation.By = 1;
            animation.EasingFunction = easing;
            animation.AccelerationRatio = 1;
            animation.DecelerationRatio = 0;
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(2000));
            animation.Completed += new EventHandler((object obj, EventArgs ev) =>
            { });

            Storyboard storyBoard = new Storyboard();
            Storyboard.SetTargetName(imageBackground, imageBackground.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.LeftProperty));
            storyBoard.Children.Add(animation);
            storyBoard.Begin(imageBackground);

            //RotateTransform rotateTransform = new RotateTransform();
            //imageBackground.RenderTransform = rotateTransform;
            //imageBackground.RenderTransformOrigin = new Point(0.5, 0.5);
            //rotateTransform.BeginAnimation(RotateTransform.AngleProperty, animation);
        }

        /// <summary>
        /// Animate transition of background image in the Y-axis direction
        /// </summary>     
        private void AnimateYaxisTransition(int moveFrom, int moveTo)
        {
            DoubleAnimation animation = new DoubleAnimation();
            EasingFunctionBase easing = new QuinticEase();
            easing.EasingMode = EasingMode.EaseOut;

            animation.AutoReverse = false;
            animation.From = moveFrom;
            animation.To = moveTo;
            animation.By = 1;
            animation.EasingFunction = easing;
            animation.AccelerationRatio = 1;
            animation.DecelerationRatio = 0;
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(2000));
            animation.Completed += new EventHandler((object obj, EventArgs ev) =>
            { });

            Storyboard storyBoard = new Storyboard();
            Storyboard.SetTargetName(imageBackground, imageBackground.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.LeftProperty));
            storyBoard.Children.Add(animation);
            storyBoard.Begin(imageBackground);

            //RotateTransform rotateTransform = new RotateTransform();
            //imageBackground.RenderTransform = rotateTransform;
            //imageBackground.RenderTransformOrigin = new Point(0.5, 0.5);
            //rotateTransform.BeginAnimation(RotateTransform.AngleProperty, animation);
        }

        /// <summary>
        /// Animate rotation background image
        /// </summary>     
        private void AnimateRotation(int rotateFrom, int rotateTo)
        {
            DoubleAnimation animation = new DoubleAnimation();
            EasingFunctionBase easing = new QuinticEase();
            // EasingFunctionBase
            easing.EasingMode = EasingMode.EaseOut;

            animation.AutoReverse = false;
            animation.From = rotateFrom;
            animation.To = rotateTo;
            animation.By = 1;
            animation.EasingFunction = easing;
            animation.AccelerationRatio = 1;
            animation.DecelerationRatio = 0;
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(2000));
            animation.Completed += new EventHandler((object obj, EventArgs ev) =>
            { });

            RotateTransform rotateTransform = new RotateTransform();
            imageBackground.RenderTransform = rotateTransform;
            imageBackground.RenderTransformOrigin = new Point(0.5, 0.5);
            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, animation);
        }
        #endregion

        #region Map Initialize and Dispose and some methods for the map
        /// <summary>
        /// initialize map control
        /// </summary>
        private void MapInitialize()
        {
            //map = new GlobeControl();
            //controlHost.Child = map;
            map = (GlobeControl)this.controlHost.Child;
            
            GlobeControl.GlobeControlInitializationOptions options = new GlobeControl.GlobeControlInitializationOptions();
            options.UseMultithreadedDevice = true;
            options.HostDomain = AppDomain.CurrentDomain;            
            
            map.AccessibleDescription = "Main Bing Maps 3D Display";
            map.AccessibleName = "Bing Maps 3D";
            map.Name = "globeControl";
            map.AllowDrop = true;
            map.FixedView = false;            
            map.SaveUserSettings = true;
            map.ForceStartAltitudeToAboveGround = false;            
            map.PrintingMode = false;
            map.RaiseLatLongEventOnMouseDown = false;            
            map.SaveUserSettings = true;
            map.StartAltitude = 5000000D;
            map.StartHeading = 0D;
            map.StartLatitude = 31.75D;
            map.StartLongitude = 31.71D;
            map.StartPitch = -90D;           
            map.UseDefaultConfiguration = true;

            // complete some setting of map after it is firstly rendered        
            map.Host.RenderEngine.FirstFrameRendered += new EventHandler((object sender, EventArgs e) =>
            {
                // this is to run manually zoom-out action
                isZoomOutOnClick = true;
                ZoomOutOnClick();

                // at this point, the control is fully initialized and we can interact with it without worry
                // set various data sources, here for elevation data, terrain data, and model data
                map.Host.DataSources.Add(new DataSourceLayerData("Elevation", "Elevation", @"http://www.bing.com/maps/Manifests/HD2.xml", DataSourceUsage.ElevationMap));
                map.Host.DataSources.Add(new DataSourceLayerData("Texture", "Texture", @"http://www.bing.com/maps/Manifests/AT2.xml", DataSourceUsage.TextureMap));
                map.Host.DataSources.Add(new DataSourceLayerData("Models", "Models", @"http://www.bing.com/maps/Manifests/MO2.xml", DataSourceUsage.Model));
                // turn on the nice atmosphere
                this.map.Host.WorldEngine.Environment.AtmosphereDisplay = EnvironmentManager.AtmosphereStyle.Scattering;
                map.Host.WorldEngine.Environment.CelestialDisplay = EnvironmentManager.CelestialStyle.Regular;
                map.Host.WorldEngine.Environment.SunPosition = SunPosition.FromRealtime();
                map.Host.RenderEngine.Graphics.Settings.UseAnisotropicFiltering = true;
                // create layers for labels, pins, shapes
                map.Host.Geometry.AddLayer(pinLayer);                
                map.Host.Geometry.AddLayer(labelLayer);
                map.Host.Geometry.AddLayer(polyLayer);
                // much more settings
                map.Host.WorldEngine.AllowFrameRender = true;
                map.Host.WorldEngine.BaseCopyrightText = "Pen | Geography ";
                map.Host.WorldEngine.Culture = CultureInfo.CurrentCulture;
                //map.Host.WorldEngine.DisplayTileInformation = true;            
                map.Host.WorldEngine.DisplayOnlyIncludedModels = false;
                map.Host.WorldEngine.Display3DCursor = false;
                map.Host.WorldEngine.EnableInertia = true;
                map.Host.WorldEngine.EnableInput = true;
                map.Host.WorldEngine.EnableScreenDragRegions = true;
                map.Host.WorldEngine.EnableStreetLevelAltitudeAdjustment = true;
                map.Host.WorldEngine.EnableStreetLevelPitchAdjustment = true;
                map.Host.WorldEngine.InertiaDecayFactor = 0.3;
                map.Host.WorldEngine.ShowCursorLocationInformation = true;
                map.Host.WorldEngine.ShowLatLongAsDegreesMinutesSeconds = false;
                map.Host.WorldEngine.ShowLoadingFeedback = true;
                map.Host.WorldEngine.ShowLoadingErrorFeedback = true;
                map.Host.WorldEngine.ShowLocation = true;
                map.Host.WorldEngine.ShowNavigationControl = true;
                map.Host.WorldEngine.ShowUI = true;
                map.Host.WorldEngine.ShowMapObjects = true;
                map.Host.WorldEngine.ShowScale = true;
                map.Host.WorldEngine.UseMetric = true;
                map.Host.WorldEngine.UseLoadingAnimations = true;
                map.Host.WorldEngine.SetWindowsCursor(System.Windows.Forms.Cursors.Hand);

                map.Host.WorldEngine.AttachWorldDateTimeToNow();
                //map.Host.WorldEngine.AddWorldObjectHighlight(1, System.Drawing.Color.Red);
                //map.Host.WorldEngine.ShowImmersiveAdvertising = true;
            });
        }                
        
        /// <summary>
        /// dispose map object so as notr to make this form crash
        /// </summary>
        private void MapDispose()
        {
            if (controlHost.Child != null)            
                controlHost.Child = null;

            if (map != null)
            {
                map.Host.RenderEngine.ShutDownRenderThread();
                map.Host.HttpManager.CancelAll();
                map.Host.Control.Dispose();                
                map = null;
            }

            GC.Collect();
        }

        enum MapViewMode { Aerial, Hyprid, Road };
        /// <summary>
        /// Toggle between Aerial, Hyprid and Road views of the map        
        /// </summary>       
        private void ShowHideTexture(MapViewMode viewMode)
        {            
            map.Host.DataSources.Remove("Texture", "Texture");

            switch (viewMode)
            {
                case MapViewMode.Aerial:
                    map.Host.DataSources.Add(new DataSourceLayerData("Texture", "Texture", @"http://www.bing.com/maps/Manifests/AT2.xml", DataSourceUsage.TextureMap));
                    break;
                case MapViewMode.Hyprid:
                    map.Host.DataSources.Add(new DataSourceLayerData("Texture", "Texture", @"http://www.bing.com/maps/Manifests/HT2.xml", DataSourceUsage.TextureMap));
                    break;
                case MapViewMode.Road:
                    map.Host.DataSources.Add(new DataSourceLayerData("Texture", "Texture", @"http://www.bing.com/maps/Manifests/RT2.xml", DataSourceUsage.TextureMap));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// add traffice layer to the map, works only for some countries like USA
        /// </summary>
        private void AddTrafficeOverLay()
        {
            /*** In the Activate method of the plug-in ***/
            map.Host.DataSources.Add(new DataSourceLayerData("terrain", "traffic", "http://www.bing.com/maps/Manifests/TR2.xml",
                DataSourceUsage.TextureMap, 10, 0.5));
        }
        #endregion

        #region Zoom-out action of the map (manually)
        private DateTime lastTouchDown = DateTime.Now;
        private bool isDoubleClicked = false;
        // there are two moods for zoom out for the map
        // 1. on click event, and in this mode the navigation plugin is unloaded of deactivated
        // 2. from the navigation plugin buttons, in this mode, the onclick zoom-out is not active
        private bool isZoomOutOnClick;               

        void map_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            lastTouchDown = DateTime.Now;
            isDoubleClicked = false;
        }

        void map_Click(object sender, EventArgs e)
        {
            // the first if condition is to check if this is a successfull click event
            // or it is just a drag and drop event            
            if ((DateTime.Now - lastTouchDown).TotalMilliseconds < 200)
            {
                Timer timer = new Timer((object obj) =>
                {
                    // the second if condition is to check if there was a second click
                    // performed after this one-click event with in a period of 200-300ms
                    // so it will bw handled as a a double-click event hence map will zoom-in
                    // instead of zoom out OR it is a normal click event
                    if (!isDoubleClicked)
                    {
                        // perform zoom-out                        
                        LatLonAlt location = map.Host.Navigation.CameraPosition.Location;
                        location.Altitude += (location.Altitude * 1.8);

                        CameraParameters camera = new CameraParameters();
                        camera.AccelPeriod = camera.DecelPeriod = 0.13;
                        camera.TransitionTime = 1.0;

                        map.Host.Navigation.FlyTo(location, pitch, 0, true, camera);
                    }

                }, null, 200, System.Threading.Timeout.Infinite);
                System.Timers.Timer t = new System.Timers.Timer();
            }
        }

        void map_DoubleClick(object sender, EventArgs e)
        {
            isDoubleClicked = true;
        }

        // subscribe/unsubscribe to event handlers responsible of performing
        // zoom-out when the map is clicked by the user        
        private void ZoomOutOnClick()
        {
            if (isZoomOutOnClick)
            {
                map.Click += new EventHandler(map_Click);
                map.DoubleClick += new EventHandler(map_DoubleClick);
                map.MouseDown += new System.Windows.Forms.MouseEventHandler(map_MouseDown);
            }
            else
            {
                map.Click -= new EventHandler(map_Click);
                map.DoubleClick -= new EventHandler(map_DoubleClick);
                map.MouseDown -= new System.Windows.Forms.MouseEventHandler(map_MouseDown);
            }
        }

        // load plugins like control plug in        
        Guid pluginID;
        Microsoft.MapPoint.PlugIns.PlugInLoader loader;
        private void NavigationPlugIn()
        {
            // this is to create loader and load the blugin for the first time only
            if (loader == null)
            {
                loader = Microsoft.MapPoint.PlugIns.PlugInLoader.CreateLoader(map.Host, AppDomain.CurrentDomain);                
                pluginID = loader.LoadPlugIn(typeof(Microsoft.MapPoint.Rendering3D.NavigationControl.NavigationPlugIn));
            }

            if (!isZoomOutOnClick)
            {
                if (!loader.IsPlugInActive(pluginID))                    
                    loader.ActivatePlugInOnRenderThread(pluginID, null);
                    //loader.ActivatePlugIn(pluginID, null);
                else throw new Exception("The plugin is already active");                
            }
            else
            {
                if (loader.IsPlugInActive(pluginID))
                    loader.DeactivatePlugInOnRenderThread(pluginID);
                    //loader.DeactivatePlugIn(pluginID);                    
                else throw new Exception("The plugin is already deactivate");
            }
            //loader.ActivatePlugInOnRenderThread(g, null);
        }
        #endregion

        #region Open/Close Facebook Panel + Window Opacity Animation
        // facebook share using facebook panel
        private void FacebookShare()
        {
            AnimateWindowOpacity(1, 0.5);
            Pen.Tools.FbWindow fbWindow = new Pen.Tools.FbWindow();
            fbWindow.ShowDialog();
            AnimateWindowOpacity(0.5, 1);
        }
        
        // animate Window Opacity when facebok panel opened/closed
        private void AnimateWindowOpacity(double from, double to)
        {
            DoubleAnimation animation = new DoubleAnimation();
            EasingFunctionBase easing = new QuinticEase();
            if (from > to)
                easing.EasingMode = EasingMode.EaseInOut;
            else
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
            storyBoard.Begin(this);
        }
        #endregion

        #region Open and Close Drawing Mode of the Map
        private bool drawingModeOn = false;  
        /// <summary>
        /// this hides the map and view the inkCanvasMap to take notes on the map
        /// </summary>
        private void OpenDrawingMode()
        {
            inkCanvasMap.EditingMode = InkCanvasEditingMode.None;
            // chekc if drawing mode is already opened, exit
            if (drawingModeOn)
                return;

            drawingModeOn = true;
            // set background            
            gridMap.Background = new SolidColorBrush(Colors.White);
            MemoryStream stream = new MemoryStream();
            map.Host.WorldEngine.CaptureScreenShot().Save(stream, ImageFormat.Png);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
            image.EndInit();
            ImageBrush background = new ImageBrush();
            background.ImageSource = image;
            background.Stretch = Stretch.None;
            // show inkCanvas and hide map                                   
            controlHost.Visibility = System.Windows.Visibility.Collapsed;
            gridMap.Opacity = 0;
            gridMap.Background = background;
            AnimateMapOpacity(gridMap.Name, 0, 0.8);
        }

        /// <summary>
        /// this hides the inkCanvasMap and open the map for viewing
        /// </summary>
        private void CloseDrawingMode()
        {
            if (!drawingModeOn)
                return;
            drawingModeOn = false;
            AnimateMapOpacity(gridMap.Name, 0.8, 1);
            inkCanvasMap.EditingMode = InkCanvasEditingMode.None;
        }

        /// <summary>
        /// Manipulate opacity animation to the map when we want to toggle betwwen 
        /// drawing mode on the map and scrolling mode
        /// </summary>        
        private void AnimateMapOpacity(string name, double from, double to)
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
            Storyboard.SetTargetName(animation, name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(InkCanvas.OpacityProperty));
            storyBoard.Children.Add(animation);
            if (!drawingModeOn)
                storyBoard.Completed += new EventHandler(storyBoard_Completed);
            storyBoard.Begin(canvasMain);
        }

        // complete some work related to toggling between map modes, after animation ends
        void storyBoard_Completed(object sender, EventArgs e)
        {
            controlHost.Visibility = System.Windows.Visibility.Visible;
            gridMap.Background = new SolidColorBrush(Colors.White);
            inkCanvasMap.Strokes.Clear();
        }
        #endregion

        #region InkCanvasMap Drawing and changing colors
        // drawing, editing, color changing, erasing
        private StylusPointCollection pointCollection;
        // save current storke for every user (every inkCanvas)
        private Dictionary<int, Stroke> strokeDictionary = new Dictionary<int, Stroke>();
        // save color and ben thickness settings for every user
        private Dictionary<int, int> strokeThickness = new Dictionary<int, int>();
        private Dictionary<int, Color> colorSettings = new Dictionary<int, Color>();
        /// <summary>
        /// Set drawing settings according to the Touchdevice ID of the
        /// Teacher and Students. Values of ID's are [10-,130,386,258] repsectively
        /// </summary>
        private void SetDefaultDrawingSettings()
        {
            // teacher default drawing settings
            strokeThickness.Add(10, 2);
            colorSettings.Add(10, Colors.FloralWhite);
            
            // students default drawing setting
            strokeThickness.Add(0, 3);
            colorSettings.Add(0, Colors.Yellow);
        }

        /// <summary>
        /// Get drawing settings according to the Touchdevice ID [10-,130,386,258]
        /// </summary>        
        private void GetDrawingSettings(ref Stroke stroke, int touchDeviceId)
        {
            Color color;
            int thickness;
            int id = Convert.ToInt32(touchDeviceId.ToString().Substring(0, 2));

            // if there is no custom settings (color, thickness) for this user, load default
            if (!colorSettings.TryGetValue(id, out color))
                colorSettings.TryGetValue(0,out color);
            
            if (!strokeThickness.TryGetValue(id, out thickness))
                strokeThickness.TryGetValue(0, out thickness);
            // finally set settings
            stroke.DrawingAttributes.Color = color;
            stroke.DrawingAttributes.Width = stroke.DrawingAttributes.Height = thickness;
        }

        private void inkCanvasMap_TouchDown(object sender, TouchEventArgs e)
        {
            // check if Editing mode == Select, return back, this is to prevent
            // Teacher inkStroke from drawing in case of the Editing Mode = Select            
            if (inkCanvasMap.EditingMode != InkCanvasEditingMode.None)
                return;            

            // get current touch point
            Point p = e.GetTouchPoint(inkCanvasMap).Position;
            pointCollection = new StylusPointCollection();
            pointCollection.Add(new StylusPoint(p.X, p.Y));

            // create storke and load its settings
            Stroke newStroke = new Stroke(pointCollection);                        
            GetDrawingSettings(ref newStroke, e.TouchDevice.Id);            
            
            // add stoke to stroke dictionary, this is to implement simultanious users drawing
            strokeDictionary.Add(e.TouchDevice.Id, newStroke);
            // add stroke to canvas
            inkCanvasMap.Strokes.Add(newStroke);
            // call inkMove to add the first Point
            inkCanvasMap_TouchMove(sender, e);
        }

        private void inkCanvasMap_TouchMove(object sender, TouchEventArgs e)
        {
            Stroke currentStorke;
            Point p = e.GetTouchPoint(inkCanvasMap).Position;
            // add new point to the current stroke of the sender inkCanvasMap
            // we use if here in order to handle the error that generated
            // by this sequence: Down --> Move --> Leave --> Move
            if (strokeDictionary.TryGetValue(e.TouchDevice.Id, out currentStorke))
                currentStorke.StylusPoints.Add(new StylusPoint(p.X, p.Y));

        }

        private void inkCanvasMap_TouchUp_TouchLeave(object sender, TouchEventArgs e)
        {
            // remove stroke from Dictionary
            //Stroke stroke;
            //strokeDictionary.TryGetValue(e.TouchDevice.Id, out stroke);
            strokeDictionary.Remove(e.TouchDevice.Id);

            // relase capture form sender inkCanvas
            inkCanvasMap.ReleaseTouchCapture(e.TouchDevice);
        }
        #endregion

        #region Changing map drawing color for users according to their deviceID
        // get the deviceid that fired touchUp event on color picker
        private int id;    
        
        /// <summary>
        /// get the deviceId who clicked color picker
        /// </summary>        
        private void radColorPicker1_TouchDown(object sender, TouchEventArgs e)
        {
            id = e.TouchDevice.Id;
        }

        /// <summary>
        /// determine if color changed or not
        /// </summary>        
        private void radColorPicker1_SelectedColorChanged(object sender, EventArgs e)
        {
            SystemSounds.Asterisk.Play();
            Color color;
            id = Convert.ToInt32(id.ToString().Substring(0, 2));
            if (colorSettings.TryGetValue(id, out color))
                colorSettings[id] = radColorPicker1.SelectedColor;
            else
                colorSettings.Add(id, radColorPicker1.SelectedColor);
        }
        #endregion

        #region Manipulation and Inertia of ScrollViewer (smooth scrolling)
        
        /// <summary>
        /// initialize event handlers required to perform smooth scrolling
        /// </summary>
        private void StartManipulationInertia()
        {
            scrollViewerContent.ManipulationStarting += new EventHandler<ManipulationStartingEventArgs>(scrollViewerContent_ManipulationStarting);
            scrollViewerContent.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(scrollViewerContent_ManipulationDelta);
            scrollViewerContent.ManipulationInertiaStarting += new EventHandler<ManipulationInertiaStartingEventArgs>(scrollViewerContent_ManipulationInertiaStarting);
            scrollViewerContent.IsManipulationEnabled = true;
        }

        /// <summary>
        /// start of manipulation (equivilant to touchDown + first touchMove events)
        /// </summary>        
        void scrollViewerContent_ManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            e.ManipulationContainer = canvasMain;
            e.Handled = true;
            e.Mode = ManipulationModes.TranslateY;
        }

        /// <summary>
        /// when there is movement updates (equivilant to touchMove)
        /// </summary>        
        void scrollViewerContent_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            scrollViewerContent.ScrollToVerticalOffset(scrollViewerContent.VerticalOffset - e.DeltaManipulation.Translation.Y);
        }

        /// <summary>
        /// when the touch is released from the control (equivilant to TouchUp or TouchLeave)
        /// </summary>        
        void scrollViewerContent_ManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            // adjust the dispalcement behaviour
            // (10 inches * 96 DIPS per inch / 1000ms^2)
            e.TranslationBehavior.DesiredDeceleration = 2.0 * 96.0 / (1000.0 * 1000.0);
            e.TranslationBehavior.InitialVelocity = e.InitialVelocities.LinearVelocity;
            e.TranslationBehavior.DesiredDisplacement = Math.Abs(e.InitialVelocities.LinearVelocity.Y) * 300;
        }
        #endregion
        
        #region Add Shapes to the map (pushpins, labels, polygons and polylines)
        // layers and counters for geometry shapes of the map        
        private string pinLayer = "pins";
        private string labelLayer = "labels";
        private string polyLayer = "polys";
        private int labelCounter = 0, polyCounter = 0, pinCounter = 0;        

        /// <summary>
        /// add geompetryPushpin to the map
        /// </summary>
        private void AddPin()
        {
            LatLonAlt position = map.Host.Navigation.CameraPosition.Location;
            position.Altitude = 0.0;
            AddPin(position);
        }
        
        /// <summary>
        /// add geompetryPushpin to the map with the given position
        /// </summary>
        private void AddPin(LatLonAlt position)
        {
            pinCounter++;
            PushpinGeometry pin = new PushpinGeometry(pinLayer, pinCounter.ToString(), position, GeometryStyle.GetPushPinStyle(GetArabicNumber(pinCounter)));
            map.Host.Geometry.AddGeometry(pin);            
        }

        /// <summary>
        /// Add label to the map. For every Title in the Lesson, there is 
        /// corresponding poition to it on the map. This posititon is set
        /// by adding a label with this method
        /// </summary>
        private void AddPositionLabel(LatLonAlt position)
        {            
            labelCounter ++;
            
            // create a new label whose ID = currentTitle    
            LabelGeometry label = new LabelGeometry(labelLayer, labelCounter.ToString() , position, GeometryStyle.GetLabelStyle(GetArabicNumber(labelCounter)));
            map.Host.Geometry.AddGeometry(label);            
        }

        ///<summary>
        ///Add polylgonGeopemtry to the map whose fill color is given
        ///</summary>
        private void AddPolygon(List<LatLonAlt> points, System.Drawing.Color color)
        {
            polyCounter++;           

            PolygonGeometry polygon = new PolygonGeometry(polyLayer, polyCounter.ToString(), null, points.ToArray(), PolygonGeometry.PolygonFormat.Polygon2D, GeometryStyle.GetPolygonStyle(color));
            map.Host.Geometry.AddGeometry(polygon);
        }

        ///<summary>
        ///Add polylineGeopemtry to the map whose fill color is given
        ///</summary>
        private void AddPolyline(List<LatLonAlt> points, System.Drawing.Color color)
        {
            polyCounter++;

            PolylineGeometry polyline = new PolylineGeometry(polyLayer, polyCounter.ToString(), null, points.ToArray(), PolylineGeometry.PolylineFormat.Polyline2D, GeometryStyle.GetPolylineStyle(color));
            map.Host.Geometry.AddGeometry(polyline);
        }
        #endregion
        
        #region Database access, to open lessons from access files
        // Ojects required to acess to MS-Access Files
        private string fileName;        
        private string CONN_STRING = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=";
        private string CONN_SOURCE;
        // DataSet needed to deal with data in the access lessonfile
        private DataSet dataSet = new DataSet("LessonData");
        /// <summary>
        /// keep traking the currentTitle choosed. It depends on a 1-based
        /// listing, which means that the first title in the LessonContent = 1
        /// </summary>
        private int currentTitle = 1;
        /// <summary>
        /// array carrying positions of Lesson Titles on the map
        /// </summary>
        LatLonAlt[] titlePositions;
        /// <summary>
        /// Keeps a reference(record) to the gridTitles of the lesson content
        /// </summary>
        Grid[] titles;

        /// <summary>
        /// Build tables of the dataset for the first time.
        /// Tables are required to retreive data from Lesson access files
        /// </summary>
        private void BuildDataSet()
        {
            //// add the current-exist elements (exist in the lesson content) to their lists            
            //this.textBoxT1.Text = ArabicNums[1] + ".  ";
            //textBoxT1.GotFocus += new RoutedEventHandler(textBoxTitle_GotFocus);
            //// focus on title1 stackPanel
            //TitleFocus(1);
            //// set selectionPeam at the end of the text
            //textBoxT1.SelectionStart = textBoxT1.Text.Length;

            // add tables to the dataSet
            dataSet.Tables.Add("Title");
            dataSet.Tables.Add("Paragraph");
            dataSet.Tables.Add("Pin");
            dataSet.Tables.Add("Poly");
        }
        
        /// <summary>
        /// 1. Open file dialog
        /// 2. Copy the access database into the dataSet (dataSet is in memory)
        /// 3. Create Lesson ELements and fill them with data form dataset
        /// 4. Load shapes to the map
        /// </summary>        
        private void OpenLessonFile()
        {
            // directory of the file containing lasted accessed directory by user
            string LessonFile = Environment.CurrentDirectory + "\\LessonDirectory.txt";
            // 1. Open file dialog
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.Title = "Open ...";
            fileDialog.DefaultExt = ".pmf";
            fileDialog.Filter = "Pen Maps File |*.pmf";
            // first, try last saved directory by user (found in Directory text file)
            // If not found, open directory to desktop
            try
            {
                TextReader reader = new StreamReader(LessonFile); 
                fileDialog.InitialDirectory = reader.ReadLine();
                reader.Close();
            }
            catch
            {
                fileDialog.InitialDirectory = "C:\\Users\\" + Environment.UserName + "\\Desktop";
            }
            // check if user clicked ok, else leave this method
            if (!fileDialog.ShowDialog() == true)
            {
                SystemSounds.Exclamation.Play();
                return;
            }            
            // save recently opened directory to (Directory text file)
            TextWriter writer = new StreamWriter(LessonFile);
            writer.Write(fileDialog.FileName.TrimEnd(fileDialog.SafeFileName.ToCharArray()));
            writer.Close();

            // 2. Load the access database into the dataSet 
            // after the brevious successful Scenario (ShowFileDialog)
            // successfull means the user clicked open

            // get full bath of Lesson file
            CONN_SOURCE = fileDialog.FileName;
            // get file name because the current
            // lesson title is named after the file name
            fileName = fileDialog.SafeFileName;
            string commandString = "";

            // clear dataset before filling it
            dataSet.Tables["Paragraph"].Clear();
            dataSet.Tables["Title"].Clear();
            dataSet.Tables["Pin"].Clear();
            dataSet.Tables["Poly"].Clear();
            // clear map before adding shapes to it, set shapes counters to 0
            map.Host.Geometry.ClearLayer(pinLayer);
            map.Host.Geometry.ClearLayer(labelLayer);
            map.Host.Geometry.ClearLayer(polyLayer);
            pinCounter = labelCounter = polyCounter = 0;

            using (OleDbCommand dbCommand = new OleDbCommand())
            {
                dbCommand.Connection = new OleDbConnection(CONN_STRING + CONN_SOURCE);
                dbCommand.Connection.Open();

                // load Title and Parapgraph tables
                commandString = "Select * From Title";
                dbCommand.CommandText = commandString;
                dataSet.Tables["Title"].Load(dbCommand.ExecuteReader());

                commandString = "Select * From Paragraph";
                dbCommand.CommandText = commandString;
                dataSet.Tables["Paragraph"].Load(dbCommand.ExecuteReader());

                commandString = "Select * From Pin";
                dbCommand.CommandText = commandString;
                dataSet.Tables["Pin"].Load(dbCommand.ExecuteReader());

                commandString = "Select * From Poly";
                dbCommand.CommandText = commandString;
                dataSet.Tables["Poly"].Load(dbCommand.ExecuteReader());

                // close connection
                dbCommand.Connection.Close();                
            }


            // 3. Fill in the lesson content (titles, dates and paragraphs)

            // Lesson Name = fileName with out the extension
            char[] chars = {'.', 'p', 'm', 'f'};
            labelLesson.Content = fileName.TrimEnd(chars);

            // clear stakcPanel of Lesson Content from its elements before
            // filling it with new ones
            stackPanelContent.Children.Clear();

            // those objects represent elements used to build Lesson Content            
            Grid grid;
            Label label;
            TextBlock textBlock;
            int titleCount = dataSet.Tables["Title"].Rows.Count;            
            titlePositions = new LatLonAlt[titleCount];
            titles = new Grid[titleCount];
            
            for (int i = 0; i < titleCount; i++)
            {                
                // get titlePosition, save it to the array
                // and add title-Position label to the map
                double lat = (double)dataSet.Tables["Title"].Rows[i]["Lat"];
                double lon = (double)dataSet.Tables["Title"].Rows[i]["Lon"];
                double alt = (double)dataSet.Tables["Title"].Rows[i]["Alt"];
                titlePositions[i] = LatLonAlt.CreateUsingDegrees(lat,lon, alt);                    
                AddPositionLabel(LatLonAlt.CreateUsingDegrees(lat, lon, 0));

                // new gridTitle
                grid = new Grid();
                grid.Style = (Style)TryFindResource("GridTitleStyle");
                // add event handler for mouse up (click) event
                // which is needed to handle the scenario Lesson Content flow over the map
                // which means when the Teacher selects a Title, the map moves to a position
                // related to this Title (titlePosition) and loads pins related to thid title
                //grid.MouseUp += new MouseButtonEventHandler(gridTitle_MouseUp);
               
                // new labelTitle
                label = new Label();
                label.Name = "labelT" + (i + 1).ToString();                
                label.Content = dataSet.Tables["Title"].Rows[i]["Text_"].ToString();
                if (Properties.Settings.Default.PenMapIsEnglish)
                    label.Style = (Style)TryFindResource("LabelTitleEnglishStyle");
                else
                    label.Style = (Style)TryFindResource("LabelTitleArabicStyle");
                grid.Children.Add(label);                

                // new labelDate                
                label = new Label();
                label.Name = "labelD" + (i + 1).ToString();                
                label.Content = dataSet.Tables["Title"].Rows[i]["Date_"].ToString();
                if (Properties.Settings.Default.PenMapIsEnglish)
                    label.Style = (Style)TryFindResource("LabelDateEnglishStyle");
                else
                    label.Style = (Style)TryFindResource("LabelDateArabicStyle");
                grid.Children.Add(label);

                // add gridTitle to stackPanel of the Lesson Content
                // and to titles array
                titles[i] = grid;
                stackPanelContent.Children.Add(grid);

                // choose paragraphs related to current title then add them
                var rows = from DataRow row in dataSet.Tables["Paragraph"].Rows
                           where (int)row["TitleID"] == i+1
                           select row;
                foreach (DataRow row in rows)
                {
                    textBlock = new TextBlock();
                    textBlock.Name = "TextBoxP" + row["PID"].ToString();                    
                    textBlock.Text = row["Text_"].ToString();
                    if (Properties.Settings.Default.PenMapIsEnglish)
                        textBlock.Style = (Style)TryFindResource("TextBlockParagraphEnglishStyle");
                    else
                        textBlock.Style = (Style)TryFindResource("TextBlockParagraphArabicStyle");
                    stackPanelContent.Children.Add(textBlock);                    
                }
            }
            // modify the style of only the first gridTitle
            ((Grid)stackPanelContent.Children[0]).Margin = new Thickness(0, 0, 0, 0);

            // 4. add/plot polys to the map
            // choose polys (polygons and polylines) related to current title
            // then add them. choose them from dataSet and add them to the map
            // This is doneby these steps:
            // 1. Get a list of polys' (IDs, types and colors)
            // 2. for each poly, get a list for its points
            // 3. you have the poly, you have the points, so go and draw it
            List<int> polys = new List<int>();
            List<bool> types = new List<bool>();
            List<System.Drawing.Color> colors = new List<System.Drawing.Color>();
            string[] colorString;

            // 1. Get a list of polys' IDs, types and colors
            foreach (DataRow row in dataSet.Tables["Poly"].Rows)
            {
                int value = (int)row["PolyID"];
                if (!polys.Contains(value))
                {
                    polys.Add(value);
                    types.Add((bool)row["Type"]);
                    colorString = ((string)row["Color"].ToString()).Split('.');
                    colors.Add(System.Drawing.Color.FromArgb(
                        Convert.ToByte(colorString[0]),
                        Convert.ToByte(colorString[1]),
                        Convert.ToByte(colorString[2]),
                        Convert.ToByte(colorString[3])));
                }
            }

            List<LatLonAlt> points;
            for (int i = 0; i < polys.Count; i++)
            {
                // get data of points related to the current polyID
                var rows = from DataRow row in dataSet.Tables["Poly"].Rows
                           where (int)row["PolyID"] == polys[i]
                           select row;

                // conver these data to a list map points (position)
                points = new List<LatLonAlt>();
                foreach (DataRow row in rows)
                    points.Add(LatLonAlt.CreateUsingDegrees((double)row["Lat"], (double)row["Lon"], (double)row["Alt"]));

                // draw the poly according to its type and color
                if (types[i] == true)
                    AddPolygon(points, colors[i]);
                else if (types[i] == false)
                    AddPolyline(points, colors[i]);
            }

            // add titlePosition and pushpins (related to this title) on the map
            FocusOnTitle(1);
        }       
        #endregion

        #region Scrolling up and down of Lesson Titles and Title Focus
        /// <summary>
        /// Scroll the stackPanel of the Lesson Content one title up/down smoothly
        /// </summary>
        private void ScrollLessonContent(object sender)
        {
            string name = ((Button)sender).Name;            

            // determine to scroll up or scroll down
            int i = 0;            
            if (name == "button10")
                i--;            
            else
                i++;

            // get Y offset of the scrollViewer
            double currentYaxis = scrollViewerContent.TransformToVisual(Application.Current.MainWindow as FrameworkElement).Transform(new Point(0, 0)).Y;
            // get Y offset of destination title (title to be scrolled up/down to)             
            double destinationYaxis = titles[currentTitle + i - 1].TransformToVisual(Application.Current.MainWindow as FrameworkElement).Transform(new Point(0, 0)).Y;

            // now we will scroll from current offset to destination offset
            double yAxisDifference = destinationYaxis - currentYaxis;

            // now scroll from the current vertical offset toh the destination vertical offset
            // destination vertical offset = current + yAxisDifference            

            mediator.ScrollViewer = scrollViewerContent;
            canvasMain.RegisterName("mediator", mediator);

            DoubleAnimation animation = new DoubleAnimation();
            EasingFunctionBase easing = new QuinticEase();
            // EasingFunctionBase
            easing.EasingMode = EasingMode.EaseOut;

            animation.From = scrollViewerContent.VerticalOffset;
            animation.To = scrollViewerContent.VerticalOffset + yAxisDifference;
            animation.By = 1;
            animation.EasingFunction = easing;
            //animation.AccelerationRatio = 0;
            //animation.DecelerationRatio = 1;
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(800));

            Storyboard storyBoard = new Storyboard();
            Storyboard.SetTargetName(animation, "mediator");
            Storyboard.SetTargetProperty(animation, new PropertyPath(ScrollViewerOffsetMediator.VerticalOffsetProperty));
            storyBoard.Children.Add(animation);
            storyBoard.Begin(this);

        }

        /// <summary>
        /// When a title is choosed by the user, we do these things:
        /// 1. Change background of the old title to unfocused background
        /// 2. Change background of choosed title to the focued mode
        /// 3. Update currentTitle number
        /// 4. Fly to the new position of the choosed title (titleID)
        /// 5. Clear map from pushpins and add those pins corresponding to the choosed title        
        /// </summary>        
        private void FocusOnTitle(int titleID)
        {
            // 1. set color unfocus style
            ((Grid)titles[currentTitle - 1]).Background = new SolidColorBrush(Color.FromArgb(255, 236, 246, 250));

            // 2. set color focus style
            ((Grid)titles[titleID - 1]).Background = (LinearGradientBrush)TryFindResource("GridTitleFocusBackground");

            // 3. update currentTitle number 
            currentTitle = titleID;

            // 4. fly to a new postiton/destination
            map.Host.Navigation.FlyTo(titlePositions[currentTitle - 1], pitch, 0);

            // 5.Clear map from pushpins and add those pins corresponding to the choosed title
            pinCounter = 0; // reset pin counter
            map.Host.Geometry.ClearLayer(pinLayer);
            var rows = from DataRow row in dataSet.Tables["Pin"].Rows
                       where (int)row["TitleID"] == currentTitle
                       select row;
            foreach (DataRow row in rows)
                AddPin(LatLonAlt.CreateUsingDegrees((double)row["Lat"], (double)row["Lon"], 0));
        }
        #endregion

        #region Event Handler for buttons under the map
        // [draw ,erase, marker, hand, facebook, pin]
        // the controls are: 1. Pen  2.Eraser  3.Hand  4. Marker 5. facebook share
        // indicates whether the map is in Viewing or Drawing Mode
        private void tool_Click(object sender, RoutedEventArgs e)
        {
            int index = Convert.ToInt32(((Button)sender).Name.Substring(6, 1));
            switch (index)
            {
                // pen
                case 1:
                    OpenDrawingMode();
                    break;

                // eraser
                case 2:
                    inkCanvasMap.EditingMode = InkCanvasEditingMode.EraseByStroke;
                    break;

                // hand
                case 3:
                    CloseDrawingMode();
                    break;

                // marker
                case 4:
                    OpenDrawingMode();
                    break;

                // facebook share
                case 5:
                    FacebookShare();
                    break;

                // facebook share
                case 6:
                    // invert the boolean and call methods responsible of
                    // 1. loading-activating/deactivating navigation plugin
                    // 2. subscribe/unsubscribe event handles to perform zoomOut on-click
                    isZoomOutOnClick = !isZoomOutOnClick;
                    NavigationPlugIn();
                    ZoomOutOnClick();
                    break;

                // add pushpin to the map
                case 7:
                    AddPin();
                    break;                

                // save settings of language and restart the application
                case 8:
                    ToggleLanguage();
                    break;

                // open file dialog to open Lesson file
                case 9:
                    OpenLessonFile();
                    break;

                default:
                    break;
            }
        }
        #endregion

        #region Event Handlers for controls of Lesson [Note, Marker, Eraser, Scroll, Change Color]
        // colors for the pen and the marker of the Lesson inkCanvas
        private Color lessonPenColor = Colors.Orange;
        private Color lessonMarkerColor = Colors.Yellow;
        
        // change editing mode of LEsson inkCanvas according to checked button
        // edtitng modes are (Ink, Marker, Delete and None)
        private void toggleButton_Click(object sender, RoutedEventArgs e)
        {
            string name = ((System.Windows.Controls.Primitives.ToggleButton)sender).Name;
            // manally unceck other buttons
            ToggleButtons(name);

            // take ink notes 
            if (toggleButton1.IsChecked == true)
            {
                inkCanvasContent.EditingMode = InkCanvasEditingMode.Ink;
                scrollViewerContent.IsManipulationEnabled = false;                
                
            }
            // marker
            else if (toggleButton2.IsChecked == true)
            {
                inkCanvasContent.EditingMode = InkCanvasEditingMode.Ink;
                scrollViewerContent.IsManipulationEnabled = false;       
            }
            // eraser
            else if (toggleButton3.IsChecked == true)
            {
                inkCanvasContent.EditingMode = InkCanvasEditingMode.EraseByStroke;
                scrollViewerContent.IsManipulationEnabled = false;
            }
            else if (toggleButton4.IsChecked == true)
            {
                inkCanvasContent.EditingMode = InkCanvasEditingMode.None;
                scrollViewerContent.IsManipulationEnabled = true;
            }            
            
            // change settings drawing mode of inkCanvas
            ChangeLessonDrawingSettings();
        }

        /// <summary>
        /// when a toggle button is checked, we want to uncheck the others
        /// </summary>
        private void ToggleButtons(string sender)
        {
            List<ToggleButton> buttons = new List<ToggleButton>();
            buttons.Add(toggleButton1);
            buttons.Add(toggleButton2);
            buttons.Add(toggleButton3);
            buttons.Add(toggleButton4);

            foreach (ToggleButton button in buttons)
                if (button.Name != sender)
                    button.IsChecked = false;            
        }

        // change the color of Pen/Marker of the Lesson Content inkCanvas
        private void radColorPicker2_SelectedColorChanged(object sender, EventArgs e)
        {
            // if Pen is checked, change its color, else if Marker is checked change its color
            // else if both of them is unchecked, do nothing

            if (toggleButton1.IsChecked == true)
            {
                lessonPenColor = radColorPicker2.SelectedColor;
                inkCanvasContent.DefaultDrawingAttributes.Color = lessonPenColor;
            }
            else if (toggleButton2.IsChecked == true)
            {
                lessonMarkerColor = radColorPicker2.SelectedColor;
                inkCanvasContent.DefaultDrawingAttributes.Color = lessonMarkerColor;
            }
        }

        /// <summary>
        /// Changes the drawing settings of the inkCanvasContent according to the checked
        /// toggleButton. There are two main setting for two drawing modes: Pen and Marker
        /// </summary>
        private void ChangeLessonDrawingSettings()
        {
            // change drawing settings to the Pen Mode if Pen toggle button is checked
            if (toggleButton1.IsChecked == true)
            {                
                inkCanvasContent.DefaultDrawingAttributes.Height
                    = inkCanvasContent.DefaultDrawingAttributes.Width = 3;
                inkCanvasContent.DefaultDrawingAttributes.StylusTip = StylusTip.Ellipse;
                inkCanvasContent.DefaultDrawingAttributes.IsHighlighter = false;
                // make selection color of the colorPicker equal to the pen color
                radColorPicker2.SelectedColor = lessonPenColor;
            }
            // change drawing settings to the Marker Mode if Marker toggle button is checked
            else if (toggleButton2.IsChecked == true)
            {                
                inkCanvasContent.DefaultDrawingAttributes.Height = 25;
                inkCanvasContent.DefaultDrawingAttributes.Width = 10;
                inkCanvasContent.DefaultDrawingAttributes.StylusTip = StylusTip.Rectangle;
                inkCanvasContent.DefaultDrawingAttributes.IsHighlighter = true;
                // make selection color of the colorPicker equal to the pen color
                radColorPicker2.SelectedColor = lessonMarkerColor;               
            }
        }
        #endregion
                
        // scroll the lesson one title up/down with animation
        private void buttonScroll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = ((Button)sender).Name;
                // check if user click up and currentTitle = 1
                // or he clicked down and currentTitle = titles.Count
                // If so return and do nothing
                if ((currentTitle == 1 && name == "button10")
                    || (currentTitle == titles.Length && name == "button11"))
                    return;

                // scroll one-title up/down
                ScrollLessonContent(sender);
                // focus on this title
                if ((Button)sender == button10)
                FocusOnTitle(currentTitle -1);
                else
                FocusOnTitle(currentTitle + 1);

            }
            catch (Exception)
            {
                SystemSounds.Hand.Play();
                button.Content = "Load File !";
            }
        }

        // new title Choosed, focus on it
        void gridTitle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SystemSounds.Asterisk.Play();
            int selectedGrid = titles.ToList().IndexOf((Grid)sender) + 1;
            FocusOnTitle(selectedGrid);
        }

        // just playing :)
        private void DoOneCompleteRotation()
        {
            //LatLonAlt location = map.Host.Navigation.CameraPosition.Location;
            //CameraParameters parameters = new CameraParameters();
            //parameters.AccelPeriod = parameters.DecelPeriod = 10;
            //parameters.Speed = 40;
            //parameters.TransitionTime = 500;
            //map.Host.Navigation.FlyAround(location, 360, new CameraParameters());            
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //Stroke stroke = inkCanvas1.Strokes[inkCanvas1.Strokes.Count -1];            
            //stroke.Transform(new Matrix(1,0,0,1,0,30), true);
            //textBlockParagraph1.Text          
            //Microsoft.Windows.Themes.ListBoxChrome
            //textBlock.Text = string.Format("One{0}Two", Environment.NewLine);
            //listBox1  

            //var location = LatLonAlt.CreateUsingDegrees(30.0810, 31.3031, 650.0);
            //CameraParameters parameters = new CameraParameters();
            //parameters.TourMode = false;
            //parameters.AccelPeriod = 0.1;
            //parameters.DecelPeriod = 0.6;
            //parameters.MinDecelSpeed = 0.5;
            ////parameters.TransitionTime = 1.5;
            //parameters.Speed = 2.0; // twice as fast
            //map.Host.Navigation.FlyTo(location, pitch, 180, false, parameters);

            //GeodeticViewpoint vp = new GeodeticViewpoint();
            //vp.Position.Location = location;
            //vp.LocalOrientation.RollPitchYaw = new Microsoft.MapPoint.Geometry.VectorMath.RollPitchYaw(0, pitch, 0);
            //DirectAnimatedCameraController dac = new DirectAnimatedCameraController(map.Host);
            //map.Host.CameraControllers.Current = dac;
            //dac.MoveTo(vp, map.Host.CameraControllers.Default);
        }
    }
}

// Trash code
#if false
    private void BlurEffect()
        {
            Effects.BlurEffect effect = new Effects.BlurEffect();
            effect.Radius = 10;
            effect.KernelType = Effects.KernelType.Gaussian;
            effect.RenderingBias = Effects.RenderingBias.Performance;
        }
#endif
