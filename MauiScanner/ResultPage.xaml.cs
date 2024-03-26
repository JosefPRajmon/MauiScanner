namespace MauiScanner;

public partial class ResultPage : ContentPage
{
    private string IdSave;
    private readonly LocalDbService _dbService;
    public ResultPage(string result, LocalDbService dbService)
	{
		InitializeComponent();
        _dbService = dbService;
        IdSave = "";

        var tapGestureRecognizer = new TapGestureRecognizer();
        tapGestureRecognizer.Tapped += async (s, e) =>
        {
            await Clipboard.SetTextAsync(IdSave);
        };
        barcodeResult.GestureRecognizers.Add(tapGestureRecognizer);

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            Customer inDatabase = await _dbService.GetById(result);
            if (inDatabase != null)
            {
                barcodeResult.Text = $"{result} je {inDatabase.Name}";
            }
            else
            {
                barcodeResult.Text = $"{result} není v databázi";
            }
            IdSave = result;
        });
    }
}