﻿//
// DisplayCurrencyViewModel.cs
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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Input;

using Xamarin.Forms;

using HodlWallet.Core.Models;
using HodlWallet.Core.Utils;
using HodlWallet.Core.Services;
using System.Linq;
using Liviano.Services.Models;
using System.Reactive.Linq;
using ReactiveUI;
using System.Threading;
using System.Reactive.Concurrency;

namespace HodlWallet.Core.ViewModels 
{
    class DisplayCurrencyViewModel : BaseViewModel
    {
        readonly CancellationTokenSource cts = new();

        public ObservableCollection<CurrencySymbolEntity> CurrencySymbolEntities { get; set; } = new();

        public Command SelectedCurrencyCommand { get; }

        CurrencyEntity rate;
        public CurrencyEntity Rate
        {
            get => rate;
            set => SetProperty(ref rate, value, nameof(Rate));
        }

        CurrencySymbolEntity selectedCurrency;
        public CurrencySymbolEntity SelectedCurrency
        {
            get
            {
                return selectedCurrency;
            }
            set => SetProperty(ref selectedCurrency, value, nameof(SelectedCurrency));
        }

        bool isBtcSelected = true;
        public bool IsBtcSelected
        {
            get => isBtcSelected;
            set
            {
                SetProperty(ref isBtcSelected, value, nameof(IsBtcSelected));

                if (!isBtcSelected && !(DisplayCurrencyService.BitcoinCurrencyCode == "BTC"))
                    return;

                DisplayCurrencyService.BitcoinCurrencyCode = "BTC";
                DisplayCurrencyService.Save();
            }
        }

        bool isSatSelected = false;
        public bool IsSatSelected
        {
            get => isSatSelected;
            set
            {
                SetProperty(ref isSatSelected, value, nameof(IsSatSelected));

                if (!isSatSelected && !(DisplayCurrencyService.BitcoinCurrencyCode == "SAT"))
                    return;

                DisplayCurrencyService.BitcoinCurrencyCode = "SAT";
                DisplayCurrencyService.Save();
            }
        }

        public DisplayCurrencyViewModel()
        {
            SelectedCurrencyCommand = new Command(SaveSelectedCurrency);


            IsBtcSelected = DisplayCurrencyService.BitcoinCurrencyCode == "BTC";
            IsSatSelected = !IsBtcSelected;

            Observable
                .Start(() =>
                {
                    IsLoading = true;

                    PopulateFiatCurrencies();

                    IsLoading = false;

                    SetSelectedCurrency();
                }, RxApp.TaskpoolScheduler)
                .Subscribe(cts.Token);
        }

        void SetSelectedCurrency()
        {
            //Lookup for the Currency previously saved
            string currencyCode = DisplayCurrencyService.FiatCurrencyCode;

            SelectedCurrency = CurrencySymbolEntities.FirstOrDefault(p => p.Code == currencyCode);
        }

        void SaveSelectedCurrency()
        {
            SecureStorageService.SetFiatCurrencyCode(SelectedCurrency.Code);
            DisplayCurrencyService.FiatCurrencyCode = selectedCurrency.Code;
            DisplayCurrencyService.Save();

            SetSelectedCurrency();
        }

        void PopulateFiatCurrencies()
        {
            foreach (var rate in PrecioService.Rates)
            {
                CurrencySymbolEntities.Add(new CurrencySymbolEntity
                {
                    Code = rate.Code,
                    Symbol = CurrencyUtils.GetSymbol(rate.Code),
                    Name = rate.Name,
                    Rate = rate.Rate
                });
            }
        }
    }
}