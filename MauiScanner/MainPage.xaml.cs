using Camera.MAUI;
using Camera.MAUI.ZXing;
using MauiScanner.Login;
using System.Threading.Tasks;
namespace MauiScanner
{
    public partial class MainPage : ContentPage
    {
        private readonly LocalDbService _dbService;
        private bool open;
        private LoginClass _loginClass;
        public MainPage(LocalDbService dbService)
        {
            InitializeComponent();
            _loginClass = new LoginClass();
            Task.Run(async () =>
            {
                Boolean log =await _loginClass.IsLoggedIn();
                if (!log)
                {
                   await Navigation.PushModalAsync(new LoginPage());
                    _loginClass = new LoginClass();
                }
                
                LoadCam();
            });
            

            _dbService = dbService;
                cameraView.BarCodeDecoder = new ZXingBarcodeDecoder();
                cameraView.BarCodeOptions = new BarcodeDecodeOptions
                {
                    AutoRotate = true,
                    PossibleFormats = { BarcodeFormat.EAN_13 },
                    ReadMultipleCodes = false,
                    TryHarder = true,
                    TryInverted = true
                };

                open = false;

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

        }
        private void LoadCam()
        {
            try
            {
                try
                {

                test.Text = _loginClass.GetUserID();
                cameraView.HeightRequest = 300;
                cameraView.WidthRequest = 300;
                }
                catch (Exception)
                {

                }/*
                try
                {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await cameraView.StopCameraAsync();
                    await cameraView.StartCameraAsync();
                });

                }
                catch (Exception)
                {
                }*/

            }
            catch (Exception)
            {

            }
        }
        private void cameraView_CamerasLoaded(object sender, EventArgs e)
        {
            try
            {
            cameraView.Camera = cameraView.Cameras[0];
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    await cameraView.StopCameraAsync();

                }
                catch (Exception)
                {
                }
                await cameraView.StartCameraAsync();
            });

            }
            catch (Exception)
            {

            }
        }

        private void cameraView_BarcodeDetected(object sender, Camera.MAUI.ZXingHelper.BarcodeEventArgs args)
        {
            if (!open)
            {
                open = true;
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Navigation.PushAsync(new ResultPage(args.Result[0].Text, _dbService));
                    try
                    {
                        await cameraView.StopCameraAsync();

                    }
                    catch (Exception)
                    {
                    }
                    await cameraView.StartCameraAsync(); 
                    await Task.Delay(1000); // Prodleva 1 sekundu
                    open = false;
                }); 
            }

        }

        private void DatabaseButton_Clicked(object sender, EventArgs e)
        {
            if (!open)
            {
                open = true;
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Navigation.PushAsync(new DatabasePage(_dbService));
                    try
                    {
                    await cameraView.StopCameraAsync();

                    }
                    catch (Exception)
                    {
                    }
                    await cameraView.StartCameraAsync();
                    await Task.Delay(1000); // Prodleva 1 sekundu
                    open = false;
                });
            }
        }
    }
}
