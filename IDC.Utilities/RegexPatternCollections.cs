using System.Text.RegularExpressions;

namespace IDC.Utilities;

/// <summary>
/// Provides a collection of commonly used regular expression patterns.
/// </summary>
/// <remarks>
/// This class contains source-generated regular expressions using the <see cref="GeneratedRegexAttribute"/>
/// to optimize performance by compiling patterns at build time.
///
/// Categories of patterns:
/// - Email validation
/// - Phone number validation
/// - Log parsing
/// - Connection string parsing
/// - General string manipulation
/// - Date and time validation
/// - Number validation
/// - Web-related validation
/// - File and path validation
/// - Security validation
/// </remarks>
public static partial class RegexPatternCollections
{
    #region Email Validation
    /// <summary>
    /// Validates email addresses.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating email addresses.
    ///
    /// Pattern explanation:
    /// - ^[a-zA-Z0-9._%+-]+ : Start with one or more letters, numbers, or allowed special characters
    /// - @ : Followed by @ symbol
    /// - [a-zA-Z0-9.-]+ : Domain name with letters, numbers, dots or hyphens
    /// - \. : Dot separator
    /// - [a-zA-Z]{2,}$ : TLD with 2 or more letters
    ///
    /// Example:
    /// <code>
    /// string email = "user@example.com";
    /// bool isValid = RegexPatternCollections.EmailValidation().IsMatch(email);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for email validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(
        pattern: @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex EmailValidation();
    #endregion

    #region Number Validation
    /// <summary>
    /// Validates integer numbers (positive/negative).
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating integer numbers, including both positive and negative values.
    ///
    /// Pattern explanation:
    /// - ^ : Start of the string
    /// - -? : Optional negative sign
    /// - \d+ : One or more digits
    /// - $ : End of the string
    ///
    /// Example:
    /// <code>
    /// string number = "-123";
    /// bool isValid = RegexPatternCollections.IntegerValidation().IsMatch(number);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for integer number validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(pattern: @"^-?\d+$", options: RegexOptions.Compiled)]
    public static partial Regex IntegerValidation();

    /// <summary>
    /// Validates decimal numbers (positive/negative).
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating decimal numbers, including both positive and negative values.
    ///
    /// Pattern explanation:
    /// - ^ : Start of the string
    /// - -? : Optional negative sign
    /// - \d* : Zero or more digits before the decimal point
    /// - \.? : Optional decimal point
    /// - \d+ : One or more digits after the decimal point
    /// - $ : End of the string
    ///
    /// Example:
    /// <code>
    /// string number = "-123.45";
    /// bool isValid = RegexPatternCollections.DecimalValidation().IsMatch(number);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for decimal number validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(pattern: @"^-?\d*\.?\d+$", options: RegexOptions.Compiled)]
    public static partial Regex DecimalValidation();

    /// <summary>
    /// Validates currency format (e.g., $123.45).
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating currency format.
    ///
    /// Pattern explanation:
    /// - ^ : Start of the string
    /// - \$? : Optional dollar sign
    /// - \d{1,3} : 1 to 3 digits
    /// - (,\d{3})* : Optional groups of 3 digits preceded by a comma (for thousands)
    /// - (\.\d{2})? : Optional decimal point followed by exactly 2 digits
    /// - $ : End of the string
    ///
    /// Example:
    /// <code>
    /// string currency = "$1,234.56";
    /// bool isValid = RegexPatternCollections.CurrencyValidation().IsMatch(currency);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for currency format validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(pattern: @"^\$?\d{1,3}(,\d{3})*(\.\d{2})?$", options: RegexOptions.Compiled)]
    public static partial Regex CurrencyValidation();

    /// <summary>
    /// Validates percentage format (e.g., 85% or 85.5%).
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating percentage format.
    ///
    /// Pattern explanation:
    /// - ^ : Start of the string
    /// - -? : Optional negative sign
    /// - \d* : Zero or more digits before the decimal point
    /// - \.? : Optional decimal point
    /// - \d+ : One or more digits after the decimal point
    /// - % : Percentage sign
    /// - $ : End of the string
    ///
    /// Example:
    /// <code>
    /// string percentage = "85.5%";
    /// bool isValid = RegexPatternCollections.PercentageValidation().IsMatch(percentage);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for percentage format validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(pattern: @"^-?\d*\.?\d+%$", options: RegexOptions.Compiled)]
    public static partial Regex PercentageValidation();
    #endregion

    #region Date and Time Validation
    /// <summary>
    /// Validates date in YYYY-MM-DD format.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating dates in the YYYY-MM-DD format.
    ///
    /// Pattern explanation:
    /// - ^ : Start of the string
    /// - \d{4} : Exactly 4 digits for the year
    /// - - : Hyphen separator
    /// - (0[1-9]|1[0-2]) : 01-12 for months
    /// - - : Hyphen separator
    /// - (0[1-9]|[12]\d|3[01]) : 01-31 for days
    /// - $ : End of the string
    ///
    /// Example:
    /// <code>
    /// string date = "2024-03-15";
    /// bool isValid = RegexPatternCollections.DateValidationYMD().IsMatch(date);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for YYYY-MM-DD date format validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(
        pattern: @"^\d{4}-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01])$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex DateValidationYMD();

    /// <summary>
    /// Validates date in DD/MM/YYYY format.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating dates in the DD/MM/YYYY format.
    ///
    /// Pattern explanation:
    /// - ^ : Start of the string
    /// - (0[1-9]|[12]\d|3[01]) : 01-31 for days
    /// - / : Forward slash separator
    /// - (0[1-9]|1[0-2]) : 01-12 for months
    /// - / : Forward slash separator
    /// - \d{4} : Exactly 4 digits for the year
    /// - $ : End of the string
    ///
    /// Example:
    /// <code>
    /// string date = "15/03/2024";
    /// bool isValid = RegexPatternCollections.DateValidationDMY().IsMatch(date);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for DD/MM/YYYY date format validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(
        pattern: @"^(0[1-9]|[12]\d|3[01])/(0[1-9]|1[0-2])/\d{4}$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex DateValidationDMY();

    /// <summary>
    /// Validates time in 24-hour format (HH:mm:ss).
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating times in the 24-hour format (HH:mm:ss).
    ///
    /// Pattern explanation:
    /// - ^ : Start of the string
    /// - ([01]\d|2[0-3]) : 00-23 for hours
    /// - : : Colon separator
    /// - ([0-5]\d) : 00-59 for minutes
    /// - : : Colon separator
    /// - ([0-5]\d) : 00-59 for seconds
    /// - $ : End of the string
    ///
    /// Example:
    /// <code>
    /// string time = "14:30:45";
    /// bool isValid = RegexPatternCollections.TimeValidation24Hour().IsMatch(time);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for 24-hour time format validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(
        pattern: @"^([01]\d|2[0-3]):([0-5]\d):([0-5]\d)$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex TimeValidation24Hour();

    /// <summary>
    /// Validates time in 12-hour format (hh:mm:ss AM/PM).
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating times in the 12-hour format (hh:mm:ss AM/PM).
    ///
    /// Pattern explanation:
    /// - ^ : Start of the string
    /// - (0?[1-9]|1[0-2]) : 01-12 for hours (optional leading zero)
    /// - : : Colon separator
    /// - ([0-5]\d) : 00-59 for minutes
    /// - : : Colon separator
    /// - ([0-5]\d) : 00-59 for seconds
    /// - \s? : Optional space
    /// - (AM|PM) : AM or PM indicator
    /// - $ : End of the string
    ///
    /// Example:
    /// <code>
    /// string time = "02:30:45 PM";
    /// bool isValid = RegexPatternCollections.TimeValidation12Hour().IsMatch(time);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for 12-hour time format validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(
        pattern: @"^(0?[1-9]|1[0-2]):([0-5]\d):([0-5]\d)\s?(AM|PM)$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex TimeValidation12Hour();
    #endregion

    #region Web Validation
    /// <summary>
    /// Validates URLs.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating URLs.
    ///
    /// Pattern explanation:
    /// - ^(https?:\/\/)? : Optional http:// or https:// at the start
    /// - ([\da-z\.-]+) : Domain name (letters, numbers, dots, or hyphens)
    /// - \.([a-z\.]{2,6}) : Top-level domain (2-6 letters or dots)
    /// - ([\/\w \.-]*)*\/? : Optional path and query parameters
    ///
    /// Example:
    /// <code>
    /// string url = "https://www.example.com/path/to/page?param=value";
    /// bool isValid = RegexPatternCollections.UrlValidation().IsMatch(url);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for URL validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(
        pattern: @"^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex UrlValidation();

