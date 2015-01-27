// Documentation

// this tool heps the teacher to build a lesson with interactive map shapes (pins, polygons and polylines)
// the lesson built is to be used in the Pen.Map application, by the teacher in a classroom
// this tools app. is built using WYSIWYG concept, whixh means that: what the teacher
// do (from text editing and shapes on the map) is what he will get ehen opening this
// lesson in the Pen.Map application

// [### 21.12.2010 ###]
// acronyms مختصرات ومصطلحات
// textBoxT1  textBox holding the text of title number 1 in the lesson
// TextBoxD1  texyBox holding the date of title number 1 in the lesson
// stackPanelT1 stackPanel containing both of textBoxT1 & textBoxD1
// textBoxP51: textBox holding text of the paragraph number1 in the title number 5 in this lesson

// 11: may refer to the ID of Paragraph, PushPin, Polygon, Polyline in the Lesson DataSet
// Pp12: the name of Pushpin number 2 related to Title number 1 in this Lesson
// Pg43: the name of Polygon number 3 related to Title number 4 in this Lesson
// Pl21: the name of Polyline number 1 related to title number 2 in this Lesson

// storing the data will be on this form
// 1. there is lists of the elements in the GUI, example, the following 
//    lists: paragraphs, titles, dates
// 2. when the user finish editing a lesson, all text information displayed 
//    on the GUI will be directly stored in the microsoft access LessonFile
//    the access file will be named after the lesson name
//    files will then stored as a .plf files (penlessonfiles) [## Later not now 23.12.2010 ##]

// retreiving the data and dispalying it on the GUI will be on this form
// 1. Open the access file, load all its data into the dataSet
// 2. loop on the data, create new GUI elements (titles, dates, parapgraphs,
//    pins, polygons, polylines) and finally fill them with these date

// [### 25.12.2010 ###]
// Scenario of a popup window of adding a new pushpin
//  if user clicked on ok (buttonPopup1), then add pushpin and close popup
//  else if he clicked on cancel (buttonPopup1) close poup
//  else don't change the sate of the cursor of the toggleButton1
//  becuase this means that the user wants to add another button in another place

// [## 26.12.2010 ###]
// when opening a lesson file, we load titlesPositions(labels) to the map
// if titlesPosition.Altitude = 0, don't load the tabel to the map, because this means
// that this label has initial value and has not been explicitly set by user
// because if we load labels with altitude = 0, this leads to crashes

// titlesPosition: 
//  For every Title in the Lesson, there is 
//  corresponding poition to it on the map. This posititon is set
//  by adding a label has a text = the corresponding titleID
//  this position is important, becuase it moves along the map from a certain position
//  to another when the teacher chooses a certaib title

// One more point to go is that: implement loading and saving of polygons and polylines

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
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using Microsoft.MapPoint.Rendering3D;
using Microsoft.MapPoint.Rendering3D.Utility;
using Microsoft.MapPoint.Rendering3D.Atmospherics;
using System.Data.OleDb;
using System.Data;
using System.Media;
using System.Windows.Media.Animation;
using Microsoft.MapPoint;
using Microsoft.MapPoint.Geometry.VectorMath;
using Microsoft.MapPoint.Rendering3D.Cameras;
using Microsoft.MapPoint.Rendering3D.Control;
using System.Threading;

namespace Pen.Map
{
    /// <summary>
    /// Interaction logic for LessonTool.xaml
    /// This Window offers a small tool for the teacher to help him build a good lesson
    /// with a good illustration flow. He can set tiles for his lesson. For each title,
    /// he can set paragraphs and shapes on the map (e.g. pushpin, polygon and plyline).
    /// </summary>
    public partial class LessonTool : Window
    {
        // textBox date foreColor #FF0588C1
        // textBox title background #130082B7 // this is very important value

        // related to map control
        private GlobeControl map;        
        private const double MILLION = 1000000;
        // initial position values of the map
        private double lat = 23.5, lon = 28.5, alt = 5000000D, pitch = -90;
        // required to number the titles with arabic numbers
        private char[] ArabicNums;

        // constructor
        public LessonTool()
        {            
            InitializeComponent();

            // load appropriate UI language according to saved settings
            if (Properties.Settings.Default.LessonToolIsEnglish)
                ConvertToEnglish();
            else
                ConvertToArabic();
        }

        private void windowMain_Loaded(object sender, RoutedEventArgs e)
        {
            // convert this wnidow to a glassy window
            Pen.Tools.GlassyWindow.LoadGlassyWindow(this);
            // load Bing Maps
            LoadMap();
            map.Host.WorldEngine.SetWindowsCursor(System.Windows.Forms.Cursors.Arrow);
            // subscribe to events responsible of scrolling scrollViewrContent
            StartScrollingEvents();
            // build dataSet required to manipulate with database
            BuildLessonDataSet();
            // Popup intializing
            InitializePopup();            
        }

        private void windowMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DisposeMap();
        }

        /// <summary>
        /// Converts the application window and its content to Arabic
        /// with right-to-left flow direction
        /// </summary>
        private void ConvertToArabic()
        {
            this.FlowDirection = System.Windows.FlowDirection.RightToLeft;
            
            // convert any text found on the window to its meaning in arabic
            ArabicNums = new char[] { '٠', '١', '٢', '٣', '٤', '٥', '٦', '٧', '٨', '٩' };
            this.Title = "  أ د ا ة  تــنــفــيـذ  ا لـد ر س";
            this.labelLesson.Content = "العنــــــــــــــــــــــــــــ                                 ـــــــــــــــــــــوان";
        }
        
