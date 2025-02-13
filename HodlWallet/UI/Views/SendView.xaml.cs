﻿//
// SendView.xaml.cs
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
using System.Threading.Tasks;

using Xamarin.Forms;

using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

using HodlWallet.Core.Utils;
using HodlWallet.Core.ViewModels;
using HodlWallet.UI.Extensions;
using HodlWallet.UI.Locale;
using ReactiveUI;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.ComponentModel;
using HodlWallet.UI.Controls;

namespace HodlWallet.UI.Views
{
    public partial class SendView : ContentPage
    {
        SendViewModel ViewModel => BindingContext as SendViewModel;

        public SendView()
        {
            InitializeComponent();
            SubscribeToMessages();
        }

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<SendViewModel>(this, "OpenBarcodeScanner", OpenBarcodeScanner);
            MessagingCenter.Subscribe<SendViewModel, string[]>(this, "DisplayProcessAlertError", DisplayProcessAlertError);
            MessagingCenter.Subscribe<SendViewModel, ValueTuple<decimal, decimal>>(this, "AskToBroadcastTransaction", AskToBroadcastTransaction);
        }

        async void AskToBroadcastTransaction(SendViewModel vm, (decimal, decimal) values)
        {
            decimal totalOut = values.Item1;
            decimal fees = values.Item2 / 100000000;
            var total = totalOut + fees;

            string title = LocaleResources.Send_transaction;

            string message = string.Format(LocaleResources.Send_transactionMessage, total, totalOut, fees, ViewModel.AddressToSendTo);
            string okButton = LocaleResources.Send_transactionOk;
            string cancelButton = LocaleResources.Send_transactionCancel;

            var res = await this.DisplayPrompt(title, message, okButton, cancelButton);

            if (!res) return;

            ViewModel.BroadcastTransaction();
        }

        async void OpenBarcodeScanner(SendViewModel _)
        {
            // Android
            if (Device.RuntimePlatform == Device.Android)
            {
                RxApp.MainThreadScheduler.Schedule(async () =>
                {
                    MobileBarcodeScanner scanner = new();
                    ZXing.Result resultAndroid = await scanner.Scan();

                    ViewModel.ProcessBarcodeScannerResult(resultAndroid.Text);
                });
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                // TODO These definitions should be moved to another place...
                //var customOverlay = new StackLayout
                //{
                //    HorizontalOptions = LayoutOptions.FillAndExpand,
                //    VerticalOptions = LayoutOptions.FillAndExpand
                //};
                //var torch = new Button
                //{
                //    Text = "Toggle Torch"
                //};
                //torch.Clicked += delegate {
                //    _ScanPage.ToggleTorch();
                //};
                //customOverlay.Children.Add(torch);

                ZXingScannerPage scanPage = new(
                    //customOverlay: customOverlay,
                    new MobileBarcodeScanningOptions
                    {
                        AutoRotate = true,
                        UseNativeScanning = true,
                        DelayBetweenAnalyzingFrames = 5,
                        DelayBetweenContinuousScans = 5
                    }
                );

                scanPage.Title = LocaleResources.Scan_Title;

                scanPage.OnScanResult += (ZXing.Result resultIOS) =>
                {
                    scanPage.IsScanning = false;

                    RxApp.MainThreadScheduler.Schedule(async () =>
                    {
                        ViewModel.ProcessBarcodeScannerResult(resultIOS.Text);

                        await Navigation.PopAsync();
                    });
                };

                await Navigation.PushAsync(scanPage);
            }
            else
            {
                throw new ArgumentException($"Platform {Device.RuntimePlatform} not supported");
            }
        }

        void DisplayProcessAlertError(SendViewModel _, string[] messageAndTitle)
        {
            string title = messageAndTitle[0] ?? Constants.DISPLAY_ALERT_ERROR_TITLE;
            string message = messageAndTitle[1];

            RxApp.MainThreadScheduler.Schedule(async () =>
                await this.DisplayToast(message ?? string.Join("", messageAndTitle))
            );
        }

        void FeeSlider_DragCompleted(object sender, EventArgs e)
        {
            //await ViewModel.SetSliderValue();
        }

        void AmountEntry_AmountChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(amountEntry.Text)) return;

            Observable
                .Start(() =>
                {
                    ViewModel.Amount = amountEntry.ViewModel.Amount;

                    ViewModel.CalculateTotals();
                }, RxApp.TaskpoolScheduler);
        }

        // FIXME These two following functions should not exists
        // I do not understand binding propertly looks like...

        void CustomFee_TextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = (sender as Entry);

            if (string.IsNullOrEmpty(entry.Text)) return;
            if (entry.Text.EndsWith(".")) return;

            Observable
                .Start(() =>
                {
                    amountEntry.Fee = decimal.Parse(entry.Text);

                    ViewModel.CalculateTotals();
                }, RxApp.TaskpoolScheduler);
        }

        void Address_Changed(object sender, PropertyChangedEventArgs e)
        {
            var entry = (sender as AddressEntry);

            if (string.IsNullOrEmpty(entry.Text)) return;

            Observable
                .Start(() =>
                {
                    amountEntry.AddressToSend = entry.Text;

                    ViewModel.CalculateTotals();
                }, RxApp.TaskpoolScheduler);
        }
    }
}
