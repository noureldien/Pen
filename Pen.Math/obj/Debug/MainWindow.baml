   M S B A M L   `   `   `  ����a  ]PresentationFramework.Aero, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35gJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.AeroMicrosoft.Windows.Themes  5   	   C ?Pen.Math, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:Pen.MathPen.Math 5   	   p lTelerik.Windows.Controls.RibbonBar, Version=2010.3.1110.40, Culture=neutral, PublicKeyToken=5803cfa389c90ce7�gclr-namespace:Telerik.Windows.Controls.RibbonBar.Primitives;assembly=Telerik.Windows.Controls.RibbonBar-Telerik.Windows.Controls.RibbonBar.Primitives 5   	   p lSystem.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35pTclr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Input.ToolkitSystem.Windows.Controls 5	   	   q mSystem.Windows.Controls.Layout.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35qUclr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Layout.ToolkitSystem.Windows.Controls 5
   	   �`clr-namespace:System.Windows.Controls.Primitives;assembly=System.Windows.Controls.Layout.Toolkit"System.Windows.Controls.Primitives 5   	   o kMicrosoft.Expression.Prototyping.Runtime, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35�gclr-namespace:Microsoft.Expression.Framework.Controls;assembly=Microsoft.Expression.Prototyping.Runtime'Microsoft.Expression.Framework.Controls 5   	   | xSystem.Windows.Controls.DataVisualization.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35�rclr-namespace:System.Windows.Controls.DataVisualization;assembly=System.Windows.Controls.DataVisualization.Toolkit)System.Windows.Controls.DataVisualization 5   	      Pen.Math.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W	 SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U
 QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation   	 
 5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml    	 5   	   \themeJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.Aero    	 5   	   e aSystem.Activities.Presentation, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35NsapDhttp://schemas.microsoft.com/netfx/2009/xaml/activities/presentation  5   	   x tTelerik.Windows.Controls.DataVisualization, Version=2010.3.1110.40, Culture=neutral, PublicKeyToken=5803cfa389c90ce7f bTelerik.Windows.Controls, Version=2010.3.1110.40, Culture=neutral, PublicKeyToken=5803cfa389c90ce7l hTelerik.Windows.Controls.Gauge, Version=2010.3.1110.40, Culture=neutral, PublicKeyToken=5803cfa389c90ce7b ^Telerik.Windows.Data, Version=2010.3.1110.40, Culture=neutral, PublicKeyToken=5803cfa389c90ce7l hTelerik.Windows.Controls.Input, Version=2010.3.1110.40, Culture=neutral, PublicKeyToken=5803cfa389c90ce7q mTelerik.Windows.Controls.Navigation, Version=2010.3.1110.40, Culture=neutral, PublicKeyToken=5803cfa389c90ce7Ktelerik1http://schemas.telerik.com/2008/xaml/presentation        5   	   .localclr-namespace:Pen.Math        5   	   |mygclr-namespace:Telerik.Windows.Controls.RibbonBar.Primitives;assembly=Telerik.Windows.Controls.RibbonBar        5   	   jmy1Tclr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Input.Toolkit        5	   	   kmy2Uclr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Layout.Toolkit        5
   	   vmy3`clr-namespace:System.Windows.Controls.Primitives;assembly=System.Windows.Controls.Layout.Toolkit        5   	   }my4gclr-namespace:Microsoft.Expression.Framework.Controls;assembly=Microsoft.Expression.Prototyping.Runtime        5   	   �visualizationToolkitrclr-namespace:System.Windows.Controls.DataVisualization;assembly=System.Windows.Controls.DataVisualization.Toolkit        5   	   -   5        $��
windowMain��5   "     � Title$  Pen Mathematics��5   	   $	��890��5   	   $
��1348��6    � WindowStartupLocation$ CenterScreen=�6#    J� IsManipulationEnabled .   � Icon$* $/Pen.Math;component/Images/logo1.png��5   R    � 	Resources 5         StoryBoardWindowDim-  'Telerik.Windows.Controls.RadColorPicker �� 
TargetType �� BasedOn/  )Telerik.Windows.Controls.ThemeResourceKey   	ThemeType   ElementType  
IconButton	 �� Property
 �� Value  
TextButton  StackPanelButton �� RelativeSource �� Mode �� AncestorType(  "Telerik.Windows.Controls.RadButton �� Path  inkCanvasTempStyle  GridInkCanvas �� EndPoint �� 
StartPoint � Color � Offset  	ScrollBar �� Triggers P� Property P� Value  ScrollViewerStyle �� 
TargetType � ColumnDefinitions � RowDefinitions �� VerticalOffset � Value �� ScrollableHeight � Maximum �� ViewportHeight �� ViewportSize* �� #ComputedVerticalScrollBarVisibility  �� Orientation! �� HorizontalOffset" �� ScrollableWidth# �� ViewportWidth,$ �� %ComputedHorizontalScrollBarVisibility  GridPageStyle3 	 -System.Windows.Media.Effects.DropShadowEffect%   
BlurRadius&   Color'   Opacity(   ShadowDepth 	 BorderPanelStyle��%B  &	        '       0�� 5   4   *  $ telerik:TransparentThemeL�$ telerik:RadColorPickerL�+1&	 ;     &	 �     &	 �    &	 _    &	 {    &	 g    &	 Y    7  7  &	     &		 �	    �� 5   
   5      �� 5   
   "  6    64   2  �� 5   
   " ��6#   .�5      �� 	 � ��$
 32��6'   �� 5      	 � ��$
 32��6&   �� 5      	 � ��$
 5,0,5,0q�6'   �� 5      	 � ��$	
 Top=�62   5      �� 5    
   " ��6#   .�5!      �� 	 � ��$
 18(�6)   �� 5"      	 � ��$
 28��6'   �� 5#      	 � ��$
 Segoe UI)�6+   5$      �� 5%   
   " ��6)   .�5&      �� 	 � ��
 6'   ��  �� $ FindAncestor=�"  $ Height��� 5'      	 � ��$

 Left=�64   �� 5(      	 �@��Orientation$
 
Horizontal=�6,   5)      �� 5+   
   " ��6+   .�5,      �� 	 �@��
Background

 ���� 6+   �� 5-      	 � ��$
 0��6&   �� 5.      	 � ��$
 0��6'   �� 5/      	 �@��Left$
 0��6,   �� 50      	 �@��Top$
 0��6+   �� 51      	 �@��EditingMode$

 None=�6,   52      �� 54   
   " �6&   .�55      �� 	 � ��
 56      �� 57      $ 0.5,1&�6*   $ 0.5,0&�6;   .��58      � $ 	#FFFEFDE1��6'   $ 0Q�69   � 59      $ 	#FFFCF2B1��6'   $ 1Q�69   5:      5<      5=      �� 5?   
   " ��6"    5@      P� 5A       �@J�IsMouseOver . . �5B      �� 	 �@J�Opacity$	
 0.8Q�60   5C      P� 5D       �@J�IsMouseOver .  . �5E      �� 	 �@J�Opacity$	
 0.4Q�60   5F      5H      �� 5J   
   " ��6*   .�5K      �� 	 �@�OverridesDefaultStyle
 . 66   �� 5L      	 � ��
 5M      �� 5N      " ��6&   .R�5O      �  5P      �� 5Q   "   $��* �63   �� 5R   "   $
��Auto �63    5T      �� 5U   "   $��* �60   �� 5V   "   $
��Auto �60   .+�5X      �� $��2��65   $��2��6I   �� 5Y      $��PART_VerticalScrollBar��5[      8��  $��1��5\      # z 5]      # z 5^      # z 5_      #y�z 5`      �� 5a      $��PART_HorizontalScrollBar��5c      8�� $  
Horizontal=�5d      $��1��5e      # z! 5f      # z" 5g      # z# 5h      #y�z$ 5i      5j      5k      5m      5n      �� 5p   
   " �6&   .�5q      �� 	 � ��$	
 400��6&   �� 5r      	 � ��$
 	20,5,20,0q�6'   �� 5s      	 �@J�Effect
 5t        5u      $% 10Q�6'   $