        /// <summary>
        /// Prepare this application to start in English language.
        /// </summary>
        private void ConvertToEnglish()
        {
            // convert any text found on the window to its meaning in arabic
            ArabicNums = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            this.Title = "Lesson Building Tool";
            this.labelLesson.Content = "Lesson                                                              Title";
            Canvas.SetLeft(labelLesson, 85);

            this.textBlockButton1.Text = "Open";
            this.textBlockButton2.Text = "Save";
            this.textBlockButton3.Text = "New";
            this.textBlockButton4.Text = "Title";
            this.textBlockButton5.Text = "Prgrph";

            this.textBlockToggleButton1.Text = "Pin";
            this.textBlockToggleButton2.Text = "Shape";
            this.textBlockToggleButton3.Text = "Line";
            this.textBlockToggleButton4.Text = "Place";

            this.textBlockColorPicker.Text = "Color";

            this.button1.Width += 14;
            this.button2.Width -= 6;
            this.button3.Width -= 5;
            this.button4.Width -= 9;
            this.button5.Width += 17;

            Canvas.SetLeft(button2, 110); // 12, 99, 198, 298, 396
            Canvas.SetLeft(button3, 203);
            Canvas.SetLeft(button4, 297);
            Canvas.SetLeft(button5, 387);

            toggleButton2.Width += 10;
            Canvas.SetLeft(toggleButton3, 712);
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

        #region Create custom cursors
        private enum Cursors_ { Pin, Polygon, Polyline, Position };        
        private System.Windows.Forms.Cursor cursorPin(Cursors_ cursorType)
        {
            string file = string.Empty;

            System.Windows.Forms.Cursor cursor = null;

            switch (cursorType)
            {
                case Cursors_.Pin:
                    file = "\\Cursors\\pin.png";
                    break;
                case Cursors_.Polygon:
                case Cursors_.Polyline:
                    file = "\\Cursors\\crosshair.png";
                    break;
                case Cursors_.Position:
                    file = "\\Cursors\\position.png";
                    break;
                default:
                    break;
            }

            //System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(Environment.CurrentDirectory + file);            
            cursor = new System.Windows.Forms.Cursor(bmp.GetHicon());
            return cursor;
        }
        #endregion

        #region Load and Dispose the map
        /// <summary>
        /// Load the map
        /// </summary>
        private void LoadMap()
        {
            map = new GlobeControl();
            controlHost.Child = map;
            //map = (GlobeControl)this.controlHost.Child;

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
            map.StartAltitude = alt;
            map.StartLatitude = lat;
            map.StartLongitude = lon;
            map.StartHeading = 0D;
            map.StartPitch = -90D;
            map.UseDefaultConfiguration = true;

            map.Host.RenderEngine.FirstFrameRendered += new EventHandler((object sender, EventArgs e) =>
            {
                // links for some databindingsources, 
                // the internet link for topic discussing that: http://blogs.msdn.com/b/virtualearth3d/archive/2009/04/06/data-format-revision.aspx
                string road = "http://www.bing.com/maps/Manifests/RT2.xml";
                string aerial = "http://www.bing.com/maps/Manifests/AT2.xml";
                string hyprid = "http://www.bing.com/maps/Manifests/HT2.xml";
                string digitalElevationModel = "http://www.bing.com/maps/Manifests/HD2.xml";
                string models = "http://www.bing.com/maps/Manifests/MO2.xml";

                map.Host.DataSources.Add(new DataSourceLayerData("Elevation", "Elevation", @"http://bing.com/maps/Manifests/HD.xml", DataSourceUsage.ElevationMap));
                map.Host.DataSources.Add(new DataSourceLayerData("Texture", "Texture", @"http://bing.com/maps/Manifests/AT.xml", DataSourceUsage.TextureMap));
                map.Host.DataSources.Add(new DataSourceLayerData("Model", "Model", @"http://bing.com/maps/Manifests/MO.xml", DataSourceUsage.Model));
                // turn on the nice atmosphere
                map.Host.WorldEngine.Environment.AtmosphereDisplay = EnvironmentManager.AtmosphereStyle.Scattering;                                
                //map.Host.WorldEngine.Environment.CelestialDisplay = EnvironmentManager.CelestialStyle.Regular;
                //map.Host.WorldEngine.Environment.SunPosition = SunPosition.FromRealtime();
                //map.Host.RenderEngine.Graphics.Settings.UseAnisotropicFiltering = true;
                // create layers for labels, pins, shapes
                map.Host.Geometry.AddLayer(pinLayer);
                map.Host.Geometry.AddLayer(labelLayer);
                map.Host.Geometry.AddLayer(polyLayer);                
                // much more settings                
                map.Host.WorldEngine.AllowFrameRender = true;
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
                map.Host.WorldEngine.SetWindowsCursor(System.Windows.Forms.Cursors.Arrow);

                // load contol plugin            
                Microsoft.MapPoint.PlugIns.PlugInLoader loader = Microsoft.MapPoint.PlugIns.PlugInLoader.CreateLoader(map.Host, AppDomain.CurrentDomain);
                Guid plugingId = loader.LoadPlugIn(typeof(Microsoft.MapPoint.Rendering3D.NavigationControl.NavigationPlugIn));
                loader.ActivatePlugInOnRenderThread(plugingId, null);               
                // event handlers
                map.Click += new EventHandler(map_Click);

                // i know this is fool but it is the way shomething goes to
                // due to a crash happens duw to loading custom images form the internet
                // for the first time, we sould add a pen and remove it again, ha ha ha 
                map.Host.Geometry.AddGeometry(new PushpinGeometry(pinLayer, "x", new LatLonAlt(0, 0, 0), GetPushPinStyle("")));
                map.Host.Geometry.RemoveGeometry(pinLayer, "x");
            });
        }
                
        /// <summary>
        /// Safely close thr map
        /// </summary>
        private void DisposeMap()
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
        #endregion

        #region manage color Selection
        private System.Drawing.Color polygonColor = System.Drawing.Color.FromArgb(155, 255, 220, 0);
        private System.Drawing.Color polylineColor = System.Drawing.Color.White;
        private void radColorPicker1_SelectedColorChanged(object sender, EventArgs e)
        {
            int r = radColorPicker1.SelectedColor.R,
                g = radColorPicker1.SelectedColor.G,
                b = radColorPicker1.SelectedColor.B;

            // set color for polygon/polyline according to the current selected toggleButton 2,3
            // if no suitable toggleButton is pressed, then the new selected 
            // color will be applied on both of polygon/ polyline, but polygon 
            // color is transparent while polyline color is solid
            if (toggleButton2.IsChecked == true)
                polygonColor = System.Drawing.Color.FromArgb(140, r, g, b);
            else if (toggleButton3.IsChecked == true)
                polylineColor = System.Drawing.Color.FromArgb(255, r, g, b);
            else
            {
                polygonColor = System.Drawing.Color.FromArgb(140, r, g, b);
                polylineColor = System.Drawing.Color.FromArgb(255, r, g, b);
            }


        }
        #endregion

        #region Manage popup window of adding pushpin
        // if user clicked on ok (buttonPopup1), then add pushpin and close popup
        // else if he clicked on cancel (buttonPopup1) close poup
        // else don't change the sate of the cursor of the toggleButton1
        // becuase this means that the user wants to add another button in another place

        // make popup ready
        private void InitializePopup()
        {
            canvasMain.Children.Remove(canvasPopup);
            popup.Child = canvasPopup;
        }

        // add new pushpin
        private void buttonPopup1_Click(object sender, RoutedEventArgs e)
        {            
            AddPin();
            PopupClose();            
        }

        // close pop up without adding a pushpin
        private void buttonPopup2_Click(object sender, RoutedEventArgs e)
        {
            PopupClose();
        }
        
        /// <summary>
        /// Close popup, unkceck button of adding a pin (toggleButton1),
        /// return cursor shape to default and clear textBox of popup
        /// </summary>
        private void PopupClose()
        {
            popup.IsOpen = false;
            toggleButton1.IsChecked = false;
            map.Cursor = System.Windows.Forms.Cursors.Arrow;
            textBoxPopup.Text = string.Empty;
        }

        #endregion

        #region Implement Scrolling of Lesson Content
        // Scrolling by two approaches 1.Smooth Touch Scrolling 2.Ordinary Mouse Scrolling        
        private bool isMouseDown = false;
        private double y1 = 0, y2 = 0;
        
        // initialize event handlers required to perform smooth scrolling
        private void StartScrollingEvents()
        {
            scrollViewerContent.ManipulationStarting += new EventHandler<ManipulationStartingEventArgs>(scrollViewer1_ManipulationStarting);
            scrollViewerContent.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(scrollViewer1_ManipulationDelta);
            scrollViewerContent.ManipulationInertiaStarting += new EventHandler<ManipulationInertiaStartingEventArgs>(scrollViewer1_ManipulationInertiaStarting);
            scrollViewerContent.IsManipulationEnabled = true;

            scrollViewerContent.MouseDown += new MouseButtonEventHandler(scrollViewerContent_MouseDown);
            scrollViewerContent.MouseMove += new MouseEventHandler(scrollViewerContent_MouseMove);
        }

        // this is ordinary scrolling (i.e using mouse events and no touch events) 
        // in case of there is no touch device connnected
        void scrollViewerContent_MouseDown(object sender, MouseButtonEventArgs e)
        {
            y1 = e.MouseDevice.GetPosition(scrollViewerContent).Y;
        }
        void scrollViewerContent_MouseMove(object sender, MouseEventArgs e)
        {
            y2 = e.MouseDevice.GetPosition(scrollViewerContent).Y;
            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
                scrollViewerContent.ScrollToVerticalOffset(scrollViewerContent.VerticalOffset - (y2 - y1));
            y1 = y2;
        }

        // start of manipulation (equivilant to touchDown + first touchMove events)
        void scrollViewer1_ManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            e.ManipulationContainer = canvasMain;
            e.Handled = true;
            e.Mode = ManipulationModes.TranslateY;
        }

