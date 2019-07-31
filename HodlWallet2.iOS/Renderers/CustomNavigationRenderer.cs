using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using HodlWallet2.iOS.Renderers;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(CustomNavigationRenderer))]
namespace HodlWallet2.iOS.Renderers
{
    public class CustomNavigationRenderer : NavigationRenderer
    {
        UIColor GRAY_BACKGROUND { get; } = new UIColor(red: 0.13f, green: 0.13f, blue: 0.13f, alpha: 1.0f);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Removes top border
            NavigationBar.Translucent = false;
            NavigationBar.ShadowImage = null;

            // Sets fonts to "bold" in this case of this font is pointsize + 2
            // But we should find a font that has a bold version
            var font = UIFont.FromName("Electrolize", 20);
            var descriptor =  font.FontDescriptor.CreateWithAttributes(new UIFontAttributes
            {
                Traits = new UIFontTraits() { SymbolicTrait = UIFontDescriptorSymbolicTraits.Bold }
            });
            var boldFont = UIFont.FromDescriptor(descriptor, font.PointSize + 2);

            NavigationBar.TitleTextAttributes = new UIStringAttributes()
            {
                ForegroundColor = UIColor.White,
                Font = boldFont
            };
        }
    }
}
