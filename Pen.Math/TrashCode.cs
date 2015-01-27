using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pen
{
    class TrashCode
    {
        #region writing manually using inkCanvas1 and button4
        //private StylusPointCollection pointCollection1;
        //private System.Windows.Ink.Stroke stroke1;
        //private bool strokeSessionEnded1 = true;

        //private void button4_TouchDown(object sender, TouchEventArgs e)
        //{
        //    strokeSessionEnded1 = false;
        //    Point i = e.GetTouchPoint(inkCanvas1).Position;
        //    pointCollection1 = new StylusPointCollection();
        //    pointCollection1.Add(new StylusPoint(i.X, i.Y));

        //    stroke1 = new System.Windows.Ink.Stroke(pointCollection1);
        //    stroke1.DrawingAttributes.Width = stroke1.DrawingAttributes.Height = 1;
        //    stroke1.DrawingAttributes.Color = Color.FromArgb(255, 255, 100, 0);

        //    inkCanvas1.Strokes.Add(stroke1);
        //}

        //private void button4_TouchMove(object sender, TouchEventArgs e)
        //{
        //    AddStylusPoint1((e.GetTouchPoint(inkCanvas1).Position));
        //}

        //private void button4_TouchUp_TouchLeave(object sender, TouchEventArgs e)
        //{
        //    strokeSessionEnded1 = true;
        //    stroke1 = null;
        //    button4.ReleaseTouchCapture(e.TouchDevice);
        //}

        //private void AddStylusPoint1(Point position)
        //{
        //    if (!strokeSessionEnded1)
        //        stroke1.StylusPoints.Add(new StylusPoint(position.X, position.Y));
        //}
        #endregion

        #region writing manually using inkCanvas2 and button5
        //private StylusPointCollection pointCollection2;
        //private System.Windows.Ink.Stroke stroke2;
        //private bool strokeSessionEnded2 = true;

        //private void button5_TouchDown(object sender, TouchEventArgs e)
        //{
        //    strokeSessionEnded2 = false;
        //    Point i = e.GetTouchPoint(inkCanvas2).Position;
        //    pointCollection2 = new StylusPointCollection();
        //    pointCollection2.Add(new StylusPoint(i.X, i.Y));

        //    stroke2 = new System.Windows.Ink.Stroke(pointCollection2);
        //    stroke2.DrawingAttributes.Width = stroke2.DrawingAttributes.Height = 1;
        //    stroke2.DrawingAttributes.Color = Color.FromArgb(255, 255, 100, 0);

        //    inkCanvas2.Strokes.Add(stroke2);
        //}

        //private void button5_TouchMove(object sender, TouchEventArgs e)
        //{
        //    AddStylusPoint2((e.GetTouchPoint(inkCanvas2).Position));
        //}

        //private void button5_TouchUp_TouchLeave(object sender, TouchEventArgs e)
        //{
        //    strokeSessionEnded2 = true;
        //    stroke2 = null;
        //    button5.ReleaseTouchCapture(e.TouchDevice);
        //}

        //private void AddStylusPoint2(Point position)
        //{
        //    if (!strokeSessionEnded2)
        //        stroke2.StylusPoints.Add(new StylusPoint(position.X, position.Y));
        //}
        #endregion

        #region drawing using scratch pad idea
        //private readonly Dictionary<int, Stroke_> _activeStrokes = new Dictionary<int, Stroke_>();
        //private readonly TouchColor _touchColor = new TouchColor();

        //private void Canvas_TouchDown(object sender, TouchEventArgs e)
        //{
        //    Stroke_ stroke;

        //    if (_activeStrokes.TryGetValue(e.TouchDevice.Id, out stroke))
        //    {
        //        FinishStroke(stroke);
        //        return;
        //    }

        //    // Create new stroke, add point and assign a color to it.
        //    Stroke_ newStroke = new Stroke_();
        //    newStroke.Color = _touchColor.GetColor();
        //    newStroke.Id = e.TouchDevice.Id;

        //    // Add new stroke to the collection of strokes in drawing.
        //    _activeStrokes[newStroke.Id] = newStroke;
        //}

        //private void Canvas_TouchMove(object sender, TouchEventArgs e)
        //{
        //    Canvas drawingCanvas = sender as Canvas;
        //    //if (drawingCanvas != null)
        //    //{ 
        //    Stroke_ stroke;
        //    if (_activeStrokes.TryGetValue(e.TouchDevice.Id, out stroke))
        //    {
        //        // Add contact point to the stroke                
        //        stroke.Add(e.GetTouchPoint(drawingCanvas).Position);
        //        stroke.AddToCanvas(drawingCanvas);
        //    }
        //    //}
        //}

        //private void Canvas_TouchUp(object sender, TouchEventArgs e)
        //{
        //    // Find the stroke in the collection of the strokes in drawing.
        //    Stroke_ stroke;
        //    if (_activeStrokes.TryGetValue(e.TouchDevice.Id, out stroke))
        //    {
        //        FinishStroke(stroke);
        //    }

        //}

        //private void FinishStroke(Stroke_ stroke)
        //{
        //    //Seal stroke for better performance
        //    stroke.Freeze();

        //    // Remove finished stroke from the collection of strokes in drawing.
        //    _activeStrokes.Remove(stroke.Id);
        //}
        #endregion

        #region inkPresenter old code
        //private Stroke newStroke;

        //private void inkP_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    inkP.CaptureMouse();
        //    newStroke = new Stroke(e.StylusDevice.GetStylusPoints(inkP));            
        //    inkP.Strokes.Add(newStroke);
        //}

        //private void inkP_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (newStroke != null)
        //    {
        //        newStroke.StylusPoints.Add(e.StylusDevice.GetStylusPoints(inkP));
        //    }
        //}

        //private void inkP_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    newStroke = null;
        //    inkP.ReleaseMouseCapture();
        //}
        #endregion

        #region Custom Scroll banel
        //<Style x:Key="{x:Type ScrollViewer}" 
        //   TargetType="{x:Type ScrollViewer}">
        //    <Setter Property="HorizontalScrollBarVisibility" Value="Hidden"/>
        //    <Setter Property="VerticalScrollBarVisibility" Value="Hidden"/>
        //    <Setter Property="Template">
        //        <Setter.Value>
        //            <ControlTemplate TargetType="{x:Type ScrollViewer}">
        //                <Grid>
        //                    <Grid Background="{TemplateBinding Background}">
        //                        <ScrollContentPresenter 
        //                            x:Name="PART_ScrollContentPresenter"
        //                            Margin="{TemplateBinding Padding}"
        //                            Content="{TemplateBinding Content}"
        //                            ContentTemplate="{TemplateBinding ContentTemplate}"
        //                            CanContentScroll="{TemplateBinding CanContentScroll}"/>
        //                    </Grid>
        //                    <Grid Background="Transparent">
        //                        <Grid.ColumnDefinitions>
        //                            <ColumnDefinition Width="*"/>
        //                            <ColumnDefinition Width="Auto"/>
        //                        </Grid.ColumnDefinitions>
        //                        <Grid.RowDefinitions>
        //                            <RowDefinition Height="*"/>
        //                            <RowDefinition Height="Auto"/>
        //                        </Grid.RowDefinitions>
        //                        <ScrollBar 
        //                            x:Name="PART_VerticalScrollBar"
        //                            Grid.Column="1"
        //                            Minimum="0.0"
        //                            Maximum="{TemplateBinding ScrollableHeight}"
        //                            ViewportSize="{TemplateBinding ViewportHeight}"
        //                            Value="{Binding RelativeSource={RelativeSource TemplatedParent}, Path=VerticalOffset, Mode=OneWay}"
        //                            Visibility="{TemplateBinding ComputedVerticalScrollBarVisibility}"         
        //                            Cursor="Arrow"
        //                            AutomationProperties.AutomationId="VerticalScrollBar"/>
        //                        <ScrollBar 
        //                            x:Name="PART_HorizontalScrollBar"
        //                            Orientation="Horizontal"
        //                            Grid.Row="1"
        //                            Minimum="0.0"
        //                            Maximum="{TemplateBinding ScrollableWidth}"
        //                            ViewportSize="{TemplateBinding ViewportWidth}"
        //                            Value="{Binding RelativeSource={RelativeSource TemplatedParent}, Path=HorizontalOffset, Mode=OneWay}"
        //                            Visibility="{TemplateBinding ComputedHorizontalScrollBarVisibility}"
        //                            Cursor="Arrow"
        //                            AutomationProperties.AutomationId="HorizontalScrollBar"/>
        //                    </Grid>
        //                </Grid>
        //            </ControlTemplate>
        //        </Setter.Value>
        //    </Setter>
        //    <Style.Triggers>
        //        <Trigger Property="IsEnabled" 
        //             Value="false">
        //            <Setter Property="Foreground" 
        //                Value="{DynamicResource {x:Static SystemColors.GrayTextBrushKey}}"/>
        //        </Trigger>
        //        <Trigger Property="IsMouseOver"
        //             Value="true">
        //            <Setter Property="HorizontalScrollBarVisibility"
        //                Value="Visible"/>
        //            <Setter Property="VerticalScrollBarVisibility"
        //                Value="Visible"/>
        //        </Trigger>
        //    </Style.Triggers>
        //</Style>
        //<Style x:Key="ScrollBarButton"
        //   TargetType="{x:Type RepeatButton}">
        //    <Setter Property="OverridesDefaultStyle"
        //        Value="true"/>
        //    <Setter Property="Focusable"
        //        Value="false"/>
        //    <Setter Property="IsTabStop"
        //        Value="false"/>
        //    <Setter Property="Template">
        //        <Setter.Value>
        //            <ControlTemplate TargetType="{x:Type RepeatButton}">
        //                <Path x:Name="Arrow" HorizontalAlignment="Center" VerticalAlignment="Center" Fill="{TemplateBinding Background}"/>
        //                <ControlTemplate.Triggers>
        //                    <Trigger Property="theme:ScrollChrome.ScrollGlyph" Value="UpArrow">
        //                        <Setter TargetName="Arrow" Property="Data" Value="M 3,0 l 3,8 l -6,0 Z"/>
        //                    </Trigger>
        //                    <Trigger Property="theme:ScrollChrome.ScrollGlyph" Value="DownArrow">
        //                        <Setter TargetName="Arrow" Property="Data" Value="M 0,0 l 6,0 l -3,8 Z"/>
        //                    </Trigger>
        //                    <Trigger Property="theme:ScrollChrome.ScrollGlyph" Value="LeftArrow">
        //                        <Setter TargetName="Arrow" Property="Data" Value="M 0,3 l 8,-3 l 0,6 Z"/>
        //                    </Trigger>
        //                    <Trigger Property="theme:ScrollChrome.ScrollGlyph" Value="RightArrow">
        //                        <Setter TargetName="Arrow" Property="Data" Value="M 0,0 l 8,3 l -8,3 Z"/>
        //                    </Trigger>
        //                </ControlTemplate.Triggers>
        //            </ControlTemplate>
        //        </Setter.Value>
        //    </Setter>
        //</Style>
        //<Style x:Key="ScrollBarThumb"
        //   TargetType="{x:Type Thumb}">
        //    <Setter Property="OverridesDefaultStyle"
        //        Value="true"/>
        //    <Setter Property="IsTabStop"
        //        Value="false"/>
        //    <Setter Property="Template">
        //        <Setter.Value>
        //            <ControlTemplate TargetType="{x:Type Thumb}">
        //                <Border Background="#FF777777" CornerRadius="2"/>
        //            </ControlTemplate>
        //        </Setter.Value>
        //    </Setter>
        //</Style>
        //<Style x:Key="HorizontalScrollBarPageButton"
        //   TargetType="{x:Type RepeatButton}">
        //    <Setter Property="OverridesDefaultStyle"
        //        Value="true"/>
        //    <Setter Property="Background"
        //        Value="Transparent"/>
        //    <Setter Property="Focusable"
        //        Value="false"/>
        //    <Setter Property="IsTabStop"
        //        Value="false"/>
        //    <Setter Property="Template">
        //        <Setter.Value>
        //            <ControlTemplate TargetType="{x:Type RepeatButton}">
        //                <Rectangle Fill="{TemplateBinding Background}"
        //                       Width="{TemplateBinding Width}"
        //                       Height="{TemplateBinding Height}"/>
        //            </ControlTemplate>
        //        </Setter.Value>
        //    </Setter>
        //</Style>
        //<Style x:Key="VerticalScrollBarPageButton"
        //   TargetType="{x:Type RepeatButton}">
        //    <Setter Property="OverridesDefaultStyle"
        //        Value="true"/>
        //    <Setter Property="Background"
        //        Value="Transparent"/>
        //    <Setter Property="Focusable"
        //        Value="false"/>
        //    <Setter Property="IsTabStop"
        //        Value="false"/>
        //    <Setter Property="Template">
        //        <Setter.Value>
        //            <ControlTemplate TargetType="{x:Type RepeatButton}">
        //                <Rectangle Fill="{TemplateBinding Background}"
        //                       Width="{TemplateBinding Width}"
        //                       Height="{TemplateBinding Height}"/>
        //            </ControlTemplate>
        //        </Setter.Value>
        //    </Setter>
        //</Style>
        //<Style x:Key="{x:Type ScrollBar}"
        //   TargetType="{x:Type ScrollBar}">
        //    <Setter Property="Background"
        //        Value="#FF979797"/>
        //    <Setter Property="Stylus.IsPressAndHoldEnabled"
        //        Value="false"/>
        //    <Setter Property="Stylus.IsFlicksEnabled"
        //        Value="false"/>
        //    <Setter Property="Foreground"
        //        Value="{DynamicResource {x:Static SystemColors.ControlTextBrushKey}}"/>
        //    <Setter Property="Width"
        //        Value="{DynamicResource {x:Static SystemParameters.VerticalScrollBarWidthKey}}"/>
        //    <Setter Property="MinWidth"
        //        Value="{DynamicResource {x:Static SystemParameters.VerticalScrollBarWidthKey}}"/>
        //    <Setter Property="Margin"
        //        Value="2"/>
        //    <Setter Property="Template">
        //        <Setter.Value> <!-- This is the control tepmlate of the grid, when you hover on it, the scroll bar appears -->
        //            <ControlTemplate TargetType="{x:Type ScrollBar}">
        //                <Border 
        //                    x:Name="Bg"
        //                    CornerRadius="4"                            
        //                    Margin="2"
        //                    Opacity="0.4"
        //                    Background="{TemplateBinding Background}"
        //                    VerticalAlignment="Bottom">
        //                    <Grid
        //                        SnapsToDevicePixels="true">
        //                        <Grid.RowDefinitions>
        //                            <RowDefinition MaxHeight="{DynamicResource {x:Static SystemParameters.VerticalScrollBarButtonHeightKey}}"/>
        //                            <RowDefinition MaxHeight="{DynamicResource {x:Static SystemParameters.VerticalScrollBarButtonHeightKey}}"/>
        //                        </Grid.RowDefinitions>
        //                        <RepeatButton
        //                            Style="{StaticResource ScrollBarButton}"
        //                            Background="White"                                    
        //                            IsEnabled="{TemplateBinding IsMouseOver}"
        //                            Command="{x:Static ScrollBar.LineUpCommand}"
        //                            theme:ScrollChrome.ScrollGlyph="UpArrow"                                    
        //                            RenderTransformOrigin="0.5, 0.5"
        //                            Margin="0,4">
        //                            <RepeatButton.RenderTransform>
        //                                <ScaleTransform ScaleX="1.5" ScaleY="1.5"/>
        //                            </RepeatButton.RenderTransform>
        //                        </RepeatButton>
        //                        <RepeatButton                   
        //                            Style="{StaticResource ScrollBarButton}"
        //                            Background="White"
        //                            Grid.Row="1"
        //                            IsEnabled="{TemplateBinding IsMouseOver}"
        //                            Command="{x:Static ScrollBar.LineDownCommand}"
        //                            theme:ScrollChrome.ScrollGlyph="DownArrow"
        //                            RenderTransformOrigin="0.5, 0.5"
        //                            Margin="0,4">
        //                            <RepeatButton.RenderTransform>
        //                                <ScaleTransform ScaleX="1.5" ScaleY="1.5"/>
        //                            </RepeatButton.RenderTransform>                                
        //                        </RepeatButton>
        //                    </Grid>
        //                </Border>
        //                <ControlTemplate.Triggers>
        //                    <Trigger Property="IsEnabled"
        //                         Value="false">
        //                        <Setter TargetName="Bg"
        //                            Property="Visibility"
        //                            Value="Hidden"/>
        //                    </Trigger>
        //                </ControlTemplate.Triggers>
        //            </ControlTemplate>
        //        </Setter.Value>
        //    </Setter>
        //    <Style.Triggers>
        //        <MultiTrigger>
        //            <MultiTrigger.Conditions>
        //                <Condition Property="IsMouseOver" Value="True"/>
        //                <Condition Property="Orientation" Value="Vertical"/>
        //            </MultiTrigger.Conditions>
        //            <MultiTrigger.Setters>
        //                <Setter Property="Width"
        //                    Value="20"/>
        //                <Setter
        //                    Property="Template">
        //                    <Setter.Value>
        //                        <ControlTemplate TargetType="{x:Type ScrollBar}">
        //                            <Border 
        //                                CornerRadius="4"                         
        //                                Margin="2"
        //                                Opacity="0.75"
        //                                Background="{TemplateBinding Background}">
        //                                <Grid
        //                                    SnapsToDevicePixels="true">
        //                                    <Grid.RowDefinitions>
        //                                        <RowDefinition Height="0.00001*"/>
        //                                        <RowDefinition MaxHeight="{DynamicResource {x:Static SystemParameters.VerticalScrollBarButtonHeightKey}}"/>
        //                                        <RowDefinition MaxHeight="{DynamicResource {x:Static SystemParameters.VerticalScrollBarButtonHeightKey}}"/>
        //                                    </Grid.RowDefinitions>
        //                                    <Track
        //                                        Name="PART_Track"
        //                                        IsEnabled="{TemplateBinding IsMouseOver}"
        //                                        IsDirectionReversed="true">
        //                                        <Track.DecreaseRepeatButton>
        //                                            <RepeatButton Style="{StaticResource VerticalScrollBarPageButton}"
        //                                                Command="{x:Static ScrollBar.PageUpCommand}"/>
        //                                        </Track.DecreaseRepeatButton>
        //                                        <Track.IncreaseRepeatButton>
        //                                            <RepeatButton Style="{StaticResource VerticalScrollBarPageButton}"
        //                                                Command="{x:Static ScrollBar.PageDownCommand}"/>
        //                                        </Track.IncreaseRepeatButton>
        //                                        <Track.Thumb>
        //                                            <Thumb Style="{StaticResource ScrollBarThumb}"
        //                                                theme:ScrollChrome.ScrollGlyph="VerticalGripper"
        //                                                Margin="2"/>
        //                                        </Track.Thumb>
        //                                    </Track>
        //                                    <RepeatButton
        //                                        Style="{StaticResource ScrollBarButton}"
        //                                        Background="White"
        //                                        Grid.Row="1"
        //                                        IsEnabled="{TemplateBinding IsMouseOver}"
        //                                        Command="{x:Static ScrollBar.LineUpCommand}"
        //                                        theme:ScrollChrome.ScrollGlyph="UpArrow"
        //                                        RenderTransformOrigin="0.5, 0.5">
        //                                        <RepeatButton.RenderTransform>
        //                                            <ScaleTransform ScaleX="1.5" ScaleY="1.5"/>
        //                                        </RepeatButton.RenderTransform>
        //                                    </RepeatButton>
        //                                    <RepeatButton                           
        //                                        Style="{StaticResource ScrollBarButton}"
        //                                        Background="#FFFFFFFF"
        //                                        Grid.Row="2"
        //                                        IsEnabled="{TemplateBinding IsMouseOver}"
        //                                        Command="{x:Static ScrollBar.LineDownCommand}"
        //                                        theme:ScrollChrome.ScrollGlyph="DownArrow"
        //                                        RenderTransformOrigin="0.5, 0.5">
        //                                        <RepeatButton.RenderTransform>
        //                                            <ScaleTransform ScaleX="1.5" ScaleY="1.5"/>
        //                                        </RepeatButton.RenderTransform>
        //                                    </RepeatButton>
        //                                </Grid>
        //                            </Border>
        //                        </ControlTemplate>
        //                    </Setter.Value>
        //                </Setter>
        //            </MultiTrigger.Setters>
        //        </MultiTrigger>
        //        <MultiTrigger>
        //            <MultiTrigger.Conditions>
        //                <Condition Property="Orientation" Value="Horizontal"/>
        //                <Condition Property="IsMouseOver" Value="False"/>
        //            </MultiTrigger.Conditions>
        //            <MultiTrigger.Setters>
        //                <Setter Property="Width"
        //                    Value="Auto"/>
        //                <Setter Property="MinWidth"
        //                    Value="0"/>
        //                <Setter Property="Height"
        //                    Value="{DynamicResource {x:Static SystemParameters.HorizontalScrollBarHeightKey}}"/>
        //                <Setter Property="MinHeight"
        //                    Value="{DynamicResource {x:Static SystemParameters.HorizontalScrollBarHeightKey}}"/>
        //                <Setter Property="Template">
        //                    <Setter.Value>
        //                        <ControlTemplate TargetType="{x:Type ScrollBar}">
        //                            <Border 
        //                                x:Name="Bg"
        //                                CornerRadius="2"                         
        //                                Margin="2"
        //                                Opacity="0.75"
        //                                Background="{TemplateBinding Background}"
        //                                HorizontalAlignment="Right">
        //                                <Grid
        //                                    SnapsToDevicePixels="true">
        //                                    <Grid.ColumnDefinitions>
        //                                        <ColumnDefinition MaxWidth="{DynamicResource {x:Static SystemParameters.HorizontalScrollBarButtonWidthKey}}"/>
        //                                        <ColumnDefinition MaxWidth="{DynamicResource {x:Static SystemParameters.HorizontalScrollBarButtonWidthKey}}"/>
        //                                    </Grid.ColumnDefinitions>
        //                                    <RepeatButton
        //                                        Style="{StaticResource ScrollBarButton}"    
        //                                        Background="#FFCBCBCB"
        //                                        IsEnabled="{TemplateBinding IsMouseOver}"
        //                                        Command="{x:Static ScrollBar.LineLeftCommand}"
        //                                        theme:ScrollChrome.ScrollGlyph="LeftArrow"
        //                                        Margin="4,0"/>
        //                                    <RepeatButton
        //                                        Style="{StaticResource ScrollBarButton}"                                        
        //                                        Background="#FFCBCBCB"
        //                                        Grid.Column="1"
        //                                        IsEnabled="{TemplateBinding IsMouseOver}"
        //                                        Command="{x:Static ScrollBar.LineRightCommand}"
        //                                        theme:ScrollChrome.ScrollGlyph="RightArrow"
        //                                        Margin="4,0"/>
        //                                </Grid>
        //                            </Border>
        //                            <ControlTemplate.Triggers>
        //                                <Trigger Property="IsEnabled"
        //                                     Value="false">
        //                                    <Setter TargetName="Bg"
        //                                        Property="Visibility"
        //                                        Value="Hidden"/>
        //                                </Trigger>
        //                            </ControlTemplate.Triggers>
        //                        </ControlTemplate>
        //                    </Setter.Value>
        //                </Setter>
        //            </MultiTrigger.Setters>
        //        </MultiTrigger>
        //        <MultiTrigger>
        //            <MultiTrigger.Conditions>
        //                <Condition Property="IsMouseOver" Value="True"/>
        //                <Condition Property="Orientation" Value="Horizontal"/>
        //            </MultiTrigger.Conditions>
        //            <MultiTrigger.Setters>
        //                <Setter Property="Width"
        //                    Value="Auto"/>
        //                <Setter Property="MinWidth"
        //                    Value="0"/>
        //                <Setter Property="Height"
        //                    Value="{DynamicResource {x:Static SystemParameters.HorizontalScrollBarHeightKey}}"/>
        //                <Setter Property="MinHeight"
        //                    Value="{DynamicResource {x:Static SystemParameters.HorizontalScrollBarHeightKey}}"/>
        //                <Setter Property="Height"
        //                    Value="30"/>
        //                <Setter
        //                    Property="Template">
        //                    <Setter.Value>
        //                        <ControlTemplate TargetType="{x:Type ScrollBar}">
        //                            <Border 
        //                                CornerRadius="4"                         
        //                                Margin="2"
        //                                Opacity="0.75"
        //                                Background="{TemplateBinding Background}">
        //                                <Grid
        //                                    SnapsToDevicePixels="true">
        //                                    <Grid.ColumnDefinitions>
        //                                        <ColumnDefinition Width="0.00001*"/>
        //                                        <ColumnDefinition MaxWidth="{DynamicResource {x:Static SystemParameters.HorizontalScrollBarButtonWidthKey}}"/>
        //                                        <ColumnDefinition MaxWidth="{DynamicResource {x:Static SystemParameters.HorizontalScrollBarButtonWidthKey}}"/>
        //                                    </Grid.ColumnDefinitions>
        //                                    <Track
        //                                        Name="PART_Track"
        //                                        IsEnabled="{TemplateBinding IsMouseOver}">
        //                                        <Track.DecreaseRepeatButton>
        //                                            <RepeatButton Style="{StaticResource HorizontalScrollBarPageButton}"
        //                                              Command="{x:Static ScrollBar.PageLeftCommand}"/>
        //                                        </Track.DecreaseRepeatButton>
        //                                        <Track.IncreaseRepeatButton>
        //                                            <RepeatButton Style="{StaticResource HorizontalScrollBarPageButton}"
        //                                              Command="{x:Static ScrollBar.PageRightCommand}"/>
        //                                        </Track.IncreaseRepeatButton>
        //                                        <Track.Thumb>
        //                                            <Thumb x:Name="Thumb" Style="{StaticResource ScrollBarThumb}"
        //                                               theme:ScrollChrome.ScrollGlyph="HorizontalGripper"
        //                                               Margin="2"/>
        //                                        </Track.Thumb>
        //                                    </Track>
        //                                    <RepeatButton
        //                                        Style="{StaticResource ScrollBarButton}"
        //                                        Background="#FFFFFFFF"
        //                                        Grid.Column="1"
        //                                        IsEnabled="{TemplateBinding IsMouseOver}"
        //                                        Command="{x:Static ScrollBar.LineLeftCommand}"
        //                                        theme:ScrollChrome.ScrollGlyph="LeftArrow"
        //                                        RenderTransformOrigin="0.5, 0.5">
        //                                        <RepeatButton.RenderTransform>
        //                                            <ScaleTransform ScaleX="1.5" ScaleY="1.5"/>
        //                                        </RepeatButton.RenderTransform>
        //                                    </RepeatButton>
        //                                    <RepeatButton                           
        //                                        Style="{StaticResource ScrollBarButton}"
        //                                        Background="#FFFFFFFF"
        //                                        Grid.Column="2"
        //                                        IsEnabled="{TemplateBinding IsMouseOver}"
        //                                        Command="{x:Static ScrollBar.LineRightCommand}"
        //                                        theme:ScrollChrome.ScrollGlyph="RightArrow"
        //                                        RenderTransformOrigin="0.5, 0.5">
        //                                        <RepeatButton.RenderTransform>
        //                                            <ScaleTransform ScaleX="1.5" ScaleY="1.5"/>
        //                                        </RepeatButton.RenderTransform>
        //                                    </RepeatButton>
        //                                </Grid>
        //                            </Border>
        //                        </ControlTemplate>
        //                    </Setter.Value>
        //                </Setter>
        //            </MultiTrigger.Setters>
        //        </MultiTrigger>
        //    </Style.Triggers>
        //</Style>

        #endregion

        // Color generator: assigns a color to the new stroke.
        //public class TouchColor
        //{
        //    private int i = 0;  // Rotating secondary color index

        //    // Returns color for the newly started stroke.
        //    public Color GetColor()
        //    {
        //        // Take current secondary color.
        //        Color color = colors[i];
        //        // Move to the next color in the array.
        //        i = (i + 1) % colors.Length;
        //        return color;
        //    }

        //    static private Color[] colors =    // colors
        //    {
        //        Colors.Black,
        //        Colors.Red,
        //        Colors.LawnGreen,
        //        Colors.Blue,
        //        Colors.Cyan,
        //        Colors.Magenta,
        //        Colors.Yellow
        //    };
        //}
    }
}