        // when there is movement updates (equivilant to touchMove)
        void scrollViewer1_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            scrollViewerContent.ScrollToVerticalOffset(scrollViewerContent.VerticalOffset - e.DeltaManipulation.Translation.Y);
        }

        // when the touch is released from the control (equivilant to TouchUp or TouchLeave)
        void scrollViewer1_ManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            // adjust the dispalcement behaviour
            // (10 inches * 96 DIPS per inch / 1000ms^2)
            e.TranslationBehavior.DesiredDeceleration = 2.0 * 96.0 / (1000.0 * 1000.0);
            e.TranslationBehavior.InitialVelocity = e.InitialVelocities.LinearVelocity;
            e.TranslationBehavior.DesiredDisplacement = Math.Abs(e.InitialVelocities.LinearVelocity.Y) * 300;
        }
        #endregion

        #region Database Mainpluations Save file, open file, edit file

        // Ojects required to acess to MS-Access Files
        private string fileName;
        private string templateFilePath = Environment.CurrentDirectory + "\\Template.pmf";
        private string CONN_STRING = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=";
        private string CONN_SOURCE;

        // DataSet needed to deal with data in the access lessonfile
        private DataSet dataSet = new DataSet("LessonData");
        private List<TextBox> titles = new List<TextBox>();
        private List<TextBox> dates = new List<TextBox>();
        private List<TextBox> paragraphs = new List<TextBox>();
        // array to save map positions for Lesson Titles
        private List<LatLonAlt> titlePositions = new List<LatLonAlt>();
        // this is a dummy map position
        private LatLonAlt zeroPosition = LatLonAlt.CreateUsingDegrees(0, 0, 0);

        /// <summary>
        /// Create a dataSet required to handle the matter of LessonData then
        /// Load dataSet schema from Template access file
        /// </summary>
        private void BuildLessonDataSet()
        {
            // add the current-exist elements (exist in the lesson content) to their lists
            titles.Add(this.textBoxT1);
            titlePositions.Add(zeroPosition);
            dates.Add(this.textBoxD1);
            paragraphs.Add(this.textBoxP11);
            this.textBoxT1.Text = ArabicNums[1] + ".  ";
            textBoxT1.GotFocus += new RoutedEventHandler(textBoxTitle_GotFocus);
            // focus on title1 stackPanel
            TitleFocus(1);            
            // set selectionPeam at the end of the text
            textBoxT1.SelectionStart = textBoxT1.Text.Length;           

            // add tables to the dataSet
            dataSet.Tables.Add("Title");
            dataSet.Tables.Add("Paragraph");
            dataSet.Tables.Add("Pin");
            dataSet.Tables.Add("Poly");
            // load schema of the table from DataBase
            CONN_SOURCE = templateFilePath;
            string commandString = "";
            using (OleDbCommand dbCommand = new OleDbCommand())
            {
                dbCommand.Connection = new OleDbConnection(CONN_STRING + CONN_SOURCE);
                dbCommand.Connection.Open();

                // load schemas of all tables(Title, Paragraph, Pin and Poly)
                // from the Template access file
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
        }

        /// <summary>
        /// Clear the tables is the dataSet 
        /// </summary>
        private void ClearDataSet()
        {
            dataSet.Tables["Paragraph"].Clear();
            dataSet.Tables["Title"].Clear();
            dataSet.Tables["Pin"].Clear();
            dataSet.Tables["Poly"].Clear();
        }

        /// <summary>
        /// Cleae lists holding elements of the Lesson Content (dates, titles and paragraphs lists)
        /// </summary>
        private void ClearElementLists()
        {
            titles.Clear();
            titlePositions.Clear();
            dates.Clear();
            paragraphs.Clear();
        }

        /// <summary>
        /// 1. create new access file whose name is the Lesson Name 2. empty the data of both
        /// the GUI elements (titles, dates, parapgraphs) and the map (pushpins, titlePositions,
        /// polygons and polylines) into this file and return a boolen to indicate if the user
        /// clicked save in the saveFile dialog or not
        /// </summary>
        private bool SaveFile()
        {
            // file name is AYB_MM.DD_HH.MM.SS [Month.Day_Hour.Minute.Second]
            string fileName = this.textBoxLesson.Text;
            string targetPath = "C:\\Users\\" + Environment.UserName + "\\Desktop";

            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog fileDialog = new Microsoft.Win32.SaveFileDialog();
            fileDialog.FileName = fileName;
            fileDialog.Title = "Save As ...";
            fileDialog.DefaultExt = ".pmf";
            fileDialog.Filter = "Pen Maps File |*.pmf";
            fileDialog.InitialDirectory = targetPath;
            if (fileDialog.ShowDialog() == true)
            {
                // copy file
                System.IO.File.Copy(templateFilePath, fileDialog.FileName, true);
                // Now save date in this newly created access file                
                CONN_SOURCE = fileDialog.FileName;

                using (OleDbCommand dbCommand = new OleDbCommand())
                {
                    dbCommand.Connection = new OleDbConnection(CONN_STRING + CONN_SOURCE);
                    dbCommand.Connection.Open();

                    // null elements
                    UIElement child;
                    StackPanel stackPanel;
                    Grid_ grid;
                    int titleCounter = 0, paragraphCounter = 0;
                    // loop on the GUI elements of the Lesson content, save Lesson
                    // data in them to Access File and finally empty them
                    for (int i = 1; i < stackPanelContent.Children.Count; i++)
                    {
                        child = stackPanelContent.Children[i];
                        // Save 1.title, date, titlePosition
                        //      2.Pushpins related to this title
                        if (child is StackPanel)
                        {
                            // 1. Save title, data and titlePosition
                            // increment title counter and reset paragraph counter
                            titleCounter++;
                            paragraphCounter = 0;
                            stackPanel = (StackPanel)child;
                            // schema of [Title] table = [TitleID](number), [Text_](text), [Date_](text), [Lat](number), [Lon](number), (Alt)(number)
                            dbCommand.CommandText = "Insert Into Title Values(" + titleCounter.ToString() + ", '" + ((TextBox)stackPanel.Children[0]).Text + "', '" + ((TextBox)stackPanel.Children[1]).Text + "', " + titlePositions[titleCounter-1].LatitudeDegrees + ", " + titlePositions[titleCounter -1].LongitudeDegrees + ", " +  titlePositions[titleCounter-1].Altitude +")";
                            dbCommand.ExecuteNonQuery();

                            // 2. Save Pins
                            // get pins related to this title (with LINQ) and save them to database (with SQL)                            
                            // schema of [Pin] table = [TitleID](number), [PinID](number), [Lat](number), [Lon](number), [Text_](text)
                            var rows = from DataRow row in dataSet.Tables["Pin"].Rows
                                               //select new { col1 = dRow["dataColumn1"], col2 = dRow["dataColumn2"] };
                                               where (int)row["TitleID"] == titleCounter
                                               select row;
                            foreach (DataRow row in rows)
                            {
                                int pinID = (int)row["PinID"];
                                double lat = (double)row["Lat"];
                                double lon = (double)row["Lon"];
                                string text = (string)row["Text_"];                                
                                dbCommand.CommandText = "Insert Into Pin Values(" + titleCounter + ", " + pinID + ", " + lat + ", " + lon + ", '" + text + "')";
                                dbCommand.ExecuteNonQuery();                                
                            }
                           
                        }
                        else if (child is Grid_)
                        {
                            paragraphCounter++;
                            grid = (Grid_)child;
                            // schema of [Paragraph] table = [TitleID](number), [ParagraphID](number), [Text_](text)
                            dbCommand.CommandText = "Insert Into Paragraph Values(" + titleCounter.ToString() + ", '" + ((titleCounter * 10) + paragraphCounter).ToString() + "', '" + ((TextBox)grid.Children[1]).Text + "')";
                            dbCommand.ExecuteNonQuery();
                        }
                        else
                            MessageBox.Show("There must be an error in the algorithm used in this method SaveFile()");                        
                    }

                    // finally save polys (polygons and polylines) from the dataset to the database
                    // schema of [Poly] table = [TitleID](number), [PolyID](number), [Lat](number), [Lon](number), (Alt)(number), [Color](text), [Type](True/False)
                    foreach (DataRow row in dataSet.Tables["Poly"].Rows)
                    {
                        dbCommand.CommandText = "Insert Into Poly Values(" + row["TitleID"] + ", " + row["PolyID"] + ", " + row["Lat"] + ", " + row["Lon"] + ", " + row["Alt"] + ", '" + row["Color"] + "', " + row["Type"] + ")";
                        dbCommand.ExecuteNonQuery();
                    }
                    
                    // close the connection                   
                    dbCommand.Connection.Close();
                }
                return true;
            }

            else
                return false;
        }

        /// <summary>
        /// Prepare the GUI for a new lesson by doing the following:
        /// 1. Clear dataset 2. Clear Lists of Content Elements(titles, dates, paragraphs)
        /// 3. Clear the map layers 4. Set focus to the title1
        /// </summary>
        private void NewFile()
        {
            // Clear the map
            ClearMap();
            // Clear the tables is the dataSet
            ClearDataSet();
            // clear the lists containing elements of Lesson Content, except the first element
            titles.RemoveRange(1, titles.Count - 1);
            titlePositions.Clear();
            titlePositions.Add(zeroPosition);
            dates.RemoveRange(1, titles.Count - 1);
            paragraphs.RemoveRange(1, titles.Count - 1);
            // Clear the GUI, remove all elements of LessonContent except first title and paragraph
            stackPanelContent.Children.RemoveRange(3, stackPanelContent.Children.Count);            

            // focus the first title and set currently selected titleID = 1;
            currentTitle = 1;
            TitleFocus(1);

            // clear text in the lesson, title, date and parapgraph textBoxes
            StackPanel child = (StackPanel)stackPanelContent.Children[1];
            this.textBoxLesson.Text = string.Empty;
            ((TextBox)child.Children[0]).Text = ArabicNums[1] + ".  ";
            ((TextBox)child.Children[1]).Text = string.Empty;
            ((TextBox)(((Grid_)(stackPanelContent.Children[2])).Children[1])).Text = string.Empty;            
        }

        /// <summary>
        /// get the full access file path in order to save Lesson content to it
        /// then load its data to dataSet 
        /// </summary>
        private bool OpenFile()
        {
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.Title = "Open ...";
            fileDialog.DefaultExt = ".pmf";
            fileDialog.Filter = "Pen Maps File |*.pmf";
            fileDialog.InitialDirectory = "C:\\Users\\" + Environment.UserName + "\\Desktop";                        
            // check if user clicked ok, else leave this method
            if (!fileDialog.ShowDialog() == true)
            {
                SystemSounds.Exclamation.Play();
                return false;
            }

            // now after a successful ShowFileDialog Scenario
            // load the access database into the dataSet
            CONN_SOURCE = fileDialog.FileName;
            // get file name because the current 
            // lesson title is named after the file name
            fileName = fileDialog.SafeFileName;
            string commandString = "";

            // clear dataset before filling it
            ClearDataSet();

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
                return true;
            }
        }

        /// <summary>
        /// Move data from dataSet to Elements in Lists 
        /// (as paragraph list, title list, date list )
        /// </summary>
        private void FillElementLists()
        {
            // clear all elements that were holding old lesson
            ClearElementLists();

            // Lesson Name = fileName with out the extension
            char[] chars = { '.', 'a', 'c', 'c', 'd', 'b' };
            textBoxLesson.Text = fileName.TrimEnd(chars);
            // fill the title list
            TextBox newTextBox;
            for (int i = 0; i < dataSet.Tables["Title"].Rows.Count; i++)
            {
                newTextBox = new TextBox();
                newTextBox.Name = "textBoxT" + (i + 1).ToString();
                newTextBox.Style = (Style)TryFindResource("TextBoxTitleStyle");
                newTextBox.Text = dataSet.Tables["Title"].Rows[i]["Text_"].ToString();
                // add event handler for mouse up (click) event
                // which is needed to handle the scenario of loading shapes
                // on the map for each title
                newTextBox.GotFocus += new RoutedEventHandler(textBoxTitle_GotFocus);
                titles.Add(newTextBox);
                titlePositions.Add(LatLonAlt.CreateUsingDegrees(
                    (double)dataSet.Tables["Title"].Rows[i]["Lat"],
                    (double)dataSet.Tables["Title"].Rows[i]["Lon"],
                    (double)dataSet.Tables["Title"].Rows[i]["Alt"]));

                // fill the date list
                newTextBox = new TextBox();
                newTextBox.Name = "textBoxD" + (i + 1).ToString();
                newTextBox.Style = (Style)TryFindResource("TextBoxDateStyle");
                newTextBox.Text = dataSet.Tables["Title"].Rows[i]["Date_"].ToString();
                dates.Add(newTextBox);
            }
            // fill the parapgraph list
            for (int i = 0; i < dataSet.Tables["Paragraph"].Rows.Count; i++)
            {
                newTextBox = new TextBox();
                newTextBox.Name = "TextBoxP" + dataSet.Tables["Paragraph"].Rows[i]["PID"].ToString();
                newTextBox.Style = (Style)TryFindResource("TextBoxParagraphStyle");
                newTextBox.Text = dataSet.Tables["Paragraph"].Rows[i]["Text_"].ToString();
                paragraphs.Add(newTextBox);
            }
        }

        /// <summary>
        /// Load the GUI with the elements from the lists (titles, paragraphs)
        /// and load map geometry to it (pins, titlePosition labels and polys)
        /// </summary>
        private void LoadGUI()
        {
            // first, clear the map
            ClearMap();
            // clear the GUI, i.e. remove all elements except Lesson Name textBox
            stackPanelContent.Children.RemoveRange(1, stackPanelContent.Children.Count - 1);

            StackPanel stackPanel;
            Grid_ grid;
            Rectangle rectangle;

            // 1. load titles, dates to the GUI
            for (int i = 0; i < titles.Count; i++)
            {
                stackPanel = new StackPanel();
                stackPanel.Style = (Style)TryFindResource("StackPanelTitleStyle");
                stackPanel.Children.Add(titles[i]);
                stackPanel.Children.Add(dates[i]);
                stackPanelContent.Children.Add(stackPanel);
                // add the corrsponding label to the current title[i] to the map
                // if position.Altitude = 0, don't load the tabel to the map, because this means
                // that this label has initial value and has not been explicitly set by user
                // because if we load labels with altitude = 0, this leads to crashes (i think so)               
                if (titlePositions[i].Altitude != 0)
                    map.Host.Geometry.AddGeometry(GetLabel(titlePositions[i], GetLabelStyle(GetArabicNumber(i + 1)), (i + 1).ToString()));

                // load paragraphs to the GUI
                for (int j = 0; j < paragraphs.Count; j++)
                    if (Convert.ToInt32(paragraphs[j].Name.Substring(8, 1)) == i + 1)
                    {
                        grid = new Grid_();
                        grid.Style = (Style)TryFindResource("GridParagraphStyle");
                        rectangle = new Rectangle();
                        rectangle.Style = (Style)TryFindResource("RectangleParagraphStyle");
                        grid.Children.Add(rectangle);
                        grid.Children.Add(paragraphs[j]);
                        stackPanelContent.Children.Add(grid);
                    }
            }

            // bacuase loading polys and ploting it to the map could take some time
            // it has it's won method with starts in a separate thread
            LoadPolys();

            // set focus mode to the first title stackPanel
            // change currentTitle into another number rather than 2
            // becuase current itle now in 1 and if we call textBoxTitleFocus()
            // with parameter = titles[0] then titleID = currentTitle so nothing happens            
            textBoxTitleFocus(titles[0]);
            // set selectionPeam at the end of the text
            titles[0].SelectionStart = titles[0].Text.Length;            
        }
                
        /// <summary>
        /// get information of polys(polygons and polylines) from the database
        /// and draw these polys to the map
        /// </summary>
        private void LoadPolys()
        {
            // load and draw polys (polylines and polygons) to the map using these steps
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
                    AddPolygon(points, colors[i], false);
                else if (types[i] == false)
                    AddPolyline(points, colors[i], false);               
            }

        }
        #endregion

        #region Adding shapes to the map [pins, titlePosition labels, polys]
        // layers and counters for geometry shapes of the map        
        private string pinLayer = "pins";
        private string labelLayer = "labels";
        private string polyLayer = "polys";
        private string temporaryLayer = "temporaryLayer";
        private int polyCounter, pinCounter;
        // keeps a record for ID's of currently selected title
        // we need to know the currently selected title in order to load the shapes
        // on the map related to this selected title        

        
        /// <summary>
        /// adding geompetry pushpin to the map
        /// </summary> 
        private void AddPin()
        {
            // pincounter increment
            pinCounter++;

            // set position
            LatLonAlt position = LatLonAlt.CreateUsingDegrees(
                map.Host.Navigation.PointerPosition.Location.LatitudeDegrees,
                map.Host.Navigation.PointerPosition.Location.LongitudeDegrees, 0);

            PushpinGeometry pin = new PushpinGeometry(pinLayer, pinCounter.ToString(), position, GetPushPinStyle(GetArabicNumber(pinCounter)));
            map.Host.Geometry.AddGeometry(pin);
            // finally add this pin to the database
            // [Pin](table) = [TitleID, PinID, Log, Lat](number), [Text_](string)
            dataSet.Tables["Pin"].Rows.Add(currentTitle, (currentTitle * 10) + pinCounter, position.LatitudeDegrees, position.LongitudeDegrees, textBoxPopup.Text);
        }

        /// <summary>
        /// takes the label of the pushpin Geometry and get the style of a pushpin
        /// </summary>        
        private PushpinInfo GetPushPinStyle(string label)
        {
            return GeometryStyle.GetPushPinStyle(label);
        }

        /// <summary>
        /// takes parameters of a pushpin and returns a pushpin
        /// </summary>        
        private PushpinGeometry GetPin(double lat, double lon, PushpinInfo style, string pinID)
        {
            LatLonAlt position = LatLonAlt.CreateUsingDegrees(lat, lon, 0.0);
            PushpinGeometry pin = new PushpinGeometry(pinLayer, pinID, this, position, style);
            return pin;
        }
        
        /// <summary>
        /// Add label to the map. For every Title in the Lesson, there is 
        /// corresponding poition to it on the map. This posititon is set
        /// by adding a label with this method
        /// </summary>
        private void AddPositionLabel()
        {
            LatLonAlt position = LatLonAlt.CreateUsingDegrees(
                map.Host.Navigation.PointerPosition.Location.LatitudeDegrees,
                map.Host.Navigation.PointerPosition.Location.LongitudeDegrees, 0);                
           
            // check if labelLayer has a label with ID = currentTitle
            // then change its position                    
            if (map.Host.Geometry.ContainsGeometry(labelLayer, currentTitle.ToString()))
                map.Host.Geometry.RemoveGeometry(labelLayer, currentTitle.ToString());

            // create a new label whose ID = currentTitle    
            LabelGeometry label = new LabelGeometry(labelLayer, currentTitle.ToString(), position, GetLabelStyle(GetArabicNumber(currentTitle)));
            map.Host.Geometry.AddGeometry(label);

            // modify values of (lat, lon, alt) in the dataset, related to the selected title
            // notice that the latitude of the label is the altitude of tha map
            // when this title is created
            position.Altitude = map.Host.Navigation.CameraPosition.Altitude;
            titlePositions[currentTitle - 1] = position;
        }

        /// <summary>
        /// takes the text/label of the labelGeometry and get the style of a labelGeometry        
        /// </summary>
        /// <param name="label"></param>        
        private LabelInfo GetLabelStyle(string labelText)
        {
            return GeometryStyle.GetLabelStyle(labelText);
        }

        /// <summary>
        /// takes parameters of a labelGeometry and returns a labelGeometry
        /// </summary>        
        private LabelGeometry GetLabel(LatLonAlt position, LabelInfo style, string labelID)
        {
            position.Altitude = 0;
            LabelGeometry label = new LabelGeometry(labelLayer, labelID, position, style);
            return label;
        }

        /// <summary>
        /// Add a point on the map that reperesents a corner in the plygon being
        /// currently drawn by user. Also add this point to polygonPoints List
        /// </summary>
        private void AddPolygonPoint()
        {
            // create a point from map position
            LatLonAlt position = LatLonAlt.CreateUsingDegrees(
                map.Host.Navigation.PointerPosition.Location.LatitudeDegrees,
                map.Host.Navigation.PointerPosition.Location.LongitudeDegrees, 0);

            // add it to polygonPoints
            polygonPoints.Add(position);

            // create a style
            LabelInfo style = LabelInfo.Default;
            style.AltitudeMode = AltitudeMode.FromGround;
            style.FadeInAndOut = true;
            style.Label = "•";
            style.FontName = "Arial";
            style.FontColor = System.Drawing.Color.White;
            style.GlowColor = System.Drawing.Color.Red;
            style.GlowSize = 1;
            style.HitDetect = HitDetectMode.On;

            // create a label and load it to the map            
            LabelGeometry label = new LabelGeometry(temporaryLayer, polygonPoints.Count.ToString(), position, style);
            map.Host.Geometry.AddGeometry(label);

        }

        /// <summary>
        /// add polygon(shape) to the map
        /// </summary> 
        private void AddPolygon(List<LatLonAlt> polygonPoints)
        {
            AddPolygon(polygonPoints, polygonColor, true);            
        }

        ///<summary>
        ///Add polylineGeopemtry to the map
        ///</summary>
        private void AddPolyline(List<LatLonAlt> polylinePoints)
        {
            AddPolyline(polygonPoints, polylineColor, true);
        }

        /// <summary>
        /// add polygon(shape) to the map whose fill color is given
        /// </summary> 
        private void AddPolygon(List<LatLonAlt> points, System.Drawing.Color color, bool saveToDataset)
        {
            // the polygon added should be visible at all altitudesunder this condition:
            // all of its points are with in the visible area of the map
            double polygonAlt = map.Host.Navigation.CameraPosition.Altitude;

            // 600          => 1
            // 15 million   => 31            
            // altitude     => ?
            //polygonAlt = 0.5 * (polygonAlt * 31.0) / (15 * MILLION);

            polyCounter++;

            PolygonGeometry polygon = new PolygonGeometry(polyLayer, polyCounter.ToString(), null, points.ToArray(), PolygonGeometry.PolygonFormat.Polygon2D, GeometryStyle.GetPolygonStyle(color));
            map.Host.Geometry.AddGeometry(polygon);

            // finally save points of the poly to dataset
            if (saveToDataset)
                SavePolyToDataset(polygonPoints, color, true);
        }

        ///<summary>
        ///Add polylineGeopemtry to the map whose fill color is given
        ///</summary>
        private void AddPolyline(List<LatLonAlt> points, System.Drawing.Color color, bool saveToDataset)
        {
            polyCounter++;

            PolylineGeometry polyline = new PolylineGeometry(polyLayer, polyCounter.ToString(), null, points.ToArray(), PolylineGeometry.PolylineFormat.Polyline2D, GeometryStyle.GetPolylineStyle(color));
            map.Host.Geometry.AddGeometry(polyline);

            // finally save points of the poly to dataset
            if (saveToDataset)
                SavePolyToDataset(polygonPoints, color, false);
        }

        /// <summary>
        /// Save points of a poly into dataSet
        /// </summary>        
        private void SavePolyToDataset(List<LatLonAlt> points, System.Drawing.Color color, bool shapeType)
        {
            // add the poly to the dataset
            // schema of [Poly] table = [TitleID](number), [PolyID](number), [Lat](number), [Lon](number), (Alt)(number), [Color](text), [Type](True/False)
            string colorString;            
            foreach (LatLonAlt point in polygonPoints)
            {
                // there is a certain format for color text, eg. if we want to save 
                // this color.fromArgb()= (255, 10, 200, 40) = "255.10.200.40"
                // the type column is true if poly is polygon, false if poly is polyline
                colorString = color.A.ToString() + "." +  color.R.ToString() + "." + color.G.ToString() + "." +  color.B.ToString();
                dataSet.Tables["Poly"].Rows.Add(
                    currentTitle,
                    (currentTitle * 10) + polyCounter,
                    point.LatitudeDegrees,
                    point.LongitudeDegrees,
                    point.Altitude, colorString, shapeType.ToString());
            }
        }

        /// <summary>
        /// Clear the map from all figures assigned to its layers
        /// Reset shapes counters to zero
        /// </summary>
        private void ClearMap()
        {
            // clear the map att all, then reassign the layers again
            map.Host.Geometry.Clear();
            map.Host.Geometry.AddLayer(pinLayer);
            map.Host.Geometry.AddLayer(polyLayer);

            // reset counters to zero
            pinCounter = polyCounter = 0;
        }        
        #endregion

        #region Add new Elements (titles, paragraph) to the Lesson Content
        /// <summary>
        /// Add a new title at the end of StackPanel of Lesson Content (stackPanelContent)
        /// </summary>
        private void AddTitle()
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Style = (Style)TryFindResource("StackPanelTitleStyle");

            // add titleTextBox
            TextBox textBox = new TextBox();
            textBox.Name = "textBoxT" + (titles.Count + 1).ToString();
            textBox.Style = (Style)TryFindResource("TextBoxTitleStyle");
            stackPanel.Children.Add(textBox);
            titles.Add(textBox);            
            // add number to the text of the title textBox            
                textBox.Text = GetArabicNumber(titles.Count) + ".  ";            

            // add event handler for mouse up (click) event
            // which is needed to handle the scenario of loading shapes
            // on the map for each title
            textBox.GotFocus += new RoutedEventHandler(textBoxTitle_GotFocus);

            // add dateTextBox
            textBox = new TextBox();
            textBox.Name = "textBoxD" + (dates.Count + 1).ToString();
            textBox.Style = (Style)TryFindResource("TextBoxDateStyle");
            stackPanel.Children.Add(textBox);
            dates.Add(textBox);

            // add titleStackPanel to ContentStackPanel
            stackPanelContent.Children.Add(stackPanel);
           
            // add titlePosition
            titlePositions.Add(zeroPosition);
        }

        /// <summary>
        /// // Add a paragraph after the currently-selected title
        /// </summary>
        private void AddParagraph()
        {
            Grid_ grid = new Grid_();
            grid.Style = (Style)TryFindResource("GridParagraphStyle");

            Rectangle rectangle = new Rectangle();
            rectangle.Style = (Style)TryFindResource("RectangleParagraphStyle");

            TextBox textBox = new TextBox();
            textBox.Name = "TextBoxP" + (paragraphs.Count + 1).ToString();
            textBox.Style = (Style)TryFindResource("TextBoxParagraphStyle");

            grid.Children.Add(rectangle);
            grid.Children.Add(textBox);
            stackPanelContent.Children.Add(grid);
            paragraphs.Add(textBox);

            // finally, scroll smoothy buttom of the scrollViewerContent
            SmoothScroll();
        }

        /// <summary>
        /// Scroll the scrollViewer smoothly (with animation) to the buttom
        /// </summary>
        private void SmoothScroll()
        {
            mediator.ScrollViewer = scrollViewerContent;
            canvasMain.RegisterName("mediator", mediator);

            DoubleAnimation animation = new DoubleAnimation();
            EasingFunctionBase easing = new BackEase();
            // EasingFunctionBase
            easing.EasingMode = EasingMode.EaseOut;
            
            animation.From = scrollViewerContent.VerticalOffset;
            animation.To = scrollViewerContent.ScrollableHeight + 110;
            animation.By = 1;
            animation.EasingFunction = easing;
            //animation.AccelerationRatio = 0;
            //animation.DecelerationRatio = 1;
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(600));

            Storyboard storyBoard = new Storyboard();
            Storyboard.SetTargetName(animation, "mediator");
            Storyboard.SetTargetProperty(animation, new PropertyPath(ScrollViewerOffsetMediator.VerticalOffsetProperty));
            storyBoard.Children.Add(animation);
            storyBoard.Begin(this);
        }
        #endregion

        #region Click event handlers for buttons: open, new, save, add title, add paragraph
        // open Access LessonFile (exist lesson)
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            // in order to fill the GUI with the lesson content, there are 3 steps
            // 1. Open the access file and load its data to dataSet
            // 2. Move data from dataSet to Elements in Lists (as paragraph list, title list, date list )
            // 3. Load the GUI with the elements from the lists
            if (OpenFile())
            {
                FillElementLists();
                LoadGUI();
            }
        }

        // save lesson to new Access file
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            // check first if user wrote a basic Lesson or not, the user at least should write
            // 1. Lesson Name, first Title, Date and Paragraph
            StackPanel child = (StackPanel)stackPanelContent.Children[1];
            string title1 = ((TextBox)child.Children[0]).Text;
            string date1 = ((TextBox)child.Children[1]).Text;
            string paragraph1 = ((TextBox)(((Grid_)(stackPanelContent.Children[2])).Children[1])).Text;

            if (string.IsNullOrEmpty(textBoxLesson.Text)
                || string.IsNullOrEmpty(title1)
                || string.IsNullOrEmpty(date1)
                || string.IsNullOrEmpty(paragraph1))
            {
                if (Properties.Settings.Default.LessonToolIsEnglish)
                    MessageBox.Show("This Lesson is unfinished, please write all required info:\nLesson Name, Title, Date and Paragraph", "Notify", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                else
                    MessageBox.Show("مـحـتـوى الـدرس غـيـر كـافٍ ، مـن فـضـلـك أكـمـل:\nاسـم الـدرس ، أول عـنـوان  وتـاريـخــه  ومـحـتـواه.", "   تـنـبـيــه", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                return;
            }

            // in order to store lesson content from the GUI to the database file
            // we do the following steps
            // 1. loop on the GUI elements carrying the Data/Content of the Lesson
            // 2. while looping, save information directly to the access file
            //    the access file will be named after the lesson name

            // Populate LessonData from GUI and save it directly to a new Access File            
            if (!SaveFile())
                SystemSounds.Exclamation.Play();
        }

        // new lesson, empty stackPanelContent nad clear the map
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            NewFile();
        }
        
        // Add a new title at the end of StackPanel of Lesson Content (stackPanelContent)
        // also add a paragraph after this title
        private void button4_Click(object sender, RoutedEventArgs e)
        {
            AddTitle();
            AddParagraph();
            // call textBoxTitle click event in order to do the following
            // set focus to the newly creared textbox
            // remoce focus from brevious one
            // remove map shapes that were assigned to the old title
            // set currentTitle = ID of new title
            TextBox textBox = titles[titles.Count - 1];
            textBoxTitleFocus(textBox);
            // set selectionPeam at the end of the text
            textBox.SelectionStart = textBox.Text.Length;
        }
       
        // Add a paragraph after the currently-selected title
        private void button5_Click(object sender, RoutedEventArgs e)
        {
            AddParagraph();
        }
        #endregion
                
        #region Click event Handlers for the map and toggleButtons under it
        
        // flag to determine the begining drawing a new shape(polygon/polyline)
        private bool isPolygonDrawingStarted = false;        
        private List<LatLonAlt> polygonPoints;
        // determine which shape to plot 
        private enum Shape_ { Polygon, Polyline};
        private Shape_ polyType;

        // add pushpin, polygon or polyline
        private void toggleButtonMap_Click(object sender, RoutedEventArgs e)
        {
            string name = ((System.Windows.Controls.Primitives.ToggleButton)sender).Name;            
            // manally unceck other buttons
            ToggleButtons(name);
            
            // draw polygon/ polyline
            if ((toggleButton2.IsChecked == false || toggleButton3.IsChecked == false) 
                && isPolygonDrawingStarted == true)
            {
                // Draw the shape, clear polygonPoints List, remove temporary layer
                // from the map and finally open the flag of finishing drawing the shape                
                isPolygonDrawingStarted = false;
                // check if polygonPoints < 3, not sufficient for plotina a polygon
                if (polygonPoints.Count > 2)
                {
                    // checked for the shape type to draw/plot the correct one
                    if (polyType == Shape_.Polygon)
                        AddPolygon(polygonPoints);
                    else if (polyType == Shape_.Polyline)
                        AddPolyline(polygonPoints);
                }

                polygonPoints.Clear();
                map.Host.Geometry.RemoveLayer(temporaryLayer);
            }

            // change cursor type
            if (toggleButton1.IsChecked == true)
                map.Cursor = cursorPin(Cursors_.Pin);
            else if (toggleButton2.IsChecked == true)
            {
                polyType = Shape_.Polygon;
                map.Cursor = cursorPin(Cursors_.Polygon);
            }
            else if (toggleButton3.IsChecked == true)
            {
                polyType = Shape_.Polyline;
                map.Cursor = cursorPin(Cursors_.Polygon);
            }
            else if (toggleButton4.IsChecked == true)
                map.Cursor = cursorPin(Cursors_.Position);
            else
                map.Cursor = System.Windows.Forms.Cursors.Arrow;
        }

        // when a toggle button is checked, we want to uncheck the others
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

        // handle map click 
        void map_Click(object sender, EventArgs e)
        {            
            // open the popup            
            if (toggleButton1.IsChecked == true)
            {
                popup.IsOpen = true;                
            }
            // add points to draw polygon, polyline
            else if (toggleButton2.IsChecked == true || toggleButton3.IsChecked  ==true)
            {
                if (!isPolygonDrawingStarted)
                {
                    isPolygonDrawingStarted = true;                    
                    // initialize the list that will carry the 
                    polygonPoints = new List<LatLonAlt>();
                    // add a temporary layer to thr map
                    map.Host.Geometry.AddLayer(temporaryLayer);
                    // add first point of the polygon
                    AddPolygonPoint();
                }
                else
                {
                    AddPolygonPoint();
                }
            }            
            // add title-position label (see above documantation for more info about title-position)
            else if(toggleButton4.IsChecked == true)
            {
                AddPositionLabel();
            }
        }                        
        #endregion

        #region Handles the click event of the title
        // When a title is clicked the map layers corresponding to this title
        // is created and loaded on the map while the other layers disappear
        // also the bakcground of the selected title StackPanel is changed to focused background
        int currentTitle = 1;
        private void textBoxTitle_GotFocus(object sender, RoutedEventArgs e)
        {            
            // get title ID from the name of the sender titleTextBox: textBoxT[ID]
            // the following check is so so so important because when thsi application
            // starts for the first time, we call TitleFocus(1) which calls KeyBoardFocus()
            // which calls textBoxTitleGotFocus() which do manipulations on the map, 
            // and up till this sentense of code (at application first start up), the map
            // is not ready to be manipualted (this bug cost me hours !)
            if (map.Host.Ready)
                textBoxTitleFocus((TextBox)sender);
        }
        
        /// <summary>
        /// change bakgrounds of focused and defocused titles, 
        /// clear map and load corresponding pins form dataSet
        /// </summary>        
        private void textBoxTitleFocus(TextBox sender)
        {
            //string name = sender.Name;
            //int titleID = Convert.ToInt32(name.Substring(8, name.Length - 8));
            int titleID = titles.IndexOf(sender) + 1;

            SystemSounds.Asterisk.Play();
            // focus currently selected textBox and defocus the brevious one
            TitleDefocus(currentTitle);
            TitleFocus(titleID);

            // set currently selected textBox to the title ID of the sender
            currentTitle = titleID;

            // clear pin layer form its assigned shapes and set pincounter
            map.Host.Geometry.ClearLayer(pinLayer);
            pinCounter = 0;
            // then load pins to the map
            // related to this selected title, load them from dataSet using Linq
            var rows = from DataRow row in dataSet.Tables["Pin"].Rows
                       where (int)row["TitleID"] == currentTitle
                       select row;
            foreach (DataRow row in rows)
            {
                pinCounter++;
                map.Host.Geometry.AddGeometry(GetPin((double)row["Lat"], lon = (double)row["Lon"], GetPushPinStyle(GetArabicNumber(pinCounter)), row["PinID"].ToString()));                
            }
        }
        
        /// <summary>
        /// change background of stackPanel containing currently-selected 
        /// titleTextBox to the focus background
        /// </summary>        
        private void TitleFocus(int titleID)
        {            
            // set color focus style
            ((StackPanel)titles[titleID - 1].Parent).Background = (LinearGradientBrush)TryFindResource("TitleFocus");
            // set keyboard focus
            Keyboard.Focus(titles[titleID - 1]);
        }

        /// <summary>
        ///  Changes back the background of the previously focused title
        /// to its original (unfocused) background
        /// </summary>        
        private void TitleDefocus(int titleID)
        {
            ((StackPanel)titles[titleID - 1].Parent).Background = new SolidColorBrush(Color.FromArgb(255, 236, 246, 250));
        }
        #endregion                       

        /// <summary>
        /// Toggle UI language of this application (between English and Arabic).
        /// </summary>        
        private void button6_Click(object sender, RoutedEventArgs e)
        {
            // Save settings that flags the starting language and reatart the application.
            Properties.Settings.Default.LessonToolIsEnglish = !Properties.Settings.Default.LessonToolIsEnglish;
            Properties.Settings.Default.Save();
            this.Hide();
            this.ShowInTaskbar = false;            
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(((LabelGeometry)map.Host.Geometry.GetGeometry(labelLayer,"2")).Geometry.Altitude.ToString());
            //MessageBox.Show(map.Host.Navigation.CameraPosition.Altitude.ToString());
            //button6.Content = textBoxD1.SelectionStart.ToString();
        }
    }

