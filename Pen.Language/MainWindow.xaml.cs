// Documentation
//
// # 1. To get translation service from bing, you have first to create an app
// to get this ID: 3F6FF87B1C84B1648DC2FDE24A1717566615940F
// while app id that was in Example: 6CE9C85A41571C050C379F60DA173D286384E0F2
// link for ping translator service: http://api.microsofttranslator.com/V1/SOAP.svc

// # Color themes used in the applications
// Pen.Math:         orange   rgb = (255, 100, 0)
// Pen.Maps:         blue     rgb = (0,180, 0)
// Pen.Maps(tool):   blue     rgb = (0,170, 0)
// Pen.Language:     green    rgb = (60,210, 60)

// # best online TTS services are
// acapela-group.com   
//               Ryan    - US English
//               Heather - US English        
//               Lucy    - UK English
//               Rachel  - UK English
//               Graham  - UK English
//               Claire  - Frensh
// Ivona.com  

// # Lesson Files
// It is to be noticed that lesson files here in this application are files responsible of
// carrying the content of the lesson. The content of the lesson has two main parts:
// 1. The text, which is written using MS Word application.
// 2. The ink illustration, which is a collection of handwriting + ink notes with the pen +
//    ink notes with the marker.
// The lesson content (text + illustration) is saved in one file with this extension: 
// plf (pen language file). This file is simply a compressed (with code) two files
// a. ms word file i.e. .docx file   b. a serialized .osl file which carries ink illustration.
// Those two files (.docx and .osl) files are compressed (zipped) by code and saved as .plf file.
// The user can open a word file and add ink illustrations to it and then save it as a .plf file.
// User (teacher/student) can also open a .plf file, edit it and save it again. This enables the user
// to save his lesson content for furthur classes in the future. This file is to be integrated
// with the online file sharing service that will be offered by the website of the project (ISA).

// #### Ideas for evolution of this application. #### There must be a way by which a teacher
//   can save his lesson (formated text + ink notes) to a certain file then share this
//   file on the internet. Also another way enables teachers to open these lesson files
//   in the application. These two features will let teachers create and share lesson 
//   files between them, which, eventually, will increase productivity. The content of 
//   any lesson file has two main parts: text and ink strokes. The text can be edited here
//   or in MS word application. The ink strokes are the notes, makrers and illustrations 
//   that the teacher takes them on the lesson text, while conducting this lesson. The ink 
//   notes are exlusively edited here in this application. So both of them (text and ink strokes)
//   form a lesson content.  Also the lesson greenboard element(textBoxWrite + inkCanvasLesson)
//   must be scrollable to provide the teacher up to 5 times the current visible space.
//   i.e. the teacher can have five pages to conduct his lesson. There must be two vertical
//   scrolling buttons to scroll between these pages (lesson content). These scrolling buttons
//   are 1. freehand scrolling 2. adjused scrolling. The free hand scrolling enables the teacher
//   to scroll between pages with the flickr action. While the adjusted scrolling will scrill 
//   untill the first line of the new page will be on the top of the greenboard.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Windows.Ink;
using System.Reflection;
using System.Threading;
using System.Media;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System.Speech.Synthesis;
using System.Speech.Recognition;

using Google.API.Translate;
using Ionic.Zip;

using Word = Microsoft.Office.Interop.Word;
using Ink = System.Windows.Ink;
using System.Windows.Markup;

