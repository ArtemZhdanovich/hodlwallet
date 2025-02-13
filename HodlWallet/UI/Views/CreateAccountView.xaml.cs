﻿//
// CreateAccountView.xaml.cs
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

using Xamarin.Forms;

using HodlWallet.Core.ViewModels;
using HodlWallet.Core.Utils;
using HodlWallet.UI.Extensions;

namespace HodlWallet.UI.Views
{
    public partial class CreateAccountView : ContentPage
    {
        CreateAccountViewModel ViewModel => (CreateAccountViewModel)BindingContext;
        public CreateAccountView()
        {
            InitializeComponent();
            SubscribeToMessages();
            SetSuggestedColor();
        }

        void OnAccountTypeCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            RadioButton button = sender as RadioButton;
            if (button != null)
            {
                ViewModel.AccountType = $"{button.Content}";
            }
        }

        async void CloseToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("../..");
        }

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<CreateAccountViewModel, string>(this, Constants.LABEL_ERROR_ACCOUNT_CREATION_MESSAGE, DisplayNotification);
            MessagingCenter.Subscribe<CreateAccountViewModel, string>(this, Constants.LABEL_ALERT_CREATING_ACCOUNT_PROGRESS_MESSAGE, DisplayNotification);
            MessagingCenter.Subscribe<CreateAccountViewModel, string>(this, Constants.LABEL_ALERT_SUCCESS_ACCOUNT_CREATION_MESSAGE, DisplayNotification);
        }

        void DisplayNotification(CreateAccountViewModel vm, string message)
        {
            _ = this.DisplayToast(message);
            // When in progress disable to avoid more than one account creations.
            if (message == Constants.DISPLAY_ALERT_CREATING_ACCOUNT_PROGRESS_MESSAGE)
                AddAcountButton.IsEnabled = false;

            // After an error message validation is shown in the view, the button should be enabled again.
            if (!string.Equals(message, Constants.DISPLAY_ALERT_CREATING_ACCOUNT_PROGRESS_MESSAGE))
                AddAcountButton.IsEnabled = true;
        }

        void SetSuggestedColor()
        {
            PickColorControl.ButtonColorSelected = AppShell.RandomColor();
        }
    }
}