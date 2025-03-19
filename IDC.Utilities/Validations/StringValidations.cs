using System.Net;
using System.Text.RegularExpressions;

namespace IDC.Utilities.Validations;

/// <summary>
/// Provides extension methods for string validation.
/// </summary>
/// <remarks>
/// A comprehensive collection of string validation methods for common use cases like URLs, emails, IP addresses, etc.
/// All methods handle null checks and return false for null or whitespace inputs.
///
/// Example usage:
/// ```csharp
/// string email = "user@example.com";
/// if (email.IsValidEmail())
/// {
///     // Process valid email
/// }
///
/// string url = "https://api.example.com";
/// if (url.IsValidUrl())
/// {
///     // Process valid URL
/// }
/// ```
/// </remarks>
public static partial class StringValidations
{
    /// <summary>
    /// Validates if the string is a valid URL.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <returns>True if the string is a valid HTTP/HTTPS URL; otherwise, false.</returns>
    /// <remarks>
    /// Validates absolute URLs with HTTP or HTTPS schemes only.
    /// Returns false for relative URLs or other schemes (FTP, etc.).
    ///
    /// Example:
    /// ```csharp
    /// // Valid URLs
    /// "https://example.com".IsValidUrl()           // true
    /// "http://api.example.com/v1".IsValidUrl()     // true
    ///
    /// // Invalid URLs
    /// "ftp://example.com".IsValidUrl()             // false
    /// "/relative/path".IsValidUrl()                // false
    /// "not-a-url".IsValidUrl()                     // false
    /// ```
    /// </remarks>
    public static bool IsValidUrl(this string? value) =>
        !string.IsNullOrWhiteSpace(value: value)
        && Uri.TryCreate(uriString: value, uriKind: UriKind.Absolute, result: out var uriResult)
        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

    /// <summary>
    /// Validates if the string is a valid email address.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <returns>True if the string is a valid email address; otherwise, false.</returns>
    /// <remarks>
    /// Uses regex pattern matching for RFC 5322 compliant email validation.
    /// Handles common email formats including subdomains and special characters.
    ///
    /// Example:
    /// ```csharp
    /// // Valid emails
    /// "user@example.com".IsValidEmail()            // true
    /// "user.name+tag@example.co.uk".IsValidEmail() // true
    ///
    /// // Invalid emails
    /// "invalid.email".IsValidEmail()               // false
    /// "@example.com".IsValidEmail()                // false
    /// "user@.com".IsValidEmail()                   // false
    /// ```
    /// </remarks>
    public static bool IsValidEmail(this string? value) =>
        !string.IsNullOrWhiteSpace(value)
        && RegexPatternCollections.EmailValidation().IsMatch(value);

    /// <summary>
    /// Validates if the string is a valid IPv4 address.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <returns>True if the string is a valid IPv4 address; otherwise, false.</returns>
    /// <remarks>
    /// Validates IPv4 addresses using the IPAddress.TryParse method.
    /// Ensures the address family is InterNetwork (IPv4).
    ///
    /// Example:
    /// ```csharp
    /// // Valid IPv4
    /// "192.168.1.1".IsValidIPv4()     // true
    /// "10.0.0.0".IsValidIPv4()        // true
    ///
    /// // Invalid IPv4
    /// "256.1.2.3".IsValidIPv4()       // false
    /// "1.2.3".IsValidIPv4()           // false
    /// "2001:db8::1".IsValidIPv4()     // false
    /// ```
    /// </remarks>
    public static bool IsValidIPv4(this string? value) =>
        !string.IsNullOrWhiteSpace(value: value)
        && IPAddress.TryParse(ipString: value, address: out var address)
        && address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork;

    /// <summary>
    /// Validates if the string is a valid IPv6 address.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <returns>True if the string is a valid IPv6 address; otherwise, false.</returns>
    /// <remarks>
    /// Validates IPv6 addresses using the IPAddress.TryParse method.
    /// Ensures the address family is InterNetworkV6 (IPv6).
    ///
    /// Example:
    /// ```csharp
    /// // Valid IPv6
    /// "2001:0db8:85a3:0000:0000:8a2e:0370:7334".IsValidIPv6() // true
    /// "fe80::1".IsValidIPv6()                                  // true
    ///
    /// // Invalid IPv6
    /// "2001:0db8:85a3".IsValidIPv6()                          // false
    /// "192.168.1.1".IsValidIPv6()                             // false
    /// ```
    /// </remarks>
    public static bool IsValidIPv6(this string? value) =>
        !string.IsNullOrWhiteSpace(value: value)
        && IPAddress.TryParse(ipString: value, address: out var address)
        && address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6;

    /// <summary>
    /// Validates if the string contains only alphanumeric characters.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <returns>True if the string contains only letters and numbers; otherwise, false.</returns>
    /// <remarks>
    /// Uses regex pattern matching to validate alphanumeric characters.
    /// Spaces and special characters are not allowed.
    ///
    /// Example:
    /// ```csharp
    /// // Valid alphanumeric
    /// "ABC123".IsAlphanumeric()        // true
    /// "123456".IsAlphanumeric()        // true
    /// "abcDEF".IsAlphanumeric()        // true
    ///
    /// // Invalid alphanumeric
    /// "ABC 123".IsAlphanumeric()       // false
    /// "ABC-123".IsAlphanumeric()       // false
    /// "ABC_123".IsAlphanumeric()       // false
    /// ```
    /// </remarks>
    public static bool IsAlphanumeric(this string? value) =>
        !string.IsNullOrWhiteSpace(value)
        && RegexPatternCollections.AlphanumericValidation().IsMatch(value);

