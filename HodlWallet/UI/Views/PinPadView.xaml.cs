﻿//
// PinPadView.xaml.cs
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
using System;
using System.Linq;

using Xamarin.Forms;

using HodlWallet.Core.ViewModels;
using HodlWallet.UI.Controls;

namespace HodlWallet.UI.Views
{
    public partial class PinPadView : ContentPage
    {
        PinPadViewModel ViewModel => (PinPadViewModel)BindingContext;
        
        public PinPadView(string nextView)
        {
            InitializeComponent();
            SetPinPadControlBindings();
            SubscribeToMessages();

            ViewModel.NextView = nextView;

            if (ViewModel.NextView == "PinPadChangeView")
            {
                NavigationPage.SetHasBackButton(this, false);
            }
            else
            {
                ToolbarItems.Clear();
                NavigationPage.SetHasBackButton(this, true);
            }
        }

        async void CloseToolbarItem_Clicked(object sender, EventArgs e)
        {
            if (ViewModel.NextView == "PinPadChangeView")
            {
                await Shell.Current.GoToAsync("../..");

                return;
            }

            await Navigation.PopModalAsync();
        }

        void SetPinPadControlBindings()
        {
            var vm = (PinPadViewModel)BindingContext;

            PinPadControl.SetBinding(PinPad.TitleProperty, "PinPadTitle");
            PinPadControl.SetBinding(PinPad.HeaderProperty, "PinPadHeader");
            PinPadControl.SetBinding(PinPad.WarningProperty, "PinPadWarning");

            PinPadControl.BindingContext = vm;
        }

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<PinPadViewModel>(this, "NavigateToNextView", NavigateToNextView);
        }

        async void NavigateToNextView(PinPadViewModel _)
        {
            switch(ViewModel.NextView)
            {
                case "NewWalletInfoView":
                    await Navigation.PushAsync(new NewWalletInfoView());
                    break;

                case "PinPadChangeView":
                    await Navigation.PushAsync(new PinPadChangeView());
                    break;

                case "RecoverInfoView":
                    await Navigation.PushAsync(new RecoverInfoView());
                    break;
            }

            // No page should go back to the pinpad view
            if (Navigation.NavigationStack.Contains(this)) Navigation.RemovePage(this);
        }
    }
}
