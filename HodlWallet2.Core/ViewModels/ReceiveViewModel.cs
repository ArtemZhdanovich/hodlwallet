using System.Threading.Tasks;
using HodlWallet2.Core.Interfaces;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using Xamarin.Essentials;

namespace HodlWallet2.Core.ViewModels
{
    public class ReceiveViewModel : BaseViewModel
    {
        readonly IWalletService _WalletService;
        string _Address;
        readonly Serilog.ILogger _Logger;

        public string ShareButtonText => "Share";
        public string RequestAmountButtonText => "Request Amount";

        public IMvxCommand ShowFaqCommand { get; }
        public IMvxCommand ShareButtonCommand { get; }
        public IMvxAsyncCommand CopyAddressCommand { get; }

        public string Address
        {
            get => _Address;
            set => SetProperty(ref _Address, value);
        }

        public ReceiveViewModel(
            IMvxLogProvider logProvider, 
            IMvxNavigationService navigationService,
            IWalletService walletService) : base(logProvider, navigationService)
        {
            _Logger = walletService.Logger;
            _WalletService = walletService;

            ShowFaqCommand = new MvxCommand(ShowFaq);
            CopyAddressCommand = new MvxAsyncCommand(ToClipboard);
            ShareButtonCommand = new MvxCommand(ShowShareIntent);
        }

        void ShowFaq()
        {
            //TODO: Implement FAQ
            throw new System.NotImplementedException();
        }

        async Task ToClipboard()
        {
            await Clipboard.SetTextAsync(Address);
        }

        void ShowShareIntent()
        {
            var sharer = Xamarin.Forms.DependencyService.Get<IShareIntent>();
            sharer.QRTextShareIntent(Address);
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();

            Address = _WalletService.GetReceiveAddress().Address;

            _Logger.Debug("New Receive Address: {0}", Address);
        }
    }
}