using MauiScanner.Login;
using System;
using System.Text.Json;

namespace MauiScanner;

public partial class LoginPage : ContentPage
{

    private LoginClass _loginClass;
    public LoginPage()
	{
		InitializeComponent();

        _loginClass = new LoginClass();
    }

    private async void log_Clicked(object sender, EventArgs e)
    {

        loginResponse responseO = await _loginClass.Login(username.Text,password.Text);
        try
        {
            if (responseO.securid.ValueKind == JsonValueKind.String)
            {
                UserClass user = new UserClass();
                user.id = responseO.securid.ToString();
                user.UserName = username.Text;
                user.Password = _loginClass.CreateMD5(password.Text);
                _loginClass.Create(user);
                Navigation.PopModalAsync();
            }
            else
            {
                test.Text = responseO.reason;
            }
        }
        catch (Exception)
        {
            test.Text = "Nepovedlo se pøihlásit";
        }
    }
}
public class loginResponse
{
    public JsonElement securid { get; set; }
    public string? reason { get; set; }
    public string? name_surname { get; set; }
    public string? email { get; set; }
    public string? telefon { get; set; }
}
