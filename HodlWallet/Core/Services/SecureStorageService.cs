﻿//
// SecureStorageService.cs
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
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Liviano.Bips;
using Liviano.Exceptions;
using Liviano.Utilities;
using Newtonsoft.Json;
#if !DEBUG
using Xamarin.Essentials;
#endif

using HodlWallet.Core.Utils;

namespace HodlWallet.Core.Services
{
#if DEBUG
    public static class DebugStorage
    {
        static string AppPath => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        static string DbFile => Path.Combine(AppPath, "storage.debug.json");

        static readonly object @lock = new();

        static Dictionary<string, string> Data = new();

        public static string Get(string key)
        {
            var json = "{}";
            if (File.Exists(DbFile))
            {
                json = File.ReadAllText(DbFile);

                if (string.IsNullOrEmpty(json)) json = "{}";
            }

            lock (@lock) Data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            if (Data.ContainsKey(key)) return Data[key];
            return null;
        }

        public static void Set(string key, string val)
        {
            lock (@lock)
            {
                Data[key] = val;

                var json = JsonConvert.SerializeObject(Data);
                File.WriteAllText(DbFile, json);
            }
        }

        public static void RemoveAll()
        {
            lock (@lock) Data.Clear();

            File.Delete(DbFile);
        }
    }
#endif

    public static class SecureStorageService
    {
        const string WALLET_ID_KEY = "wallet-id";
        const string PIN_KEY = "pin";
        const string PASSWORD_KEY = "password";
        const string MNEMONIC_KEY = "mnemonic";
        const string NETWORK_KEY = "network";
        const string DISPLAY_CURRENCY_TYPE_KEY = "display-currency-type";
        const string FIAT_CURRENCY_CODE_KEY = "fiat-currency-code";
        const string BITCOIN_CURRENCY_CODE_KEY = "bitcoin-currency-code";

        /*
         * Key string format to identify the color bellowing to an account:
         * ACCOUNT_COLOR_PREFIX_KEY + WALLET_ID + account_name
         */
        const string ACCOUNT_COLOR_PREFIX_KEY = "account:";
        const string ACCOUNT_COLOR_SEPARATOR_KEY = ":";

        public static string GetWalletId()
        {
            return Get(WALLET_ID_KEY);
        }

        public static bool HasWalletId()
        {
            return !string.IsNullOrEmpty(GetWalletId());
        }

        public static void SetWalletId(string walletId)
        {
            Set(WALLET_ID_KEY, walletId);
        }

        public static string GetMnemonic()
        {
            return Get(MNEMONIC_KEY);
        }

        public static bool HasMnemonic()
        {
            return !string.IsNullOrEmpty(GetMnemonic());
        }

        public static void SetMnemonic(string mnemonic)
        {
            Guard.NotEmpty(mnemonic, nameof(mnemonic));

            if (!Hd.IsValidChecksum(mnemonic))
                throw new WalletException("Invalid mnemonic the checksum wasn't validated");

            Set(MNEMONIC_KEY, mnemonic);
        }

        public static string GetPin()
        {
            return Get(PIN_KEY);
        }

        public static bool HasPin()
        {
            return !string.IsNullOrEmpty(GetPin());
        }

        public static void SetPin(string pin)
        {
            Guard.NotEmpty(pin, nameof(pin));

            if (!pin.All(char.IsDigit))
                throw new WalletException($"Unable to set pin: '{pin}' includes charactes that aren't digits");

            if (pin.Length != 6)
                throw new WalletException($"Unable to set pin: '{pin}' it must be 6 digits long");

            Set(PIN_KEY, pin);
        }

        public static string GetPassword()
        {
            return Get(PASSWORD_KEY);
        }

        public static bool HasPassword()
        {
            return !string.IsNullOrEmpty(GetPassword());
        }

        public static void SetPassword(string password)
        {
            Guard.NotEmpty(password, nameof(password));

            Set(PASSWORD_KEY, password);
        }

