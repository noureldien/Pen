   M S B A M L   `   `   `  ����B  >Pen.Map, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:Pen.MapPen.Map  5   	   p lTelerik.Windows.Controls.RibbonBar, Version=2010.3.1110.40, Culture=neutral, PublicKeyToken=5803cfa389c90ce7�gclr-namespace:Telerik.Windows.Controls.RibbonBar.Primitives;assembly=Telerik.Windows.Controls.RibbonBar-Telerik.Windows.Controls.RibbonBar.Primitives 5   	       Pen.Map.LessonTool   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:Pen.Map     5   	   f bTelerik.Windows.Controls, Version=2010.3.1110.40, Culture=neutral, PublicKeyToken=5803cfa389c90ce7l	 hTelerik.Windows.Controls.Input, Version=2010.3.1110.40, Culture=neutral, PublicKeyToken=5803cfa389c90ce7q
 mTelerik.Windows.Controls.Navigation, Version=2010.3.1110.40, Culture=neutral, PublicKeyToken=5803cfa389c90ce7Etelerik1http://schemas.telerik.com/2008/xaml/presentation   	 
 5   	   utgclr-namespace:Telerik.Windows.Controls.RibbonBar.Primitives;assembly=Telerik.Windows.Controls.RibbonBar   	 
 5   	   -   5        $��
windowMain��5	        � Title$  Lesson Tool��6	    � WindowStartupLocation$ CenterScreen=�5
   	   $	��600��5      $	��950��6(    � Icon$( "/Pen.Map;component/Images/tool.png��5   	   
������  � 	Resources 5      - 	 'Telerik.Windows.Controls.RadColorPicker �� 
TargetType �� BasedOn/  )Telerik.Windows.Controls.ThemeResourceKey   	ThemeType   ElementType   StackPanelTitleStyle �� Property	 �� Value  TextBoxTitleStyle  TextBoxDateStyle  GridParagraphStyle  TextBoxParagraphStyle  RectangleParagraphStyle  ButtonStyle  ToggleButtonStyle  
IconButton 	 
TextButton 
 TextButtonArabicStyle  StackPanelButton  
TitleFocus
 �� EndPoint �� 
StartPoint � Color � Offset��%  '        0�� 5   4   *  $ telerik:SummerThemeL�$ telerik:RadColorPickerL�+1&	        &	 h    &	 {    &	 �    &	 {    &	 �    &	 �    &	 �    &	 �    &		 �	    &	
 #
    &	 �
    &	 k    �� 6
   "  6    64   2  �� 5   
   " ��6-   .�5      ��  �@��Left$	 0��6,   �� 5       �@��Orientation$	 
Horizontal=�6,   �� 5       � ��$	 50��6'   �� 5       � ��	 �	#FFECF6FA6+   �� 5       � ��$		 1,0q�6'   �� 5       � ��$		 458��6&   �� 5       � ��$	 1,10,1,0q�6'   5      �� 5   
   " ��6*   .�5      ��  � ��$		 300��6&   �� 5       � ��$	 Traditional Arabic)�6+   �� 5        � ��$	 28(�6)   �� 5!       � ��$
	 15,0q�6'   �� 5"       � ��
	 ���� 6+   �� 5#       � ��
	 ���� 6,   5$      �� 5&   
   " ��6)   .�5'      ��  � ��$		 110��6&   �� 5(       � ��$	 Traditional Arabic)�6+   �� 5)       � ��$	 28(�6)   �� 5*       � ��
	 ���� 6+   �� 5+       � ��
	 ���� 6,   �� 5,       � ��	 �	#FF0588C16+   �� 5-       � ��$	 Right=�6;   5.      �� 50   
   " �6+   .�51      ��  �@��Left$	 20��6,   �� 52       � ��$	 98��6'   �� 53       � ��$		 420��6&   �� 54       � ��$	 0,10,0,0q�6'   55      �� 57   
   " ��6.   .�58      ��  �@��AcceptsReturn	 . 6.   �� 59       �@��TextWrapping$
	 Wrap=�6-   �� 5:       � ��$	 Sakkal Majalla)�6+   �� 5;       � ��$	 22(�6)   �� 5<       � ��
	 �����6+   �� 5=       � ��
	 ���� 6,   �� 5>       � ��
	 ���� 6+   �� 5?       �@��TextAlignment$	 Justify=�6.   5@      �� 5B   
   " ��60   .�5C      ��  �@��StrokeDashArray$		 5.0R�60   �� 5D       � ��
	 �����6'   �� 5E       �@��StrokeDashCap$
	 Flat=�6.   �� 5F       � ��$	 1��60   5G      �� 5M   
   " ��6$   .�5N      ��  � ��$	 40��6'   �� 5O       � ��$	 0q�60   �� 5P       � ��
	 ���� 6,   �� 5Q       � ��$
	 Left=�6;   �� 5R       � ��
	 ���� 6+   �� 5S       � |�	 .  6,   5T      �� 5U   
   " d�6*   .�5V      ��  � ��$	 40��6'   �� 5W       � ��$	 0q�60   �� 5X       � ��
	 ���� 6,   �� 5Y       � ��$
	 Left=�6;   �� 5Z       � ��
	 ���� 6+   �� 5[       � |�	 .  6,   5\      �� 5]   
   " ��6#   .�5^      ��  � ��$	 32��6'   �� 5_       � ��$	 32��6&   �� 5`       � ��$	 5,2,5,0q�6'   �� 5a       � ��$		 Top=�62   5b      �� 5c   
   " ��6#   .�5d      ��  � ��$	 18(�6)   �� 5e       � ��$	 28��6'   �� 5f       � ��$	 Segoe UI)�6+   5g      �� 5h   
   " ��6.   .�5i      ��  � ��$	 24(�6)   �� 5j       � ��$	 0,2,0,0q�6'   �� 5k       � ��$		 Top=�62   �� 5l       � ��$	 Sakkal Majalla)�6+   5m      �� 5n   
   " ��6)   .�5p      ��  � ��$
	 Left=�64   �� 5q       �@��Orientation$	 
