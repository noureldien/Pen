﻿#pragma checksum "..\..\..\ColorPicker\ColorPicker.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "3E2D853A6CC413D59AB7E94720619FE5"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace WPF {
    
    
    /// <summary>
    /// ColorPicker
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
    public partial class ColorPicker : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 39 "..\..\..\ColorPicker\ColorPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image ColorImage;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\ColorPicker\ColorPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas CanvImage;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\..\ColorPicker\ColorPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Ellipse ellipsePixel;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/WPF;component/colorpicker/colorpicker.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\ColorPicker\ColorPicker.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.ColorImage = ((System.Windows.Controls.Image)(target));
            return;
            case 2:
            this.CanvImage = ((System.Windows.Controls.Canvas)(target));
            
            #line 48 "..\..\..\ColorPicker\ColorPicker.xaml"
            this.CanvImage.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.CanvImage_MouseDown);
            
            #line default
            #line hidden
            
            #line 49 "..\..\..\ColorPicker\ColorPicker.xaml"
            this.CanvImage.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.CanvImage_MouseUp);
            
            #line default
            #line hidden
            
            #line 50 "..\..\..\ColorPicker\ColorPicker.xaml"
            this.CanvImage.MouseMove += new System.Windows.Input.MouseEventHandler(this.CanvImage_MouseMove);
            
            #line default
            #line hidden
            return;
            case 3:
            this.ellipsePixel = ((System.Windows.Shapes.Ellipse)(target));
            
            #line 52 "..\..\..\ColorPicker\ColorPicker.xaml"
            this.ellipsePixel.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.CanvImage_MouseUp);
            
            #line default
            #line hidden
            
            #line 52 "..\..\..\ColorPicker\ColorPicker.xaml"
            this.ellipsePixel.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.CanvImage_MouseDown);
            
            #line default
            #line hidden
            
            #line 52 "..\..\..\ColorPicker\ColorPicker.xaml"
            this.ellipsePixel.MouseMove += new System.Windows.Input.MouseEventHandler(this.CanvImage_MouseMove);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
