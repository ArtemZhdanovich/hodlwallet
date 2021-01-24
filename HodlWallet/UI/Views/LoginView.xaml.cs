﻿//
// LoginView.xaml.cs
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
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using HodlWallet.Core.ViewModels;

namespace HodlWallet.UI.Views
{
    public partial class LoginView : ContentPage
    {
        uint _AnimationTimeput = 50;

        Color _DigitOnColor => (Color)Application.Current.Resources["InputPinOn"];
        Color _DigitOffColor => (Color)Application.Current.Resources["InputPinOff"];

        RootView _RootView;

        LoginViewModel _ViewModel => (LoginViewModel)BindingContext;

        public LoginView()
        {
            InitializeComponent();

            SubscribeToMessages();

            // Little RootView optimization to allow it to be creaed before,
            // this only call all the constructors inside of it.
            // DOES NOT solve all performance issue, that's related to the
            // HomeView's layout being too complex (tm)
            Task.Run(() =>
                _RootView = new RootView()
            );
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (_ViewModel.IsLoading) _ViewModel.IsLoading = false;
        }

        void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<LoginViewModel, int>(this, "DigitAdded", DigitAdded);
            MessagingCenter.Subscribe<LoginViewModel, int>(this, "DigitRemoved", DigitRemoved);
            MessagingCenter.Subscribe<LoginViewModel>(this, "IncorrectPinAnimation", (vm) => _ = IncorrectPinAnimation(vm));
            MessagingCenter.Subscribe<LoginViewModel>(this, "NavigateToRootView", NavigateToRootView);
            MessagingCenter.Subscribe<LoginViewModel>(this, "ResetPin", ResetPin);
        }

        void DigitAdded(LoginViewModel _, int index)
        {
            Debug.WriteLine($"[SubscribeToMessage][DigitAdded] Add digit: {index}");

            ColorDigitTo(index, _DigitOnColor);
        }

        void DigitRemoved(LoginViewModel _, int index)
        {
            Debug.WriteLine($"[SubscribeToMessage][DigitRemoved] Remove digit: {index}");

            ColorDigitTo(index, _DigitOffColor);
        }

        async Task IncorrectPinAnimation(LoginViewModel _)
        {
            Debug.WriteLine($"[SubscribeToMessage][IncorrectPinAnimation]");

            // Shake ContentView Re-Enter PIN
            await InputGrid.TranslateTo(-15, 0, _AnimationTimeput);
            await InputGrid.TranslateTo(15, 0, _AnimationTimeput);
            await InputGrid.TranslateTo(-10, 0, _AnimationTimeput);
            await InputGrid.TranslateTo(10, 0, _AnimationTimeput);
            await InputGrid.TranslateTo(-5, 0, _AnimationTimeput);
            await InputGrid.TranslateTo(5, 0, _AnimationTimeput);

            InputGrid.TranslationX = 0;

            await Task.Delay(500);
        }

        void NavigateToRootView(LoginViewModel _)
        {
            Debug.WriteLine($"[SubscribeToMessage][NavigateToRootView]");

            // Incase we're faster than light, we call the constructor anyways.
            Navigation.PushAsync(_RootView ?? new RootView());
        }

        void ResetPin(LoginViewModel _)
        {
            Debug.WriteLine($"[SubscribeToMessage][ResetPin]");

            ColorDigitTo(Pin1, _DigitOffColor);
            ColorDigitTo(Pin2, _DigitOffColor);
            ColorDigitTo(Pin3, _DigitOffColor);
            ColorDigitTo(Pin4, _DigitOffColor);
            ColorDigitTo(Pin5, _DigitOffColor);
            ColorDigitTo(Pin6, _DigitOffColor);
        }

        void ColorDigitTo(int index, Color color)
        {
            var pin = (BoxView)FindByName($"Pin{index}");

            ColorDigitTo(pin, color);
        }

        void ColorDigitTo(BoxView pin, Color color)
        {
            pin.Color = color;
        }
    }
}
