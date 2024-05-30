using System.Globalization;

namespace MauiScanner
{
    public class StatusToColorConverter : IValueConverter
    {
        private string hexcolor = "#D3D3D3";
        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            string status = (string)value;
            if( status == "error" )
            {
                return Color.FromHex( hexcolor );//Colors.Gray;
            }
            else
            {
                return Colors.White;
            }
        }


        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }
    public class StatusToColorConverterClicked : IValueConverter
    {
        private string hexcolor = "#D3D3D3";
        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            string status = (string)value;
            if( status == "error" )
            {
                return Color.FromHex( hexcolor );//return Colors.Gray;
            }
            else
            {
                return Color.FromHex( "#006da6" );
            }
        }


        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }
    public class StatusToColorConverterClickedText : IValueConverter
    {

        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            string status = (string)value;
            if( status == "error" )
            {
                return Colors.Black;
            }
            else
            {
                return Colors.White;
            }
        }


        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }
    public class StatusToColorConverterLoginClickedText : IValueConverter
    {

        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            string status = (string)value;
            if( status == "error" )
            {
                return Colors.Black;
            }
            else
            {
                return Colors.White;
            }
        }


        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }
}
