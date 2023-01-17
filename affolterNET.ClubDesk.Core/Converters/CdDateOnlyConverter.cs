using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace affolterNET.ClubDesk.Core.Converters;

public class CdDateOnlyConverter: DefaultTypeConverter
{
    public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null!;
            }
                
            return ParseDate(text);
        }
        catch
        {
            Console.WriteLine($"could not parse \"{text}\" to DateOnly");
            throw;
        }
    }

    public override string ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
    {
        if (value is DateTime dt)
        {
            return dt.ToString("dd.MM.yyyy");
        }

        return string.Empty;
    }
    
    private static DateTime ParseDate(string input)
    {
        if (!DateTime.TryParseExact(input, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var datum))
        {
            throw new InvalidOperationException($"invalid date found: {input}");
        }

        return datum;
    }
}