        public static string GetNetwork()
        {
            return Get(NETWORK_KEY);
        }

        public static bool HasNetwork()
        {
            return !string.IsNullOrEmpty(GetNetwork());
        }

        public static void SetNetwork(string network)
        {
            Set(NETWORK_KEY, network);
        }

        public static string GetAccountColor(string walletId, string accountId)
        {
            Guard.NotEmpty(walletId, nameof(walletId));
            Guard.NotEmpty(accountId, nameof(accountId));
            string accountColorKey = $"{ACCOUNT_COLOR_PREFIX_KEY}{walletId}{ACCOUNT_COLOR_SEPARATOR_KEY}{accountId}";
            return Get(accountColorKey);
        }

        public static void SetAccountColor(string walletId, string accountId, string color)
        {
            Guard.NotEmpty(walletId, nameof(walletId));
            Guard.NotEmpty(accountId, nameof(accountId));
            Guard.NotEmpty(color, nameof(color));
            string accountColorKey = $"{ACCOUNT_COLOR_PREFIX_KEY}{walletId}{ACCOUNT_COLOR_SEPARATOR_KEY}{accountId}";
            Set(accountColorKey, color);
        }

        public static string GetFiatCurrencyCode()
        {
            string currentCode = Get(FIAT_CURRENCY_CODE_KEY);
            return !string.IsNullOrEmpty(currentCode) ? currentCode : Constants.DEFAULT_FIAT_CURRENCY_CODE;
        }

        public static void SetFiatCurrencyCode(string currencyCode)
        {
            Set(FIAT_CURRENCY_CODE_KEY, currencyCode);
        }

        public static DisplayCurrencyType GetDisplayCurrencyType()
        {
            return Get(DISPLAY_CURRENCY_TYPE_KEY) switch
            {
                "bitcoin" => DisplayCurrencyType.Bitcoin,
                "fiat" => DisplayCurrencyType.Fiat,
                _ => DisplayCurrencyType.Bitcoin,
            };
        }

        public static void SetBitcoinCurrencyCode(string currencyCode)
        {
            if (!currencyCode.Equals("BTC") && !currencyCode.Equals("SAT"))
                throw new ArgumentException($"Invalid currency code: {currencyCode} only BTC and SAT are allowed");

            Set(BITCOIN_CURRENCY_CODE_KEY, currencyCode);
        }

        public static void SetDisplayCurrencyType(DisplayCurrencyType displayCurrencyType)
        {
            switch (displayCurrencyType)
            {
                case DisplayCurrencyType.Bitcoin:
                    Set(DISPLAY_CURRENCY_TYPE_KEY, "bitcoin");
                    break;
                case DisplayCurrencyType.Fiat:
                    Set(DISPLAY_CURRENCY_TYPE_KEY, "fiat");
                    break;
                default:
                    Set(DISPLAY_CURRENCY_TYPE_KEY, "bitcoin");
                    break;
            }
        }

        public static string GetBitcoinCurrencyCode()
        {
            var code = Get(BITCOIN_CURRENCY_CODE_KEY);

            if (string.IsNullOrEmpty(code)) return "BTC";

            return code;
        }

        public static string GetNote(string txId)
        {
            return Get($"note:{txId}");
        }

        public static void SetNote(string txId, string content)
        {
            Set($"note:{txId}", content);
        }

        public static bool UserDidSetup()
        {
            return HasPin() && HasMnemonic() && HasWalletId() && HasNetwork();
        }

        public static void RemoveAll()
        {
#if DEBUG
            DebugStorage.RemoveAll();
#else
            SecureStorage.RemoveAll();
#endif
        }

        static string Get(string key)
        {
#if DEBUG
            return DebugStorage.Get(key);
#else
            return SecureStorage.GetAsync(key).Result;
#endif
        }

        static void Set(string key, string val)
        {
#if DEBUG
            DebugStorage.Set(key, val);
#else
            SecureStorage.SetAsync(key, val);
#endif
        }
    }
}