namespace Pen.Language
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
            FullScreen();
            imageBackground.Height = 1280;
            this.VisualTextHintingMode = TextHintingMode.Animated;

            // this is required to be able to take flowDocument content
            // of richTextBoxTool
            var mainWindowViewModel = new MainWindowViewModel();
            this.DataContext = mainWindowViewModel;
        }

        // load/initialize required objects
        private void windowMain_Loaded(object sender, RoutedEventArgs e)
        {
            // start the watch
            StartTime();
            // this populates a list of different language names
            // this is required for language translation with bingTranslator() method
            // and for languageSpecifier() method
            LoadLanguageList();
            // Consumes 2 Mbutes RAM    
            InitializeInkAnalyzer();
            // consumes 8-10 MBytes RAM
            InitializeSpeechRecognition();
            // Cosumes 3-4 MByes RAM
            InitializeSpellingChecker();
            // Initialize scrolling events
            InitializeScrollingEvents();
            // set the inkCanvas of the lesson to its initial mode
            // which is handwriting mode
            toggleButton1.IsChecked = true;
            inkCanvasLesson.DefaultDrawingAttributes.Color = handwritingColor;
            // set style of the webfilePanel
            webfilePanel.FileType_ = Tools.FileType.Language;

            //textBoxLesson.SpellCheck.IsEnabled = true;
            //windowMain.UseLayoutRounding = true;
        }

        /// <summary>
        /// Because this application is multilanguage, we have to load the starting
        /// language of the appllication. UP till now, we target English, Frensh and Germany
        /// </summary>
        private void LanguageSpecifier(Languages startLanguage)
        {
            //button1.Content = "Translate";
            //button2.Content = "Spelling";
            //button3.Content = "Write";

            if (startLanguage == Languages.French)
            {
                button1.Content = "Traduire";
                button2.Content = "Orthographe";
                button3.Content = "Écrire";
                // set current input language
                InputLanguageManager.Current.CurrentInputLanguage = System.Globalization.CultureInfo.GetCultureInfo(1025);

            }
            else if (startLanguage == Languages.Germany)
            {

            }
        }

        #region Current Window event handlers and methods
        // close this application if Esc key ie pressed
        private void windowMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Application.Current.Shutdown();
        }

        // safely close currently loaded objects
        private void windowMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CloseSpellingChecker();
            DisposeInkAnalyzer();
            DisposeSpeechRecognizer();
        }

        // event handler for close button
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
        /// show and update the time in the captionText of the windows. Also Update animation
        /// of application background.
        /// </summary>
        private void StartTime()
        {
            string time = DateTime.Now.ToShortTimeString();
            textCaption.Text = "Pen Enlgish |  " + time.Remove(time.Length - 3);

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(1);
            timer.Tick += new EventHandler((object sender, EventArgs e) =>
            {
                time = DateTime.Now.ToShortTimeString();
                textCaption.Text = "Pen Enlgish |  " + time.Remove(time.Length - 3);
                //AnimateBackground();
            });
            timer.Start();
        }
        #endregion

        #region Background Animation
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
            AnimatwRotation(rotateFrom, rotateTo);
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
        private void AnimatwRotation(int rotateFrom, int rotateTo)
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

        #region Language translation
        private enum Languages { Arabic, English, French, Germany };
        private Dictionary<Languages, string> languages = new Dictionary<Languages, string>();
        /// <summary>
        /// Load language list with language names. This is required
        /// for Bing language translator
        /// </summary>
        private void LoadLanguageList()
        {
            languages.Add(Languages.Arabic, "ar");
            languages.Add(Languages.English, "en");
            languages.Add(Languages.French, "fr");
            languages.Add(Languages.Germany, "gr");
        }

        /// <summary>
        /// Takes a string to translate its text form (source language)to (result language).
        /// Takes also a ref of textBox to set (aschynchronously) result text to it
        /// Takes also source language(from) and result language (to)       
        /// </summary>
        private void BingTranslator(string source, TextBox result, Languages from, Languages to)
        {
            string appID = "3F6FF87B1C84B1648DC2FDE24A1717566615940F";
            string resultText; // Traduire ce texte pour moi!            

            Thread thread = new Thread(() =>
            {
                // run tranlation process
                TranslatorService.LanguageServiceClient translator = new TranslatorService.LanguageServiceClient();
                translator = new TranslatorService.LanguageServiceClient();
                resultText = translator.Translate(appID, source, languages[from], languages[to]);
                WPFThreadingExtensions.SetTextThreadSafe(result, resultText);
            });

            thread.Start();
        }

        /// <summary>
        /// Takes a string to translate its text form (source language)to (result language).
        /// Takes also a ref of textBox to set (aschynchronously) result text to it
        /// Takes also source language(from) and result language (to)       
        /// </summary>
        private void GoogleTranslator(string source, TextBox result, Google.API.Translate.Language from, Google.API.Translate.Language to)
        {
            string resultText;
            Thread thread = new Thread(() =>
            {
                resultText = Translator.Translate(source, from, to);
                WPFThreadingExtensions.SetTextThreadSafe(result, resultText);
            });

            thread.Start();
        }

        /// <summary>
        /// Takes a string to translate its text form (source language)to (result language).        
        /// Takes also source language(from) and result language (to)       
        /// </summary>
        private string BingTranslator(string source, Languages from, Languages to)
        {
            string appID = "3F6FF87B1C84B1648DC2FDE24A1717566615940F";
            string resultText; // Traduire ce texte pour moi!            

            // run tranlation process
            TranslatorService.LanguageServiceClient translator = new TranslatorService.LanguageServiceClient();
            translator = new TranslatorService.LanguageServiceClient();
            resultText = translator.Translate(appID, source, languages[from], languages[to]);
            return resultText;
        }
        #endregion

        #region Spelling Checking with MS Word
        // required objects
        Word._Application wordApp;
        Word._Document wordDoc;
        Word.ProofreadingErrors spellingErrors;

        /// <summary>
        /// Initialize and open Microsoft Word Spelling Checker. Start office word spell checker
        /// in a separate thread to improve performance and it works fine.
        /// </summary>
        private void InitializeSpellingChecker()
        {
            Thread thread = new Thread(() =>
            {
                // Setting these variables is comparable to passing null to the function. 
                // This is necessary because the C# null cannot be passed by reference.
                object template = Missing.Value;
                object newTemplate = Missing.Value;
                object documentType = Missing.Value;
                object visible = false;
                object optional = Missing.Value;

                // create word app and word document            
                wordApp = new Word.Application();
                wordDoc = wordApp.Documents.Add(ref template,
                    ref newTemplate, ref documentType, ref visible);
                spellingErrors = wordDoc.SpellingErrors;
            });
            thread.Start();
        }

        /// <summary>
        /// Close Microsoft Word Spelling Checker
        /// </summary>
        private void CloseSpellingChecker()
        {
            // close the app with out saving
            object saveChanges = false;
            object originalFormat = Missing.Value;
            object routeDocument = Missing.Value;

            if (wordDoc != null)
                wordDoc.Close(ref saveChanges, ref originalFormat, ref routeDocument);

            if (wordApp != null)
                wordApp.Quit(ref saveChanges, ref originalFormat, ref routeDocument);
        }

        /// <summary>
        /// Take a text to apply spellChecking on it and returns the corrected text
        /// </summary>        
        private string WordSpellingChecker(String text)
        {
            object optional = Missing.Value;
            wordDoc.Words.First.InsertBefore(text);

            // check spelling            
            wordDoc.CheckSpelling(
                ref optional, ref optional, ref optional, ref optional,
                ref optional, ref optional, ref optional, ref optional,
                ref optional, ref optional, ref optional, ref optional);

            object first = 0;
            object last = wordDoc.Characters.Count - 1;
            Word.Range wordRange = wordDoc.Range(ref first, ref last);

            // take the corrected string from the word and clear the word text range
            string correctedText = wordRange.Text;
            wordRange.Delete();
            return correctedText;
        }
        #endregion

        #region Speech Recognition and Text-to-Speech TTS
        // speech recognition object
        SpeechRecognizer speechRec;
        SpeechRecognitionEngine speechRecEngine;

        /// <summary>
        /// Event handler for speach recognition button
        /// </summary>        
        private void buttonSpeechRecognition_Click(object sender, RoutedEventArgs e)
        {
            // speech recognititon
            SpeechRecognition((ToggleButton)sender);
        }

        /// <summary>
        /// Use it to speak the sent text. SpeechSynthesizer Sould be used 
        /// locally so as to dispose the it after using it. This is because
        /// there are unmanaged resources that need to be freed. This method runs in
        /// a separate thread to improve performance.
        /// </summary>        
        private void SpeechSynthesize(string textToSpeak)
        {
            // the difference between by approach used below and using speakAsynch() method
            // is that when the application closes suddenly by user while there is text is being
            // spoken (synthesized), speakAsynch() method makes the application crash 
            // while my approach doen't crash due to this suddent shut down           

            Thread speechThread = new Thread(() =>
            {
                SpeechSynthesizer speechSynth = new SpeechSynthesizer();
                speechSynth.Speak(textToSpeak);
                speechSynth.Dispose();
            });
            speechThread.Start();
        }

        /// <summary>
        /// Intialize objects and grammars required for speech recognition using Microsoft 
        /// Speeach API (SAPI). Load grammar in a separate thread to improve performance.
        /// </summary>
        private void InitializeSpeechRecognition()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler((object sender, EventArgs e) =>
            {
                //speechRec = new SpeechRecognizer();
                speechRecEngine = new SpeechRecognitionEngine();

                // Train the system to recognize these sentences            
                Choices c = new Choices();
                for (var i = 0; i <= 100; i++)
                    c.Add(i.ToString());

                var grammarBuilder = new GrammarBuilder(c);
                Grammar grammar = new Grammar(grammarBuilder);

                //speechRecEngine.LoadGrammar(new DictationGrammar());
                speechRecEngine.LoadGrammar(grammar);
                speechRecEngine.SetInputToDefaultAudioDevice();
                speechRecEngine.SpeechRecognized += Rec_SpeechRecognized;
                timer.Stop();

            });

            timer.Start();
        }

        /// <summary>
        /// Start/Stop Speech Recognition according to state of toggle button sent
        /// </summary>
        private void SpeechRecognition(ToggleButton button)
        {
            System.Windows.Media.Effects.DropShadowEffect effect = new System.Windows.Media.Effects.DropShadowEffect();
            effect.Color = Colors.Red; //.GreenYellow;
            effect.Opacity = 1;
            effect.ShadowDepth = 0;
            effect.BlurRadius = 15;

            // start voice recognition
            if (button.IsChecked == true)
            {
                ((TextBlock)((StackPanel)button.Content).Children[1]).Text = "On";
                speechRecEngine.RecognizeAsync(RecognizeMode.Multiple);
                ((Image)((StackPanel)button.Content).Children[0]).Source = new BitmapImage(new Uri("/Pen.Language;component/Images/micon.png", UriKind.Relative));
                button.Effect = effect;
            }

            // stop speech recognition
            else
            {
                speechRecEngine.RecognizeAsyncStop();
                ((TextBlock)((StackPanel)button.Content).Children[1]).Text = "Off";
                ((Image)((StackPanel)button.Content).Children[0]).Source = new BitmapImage(new Uri("/Pen.Language;component/Images/micoff.png", UriKind.Relative));
                button.Effect = null;
            }
        }

        /// <summary>
        /// Event handler for voice recognition. Called when a new voice is recognized.
        /// </summary>        
        private void Rec_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            textBoxLesson.AppendText(e.Result.Text);
        }

        /// <summary>
        /// Safely close the speech recognizer
        /// </summary>
        private void DisposeSpeechRecognizer()
        {
            if (speechRecEngine != null)
            {
                speechRecEngine.RecognizeAsyncStop();
                speechRecEngine.Dispose();
            }

        }
        #endregion

        #region Handwriting recognition by inkAnalyser, WPF
        /// <summary>
        /// object resposible of handwriting recognititon
        /// </summary>
        InkAnalyzer inkAnalyzer = new InkAnalyzer();
        /// <summary>
        /// Handles the automation of handwriting recognititon. i.e. the recognititon is 
        /// done automaticly after the user write something.
        /// </summary>
        System.Timers.Timer inkAnalyzerTimer = new System.Timers.Timer();
        /// <summary>
        /// Reference to the strokes of the handwriting. These strokes are tow be added every
        /// handwriting session and removed after handwriting recognition. (end of session)
        /// </summary>
        StrokeCollection handwritingStrokes = new StrokeCollection();

        /// <summary>
        /// Initilize ink analyzer and timer. Both of them are required in handwriting recognition.
        /// </summary>
        private void InitializeInkAnalyzer()
        {
            // writing settings of inkCanvasLesson
            inkCanvasLesson.DefaultDrawingAttributes.IgnorePressure = true;
            inkCanvasLesson.DefaultDrawingAttributes.Height =
                inkCanvasLesson.DefaultDrawingAttributes.Width = 2;

            inkAnalyzer.ResultsUpdated += new ResultsUpdatedEventHandler(inkAnalyzer_ResultsUpdated);
            inkCanvasLesson.StrokeCollected += new InkCanvasStrokeCollectedEventHandler(inkCanvasLesson_StrokeCollected);

            inkAnalyzerTimer.Interval = 1500;
            inkAnalyzerTimer.AutoReset = false;
            inkAnalyzerTimer.Elapsed += new System.Timers.ElapsedEventHandler((object obj, System.Timers.ElapsedEventArgs eventArgs) =>
            {
                //inkAnalyzer.AddStrokes(handwritingStrokes);
                inkAnalyzer.BackgroundAnalyze();
            });
        }

        /// <summary>
        /// Stop the inkAnalyzer timer (if it is running), becuse user start wirting a new stroke.
        /// </summary>        
        private void inkCanvasLesson_TouchDown(object sender, TouchEventArgs e)
        {
            inkAnalyzerTimer.Stop();
        }

        /// <summary>
        /// Start the inkAnalyzer timer, which waits for a small period (1.5 second), if the user don't
        /// hit a new stroke, then the timer will do only one tick to analyze the handwriting.
        /// </summary> 
        private void inkCanvasLesson_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            // background analyzing
            if (isHandwriting)
            {
                handwritingStrokes.Add(e.Stroke);
                inkAnalyzer.AddStroke(e.Stroke);
                inkAnalyzerTimer.Start();
            }
        }

        /// <summary>
        /// Returns the result text form handwrtiting recognition (ink analyzer)
        /// </summary>        
        private void inkAnalyzer_ResultsUpdated(object sender, ResultsUpdatedEventArgs e)
        {
            string recognizedText = inkAnalyzer.GetRecognizedString();
            // remove handwriting strokes only from: the inkCanvasLesson and the inkAnalyzer
            inkAnalyzer.RemoveStrokes(handwritingStrokes);
            inkCanvasLesson.Strokes.Remove(handwritingStrokes);
            handwritingStrokes.Clear();

            // First of all, we have t1: text of the textBox 
            //                       t2: selected text of the textBox
            //                       t3: recognized text
            // If there is (t2), then (t2) is replaced with (t3 + space)
            // If there is no (t2), the apend (space + t3) to (t1)           
            if (!string.IsNullOrEmpty(textBoxLesson.Selection.Text))
            {
                textBoxLesson.Selection.Text = recognizedText + " ";
                // remove selection after setting the selected text
                textBoxLesson.Selection.Select(textBoxLesson.Selection.Start, textBoxLesson.Selection.Start);
            }
            else
                textBoxLesson.AppendText(" " + recognizedText);
        }

        /// <summary>
        /// Safely close ink analyzer
        /// </summary>
        private void DisposeInkAnalyzer()
        {
            if (inkAnalyzer != null)
                inkAnalyzer.Dispose();
        }
        #endregion

        #region Change Editing mode of inkCanvasLesson and Color of Pen/Marker
        /// <summary>
        /// Determine if the inkCanvasLesson is now in handwriting mode or not.
        /// This is to determine wheather to deal with the drawn stokes
        /// as handwriting or as just ordinary/normal notes taken by the user.
        /// </summary>
        private bool isHandwriting = true;
        // default colors for the pen and the marker of the Lesson inkCanvas
        private Color penColor = Colors.Orange;
        private Color markerColor = Colors.Yellow;
        private Color handwritingColor = Colors.Gray;

        /// <summary>
        /// Event handlers for button responsible of changing the mode of Writing Grid.
        /// These modes are (Text Editing, hand writing, Scrolling, Pen, Eraser and Marker).
        /// </summary>        
        private void buttonMode_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = (ToggleButton)sender;
            string name = toggleButton.Name;

            // this makes the toggleButton stickey, which means that when user
            // try to un-check a checked toggleButton, it will not works
            if (toggleButton.IsChecked == false)
            {
                toggleButton.IsChecked = true;
                return;
            }

            // determine which mode to set according to the sender toggleButton
            string buttonId = name.Substring(12, 1);

            ToggleButtons(toggleButton);
            if (buttonId == "1" || buttonId == "3" || buttonId == "4")
                ChangeDrawingSettings();

            switch (buttonId)
            {
                // handwriting recognition mode
                case "1":
                    isHandwriting = true;
                    textBoxLesson.IsHitTestVisible = false;
                    scrollViewerLesson.IsManipulationEnabled = false;
                    inkCanvasLesson.EditingMode = InkCanvasEditingMode.Ink;                    
                    break;

                // text editing mode
                case "2":
                    textBoxLesson.IsHitTestVisible = true;
                    scrollViewerLesson.IsManipulationEnabled = false;
                    inkCanvasLesson.EditingMode = InkCanvasEditingMode.None;
                    break;

                // (pen drawing mode) take notes with the pen
                case "3":
                    isHandwriting = false;
                    textBoxLesson.IsHitTestVisible = false;
                    scrollViewerLesson.IsManipulationEnabled = false;
                    inkCanvasLesson.EditingMode = InkCanvasEditingMode.Ink;
                    break;

                // (marker drawing mode) take notes with the marker
                case "4":
                    isHandwriting = false;
                    textBoxLesson.IsHitTestVisible = false;
                    scrollViewerLesson.IsManipulationEnabled = false;
                    inkCanvasLesson.EditingMode = InkCanvasEditingMode.Ink;
                    break;

                // erasing mode
                case "5":
                    textBoxLesson.IsHitTestVisible = false;
                    scrollViewerLesson.IsManipulationEnabled = false;
                    inkCanvasLesson.EditingMode = InkCanvasEditingMode.EraseByStroke;
                    break;

                // Scrolling
                case "6":
                    textBoxLesson.IsHitTestVisible = false;
                    scrollViewerLesson.IsManipulationEnabled = true;
                    inkCanvasLesson.EditingMode = InkCanvasEditingMode.None;
                    break;

                default:
                    MessageBox.Show("There must be an error in switch case in buttonMode_Click() method");
                    break;
            }
        }

        /// <summary>
        /// when a toggle button is checked, we want to uncheck the others
        /// </summary>
        private void ToggleButtons(ToggleButton buttonChecked)
        {
            List<ToggleButton> buttons = new List<ToggleButton>();
            buttons.Add(toggleButton1);
            buttons.Add(toggleButton2);
            buttons.Add(toggleButton3);
            buttons.Add(toggleButton4);
            buttons.Add(toggleButton5);
            buttons.Add(toggleButton6);

            foreach (ToggleButton button in buttons)
                if (button.Name != buttonChecked.Name && button.IsChecked == true)
                    button.IsChecked = false;
        }

        /// <summary>
        /// Event Handler for selected color changed of colorPicker
        /// </summary>       
        private void radColorPicker1_SelectedColorChanged(object sender, EventArgs e)
        {
            // if Pen is checked, change its color               // toggleButton 3
            // else if Marker is checked change its color        // toggleButton 4
            // else if Handwriting is checked, change its color  // toggleButton 1
            // else if both of them is unchecked, do nothing

            if (toggleButton1.IsChecked == true)
            {
                handwritingColor = radColorPicker1.SelectedColor;
                inkCanvasLesson.DefaultDrawingAttributes.Color = handwritingColor;
            }
            else if (toggleButton3.IsChecked == true)
            {
                penColor = radColorPicker1.SelectedColor;
                inkCanvasLesson.DefaultDrawingAttributes.Color = penColor;
            }
            else if (toggleButton4.IsChecked == true)
            {
                markerColor = radColorPicker1.SelectedColor;
                inkCanvasLesson.DefaultDrawingAttributes.Color = markerColor;
            }
        }

        /// <summary>
        /// Changes the drawing settings of the inkCanvasLesson according to the checked
        /// toggleButton. There are 2 drawing setting for 2 drawing modes: Pen and Marker
        /// There are 3 color settings for 4 modes: HandWriting, Pen and Marker
        /// </summary>
        private void ChangeDrawingSettings()
        {
            // change drawing settings to the  Handwriting and Pen Modes
            // if Handwriting or Pen toggle button is checked
            if (toggleButton1.IsChecked == true || toggleButton3.IsChecked == true)
            {
                inkCanvasLesson.DefaultDrawingAttributes.Height
                    = inkCanvasLesson.DefaultDrawingAttributes.Width = 2;
                inkCanvasLesson.DefaultDrawingAttributes.StylusTip = StylusTip.Ellipse;
                inkCanvasLesson.DefaultDrawingAttributes.IsHighlighter = false;

                // make selection color of the colorPicker equal to the pen color
                if (toggleButton1.IsChecked == true)
                    radColorPicker1.SelectedColor = handwritingColor;
                else
                    radColorPicker1.SelectedColor = penColor;
            }
            // change drawing settings to the Marker Mode if Marker toggle button is checked
            else if (toggleButton4.IsChecked == true)
            {
                inkCanvasLesson.DefaultDrawingAttributes.Height = 25;
                inkCanvasLesson.DefaultDrawingAttributes.Width = 10;
                inkCanvasLesson.DefaultDrawingAttributes.StylusTip = StylusTip.Rectangle;
                inkCanvasLesson.DefaultDrawingAttributes.IsHighlighter = true;
                // make selection color of the colorPicker equal to the pen color
                radColorPicker1.SelectedColor = markerColor;
            }
        }
        #endregion

        #region Scrolling Up and Down the Lesson Content, smoothly
        /// <summary>
        /// Determine which page is currently viewed in the lesson content
        /// </summary>
        private int currentPage = 1;

        // scroll the lesson content (pages) one page up/down smoothly
        private void buttonScroll_Click(object sender, RoutedEventArgs e)
        {
            ScrollLessonContent((Button)sender);
        }

        /// <summary>
        /// Scroll the stackPanel of the Lesson Content one title up/down smoothly
        /// </summary>
        private void ScrollLessonContent(Button button)
        {
            // check if we are at first page and user scrolling up
            // or we are at last page and user scrolling down.
            if ((button == buttonUp && currentPage == 1) || (button == buttonDown && currentPage == 4))
                return;

            // Height of a page of the Lesson Content
            // Lesson Content has 4 pages, height of each is 620 pixel.
            int step = 0;

            // determine to scroll up or scroll down
            if (button == buttonDown)
                step = 1;
            else
                step = -1;

            currentPage += step;
            step *= 620;

            // get Y offset of the scrollViewer
            double currentYaxis = scrollViewerLesson.TransformToVisual(Application.Current.MainWindow as FrameworkElement).Transform(new Point(0, 0)).Y;

            // now scroll from the current vertical offset to the destination vertical offset
            // which is equal to the current + step           

            mediator.ScrollViewer = scrollViewerLesson;
            canvasMain.RegisterName("mediator", mediator);

            DoubleAnimation animation = new DoubleAnimation();
            EasingFunctionBase easing = new CubicEase();
            easing.EasingMode = EasingMode.EaseOut;

            animation.By = 1;
            animation.EasingFunction = easing;
            animation.From = scrollViewerLesson.VerticalOffset;
            animation.To = scrollViewerLesson.VerticalOffset + step;
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(800));

            Storyboard storyBoard = new Storyboard();
            Storyboard.SetTargetName(animation, "mediator");
            Storyboard.SetTargetProperty(animation, new PropertyPath(ScrollViewerOffsetMediator.VerticalOffsetProperty));
            storyBoard.Children.Add(animation);
            storyBoard.Begin(this);

        }
        #endregion

        #region Processing Lesson Files (New, Open, Save)
        /// <summary>
        /// Clear the Content of the Lesson elements so that a user can write new one.
        /// </summary>
        private void NewLesson()
        {
            // clear text
            textBoxLesson.Document.Blocks.Clear();
            // clear all old strokes (handwriting strokes from analyzer
            // and illustration strokes from inkCanvas           
            if (inkAnalyzer.IsAnalyzing)
                inkAnalyzer.Abort();
            inkAnalyzer.RemoveStrokes(handwritingStrokes);
            handwritingStrokes.Clear();
            inkCanvasLesson.Strokes.Clear();
        }

        /// <summary>
        /// Open Lesson File using open file dialog
        /// </summary>
        private void OpenLessonFile()
        {
            // directory of the file containing lasted accessed directory by user
            string LessonFile = Environment.CurrentDirectory + "\\LessonDirectory.txt";

            // 1. Open file dialog
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.Title = "Open ...";
            fileDialog.Filter = " Pen Language File (*.plf)|*.plf| MS Word File (*.rtf)|*.rtf";
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
            writer.Write(System.IO.Path.GetDirectoryName(fileDialog.FileName));
            writer.Close();

            // 2. Load the Lesson Content
            // after the brevious successful Scenario (ShowFileDialog)
            // successfull means the user clicked open
            ExtractLesson(fileDialog.FileName);
        }

        /// <summary>
        /// Save lesson content to a file, using save file dialog
        /// </summary>
        private void SaveLessonFile()
        {
            // directory of the file containing lasted accessed directory by user
            string LessonDirectory = Environment.CurrentDirectory + "\\LessonDirectory.txt";

            string fileName = string.Empty;
            string targetPath = "C:\\Users\\" + Environment.UserName + "\\Desktop";

            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog fileDialog = new Microsoft.Win32.SaveFileDialog();
            fileDialog.FileName = fileName;
            fileDialog.Title = "Save As ...";
            fileDialog.Filter = " Pen Language File (*.plf)|*.plf| Rich Text File (*.rtf)|*.rtf| Word Document (*.docx)|*.docx";

            // first, try last saved directory by user (found in Directory text file)
            // If not found, open directory to desktop
            try
            {
                TextReader reader = new StreamReader(LessonDirectory);
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
            TextWriter writer = new StreamWriter(LessonDirectory);
            writer.Write(System.IO.Path.GetDirectoryName(fileDialog.FileName));
            writer.Close();

            // 2. Save the Lesson Content
            // after the brevious successful Scenario (ShowFileDialog)
            // successfull means the user clicked save
            CompressLesson(fileDialog.FileName);
        }

        /// <summary>
        /// Read the content of MS word File. 
        /// Takes the file path and put its content in the lesson richTextBox.
        /// </summary>
        private void ReadWordFile(string filePath)
        {
            // there are two way to get this job done: easy and hard

            // 1. The easy way
            FileStream stream = new FileStream(filePath, FileMode.Open);
            TextRange textRange = new TextRange(textBoxLesson.Document.ContentStart, textBoxLesson.Document.ContentEnd);
            textRange.Load(stream, DataFormats.Rtf);
            stream.Close();

            #region 2. The hard way
            // Specify path for word file
            filePath = @"C:\Users\Nour El-Dien\Desktop\Development_Program.doc";
            object file = filePath;
            object visible = false;
            object nullobj = System.Reflection.Missing.Value;
            Word._Application app = new Word.Application();
            Word._Document doc = app.Documents.Open(ref file, ref nullobj, ref nullobj, ref nullobj,
                                                   ref nullobj, ref nullobj, ref nullobj, ref nullobj,
                                                   ref nullobj, ref nullobj, ref nullobj, ref visible,
                                                   ref nullobj, ref nullobj, ref nullobj, ref nullobj);
            doc.ActiveWindow.Selection.WholeStory();
            doc.ActiveWindow.Selection.Copy();

            // get data from file
            IDataObject data = Clipboard.GetDataObject();
            string text = data.GetData(DataFormats.Rtf).ToString();
            doc.Close(ref nullobj, ref nullobj, ref nullobj);
            wordApp.Quit(ref nullobj, ref nullobj, ref nullobj);
            #endregion
        }

        /// <summary>
        /// Compress Lesson Data (text data and inkNotes data) to the Lesson file 
        /// whose name is the sent string.
        /// </summary>
        private void CompressLesson(string zipFilePath)
        {
            // file name is temp_MM.DD_HH.MM.SS.osl [Month.Day_Hour.Minute.Second]
            string tempFile = "temp_" +
                DateTime.Now.Day.ToString() + "." + DateTime.Now.Month.ToString() + "_" +
                DateTime.Now.Hour.ToString() + "." + DateTime.Now.Minute.ToString() + "." +
                DateTime.Now.Second.ToString() + ".zip";

            // 1.Serialize the strokes of inkCanvasLesson to a temporary file.
            Stream stream = File.Open(tempFile + "ink", FileMode.Create);
            inkCanvasLesson.Strokes.Save(stream);
            stream.Close();

            // 2.Save Document Data of the textBoxLesson to a temporary file.
            stream = File.Open(tempFile + "doc", FileMode.Create);
            System.Windows.Markup.XamlWriter.Save(textBoxLesson.Document, stream);
            stream.Close();
            stream.Dispose();

            // 3. Copmress Both files
            using (ZipFile zip = new ZipFile())
            {
                // add these files into the zip archive
                zip.AddFile(tempFile + "ink");
                zip.AddFile(tempFile + "doc");
                zip.Save(zipFilePath);
            }
        }

        /// <summary>
        /// Extract Lesson file (whose name is the sent string) 
        /// and load its data (text data and inkNotes data) to the lesson elements/GUI.
        /// </summary>
        private void ExtractLesson(string zipFilePath)
        {
            // 1. Extract/Decompress the file and get the path of the ink file and document file            
            string fileName = "";
            string tempDirectory = "Temp";
            Directory.CreateDirectory(tempDirectory);

            using (ZipFile zip = ZipFile.Read(zipFilePath))
            {
                zip.ExtractAll(tempDirectory);

                // 2.Load inkCollection from the temp file to inkCanvas of the Lesson.                
                Stream stream = File.Open(tempDirectory + "\\" + zip[0].FileName, FileMode.Open);
                inkCanvasLesson.Strokes = new StrokeCollection(stream);
                stream.Close();
                stream.Dispose();

                // 3.Load Document Data from the temp file to the richTextBox of the Lesson.
                stream = File.Open(tempDirectory + "\\" + zip[1].FileName, FileMode.Open);
                textBoxLesson.Document = (FlowDocument)System.Windows.Markup.XamlReader.Load(stream);
                stream.Close();
                stream.Dispose();
            }

            Directory.Delete(tempDirectory, true);
            //foreach (ZipEntry entry in zip)
            //{                    
            //    entry.Extract(tempDirectory, ExtractExistingFileAction.OverwriteSilently);
            //}
        }
        #endregion

        #region Manage Popup (Show/Hide with animation, Ok button, Select Color)

        /// <summary>
        /// Event handler for build button click. Open popup of Build Lesson
        /// </summary>        
        private void buttonBuild_Click(object sender, RoutedEventArgs e)
        {
            BuildLessonPopup();
        }

        /// <summary>
        /// Event handler of Ok button of popup. Apply changes in the richTextBox of the popup
        /// to the richTextBox of the Lesson. i.e copy all text form Tool to Lesson.
        /// </summary>       
        private void buttonPopupOk_Click(object sender, RoutedEventArgs e)
        {
            // copy document content from lesson textBox to buildLesson textBox
            using (MemoryStream stream = new MemoryStream())
            {
                TextRange toolRange = new TextRange(textBoxTool.Document.ContentStart, textBoxTool.Document.ContentEnd);
                TextRange lessonRange = new TextRange(textBoxLesson.Document.ContentStart, textBoxLesson.Document.ContentEnd);
                toolRange.Save(stream, DataFormats.Rtf);
                lessonRange.Load(stream, DataFormats.Rtf);
            }
            popupLessonTool.IsOpen = false;
        }

        /// <summary>
        /// Change color if the selected text in the richTextBox of the popup
        /// into the selected color
        /// </summary>        
        private void radColorPickerPopup_SelectedColorChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Opens a popup with a textbox that enables the teacher to edit the text content
        /// of the current Lesson or add new text content to make new Lesson. Also send the document
        /// content of the lesson textBox to the build lesson textBox
        /// </summary>
        private void BuildLessonPopup()
        {
            // open popup            
            popupLessonTool.Height = 570;
            popupLessonTool.Width = 800;

            popupLessonTool.IsOpen = true;
            AnimateWindowOpacity(1, 0.7);
            AnimatePopupOpacity(0, 1, popupLessonTool);

            popupLessonTool.Focus();
            textBoxTool.Focus();
            Keyboard.Focus(textBoxTool);

            // copy document content from build lesson textBox to lesson textBox
            using (MemoryStream stream = new MemoryStream())
            {
                TextRange toolRange = new TextRange(textBoxTool.Document.ContentStart, textBoxTool.Document.ContentEnd);
                TextRange lessonRange = new TextRange(textBoxLesson.Document.ContentStart, textBoxLesson.Document.ContentEnd);
                lessonRange.Save(stream, DataFormats.Rtf);
                toolRange.Load(stream, DataFormats.Rtf);
            }
        }

        /// <summary>
        /// Fade-in opacity of the application window when popup is closed.
        /// </summary>        
        private void popupLessonTool_Closed(object sender, EventArgs e)
        {
            AnimateWindowOpacity(0.7, 1);
            this.Focus();
        }

        /// <summary>
        /// Animate the opacity fade-out/in of the application 
        /// when using Lesson Tool popup is shown/hidden respectively.
        /// </summary>
        private void AnimateWindowOpacity(double from, double to)
        {
            DoubleAnimation animation = new DoubleAnimation();
            EasingFunctionBase easing = new QuadraticEase();
            easing.EasingMode = EasingMode.EaseInOut;

            animation.From = from;
            animation.To = to;
            animation.By = 1;
            animation.EasingFunction = easing;
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(800));

            Storyboard storyBoard = new Storyboard();
            Storyboard.SetTargetName(animation, this.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Window.OpacityProperty));
            storyBoard.Children.Add(animation);
            storyBoard.Begin(this);
        }

        /// <summary>
        /// Animate the opacity fade-out/in of the application 
        /// when using Lesson Tool popup is shown/hidden respectively.
        /// </summary>
        private void AnimatePopupOpacity(double from, double to, Popup popup)
        {
            DoubleAnimation animation = new DoubleAnimation();
            EasingFunctionBase easing = new QuadraticEase();
            easing.EasingMode = EasingMode.EaseInOut;

            animation.From = from;
            animation.To = to;
            animation.By = 1;
            animation.EasingFunction = easing;
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(800));

            Storyboard storyBoard = new Storyboard();
            if (popup == popupLessonTool)
                Storyboard.SetTargetName(animation, borderPopup.Name);
            else if (popup == popupFilePanel)
                Storyboard.SetTargetName(animation, webfilePanel.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Border.OpacityProperty));
            storyBoard.Children.Add(animation);
            storyBoard.Begin(this);
        }
        #endregion

        #region Manage open and close of filePanel poupup
        /// <summary>
        /// Open lesson files form the internet (pen-app.com)
        /// </summary>        
        private void buttonWeb_Click(object sender, RoutedEventArgs e)
        {
            OpenPopupFilePanel();
        }

        /// <summary>
        /// Create and open Popup of webfilePanel to download lesson files form the internet.
        /// </summary>
        private void OpenPopupFilePanel()
        {
            //create poup up, set its properties and open it
            //Popup popup = new Popup();
            //Tools.WebfilePanel filepanel = new Tools.WebfilePanel();
            //popupFilePanel.Child = webfilePanel;

            Canvas.SetLeft(popupFilePanel, 390);
            Canvas.SetTop(popupFilePanel, 215);
            popupFilePanel.Width = webfilePanel.Width;
            popupFilePanel.Height = webfilePanel.Height;            

            popupFilePanel.IsOpen = true;
            AnimateWindowOpacity(1, 0.7);
            AnimatePopupOpacity(0, 1, popupFilePanel);            
        }

        /// <summary>
        /// Fade-in opacity of the application window when popup of filePanel is closed.
        /// </summary>        
        private void popupFilePanel_Closed(object sender, EventArgs e)
        {
            AnimateWindowOpacity(0.7, 1);
            this.Focus();
        }
        #endregion

        #region Implement Scrolling of Lesson Content
        // Scrolling by two approaches 1.Smooth Touch Scrolling 2.Ordinary Mouse Scrolling
        // initialize event handlers required to perform smooth scrolling
        
        /// <summary>
        /// Initialize event handlers required to perform smooth scrolling.
        /// </summary>
        private void InitializeScrollingEvents()
        {
            scrollViewerLesson.ManipulationStarting += new EventHandler<ManipulationStartingEventArgs>(scrollViewerLesson_ManipulationStarting);
            scrollViewerLesson.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(scrollViewerLesson_ManipulationDelta);
            scrollViewerLesson.ManipulationInertiaStarting += new EventHandler<ManipulationInertiaStartingEventArgs>(scrollViewerLesson_ManipulationInertiaStarting);

            // manipulation enabled is set to false. It is set to true only when  the user
            // click the free scrolling mode.
            scrollViewerLesson.IsManipulationEnabled = false;
        }

        /// <summary>
        /// Start of manipulation (equivilant to touchDown + first touchMove events).
        /// </summary>        
        void scrollViewerLesson_ManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            e.ManipulationContainer = canvasMain;
            e.Handled = true;
            e.Mode = ManipulationModes.TranslateY;
        }

        /// <summary>
        /// When there is movement updates (equivilant to touchMove).
        /// </summary>        
        void scrollViewerLesson_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            scrollViewerLesson.ScrollToVerticalOffset(scrollViewerLesson.VerticalOffset - e.DeltaManipulation.Translation.Y);
        }

        /// <summary>
        /// When the touch is released from the control (equivilant to TouchUp or TouchLeave).
        /// </summary>        
        void scrollViewerLesson_ManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            // adjust the dispalcement behaviour
            // (10 inches * 96 DIPS per inch / 1000ms^2)
            e.TranslationBehavior.DesiredDeceleration = 2.0 * 96.0 / (1000.0 * 1000.0);
            e.TranslationBehavior.InitialVelocity = e.InitialVelocities.LinearVelocity;
            e.TranslationBehavior.DesiredDisplacement = Math.Abs(e.InitialVelocities.LinearVelocity.Y) * 300;
        }
        #endregion

        /// <summary>
        /// Open and close facebook panel to share a post on facebook wall.
        /// </summary>
        private void FacebookShare()
        {
            AnimateWindowOpacity(1, 0.7);
            Pen.Tools.FbWindow fbWindow = new Pen.Tools.FbWindow();
            fbWindow.ShowDialog();            
            AnimateWindowOpacity(0.7, 1);
        }               
        
        /// <summary>
        /// event handler for buttons responsible of lesson files (New, Open, Save, Build and Share)
        /// </summary>        
        private void buttonFile_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            // new lesson
            if (button == buttonNew)
                NewLesson();

            // open lesson file
            else if (button == buttonOpen)
                OpenLessonFile();

            // save lesson file
            else if (button == buttonSave)
                SaveLessonFile();

                // save lesson file
            else if (button == buttonShare)
                FacebookShare();

            else
                MessageBox.Show("There must be an error in event buttonFile_Click() method");
        }

        /// <summary>
        /// Event handler for buttons doing manipulations on text (check spelling, speak, translate).        
        /// </summary>        
        private void buttonTool_Click(object sender, RoutedEventArgs e)
        {
            // check if textboxLesson is empty, get out of here
            // because nothing to do with empty text
            string text = new TextRange(textBoxLesson.Document.ContentStart, textBoxLesson.Document.ContentEnd).Text;
            if (string.IsNullOrWhiteSpace(text))
                return;

            // if there is selected text in the textBox, then choose this text to do
            // manipulations on it (translate, check spelling, speak)
            if (textBoxLesson.Selection.Text.Length > 0)
                text = textBoxLesson.Selection.Text;

            string buttonId = ((ButtonBase)sender).Name.Substring(6, 1);
            switch (buttonId)
            {
                // Translation
                case "1":
                    GoogleTranslator(text, textBox2, Google.API.Translate.Language.English, Google.API.Translate.Language.Arabic);
                    //BingTranslator(textBoxLesson.Text, textBox2, Languages.English, Languages.Arabic);
                    break;

                // 1. Spelling check and correct using MS Word 2. WPF spelling checker
                case "2":
                    textBoxLesson.SpellCheck.IsEnabled = true;
                    //textBoxLesson.Text = WordSpellingChecker(textBoxLesson.Text);
                    //textBoxLesson.SpellCheck.CustomDictionaries.Add(new Uri(Environment.CurrentDirectory + "\\Dictionary.txt"));                    
                    break;

                // Text to Speech
                case "3":
                    SpeechSynthesize(text);
                    break;

                default:
                    MessageBox.Show("There must be an error in switch case in buttonTool_Click() method");
                    break;
            }
        }

        // experiments button
        private void button_Click(object sender, RoutedEventArgs e)
        {
            // # change application input (keyboard) language #
            //string ietf = System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag;
            //string lcid = System.Globalization.CultureInfo.CurrentCulture.LCID.ToString();
            //InputLanguageManager.SetInputLanguage(textBox2, System.Globalization.CultureInfo.GetCultureInfo(1025));
            //MessageBox.Show("LCID:" + lcid + ", IETF: " + ietf);

            // # handwriting recognition with different languages #
            // french  france     1036
            // english uk         2057
            // english uk         1033
            // germany germany    1031
            //inkAnalyzer.AddStrokes(inkCanvasLesson.Strokes);
            //inkAnalyzer.SetStrokesLanguageId(inkCanvasLesson.Strokes, 1031);
            //inkAnalyzer.BackgroundAnalyze();

            // inkCanvasLesson background: #102CBE4B
            // border and separator color: #FF207645 || new color (just changed the transparency level) #6F207645

            // # trials for setting positions of background animation #
            //RotateTransform rotateTransform = new RotateTransform();
            //imageBackground.RenderTransform = rotateTransform;
            //imageBackground.RenderTransformOrigin = new Point(0.5, 0.5);
            //rotateTransform.Angle = numericUpDown1.Value;            

            AnimateBackground();
        }        

        #region show notifyIcon
        //private void ShowNotifyIcon()
        //{
        //    Hardcodet.Wpf.TaskbarNotification.TaskbarIcon taskParIcon = new Hardcodet.Wpf.TaskbarNotification.TaskbarIcon();
        //    taskParIcon.ShowBalloonTip(Title, "Message", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Warning);
        //}

        //protected override void OnStateChanged(EventArgs e)
        //{
        //    if (WindowState == WindowState.Minimized)
        //        this.Hide();

        //    base.OnStateChanged(e);
        //}
        #endregion
    }
}

