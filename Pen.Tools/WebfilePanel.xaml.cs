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
using System.Data;
using System.Data.OleDb;


namespace Pen.Tools
{
    /// <summary>
    /// Determine the file type of the application used.
    /// </summary>
    public enum FileType 
    { 
        /// <summary>
        /// Pen Language File (.plf)
        /// </summary>
        Language, 
        /// <summary>
        /// Pen Map File (.pmf)
        /// </summary>
        Map 
    }

    /// <summary>
    /// Interaction logic for WebfilePanel.xaml. This panel offers an interface to enable user
    /// to login to his account on the wepsite (pen-app.com), select lesson files and download it.
    /// All this while he is still openning his application.
    /// </summary>
    public partial class WebfilePanel : UserControl
    {
        // private file type
        private FileType fileType;

        // objects required to connect to database
        private DataTable dataTable;
        private OleDbConnection dbConnection;
        private OleDbDataAdapter dataAdapter;        

        /// <summary>
        /// The type of lesson files used in the application. Also Set the style of WebfilePanel 
        /// according ot the type of the program (i.e. according to file type).
        /// </summary>
        public FileType FileType_
        {
            get
            {
                return fileType;
            }

            set
            {
                fileType = value;
                if (fileType == FileType.Language)
                    borderPopup.Background = (LinearGradientBrush)TryFindResource("BorderGreenColor");
                else
                    borderPopup.Background = (LinearGradientBrush)TryFindResource("BorderBlueColor");
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public WebfilePanel()
        {   
            InitializeComponent();
            this.canvas1.Children.Clear();            
            StartScrollingEvents();
            InitializeDatabase();
        }
        
        /// <summary>
        /// Initialize objects required to connect to Lesson Files Database
        /// </summary>
        private void InitializeDatabase()
        {
            //        set conn = CreateObject("ADODB.Connection") 
            //conn.open "Provider=MS Remote;" &_  
            //    "Remote Server=http://<ip address or server name>;" &_ 
            //    "Remote Provider=Microsoft.Jet.OLEDB.4.0;" &_ 
            //    "Data Source=c:\inetpub\wwwroot\file1.mdb;"

            string connectionStr;
            connectionStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=E:\Program Files\VS Projects\WebSites\Pen-app\App_Data\PenAppDB.accdb;Persist Security Info=True";
            //connectionStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\"" + dbSource + "\";Persist Security Info=True";
            //connectionStr =  @"Provider=Microsoft.JET.OLEDB.4.0;" + @"data source=" + dbSource;
            //connectionStr = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source= http://www.pen-app.com//penlessonfilesDatab//PenAppDB.accdb";
            //connectionStr = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + @"www.pen-app.com/penlessonfilesDatab/PenAppDB.accdb";
            //connectionStr = "Provider=MS Remote;" + "Remote Server=http://localhost/;" +
            //                "Remote Provider=Microsoft.ACE.OLEDB.12.0;" +
            //                "Data Source= http://localhost/PenAppDB.accdb ;Persist Security Info=False";
            //connectionStr = "Provider=MS Remote; Remote Provider=Microsoft.ACE.OLEDB.12.0;Remote Server=http://localhost/" +
            //                ";Data Source=PenAppDB.accdb;";
            //connectionStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=http://localhost/PenAppDB.accdb; Persist Security Info=False";

            dbConnection = new OleDbConnection(connectionStr);
            dataTable = new DataTable();
        }

        #region Login button clicked + login to website + download Lesson File
        /// <summary>
        /// Mouseclick event handler for user login button.
        /// </summary>        
        private void buttonLogin_Click(object sender, RoutedEventArgs e)
        {
            // this is commenter for the demonstration purpose only
            //CheckLoginProcess();

            // login directly with the following username
            textBoxUser.Text = "nour";
            LoginToWebsite(); 
        }

        /// <summary>
        /// Touchdown event handler for user login button.
        /// </summary>        
        private void buttonLogin_TouchDown(object sender, TouchEventArgs e)
        {
            // this is commenter for the demonstration purpose only
            //CheckLoginProcess();

            // login directly with the following username
            textBoxUser.Text = "nour";
            LoginToWebsite();            
        }

        /// <summary>
        /// Check for valid username and password, then call loginToWebSite() method.
        /// </summary>
        private void CheckLoginProcess()
        {
            passwordBox.Background = null;
            textBoxUser.Background = null;
            textBlockError.Text = string.Empty;
            // 1. Check for empty usename or password
            if (string.IsNullOrWhiteSpace(passwordBox.Password) || string.IsNullOrWhiteSpace(textBoxUser.Text))
            {
                if (string.IsNullOrWhiteSpace(passwordBox.Password))
                {
                    passwordBox.Background = new SolidColorBrush(Color.FromArgb(30, 250, 90, 70));
                    textBlockError.Text = "Sorry, empty Password";
                }
                if (string.IsNullOrWhiteSpace(textBoxUser.Text))
                {
                    textBoxUser.Background = new SolidColorBrush(Color.FromArgb(30, 250, 90, 70));
                    textBlockError.Text = "Sorry, empty Username";
                }
                System.Media.SystemSounds.Asterisk.Play();
                return;
            }

            // 2. Check for not found usename and wrong password
            // connect to the DB and search for the username and password
            string commandString = "SELECT User.Password " +
                                   "FROM [User] " +
                                   "WHERE (User.Username)= '" + textBoxUser.Text + "'";
            dataAdapter = new OleDbDataAdapter(commandString, dbConnection);
            dataTable.Clear();
            dataTable.Columns.Clear();
            dbConnection.Open();
            dataAdapter.Fill(dataTable);
            dbConnection.Close();
            // check for found usrname
            if (dataTable.Rows.Count == 0)
            {
                textBlockError.Text = "Sorry, invalid Username!";
                return;
            }            
            // check for valid password         
            else if ((string)dataTable.Rows[0][0] != passwordBox.Password)
            {
                textBlockError.Text = "Sorry, invalid Password!";
                return;
            }

            //LoginToWebsite();
        }

        /// <summary>
        /// Login to the website, retreive files in the user accounts and show the user interface.
        /// </summary>
        private void LoginToWebsite()
        {            
            // connect to the DB and retreive the user's Lesson Files form DB            
            string commandString = "SELECT LessonFile.DateAdded, LessonFile.Title, LessonFile.Description, LessonFile.LessonFile " +
                                   "FROM [User] INNER JOIN (LessonFile INNER JOIN [User-LessonFile] ON LessonFile.LessonFileID = [User-LessonFile].LessonFileID) ON User.Username = [User-LessonFile].Username " +
                                   "WHERE (User.Username='" + textBoxUser.Text + "' AND LessonFile.Type='" + fileType.ToString()[0] + "')";
            dataAdapter = new OleDbDataAdapter(commandString, dbConnection);
            dataTable.Clear();
            dataTable.Columns.Clear();
            dbConnection.Open();
            dataAdapter.Fill(dataTable);
            dbConnection.Close();
            //MessageBox.Show(fileTable.Columns[0].Caption + ", " + fileTable.Columns[1].Caption + ", " + fileTable.Columns[2].Caption + ", " + fileTable.Columns[3].Caption);

            // check if there is lesson files found in the user account or not
            int numOfRetrievedFiles = dataTable.Rows.Count;
            if (numOfRetrievedFiles == 0)
            {
                textBlockError.Text = "Sorry, you don't have Pen " + fileType.ToString() + " files in your account.";
                return;
            }          

            // now, after retreiving lessonFiles form the user account on the website
            // ,add lessonfile items according to the number of Lesson Files retreived.
            stackPanel.Children.Clear();

            Canvas canvas;
            TextBlock textBlockDescription;
            Label labelTitle;
            Label labelDate;
            Image image;
            string name;

            // 1. Create Elements with their names and styles
            // 2. Add the information retreived form the website (title + description + date)
            // 3. Add elements to the canvas and add canvas as a new item to the stackPanel
            for (int i = 0; i < numOfRetrievedFiles; i++)
            {
                // 1.
                name = (i + 2).ToString();

                textBlockDescription = new TextBlock();
                textBlockDescription.Name = "textBlock" + name;
                textBlockDescription.Style = (Style)TryFindResource("TextBlockStyle");

                labelTitle = new Label();
                labelTitle.Name = "labelTitle" + name;
                labelTitle.Style = (Style)TryFindResource("LabelTitleStyle");

                labelDate = new Label();
                labelDate.Name = "labelDate" + name;
                labelDate.Style = (Style)TryFindResource("LabelDateStyle");

                image = new Image();
                image.Name = "image" + name;
                // support both mouse and touch events so the user can work using
                // the mouse or the pen
                image.MouseDown += new MouseButtonEventHandler(image_MouseDown);
                image.TouchDown += new EventHandler<TouchEventArgs>(image_TouchDown);               
                
                canvas = new Canvas();
                canvas.Name = "canvas" + name;
                canvas.Style = (Style)TryFindResource("CanvasStyle");
                if (fileType == FileType.Language)
                {
                    image.Style = (Style)TryFindResource("ImageGreenStyle");
                    if (i % 2 == 0) canvas.Background = (SolidColorBrush)TryFindResource("GreenColor");
                }
                else
                {
                    image.Style = (Style)TryFindResource("ImageBlueStyle");
                    if (i % 2 == 0) canvas.Background = (SolidColorBrush)TryFindResource("BlueColor");
                }
                
                // 2.
                //labelTitle.Content = "If Condition Lesson";
                //labelDate.Content = "15.3.2010";
                //textBlockDescription.Text = "This is to help teacher in teaching the lesson of If condition " +
                //                    "with it's different 3 types.";
                labelTitle.Content = (string)dataTable.Rows[i]["Title"];
                labelDate.Content = ((DateTime)dataTable.Rows[i]["DateAdded"]).ToShortDateString();
                textBlockDescription.Text = (string)dataTable.Rows[i]["Description"];

                // 3.
                canvas.Children.Add(image);
                canvas.Children.Add(labelTitle);
                canvas.Children.Add(labelDate);
                canvas.Children.Add(textBlockDescription);
                stackPanel.Children.Add(canvas);
            }
        }

        /// <summary>
        /// Mousedown event handler for image of the Lesson File.
        /// </summary>  
        private void image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int index = Convert.ToInt32(((Image)sender).Name.Substring(5, 1));
            DownloadFile(index);
        }

        /// <summary>
        /// Touchdown event handler for image of the Lesson File.
        /// </summary>        
        private void image_TouchDown(object sender, TouchEventArgs e)
        {
            int index = Convert.ToInt32(((Image)sender).Name.Substring(5, 1));
            DownloadFile(index);           
        }

        /// <summary>
        /// Download the file with thr sent index.
        /// </summary>
        /// <param name="fileIndex">Index starts from 1.</param>
        private void DownloadFile(int fileIndex)
        {
            System.Media.SystemSounds.Exclamation.Play();
        }
        #endregion

        #region Implement Scrolling of Lesson Content
        // Scrolling by two approaches 1.Smooth Touch Scrolling 2.Ordinary Mouse Scrolling       
        private double y1 = 0, y2 = 0;

        /// <summary>
        /// Initialize event handlers required to perform smooth scrolling.
        /// </summary>
        private void StartScrollingEvents()
        {
            scrollViewer.ManipulationStarting += new EventHandler<ManipulationStartingEventArgs>(scrollViewer_ManipulationStarting);
            scrollViewer.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(scrollViewer_ManipulationDelta);
            scrollViewer.ManipulationInertiaStarting += new EventHandler<ManipulationInertiaStartingEventArgs>(scrollViewer_ManipulationInertiaStarting);
            scrollViewer.IsManipulationEnabled = true;

            scrollViewer.MouseDown += new MouseButtonEventHandler(scrollViewerContent_MouseDown);
            scrollViewer.MouseMove += new MouseEventHandler(scrollViewerContent_MouseMove);
        }

        // this is ordinary scrolling (i.e using mouse events and no touch events) 
        // in case of there is no touch device connnected
        void scrollViewerContent_MouseDown(object sender, MouseButtonEventArgs e)
        {
            y1 = e.MouseDevice.GetPosition(scrollViewer).Y;
        }
        void scrollViewerContent_MouseMove(object sender, MouseEventArgs e)
        {
            y2 = e.MouseDevice.GetPosition(scrollViewer).Y;
            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - (y2 - y1));
            y1 = y2;
        }

        /// <summary>
        /// Start of manipulation (equivilant to touchDown + first touchMove events).
        /// </summary>        
        void scrollViewer_ManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            e.ManipulationContainer = canvasMain;
            e.Handled = true;
            e.Mode = ManipulationModes.TranslateY;
        }

        /// <summary>
        ///  When there is movement updates (equivilant to touchMove).
        /// </summary>        
        void scrollViewer_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.DeltaManipulation.Translation.Y);
        }

        /// <summary>
        /// When the touch is released from the control (equivilant to TouchUp or TouchLeave).
        /// </summary>        
        void scrollViewer_ManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            // adjust the dispalcement behaviour
            // (10 inches * 96 DIPS per inch / 1000ms^2)
            e.TranslationBehavior.DesiredDeceleration = 2.0 * 96.0 / (1000.0 * 1000.0);
            e.TranslationBehavior.InitialVelocity = e.InitialVelocities.LinearVelocity;
            e.TranslationBehavior.DesiredDisplacement = Math.Abs(e.InitialVelocities.LinearVelocity.Y) * 300;
        }
        #endregion               
    }
}