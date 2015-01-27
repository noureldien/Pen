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
using System.Windows.Media.Animation;

namespace Pen.Intro
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            image1.Opacity = 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FadeInAnimation();
        }

        void FadeInAnimation()
        {
            DoubleAnimation animation = new DoubleAnimation();
            EasingFunctionBase easing = new QuinticEase();
            easing.EasingMode = EasingMode.EaseIn;

            animation.AutoReverse = false;
            animation.From = 0;
            animation.To = 1;
            animation.By = 1;
            animation.EasingFunction = easing;
            animation.AccelerationRatio = 0;
            animation.DecelerationRatio = 1;
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(300));
            animation.Completed += new EventHandler((object obj, EventArgs ev) => RotateAnimation());

            Storyboard storyBoard = new Storyboard();
            Storyboard.SetTargetName(image1, image1.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Image.OpacityProperty));
            storyBoard.Children.Add(animation);
            storyBoard.Begin(image1);
        }

        void RotateAnimation()
        {
            DoubleAnimation animation = new DoubleAnimation();
            EasingFunctionBase easing = new BackEase();
            // EasingFunctionBase
            easing.EasingMode = EasingMode.EaseOut;

            animation.AutoReverse = false;
            animation.From = 0;
            animation.To = 720;
            animation.By = 1;
            animation.EasingFunction = easing;
            animation.AccelerationRatio = 1;
            animation.DecelerationRatio = 0;
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(2000));
            animation.Completed += new EventHandler((object obj, EventArgs ev) =>
            {
                System.Threading.Thread.Sleep(400);
                FadeOutAnimation();
            });

            RotateTransform rotateTransform = new RotateTransform();
            image1.RenderTransform = rotateTransform;
            image1.RenderTransformOrigin = new Point(0.5, 0.5);
            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, animation);

            //Storyboard storyBoard = new Storyboard();
            //Storyboard.SetTargetName(image1, image1.Name);
            //Storyboard.SetTargetProperty(animation, new PropertyPath(RotateTransform.AngleProperty));
            //storyBoard.Children.Add(animation);
            //storyBoard.Begin(this);
        }
     
        void FadeOutAnimation()
        {
            DoubleAnimation animation = new DoubleAnimation();
            EasingFunctionBase easing = new QuinticEase();
            easing.EasingMode = EasingMode.EaseInOut;

            animation.AutoReverse = false;
            animation.From = 1;
            animation.To = 0;
            animation.By = 1;
            animation.EasingFunction = easing;
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(2000));
            animation.Completed += new EventHandler((object obj, EventArgs ev) => this.Close());

            Storyboard storyBoard = new Storyboard();
            Storyboard.SetTargetName(image1, image1.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Image.OpacityProperty));
            storyBoard.Children.Add(animation);
            storyBoard.Begin(image1);
        }

        void GrowAnimation()
        {
            DoubleAnimation heightAnimation = new DoubleAnimation();
            DoubleAnimation widthAnimation = new DoubleAnimation();
            EasingFunctionBase easing = new QuinticEase();
            easing.EasingMode = EasingMode.EaseOut;

            heightAnimation.AutoReverse = false;
            heightAnimation.From = 570;
            heightAnimation.To = 200;
            heightAnimation.By = 1;
            heightAnimation.EasingFunction = easing;
            heightAnimation.AccelerationRatio = 1;
            heightAnimation.DecelerationRatio = 0;
            heightAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(1000));

            Storyboard storyBoard = new Storyboard();
            Storyboard.SetTargetName(image1, image1.Name);
            Storyboard.SetTargetProperty(heightAnimation, new PropertyPath(Image.WidthProperty));
            storyBoard.Children.Add(heightAnimation);
            storyBoard.Begin(image1);
        }
    }
}
