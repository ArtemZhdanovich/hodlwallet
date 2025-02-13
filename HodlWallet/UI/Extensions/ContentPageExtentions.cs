﻿//
// ContentPageExtentions.cs
//
// Author:
//       Igor Guerrero <igorgue@protonmail.com>
//
// Copyright (c) 2019 
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
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Rg.Plugins.Popup.Extensions;

using Liviano.Utilities;
using HodlWallet.UI.Controls;
using System.Diagnostics;

namespace HodlWallet.UI.Extensions
{
    public static class ContentPageExtentions
    {
        const int TOAST_DURATION_MS = 2500;

        public static async Task DisplayToast(this ContentPage view, string message)
        {
            Guard.NotNull(message, nameof(message));
            Guard.NotEmpty(message, nameof(message));

            var res = Application.Current.Resources;
            var bg = (Color)res["Bg2"];
            var fg = (Color)res["Fg2"];
            var fontFamily = (string)res["Sans-Bold"];
            var font = Font.OfSize(fontFamily, Device.GetNamedSize(NamedSize.Medium, typeof(Label)));

            var toastOptions = new ToastOptions
            {
                BackgroundColor = bg,
                MessageOptions = new MessageOptions
                {
                    Font = font,
                    Foreground = fg,
                    Message = message
                },
                Duration = TimeSpan.FromMilliseconds(TOAST_DURATION_MS),
                CornerRadius = 5
            };

            try
            {
                await view.DisplayToastAsync(toastOptions);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DisplayToast] Failed to show toast, error: {ex}");
            }
        }

        public static async Task<bool> DisplayPrompt(this ContentPage view, string title, string message = null, string okButton = null, string cancelButton = null)
        {
            Guard.NotNull(title, nameof(title));
            Guard.NotEmpty(title, nameof(title));

            var promptTaskSource = new TaskCompletionSource<bool>();
            var prompt = new PromptView(title, message, okButton, cancelButton);

            await view.Navigation.PushPopupAsync(prompt);

            prompt.Responded += (object sender, bool res) =>
            {
                promptTaskSource.SetResult(res);
            };

            return await promptTaskSource.Task;
        }
    }
}