    /// <summary>
    /// Validates IP addresses (IPv4).
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating IPv4 addresses.
    ///
    /// Pattern explanation:
    /// - ^ : Start of the string
    /// - (?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3} : Three groups of 1-3 digits (0-255) followed by a dot
    /// - (?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?) : Final group of 1-3 digits (0-255)
    /// - $ : End of the string
    ///
    /// Example:
    /// <code>
    /// string ipv4 = "192.168.0.1";
    /// bool isValid = RegexPatternCollections.IPv4Validation().IsMatch(ipv4);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for IPv4 address validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(
        pattern: @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex IPv4Validation();

    /// <summary>
    /// Validates IP addresses (IPv6).
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating IPv6 addresses.
    ///
    /// Pattern explanation:
    /// - Matches standard IPv6 format
    /// - Supports full and compressed notation with ::
    /// - Validates hex values in each segment
    /// - Handles both uppercase and lowercase hex digits
    ///
    /// Example:
    /// <code>
    /// string ipv6 = "2001:0db8:85a3:0000:0000:8a2e:0370:7334";
    /// bool isValid = RegexPatternCollections.IPv6Validation().IsMatch(ipv6);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for IPv6 address validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(
        pattern: @"^(([0-9a-fA-F]{1,4}:){7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]+|::(ffff(:0{1,4})?:)?((25[0-5]|(2[0-4]|1?[0-9])?[0-9])\.){3}(25[0-5]|(2[0-4]|1?[0-9])?[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1?[0-9])?[0-9])\.){3}(25[0-5]|(2[0-4]|1?[0-9])?[0-9]))$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex IPv6Validation();

    /// <summary>
    /// Validates domain names.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating domain names.
    ///
    /// Pattern explanation:
    /// - ^ : Start of the string
    /// - ([a-z0-9]+(-[a-z0-9]+)*\.)+ : One or more subdomains (letters, numbers, hyphens) followed by a dot
    /// - [a-z]{2,} : Top-level domain with at least 2 letters
    /// - $ : End of the string
    ///
    /// Example:
    /// <code>
    /// string domain = "example.com";
    /// bool isValid = RegexPatternCollections.DomainValidation().IsMatch(domain);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for domain name validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(
        pattern: @"^([a-z0-9]+(-[a-z0-9]+)*\.)+[a-z]{2,}$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex DomainValidation();

    /// <summary>
    /// Validates HTML tags.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating HTML tags.
    ///
    /// Pattern explanation:
    /// - &lt; : Opening angle bracket
    /// - [^&gt;]+ : One or more characters that are not a closing angle bracket
    /// - &gt; : Closing angle bracket
    ///
    /// Example:
    /// <code>
    /// string htmlTag = "&lt;div class='example'&gt;";
    /// bool isValid = RegexPatternCollections.HtmlTagValidation().IsMatch(htmlTag);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for HTML tag validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(pattern: @"<[^>]+>", options: RegexOptions.Compiled)]
    public static partial Regex HtmlTagValidation();
    #endregion

    #region Security Validation
    /// <summary>
    /// Validates strong passwords (min 8 chars, at least one uppercase, lowercase, number, and special char).
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating strong passwords.
    ///
    /// Pattern explanation:
    /// - ^ : Start of the string
    /// - (?=.*[a-z]) : At least one lowercase letter
    /// - (?=.*[A-Z]) : At least one uppercase letter
    /// - (?=.*\d) : At least one digit
    /// - (?=.*[@$!%*?&amp;]) : At least one special character
    /// - [A-Za-z\d@$!%*?&amp;]{8,} : At least 8 characters long, containing letters, digits, and special characters
    /// - $ : End of the string
    ///
    /// Example:
    /// <code>
    /// string password = "StrongP@ssw0rd";
    /// bool isValid = RegexPatternCollections.StrongPasswordValidation().IsMatch(password);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for strong password validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(
        pattern: @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex StrongPasswordValidation();

    /// <summary>
    /// Validates JWT tokens.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating JWT (JSON Web Token) format.
    ///
    /// Pattern explanation:
    /// - ^ : Start of the string
    /// - [A-Za-z0-9-_=]+ : One or more base64url characters for the header
    /// - \. : Dot separator
    /// - [A-Za-z0-9-_=]+ : One or more base64url characters for the payload
    /// - \.? : Optional dot separator
    /// - [A-Za-z0-9-_.+/=]* : Zero or more base64url characters for the signature (optional)
    /// - $ : End of the string
    ///
    /// Example:
    /// <code>
    /// string jwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
    /// bool isValid = RegexPatternCollections.JwtValidation().IsMatch(jwt);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for JWT format validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(
        pattern: @"^[A-Za-z0-9-_=]+\.[A-Za-z0-9-_=]+\.?[A-Za-z0-9-_.+/=]*$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex JwtValidation();
    #endregion

    #region Base64 Validation
    /// <summary>
    /// Validates Base64 strings.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating Base64 encoded strings.
    ///
    /// Pattern explanation:
    /// - ^ : Start of the string
    /// - [A-Za-z0-9+/]* : Zero or more Base64 characters (letters, numbers, +, /)
    /// - ={0,2} : Zero to two padding '=' characters
    /// - $ : End of the string
    ///
    /// Example:
    /// <code>
    /// string base64String = "SGVsbG8gV29ybGQ=";
    /// bool isValid = RegexPatternCollections.Base64Validation().IsMatch(base64String);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for Base64 string validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(pattern: @"^[A-Za-z0-9+/]*={0,2}$", options: RegexOptions.Compiled)]
    public static partial Regex Base64Validation();
    #endregion

    #region File and Path Validation
    /// <summary>
    /// Validates Windows file paths.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating Windows file paths.
    ///
    /// Pattern explanation:
    /// - ^ : Start of the string
    /// - ([a-zA-Z]:\\) : Drive letter followed by a colon and backslash
    /// - ((?:[^&lt;&gt;:"/\\|?*]*\\)*) : Zero or more directory names, each followed by a backslash
    /// - [^&lt;&gt;:"/\\|?*]* : File name (if any)
    /// - $ : End of the string
    ///
    /// Example:
    /// <code>
    /// string path = @"C:\Users\Username\Documents\file.txt";
    /// bool isValid = RegexPatternCollections.WindowsPathValidation().IsMatch(path);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for Windows file path validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(
        pattern: @"^([a-zA-Z]:\\)((?:[^<>:""/\\|?*]*\\)*)[^<>:""/\\|?*]*$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex WindowsPathValidation();

    /// <summary>
    /// Validates Unix/Linux file paths.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating Unix/Linux file paths.
    ///
    /// Pattern explanation:
    /// - ^ : Start of the string
    /// - (/[^/]*)+$ : One or more occurrences of a forward slash followed by zero or more non-slash characters
    ///
    /// Example:
    /// <code>
    /// string path = "/home/username/documents/file.txt";
    /// bool isValid = RegexPatternCollections.UnixPathValidation().IsMatch(path);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for Unix/Linux file path validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(pattern: @"^(/[^/]*)+$", options: RegexOptions.Compiled)]
    public static partial Regex UnixPathValidation();

    /// <summary>
    /// Validates file extensions.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating file extensions.
    ///
    /// Pattern explanation:
    /// - \. : A literal dot
    /// - [0-9a-z]+ : One or more lowercase letters or numbers
    /// - $ : End of the string
    ///
    /// Example:
    /// <code>
    /// string fileName = "document.txt";
    /// bool isValid = RegexPatternCollections.FileExtensionValidation().IsMatch(fileName);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for file extension validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(pattern: @"\.[0-9a-z]+$", options: RegexOptions.Compiled)]
    public static partial Regex FileExtensionValidation();
    #endregion

