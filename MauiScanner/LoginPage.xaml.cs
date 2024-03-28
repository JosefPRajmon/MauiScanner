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
        HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync("https://www.as4u.cz/mobile/json.php?akce=login&name="+ username.Text+"&pass="+ password.Text);
        string responseS = await response.Content.ReadAsStringAsync();
        loginResponse responseO = JsonSerializer.Deserialize<loginResponse>(responseS);
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
            test.Text = "Nepovedlo se p�ihl�sit";
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
