﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using Foundation;
using HodlWallet2.iOS;
using HodlWallet2.Core.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(ShareIntent))]
namespace HodlWallet2.iOS
{
	public class ShareIntent : IShareIntent
	{
		public void TextShareIntent(string text)
		{
			var activityController = new UIActivityViewController(new NSObject[] { UIActivity.FromObject(text) }, null);
			var topController = UIApplication.SharedApplication.KeyWindow.RootViewController;

			while (topController.PresentedViewController != null)
			{
				topController = topController.PresentedViewController;
			}

			topController.PresentViewController(activityController, true, null);
		}
	}
}