Horizontal=�6,   5r      �� 5u   
   $
 0.5,1&�61   $ 0.5,0&�6B   .��5v      � $ 	#CF454545��6   $ 0Q�6-   � 5w      $ 	#FFF2F8FF��6   $ 0.064Q�6-   � 5x      $ 	#FFECF6FA��6   $ 1Q�6-   5y      5{      .��5}      � -   5        $��gridMain��5}      .+�5~   
   �� -   5        $��
borderMain��5~      $��2q�6#    �� CornerRadius$ 6��6.   $��1q�6?   ���	#FF003B9D._�5      �� -   5        $��
canvasMain��5      .+�5�      (   "Pen.Map.ScrollViewerOffsetMediator  -   5          Name$ mediator��5�   3   � 5�      -   5        $��popup��5�       � 	StaysOpen .  $��
MousePoint=�67   5�      �� 5�      -   5        $��canvasPopup��5�      $��40��6,   $	��180��68    �� Left$	 485��6D   
 �� Top$	 379��6V   .+�5�      �� -   5        $��buttonPopup1��5�   J   $	 119��6   $ 7��6/   $��27��6>   $��25��6^   ��5�      �� 5�       �� ImageSource$&  /Pen.Map;component/Images/ok.png��6)    j� Stretch$
 None=�6X    j� TileMode$
 None=�6g   5�      �� 5�      -	   5        $��buttonPopup2��5�   J   $	 149��6   $ 7��6/   $��27��6>   $��25��6^   ��5�      �� 5�      $* $/Pen.Map;component/Images/cancel.png��6)   $
 None=�6\   $
 None=�6k   5�      �� 5�      -
   5        $��textBoxPopup��5�      $ 7��62   $ 6��6B   $��28��6Q   $	��105��6]   $��Sakkal Majalla)�6i   $��18(�6�   ��5�      �� 5�      $
 0.5,1&�6.   $ 0.5,0&�6?   .��5�      � $ White��6+   $ 0Q�69   � 5�      $ 	#FFC7C7C7��6+   $ 1Q�6=   5�      5�      � 5�      -   5        $��gridConrent��5�      $	��490��6*   $	��460��67   $ 13��6C   $ 15��6T   
�������.+�5�      >  8Telerik.Windows.Controls.RibbonBar.Primitives.DropShadow  -   5        $��dropShadowContent��5�   #   �� 5�      -   5        $��scrollViewerContent��5�   $   $��Hidden=�6?   .��5�      �� -   5         ��Name$ stackPanelContent��5�   %   $��0,0,0,20q�6@   .+�5�      �� -   5        $��canvasLessonName��5�   %   $��50��6=   $	��460��6I   .+�5�   "   �� -   5        $��labelLesson��5�   �   $	 109��6(   $ 12��6:   $����العنــــــــــــــــــــــــــــ                                 ـــــــــــــــــــــوان��6J   
�������$��Sakkal Majalla)�6�   $��20(�6�   �� 5�   "   -   5        $��textBoxLesson��5�   F   $ 14��6*   $��34��6:   $	��170��6[   $��22(�6g   $��Sakkal Majalla)�6u   ���	#FF0588C1 �� TextAlignment$ Center=�6�   $	 140��6�   5�      �� 5�      -   5        $��stackPanelT1��5�   )   #��[  6=   .+�5�   "   �� -   5        $��	textBoxT1��5�   *   #��[ 6;   �� 5�   "   -   5        $��	textBoxD1��5�   *   #��[ 6;   5�         Pen.Map.Grid_  5�      #��[ 6*   .+�5�   "   �� #��[ 6,   �� 5�   "   -   5        $��
textBoxP11��5�   *   #��[ 6<   5�      5�      5�      5�      �� 5�      -   5        $��button1��5�      #��[ 6(   $ 12��6M   $	 510��6^   $��81��6o   .��5�      �� #��[ 6!   .+�5�      �� #��[ 6    $(��"/Pen.Map;component/Images/open.png��6D   �� 5�      -   5        $��textBlockButton1��5�   ^   #��[
 6$   $��فتح��6S   6^   5�      5�      �� 5�      -   5        $��button2��5�      #��[ 6(   $ 99��6M   $	 510��6^   $��94��6o   .��5�      �� #��[ 6!   .+�5�      �� #��[ 6    $(��"/Pen.Map;component/Images/save.png��6D   �� 5�      -   5        $��textBlockButton2��5�   ^   #��[
 6$   $��حفظ��6S   6^   5�      5�      �� 5�      -   5        $��button3��5�      #��[ 6(   $	 198��6M   $	 510��6_   $��94��6p   .��5�      �� #��[ 6!   .+�5�      �� #��[ 6    $'��!/Pen.Map;component/Images/new.png��6D   �� 5�      -   5        $��textBlockButton3��5�   _   #��[
 6$   $��جديد��6S   6_   5�      5�      �� 5�      -   5        $��button4��5�      #��[ 6(   $	 298��6M   $	 510��6_   $��94��6p   .��5�      �� #��[ 6!   .+�5�      �� #��[ 6    $)��#/Pen.Map;component/Images/title.png��6D   �� 5�      -   5        $��textBlockButton4��5�   `   #��[
 6$   $��
عنوان��6S   6`   5�      5�      �� 5�      -   5        $��button5��5�      #��[ 6(   $	 396��6M   $	 510��6_   $��86��6p   .��5�      �� #��[ 6!   .+�5�      �� #��[ 6    $-��'/Pen.Map;component/Images/paragraph.png��6D   �� 5�      -   5        $��textBlockButton5��5�   _   #��[
 6$   $��فقرة��6S   6_   5�      5�      �� 5�      -    5        $��button6��5�      #��[ 6(   $	 869��6M   $	 510��6_   $��48��6p   .��5�      �� #��[ 6!   .+�5�      �� #��[ 6    $,��&/Pen.Map;component/Images/language.png��6D   �� 5�      -!   5        $��textBlockButton6��5�   [   #��[
 6$   $�� ��6S   6[   5�      5�      � 5�      -"   5        $��gridMap��5�      $	��422��6&   $	��410��63   
�������$	 507��6R   $ 15��6d   .+�5�      7  1System.Windows.Forms.Integration.WindowsFormsHost  -#   5        $��controlHost��5�   '   $��2q�6:     5�      -$   5        $��dropShadow1��5�   #   5�      d� 5�      -%   5        $��toggleButton1��5�      #��[ 64   $	 506��6_   $	 448��6q   $��94��6�   .��5�      �� #��[ 6!   $
��Auto��6K   .+�5�      �� #��[ 6    $+��%/Pen.Map;component/Images/pushpin.png��6D   �� 5�      -&   5        $��textBlockToggleButton1��5�   _   #��[
 6$   $��دبوس��6S   6_   5�      5�      d� 5�      -'   5        $��toggleButton2��5�      #��[ 64   $	 606��6_   $	 448��6q   $��90��6�   .��5�      �� #��[ 6!   $
��Auto��6K   .+�5�      �� #��[ 6    $)��#/Pen.Map;component/Images/shape.png��6D   �� 5�      -(   5        $��textBlockToggleButton2��5�   ^   #��[
 6$   $��شكل��6S   6^   5�      5�      d� 5�      -)   5        $��toggleButton3��5�      #��[ 64   $	 704��6_   $	 448��6q   $��86��6�   .��5�      �� #��[ 6!   $
��Auto��6K   .+�5�      �� #��[ 6    $(��"/Pen.Map;component/Images/line.png��6D   �� 5�      -*   5        $��textBlockToggleButton3��5�   ]   #��[
 6$   $
��خط��6S   6]   5�      5�      d� 5�      -+   5        $��toggleButton4��5�      #��[ 64   $	 506��6_   $	 490��6q   $��94��6�   .��5�      �� #��[ 6!   $
��Auto��6K   .+�5�      �� #��[ 6    $,��&/Pen.Map;component/Images/position.png��6D   �� 5�      -,   5        $��textBlockToggleButton4��5�   _   #��[
 6$   $��مؤشر��6S   6_   5�      5      �� 5     --   5        $��textBlockColorPicker��5  z   #��[
 6   $��لـون��6K   $	 654��6W   $	 494��6i   6z     5     -.   5        $��radColorPicker1��5  L   $	 618��6)   $	 500��6;   6c   �� 5     -/   5        $��button��5  6   $��Button��6   $��23��6*   $��64��6D   $	 799��6O   $	 520��6a   $y�Hidden=�6�   5     ��5	     �� 5
     $
 0.5,1&�6&   $ 0.5,0&�67   .��5     � $ 	#FFC8E6F0��6#   $ 0Q�65   � 5     $ White��6#   $ 0.081Q�61   � 5     $ White��6#   $ 0.907Q�61   � 5     $ 	#FFC8E6F0��6#   $ 1Q�65   5     5     5     5     