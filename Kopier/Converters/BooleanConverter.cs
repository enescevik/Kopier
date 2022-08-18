using System.Globalization;

namespace Kopier.Converters;

public class BooleanConverter : IValueConverter, IMarkupExtension
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => !(bool)value;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value;

    public object ProvideValue(IServiceProvider serviceProvider) => this;
}