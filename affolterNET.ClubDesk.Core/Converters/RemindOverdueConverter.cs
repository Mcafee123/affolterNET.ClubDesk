using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace affolterNET.ClubDesk.Core.Converters;

// Doppelte Verneinung korrigiert
public class RemindOverdueConverter: DefaultTypeConverter
{
    public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        return text == "Nein";
    }

    public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
    {
        if (value is bool b && b)
        {
            return "Nein";
        }

        return string.Empty;
    }
}