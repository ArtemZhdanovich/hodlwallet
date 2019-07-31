using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Serilog;

using HodlWallet2.Core.Services;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace HodlWallet2.UI
{
    public partial class App : Application
    {
        readonly WalletService _WalletService;
        readonly ILogger _Logger;

        public App()
        {
            InitializeComponent();

            _WalletService = WalletService.Instance;
            _Logger = _WalletService.Logger;
        }

        protected override async void OnStart()
        {
            _Logger.Information($"App started at: {DateTime.Now}");
        }

        protected override void OnSleep()
        {
            _Logger.Information($"App sleept at: {DateTime.Now}");
        }

        protected override void OnResume()
        {
            _Logger.Information($"App resumed at: {DateTime.Now}");
        }
    }
}
