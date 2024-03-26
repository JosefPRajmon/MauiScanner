namespace MauiScanner;

public partial class DatabasePage : ContentPage
{
    private readonly LocalDbService _dbService;
    private string _editCustomerId;
    public DatabasePage(LocalDbService dbService)
	{
		InitializeComponent();
        _dbService = dbService;
        _editCustomerId = "0";
        Task.Run(async () => listView.ItemsSource = await _dbService.GetCustomers());
    }
    private async void saveButton_Clicked(object sender, EventArgs e)
    {
        if ( _editCustomerId == "0")
        {
            //add customer
            await _dbService.Create(new Customer
            {
                Name = nameEntryField.Text,
                Id = MobileEntryField.Text
            });
        }
        else
        {
            //edit customer
            await _dbService.Update(new Customer
            {
                Id = MobileEntryField.Text,//_editCustomerId,
                Name = nameEntryField.Text,
                //Mobile = MobileEntryField.Text
            });

            _editCustomerId = "0";
        }

        nameEntryField.Text = string.Empty;
        MobileEntryField.Text = string.Empty;
        listView.ItemsSource = await _dbService.GetCustomers();
    }

    private async void listView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        var customer = (Customer)e.Item;
        var action = await DisplayActionSheet("Action", "Cancel", null, "Edit", "Delete");
        switch (action)
        {
            case "Edit":
                _editCustomerId = customer.Id;
                nameEntryField.Text = customer.Name;
                MobileEntryField.Text = customer.Id;
                break;
            case "Delete":
                await _dbService.Delete(customer);
                listView.ItemsSource = await _dbService.GetCustomers();
                break;
        }
    }
}