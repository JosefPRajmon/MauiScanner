using MauiScanner.Login;

namespace MauiScanner;

public partial class LoginPage : ContentPage
{

    private LoginClass _loginClass;
    public LoginPage()
    {
        InitializeComponent();

        _loginClass = new LoginClass();
    }

    private async void log_Clicked( object sender, EventArgs e )
    {

        List<string> responseO = await _loginClass.Login( username.Text, password.Text );
        try
        {
            if( responseO[ 0 ] != "0" )
            {
                UserClass user = new UserClass();
                user.XUser = responseO[ 0 ];
                user.UserName = username.Text;
                user.Password = _loginClass.CreateMD5( password.Text );
                await _loginClass.Create( user );
                await Navigation.PopModalAsync();
            }
            else
            {
                test.Text = responseO[ 1 ];
            }
        }
        catch( Exception )
        {

            App.Current.Resources.TryGetValue( "LoginFail", out object loginFail );
            test.Text = (string)loginFail;
        }
    }
}

