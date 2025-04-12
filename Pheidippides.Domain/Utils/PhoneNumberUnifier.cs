using System.Text.RegularExpressions;
using Pheidippides.Domain.Exceptions;

namespace Pheidippides.Domain.Utils;

public partial class PhoneNumberUnifier
{
    private const string DEFAULT_COUNTRY_CODE = "7";
    private const int NATIONAL_NUMBER_LENGTH = 10;
    private static readonly Regex DigitsRegex = MyRegex();

    public static string Standardize(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            throw new BadRequestException("Номер телефона не может быть пустым");
        }

        var digits = DigitsRegex.Replace(phoneNumber, "");

        digits = digits.TrimStart('0');

        if (digits.Length > 0 && digits[0] == '8' && DEFAULT_COUNTRY_CODE != "8")
        {
            digits = string.Concat(DEFAULT_COUNTRY_CODE, digits.AsSpan(1));
        }

        digits = digits.Length switch
        {
            NATIONAL_NUMBER_LENGTH => DEFAULT_COUNTRY_CODE + digits,
            < NATIONAL_NUMBER_LENGTH => throw new BadRequestException("Номер телефона слишком короткий"),
            _ => digits
        };

        return $"+{digits}";
    }

    [GeneratedRegex(@"[^\d]")]
    private static partial Regex MyRegex();
}