& Gray��67   $	' 0.6Q�6D   $( 0Q�6R   5w      �� 5x      	 � ��
 5y      �� 5z      $ 0.5,1&�6*   $ 0.5,0&�6;   .��5{      � $ 	#FFFEFDE1��6'   $ 0Q�69   � 5|      $ 	#FFFCF2B1��6'   $ 1Q�69   5}      5      5�      �� 5�   
   " ��6)   .�5�      �� 	 � ��
 5�      �� 5�      $ 0.5,1&�6*   $ 0.5,0&�6;   .��5�      � $ Transparent��6'   $ 0Q�6;   � 5�      $ 	#2DFFD600��6'   $ 0.471Q�69   5�      5�      �� 5�      	 � ��
 �	#8EA028006,   �� 5�      	 � ��$	
 100��6'   �� 5�      	 � ��$	
 0.3q�60   �� 5�      	 �@��CornerRadius$
 5��6-   �� 5�      	 �@��Top$	
 705��6+   5�      5�      .��5�      � -   5        $��gridMain��5�      $	��800��6   $
��1280��6(    5�   
   ) �� MergedDictionaries* �� Source�� 5�      ) 5�      �� 5�      $* Themes\ToggleButton.xaml?�6)   �� 5�      $* Themes\Button.xaml?�6)   %    5�      .+�5�   
   �� -   5        $��
