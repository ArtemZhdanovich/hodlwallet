﻿//
// SyncingToolbarItem.xaml.cs
//
// Author:
//       Igor Guerrero <igorgue@protonmail.com>
//
// Copyright (c) 2021 HODL Wallet
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
using System.Threading.Tasks;

using Xamarin.Forms;

namespace HodlWallet.UI.Controls
{
    public partial class SyncingToolbarItem : HideableToolbarItem
    {
        readonly string[] states = new [] { "node", "node_0_connections", "node_1_connection", "node_2_connections", "node_3_connections" };
        readonly CancellationTokenSource Cts;

        const int FLIP_DELAY_MS = 1_000;

        public SyncingToolbarItem()
        {
            InitializeComponent();

            Cts = new CancellationTokenSource();

            Observable
                .Interval(TimeSpan.FromMilliseconds(FLIP_DELAY_MS))
                .Subscribe(_ => DoFlip(), Cts.Token);
        }

        void DoFlip()
        {
            var fileName = (IconImageSource as FileImageSource).File;
            var index = Array.IndexOf(states, fileName);

            var newFileName = states[++index % 5];

            IconImageSource = newFileName;
        }
    }
}
