using System.Net;
using System.Text.RegularExpressions;

namespace IDC.Utilities.Validations;

/// <summary>
/// Provides extension methods for string validation.
/// </summary>
public static partial class StringValidations
{
    /// <summary>
    /// Generates a compiled regular expression for validating email addresses.
    /// </summary>
    /// <remarks>
    /// Pattern explanation:
    /// - ^[a-zA-Z0-9._%+-]+ : Start with one or more letters, numbers, or allowed special characters
    /// - @ : Followed by @ symbol
    /// - [a-zA-Z0-9.-]+ : Domain name with letters, numbers, dots or hyphens
    /// - \. : Dot separator
    /// - [a-zA-Z]{2,}$ : TLD with 2 or more letters
    /// </remarks>
    /// <returns>Compiled Regex for email validation</returns>
    [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled)]
    private static partial Regex REEmailAddressValidation();

    /// <summary>
    /// Generates a compiled regular expression for validating alphanumeric strings.
    /// </summary>
    /// <remarks>
    /// Pattern explanation:
    /// - ^[a-zA-Z0-9]*$ : Match string containing only letters and numbers from start to end
    /// </remarks>
    /// <returns>Compiled Regex for alphanumeric validation</returns>
    [GeneratedRegex("^[a-zA-Z0-9]*$", RegexOptions.Compiled)]
    private static partial Regex REAlphanumerValidation();

    /// <summary>
    /// Generates a compiled regular expression for validating phone numbers.
    /// </summary>
    /// <remarks>
    /// Pattern explanation:
    /// - ^\+? : Optional + at start
    /// - (\d[\d-. ]+)? : Optional digits with separators
    /// - (\([\d-. ]+\))? : Optional parenthesized part
    /// - [\d-. ]+\d$ : Digits with separators, ending with digit
    /// </remarks>
    /// <returns>Compiled Regex for phone number validation</returns>
    [GeneratedRegex(@"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$", RegexOptions.Compiled)]
    private static partial Regex REPhoneNumberValidation();

    /// <summary>
    /// Validates if the string is a valid URL.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <returns>True if the string is a valid URL; otherwise, false.</returns>
    /// <example>
    /// <code>
    /// string url = "https://example.com";
    /// bool isValid = url.IsValidUrl(); // returns true
    /// </code>
    /// </example>
    public static bool IsValidUrl(this string? value) =>
        !string.IsNullOrWhiteSpace(value)
        && Uri.TryCreate(value, UriKind.Absolute, out var uriResult)
        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

    /// <summary>
    /// Validates if the string is a valid email address.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <returns>True if the string is a valid email address; otherwise, false.</returns>
    /// <example>
    /// <code>
    /// string email = "user@example.com";
    /// bool isValid = email.IsValidEmail(); // returns true
    /// </code>
    /// </example>
    public static bool IsValidEmail(this string? value) =>
        !string.IsNullOrWhiteSpace(value) && REEmailAddressValidation().IsMatch(value);

    /// <summary>
    /// Validates if the string is a valid IPv4 address.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <returns>True if the string is a valid IPv4 address; otherwise, false.</returns>
    /// <example>
    /// <code>
    /// string ip = "192.168.1.1";
    /// bool isValid = ip.IsValidIPv4(); // returns true
    /// </code>
    /// </example>
    public static bool IsValidIPv4(this string? value) =>
        !string.IsNullOrWhiteSpace(value)
        && IPAddress.TryParse(value, out var address)
        && address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork;

    /// <summary>
    /// Validates if the string is a valid IPv6 address.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <returns>True if the string is a valid IPv6 address; otherwise, false.</returns>
    /// <example>
    /// <code>
    /// string ip = "2001:0db8:85a3:0000:0000:8a2e:0370:7334";
    /// bool isValid = ip.IsValidIPv6(); // returns true
    /// </code>
    /// </example>
    public static bool IsValidIPv6(this string? value) =>
        !string.IsNullOrWhiteSpace(value)
        && IPAddress.TryParse(value, out var address)
        && address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6;

    /// <summary>
    /// Validates if the string contains only alphanumeric characters.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <returns>True if the string contains only alphanumeric characters; otherwise, false.</returns>
    /// <example>
    /// <code>
    /// string text = "ABC123";
    /// bool isValid = text.IsAlphanumeric(); // returns true
    /// </code>
    /// </example>
    public static bool IsAlphanumeric(this string? value) =>
        !string.IsNullOrWhiteSpace(value) && REAlphanumerValidation().IsMatch(value);

    /// <summary>
    /// Validates if the string is a valid phone number.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <returns>True if the string is a valid phone number; otherwise, false.</returns>
    /// <example>
    /// <code>
    /// string phone = "+1-234-567-8900";
    /// bool isValid = phone.IsValidPhoneNumber(); // returns true
    /// </code>
    /// </example>
    public static bool IsValidPhoneNumber(this string? value) =>
        !string.IsNullOrWhiteSpace(value) && REPhoneNumberValidation().IsMatch(value);

    /// <summary>
    /// Validates if the string is a valid date in the specified format.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <param name="format">The expected date format.</param>
    /// <returns>True if the string is a valid date in the specified format; otherwise, false.</returns>
    /// <example>
    /// <code>
    /// string date = "2024-03-02";
    /// bool isValid = date.IsValidDate("yyyy-MM-dd"); // returns true
    /// </code>
    /// </example>
    public static bool IsValidDate(this string? value, string format) =>
        !string.IsNullOrWhiteSpace(value)
        && DateTime.TryParseExact(
            value,
            format,
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None,
            out _
        );

    /// <summary>
    /// Validates if the string matches the specified regular expression pattern.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <param name="pattern">The regular expression pattern to match.</param>
    /// <returns>True if the string matches the pattern; otherwise, false.</returns>
    /// <example>
    /// <code>
    /// string text = "ABC-123";
    /// bool isValid = text.MatchesPattern(@"^[A-Z]+-\d+$"); // returns true
    /// </code>
    /// </example>
    public static bool MatchesPattern(this string? value, string pattern) =>
        !string.IsNullOrWhiteSpace(value) && Regex.IsMatch(value, pattern, RegexOptions.Compiled);

    /// <summary>
    /// Validates if the string contains only numeric characters.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <returns>True if the string contains only numeric characters; otherwise, false.</returns>
    /// <example>
    /// <code>
    /// string number = "12345";
    /// bool isValid = number.IsNumeric(); // returns true
    /// </code>
    /// </example>
    public static bool IsNumeric(this string? value) =>
        !string.IsNullOrWhiteSpace(value) && value.All(char.IsDigit);

    /// <summary>
    /// Validates if the string length is within the specified range.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <param name="minLength">The minimum allowed length.</param>
    /// <param name="maxLength">The maximum allowed length.</param>
    /// <returns>True if the string length is within the specified range; otherwise, false.</returns>
    /// <example>
    /// <code>
    /// string text = "Hello";
    /// bool isValid = text.HasValidLength(minLength: 3, maxLength: 10); // returns true
    /// </code>
    /// </example>
    public static bool HasValidLength(this string? value, int minLength, int maxLength) =>
        !string.IsNullOrWhiteSpace(value) && value.Length >= minLength && value.Length <= maxLength;
}
