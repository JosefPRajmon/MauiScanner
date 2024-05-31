using CommunityToolkit.Maui.Core.Platform;
using MauiScanner.Login;
namespace MauiScanner;

public partial class LoginPage : ContentPage
{
    loginResponse loginRe;
    UserClass userClass;
    private LoginClass _loginClass;

    /// <summary>
    /// Create login page 
    /// </summary>
    /// <param name="loginClass"></param>
    public LoginPage( LoginClass loginClass )
    {
        InitializeComponent();

        _loginClass = loginClass;
        // Skrytí horní navigaèní lišty
        NavigationPage.SetHasNavigationBar( this, false );
#if DEBUG
        password.IsPassword = false;
#endif
    }

    /// <summary>
    /// in reloaded page check if user is loggin. oppen main page else load log out users
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();

        login.IsVisible = true;
        AllUsers.IsVisible = true;
        Task.Run( async () =>
        {
            userClass = await _loginClass.GetUser();
            try
            {
                List<UserClass> users = await _loginClass.GetUsers();

                if( userClass != null )
                {
                    //loginRe = await _loginClass.LogingChecker( userClass.UserName, userClass.Password );
                    LoginControler();

                }
                else
                {
                    MainThread.BeginInvokeOnMainThread( () =>
                    {
                        AllUsers.ItemsSource = users;
                        AllUsers.IsVisible = false;
                        login.IsVisible = true;
                    } );
                }
            }
            catch( Exception )
            {

            }
        } );
    }
    private void HideKeyboard()
    {
        username.HideKeyboardAsync( CancellationToken.None );
        password.HideKeyboardAsync( CancellationToken.None );
    }

    /// <summary>
    /// Login to as4u server and save user to tababase
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void log_Clicked( object sender, EventArgs e )
    {

        try
        {

            string nd5Pass = _loginClass.CreateMD5( password.Text );
            List<string> responseO = await _loginClass.Login( username.Text, nd5Pass );

            test.Text = string.Empty;
            try
            {
                if( responseO[ 0 ] != "0" )
                {
                    try
                    {

                        var isUser = await _loginClass.GetUser( username.Text );
                        if( isUser != null )
                        {
                            isUser.XUser = responseO[ 0 ];
                            isUser.Password = nd5Pass;
                            await _loginClass.Update( isUser );
                        }
                        else
                        {
                            UserClass user = new UserClass();
                            user.XUser = responseO[ 0 ];
                            user.UserName = username.Text;
                            user.Password = nd5Pass;
                            await _loginClass.Create( user );
                        }

                        userClass = await _loginClass.GetUser();
                        HideKeyboard();
                        LoginControler();
                    }
                    catch( Exception ex )
                    {
                        /* UserClass user = await _loginClass.GetUser( username.Text );
                         user.Password = nd5Pass;
                         user.XUser = responseO[ 0 ];
                         await _loginClass.Update( user );
                         LoginControler();*/
                    }

                }
                else
                {
                    test.Text = responseO[ 1 ];
                }
            }
            catch( Exception er )
            {

                App.Current.Resources.TryGetValue( "LoginFail", out object loginFail );
                test.Text = (string)loginFail;
            }

        }
        catch( Exception exx )
        {

        }
    }
    /// <summary>
    /// Change username to selected name when selected from logOut users
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AllUsers_SelectionChanged( object sender, SelectionChangedEventArgs e )
    {
        try
        {
            UserClass user = (UserClass)e.CurrentSelection[ 0 ];
            username.Text = user.UserName;
        }
        catch( Exception )
        {
            username.Text = string.Empty;
        }
    }

    public async void LoginControler( bool down = true )
    {
        try
        {
            if( down )
            {
                if( userClass is null )
                {
                    userClass = await _loginClass.GetUser();
                }
                loginRe = await _loginClass.LogingChecker( userClass.UserName, userClass.Password );
            }
            MainThread.BeginInvokeOnMainThread( async () =>
            {
                try
                {
                    AllUsers.IsVisible = false;
                    login.IsVisible = false;
                    if( userClass.Companie != string.Empty )
                    {
                        if( userClass.Workshop != string.Empty )
                        {
                            Application.Current.MainPage = new NavigationPage( new MainPage( _loginClass ) );
                            Navigation.PopToRootAsync();
                        }
                        else
                        {
                            var a = loginRe.Companies.Where( a => a.Key == userClass.Companie ).FirstOrDefault();
                            Dictionary<string, string> workshopDictionary = a.Value.Worksshop;
                            CompanyStack.IsVisible = true;
                            if( workshopDictionary.Count == 1 )
                            {
                                userClass.Workshop = workshopDictionary.Keys.FirstOrDefault();
                                await _loginClass.Update( userClass );
                                LoginControler( false );
                            }
                            else if( workshopDictionary.Count == 0 )
                            {
                                errorSelect.Text = "Nemáte přiřazenou zádnou provozovnu, kliknutím se odhlásíte.";
                                errorSelect.GestureRecognizers.Clear();
                                var tapGestureRecognizer = new TapGestureRecognizer();
                                tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
                                errorSelect.GestureRecognizers.Add( tapGestureRecognizer );

                                Dictionary<string, CompaniesClass> companies = loginRe.Companies;

                                Companies.ItemsSource = companies.Values;
                                

                            }
                            else if( workshopDictionary.Count > 1 )
                            {
                                Dictionary<string, CompaniesClass> companies = loginRe.Companies;
                                Companies.ItemsSource = companies.Values;
                                try
                                {
                                    if( companies.Count == 1 )
                                    {
                                        Companies.SelectedItem = companies.FirstOrDefault().Value;
                                    }
                                }
                                catch( Exception )
                                {

                                }

                            }
                            else
                            {

                                errorSelect.Text = "Prosím vyberte správnou provozovnu.";
                                errorSelect.GestureRecognizers.Clear();


                            }
                        }
                    }
                    else
                    {

                        if (loginRe.Companies is null)
                        {
                            _loginClass.LogOut();
                            login.IsVisible = true;
                            test.Text = Application.Current.Resources["ZeroCompany"].ToString();
                            return;
                        }
                        CompanyStack.IsVisible = true;
                        if( loginRe.Companies.Count > 1 )//!=1
                        {
                            Dictionary<string, CompaniesClass> companies = loginRe.Companies;
                            Companies.ItemsSource = companies.Values;
                        }
                        else if( loginRe.Companies.Count == 1 )
                        {
                            userClass.Companie = loginRe.Companies.Keys.FirstOrDefault();
                            userClass.CompanieName = loginRe.Companies.Values.FirstOrDefault().Name;
                            await _loginClass.Update( userClass );
                            LoginControler( false );

                        }
                        else
                        {
                            errorSelect.Text = "Nemáte přiřazenou zádnou společnost, kliknutím se odhlásíte.";
                            errorSelect.GestureRecognizers.Clear();
                            var tapGestureRecognizer = new TapGestureRecognizer();
                            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
                            errorSelect.GestureRecognizers.Add( tapGestureRecognizer );
                        }
                        //
                    }
                }
                catch( Exception ex )
                {
                    userClass.Companie = string.Empty;
                    loginRe = await _loginClass.LogingChecker( userClass.UserName, userClass.Password );
                    LoginControler();
                }

            } );

        }
        catch( Exception de )
        {

        }
    }

    private async void Button_Clicked( object sender, EventArgs e )
    {

        LoginControler();
    }



    private void Selectworkshop( object sender, SelectionChangedEventArgs e )
    {

        try
        {
            HapticFeedback.Default.Perform( HapticFeedbackType.Click );
            var compa = (string)Workshops.SelectedItem;

            MainThread.BeginInvokeOnMainThread( async () =>
            {
                try
                {
                    if( compa != null )
                    {
                        foreach( var item in loginRe.Companies.Values.Where( a => a.Name == userClass.CompanieName ).FirstOrDefault().Worksshop )
                        {
                            if( item.Value == compa )
                            {
                                userClass.WorkshopName = compa;
                                userClass.Workshop = item.Key;
                                await _loginClass.Update( userClass );
                                ButtonSelecter.IsVisible = true;
                            }
                        }
                    }
                    else
                    {
                        ButtonSelecter.IsVisible = false;
                    }
                }
                catch( Exception )
                {
                }

            } );

        }
        catch( Exception )
        {

        }

    }



    private async void Companies_SelectionChanged( object sender, SelectionChangedEventArgs e )
    {

        CompaniesClass companyC = (CompaniesClass)e.CurrentSelection.FirstOrDefault();
        Workshops.ItemsSource = companyC.Worksshop.Values;

        HapticFeedback.Default.Perform( HapticFeedbackType.Click );
        var compa = (CompaniesClass)Companies.SelectedItem;

        if( compa != null )
        {
            foreach( var item in loginRe.Companies )
            {
                if( item.Value.Name == compa.Name )
                {
                    tittleWorkshopDown.IsVisible = true;
                    userClass.CompanieName = compa.Name;
                    userClass.Companie = item.Key;
                    userClass.Workshop = string.Empty;
                    userClass.WorkshopName = compa.Name;
                    await _loginClass.Update( userClass );

                    if( companyC.Worksshop.Count == 1 )
                    {
                        Workshops.SelectedItem = companyC.Worksshop.FirstOrDefault().Value;
                    }
                    else
                    {
                        Workshops.SelectedItem = null;
                    }
                }
            }
        }
    }

    private async void TapGestureRecognizer_Tapped( object sender, TappedEventArgs e )
    {
        userClass.XUser = string.Empty;
        userClass.Password = string.Empty;
        await _loginClass.Update( userClass );

        AllUsers.IsVisible = true;
        login.IsVisible = true;
        CompanyStack.IsVisible = false;
    }
}

