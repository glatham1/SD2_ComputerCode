﻿#pragma checksum "..\..\MainWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "82ADB820F9B0FE1F873EDB658BBC45EA50E93338D20579B501A3EE50E7E4BBA1"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Arction.Wpf.Charting;
using Arction.Wpf.Charting.Annotations;
using Arction.Wpf.Charting.Axes;
using Arction.Wpf.Charting.ChartManager;
using Arction.Wpf.Charting.EventMarkers;
using Arction.Wpf.Charting.Maps;
using Arction.Wpf.Charting.OverlayElements;
using Arction.Wpf.Charting.Series3D;
using Arction.Wpf.Charting.SeriesPolar;
using Arction.Wpf.Charting.SeriesRound;
using Arction.Wpf.Charting.SeriesSmith;
using Arction.Wpf.Charting.SeriesXY;
using Arction.Wpf.Charting.Titles;
using Arction.Wpf.Charting.TypeConverters;
using Arction.Wpf.Charting.Views;
using Arction.Wpf.Charting.Views.View3D;
using Arction.Wpf.Charting.Views.ViewPie3D;
using Arction.Wpf.Charting.Views.ViewPolar;
using Arction.Wpf.Charting.Views.ViewRound;
using Arction.Wpf.Charting.Views.ViewSmith;
using Arction.Wpf.Charting.Views.ViewXY;
using NanoPSI;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
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


namespace NanoPSI {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 84 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox mcuConnect;
        
        #line default
        #line hidden
        
        
        #line 89 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label mcuStatus;
        
        #line default
        #line hidden
        
        
        #line 100 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label sqlConnLabel;
        
        #line default
        #line hidden
        
        
        #line 114 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox minThresh;
        
        #line default
        #line hidden
        
        
        #line 118 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox maxThresh;
        
        #line default
        #line hidden
        
        
        #line 127 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button startButton;
        
        #line default
        #line hidden
        
        
        #line 128 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button pauseButton;
        
        #line default
        #line hidden
        
        
        #line 129 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button stopButton;
        
        #line default
        #line hidden
        
        
        #line 133 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblStartTime;
        
        #line default
        #line hidden
        
        
        #line 137 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblElapsedTime;
        
        #line default
        #line hidden
        
        
        #line 141 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblEndTime;
        
        #line default
        #line hidden
        
        
        #line 171 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid dataGrid;
        
        #line default
        #line hidden
        
        
        #line 202 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox checkBoxShowValuesNextToCursor;
        
        #line default
        #line hidden
        
        
        #line 204 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox checkBoxSnapToNearestDataPoint;
        
        #line default
        #line hidden
        
        
        #line 206 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton radioButtonAccurate;
        
        #line default
        #line hidden
        
        
        #line 208 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton radioButtonCoarse;
        
        #line default
        #line hidden
        
        
        #line 210 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid gridChart;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/NanoPSI;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.mcuConnect = ((System.Windows.Controls.ComboBox)(target));
            
            #line 84 "..\..\MainWindow.xaml"
            this.mcuConnect.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.mcuConnect_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            this.mcuStatus = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            
            #line 91 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.sqlConnLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 5:
            
            #line 103 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click_1);
            
            #line default
            #line hidden
            return;
            case 6:
            this.minThresh = ((System.Windows.Controls.TextBox)(target));
            
            #line 114 "..\..\MainWindow.xaml"
            this.minThresh.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.MinThresh_TextChanged);
            
            #line default
            #line hidden
            return;
            case 7:
            this.maxThresh = ((System.Windows.Controls.TextBox)(target));
            
            #line 118 "..\..\MainWindow.xaml"
            this.maxThresh.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.MaxThresh_TextChanged);
            
            #line default
            #line hidden
            return;
            case 8:
            this.startButton = ((System.Windows.Controls.Button)(target));
            
            #line 127 "..\..\MainWindow.xaml"
            this.startButton.Click += new System.Windows.RoutedEventHandler(this.Button_Click_2);
            
            #line default
            #line hidden
            return;
            case 9:
            this.pauseButton = ((System.Windows.Controls.Button)(target));
            
            #line 128 "..\..\MainWindow.xaml"
            this.pauseButton.Click += new System.Windows.RoutedEventHandler(this.Button_Click_PauseTest);
            
            #line default
            #line hidden
            return;
            case 10:
            this.stopButton = ((System.Windows.Controls.Button)(target));
            
            #line 129 "..\..\MainWindow.xaml"
            this.stopButton.Click += new System.Windows.RoutedEventHandler(this.Button_Click_StopTest);
            
            #line default
            #line hidden
            return;
            case 11:
            this.lblStartTime = ((System.Windows.Controls.Label)(target));
            return;
            case 12:
            this.lblElapsedTime = ((System.Windows.Controls.Label)(target));
            return;
            case 13:
            this.lblEndTime = ((System.Windows.Controls.Label)(target));
            return;
            case 14:
            
            #line 151 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click_3);
            
            #line default
            #line hidden
            return;
            case 15:
            
            #line 159 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click_4);
            
            #line default
            #line hidden
            return;
            case 16:
            this.dataGrid = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 17:
            this.checkBoxShowValuesNextToCursor = ((System.Windows.Controls.CheckBox)(target));
            
            #line 202 "..\..\MainWindow.xaml"
            this.checkBoxShowValuesNextToCursor.Checked += new System.Windows.RoutedEventHandler(this.checkBox1_CheckedChanged);
            
            #line default
            #line hidden
            
            #line 202 "..\..\MainWindow.xaml"
            this.checkBoxShowValuesNextToCursor.Unchecked += new System.Windows.RoutedEventHandler(this.checkBox1_CheckedChanged);
            
            #line default
            #line hidden
            return;
            case 18:
            this.checkBoxSnapToNearestDataPoint = ((System.Windows.Controls.CheckBox)(target));
            
            #line 204 "..\..\MainWindow.xaml"
            this.checkBoxSnapToNearestDataPoint.Checked += new System.Windows.RoutedEventHandler(this.checkBoxSnapToNearestDataPoint_Checked);
            
            #line default
            #line hidden
            
            #line 204 "..\..\MainWindow.xaml"
            this.checkBoxSnapToNearestDataPoint.Unchecked += new System.Windows.RoutedEventHandler(this.checkBoxSnapToNearestDataPoint_Unchecked);
            
            #line default
            #line hidden
            return;
            case 19:
            this.radioButtonAccurate = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 20:
            this.radioButtonCoarse = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 21:
            this.gridChart = ((System.Windows.Controls.Grid)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