    /// <summary>
    /// Validates if the string is a valid phone number.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <returns>True if the string is a valid phone number; otherwise, false.</returns>
    /// <remarks>
    /// Uses regex pattern matching for international phone number validation.
    /// Supports various formats including country codes and extensions.
    ///
    /// Example:
    /// ```csharp
    /// // Valid phone numbers
    /// "+1-234-567-8900".IsValidPhoneNumber()     // true
    /// "+44 20 7123 4567".IsValidPhoneNumber()    // true
    /// "1234567890".IsValidPhoneNumber()          // true
    ///
    /// // Invalid phone numbers
    /// "123-ABC-4567".IsValidPhoneNumber()        // false
    /// "+1234".IsValidPhoneNumber()               // false
    /// "12345".IsValidPhoneNumber()               // false
    /// ```
    /// </remarks>
    public static bool IsValidPhoneNumber(this string? value) =>
        !string.IsNullOrWhiteSpace(value)
        && RegexPatternCollections.PhoneNumberValidation().IsMatch(value);

    /// <summary>
    /// Validates if the string is a valid date in the specified format.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <param name="format">The expected date format.</param>
    /// <returns>True if the string is a valid date in the specified format; otherwise, false.</returns>
    /// <remarks>
    /// Uses DateTime.TryParseExact for strict format validation.
    /// Culture-invariant parsing for consistent results.
    ///
    /// Example:
    /// ```csharp
    /// // Valid dates
    /// "2024-03-15".IsValidDate("yyyy-MM-dd")     // true
    /// "15/03/2024".IsValidDate("dd/MM/yyyy")     // true
    /// "03/15/2024".IsValidDate("MM/dd/yyyy")     // true
    ///
    /// // Invalid dates
    /// "2024-13-15".IsValidDate("yyyy-MM-dd")     // false
    /// "15/13/2024".IsValidDate("dd/MM/yyyy")     // false
    /// "2024-03-15".IsValidDate("dd/MM/yyyy")     // false
    /// ```
    /// </remarks>
    public static bool IsValidDate(this string? value, string format) =>
        !string.IsNullOrWhiteSpace(value: value)
        && DateTime.TryParseExact(
            s: value,
            format: format,
            provider: System.Globalization.CultureInfo.InvariantCulture,
            style: System.Globalization.DateTimeStyles.None,
            result: out _
        );

    /// <summary>
    /// Validates if the string matches the specified regular expression pattern.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <param name="pattern">The regular expression pattern to match.</param>
    /// <returns>True if the string matches the pattern; otherwise, false.</returns>
    /// <remarks>
    /// Uses compiled regex for better performance.
    /// Pattern matching is case-sensitive by default.
    ///
    /// Example:
    /// ```csharp
    /// // Valid patterns
    /// "ABC-123".MatchesPattern(@"^[A-Z]+-\d+$")          // true
    /// "user_123".MatchesPattern(@"^[a-z]+_\d+$")         // true
    ///
    /// // Invalid patterns
    /// "abc-123".MatchesPattern(@"^[A-Z]+-\d+$")          // false
    /// "USER_123".MatchesPattern(@"^[a-z]+_\d+$")         // false
    /// ```
    /// </remarks>
    public static bool MatchesPattern(this string? value, string pattern) =>
        !string.IsNullOrWhiteSpace(value: value)
        && Regex.IsMatch(input: value, pattern: pattern, options: RegexOptions.Compiled);

    /// <summary>
    /// Validates if the string contains only numeric characters.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <returns>True if the string contains only digits; otherwise, false.</returns>
    /// <remarks>
    /// Uses char.IsDigit for each character validation.
    /// Does not allow decimal points, signs, or separators.
    ///
    /// Example:
    /// ```csharp
    /// // Valid numeric strings
    /// "12345".IsNumeric()         // true
    /// "0123".IsNumeric()          // true
    ///
    /// // Invalid numeric strings
    /// "12.34".IsNumeric()         // false
    /// "-123".IsNumeric()          // false
    /// "12,345".IsNumeric()        // false
    /// ```
    /// </remarks>
    public static bool IsNumeric(this string? value) =>
        !string.IsNullOrWhiteSpace(value: value) && value.All(predicate: char.IsDigit);

    /// <summary>
    /// Validates if the string length is within the specified range.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <param name="minLength">The minimum allowed length.</param>
    /// <param name="maxLength">The maximum allowed length.</param>
    /// <returns>True if the string length is within range; otherwise, false.</returns>
    /// <remarks>
    /// Validates string length inclusively between minLength and maxLength.
    /// Returns false for null or whitespace strings.
    ///
    /// Example:
    /// ```csharp
    /// // Valid lengths
    /// "Hello".HasValidLength(3, 10)           // true
    /// "A".HasValidLength(1, 1)                // true
    /// "Testing123".HasValidLength(5, 10)      // true
    ///
    /// // Invalid lengths
    /// "Hi".HasValidLength(3, 10)              // false
    /// "TooLongString".HasValidLength(1, 5)    // false
    /// "".HasValidLength(1, 10)                // false
    /// ```
    /// </remarks>
    public static bool HasValidLength(this string? value, int minLength, int maxLength) =>
        !string.IsNullOrWhiteSpace(value: value)
        && value.Length >= minLength
        && value.Length <= maxLength;
}
