using System.Globalization;

namespace TouCart.Converters;

/// <summary>Returns a font size that keeps a title within 2 lines for names up to 50 chars.</summary>
public class AdaptiveFontSizeConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var text = value as string ?? string.Empty;
        return text.Length switch
        {
            <= 20 => 28.0,
            <= 30 => 22.0,
            <= 40 => 18.0,
            _     => 15.0   // 41–50 chars
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
