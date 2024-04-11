using Camera.MAUI;
using Camera.MAUI.ZXing;
using MauiScanner.Login;
using MauiScanner.Scanned;
namespace MauiScanner
{
    public partial class MainPage : ContentPage
    {
        private readonly LocalDbService _dbService;
        private bool open;
        private LoginClass _loginClass;
        private OnlineCheckClass onlineCheckClass;
        private ScannedResponseClass scannedResponseClass;
        private string cardId;
        private bool SetDecemal= false;

        public MainPage(LocalDbService dbService)
        {
            InitializeComponent();

            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("MyCustomization", (handler, view) =>
            {
#if ANDROID
        handler.PlatformView.ShowSoftInputOnFocus=false;
#endif
            });
            _loginClass = new LoginClass();
            Task.Run(async () =>
            {
                Boolean log =await _loginClass.IsLoggedIn();
                if (!log)
                {
                   await Navigation.PushModalAsync(new LoginPage());
                    _loginClass = new LoginClass();
                }
                //usePriceCheckbox.IsChecked = _loginClass._userClass.UsePrice;
            });


            onlineCheckClass = new OnlineCheckClass();
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
            try
            {
            if (!open)
            {
                open = true;
                    Task.Run(async () =>
                    {
                        TimeSpan vibrationLength = TimeSpan.FromSeconds(0.5);

                        Vibration.Default.Vibrate(vibrationLength);
                    });
                    MainThread.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        cardId = args.Result[0].Text;
                             scannedResponseClass = await onlineCheckClass.CheckSale(await _loginClass.GetUserID(), cardId);
                        visiblePlatnost.IsVisible = true;
                        if (scannedResponseClass.status == "OK")
                        {
                            platnost.TextColor = Color.FromHex("#B3FFFFFF");
                            backgraundPlatnost.BackgroundColor = Colors.Green;
                            platnost.Text = $"Držitel: <b>{scannedResponseClass.drzitel}</b><br>{scannedResponseClass.infotext}";
                        }
                        else
                        {
                            platnost.TextColor = Colors.Black;
                            backgraundPlatnost.BackgroundColor= Colors.Red;
                        }
                        if (scannedResponseClass.status == "error")
                        {
                            Butt.IsVisible = false;
                            entryNum.IsVisible = false;
                            cena.IsVisible = false;
                            Kalkulacka.IsVisible = false;
                            platnost.Text = $"{scannedResponseClass.infotext}";
                        }
                        else
                        {
                            Butt.IsVisible = true;
                            entryNum.IsVisible= true;
                            cena.IsVisible = true;
                            Kalkulacka.IsVisible = true;
                            cena.Text = string.Empty;
                            entryNum.Text = string.Empty;
                        }
                        try
                        {
                            await cameraView.StopCameraAsync();

                            }
                            catch (Exception)
                            {
                            }
                        try
                        {

                        await cameraView.StartCameraAsync(); 
                        }
                        catch (Exception)
                        {

                        }
                        await Task.Delay(1000); // Prodleva 1 sekundu
                        open = false;
                        }
                    catch (Exception e)
                    {
                    }
                }); 
            }
            }
            catch (Exception)
            {
            }


        }

        /*private void DatabaseButton_Clicked(object sender, EventArgs e)
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
        }*/

        private void Vypocet_Ceny(object sender, TextChangedEventArgs e)
        {
            try
            {
                double puvCena = double.Parse(entryNum.Text);
                double percent = puvCena / 100;
                string price = $"{puvCena - (percent * double.Parse(scannedResponseClass.sale)):C}";
                cena.Text = price.Split("K")[0];
                //KeyboardTausend();
            }
            catch (Exception)
            {
                cena.Text= string.Empty;
            }
        }

        private async void Use_Sale(object sender, EventArgs e)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            ScannedResponseClass result = await onlineCheckClass.UseSale(await _loginClass.GetUserID(), cardId);
            Butt.IsVisible = false;
            entryNum.Text = string.Empty;
            entryNum.IsVisible = false;
            cena.IsVisible = false;
            platnost.Text = result.infotext;
            platnost.TextColor = Color.FromHex("#B3FFFFFF");
            backgraundPlatnost.BackgroundColor = Colors.Black;
        }
        private string FormaterEntery(string entery,int add, Button button)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            button.BackgroundColor = Color.FromHex("#006da4");
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(500);
                button.BackgroundColor = Colors.Gray;
            });
            string addS = $"{add}";
            if (add == -1) {
                addS = "";
            }
            if (SetDecemal)
            {
                double doublee = double.Parse($"{entery}{addS}");
                string result =  $"{doublee:C}".Split("K")[0].Trim();
                if (result[result.Length - 1] == "0"[0])
                {
                    result = result.Remove(result.Length - 1);
                }
                return result;
            }
            else
            {
                double doublee = 0.00;
                string result = "";
                var parts = entery.Split(",");
                if (entery.Contains(",")&& String.IsNullOrEmpty(parts[1]))
                {
                    addS = ""; parts[1] = $"{add}";
                } try
                {
                     doublee = double.Parse($"{parts[0]}{addS},{parts[1]}");
                    result=$"{doublee:C}".Split('K')[0].Trim();
                    if (result[result.Length-1] == "0"[0])
                    {
                        result=result.Remove(result.Length - 1);
                    }
                }
                catch (Exception)
                {
                 doublee = double.Parse($"{parts[0]}{addS}");
                    result = $"{doublee:C}".Split(',')[0].Trim();
                }

                return result;

            }
        }
        private void Keyboard_0(object sender, EventArgs e)
        {

            //double a = double.Parse($"{entryNum.Text}0");
            entryNum.Text = FormaterEntery(entryNum.Text, 0,(Button)sender); //$"{a:C}".Split("K")[0];
        }

        private void Keyboard_1(object sender, EventArgs e)
        {
            entryNum.Text = FormaterEntery(entryNum.Text, 1, (Button)sender);//entryNum.Text + "1";

        }

        private void Keyboard_2(object sender, EventArgs e)
        {
            entryNum.Text = FormaterEntery(entryNum.Text, 2, (Button)sender);//entryNum.Text + "2";
        }

        private void Keyboard_3(object sender, EventArgs e)
        {
            entryNum.Text = FormaterEntery(entryNum.Text, 3, (Button)sender);//entryNum.Text + "3";
        }

        private void Keyboard_4(object sender, EventArgs e)
        {
            entryNum.Text = FormaterEntery(entryNum.Text, 4, (Button)sender);//entryNum.Text + "4";
        }

        private void Keyboard_5(object sender, EventArgs e)
        {
            entryNum.Text = FormaterEntery(entryNum.Text, 5, (Button)sender);//entryNum.Text + "5";
        }

        private void Keyboard_6(object sender, EventArgs e)
        {
            entryNum.Text = FormaterEntery(entryNum.Text, 6, (Button)sender);//entryNum.Text + "6";
        }

        private void Keyboard_7(object sender, EventArgs e)
        {
            entryNum.Text = FormaterEntery(entryNum.Text, 7, (Button)sender);//entryNum.Text + "7";
        }

        private void Keyboard_8(object sender, EventArgs e)
        {
            entryNum.Text = FormaterEntery(entryNum.Text, 8, (Button)sender);//entryNum.Text + "8";
        }

        private void Keyboard_9(object sender, EventArgs e)
        {
            entryNum.Text = FormaterEntery(entryNum.Text, 9, (Button)sender);//entryNum.Text + "9";

        }
        public bool backClicked=false;
        private void Keyboard_back(object sender, EventArgs e)
        {
            if (!backClicked)
            {
                backClicked = true;
                 HapticFeedback.Default.Perform(HapticFeedbackType.Click);
                entryNum.Text = entryNum.Text.Length > 0 ? entryNum.Text.Substring(0, entryNum.Text.Length - 1) : string.Empty;
                if (!entryNum.Text.Contains(","))
                {
                    SetDecemal = false;
                }
                Button button = (Button)sender;
                button.BackgroundColor = Color.FromHex("#006da4");
                MainThread.BeginInvokeOnMainThread(async() =>
                {
                    await Task.Delay(500);
                    button.BackgroundColor = Colors.Gray;
                });
                backClicked = false;
            }

            

        }

        private void Keyboard_Decimal(object sender, EventArgs e)
        {

            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            string symbol = ",";
            if (!entryNum.Text.Contains(symbol))
            {
                if (entryNum.Text.Length==0)
                {
                    entryNum.Text = "0";
                }
                entryNum.Text += symbol;
                SetDecemal = true;
            }
            Button button = (Button)sender;
            button.BackgroundColor = Color.FromHex("#006da4");
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(500);
                button.BackgroundColor = Colors.Gray;
            });
        }

        private void KeyboardTausend()
        {
            cena.Text = $"{cena.Text:C}";

        }
        public string Reverse(string text)
        {
            char[] cArray = text.ToCharArray();
            string reverse = String.Empty;
            for (int i = cArray.Length - 1; i > -1; i--)
            {
                reverse += cArray[i];
            }
            return reverse;
        }
        private void ResetButton(object sender, EventArgs e)
        {

            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            cardId = "";
            visiblePlatnost.IsVisible = false;
            visiblePlatnost.IsVisible = false;
            cena.IsVisible = false;
            entryNum.IsVisible = false;
            Kalkulacka.IsVisible = false;
            Butt.IsVisible = false;
        }
    }
}