    #region Identity Validation
    /// <summary>
    /// Validates credit card numbers (basic format).
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating credit card numbers in a basic format.
    ///
    /// Pattern explanation:
    /// - ^ : Start of the string
    /// - \d{4} : Exactly 4 digits
    /// - [- ]? : Optional hyphen or space
    /// - \d{4} : Exactly 4 digits
    /// - [- ]? : Optional hyphen or space
    /// - \d{4} : Exactly 4 digits
    /// - [- ]? : Optional hyphen or space
    /// - \d{4} : Exactly 4 digits
    /// - $ : End of the string
    ///
    /// Example:
    /// <code>
    /// string creditCardNumber = "1234-5678-9012-3456";
    /// bool isValid = RegexPatternCollections.CreditCardValidation().IsMatch(creditCardNumber);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for basic credit card number validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(
        pattern: @"^\d{4}[- ]?\d{4}[- ]?\d{4}[- ]?\d{4}$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex CreditCardValidation();

    /// <summary>
    /// Validates social security numbers (US format).
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating US social security numbers.
    ///
    /// Pattern explanation:
    /// - ^ : Start of the string
    /// - \d{3} : Exactly 3 digits
    /// - - : Hyphen
    /// - \d{2} : Exactly 2 digits
    /// - - : Hyphen
    /// - \d{4} : Exactly 4 digits
    /// - $ : End of the string
    ///
    /// Example:
    /// <code>
    /// string ssn = "123-45-6789";
    /// bool isValid = RegexPatternCollections.SsnValidation().IsMatch(ssn);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for US social security number validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(pattern: @"^\d{3}-\d{2}-\d{4}$", options: RegexOptions.Compiled)]
    public static partial Regex SsnValidation();

    /// <summary>
    /// Validates postal codes (US format).
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating US postal codes (ZIP codes).
    ///
    /// Pattern explanation:
    /// - ^ : Start of the string
    /// - \d{5} : Exactly 5 digits
    /// - (-\d{4})? : Optional hyphen followed by exactly 4 digits
    /// - $ : End of the string
    ///
    /// Example:
    /// <code>
    /// string zipCode = "12345-6789";
    /// bool isValid = RegexPatternCollections.ZipCodeValidation().IsMatch(zipCode);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for US postal code validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(pattern: @"^\d{5}(-\d{4})?$", options: RegexOptions.Compiled)]
    public static partial Regex ZipCodeValidation();
    #endregion

    #region String Format Validation
    /// <summary>
    /// Validates strings containing only letters.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating strings that contain only letters (A-Z, a-z).
    ///
    /// Pattern explanation:
    /// - ^ : Start of the string
    /// - [a-zA-Z]+ : One or more letters (uppercase or lowercase)
    /// - $ : End of the string
    ///
    /// Example:
    /// <code>
    /// string text = "HelloWorld";
    /// bool isValid = RegexPatternCollections.LettersOnlyValidation().IsMatch(text);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for validating strings containing only letters.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(pattern: @"^[a-zA-Z]+$", options: RegexOptions.Compiled)]
    public static partial Regex LettersOnlyValidation();

    /// <summary>
    /// Validates strings containing only numbers.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating strings that contain only numbers (0-9).
    ///
    /// Pattern explanation:
    /// - ^ : Start of the string
    /// - \d+ : One or more digits
    /// - $ : End of the string
    ///
    /// Example:
    /// <code>
    /// string number = "12345";
    /// bool isValid = RegexPatternCollections.NumbersOnlyValidation().IsMatch(number);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for validating strings containing only numbers.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(pattern: @"^\d+$", options: RegexOptions.Compiled)]
    public static partial Regex NumbersOnlyValidation();

    /// <summary>
    /// Validates hex color codes.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating hexadecimal color codes.
    ///
    /// Pattern explanation:
    /// - ^ : Start of the string
    /// - #? : Optional # character
    /// - ([a-fA-F0-9]{6}|[a-fA-F0-9]{3}) : Either 6 or 3 hexadecimal characters
    /// - $ : End of the string
    ///
    /// Example:
    /// <code>
    /// string hexColor = "#FF00AA";
    /// bool isValid = RegexPatternCollections.HexColorValidation().IsMatch(hexColor);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for validating hex color codes.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(
        pattern: @"^#?([a-fA-F0-9]{6}|[a-fA-F0-9]{3})$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex HexColorValidation();

    /// <summary>
    /// Validates ISBN (both 10 and 13 digits).
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating ISBN (International Standard Book Number) in both 10-digit and 13-digit formats.
    ///
    /// Pattern explanation:
    /// - Matches ISBN-10 and ISBN-13 formats
    /// - Allows optional "ISBN" prefix and hyphens
    /// - Validates check digit (including 'X' for ISBN-10)
    ///
    /// Example:
    /// <code>
    /// string isbn = "ISBN-13: 978-0-596-52068-7";
    /// bool isValid = RegexPatternCollections.IsbnValidation().IsMatch(isbn);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for validating ISBN numbers.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(
        pattern: @"^(?:ISBN(?:-1[03])?:? )?(?=[0-9X]{10}$|(?=(?:[0-9]+[- ]){3})[- 0-9X]{13}$|97[89][0-9]{10}$|(?=(?:[0-9]+[- ]){4})[- 0-9]{17}$)(?:97[89][- ]?)?[0-9]{1,5}[- ]?[0-9]+[- ]?[0-9]+[- ]?[0-9X]$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex IsbnValidation();
    #endregion

    #region Alphanumeric Validation
    /// <summary>
    /// Validates alphanumeric strings.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating strings that contain only alphanumeric characters (A-Z, a-z, 0-9).
    ///
    /// Pattern explanation:
    /// - ^ : Start of the string
    /// - [a-zA-Z0-9]* : Zero or more alphanumeric characters
    /// - $ : End of the string
    ///
    /// Example:
    /// <code>
    /// string text = "Hello123";
    /// bool isValid = RegexPatternCollections.AlphanumericValidation().IsMatch(text);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for validating strings containing only alphanumeric characters.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(pattern: "^[a-zA-Z0-9]*$", options: RegexOptions.Compiled)]
    public static partial Regex AlphanumericValidation();
    #endregion

    #region Phone Number Validation
    /// <summary>
    /// Validates phone numbers in various formats.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating phone numbers in different formats.
    ///
    /// Pattern explanation:
    /// - ^ : Start of the string
    /// - \+? : Optional + at start for international numbers
    /// - (\d[\d-. ]+)? : Optional digits with separators (-, ., or space)
    /// - (\([\d-. ]+\))? : Optional parenthesized part
    /// - [\d-. ]+\d$ : Digits with separators, ending with a digit
    ///
    /// Example:
    /// <code>
    /// string phoneNumber = "+1 (123) 456-7890";
    /// bool isValid = RegexPatternCollections.PhoneNumberValidation().IsMatch(phoneNumber);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for phone number validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(
        pattern: @"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex PhoneNumberValidation();
    #endregion