canvasMain��5�      .+�5�      �� -   5        $��
textBlock1��5�   G   + �� Left$	+ 608��6   
, �� Top$	, 702��6*   $��85��6;   $��W��6Y   $��60��6b   $��60(�6m   $��Century Gothic)�6{   - J� Opacity$	- 0.4Q�6�   . J� IsHitTestVisible. .  ��5�      �� 5�      $ 0.5,1&�6*   $ 0.5,0&�6;   .��5�      � $ 	#FFFFD242��6'   $ 0Q�69   � 5�      $ 	#FFFF4200��6'   $ 1Q�69   5�      5�   *   �� 5�      -   5        $��inkCanvasCommand��5�   H   $	+ 533��6   $	, 685��6*   $	��115��6;   $	��223��6`   / �� 
Background/ 5�      �� 5�      5�      )  #Pen.Math.ScrollViewerOffsetMediator  5�      -   5        0  Name$0 Mediator��5�   /   �� 5�      -   5        $��stackPanelControlBox��5�      1 �� Orientation$1 
Horizontal=�65   $
+ 1177��6N   $, 0��6a   $	- 0.6Q�6p   .+�5�      �� -   5        $��
buttonHelp��5�   `    
 ButtonStyle#��[
 6   $
+ 1223��6>   $, 0��6Q   $��50��6r   .��5�      �� $)��#/Pen.Math;component/Images/help.png��6   6K   5�      �� 5�      -	   5        $��buttonClose��5�   `   $
+ 1173��6   #��[
 6,   $, 0��6Q   $��50��6s   .��5�      �� $(��"/Pen.Math;component/Images/off.png��6   5�      5�      �� 5�      -
   5        $��stackPanelCaption��5�      $1 
Horizontal=�63   .+�5�      �� -   5        $��	imageLogo��5�   C   $+ 0��6   $, 6��6(   $��40��67   $��75��6T   $*��$/Pen.Math;component/Images/logo1.png��6_   $
��None=�6�   �� 5�      -   5        $��textCaption��5�      $��Pen Math | ��6/   $��Center=�6B   ���	#FF878787$��18(�6t   $��Segoe WP Light)�6�   5�        Pen.Math.InkCanvas_  5�      -   5        2  Name$2 
inkCanvas3��5�   <     Pen.Math.InkCanvas_3   
TouchLeave$"3 inkCanvas_TouchUp_TouchLeave��5�      4   	TouchMove$4 inkCanvas_TouchMove��6I   5   TouchUp$"5 inkCanvas_TouchUp_TouchLeave��5�      6   	TouchDown$6 inkCanvas_TouchDown��6F   7   SelectionChanged$&7  inkCanvasSource_SelectionChanged��5�      8   StrokeCollected$&8  inkCanvasCommand_StrokeCollected��5�      $+ 0��5�      $	��658��6/   $	��834��6P   9 �� EditingMode$
9 None=�6\   
/ �����$, 47��5�   2   �� 5�      -   5        $��canvasStudent1��5�   C   $+ 0��6   $	, 200��6%   $	��600��66   $	��425��6Y   .+�5�      �� -   5        $��
scrollBar1��5�      $	+ 120��6.   $	, 516��6@   $��84��6Q   $	��300��6]   $9 GestureOnly=�6i   6�   �� 5�      -   5        $��scrollViewer1��5�      $��Hidden=�64   $	��510��6[   $	��425��6h   $��Hidden=�6t   $, 0��6�   $+ 0��6�   .��5�      �� -   5        $��
inkCanvas1��5�       $	��510��62   $
9 None=�6�   $
��1760��6�   : J� 	AllowDrop: . .H�5�      �� $1 
Horizontal=�6%   $��0q�6>   .+�5�        Pen.Math.Grid_  -   5        ;  Name$; 	gridPage1��5�   *   $	��500��6=   #��[ 6J   .+�5�   "   �� -   5        $��
labelPage1��5�   )   $��1��6;   $��24(�6G   $��
359,0,17,9q�6U   $��Bottom=�6i   $��Right=�6�   ���	#FFFF22006�   5�        5�      -   5        $; 	gridPage2��5�   *   $	��500��6=   #��[ 6K   .+�5�   "   �� -   5        $��
labelPage2��5�   )   $��2��6;   $��24(�6G   $��
359,0,17,9q�6U   $��Bottom=�6i   $��Right=�6�   ���	#FFFF22006�   5�        5�      -   5        $; 	gridPage3��5�   *   $	��500��6=   #��[ 6J   .+�5�   "   �� -   5        $��
labelPage3��5�   )   $��3��6;   $��24(�6G   $��
359,0,17,9q�6U   $��Bottom=�6i   $��Right=�6�   ���	#FFFF22006�   5�        5�      -   5        $; 	gridPage4��5�   *   $	��500��6>   #��[ 6K   .+�5�   "   �� -   5        $��
labelPage4��5�   )   $��4��6;   $��24(�6G   $��
359,0,17,9q�6U   $��Bottom=�6i   $��Right=�6�   ���	#FFFF22006�   5�      5�      5�      5�      d� 5�      -   5        $��
buttonPen1��5�        ToggleButtonStyle#��[ 61   $+ 20��6\   $	, 520��6m   $��47��6~   .��5�      �� #��[ 6   $(��"/Pen.Math;component/Images/pen.png��6@   5�      d� 5�      -   5        $��buttonEraser1��5�      #��[ 64   $+ 70��6_   $	, 520��6p   $��47��6�   .��5�      �� #��[ 6   $+��%/Pen.Math;component/Images/eraser.png��6@   5�        5�      -   5        $��radColorPicker1��5�   K   $+ 26��6)   $	, 563��6:   <   AutomaticColor$< 	#FFFF6400��6b   =   SelectedColor$= 	#FFFF6400��6}   6�   5�      �� 5�      -   5        $��
scrollBar2��5�      $	+ 860��6*   $	, 716��6<   $��84��6M   $	��300��6Y   $9 GestureOnly=�6e   6   �� 5�      -   5        $��scrollViewer2��5�      $��Hidden=�60   $	��510��6W   $	+ 840��6d   $	, 200��6v   $	��425��6�   $��Hidden=�6�   .��5�      �� -   5        $��
inkCanvas2��5�      $	��510��6.   $
9 None=�6�   $
��1760��6�   : . .H�5�      �� $1 
Horizontal=�6!   $��0q�6:   .+�5�        -    5        $; 	gridPage5��5�   &   $	��500��69   #��[ 6F   .+�5�      �� -!   5        $��
labelPage5��5�   %   $��1��67   $��24(�6C   $��
359,0,17,9q�6Q   $��Bottom=�6e   $��Right=�6�   ���	#FFFF22006�   5�        5�      -"   5        $; 	gridPage6��5�   &   $	��500��69   #��[ 6G   .+�5�      �� -#   5        $��
labelPage6��5�   %   $��2��67   $��24(�6C   $��
359,0,17,9q�6Q   $��Bottom=�6e   $��Right=�6�   ���	#FFFF22006�   5�        5�      -$   5        $; 	gridPage7��5�   &   $	��500��69   #��[ 6F   .+�5�      �� -%   5        $��
labelPage7��5�   %   $��3��67   $��24(�6C   $��
359,0,17,9q�6Q   $��Bottom=�6e   $��Right=�6�   ���	#FFFF22006�   5�        5�      -&   5        $; 	gridPage8��5�   &   $	��500��6:   #��[ 6G   .+�5�      �� -'   5        $��
labelPage8��5�   %   $��4��67   $��24(�6C   $��
359,0,17,9q�6Q   $��Bottom=�6e   $��Right=�6�   ���	#FFFF22006�   5�      5�      5�      5�      d� 5�      -(   5        $��
buttonPen2��5�      #��[ 6-   $
+ 1214��6X   $	, 720��6k   $��47��6|   .��5�      �� #��[ 6   $(��"/Pen.Math;component/Images/pen.png��6<   5�      d� 5      -)   5        $��buttonEraser2��5      #��[ 60   $
+ 1164��6[   $	, 720��6n   $��47��6   .��5     �� #��[ 6   $+��%/Pen.Math;component/Images/eraser.png��6<   5       5     -*   5        $��radColorPicker2��5  I   $
+ 1226��6%   $	, 763��68   $< 	#FFFF6400��6`   $= 	#FFFF6400��6{   6�   d� 5     -+   5        $��
buttonPen3��5     #��[ 6-   $	+ 810��6Y   $	, 712��6k   $��47��6|   .��5     �� #��[ 6   $(��"/Pen.Math;component/Images/pen.png��6<   5	     d� 5     -,   5        $��buttonEraser3��5     #��[ 60   $	+ 810��6\   $	, 756��6n   $��47��6   .��5     �� #��[ 6   $+��%/Pen.Math;component/Images/eraser.png��6<   5     d� 5     --   5        $��buttonSelect��5     #��[ 6/   $	+ 760��6[   $	, 712��6m   $��47��6~   .��5     �� #��[ 6   $+��%/Pen.Math;component/Images/select.png��6<   5       5     -.   5        $��radColorPicker3��5  H   $	+ 767��6%   $	, 761��67   6_   �� 5     -/   5        $��inkCanvasTemp��5     #��[ 6-   d� 5     -0   5        $��buttonExpander��5     $��32��61   $+ -1��6<   $	, 152��6M   #��[ 6{   $��30��6�   .��5     �� -1   5        $��imageButtonExpander��5  �   #��[ 6   $)��#/Pen.Math;component/Images/left.png��6<   $��16��6i   $��16��6u   6�   5     d� 5     -2   5        $��buttonGeometryMode��5  =   $+ -1��6   $	, 112��6,   #��[ 6W   $��32��6�   $��32��6�   .��5     �� -3   5        $��imageButtonGeometry��5  �   $'��!/Pen.Math;component/Images/G2.png��6   $��16��6G   $��16��6R   #��[ 6^   6�   5      d� 5"     -4   5        $��buttonAlgebraMode��5"  <   $+ -1��6   $, 71��6,   #��[ 6U   $��32��6�   $��32��6�   .��5#     �� -5   5        $��imageButtonAlgebra��5#  �   $'��!/Pen.Math;component/Images/A1.png��6   $��16��6G   $��16��6R   #��[ 6^   6�   5$     �� 5'     -6   5        $��borderAlgebraPanel��5'     $��57��60   $	+ 425��6;   #��[	 6M   ._�5(     �� .+�5*     �� -7   5        $��buttonMathPanel��5*  a   #��[
 6   $, 5��6B   $+ 5��6Q   $��47��6�   .��5+     �� #��[ 6%   .+�5,     �� #��[ 6$   $-��'/Pen.Math;component/Images/equation.png��6H   �� 5-     #��[ 6(   5.     5/     �� 51     -8   5        $��buttonGraph��51  b   #��[
 6   $, 47��6B   $+ 5��6R   $��47��6�   .��52     �� #��[ 6%   .+�53     �� #��[ 6$   $*��$/Pen.Math;component/Images/graph.png��6H   �� 54     #��[ 6(   55     56     57     58     �� 5:     -9   5        $��borderGeometryPanel��5:     $	��247��60   $+ 15��6<   #��[	 6M   $	, 810��6w   ._�5;     �� .+�5=     �� -:   5        $��
buttonLine��5=     $, 5��6/   $+ 5��6>   $��47��6N   #��[
 6Y   .��5>     �� #��[ 6%   .+�5?     �� #��[ 6$   $)��#/Pen.Math;component/Images/line.png��6H   �� 5@     #��[ 6(   5A     5B     �� 5D     -;   5        $��buttonRectangle��5D     $, 52��64   $+ 5��6D   $��47��6T   #��[
 6_   .��5E     �� #��[ 6%   .+�5F     �� #��[ 6$   $.��(/Pen.Math;component/Images/rectangle.png��6H   �� 5G     #��[ 6(   5H     5I     �� 5K     -<   5        $��buttonTriangle��5K     $, 5��63   $+ 55��6B   $��47��6S   #��[
 6^   .��5L     �� #��[ 6%   .+�5M     �� #��[ 6$   $-��'/Pen.Math;component/Images/triangle.png��6H   �� 5N     #��[ 6(   5O     5P     �� 5R     -=   5        $��buttonCircle��5R     $, 52��61   $+ 55��6A   $��47��6R   #��[
 6]   .��5S     �� #��[ 6%   .+�5T     �� #��[ 6$   $+��%/Pen.Math;component/Images/circle.png��6H   �� 5U     #��[ 6(   5V     5W     5Y     5Z     �� 5_     ->   5        $��button9��5_  i   
�������$
+ 1145��6(   $	, 112��6;   $��Button��6L   $��28��6]   $��98��6x   $y�Hidden=�6�   > J� Effect> 5`       5a     $% 10Q�6'   $
& Gray��67   $	' 0.6Q�6D   $( 0Q�6R   5c     �� 5d     -?   5        $��button6��5d  D   $
+ 1147��6   $, 76��6(   $��29��68   $	��114��6S   $��Button 6��6_   $y�Hidden=�6�   5h     5i     5j     