// trash code
#if false
       
  int elements = 0, stackPanels = 0;
  // loop on the elements until we found the wanted stackPanels
  // and change their backgorund
  foreach (UIElement item in stackPanelContent.Children)
  {
   if (item is StackPanel)
    {
       stackPanels++;
       if (stackPanels == nextID)
          ((StackPanel)stackPanelContent.Children[elements]).Background = (LinearGradientBrush)TryFindResource("TitleFocus");
      else if (stackPanels == breviousID)
           ((StackPanel)stackPanelContent.Children[elements]).Background = new SolidColorBrush(Color.FromArgb(255, 236, 246, 250));
   }
   elements++;
}
((StackPanel)stackPanelContent.Children[1]).Background = (LinearGradientBrush)TryFindResource("TitleFocus");
       
#endif
#if false
    
        PushpinInfo pinStyle;
        private void DeSerializePinStyle()
        {
            //Stream stream = File.Open("PushPinStyle.osl", FileMode.Create);
            //BinaryFormatter bformatter = new BinaryFormatter();
            //bformatter.Serialize(stream, GetPushPinStyle(string.Empty));
            //stream.Close();
            Stream stream = File.Open("PushPinStyle.osl", FileMode.Open);
            BinaryFormatter bformatter = new BinaryFormatter();
            pinStyle = (PushpinInfo)bformatter.Deserialize(stream);
            stream.Close();
        }

#endif
}