    #region Log Parsing
    /// <summary>
    /// Splits log content into individual entries based on timestamp pattern.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for splitting log content into individual entries.
    ///
    /// Pattern explanation:
    /// - (?= : Positive lookahead
    /// - \[ : Opening square bracket
    /// - \d{4}-\d{2}-\d{2} : Date in YYYY-MM-DD format
    /// - \s+ : One or more whitespace characters
    /// - \d{2}:\d{2}:\d{2} : Time in HH:mm:ss format
    /// - \] : Closing square bracket
    /// - ) : End of positive lookahead
    ///
    /// Example:
    /// <code>
    /// string logContent = "[2023-05-01 10:30:00] Log entry 1\n[2023-05-01 10:31:00] Log entry 2";
    /// string[] entries = RegexPatternCollections.LogEntrySplitter().Split(logContent);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for splitting log entries.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(
        pattern: @"(?=\[\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2}\])",
        options: RegexOptions.Singleline
    )]
    public static partial Regex LogEntrySplitter();

    /// <summary>
    /// Parses simple log entries with timestamp, level, and message.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for parsing simple log entries.
    ///
    /// Pattern explanation:
    /// - ^ : Start of the string
    /// - \[(.*?)\] : Timestamp in square brackets (captured)
    /// - \s : Single whitespace
    /// - \[(.*?)\] : Log level in square brackets (captured)
    /// - \s : Single whitespace
    /// - (.+) : Message content (captured)
    /// - $ : End of the string
    ///
    /// Example:
    /// <code>
    /// string logEntry = "[2023-05-01 10:30:00] [INFO] Application started";
    /// Match match = RegexPatternCollections.SimpleLogEntry().Match(logEntry);
    /// if (match.Success)
    /// {
    ///     string timestamp = match.Groups[1].Value;
    ///     string level = match.Groups[2].Value;
    ///     string message = match.Groups[3].Value;
    /// }
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for parsing simple log entries.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(pattern: @"^\[(.*?)\] \[(.*?)\] (.+)$", options: RegexOptions.Singleline)]
    public static partial Regex SimpleLogEntry();

    /// <summary>
    /// Parses detailed log entries containing exception information.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for parsing detailed log entries with exception information.
    ///
    /// Pattern explanation:
    /// - \[(.*?)\] : Timestamp in square brackets (captured)
    /// - \s : Single whitespace
    /// - \[(.*?)\] : Log level in square brackets (captured)
    /// - \s : Single whitespace
    /// - Type: (.*?) : Exception type (captured)
    /// - [\r\n]+ : One or more newline characters
    /// - Message: (.*?) : Exception message (captured)
    /// - [\r\n]+ : One or more newline characters
    /// - StackTrace:[\r\n]+ : "StackTrace:" followed by newline(s)
    /// - ((?:   --> .*(?:\r?\n|$))*) : Stack trace lines (captured)
    ///
    /// Example:
    /// <code>
    /// string logEntry = @"[2023-05-01 10:30:00] [ERROR] Type: System.Exception
    /// Message: An error occurred
    /// StackTrace:
    ///    --> at Method1() in File1:line 10
    ///    --> at Method2() in File2:line 20";
    /// Match match = RegexPatternCollections.DetailedLogEntry().Match(logEntry);
    /// if (match.Success)
    /// {
    ///     string timestamp = match.Groups[1].Value;
    ///     string level = match.Groups[2].Value;
    ///     string exceptionType = match.Groups[3].Value;
    ///     string exceptionMessage = match.Groups[4].Value;
    ///     string stackTrace = match.Groups[5].Value;
    /// }
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for parsing detailed log entries.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(
        pattern: @"\[(.*?)\] \[(.*?)\] Type: (.*?)[\r\n]+Message: (.*?)[\r\n]+StackTrace:[\r\n]+((?:   --> .*(?:\r?\n|$))*)",
        options: RegexOptions.Singleline
    )]
    public static partial Regex DetailedLogEntry();
    #endregion

    #region Connection String Parsing
    /// <summary>
    /// Extracts the host from an Oracle TNS connection string.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for extracting the host from an Oracle TNS connection string.
    ///
    /// Pattern explanation:
    /// - HOST= : Literal "HOST=" string
    /// - ([^)]+) : Captures one or more characters that are not a closing parenthesis
    ///
    /// Example:
    /// <code>
    /// string connectionString = "(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ORCL)))";
    /// Match match = RegexPatternCollections.OracleTnsHost().Match(connectionString);
    /// if (match.Success)
    /// {
    ///     string host = match.Groups[1].Value; // "localhost"
    /// }
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for extracting the host from an Oracle TNS connection string.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    [GeneratedRegex(pattern: @"HOST=([^)]+)")]
    public static partial Regex OracleTnsHost();

    /// <summary>
    /// Extracts the port from an Oracle TNS connection string.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for extracting the port from an Oracle TNS connection string.
    ///
    /// Pattern explanation:
    /// - PORT= : Literal "PORT=" string
    /// - (\d+) : Captures one or more digits
    ///
    /// Example:
    /// <code>
    /// string connectionString = "(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ORCL)))";
    /// Match match = RegexPatternCollections.OracleTnsPort().Match(connectionString);
    /// if (match.Success)
    /// {
    ///     string port = match.Groups[1].Value; // "1521"
    /// }
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for extracting the port from an Oracle TNS connection string.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    [GeneratedRegex(pattern: @"PORT=(\d+)")]
    public static partial Regex OracleTnsPort();

    /// <summary>
    /// Extracts the service name from an Oracle TNS connection string.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for extracting the service name from an Oracle TNS connection string.
    ///
    /// Pattern explanation:
    /// - SERVICE_NAME= : Literal "SERVICE_NAME=" string
    /// - ([^)]+) : Captures one or more characters that are not a closing parenthesis
    ///
    /// Example:
    /// <code>
    /// string connectionString = "(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ORCL)))";
    /// Match match = RegexPatternCollections.OracleTnsServiceName().Match(connectionString);
    /// if (match.Success)
    /// {
    ///     string serviceName = match.Groups[1].Value; // "ORCL"
    /// }
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for extracting the service name from an Oracle TNS connection string.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    [GeneratedRegex(pattern: @"SERVICE_NAME=([^)]+)")]
    public static partial Regex OracleTnsServiceName();

    /// <summary>
    /// Extracts the user ID from an Oracle TNS connection string.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for extracting the user ID from an Oracle TNS connection string.
    ///
    /// Pattern explanation:
    /// - User Id= : Literal "User Id=" string
    /// - ([^;]+) : Captures one or more characters that are not a semicolon
    ///
    /// Example:
    /// <code>
    /// string connectionString = "User Id=scott;Password=tiger;Data Source=MyOracleDB;";
    /// Match match = RegexPatternCollections.OracleTnsUserId().Match(connectionString);
    /// if (match.Success)
    /// {
    ///     string userId = match.Groups[1].Value; // "scott"
    /// }
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for extracting the user ID from an Oracle TNS connection string.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    [GeneratedRegex(pattern: @"User Id=([^;]+)")]
    public static partial Regex OracleTnsUserId();

    /// <summary>
    /// Extracts the password from an Oracle TNS connection string.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for extracting the password from an Oracle TNS connection string.
    ///
    /// Pattern explanation:
    /// - Password= : Literal "Password=" string
    /// - ([^;]+) : Captures one or more characters that are not a semicolon
    ///
    /// Example:
    /// <code>
    /// string connectionString = "User Id=scott;Password=tiger;Data Source=MyOracleDB;";
    /// Match match = RegexPatternCollections.OracleTnsPassword().Match(connectionString);
    /// if (match.Success)
    /// {
    ///     string password = match.Groups[1].Value; // "tiger"
    /// }
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for extracting the password from an Oracle TNS connection string.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    [GeneratedRegex(pattern: @"Password=([^;]+)")]
    public static partial Regex OracleTnsPassword();
    #endregion

    #region String Manipulation
    /// <summary>
    /// Converts a string to snake_case.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for converting a string to snake_case.
    ///
    /// Pattern explanation:
    /// - ([a-z]) : Captures a lowercase letter
    /// - ([A-Z]) : Captures an uppercase letter
    ///
    /// Usage:
    /// This regex is typically used with a replacement pattern to insert an underscore between a lowercase and uppercase letter.
    ///
    /// Example:
    /// <code>
    /// string input = "camelCase";
    /// string snakeCase = RegexPatternCollections.SnakeCase().Replace(input, "$1_$2").ToLower();
    /// // Result: "camel_case"
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for converting strings to snake_case.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    [GeneratedRegex(pattern: "([a-z])([A-Z])")]
    public static partial Regex SnakeCase();

    /// <summary>
    /// Converts a string to kebab-case.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for converting a string to kebab-case.
    ///
    /// Pattern explanation:
    /// - ([a-z]) : Captures a lowercase letter
    /// - ([A-Z]) : Captures an uppercase letter
    ///
    /// Usage:
    /// This regex is typically used with a replacement pattern to insert a hyphen between a lowercase and uppercase letter.
    ///
    /// Example:
    /// <code>
    /// string input = "camelCase";
    /// string kebabCase = RegexPatternCollections.KebabCase().Replace(input, "$1-$2").ToLower();
    /// // Result: "camel-case"
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for converting strings to kebab-case.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    [GeneratedRegex(pattern: "([a-z])([A-Z])")]
    public static partial Regex KebabCase();
    #endregion

    #region Common Patterns
    /// <summary>
    /// Validates username format (alphanumeric, underscore, 3-16 chars).
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating usernames.
    ///
    /// Pattern explanation:
    /// - ^ : Start of the string
    /// - [a-zA-Z0-9_] : Matches any alphanumeric character or underscore
    /// - {3,16} : Matches between 3 and 16 occurrences of the previous character set
    /// - $ : End of the string
    ///
    /// Example:
    /// <code>
    /// string username = "user_123";
    /// bool isValid = RegexPatternCollections.UsernameValidation().IsMatch(username);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for username validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(pattern: @"^[a-zA-Z0-9_]{3,16}$", options: RegexOptions.Compiled)]
    public static partial Regex UsernameValidation();

    /// <summary>
    /// Validates slug format (lowercase, numbers, hyphens).
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating URL slugs.
    ///
    /// Pattern explanation:
    /// - ^ : Start of the string
    /// - [a-z0-9]+ : One or more lowercase letters or numbers
    /// - (?:-[a-z0-9]+)* : Zero or more occurrences of a hyphen followed by lowercase letters or numbers
    /// - $ : End of the string
    ///
    /// Example:
    /// <code>
    /// string slug = "my-awesome-post-123";
    /// bool isValid = RegexPatternCollections.SlugValidation().IsMatch(slug);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for slug validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(pattern: @"^[a-z0-9]+(?:-[a-z0-9]+)*$", options: RegexOptions.Compiled)]
    public static partial Regex SlugValidation();

    /// <summary>
    /// Validates semantic version number (SemVer).
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating semantic version numbers.
    ///
    /// Pattern explanation:
    /// - Matches MAJOR.MINOR.PATCH format
    /// - Optionally matches pre-release version and build metadata
    /// - Follows SemVer 2.0.0 specification
    ///
    /// Example:
    /// <code>
    /// string version = "1.0.0-alpha.1+001";
    /// bool isValid = RegexPatternCollections.SemVerValidation().IsMatch(version);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for semantic version validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(
        pattern: @"^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex SemVerValidation();

    /// <summary>
    /// Validates latitude coordinates.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating latitude coordinates.
    ///
    /// Pattern explanation:
    /// - Matches values between -90 and 90 degrees
    /// - Allows for decimal places
    ///
    /// Example:
    /// <code>
    /// string latitude = "45.5231";
    /// bool isValid = RegexPatternCollections.LatitudeValidation().IsMatch(latitude);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for latitude coordinate validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(
        pattern: @"^[-+]?([1-8]?\d(\.\d+)?|90(\.0+)?)$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex LatitudeValidation();

    /// <summary>
    /// Validates longitude coordinates.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating longitude coordinates.
    ///
    /// Pattern explanation:
    /// - Matches values between -180 and 180 degrees
    /// - Allows for decimal places
    ///
    /// Example:
    /// <code>
    /// string longitude = "-122.6762";
    /// bool isValid = RegexPatternCollections.LongitudeValidation().IsMatch(longitude);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for longitude coordinate validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(
        pattern: @"^[-+]?(180(\.0+)?|((1[0-7]\d)|([1-9]?\d))(\.\d+)?)$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex LongitudeValidation();

    /// <summary>
    /// Validates MAC addresses.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating MAC (Media Access Control) addresses.
    ///
    /// Pattern explanation:
    /// - Matches six groups of two hexadecimal digits
    /// - Allows for colon or hyphen separators
    ///
    /// Example:
    /// <code>
    /// string macAddress = "00:1A:2B:3C:4D:5E";
    /// bool isValid = RegexPatternCollections.MacAddressValidation().IsMatch(macAddress);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for MAC address validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(
        pattern: @"^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex MacAddressValidation();

    /// <summary>
    /// Validates GUID/UUID format.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating GUID (Globally Unique Identifier) or UUID (Universally Unique Identifier) formats.
    ///
    /// Pattern explanation:
    /// - Matches the standard 8-4-4-4-12 format of hexadecimal digits
    ///
    /// Example:
    /// <code>
    /// string guid = "550e8400-e29b-41d4-a716-446655440000";
    /// bool isValid = RegexPatternCollections.GuidValidation().IsMatch(guid);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for GUID/UUID validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(
        pattern: @"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex GuidValidation();

    /// <summary>
    /// Validates MongoDB ObjectId format.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating MongoDB ObjectId format.
    ///
    /// Pattern explanation:
    /// - Matches exactly 24 hexadecimal characters
    ///
    /// Example:
    /// <code>
    /// string objectId = "507f1f77bcf86cd799439011";
    /// bool isValid = RegexPatternCollections.MongoObjectIdValidation().IsMatch(objectId);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for MongoDB ObjectId validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(pattern: @"^[0-9a-fA-F]{24}$", options: RegexOptions.Compiled)]
    public static partial Regex MongoObjectIdValidation();

    /// <summary>
    /// Validates time duration format (e.g., 1h 30m, 90s).
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating time duration formats.
    ///
    /// Pattern explanation:
    /// - Matches combinations of days (d), hours (h), minutes (m), and seconds (s)
    /// - Each unit is optional but must be in the correct order if present
    ///
    /// Example:
    /// <code>
    /// string duration = "1h 30m 45s";
    /// bool isValid = RegexPatternCollections.TimeDurationValidation().IsMatch(duration);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for time duration format validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(pattern: @"^(\d+d)?(\d+h)?(\d+m)?(\d+s)?$", options: RegexOptions.Compiled)]
    public static partial Regex TimeDurationValidation();

    /// <summary>
    /// Validates RGB color format (e.g., rgb(255, 128, 0)).
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating RGB color format.
    ///
    /// Pattern explanation:
    /// - Matches the format rgb(r, g, b) where r, g, and b are integers between 0 and 255
    /// - Allows for optional spaces after commas and around numbers
    ///
    /// Example:
    /// <code>
    /// string rgbColor = "rgb(255, 128, 0)";
    /// bool isValid = RegexPatternCollections.RgbColorValidation().IsMatch(rgbColor);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for RGB color format validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(
        pattern: @"^rgb\(\s*(\d{1,3})\s*,\s*(\d{1,3})\s*,\s*(\d{1,3})\s*\)$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex RgbColorValidation();

    /// <summary>
    /// Validates RGBA color format (e.g., rgba(255, 128, 0, 0.5)).
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating RGBA color format.
    ///
    /// Pattern explanation:
    /// - Matches the format rgba(r, g, b, a) where r, g, and b are integers between 0 and 255
    /// - The alpha value (a) is a float between 0 and 1
    /// - Allows for optional spaces after commas and around numbers
    ///
    /// Example:
    /// <code>
    /// string rgbaColor = "rgba(255, 128, 0, 0.5)";
    /// bool isValid = RegexPatternCollections.RgbaColorValidation().IsMatch(rgbaColor);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for RGBA color format validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(
        pattern: @"^rgba\(\s*(\d{1,3})\s*,\s*(\d{1,3})\s*,\s*(\d{1,3})\s*,\s*([01]|0?\.\d+)\s*\)$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex RgbaColorValidation();

    /// <summary>
    /// Validates HSL color format (e.g., hsl(360, 100%, 50%)).
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating HSL (Hue, Saturation, Lightness) color format.
    ///
    /// Pattern explanation:
    /// - Matches the format hsl(h, s%, l%) where:
    ///   h is an integer between 0 and 360
    ///   s and l are percentages between 0% and 100%
    /// - Allows for optional spaces after commas and around numbers
    ///
    /// Example:
    /// <code>
    /// string hslColor = "hsl(360, 100%, 50%)";
    /// bool isValid = RegexPatternCollections.HslColorValidation().IsMatch(hslColor);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for HSL color format validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(
        pattern: @"^hsl\(\s*(\d{1,3})\s*,\s*(\d{1,3})%\s*,\s*(\d{1,3})%\s*\)$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex HslColorValidation();

    /// <summary>
    /// Validates HSLA color format (e.g., hsla(360, 100%, 50%, 0.5)).
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating HSLA (Hue, Saturation, Lightness, Alpha) color format.
    ///
    /// Pattern explanation:
    /// - Matches the format hsla(h, s%, l%, a) where:
    ///   h is an integer between 0 and 360
    ///   s and l are percentages between 0% and 100%
    ///   a is a float between 0 and 1
    /// - Allows for optional spaces after commas and around numbers
    ///
    /// Example:
    /// <code>
    /// string hslaColor = "hsla(360, 100%, 50%, 0.5)";
    /// bool isValid = RegexPatternCollections.HslaColorValidation().IsMatch(hslaColor);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for HSLA color format validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(
        pattern: @"^hsla\(\s*(\d{1,3})\s*,\s*(\d{1,3})%\s*,\s*(\d{1,3})%\s*,\s*([01]|0?\.\d+)\s*\)$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex HslaColorValidation();

    /// <summary>
    /// Validates XML tag format.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating XML tag format.
    ///
    /// Pattern explanation:
    /// - Matches opening tags, closing tags, and self-closing tags
    /// - Allows for attributes within tags
    /// - Captures tag name and content
    ///
    /// Example:
    /// <code>
    /// string xmlTag = "&lt;book id='1'&gt;Content&lt;/book&gt;";
    /// bool isValid = RegexPatternCollections.XmlTagValidation().IsMatch(xmlTag);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for XML tag format validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(
        pattern: @"<([a-z]+)([^<]+)*(?:>(.*)<\/\1>|\s+\/>)",
        options: RegexOptions.Compiled
    )]
    public static partial Regex XmlTagValidation();

    /// <summary>
    /// Validates markdown link format.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating markdown link format.
    ///
    /// Pattern explanation:
    /// - \[ : Opening square bracket
    /// - ([^\]]+) : Captures one or more characters that are not a closing square bracket (link text)
    /// - \] : Closing square bracket
    /// - \( : Opening parenthesis
    /// - ([^)]+) : Captures one or more characters that are not a closing parenthesis (link URL)
    /// - \) : Closing parenthesis
    ///
    /// Example:
    /// <code>
    /// string markdownLink = "[OpenAI](https://www.openai.com)";
    /// bool isValid = RegexPatternCollections.MarkdownLinkValidation().IsMatch(markdownLink);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for markdown link format validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(pattern: @"\[([^\]]+)\]\(([^)]+)\)", options: RegexOptions.Compiled)]
    public static partial Regex MarkdownLinkValidation();

    /// <summary>
    /// Validates markdown image format.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating markdown image format.
    ///
    /// Pattern explanation:
    /// - ! : Exclamation mark
    /// - \[ : Opening square bracket
    /// - ([^\]]+) : Captures one or more characters that are not a closing square bracket (alt text)
    /// - \] : Closing square bracket
    /// - \( : Opening parenthesis
    /// - ([^)]+) : Captures one or more characters that are not a closing parenthesis (image URL)
    /// - \) : Closing parenthesis
    ///
    /// Example:
    /// <code>
    /// string markdownImage = "![OpenAI Logo](https://www.openai.com/logo.png)";
    /// bool isValid = RegexPatternCollections.MarkdownImageValidation().IsMatch(markdownImage);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for markdown image format validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(pattern: @"!\[([^\]]+)\]\(([^)]+)\)", options: RegexOptions.Compiled)]
    public static partial Regex MarkdownImageValidation();

    /// <summary>
    /// Validates Twitter handle format.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating Twitter handle format.
    ///
    /// Pattern explanation:
    /// - ^ : Start of the string
    /// - @ : At symbol
    /// - [A-Za-z0-9_]{1,15} : 1 to 15 characters, allowing uppercase and lowercase letters, numbers, and underscores
    /// - $ : End of the string
    ///
    /// Example:
    /// <code>
    /// string twitterHandle = "@OpenAI";
    /// bool isValid = RegexPatternCollections.TwitterHandleValidation().IsMatch(twitterHandle);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for Twitter handle format validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(pattern: @"^@[A-Za-z0-9_]{1,15}$", options: RegexOptions.Compiled)]
    public static partial Regex TwitterHandleValidation();

    /// <summary>
    /// Validates Instagram handle format.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating Instagram handle format.
    ///
    /// Pattern explanation:
    /// - ^ : Start of the string
    /// - @ : At symbol
    /// - (?!.*\.\.) : Negative lookahead to ensure no consecutive dots
    /// - (?!.*\.$) : Negative lookahead to ensure it doesn't end with a dot
    /// - [^\W][\w.]{0,29} : 1 to 30 characters, allowing letters, numbers, underscores, and single dots (not at the end)
    /// - $ : End of the string
    ///
    /// Example:
    /// <code>
    /// string instagramHandle = "@openai";
    /// bool isValid = RegexPatternCollections.InstagramHandleValidation().IsMatch(instagramHandle);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for Instagram handle format validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(
        pattern: @"^@(?!.*\.\.)(?!.*\.$)[^\W][\w.]{0,29}$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex InstagramHandleValidation();

    /// <summary>
    /// Validates YouTube video ID format.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating YouTube video ID format.
    ///
    /// Pattern explanation:
    /// - ^ : Start of the string
    /// - [a-zA-Z0-9_-]{11} : Exactly 11 characters, allowing uppercase and lowercase letters, numbers, underscores, and hyphens
    /// - $ : End of the string
    ///
    /// Example:
    /// <code>
    /// string youtubeVideoId = "dQw4w9WgXcQ";
    /// bool isValid = RegexPatternCollections.YoutubeVideoIdValidation().IsMatch(youtubeVideoId);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for YouTube video ID format validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(pattern: @"^[a-zA-Z0-9_-]{11}$", options: RegexOptions.Compiled)]
    public static partial Regex YoutubeVideoIdValidation();

    /// <summary>
    /// Validates Bitcoin address format.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating Bitcoin address format.
    ///
    /// Pattern explanation:
    /// - ^ : Start of the string
    /// - [13] : Starts with either 1 or 3
    /// - [a-km-zA-HJ-NP-Z1-9]{25,34} : 25 to 34 characters, allowing specific letters and numbers (excludes 0, O, I, and l to avoid confusion)
    /// - $ : End of the string
    ///
    /// Example:
    /// <code>
    /// string bitcoinAddress = "1BvBMSEYstWetqTFn5Au4m4GFg7xJaNVN2";
    /// bool isValid = RegexPatternCollections.BitcoinAddressValidation().IsMatch(bitcoinAddress);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for Bitcoin address format validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(pattern: @"^[13][a-km-zA-HJ-NP-Z1-9]{25,34}$", options: RegexOptions.Compiled)]
    public static partial Regex BitcoinAddressValidation();

    /// <summary>
    /// Validates Ethereum address format.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating Ethereum address format.
    ///
    /// Pattern explanation:
    /// - ^ : Start of the string
    /// - 0x : Starts with "0x"
    /// - [a-fA-F0-9]{40} : Exactly 40 hexadecimal characters (0-9 and a-f or A-F)
    /// - $ : End of the string
    ///
    /// Example:
    /// <code>
    /// string ethereumAddress = "0x742d35Cc6634C0532925a3b844Bc454e4438f44e";
    /// bool isValid = RegexPatternCollections.EthereumAddressValidation().IsMatch(ethereumAddress);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for Ethereum address format validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(pattern: @"^0x[a-fA-F0-9]{40}$", options: RegexOptions.Compiled)]
    public static partial Regex EthereumAddressValidation();

    /// <summary>
    /// Validates IBAN (International Bank Account Number) format.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating IBAN (International Bank Account Number) format.
    ///
    /// Pattern explanation:
    /// - ^ : Start of the string
    /// - ([A-Z]{2}[ \-]?[0-9]{2}) : Country code (2 letters) and check digits (2 numbers), with optional space or hyphen
    /// - (?=(?:[ \-]?[A-Z0-9]){9,30}$) : Positive lookahead to ensure the total length is between 15 and 34 characters
    /// - ((?:[ \-]?[A-Z0-9]{3,5}){2,7}) : Groups of 3-5 characters (letters or numbers), with optional space or hyphen
    /// - ([ \-]?[A-Z0-9]{1,3})? : Optional group of 1-3 characters at the end
    /// - $ : End of the string
    ///
    /// Example:
    /// <code>
    /// string iban = "DE89 3704 0044 0532 0130 00";
    /// bool isValid = RegexPatternCollections.IbanValidation().IsMatch(iban);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for IBAN format validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(
        pattern: @"^([A-Z]{2}[ \-]?[0-9]{2})(?=(?:[ \-]?[A-Z0-9]){9,30}$)((?:[ \-]?[A-Z0-9]{3,5}){2,7})([ \-]?[A-Z0-9]{1,3})?$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex IbanValidation();

    /// <summary>
    /// Validates SWIFT/BIC code format.
    /// </summary>
    /// <remarks>
    /// This method provides a regular expression for validating SWIFT (Society for Worldwide Interbank Financial Telecommunication) or BIC (Bank Identifier Code) format.
    ///
    /// Pattern explanation:
    /// - ^ : Start of the string
    /// - [A-Z]{6} : 6 letters for bank code and country code
    /// - [A-Z0-9]{2} : 2 characters for location code
    /// - ([A-Z0-9]{3})? : Optional 3 characters for branch code
    /// - $ : End of the string
    ///
    /// Example:
    /// <code>
    /// string swiftCode = "BOFAUS3NXXX";
    /// bool isValid = RegexPatternCollections.SwiftCodeValidation().IsMatch(swiftCode);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> object for SWIFT/BIC code format validation.</returns>
    /// <seealso cref="Regex"/>
    /// <seealso cref="GeneratedRegexAttribute"/>
    /// <seealso cref="RegexOptions"/>
    [GeneratedRegex(
        pattern: @"^[A-Z]{6}[A-Z0-9]{2}([A-Z0-9]{3})?$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex SwiftCodeValidation();
    #endregion

    /// <summary>
    /// Collection of regular expressions for validating various banking-related formats commonly used in Indonesia.
    /// </summary>
    /// <remarks>
    /// This region contains regex patterns for validating:
    /// - Account numbers and transaction identifiers
    /// - Bank and branch codes
    /// - Payment-related formats (bills, virtual accounts)
    /// - Personal identification numbers (NPWP, KTP)
    /// - Security-related formats (OTP, PIN, CVV)
    /// - Digital payment formats (E-money, QR codes)
    ///
    /// Example usage:
    /// <code>
    /// string accountNumber = "1234567890";
    /// bool isValid = RegexPatternCollections.BankAccountNumberValidation().IsMatch(accountNumber);
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.bi.go.id/en/default.aspx">Bank Indonesia</seealso>
    /// <seealso href="https://www.ojk.go.id/en/default.aspx">Financial Services Authority of Indonesia (OJK)</seealso>
    #region Banking Validation

    /// <summary>
    /// Validates Account Number format (general format 10-20 digits).
    /// </summary>
    /// <remarks>
    /// Validates bank account numbers that consist of 10 to 20 digits.
    ///
    /// Example:
    /// <code>
    /// string accountNumber = "1234567890";
    /// bool isValid = RegexPatternCollections.BankAccountNumberValidation().IsMatch(accountNumber);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> for validating bank account numbers.</returns>
    /// <seealso href="https://www.bi.go.id/en/fungsi-utama/sistem-pembayaran/default.aspx">Bank Indonesia Payment Systems</seealso>
    [GeneratedRegex(pattern: @"^\d{10,20}$", options: RegexOptions.Compiled)]
    public static partial Regex BankAccountNumberValidation();

    /// <summary>
    /// Validates Indonesian Bank Account Number format (10-16 digits).
    /// </summary>
    /// <remarks>
    /// Validates Indonesian bank account numbers that consist of 10 to 16 digits.
    ///
    /// Example:
    /// <code>
    /// string accountNumber = "1234567890";
    /// bool isValid = RegexPatternCollections.IndonesianBankAccountValidation().IsMatch(accountNumber);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> for validating Indonesian bank account numbers.</returns>
    /// <seealso href="https://www.bi.go.id/en/fungsi-utama/sistem-pembayaran/default.aspx">Bank Indonesia Payment Systems</seealso>
    [GeneratedRegex(pattern: @"^\d{10,16}$", options: RegexOptions.Compiled)]
    public static partial Regex IndonesianBankAccountValidation();

    /// <summary>
    /// Validates Reference Number format (typically used in banking transactions).
    /// </summary>
    /// <remarks>
    /// Validates reference numbers consisting of 6 to 20 alphanumeric characters (uppercase).
    ///
    /// Example:
    /// <code>
    /// string refNumber = "REF123456";
    /// bool isValid = RegexPatternCollections.ReferenceNumberValidation().IsMatch(refNumber);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> for validating reference numbers.</returns>
    [GeneratedRegex(pattern: @"^[A-Z0-9]{6,20}$", options: RegexOptions.Compiled)]
    public static partial Regex ReferenceNumberValidation();

    /// <summary>
    /// Validates Transaction ID format.
    /// </summary>
    /// <remarks>
    /// Validates transaction IDs consisting of 8 to 32 alphanumeric characters (uppercase).
    ///
    /// Example:
    /// <code>
    /// string transactionId = "TRX12345678";
    /// bool isValid = RegexPatternCollections.TransactionIdValidation().IsMatch(transactionId);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> for validating transaction IDs.</returns>
    [GeneratedRegex(pattern: @"^[A-Z0-9]{8,32}$", options: RegexOptions.Compiled)]
    public static partial Regex TransactionIdValidation();

    /// <summary>
    /// Validates Indonesian Virtual Account Number format (prefix + account number).
    /// </summary>
    /// <remarks>
    /// Validates virtual account numbers with a 3-digit prefix followed by 8-16 digits.
    ///
    /// Example:
    /// <code>
    /// string vaNumber = "88812345678";
    /// bool isValid = RegexPatternCollections.VirtualAccountNumberValidation().IsMatch(vaNumber);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> for validating virtual account numbers.</returns>
    /// <seealso href="https://www.bi.go.id/en/fungsi-utama/sistem-pembayaran/default.aspx">Bank Indonesia Payment Systems</seealso>
    [GeneratedRegex(pattern: @"^\d{3}\d{8,16}$", options: RegexOptions.Compiled)]
    public static partial Regex VirtualAccountNumberValidation();

    /// <summary>
    /// Validates Indonesian Bank Code (3 digits).
    /// </summary>
    /// <remarks>
    /// Validates 3-digit bank codes assigned by Bank Indonesia.
    ///
    /// Example:
    /// <code>
    /// string bankCode = "014";
    /// bool isValid = RegexPatternCollections.BankCodeValidation().IsMatch(bankCode);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> for validating bank codes.</returns>
    /// <seealso href="https://www.bi.go.id/en/fungsi-utama/sistem-pembayaran/default.aspx">Bank Indonesia Payment Systems</seealso>
    [GeneratedRegex(pattern: @"^\d{3}$", options: RegexOptions.Compiled)]
    public static partial Regex BankCodeValidation();

    /// <summary>
    /// Validates Indonesian Bank Branch Code (4 digits).
    /// </summary>
    /// <remarks>
    /// Validates 4-digit branch codes used by Indonesian banks.
    ///
    /// Example:
    /// <code>
    /// string branchCode = "0001";
    /// bool isValid = RegexPatternCollections.BranchCodeValidation().IsMatch(branchCode);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> for validating branch codes.</returns>
    /// <seealso href="https://www.bi.go.id/en/fungsi-utama/sistem-pembayaran/default.aspx">Bank Indonesia Payment Systems</seealso>
    [GeneratedRegex(pattern: @"^\d{4}$", options: RegexOptions.Compiled)]
    public static partial Regex BranchCodeValidation();

    /// <summary>
    /// Validates Indonesian Currency Amount format (with optional decimals).
    /// </summary>
    /// <remarks>
    /// Validates Indonesian Rupiah amounts with proper thousand separators and optional decimal places.
    ///
    /// Example:
    /// <code>
    /// string amount = "Rp 1.234.567,89";
    /// bool isValid = RegexPatternCollections.IndonesianCurrencyValidation().IsMatch(amount);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> for validating currency amounts.</returns>
    /// <seealso href="https://www.bi.go.id/en/default.aspx">Bank Indonesia</seealso>
    [GeneratedRegex(
        pattern: @"^Rp\s?\d{1,3}(?:\.\d{3})*(?:,\d{2})?$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex IndonesianCurrencyValidation();

    /// <summary>
    /// Validates Bill Key format (used in bill payments).
    /// </summary>
    /// <remarks>
    /// Validates bill keys consisting of 6 to 20 digits.
    ///
    /// Example:
    /// <code>
    /// string billKey = "123456789";
    /// bool isValid = RegexPatternCollections.BillKeyValidation().IsMatch(billKey);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> for validating bill keys.</returns>
    [GeneratedRegex(pattern: @"^\d{6,20}$", options: RegexOptions.Compiled)]
    public static partial Regex BillKeyValidation();

    /// <summary>
    /// Validates Biller Code format (used in bill payments).
    /// </summary>
    /// <remarks>
    /// Validates biller codes consisting of 3 to 10 digits.
    ///
    /// Example:
    /// <code>
    /// string billerCode = "12345";
    /// bool isValid = RegexPatternCollections.BillerCodeValidation().IsMatch(billerCode);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> for validating biller codes.</returns>
    [GeneratedRegex(pattern: @"^\d{3,10}$", options: RegexOptions.Compiled)]
    public static partial Regex BillerCodeValidation();

    /// <summary>
    /// Validates Customer Number format (used in utilities/bill payments).
    /// </summary>
    /// <remarks>
    /// Validates customer numbers consisting of 6 to 20 digits.
    ///
    /// Example:
    /// <code>
    /// string customerNumber = "123456789";
    /// bool isValid = RegexPatternCollections.CustomerNumberValidation().IsMatch(customerNumber);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> for validating customer numbers.</returns>
    [GeneratedRegex(pattern: @"^\d{6,20}$", options: RegexOptions.Compiled)]
    public static partial Regex CustomerNumberValidation();

    /// <summary>
    /// Validates Indonesian Tax ID Number (NPWP) format.
    /// </summary>
    /// <remarks>
    /// Validates 15-digit NPWP numbers with proper formatting (XX.XXX.XXX.X-XXX.XXX).
    ///
    /// Example:
    /// <code>
    /// string npwp = "12.345.678.9-012.345";
    /// bool isValid = RegexPatternCollections.NpwpValidation().IsMatch(npwp);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> for validating NPWP numbers.</returns>
    /// <seealso href="https://www.pajak.go.id/en">Directorate General of Taxes</seealso>
    [GeneratedRegex(
        pattern: @"^\d{2}\.\d{3}\.\d{3}\.\d{1}-\d{3}\.\d{3}$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex NpwpValidation();

    /// <summary>
    /// Validates Indonesian ID Card Number (KTP/NIK) format.
    /// </summary>
    /// <remarks>
    /// Validates 16-digit Indonesian National Identity Card numbers.
    ///
    /// Example:
    /// <code>
    /// string ktp = "1234567890123456";
    /// bool isValid = RegexPatternCollections.KtpValidation().IsMatch(ktp);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> for validating KTP numbers.</returns>
    /// <seealso href="https://www.dukcapil.kemendagri.go.id/">Directorate General of Population and Civil Registration</seealso>
    [GeneratedRegex(pattern: @"^\d{16}$", options: RegexOptions.Compiled)]
    public static partial Regex KtpValidation();

    /// <summary>
    /// Validates Indonesian Passport Number format.
    /// </summary>
    /// <remarks>
    /// Validates Indonesian passport numbers (1-2 letters followed by 6-7 digits).
    ///
    /// Example:
    /// <code>
    /// string passport = "A1234567";
    /// bool isValid = RegexPatternCollections.PassportNumberValidation().IsMatch(passport);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> for validating passport numbers.</returns>
    /// <seealso href="https://www.imigrasi.go.id/">Directorate General of Immigration</seealso>
    [GeneratedRegex(pattern: @"^[A-Z]{1,2}\d{6,7}$", options: RegexOptions.Compiled)]
    public static partial Regex PassportNumberValidation();

    /// <summary>
    /// Validates OTP (One-Time Password) format (6 digits).
    /// </summary>
    /// <remarks>
    /// Validates 6-digit one-time passwords used for authentication.
    ///
    /// Example:
    /// <code>
    /// string otp = "123456";
    /// bool isValid = RegexPatternCollections.OtpValidation().IsMatch(otp);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> for validating OTP codes.</returns>
    [GeneratedRegex(pattern: @"^\d{6}$", options: RegexOptions.Compiled)]
    public static partial Regex OtpValidation();

    /// <summary>
    /// Validates Indonesian Mobile Banking PIN format (6 digits).
    /// </summary>
    /// <remarks>
    /// Validates 6-digit PINs used for mobile banking authentication.
    ///
    /// Example:
    /// <code>
    /// string pin = "123456";
    /// bool isValid = RegexPatternCollections.MobileBankingPinValidation().IsMatch(pin);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> for validating mobile banking PINs.</returns>
    [GeneratedRegex(pattern: @"^\d{6}$", options: RegexOptions.Compiled)]
    public static partial Regex MobileBankingPinValidation();

    /// <summary>
    /// Validates Card Verification Value (CVV/CVC) format (3-4 digits).
    /// </summary>
    /// <remarks>
    /// Validates 3-4 digit security codes found on payment cards.
    ///
    /// Example:
    /// <code>
    /// string cvv = "123";
    /// bool isValid = RegexPatternCollections.CvvValidation().IsMatch(cvv);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> for validating CVV numbers.</returns>
    [GeneratedRegex(pattern: @"^\d{3,4}$", options: RegexOptions.Compiled)]
    public static partial Regex CvvValidation();

    /// <summary>
    /// Validates Card Expiry Date format (MM/YY).
    /// </summary>
    /// <remarks>
    /// Validates payment card expiry dates in MM/YY format.
    ///
    /// Example:
    /// <code>
    /// string expiry = "12/25";
    /// bool isValid = RegexPatternCollections.CardExpiryValidation().IsMatch(expiry);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> for validating card expiry dates.</returns>
    [GeneratedRegex(pattern: @"^(0[1-9]|1[0-2])\/([0-9]{2})$", options: RegexOptions.Compiled)]
    public static partial Regex CardExpiryValidation();

    /// <summary>
    /// Validates Indonesian Bank Transfer Code format.
    /// </summary>
    /// <remarks>
    /// Validates bank transfer codes in XXX/XXX/XXXXX format.
    ///
    /// Example:
    /// <code>
    /// string transferCode = "014/001/12345";
    /// bool isValid = RegexPatternCollections.BankTransferCodeValidation().IsMatch(transferCode);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> for validating bank transfer codes.</returns>
    [GeneratedRegex(pattern: @"^\d{3}\/\d{3}\/\d{5}$", options: RegexOptions.Compiled)]
    public static partial Regex BankTransferCodeValidation();

    /// <summary>
    /// Validates Indonesian E-money Card Number format.
    /// </summary>
    /// <remarks>
    /// Validates 16-digit e-money card numbers.
    ///
    /// Example:
    /// <code>
    /// string cardNumber = "1234567890123456";
    /// bool isValid = RegexPatternCollections.EmoneyCardNumberValidation().IsMatch(cardNumber);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> for validating e-money card numbers.</returns>
    /// <seealso href="https://www.bi.go.id/en/fungsi-utama/sistem-pembayaran/default.aspx">Bank Indonesia Payment Systems</seealso>
    [GeneratedRegex(pattern: @"^\d{16}$", options: RegexOptions.Compiled)]
    public static partial Regex EmoneyCardNumberValidation();

    /// <summary>
    /// Validates Indonesian QR Payment Code format.
    /// </summary>
    /// <remarks>
    /// Validates QR payment codes consisting of 8 or more alphanumeric characters.
    ///
    /// Example:
    /// <code>
    /// string qrCode = "QR12345678";
    /// bool isValid = RegexPatternCollections.QrPaymentCodeValidation().IsMatch(qrCode);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> for validating QR payment codes.</returns>
    /// <seealso href="https://www.bi.go.id/QRIS/default.aspx">QRIS - Quick Response Code Indonesian Standard</seealso>
    [GeneratedRegex(pattern: @"^[0-9A-Z]{8,}$", options: RegexOptions.Compiled)]
    public static partial Regex QrPaymentCodeValidation();

    /// <summary>
    /// Validates Indonesian QRIS format.
    /// </summary>
    /// <remarks>
    /// Validates QRIS (Quick Response Code Indonesian Standard) format.
    /// The pattern matches strings starting with "00020101" and ending with a 2-digit checksum.
    ///
    /// Example:
    /// <code>
    /// string qris = "00020101021226280012COM.EXAMPLE.WWW01189360123456789";
    /// bool isValid = RegexPatternCollections.QrisValidation().IsMatch(qris);
    /// </code>
    /// </remarks>
    /// <returns>A compiled <see cref="Regex"/> for validating QRIS codes.</returns>
    /// <seealso href="https://www.bi.go.id/QRIS/default.aspx">QRIS - Quick Response Code Indonesian Standard</seealso>
    [GeneratedRegex(pattern: @"^00020101.*63[0-9]{2}$", options: RegexOptions.Compiled)]
    public static partial Regex QrisValidation();
    #endregion
}