#if null
private void TrashCode()
{
            // # textbox EditingCommand #
            //MenuItem item = new MenuItem();
            //item.Header = "How are you today !";
            //item.Click += new RoutedEventHandler(item_Click);
            //item.Command = EditingCommands.IgnoreSpellingError;
            //item.CommandParameter = textBoxLesson.Text;
            //item.CommandTarget = textBoxLesson;

            // # set image source #
            //image1.Source = new BitmapImage(new Uri("/Pen.Language;component/Images/flare.png", UriKind.Relative));

            //GetInstalledVoices();
            //speechSynth.SpeakAsync("Where are you from ? I guess you are from Egypt, aren't you ? Huhh!!!");

// # Taskbar icon #
//Microsoft.Expression.Prototyping.Annotation.AuthorInfoDialog autorDialog = new Microsoft.Expression.Prototyping.Annotation.AuthorInfoDialog();
            TaskbarItemInfo = new System.Windows.Shell.TaskbarItemInfo();
            TaskbarItemInfo.Description = "This is a description Title Trial";
            TaskbarItemInfo.ThumbnailClipMargin = new Thickness(500);    

}

{

            // another method
            // The prevoius method is too slow but universal. 
            // I have a similar task. I need to load a RTF-file and display it 
            // in FlowDocumentScrollViewer. I use such a code and it faster.

            //// Load a RTF-file into RichTextBox.
            //RichTextBox lRichTextBox = new RichTextBox();
            //lRichTextBox.Selection.Load(aDocumentStream, DataFormats.Rtf);

            //// Save FlowDocument in a temporary variable.
            //FlowDocument lDocument = lRichTextBox.Document;
            //// Unbind FlowDocument from RichTextBox. You can't assign null because it throws an exception.
            //lRichTextBox.Document = new FlowDocument();

            //FlowDocumentScrollViewer lFlowDocumentViewer = new FlowDocumentScrollViewer();
            //lFlowDocumentViewer.Document = lDocument;
}
#endif