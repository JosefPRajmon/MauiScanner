using Camera.MAUI;
using Camera.MAUI.ZXing;
namespace MauiScanner
{
    public partial class MainPage : ContentPage
    {
        private readonly LocalDbService _dbService;
        private bool open;
        public MainPage(LocalDbService dbService)
        {
            InitializeComponent();
            cameraView.BarCodeDecoder = new ZXingBarcodeDecoder();
            cameraView.BarCodeOptions = new BarcodeDecodeOptions
            {
                AutoRotate = true,
                PossibleFormats = { BarcodeFormat.EAN_13 },
                ReadMultipleCodes = false,
                TryHarder = true,
                TryInverted = true
            };
            _dbService = dbService;

            open = false;
        }

        private void cameraView_CamerasLoaded(object sender, EventArgs e)
        {
            cameraView.Camera = cameraView.Cameras[0];
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await cameraView.StopCameraAsync();
                await cameraView.StartCameraAsync();
            });
        }

        private void cameraView_BarcodeDetected(object sender, Camera.MAUI.ZXingHelper.BarcodeEventArgs args)
        {
            if (!open)
            {
                open = true;
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Navigation.PushAsync(new ResultPage(args.Result[0].Text, _dbService));
                    await cameraView.StopCameraAsync();
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
                    await cameraView.StopCameraAsync();
                    await cameraView.StartCameraAsync();
                    await Task.Delay(1000); // Prodleva 1 sekundu
                    open = false;
                });
            }
        }
    }
}
