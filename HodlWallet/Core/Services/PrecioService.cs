﻿//
// PrecioService.cs
//
// Author:
//       Igor Guerrero <igorgue@protonmail.com>
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
using System.Reactive.Linq;
using System.Threading;

using Liviano.Services;
using Liviano.Services.Models;
using ReactiveUI;
using Xamarin.Forms;

using HodlWallet.Core.Interfaces;
using HodlWallet.Core.Services;

[assembly: Dependency(typeof(PrecioService))]
namespace HodlWallet.Core.Services
{
    public class PrecioService : ReactiveObject, IPrecioService
    {
        readonly Price service = new();

        CurrencyEntity[] rates;
        public CurrencyEntity[] Rates
        {
            get => rates;
            set => this.RaiseAndSetIfChanged(ref rates, value);
        }

        PrecioEntity precio;
        public PrecioEntity Precio
        {
            get => precio;
            set => this.RaiseAndSetIfChanged(ref precio, value);
        }

        public PrecioService()
        {
            service
                .WhenAnyValue(service => service.Rates)
                .Subscribe(_ => Rates = service.Rates);

            service
                .WhenAnyValue(service => service.Precio)
                .Subscribe(_ => Precio = service.Precio);
        }

        public void Start()
        {
            service.Start();
        }
    }
}
