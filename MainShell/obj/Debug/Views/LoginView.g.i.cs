﻿#pragma checksum "..\..\..\Views\LoginView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "07C49D168C70560FA806362D21DD0D3C"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using LoginSample.Hepler;
using SampleUI.Controls;
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


namespace LoginSample.Views {
    
    
    /// <summary>
    /// LoginView
    /// </summary>
    public partial class LoginView : SampleUI.Controls.MetroWindow, System.Windows.Markup.IComponentConnector {
        
        
        #line 60 "..\..\..\Views\LoginView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal SampleUI.Controls.CaptionButton Close;
        
        #line default
        #line hidden
        
        
        #line 77 "..\..\..\Views\LoginView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal SampleUI.Controls.CaptionButton Min;
        
        #line default
        #line hidden
        
        
        #line 124 "..\..\..\Views\LoginView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TbUserName;
        
        #line default
        #line hidden
        
        
        #line 143 "..\..\..\Views\LoginView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox PbPwd;
        
        #line default
        #line hidden
        
        
        #line 165 "..\..\..\Views\LoginView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Documents.Hyperlink LinkReg;
        
        #line default
        #line hidden
        
        
        #line 174 "..\..\..\Views\LoginView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Documents.Hyperlink LinRecoverPwd;
        
        #line default
        #line hidden
        
        
        #line 186 "..\..\..\Views\LoginView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox ChkRemeberPwd;
        
        #line default
        #line hidden
        
        
        #line 193 "..\..\..\Views\LoginView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox ChkAutoLogin;
        
        #line default
        #line hidden
        
        
        #line 219 "..\..\..\Views\LoginView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid MsgGrid;
        
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
            System.Uri resourceLocater = new System.Uri("/FileFerry;component/views/loginview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Views\LoginView.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
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
            this.Close = ((SampleUI.Controls.CaptionButton)(target));
            
            #line 65 "..\..\..\Views\LoginView.xaml"
            this.Close.Click += new System.Windows.RoutedEventHandler(this.Close_OnClick);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Min = ((SampleUI.Controls.CaptionButton)(target));
            
            #line 83 "..\..\..\Views\LoginView.xaml"
            this.Min.Click += new System.Windows.RoutedEventHandler(this.Min_OnClick);
            
            #line default
            #line hidden
            return;
            case 3:
            this.TbUserName = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.PbPwd = ((System.Windows.Controls.PasswordBox)(target));
            
            #line 154 "..\..\..\Views\LoginView.xaml"
            this.PbPwd.PasswordChanged += new System.Windows.RoutedEventHandler(this.PbPwd_OnPasswordChanged);
            
            #line default
            #line hidden
            return;
            case 5:
            this.LinkReg = ((System.Windows.Documents.Hyperlink)(target));
            
            #line 166 "..\..\..\Views\LoginView.xaml"
            this.LinkReg.Click += new System.Windows.RoutedEventHandler(this.LinkReg_OnClick);
            
            #line default
            #line hidden
            return;
            case 6:
            this.LinRecoverPwd = ((System.Windows.Documents.Hyperlink)(target));
            
            #line 175 "..\..\..\Views\LoginView.xaml"
            this.LinRecoverPwd.Click += new System.Windows.RoutedEventHandler(this.LinRecoverPwd_OnClick);
            
            #line default
            #line hidden
            return;
            case 7:
            this.ChkRemeberPwd = ((System.Windows.Controls.CheckBox)(target));
            
            #line 190 "..\..\..\Views\LoginView.xaml"
            this.ChkRemeberPwd.Click += new System.Windows.RoutedEventHandler(this.ChkRemeberPwd_OnClick);
            
            #line default
            #line hidden
            return;
            case 8:
            this.ChkAutoLogin = ((System.Windows.Controls.CheckBox)(target));
            
            #line 196 "..\..\..\Views\LoginView.xaml"
            this.ChkAutoLogin.Click += new System.Windows.RoutedEventHandler(this.ChkAutoLogin_OnClick);
            
            #line default
            #line hidden
            return;
            case 9:
            this.MsgGrid = ((System.Windows.Controls.Grid)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

