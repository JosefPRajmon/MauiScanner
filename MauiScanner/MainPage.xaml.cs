using Camera.MAUI;
using Camera.MAUI.ZXing;
using MauiScanner.Login;
using MauiScanner.Scanned;
using System.Net.Http.Json;
namespace MauiScanner
{

    public partial class MainPage : ContentPage
    {
        //private readonly LocalDbService _dbService;
        private bool open;
        private LoginClass _loginClass;
        private OnlineCheckClass onlineCheckClass;
        private ScannedResponseClass scannedResponseClass;
        private string CardId { get; set; }
        private bool SetDecemal = false;
        private int buttonTimer = 100;
        private string KeysDictonary;
        public string checkNumber;
        private List<string> KeysListonary { get; set; }

        public MainPage( LoginClass loginClass )
        {
            InitializeComponent();
            KeysListonary = new List<string>();
            /*TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += async(s, e) => {
                await Clipboard.Default.SetTextAsync(cardId); ;
            };
            platnost.GestureRecognizers.Add(tapGestureRecognizer);*/
            _loginClass = loginClass;

            // Skrytí horní navigaèní lišty
            NavigationPage.SetHasNavigationBar( this, false );

            onlineCheckClass = new OnlineCheckClass();
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

        /// <summary>
        /// methot for seting samera after loading xaml page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cameraView_CamerasLoaded( object sender, EventArgs e )
        {
            var b = imageTitle.Height;
            try
            {
                cameraView.Camera = cameraView.Cameras[ 0 ];
                MainThread.BeginInvokeOnMainThread( async () =>
                {
                    try
                    {
                        await cameraView.StopCameraAsync();

                    }
                    catch( Exception )
                    {
                    }
                    await cameraView.StartCameraAsync();
                } );

            }
            catch( Exception )
            {

            }
        }

        /// <summary>
        /// Methot aktivate when mobile cam detected barcode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void cameraView_BarcodeDetected( object sender, Camera.MAUI.ZXingHelper.BarcodeEventArgs args )
        {
            CheckCard( args.Result[ 0 ].Text );
        }

        /// <summary>
        /// its method to Check idcard on server as4u
        /// </summary>
        /// <param name="cardId"></param>
        private void CheckCard( string cardId )
        {
            try
            {
                if( !open )
                {
                    open = true;
                    Task.Run( () =>
                    {
                        TimeSpan vibrationLength = TimeSpan.FromSeconds( 0.5 );

                        Vibration.Default.Vibrate( vibrationLength );
                    } );
                    MainThread.BeginInvokeOnMainThread( async () =>
                    {
                        try
                        {
                            string a = cardId.Substring( cardId.Length - 1, 1 ).ToString();
                            checkNumber = a;
                            try
                            {

                                cardId = cardId.Substring( 0, 12 ).TrimStart( '0' );
                            }
                            catch( Exception )
                            {

                            }
                            CardId = cardId;
                            try
                            {
                                HttpResponseMessage scannedResponseClassResponse = await onlineCheckClass.CheckSale( await _loginClass.GetUserID(), cardId );
                                scannedResponseClass = await scannedResponseClassResponse.Content.ReadFromJsonAsync<ScannedResponseClass>();
                            }
                            catch( Exception e )
                            {
                                Application.Current.MainPage = new NavigationPage( new LoginPage( _loginClass ) );
                                Navigation.PopToRootAsync();
                                return;
                            }


                            visiblePlatnost.IsVisible = true;
                            ResponseCardClass card = scannedResponseClass.Card;
                            KeysListonary = new List<string>();
                            if( scannedResponseClass.Status == "OK" )
                            {
                                App.Current.Resources.TryGetValue( "CartInfoOkFormated", out object cartInfoText );
                                cartInfo.Text = string.Format(/*"Id karty: {0}<br>Držitel: {1}<br>Sarozen/a roku: {2}"*/(string)cartInfoText, /*card.HolderID*/CardId + checkNumber, card.HolderName, card.HolderYearBirth );
                                //cartInfo.TextType= TextType.Text;
                                ColectionViewSales.IsVisible = true;
                                foreach( var item in scannedResponseClass.Sales.Keys )
                                {
                                    scannedResponseClass.Sales[ item ].Key = item;
                                }
                                ColectionViewSales.ItemsSource = scannedResponseClass.Sales.Values;
                                Butt.IsVisible = true;
                                backgraundPlatnost.BackgroundColor = Colors.Green;
                                Kalkulacka.IsVisible = false;
                                idCardEntry.Text = string.Empty;
                                cartInfo.TextColor = Colors.White;
                                maunalyButton.IsVisible = false;
                            }
                            else if( scannedResponseClass.Status == "Error" )
                            {
                                cartInfo.Text = "Znovu se přihlašuji.\nPočkejte prosím.";
                                bool reLoged = await _loginClass.ReLogin();
                                if( reLoged )
                                {
                                    open = false;
                                    CheckCard( cardId );
                                    return;
                                }
                            }
                            else
                            {
                                App.Current.Resources.TryGetValue( "CartInfoErrorFormated", out object cartInfoErrorFormated );
                                cartInfo.Text = string.Format( (string)cartInfoErrorFormated, scannedResponseClass.Infotext, cardId + checkNumber );
                                ColectionViewSales.IsVisible = false;
                                cartInfo.TextColor = Colors.White;
                                backgraundPlatnost.BackgroundColor = Colors.Red;
                                Butt.IsVisible = false;
                                entryNum.IsVisible = false;
                                cena.IsVisible = false;
                                Kalkulacka.IsVisible = true;
                                IdCardLayour.IsVisible = true;
                                cenaKalkulacka.IsVisible = false;
                                entryNum.Text = string.Empty;
                                decimalOrSend.Source = "send_icon.png";
                                maunalyButton.IsVisible = false;
                                //idCardEntry.Text = string.Empty;
                            }

                            try
                            {
                                await cameraView.StopCameraAsync();

                            }
                            catch( Exception e )
                            {
                            }
                            try
                            {

                                await cameraView.StartCameraAsync();
                            }
                            catch( Exception e )
                            {

                            }
                            await Task.Delay( 2000 ); // Prodleva 2 sekundu
                            open = false;
                        }
                        catch( Exception )
                        {
                            open = false;
                        }
                    } );
                }
            }
            catch( Exception )
            {
                open = false;
            }
        }


        /// <summary>
        /// Persion carculate
        /// </summary>
        private void Vypocet_Ceny()
        {
            try
            {
                double puvCena = double.Parse( entryNum.Text );
                double percent = puvCena / 100;
                //string price = $"{puvCena - (percent * double.Parse(scannedResponseClass.sale)):C}";
                //cena.Text = price.Split("K")[0];
                //KeyboardTausend();
            }
            catch( Exception )
            {
                cena.Text = string.Empty;
            }
        }


        /// <summary>
        /// function for buttont to send selected Sales on server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Use_Sale( object sender, EventArgs e )
        {
            HapticFeedback.Default.Perform( HapticFeedbackType.Click );
            KeysDictonary = "";
            foreach( var item in KeysListonary )
            {
                KeysDictonary += $"{item},";
            }
            KeysDictonary = KeysDictonary.TrimEnd( ',' );

            if( KeysDictonary.Length < 1 )
            {
                App.Current.Resources.TryGetValue( "ZeroSale", out object Error );
                cartInfo.Text = (string)Error;
                cartInfo.TextColor = Colors.White;
                backgraundPlatnost.BackgroundColor = Colors.Red;
                return;
            }

            HttpResponseMessage scannedResponseClassResponse = await onlineCheckClass.UseSale( await _loginClass.GetUserID(), CardId, KeysDictonary );
            ScannedResponseClass result = await scannedResponseClassResponse.Content.ReadFromJsonAsync<ScannedResponseClass>();
            Butt.IsVisible = false;
            entryNum.Text = string.Empty;
            entryNum.IsVisible = false;
            cena.IsVisible = false;
            Kalkulacka.IsVisible = false;
            ColectionViewSales.IsVisible = false;
            string responseSales = "";
            foreach( var item in KeysListonary )
            {
                ResponseSaleClass sale = result.Sales[ item ];
                responseSales += string.Format( "{0}: <br>{1}<br><br>", sale.SaleName, sale.InfoText );
            }
            responseSales = responseSales.Substring( 0, responseSales.LastIndexOf( "<br><br>" ) );
            responseSales = responseSales.Replace( "<br>", "\n" );

            cartInfo.Text = responseSales;
            cartInfo.TextColor = Color.FromHex( "#B3FFFFFF" );
            backgraundPlatnost.BackgroundColor = Color.FromHex( "#006da4" );
            maunalyButton.IsVisible = false;//true
            Kalkulacka.IsVisible = true;
            visiblePlatnost.IsVisible = true;
            idCardEntry.Text = string.Empty;
        }

        /// <summary>
        /// function to formate my Numbers to Money value
        /// </summary>
        /// <param name="entery"></param>
        /// <param name="add"></param>
        /// <param name="button"></param>
        /// <returns></returns>
        private string FormaterEntery( string entery, int add, Button button )
        {

            string addS = $"{add}";
            if( add == -1 )
            {
                addS = "";
            }
            if( SetDecemal )
            {
                double doublee = double.Parse( $"{entery}{addS}" );
                string result = $"{doublee:C}".Split( "K" )[ 0 ].Trim();
                if( result[ result.Length - 1 ] == "0"[ 0 ] )
                {
                    result = result.Remove( result.Length - 1 );
                }
                return result;
            }
            else
            {
                double doublee = 0.00;
                string result = "";
                var parts = entery.Split( "," );
                if( entery.Contains( "," ) && String.IsNullOrEmpty( parts[ 1 ] ) )
                {
                    addS = ""; parts[ 1 ] = $"{add}";
                }
                try
                {
                    doublee = double.Parse( $"{parts[ 0 ]}{addS},{parts[ 1 ]}" );
                    result = $"{doublee:C}".Split( 'K' )[ 0 ].Trim();
                    if( result[ result.Length - 1 ] == "0"[ 0 ] )
                    {
                        result = result.Remove( result.Length - 1 );
                    }
                }
                catch( Exception )
                {
                    doublee = double.Parse( $"{parts[ 0 ]}{addS}" );
                    result = $"{doublee:C}".Split( ',' )[ 0 ].Trim();
                }

                return result;

            }
        }

        /// <summary>
        /// universal method, to chan symulate button and edit text in seted labels
        /// </summary>
        /// <param name="value"></param>
        /// <param name="button"></param>
        public void EditIdCart( int value, Button button )
        {
            HapticFeedback.Default.Perform( HapticFeedbackType.Click );
            button.BackgroundColor = Color.FromHex( "#006da4" );
            MainThread.BeginInvokeOnMainThread( async () =>
            {
                await Task.Delay( buttonTimer );
                button.BackgroundColor = Colors.Gray;
            } );
            if( cenaKalkulacka.IsVisible )
            {
                entryNum.Text = FormaterEntery( entryNum.Text, value, button );
                Vypocet_Ceny();
            }
            else
            {
                idCardEntry.Text = idCardEntry.Text.Trim() + $"{value}";
            }
        }


        private void Keyboard_0( object sender, EventArgs e )
        {
            EditIdCart( 0, (Button)sender );
        }


        private void Keyboard_1( object sender, EventArgs e )
        {
            EditIdCart( 1, (Button)sender );
        }


        private void Keyboard_2( object sender, EventArgs e )
        {
            EditIdCart( 2, (Button)sender );
        }


        private void Keyboard_3( object sender, EventArgs e )
        {
            EditIdCart( 3, (Button)sender );
        }


        private void Keyboard_4( object sender, EventArgs e )
        {
            EditIdCart( 4, (Button)sender );
        }


        private void Keyboard_5( object sender, EventArgs e )
        {
            EditIdCart( 5, (Button)sender );
        }


        private void Keyboard_6( object sender, EventArgs e )
        {
            EditIdCart( 6, (Button)sender );
        }


        private void Keyboard_7( object sender, EventArgs e )
        {
            EditIdCart( 7, (Button)sender );
        }


        private void Keyboard_8( object sender, EventArgs e )
        {
            EditIdCart( 8, (Button)sender );
        }


        private void Keyboard_9( object sender, EventArgs e )
        {
            EditIdCart( 9, (Button)sender );
        }
        public bool backClicked = false;


        private void Keyboard_back( object sender, EventArgs e )
        {
            Button button = (Button)sender;
            button.BackgroundColor = Color.FromHex( "#006da4" );
            MainThread.BeginInvokeOnMainThread( async () =>
                    {
                        await Task.Delay( buttonTimer );
                        button.BackgroundColor = Colors.Gray;
                    } );
            HapticFeedback.Default.Perform( HapticFeedbackType.Click );
            if( cenaKalkulacka.IsVisible )
            {
                if( !backClicked )
                {
                    backClicked = true;
                    entryNum.Text = entryNum.Text.Length > 0 ? entryNum.Text.Substring( 0, entryNum.Text.Length - 1 ) : string.Empty;
                    if( !entryNum.Text.Contains( "," ) )
                    {
                        SetDecemal = false;
                    }


                    Vypocet_Ceny();
                    backClicked = false;
                }
            }
            else
            {
                idCardEntry.Text = idCardEntry.Text.Length > 0 ? idCardEntry.Text.Substring( 0, idCardEntry.Text.Length - 1 ) : string.Empty;
            }
        }


        private void Keyboard_Decimal( object sender, EventArgs e )
        {

            HapticFeedback.Default.Perform( HapticFeedbackType.Click );
            ImageButton button = (ImageButton)sender;
            button.BackgroundColor = Color.FromHex( "#006da4" );
            MainThread.BeginInvokeOnMainThread( async () =>
            {
                await Task.Delay( buttonTimer );
                button.BackgroundColor = Colors.Green;
            } );
            if( IdCardLayour.IsVisible )
            {
                string checkedIdString = idCardEntry.Text;
                if( checkedIdString.Length > 3 )
                {
                    if( checkedIdString.Length < 13 )
                    {
                        checkedIdString = checkedIdString.PadLeft( 13, '0' );
                    }
                    int checker = int.Parse( checkedIdString.Substring( 12, 1 ) );
                    string toCheck = checkedIdString.Substring( 0, 12 );
                    checkNumber = toCheck;
                    List<int> evenPositions = new List<int>();//sudy
                    List<int> oddPositions = new List<int>();//lichy
                    int even = 0; int odd = 0;
                    for( int i = 0; i < toCheck.Length; i++ )
                    {
                        // Pozice jsou indexovány od 0, takže sudé pozice jsou na lichých indexech
                        if( i % 2 == 0 )
                        {
                            oddPositions.Add( int.Parse( toCheck[ i ].ToString() ) );
                        }
                        else
                        {
                            evenPositions.Add( int.Parse( toCheck[ i ].ToString() ) );
                        }
                    }
                    foreach( var item in evenPositions )
                    {
                        even = even + item;
                    }
                    even = even * 3;
                    foreach( var item in oddPositions )
                    {
                        odd = odd + item;
                    }
                    if( 10 - ( ( even + odd ) % 10 ) == checker )
                    {
                        CheckCard( checkedIdString );
                    }
                    else
                    {
                        CheckCard( idCardEntry.Text );
                    }
                }
            }
            else
            {
                string symbol = ",";
                if( !entryNum.Text.Contains( symbol ) )
                {
                    if( entryNum.Text.Length == 0 )
                    {
                        entryNum.Text = "0";
                    }
                    entryNum.Text += symbol;
                    SetDecemal = true;
                }

                Vypocet_Ceny();
            }

        }
        /*
        private void KeyboardTausend()
        {
            cena.Text = $"{cena.Text:C}";
        }
        public string Reverse( string text )
        {
            char[] cArray = text.ToCharArray();
            string reverse = String.Empty;
            for( int i = cArray.Length - 1; i > -1; i-- )
            {
                reverse += cArray[ i ];
            }
            return reverse;
        }*/
        /// <summary>
        /// Function tu button
        /// This function default screen of application 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetButton( object sender, EventArgs e )
        {

            HapticFeedback.Default.Perform( HapticFeedbackType.Click );
            CardId = "";
            visiblePlatnost.IsVisible = false;
            visiblePlatnost.IsVisible = false;
            cena.IsVisible = false;
            entryNum.IsVisible = false;
            Kalkulacka.IsVisible = true;
            Butt.IsVisible = false;
            ColectionViewSales.IsVisible = false;
            maunalyButton.IsVisible = false;//true;
            idCardEntry.Text = string.Empty;

        }

        /// <summary>
        /// function to select of collectionView
        /// When i select items this item functtion add selected item to List bycause e,currentSelected no removing all items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColectionViewSales_SelectionChanged( object sender, SelectionChangedEventArgs e )
        {

            var added = e.CurrentSelection.Except( e.PreviousSelection ).ToList();
            var removed = e.PreviousSelection.Except( e.CurrentSelection ).ToList();

            if( removed.Count == 0 )
            {
                foreach( ResponseSaleClass item in added )
                {
                    if( item.Status != "error" )
                    {

                        KeysListonary.Add( item.Key );
                    }
                }
            }
            else
            {
                foreach( ResponseSaleClass item in removed )
                {
                    if( item.Status != "error" )
                    {
                        KeysListonary.Remove( item.Key );
                    }
                }

            }
        }

        /// <summary>
        /// this function show wmanualy writer to id Cart 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManualyAddCardId( object sender, EventArgs e )
        {
            maunalyButton.IsVisible = false;
            idCardEntry.Text = string.Empty;
            Kalkulacka.IsVisible = true;
            IdCardLayour.IsVisible = true;

            cenaKalkulacka.IsVisible = false;
            ColectionViewSales.IsVisible = false;
            Butt.IsVisible = false;
        }
        /// <summary>
        /// logOut funcion remove password and Xuser and set login page to mainPage and finaly go LoginPage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void LogOut_Clicked( object sender, EventArgs e )
        {
            try
            {
                UserClass user = await _loginClass.GetUser();
                user.Password = string.Empty;
                user.XUser = string.Empty;
                user.Workshop = string.Empty;
                user.Companie = string.Empty;
                await _loginClass.Update( user );
                try
                {
                    Application.Current.MainPage = new NavigationPage( new LoginPage( _loginClass ) );
                }
                catch( Exception )
                {

                }
                try
                {
                    Navigation.PopToRootAsync();
                }
                catch( Exception )
                {

                }
            }
            catch( Exception )
            {
            }
        }

        private void imageTitle_SizeChanged( object sender, EventArgs e )
        {
            Image image = (Image)sender;
            tittleGrid.HeightRequest = image.Height;
        }
    }
}
