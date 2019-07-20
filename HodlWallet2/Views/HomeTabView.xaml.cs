﻿using System;
using System.Collections.Generic;
using HodlWallet2.Core.ViewModels;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using Xamarin.Forms;

namespace HodlWallet2.Views
{
    [MvxTabbedPagePresentation(TabbedPosition.Tab, WrapInNavigationPage = false, Title = "")]
    public partial class HomeTabView : MvxContentPage<HomeTabViewModel>
    {
        public HomeTabView()
        {
            IconImageSource = "home_tab.png";

            InitializeComponent();
        }
    }
}
