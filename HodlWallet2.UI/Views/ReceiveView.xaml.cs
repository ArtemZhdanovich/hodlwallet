﻿using MvvmCross.Base;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using MvvmCross.ViewModels;

using HodlWallet2.Core.Interactions;
using HodlWallet2.Core.ViewModels;
using HodlWallet2.Locale;

namespace HodlWallet2.UI.Views
{
    [MvxTabbedPagePresentation(TabbedPosition.Tab)]
    public partial class ReceiveView : MvxContentPage<ReceiveViewModel>
    {
        IMvxInteraction<DisplayAlertContent> _CopiedToClipboardInteraction = new MvxInteraction<DisplayAlertContent>();
        public IMvxInteraction<DisplayAlertContent> CopiedToClipboardInteraction
        {
            get => _CopiedToClipboardInteraction;

            set
            {
                if (_CopiedToClipboardInteraction == null)
                    _CopiedToClipboardInteraction.Requested -= CopiedToClipboardInteraction_Requested;

                _CopiedToClipboardInteraction = value;
                _CopiedToClipboardInteraction.Requested += CopiedToClipboardInteraction_Requested;
            }
        }

        public ReceiveView()
        {
            IconImageSource = "receive_tab.png";

            InitializeComponent();
        }

        protected override void OnViewModelSet()
        {
            base.OnViewModelSet();

            CreateInteractionBindings();

        }

        void CreateInteractionBindings()
        {
            var set = this.CreateBindingSet<ReceiveView, ReceiveViewModel>();

            set.Bind(this)
                .For(view => view.CopiedToClipboardInteraction)
                .To(viewModel => viewModel.CopiedToClipboardInteraction)
                .OneWay();

            set.Apply();
        }

        async void CopiedToClipboardInteraction_Requested(object sender, MvxValueEventArgs<DisplayAlertContent> e)
        {
            var displayAlertContent = e.Value;

            await DisplayAlert(
               displayAlertContent.Title, displayAlertContent.Message, displayAlertContent.Buttons[0]
           );
        }
    }
}