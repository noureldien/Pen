using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Media;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Ink;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using Telerik.Windows.Controls;
using Word = Microsoft.Office.Interop.Word;
using System.Windows.Controls.Primitives;

namespace Pen.Math
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // constructor
        public MainWindow()
        {
            InitializeComponent();
            // run the form in the full scree mode
            FullScreen();
        }

        // run math panel, control themes, inkAnalyzer, ...etc after form load
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // start time
            StartTime();

            // create new math input panel
            mathPanel = new MathPanel();
            // add input event handler for math input panel
            mathPanel.mathPanel.Insert += new micautLib._IMathInputControlEvents_InsertEventHandler(mathPanel_Insert);
            // run ink analyzer
            InkAnalyzerInitialize();

            // intial drawing settnigs of inkCanvases
            LoadDefaultDrawingSettings();

            radColorPicker1.SelectedColor = radColorPicker2.SelectedColor = Color.FromArgb(255, 255, 100, 0);
            radColorPicker3.SelectedColor = Colors.Black;

            // set pen mode as the initial mode for teacher and studings
            buttonPen1.IsChecked = true;
            buttonPen2.IsChecked = true;
            buttonPen3.IsChecked = true;
            buttonExpander.IsChecked = true;
            buttonAlgebraMode.IsChecked = true;

            // set some settings for smart action inkCanvases (scrollBar1,2 and inkCanvasCommand)
            DrawingAttributes settings = new DrawingAttributes();
            settings.IgnorePressure = true;
            settings.Height = settings.Width = 2;
            scrollBar1.DefaultDrawingAttributes = scrollBar2.DefaultDrawingAttributes
                = inkCanvasCommand.DefaultDrawingAttributes = settings;

        }

        #region Main Window Event Handlers
        // shut down on Esc Key Press
        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Application.Current.Shutdown();
        }

        // dispose all objects that obstacles window closing as inkAnalyzer
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // close math input panel
            mathPanel.Dispose();

            if (inkAnalyzer != null)
                inkAnalyzer.Dispose();

            // close office Word Application
            try
            {
                wordDoc.Close(ref missing, ref missing, ref missing);
                wordApp.Quit(ref missing, ref missing, ref missing);
            }
            catch { }
            wordDoc = null;
            wordApp = null;
            GC.Collect();
        }

        /// <summary>
        /// close current form
        /// </summary>        
        private void buttonClose_TouchDown(object sender, TouchEventArgs e)
        {
            //this.gridMain.Opacity = 0.3;
            //Application.Current.Windows[0].Opacity = 0.55;
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
            textCaption.Text = "Pen Math |  " + time.Remove(time.Length - 3);

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(1);
            timer.Tick += new EventHandler((object sender, EventArgs e) =>
            {
                time = DateTime.Now.ToShortTimeString();
                textCaption.Text = "Pen Math |  " + time.Remove(time.Length - 3);

            });
            timer.Start();
        }
        #endregion

        #region hand writing recognition by inkAnalyser, WPF
        InkAnalyzer inkAnalyzer = new InkAnalyzer();

        private void InkAnalyzerInitialize()
        {
            inkAnalyzer.ResultsUpdated += new ResultsUpdatedEventHandler(inkAnalyzer_ResultsUpdated);
        }

        private void inkCanvasCommand_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
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
            string recognizedText = inkAnalyzer.GetRecognizedString();
            inkAnalyzer.RemoveStrokes(inkCanvasCommand.Strokes);
            inkCanvasCommand.Strokes.Clear();

            switch (recognizedText)
            {
                // select button
                case "S":
                case "s":
                    //selectButton.RaiseEvent(eventArgs);
                    selectButton_TouchUp(buttonSelect, null);
                    break;
                // eraser button
                case "e":
                    eraserButton_TouchUp(buttonEraser3, null);
                    break;
                // pen button
                case "P":
                case "p":
                    penButton_TouchUp(buttonPen3, null);
                    break;
                // open facebook panel
                case "f":
                case "F":
                    // connect to facebook
                    break;
                default:
                    break;
            }
            textBlock1.Text = recognizedText;
        }
        #endregion

        #region math input panel and office manipualtion
        private MathPanel mathPanel;          // class to control math input panel
        private string mathPanelResult;       // result of math panel recognition
        private bool isWordDocOpened = false; // determines creating new Word Doc or open the recently created one
        private Word.Application wordApp;
        private Word.Document wordDoc;
        private Word.Paragraph wordParagraph;
        // object of missing (null value)
        Object missing = System.Reflection.Missing.Value;

        // event handler for math input panel, which brings the recognition result
        // as XML (Microsoft Math Language)
        // after that, we use this recognition result and sed it to word, ..... ,pla pla pla
        void mathPanel_Insert(string RecoResult)
        {
            mathPanelResult = RecoResult;
        }

        // this creates Word app, gets recognized math equation from math input panel
        //and do other Word app stuff
        private void RunWordApp()
        {
            if (!isWordDocOpened)
            {
                isWordDocOpened = true;
                // create new Word App. and Doc. objects
                wordApp = new Microsoft.Office.Interop.Word.Application();
                wordDoc = new Microsoft.Office.Interop.Word.Document();
                wordDoc = wordApp.Documents.Add(ref missing, ref missing, ref missing, ref missing);
                // make the app. visible
                wordApp.Caption = "Solve New Equation";
                wordApp.Visible = true;
            }

            if (!string.IsNullOrEmpty(mathPanelResult))
            {
                Clipboard.SetText(mathPanelResult);
                wordDoc.Content.Delete(ref missing, ref missing);
                wordDoc.Content.Paste();
                wordApp.Activate();
            }

            #region add new paragraph
            // add new paragraph after at the end of the document
            //wordParagraph = wordDoc.Content.Paragraphs.Add(wordDoc.Bookmarks["endofdoc"].Range);
            // else add new paragraph
            //wordParagraph = wordDoc.Content.Paragraphs.Add(ref missing);
            //wordParagraph.Range.Text = Clipboard.GetData(System.Windows.DataFormats.;
            //wordParagraph.Range.InsertXML(mathPanelResult, ref missing);            
            //wordParagraph.Format.SpaceAfter = 1;
            //wordParagraph.Range.InsertParagraphAfter();
            #endregion

        }
        #endregion

        #region manipulating and processing inkCanvases and its Editing Modes (Draw and Erase)
        // drawing, editing, color changing, erasing
        private StylusPointCollection pointCollection;
        // save current storke for every user (every inkCanvas)
        private Dictionary<int, Stroke> strokeDictionary = new Dictionary<int, Stroke>();
        // save color settings for every user
        private Color[] colorSettings = new Color[3];
        // save ben thickness settings
        private double[] strokeThickness = new double[3];

        /// <summary>
        /// called only one time to set setting of writing on canvases
        /// </summary>
        private void LoadDefaultDrawingSettings()
        {
            // adjust settings of  students inkCanvases
            for (int i = 0; i < 2; i++)
            {
                strokeThickness[i] = 3;
                colorSettings[i] = Color.FromArgb(255, 255, 100, 0);
            }
            // adjust settings for Teacher inkCanvas
            strokeThickness[2] = 2;
            colorSettings[2] = Color.FromArgb(255, 0, 160, 230);

            // adjust settings for those smartInKCanvases lies under inkCanvas1,2
            //DrawingAttributes settings = new DrawingAttributes();
            //settings.Height = settings.Width = 2;
            //settings.IsHighlighter = false;
            //settings.Color = Colors.Black;
            //scrollBar1.DefaultDrawingAttributes
            //    = scrollBar2.DefaultDrawingAttributes = settings;            
        }

        /// <summary>
        /// Set the attributes of the new stroke to begin drawing it 
        /// </summary>        
        private void inkCanvas_TouchDown(object sender, TouchEventArgs e)
        {
            InkCanvas inkCanvas = sender as InkCanvas;

            // get current touch point
            Point p = e.GetTouchPoint(inkCanvas).Position;
            pointCollection = new StylusPointCollection();
            pointCollection.Add(new StylusPoint(p.X, p.Y));

            // check if Editing mode == Select, return back, this is to prevent
            // Teacher inkStroke from drawing in case of the Editing Mode = Select
            // also check for modes of drawing shapes
            if (inkCanvas.EditingMode != InkCanvasEditingMode.None && shape == Shape_.None)
                return;
            else if (shape == Shape_.Line)
            {
                pointCollection.Add(new StylusPoint(p.X, p.Y));
                pointCollection.Add(new StylusPoint(p.X, p.Y));
                pointCollection.Add(new StylusPoint(p.X, p.Y));
            }
            else if (shape == Shape_.Triangle)
            {
                pointCollection.Add(new StylusPoint(p.X, p.Y));
                pointCollection.Add(new StylusPoint(p.X, p.Y));
                pointCollection.Add(new StylusPoint(p.X, p.Y));
            }
            else if (shape == Shape_.Rectangle)
            {
                pointCollection.Add(new StylusPoint(p.X, p.Y));
                pointCollection.Add(new StylusPoint(p.X, p.Y));
                pointCollection.Add(new StylusPoint(p.X, p.Y));
                pointCollection.Add(new StylusPoint(p.X, p.Y));
            }

            // create storke
            System.Windows.Ink.Stroke newStroke = new Stroke(pointCollection);
            int index = Convert.ToInt32(inkCanvas.Name.Substring(9, 1)) - 1;

            // change stoke settings (color and thickness)
            newStroke.DrawingAttributes.Width = newStroke.DrawingAttributes.Height = strokeThickness[index];
            newStroke.DrawingAttributes.Color = colorSettings[index];

            // add stoke to stroke dictinary
            strokeDictionary.Add(e.TouchDevice.Id, newStroke);
            // add stroke to canvas
            inkCanvas.Strokes.Add(newStroke);
            // call inkMove to add the first Point
            inkCanvas_TouchMove(sender, e);
        }

        /// <summary>
        /// Update the stroke that is being drawn currently
        /// </summary>        
        private void inkCanvas_TouchMove(object sender, TouchEventArgs e)
        {
            Stroke currentStorke;
            Point p = e.GetTouchPoint((InkCanvas)sender).Position;
            // add new point to the current stroke of the sender inkCanvas
            // we use if here in order to taccle this sequence: Down --> Move --> Leave --> Move
            if (strokeDictionary.TryGetValue(e.TouchDevice.Id, out currentStorke))
                if (shape == Shape_.None)
                    currentStorke.StylusPoints.Add(new StylusPoint(p.X, p.Y));
                else
                {
                    // rectangle, triangle and line can be drawn using 
                    // two points: initial point and new point

                    // initial point
                    StylusPoint po = currentStorke.StylusPoints[0];

                    switch (shape)
                    {
                        case Shape_.Line:
                            currentStorke.StylusPoints[1] = new StylusPoint(p.X, p.Y);
                            break;

                        case Shape_.Rectangle:
                            currentStorke.StylusPoints[1] = new StylusPoint(p.X, po.Y);
                            currentStorke.StylusPoints[2] = new StylusPoint(p.X, p.Y);
                            currentStorke.StylusPoints[3] = new StylusPoint(po.X, p.Y);
                            currentStorke.StylusPoints[4] = po;
                            break;

                        case Shape_.Triangle:
                            currentStorke.StylusPoints[1] = new StylusPoint(p.X, po.Y);
                            currentStorke.StylusPoints[2] = new StylusPoint((p.X / 2) + (po.X / 2), p.Y);
                            currentStorke.StylusPoints[3] = po;
                            break;

                        case Shape_.Cicle:
                            break;
                    }
                }
        }

        /// <summary>
        /// end drawing the stroke
        /// </summary>        
        private void inkCanvas_TouchUp_TouchLeave(object sender, TouchEventArgs e)
        {
            // remove stroke from Dictionary
            //Stroke stroke;
            //strokeDictionary.TryGetValue(e.TouchDevice.Id, out stroke);
            strokeDictionary.Remove(e.TouchDevice.Id);

            // relase capture form sender inkCanvas
            ((UIElement)sender).ReleaseTouchCapture(e.TouchDevice);

            // remove shape mode if exist
            shape = Shape_.None;
        }

        /// <summary>
        /// change Editing Mode to pen
        /// </summary>        
        private void penButton_TouchUp(object sender, TouchEventArgs e)
        {
            switch (((ToggleButton)sender).Name)
            {
                case "buttonPen1":
                    inkCanvas1.EditingMode = InkCanvasEditingMode.None;
                    buttonEraser1.IsChecked = false;
                    break;
                case "buttonPen2":
                    inkCanvas2.EditingMode = InkCanvasEditingMode.None;
                    buttonEraser2.IsChecked = false;
                    break;
                case "buttonPen3":
                    // resubscribe (subscribe again) to touch events
                    // that's because the confliction between 
                    // Selection Editing Mode and Manual Drawing Mode
                    inkCanvas3.EditingMode = InkCanvasEditingMode.None;
                    SubscribeToDrawingModeEvents(ref inkCanvas3);
                    buttonEraser3.IsChecked = false;
                    buttonSelect.IsChecked = false;
                    break;
                default:
                    MessageBox.Show("Error in switch case in penButton_MouseMove Method");
                    break;
            }
        }

        /// <summary>
        /// change Editing Mode to eraser
        /// </summary>        
        private void eraserButton_TouchUp(object sender, TouchEventArgs e)
        {
            switch (((ToggleButton)sender).Name)
            {
                case "buttonEraser1":
                    inkCanvas1.EditingMode = InkCanvasEditingMode.EraseByStroke;
                    buttonPen1.IsChecked = false;
                    break;
                case "buttonEraser2":
                    inkCanvas2.EditingMode = InkCanvasEditingMode.EraseByStroke;
                    buttonPen2.IsChecked = false;
                    break;
                case "buttonEraser3":
                    inkCanvas3.EditingMode = InkCanvasEditingMode.EraseByStroke;
                    buttonPen3.IsChecked = false;
                    buttonSelect.IsChecked = false;
                    break;
                default:
                    MessageBox.Show("Error in switch case in eraserButton_MouseDown Method");
                    break;
            }
        }

        /// <summary>
        /// change inkCanvas stroke color
        /// </summary>        
        private void radColorPicker_SelectedColorChanged(object sender, EventArgs e)
        {
            switch (((RadColorPicker)sender).Name)
            {
                case "radColorPicker1":
                    colorSettings[0] = radColorPicker1.SelectedColor;
                    break;
                case "radColorPicker2":
                    colorSettings[1] = radColorPicker2.SelectedColor;
                    break;
                case "radColorPicker3":
                    colorSettings[2] = radColorPicker3.SelectedColor;
                    break;
                default:
                    MessageBox.Show("Error in switch case in radColorPicker_SelectedColorChanged Method");
                    break;
            }
        }

        /// <summary>
        /// change inkCanvas3(teacher inkCanvas) Editing Mode to Select
        /// </summary>        
        private void selectButton_TouchUp(object sender, TouchEventArgs e)
        {
            // unsubscribe from touch events
            // that's because the confliction between 
            // Selection Editing Mode and Manual Drawing Mode
            SubscribeToSelectionModeEvents();
            inkCanvas3.EditingMode = InkCanvasEditingMode.Select;
            buttonPen3.IsChecked = false;
            buttonEraser3.IsChecked = false;
        }
        #endregion

        #region managing inkCanvas Scrolling
        // determine several actions according to send gesture types, like
        // 1. flickr inCanvas Pages   2. Change color, Editing mode 
        private void InkCanvas_Gesture(object sender, InkCanvasGestureEventArgs e)
        {
            // find the sender of this event
            string senderName = ((InkCanvas)sender).Name;
            ApplicationGesture gesture = e.GetGestureRecognitionResults()[0].ApplicationGesture;

            // check if gesture is backword or forward or not
            if (gesture == ApplicationGesture.Right || gesture == ApplicationGesture.Left)
                FlickrInkCanvasPages(senderName, gesture, e.Strokes[0].StylusPoints.Count);
            //else if              
        }

        // this is responsilbe of flickering inkCanvas Pages with animation
        private void FlickrInkCanvasPages(string senderName, ApplicationGesture gesture, int strokesCount)
        {
            // calculate offset
            double currentOffset = 0;
            double targetOffset = 0;

            // set current offset according to scrollViewer Name
            if (senderName == "scrollBar1")
            {
                Mediator.ScrollViewer = scrollViewer1;
                currentOffset = scrollViewer1.HorizontalOffset;
            }
            else if (senderName == "scrollBar2")
            {
                Mediator.ScrollViewer = scrollViewer2;
                currentOffset = scrollViewer2.HorizontalOffset;
            }

            // check if the scrolling direction is right or wrong
            if (gesture == ApplicationGesture.Left && currentOffset == 0
             || gesture == ApplicationGesture.Right && currentOffset == 1320)
                return;

            // determine length of the gesture stroke to determine how many pages to scroll

            int scrollingOffset = 440;
            if (strokesCount <= 25) ;
            else if (strokesCount <= 32)
                scrollingOffset *= 2;
            else
                scrollingOffset *= 3;

            // determine scrolling direction
            if (gesture == ApplicationGesture.Right)
                targetOffset = currentOffset + scrollingOffset;
            else if (gesture == ApplicationGesture.Left)
                targetOffset = currentOffset - scrollingOffset;

            // check if boundaries exceed due to adding several pages
            targetOffset = (targetOffset > 440 * 3) ? (440 * 3) : targetOffset;
            targetOffset = (targetOffset < 0) ? (0) : targetOffset;

            canvasMain.RegisterName("Mediator", Mediator);

            //DoubleAnimation animation =(DoubleAnimation) TryFindResource("scrollAnimation");
            DoubleAnimation animation = new DoubleAnimation();
            ExponentialEase easing = new ExponentialEase();
            easing.EasingMode = EasingMode.EaseInOut;

            animation.From = currentOffset;
            animation.To = targetOffset;
            animation.By = 1;
            animation.DecelerationRatio = 0.5;
            animation.AccelerationRatio = 0.5;
            //animation.EasingFunction = easing;
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(650));

            Storyboard storyBoard = new Storyboard();
            Storyboard.SetTargetName(animation, "Mediator");
            Storyboard.SetTargetProperty(animation, new PropertyPath(ScrollViewerOffsetMediator.HorizontalOffsetProperty));
            storyBoard.Children.Add(animation);
            storyBoard.Begin(canvasMain);
            //storyBoard.Begin(this);
        }
        #endregion

        #region Manageing Stroke Selection, strokeMove animation and strokeDelete animation
        // This scenario is known as (Teacher-assigns-taskes-to-Students)

        // adds event handlers to inkCanvas that: enables drawing mode and stops selection mode
        private void SubscribeToDrawingModeEvents(ref InkCanvas_ inkCanvas)
        {
            inkCanvasTemp.TouchDown -= new EventHandler<TouchEventArgs>(inkCanvasTemp_TouchDown);
            inkCanvasTemp.TouchMove -= new EventHandler<TouchEventArgs>(inkCanvasTemp_TouchMove);
            inkCanvasTemp.TouchUp -= new EventHandler<TouchEventArgs>(inkCanvasTemp_TouchUp);

            // subscribe to drwaing mode event handlers
            // it is found that you MUST NOT subscribe and unsubscribe from
            // the TouchDown event handler because this crashes the working of stroke dictionary
            // when a user(1) is drawing, and user(2) stops drawing then erase then restart drawing
            // if Touch subscribition and unsubscribition was working, then they dectionary
            // is going to record a new key for user(2) with touchDevice.ID = to ID user(1)
            // because subscribtion and unsubscription makes TouchDown forget the old ID of user(2)
            // and deal with user(2) as like as user(1) with both of them has the same TouchDevice.ID
            //inkCanvas.TouchDown += new EventHandler<TouchEventArgs>(inkCanvas_TouchDown);
            inkCanvas.TouchMove += new EventHandler<TouchEventArgs>(inkCanvas_TouchMove);
            inkCanvas.TouchUp += new EventHandler<TouchEventArgs>(inkCanvas_TouchUp_TouchLeave);
            inkCanvas.TouchLeave += new EventHandler<TouchEventArgs>(inkCanvas_TouchUp_TouchLeave);
        }

        /// <summary>
        ///  adds event handlers to inkCanvas that: enables selection mode and stops drawing mode
        /// </summary>
        private void SubscribeToSelectionModeEvents()
        {
            // unsubscribe from drawing mode event handlers
            //inkCanvas3.TouchDown -= new EventHandler<TouchEventArgs>(inkCanvas_TouchDown);
            inkCanvas3.TouchMove -= new EventHandler<TouchEventArgs>(inkCanvas_TouchMove);
            inkCanvas3.TouchUp -= new EventHandler<TouchEventArgs>(inkCanvas_TouchUp_TouchLeave);
            inkCanvas3.TouchLeave -= new EventHandler<TouchEventArgs>(inkCanvas_TouchUp_TouchLeave);

            inkCanvasTemp.TouchDown += new EventHandler<TouchEventArgs>(inkCanvasTemp_TouchDown);
            inkCanvasTemp.TouchMove += new EventHandler<TouchEventArgs>(inkCanvasTemp_TouchMove);
            inkCanvasTemp.TouchUp += new EventHandler<TouchEventArgs>(inkCanvasTemp_TouchUp);
        }

        /// <summary>
        /// stroke selection ended, the user should start after it selectionMoving 
        /// </summary>        
        private void inkCanvasSource_SelectionChanged(object sender, EventArgs e)
        {
            // > Every SelectionMoving is considered a standAlone moving session 
            //    starts with calling selectionMoving() method        
            //    then set selectionMovingBegin = false
            //    finally ends with selectionMoved method
            // > A successfull selectionMoving Session [when the moving selection reaches destination]
            //    results in a drag-and-drop action for the selected strokes
            //    from source (inkCanvas3, which is teacher inkCanvas) 
            //    to destination (incanvas1 or 2, ehich arer students')

            if (inkCanvas3.GetSelectedStrokes().Count > 0)
            {
                Rect rect = inkCanvas3.GetSelectionBounds();
                Point point = inkCanvas3.TranslatePoint(new Point()
                {
                    X = rect.X,
                    Y = rect.Y
                }, canvasMain);

                // set position
                Canvas.SetLeft(inkCanvasTemp, point.X);
                Canvas.SetTop(inkCanvasTemp, point.Y);
                // set dimensions (height, width)
                inkCanvasTemp.Height = rect.Height;
                inkCanvasTemp.Width = rect.Width;
                // copy                
                //inkCanvas3.CopySelection();
                //inkCanvas3.Strokes.Remove(inkCanvas3.GetSelectedStrokes());
                inkCanvas3.CutSelection();
                inkCanvasTemp.Paste();
            }
        }

        private Point local;         // initial position of stroke to be animated
        private int animateToValue;  // xAxis value that stroke will move to 

        void inkCanvasTemp_TouchDown(object sender, TouchEventArgs e)
        {
            local = e.TouchDevice.GetTouchPoint(inkCanvasTemp).Position;
            inkCanvas3.EditingMode = InkCanvasEditingMode.None;
        }

        void inkCanvasTemp_TouchMove(object sender, TouchEventArgs e)
        {
            Point absolute = e.TouchDevice.GetTouchPoint(canvasMain).Position;
            Canvas.SetTop(inkCanvasTemp, absolute.Y - local.Y);
            Canvas.SetLeft(inkCanvasTemp, absolute.X - local.X);
        }

        void inkCanvasTemp_TouchUp(object sender, TouchEventArgs e)
        {
            // selectionMoving session ended, we will check boundaries to take one of those actions
            // 1. return stroke back to inkCanvas3 and change editing mode of inkCanvas3 to Select       
            // 2. do animation to move it to destination inkCanvas and change editing mode of inkCanvas3 to None

            double leftBoundary = Canvas.GetLeft(inkCanvasTemp);
            double rightBoundary = Canvas.GetLeft(inkCanvasTemp) + inkCanvasTemp.Width;
            // check if it is a successfull selectionMoving Session
            // i.e selectedStrokes are on the boundaries of the destination inkCanvas
            // if so, begin copying selectedStrokes to the destination
            if (leftBoundary < 400)
                AnimateStrokeMove(40);
            else if (rightBoundary > (1280 - 400))
                AnimateStrokeMove(1280 - 40 - (int)inkCanvasTemp.Width);
            // delete the storkeCollection back to inkCanvas3
            // because of unsuccessfull selectionMoving session           
            else
                AnimateStrokeDelete();
        }

        /// <summary>
        /// animate movement of stroke representing the task form teacher to student
        /// </summary>        
        private void AnimateStrokeMove(int animateToValue)
        {
            this.animateToValue = animateToValue;

            DoubleAnimation animationX = new DoubleAnimation(); // animate X Axis movement
            DoubleAnimation animationY = new DoubleAnimation(); // animate Y Axis movement
            //CubicEase easing = new CubicEase();
            ExponentialEase easing = new ExponentialEase();
            easing.EasingMode = EasingMode.EaseInOut;

            animationX.From = Canvas.GetLeft(inkCanvasTemp);
            animationX.To = animateToValue;

            animationY.From = Canvas.GetTop(inkCanvasTemp);
            animationY.To = 230;

            animationX.By = animationY.By = 1;
            //animationX.EasingFunction = animationY.EasingFunction = easing;
            //animationX.AccelerationRatio = animationY.AccelerationRatio = 0.5;
            animationX.DecelerationRatio = animationY.DecelerationRatio = 1;
            animationX.Duration = animationY.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            animationX.FillBehavior = animationY.FillBehavior = FillBehavior.Stop; // this is so important
            animationX.Completed += new EventHandler(AnimateMove_Completed);

            Storyboard storyBoardX = new Storyboard();
            Storyboard.SetTargetName(animationX, "inkCanvasTemp");
            Storyboard.SetTargetProperty(animationX, new PropertyPath(Canvas.LeftProperty));
            storyBoardX.FillBehavior = FillBehavior.Stop;      // this is so important
            storyBoardX.Children.Add(animationX);
            storyBoardX.Begin(canvasMain);

            Storyboard storyBoardY = new Storyboard();
            Storyboard.SetTargetName(animationY, "inkCanvasTemp");
            Storyboard.SetTargetProperty(animationY, new PropertyPath(Canvas.TopProperty));
            storyBoardY.FillBehavior = FillBehavior.Stop;      // this is so important
            storyBoardY.Children.Add(animationY);
            storyBoardY.Begin(canvasMain);
        }

        /// <summary>
        /// animate the delete of inkCanvasTemp strokes (in case of a bad movement session)
        /// </summary>
        private void AnimateStrokeDelete()
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 1;
            animation.To = 0;
            animation.By = 1;
            animation.DecelerationRatio = 1;
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(300));
            animation.FillBehavior = FillBehavior.Stop;    // this is so important
            animation.Completed += new EventHandler(animateDelete_Completed);

            Storyboard storyBoard = new Storyboard();
            Storyboard.SetTargetName(animation, "inkCanvasTemp");
            Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.OpacityProperty));
            storyBoard.FillBehavior = FillBehavior.Stop;   // this is so important
            storyBoard.Children.Add(animation);
            storyBoard.Begin(canvasMain);
        }

        private void AnimateMove_Completed(object sender, EventArgs e)
        {
            // add the stroke to inkCanvas1,2 (student inkanvas)
            // then clear and reset inkCanvasTemp to its initial state
            Point point = new Point(animateToValue, 230);
            inkCanvasTemp.CutSelection();
            // ckeck where to copy the animated tast, to inCanvas1 ro to inkCanvas2            
            if (animateToValue < 50)
                inkCanvas1.Paste(canvasMain.TranslatePoint(point, inkCanvas1));
            else
                inkCanvas2.Paste(canvasMain.TranslatePoint(point, inkCanvas2));
            ResetAfterAnimation();
        }

        private void animateDelete_Completed(object sender, EventArgs e)
        {
            ResetAfterAnimation();
        }

        /// <summary>
        /// Reset inkCanvasTemp tot its initial state
        /// </summary>
        private void ResetAfterAnimation()
        {
            inkCanvasTemp.Strokes.Clear();
            inkCanvasTemp.Height = inkCanvasTemp.Width = 0;
            inkCanvasTemp.Background = new SolidColorBrush(Colors.Transparent);
            Canvas.SetTop(inkCanvasTemp, 0);
            Canvas.SetLeft(inkCanvasTemp, 0);

            // reset inkCanvas Editing Mode = None
            inkCanvas3.EditingMode = InkCanvasEditingMode.None;
            SubscribeToDrawingModeEvents(ref inkCanvas3);
            buttonPen3.IsChecked = true;
            buttonEraser3.IsChecked = false;
            buttonSelect.IsChecked = false;
        }
        #endregion

        #region Expand/Collapse panel of student 1 with animation
        /// <summary>
        /// Expand / collapse panel of student 1 (the panel on the left)
        /// </summary>       
        private void buttonExpander_Click(object sender, RoutedEventArgs e)
        {
            // change button image and start animations
            if (buttonExpander.IsChecked == true)
                imageButtonExpander.Source = new BitmapImage(new Uri("/Pen.Math;component/Images/left.png", UriKind.Relative));
            else
                imageButtonExpander.Source = new BitmapImage(new Uri("/Pen.Math;component/Images/right.png", UriKind.Relative));

            AnimatePanelExpand((bool)buttonExpander.IsChecked);
            AnimatePanelFade((bool)buttonExpander.IsChecked);
            AnimateMovingAlgebraPanel((bool)buttonExpander.IsChecked);
        }

        /// <summary>
        /// Animate expand/collapse of student 1 panel 
        /// </summary>        
        private void AnimatePanelExpand(bool expand)
        {
            DoubleAnimation animation = new DoubleAnimation();
            // expand
            if (expand)
            {
                animation.From = -425;
                animation.To = 0;
                animation.AccelerationRatio = 0;
                animation.DecelerationRatio = 1;
            }
            // collapse
            else
            {
                animation.From = 0;
                animation.To = -425;
                animation.AccelerationRatio = 1;
                animation.DecelerationRatio = 0;
            }
            animation.By = 1;
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(500));

            Storyboard storyBoard = new Storyboard();
            Storyboard.SetTargetName(canvasStudent1, canvasStudent1.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.LeftProperty));
            storyBoard.Children.Add(animation);
            storyBoard.Begin(canvasStudent1);
        }

        /// <summary>
        /// Animate fade in/out of student 1 panel 
        /// </summary>        
        private void AnimatePanelFade(bool expand)
        {
            DoubleAnimation animation = new DoubleAnimation();
            EasingFunctionBase easing = new QuadraticEase();
            easing.EasingMode = EasingMode.EaseIn;
            animation.EasingFunction = easing;

            // fade in
            if (expand)
            {
                animation.From = 0;
                animation.To = 1;
            }
            // fadeout
            else
            {
                animation.From = 1;
                animation.To = 0;
            }
            animation.By = 1;
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            //animation.Completed += new EventHandler((object obj, EventArgs ev) => RotateAnimation());

            Storyboard storyBoard = new Storyboard();
            Storyboard.SetTargetName(canvasStudent1, canvasStudent1.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.OpacityProperty));
            storyBoard.Children.Add(animation);
            storyBoard.Begin(canvasStudent1);
        }

        /// <summary>
        /// Animate the movement of the algebra panel when student 1 panel is expanded/collapsed
        /// </summary>       
        private void AnimateMovingAlgebraPanel(bool expand)
        {
            //borderAlgebraPanel
            DoubleAnimation animation = new DoubleAnimation();
            EasingFunctionBase easing = new PowerEase();
            animation.EasingFunction = easing;
            easing.EasingMode = EasingMode.EaseOut;

            // move to right
            if (expand)
            {
                animation.From = 15;
                animation.To = 425;
            }
            // move to left
            else
            {
                animation.From = 425;
                animation.To = 15;
            }
            animation.By = 1;
            animation.Duration = new Duration(TimeSpan.FromMilliseconds((expand) ? 400 : 600));
            Storyboard storyBoard = new Storyboard();
            Storyboard.SetTargetName(borderAlgebraPanel, borderAlgebraPanel.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.LeftProperty));
            storyBoard.Children.Add(animation);
            storyBoard.Begin(borderAlgebraPanel);
        }
        #endregion

        #region Toggle Application Mode (Algebra and Geometry)

        /// <summary>
        /// Toggle between the two modes of this application window (Pen.Math)
        /// We have two modes: Algebra and Geometry. GUI only is to be changed slightly.
        /// </summary>         
        private void buttonApplicationMode_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton button = (ToggleButton)sender;

            // the following check is done to keep toggle buttons sticky, i.e toggle
            // button can't change it's state when it is clicked and is checked
            if (button == buttonAlgebraMode && buttonAlgebraMode.IsChecked == false)
            {
                buttonAlgebraMode.IsChecked = true;
                return;
            }
            else if (button == buttonGeometryMode && buttonGeometryMode.IsChecked == false)
            {
                buttonGeometryMode.IsChecked = true;
                return;
            }


            if (button == buttonAlgebraMode && buttonAlgebraMode.IsChecked == true)
            {
                // change button image 
                imageButtonAlgebra.Source = new BitmapImage(new Uri("/Pen.Math;component/Images/A1.png", UriKind.Relative));
                imageButtonGeometry.Source = new BitmapImage(new Uri("/Pen.Math;component/Images/G2.png", UriKind.Relative));
                buttonGeometryMode.IsChecked = false;
                AlgebraMode();
            }
            else
            {
                // change button image 
                imageButtonAlgebra.Source = new BitmapImage(new Uri("/Pen.Math;component/Images/A2.png", UriKind.Relative));
                imageButtonGeometry.Source = new BitmapImage(new Uri("/Pen.Math;component/Images/G1.png", UriKind.Relative));
                buttonAlgebraMode.IsChecked = false;
                GeometryMode();
            }

        }

        /// <summary>
        /// Change GUI of the application to be suitable for Algebra Mode.
        /// </summary>
        private void AlgebraMode()
        {
            // 1. change background of teacher inkCanvcas to white
            // 2. show panel of Algebra controls
            // 3. hide panel of Geometry controls
            // 4. enable Select, Expand  button

            inkCanvas3.Background = new SolidColorBrush(Colors.White);
            ExpandPanel(borderAlgebraPanel, true);
            ExpandPanel(borderGeometryPanel, false);
            buttonSelect.IsEnabled = true;
            buttonExpander.IsEnabled = true;
        }

        /// <summary>
        /// Change GUI of the application to be suitable for Geometry Mode.
        /// </summary>
        private void GeometryMode()
        {
            // 1. Collapse panel of student 1 if expanded
            // 2. change background of teacher inkCanvcas to transparent
            // 3. show panel of Geometry controls
            // 4. hide panel of Algebra controls

            if (buttonExpander.IsChecked == true)
            {
                imageButtonExpander.Source = new BitmapImage(new Uri("/Pen.Math;component/Images/right.png", UriKind.Relative));
                buttonExpander.IsChecked = false;
                AnimatePanelExpand(false);
                AnimatePanelFade(false);
                AnimateMovingAlgebraPanel(false);
            }

            inkCanvas3.Background = new SolidColorBrush(Colors.Transparent);
            ExpandPanel(borderGeometryPanel, true);
            ExpandPanel(borderAlgebraPanel, false);
            buttonSelect.IsEnabled = false;
            buttonExpander.IsEnabled = false;
        }

        /// <summary>
        /// Animate expand/collapse of panel of Geometry controls
        /// </summary>        
        private void ExpandPanel(Border border, bool expand)
        {
            //borderAlgebraPanel
            DoubleAnimation animation = new DoubleAnimation();
            EasingFunctionBase easing = new PowerEase();
            animation.EasingFunction = easing;
            easing.EasingMode = EasingMode.EaseOut;

            // move up
            if (expand)
            {
                animation.From = 810;
                animation.To = 705;
            }
            // move to down
            else
            {
                animation.From = 705;
                animation.To = 810;
            }
            animation.By = 1;
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            Storyboard storyBoard = new Storyboard();
            Storyboard.SetTargetName(border, border.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.TopProperty));
            storyBoard.Children.Add(animation);
            storyBoard.Begin(border);

        }

        ///// <summary>
        ///// Animate expand/collapse of panel of Algebra controls
        ///// </summary>     
        //private void ExpandAlgebraPanel(bool expand)
        //{
        //}
        #endregion

        #region Event handlers of Algebra Mode buttons
        /// <summary>
        /// open math input panel
        /// </summary>
        private void buttonMathPanel_TouchDown(object sender, TouchEventArgs e)
        {
            mathPanel.MathPanelShow();
        }

        /// <summary>
        /// graph an equation using MS Word Mathematics addin
        /// </summary>        
        private void buttonGraph_TouchDown(object sender, TouchEventArgs e)
        {
            // Run this in a separate thread, performance issue
            // Note: I found that this is not a good idea as we can't access 
            // office objects (wordApp, wordDoc) after this thread ended            
            //Thread wordThread = new Thread(() => RunOfficeApp());
            //wordThread.SetApartmentState(ApartmentState.STA);
            //wordThread.Start();
            // run office word app           
            RunWordApp();
        }
        #endregion

        /// <summary>
        /// Define the type of shape to be drawn, or None if no shape to be drawn.
        /// </summary>
        enum Shape_ { Cicle, Line, None, Rectangle, Triangle };        
        Shape_ shape = Shape_.None;
        /// <summary>
        /// Start drawing Shapes
        /// </summary>        
        private void buttonShape_Click(object sender, RoutedEventArgs e)
        {
            string name = ((Button)sender).Name;

            switch (name)
            {
                case "buttonLine":
                    shape = Shape_.Line;
                    break;
                case "buttonRectangle":
                    shape = Shape_.Rectangle;
                    break;
                case "buttonCircle":
                    shape = Shape_.Cicle;
                    break;
                case "buttonTriangle":
                    shape = Shape_.Triangle;
                    break;
                default:
                    MessageBox.Show("Error is switch case in buttonShape_Click() method");
                    break;
            }
        }

        private void button6_TouchDown(object sender, TouchEventArgs e)
        {
            TouchDevice touch1 = e.TouchDevice;
            //touch1
            //textBox1.Text =  e.TouchDevice.Id.ToString();
            //e.TouchDevice.
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            //PostOnFaceBookWall("First Post ON Facebook");           

        }        
    }
}