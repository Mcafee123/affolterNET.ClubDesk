using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace affolterNET.ClubDesk.Core.Converters;

public class BillByEmailConverter: DefaultTypeConverter
{
    public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        return text == "E-Mail";
    }

    public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
    {
        if (value is bool b && b)
        {
            return "E-Mail";
        }

        return string.Empty;
    }
}