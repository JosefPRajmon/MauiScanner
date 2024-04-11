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
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("MyCustomization", (handler, view) =>
        {
#if ANDROID
            handler.PlatformView.ShowSoftInputOnFocus = true;
#endif
        });
        _loginClass = new LoginClass();
    }

    private async void log_Clicked(object sender, EventArgs e)
    {

        List<string> responseO = await _loginClass.Login(username.Text,password.Text);
        try
        {
            if (responseO[0] !="0")
            {
                UserClass user = new UserClass();
                user.Id = responseO[0];
                user.UserName = username.Text;
                user.Password = _loginClass.CreateMD5(password.Text);
                await _loginClass.Create(user);
                await Navigation.PopModalAsync();
            }
            else
            {
                test.Text = responseO[1];
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
