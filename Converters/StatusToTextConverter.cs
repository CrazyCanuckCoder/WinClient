using System.Globalization;
using System.Windows.Data;
using WinClient.Logic;

namespace WinClient.Converters;

public class StatusToTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ClientStatus status)
        {
            return status switch
            {
                ClientStatus.Unregistered => "Unregistered",
                ClientStatus.Registered => "Registered",
                ClientStatus.LoggedIn => "Logged In",
                ClientStatus.Registering => "Registering...",
                ClientStatus.LoggingIn => "Logging In...",
                _ => "Unknown",
            };
        }

        return "Unknown";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
