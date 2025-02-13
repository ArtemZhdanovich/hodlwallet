﻿//
// AppDelegate.cs
//
// Copyright (c) 2019 HODL Wallet
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using Foundation;
using UIKit;

using Rg.Plugins.Popup;
using Serilog;

using HodlWallet.Core.Interfaces;
using HodlWallet.UI;

[assembly: global::Xamarin.Forms.ResolutionGroupName("HodlWallet")]
namespace HodlWallet.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        IWalletService WalletService => global::Xamarin.Forms.DependencyService.Get<IWalletService>();
        UIVisualEffectView blurView = null;

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Popup.Init();

            global::Xamarin.Forms.Forms.Init();
            global::ZXing.Net.Mobile.Forms.iOS.Platform.Init();

            LoadApplication(new App());

            SetupLogging();

            return base.FinishedLaunching(app, options);
        }

        public override void OnActivated(UIApplication uiApplication)
        {
            blurView?.RemoveFromSuperview();
            blurView?.Dispose();
            blurView = null;

            base.OnActivated(uiApplication);
        }

        public override void OnResignActivation(UIApplication uiApplication)
        {
            using var blurEffect = UIBlurEffect.FromStyle(UIBlurEffectStyle.Dark);

            blurView = new UIVisualEffectView(blurEffect)
            {
                Frame = UIApplication.SharedApplication.KeyWindow.RootViewController.View.Bounds
            };

            UIApplication.SharedApplication.KeyWindow.RootViewController.View.AddSubview(blurView);

            if (UIApplication.SharedApplication.KeyWindow.RootViewController.PresentedViewController != null)
            {
                UIApplication.SharedApplication.KeyWindow.RootViewController.PresentedViewController.View.AddSubview(blurView);
            }

            base.OnResignActivation(uiApplication);
        }

        void SetupLogging()
        {
            // Call after LoadApplication
#if DEBUG
            WalletService.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.NSLog()
                .Enrich.WithProperty(Serilog.Core.Constants.SourceContextPropertyName, "HodlWallet") // Sets the tag fields
                .CreateLogger();
#else
            WalletService.Logger = new LoggerConfiguration()
                .MinimumLevel.Error()
                .WriteTo.NSLog()
                .Enrich.WithProperty(Serilog.Core.Constants.SourceContextPropertyName, "HodlWallet") // Sets the tag fields
                .CreateLogger();
#endif
        }
    }
}
