﻿//
// BackgroundService.cs
//
// Copyright (c) 2022 HODL Wallet
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
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading.Tasks;

using ReactiveUI;
using Xamarin.Forms;
using UIKit;

using HodlWallet.Core.Interfaces;
using HodlWallet.iOS.Services;

[assembly: Dependency(typeof(BackgroundService))]
namespace HodlWallet.iOS.Services
{
    public class BackgroundService : IBackgroundService
    {
        public async Task Start(string name, Func<Task> func)
        {
            var taskId = UIApplication.SharedApplication.BeginBackgroundTask(name, () =>
                Debug.WriteLine("[BackgroundService] expired at {0}", DateTime.UtcNow.ToLocalTime())
            );
            
            Debug.WriteLine("[BackgroundService] {0} started at {1}", name, DateTime.UtcNow.ToLocalTime());

            await Observable.Start(async () => await func.Invoke(), RxApp.TaskpoolScheduler);
            
            UIApplication.SharedApplication.EndBackgroundTask(taskId);

            Debug.WriteLine("[BackgroundService] {0} stopped at {1}", name, DateTime.UtcNow.ToLocalTime());
        }
    }
}