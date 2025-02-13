﻿//
// BackupViewModel.cs
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
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using HodlWallet.Core.Services;
using ReactiveUI;

namespace HodlWallet.Core.ViewModels
{
    public class BackupViewModel : BaseViewModel
    {
        public ICommand WriteDownWordsCommand { get; }

        CancellationTokenSource Cts { get; set; }

        public BackupViewModel()
        {
            Cts ??= new CancellationTokenSource();

            CreateSeedIfNeeded();
        }

        void CreateSeedIfNeeded()
        {
            if (SecureStorageService.HasMnemonic())
            {
                if (!WalletService.IsStarted)
                    Observable.Start(() => WalletService.StartWalletWithWalletId(), RxApp.TaskpoolScheduler);

                return;
            }

            string rawMnemonic = Services.WalletService.GetNewMnemonic(WalletService.GetWordListLanguage(), GetWordCount());

            WalletService.Logger.Information($"Wallet generated a new mnemonic, mnemonic: {rawMnemonic}");

            SecureStorageService.SetMnemonic(rawMnemonic);

            WalletService.Logger.Information("Saved mnemonic to secure storage.");
        }

        int GetWordCount()
        {
            // TODO This should read from the user's preference eventually.
            int wordCount = 12;

            WalletService.Logger.Information($"Word count is {wordCount}");

            return wordCount;
        }
    }
}
