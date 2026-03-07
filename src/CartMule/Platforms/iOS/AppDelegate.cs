using Foundation;
using UIKit;

namespace CartMule
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // Change text-field cursor (caret) from blue to black app-wide
            UITextField.Appearance.TintColor = UIColor.Black;
            UITextView.Appearance.TintColor  = UIColor.Black;
            return base.FinishedLaunching(application, launchOptions);
        }
    }
}
