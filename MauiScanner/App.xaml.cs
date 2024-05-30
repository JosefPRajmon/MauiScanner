namespace MauiScanner
{
    public partial class App : Application
    {
        public App( LoginPage mainPage )
        {
            InitializeComponent();
            MainPage = new NavigationPage( mainPage );
        }
    }
}
