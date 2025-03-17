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
    /// Pattern explanation:
    /// - ^[a-zA-Z0-9._%+-]+ : Start with one or more letters, numbers, or allowed special characters
    /// - @ : Followed by @ symbol
    /// - [a-zA-Z0-9.-]+ : Domain name with letters, numbers, dots or hyphens
    /// - \. : Dot separator
    /// - [a-zA-Z]{2,}$ : TLD with 2 or more letters
    /// </remarks>
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
    [GeneratedRegex(pattern: @"^-?\d+$", options: RegexOptions.Compiled)]
    public static partial Regex IntegerValidation();

    /// <summary>
    /// Validates decimal numbers (positive/negative).
    /// </summary>
    [GeneratedRegex(pattern: @"^-?\d*\.?\d+$", options: RegexOptions.Compiled)]
    public static partial Regex DecimalValidation();

    /// <summary>
    /// Validates currency format (e.g., $123.45).
    /// </summary>
    [GeneratedRegex(pattern: @"^\$?\d{1,3}(,\d{3})*(\.\d{2})?$", options: RegexOptions.Compiled)]
    public static partial Regex CurrencyValidation();

    /// <summary>
    /// Validates percentage format (e.g., 85% or 85.5%).
    /// </summary>
    [GeneratedRegex(pattern: @"^-?\d*\.?\d+%$", options: RegexOptions.Compiled)]
    public static partial Regex PercentageValidation();
    #endregion

    #region Date and Time Validation
    /// <summary>
    /// Validates date in YYYY-MM-DD format.
    /// </summary>
    [GeneratedRegex(
        pattern: @"^\d{4}-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01])$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex DateValidationYMD();

    /// <summary>
    /// Validates date in DD/MM/YYYY format.
    /// </summary>
    [GeneratedRegex(
        pattern: @"^(0[1-9]|[12]\d|3[01])/(0[1-9]|1[0-2])/\d{4}$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex DateValidationDMY();

    /// <summary>
    /// Validates time in 24-hour format (HH:mm:ss).
    /// </summary>
    [GeneratedRegex(
        pattern: @"^([01]\d|2[0-3]):([0-5]\d):([0-5]\d)$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex TimeValidation24Hour();

    /// <summary>
    /// Validates time in 12-hour format (hh:mm:ss AM/PM).
    /// </summary>
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
    [GeneratedRegex(
        pattern: @"^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex UrlValidation();

    /// <summary>
    /// Validates IP addresses (IPv4).
    /// </summary>
    [GeneratedRegex(
        pattern: @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex IPv4Validation();

    /// <summary>
    /// Validates IP addresses (IPv6).
    /// </summary>
    /// <remarks>
    /// Pattern explanation:
    /// - Matches standard IPv6 format
    /// - Supports full and compressed notation with ::
    /// - Validates hex values in each segment
    /// - Handles both uppercase and lowercase hex digits
    /// </remarks>
    [GeneratedRegex(
        pattern: @"^(([0-9a-fA-F]{1,4}:){7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]+|::(ffff(:0{1,4})?:)?((25[0-5]|(2[0-4]|1?[0-9])?[0-9])\.){3}(25[0-5]|(2[0-4]|1?[0-9])?[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1?[0-9])?[0-9])\.){3}(25[0-5]|(2[0-4]|1?[0-9])?[0-9]))$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex IPv6Validation();

    /// <summary>
    /// Validates domain names.
    /// </summary>
    [GeneratedRegex(
        pattern: @"^([a-z0-9]+(-[a-z0-9]+)*\.)+[a-z]{2,}$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex DomainValidation();
    #endregion

    /// <summary>
    /// Validates HTML tags.
    /// </summary>
    [GeneratedRegex(pattern: @"<[^>]+>", options: RegexOptions.Compiled)]
    public static partial Regex HtmlTagValidation();

    #region Security Validation
    /// <summary>
    /// Validates strong passwords (min 8 chars, at least one uppercase, lowercase, number, and special char).
    /// </summary>
    [GeneratedRegex(
        pattern: @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex StrongPasswordValidation();

    /// <summary>
    /// Validates JWT tokens.
    /// </summary>
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
    [GeneratedRegex(pattern: @"^[A-Za-z0-9+/]*={0,2}$", options: RegexOptions.Compiled)]
    public static partial Regex Base64Validation();
    #endregion

    #region File and Path Validation
    /// <summary>
    /// Validates Windows file paths.
    /// </summary>
    [GeneratedRegex(
        pattern: @"^([a-zA-Z]:\\)((?:[^<>:""/\\|?*]*\\)*)[^<>:""/\\|?*]*$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex WindowsPathValidation();

    /// <summary>
    /// Validates Unix/Linux file paths.
    /// </summary>
    [GeneratedRegex(pattern: @"^(/[^/]*)+$", options: RegexOptions.Compiled)]
    public static partial Regex UnixPathValidation();

    /// <summary>
    /// Validates file extensions.
    /// </summary>
    [GeneratedRegex(pattern: @"\.[0-9a-z]+$", options: RegexOptions.Compiled)]
    public static partial Regex FileExtensionValidation();
    #endregion

    #region Identity Validation
    /// <summary>
    /// Validates credit card numbers (basic format).
    /// </summary>
    [GeneratedRegex(
        pattern: @"^\d{4}[- ]?\d{4}[- ]?\d{4}[- ]?\d{4}$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex CreditCardValidation();

    /// <summary>
    /// Validates social security numbers (US format).
    /// </summary>
    [GeneratedRegex(pattern: @"^\d{3}-\d{2}-\d{4}$", options: RegexOptions.Compiled)]
    public static partial Regex SsnValidation();

    /// <summary>
    /// Validates postal codes (US format).
    /// </summary>
    [GeneratedRegex(pattern: @"^\d{5}(-\d{4})?$", options: RegexOptions.Compiled)]
    public static partial Regex ZipCodeValidation();
    #endregion

    #region String Format Validation
    /// <summary>
    /// Validates strings containing only letters.
    /// </summary>
    [GeneratedRegex(pattern: @"^[a-zA-Z]+$", options: RegexOptions.Compiled)]
    public static partial Regex LettersOnlyValidation();

    /// <summary>
    /// Validates strings containing only numbers.
    /// </summary>
    [GeneratedRegex(pattern: @"^\d+$", options: RegexOptions.Compiled)]
    public static partial Regex NumbersOnlyValidation();

    /// <summary>
    /// Validates hex color codes.
    /// </summary>
    [GeneratedRegex(
        pattern: @"^#?([a-fA-F0-9]{6}|[a-fA-F0-9]{3})$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex HexColorValidation();

    /// <summary>
    /// Validates ISBN (both 10 and 13 digits).
    /// </summary>
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
    /// Pattern explanation:
    /// - ^[a-zA-Z0-9]*$ : Match string containing only letters and numbers from start to end
    /// </remarks>
    [GeneratedRegex(pattern: "^[a-zA-Z0-9]*$", options: RegexOptions.Compiled)]
    public static partial Regex AlphanumericValidation();
    #endregion

    #region Phone Number Validation
    /// <summary>
    /// Validates phone numbers.
    /// </summary>
    /// <remarks>
    /// Pattern explanation:
    /// - ^\+? : Optional + at start
    /// - (\d[\d-. ]+)? : Optional digits with separators
    /// - (\([\d-. ]+\))? : Optional parenthesized part
    /// - [\d-. ]+\d$ : Digits with separators, ending with digit
    /// </remarks>
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
    /// Pattern matches the start of each log entry by looking for timestamp in format [YYYY-MM-DD HH:mm:ss].
    /// Uses positive lookahead to preserve the timestamp in the split result.
    /// </remarks>
    [GeneratedRegex(
        pattern: @"(?=\[\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2}\])",
        options: RegexOptions.Singleline
    )]
    public static partial Regex LogEntrySplitter();

    /// <summary>
    /// Parses simple log entries with timestamp, level and message.
    /// </summary>
    /// <remarks>
    /// Pattern captures three groups:
    /// 1. Timestamp
    /// 2. Log level
    /// 3. Message content
    /// </remarks>
    [GeneratedRegex(pattern: @"^\[(.*?)\] \[(.*?)\] (.+)$", options: RegexOptions.Singleline)]
    public static partial Regex SimpleLogEntry();

    /// <summary>
    /// Parses detailed log entries containing exception information.
    /// </summary>
    /// <remarks>
    /// Pattern captures five groups:
    /// 1. Timestamp
    /// 2. Log level
    /// 3. Exception type
    /// 4. Exception message
    /// 5. Stack trace lines
    /// </remarks>
    [GeneratedRegex(
        pattern: @"\[(.*?)\] \[(.*?)\] Type: (.*?)[\r\n]+Message: (.*?)[\r\n]+StackTrace:[\r\n]+((?:   --> .*(?:\r?\n|$))*)",
        options: RegexOptions.Singleline
    )]
    public static partial Regex DetailedLogEntry();
    #endregion

    #region Connection String Parsing
    /// <summary>
    /// Extracts Oracle TNS connection parameters.
    /// </summary>
    [GeneratedRegex(pattern: @"HOST=([^)]+)")]
    public static partial Regex OracleTnsHost();

    [GeneratedRegex(pattern: @"PORT=(\d+)")]
    public static partial Regex OracleTnsPort();

    [GeneratedRegex(pattern: @"SERVICE_NAME=([^)]+)")]
    public static partial Regex OracleTnsServiceName();

    [GeneratedRegex(pattern: @"User Id=([^;]+)")]
    public static partial Regex OracleTnsUserId();

    [GeneratedRegex(pattern: @"Password=([^;]+)")]
    public static partial Regex OracleTnsPassword();
    #endregion

    #region String Manipulation
    /// <summary>
    /// Converts string to snake_case.
    /// </summary>
    [GeneratedRegex(pattern: "([a-z])([A-Z])")]
    public static partial Regex SnakeCase();

    /// <summary>
    /// Converts string to kebab-case.
    /// </summary>
    [GeneratedRegex(pattern: "([a-z])([A-Z])")]
    public static partial Regex KebabCase();
    #endregion

    #region Common Patterns
    /// <summary>
    /// Validates username format (alphanumeric, underscore, 3-16 chars).
    /// </summary>
    [GeneratedRegex(pattern: @"^[a-zA-Z0-9_]{3,16}$", options: RegexOptions.Compiled)]
    public static partial Regex UsernameValidation();

    /// <summary>
    /// Validates slug format (lowercase, numbers, hyphens).
    /// </summary>
    [GeneratedRegex(pattern: @"^[a-z0-9]+(?:-[a-z0-9]+)*$", options: RegexOptions.Compiled)]
    public static partial Regex SlugValidation();

    /// <summary>
    /// Validates semantic version number (SemVer).
    /// </summary>
    [GeneratedRegex(
        pattern: @"^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex SemVerValidation();

    /// <summary>
    /// Validates latitude coordinates.
    /// </summary>
    [GeneratedRegex(
        pattern: @"^[-+]?([1-8]?\d(\.\d+)?|90(\.0+)?)$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex LatitudeValidation();

    /// <summary>
    /// Validates longitude coordinates.
    /// </summary>
    [GeneratedRegex(
        pattern: @"^[-+]?(180(\.0+)?|((1[0-7]\d)|([1-9]?\d))(\.\d+)?)$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex LongitudeValidation();

    /// <summary>
    /// Validates MAC addresses.
    /// </summary>
    [GeneratedRegex(
        pattern: @"^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex MacAddressValidation();

    /// <summary>
    /// Validates GUID/UUID format.
    /// </summary>
    [GeneratedRegex(
        pattern: @"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex GuidValidation();

    /// <summary>
    /// Validates MongoDB ObjectId format.
    /// </summary>
    [GeneratedRegex(pattern: @"^[0-9a-fA-F]{24}$", options: RegexOptions.Compiled)]
    public static partial Regex MongoObjectIdValidation();

    /// <summary>
    /// Validates time duration format (e.g., 1h 30m, 90s).
    /// </summary>
    [GeneratedRegex(pattern: @"^(\d+d)?(\d+h)?(\d+m)?(\d+s)?$", options: RegexOptions.Compiled)]
    public static partial Regex TimeDurationValidation();

    /// <summary>
    /// Validates RGB color format (e.g., rgb(255, 128, 0)).
    /// </summary>
    [GeneratedRegex(
        pattern: @"^rgb\(\s*(\d{1,3})\s*,\s*(\d{1,3})\s*,\s*(\d{1,3})\s*\)$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex RgbColorValidation();

    /// <summary>
    /// Validates RGBA color format (e.g., rgba(255, 128, 0, 0.5)).
    /// </summary>
    [GeneratedRegex(
        pattern: @"^rgba\(\s*(\d{1,3})\s*,\s*(\d{1,3})\s*,\s*(\d{1,3})\s*,\s*([01]|0?\.\d+)\s*\)$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex RgbaColorValidation();

    /// <summary>
    /// Validates HSL color format (e.g., hsl(360, 100%, 50%)).
    /// </summary>
    [GeneratedRegex(
        pattern: @"^hsl\(\s*(\d{1,3})\s*,\s*(\d{1,3})%\s*,\s*(\d{1,3})%\s*\)$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex HslColorValidation();

    /// <summary>
    /// Validates HSLA color format (e.g., hsla(360, 100%, 50%, 0.5)).
    /// </summary>
    [GeneratedRegex(
        pattern: @"^hsla\(\s*(\d{1,3})\s*,\s*(\d{1,3})%\s*,\s*(\d{1,3})%\s*,\s*([01]|0?\.\d+)\s*\)$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex HslaColorValidation();

    /// <summary>
    /// Validates XML tag format.
    /// </summary>
    [GeneratedRegex(
        pattern: @"<([a-z]+)([^<]+)*(?:>(.*)<\/\1>|\s+\/>)",
        options: RegexOptions.Compiled
    )]
    public static partial Regex XmlTagValidation();

    /// <summary>
    /// Validates markdown link format.
    /// </summary>
    [GeneratedRegex(pattern: @"\[([^\]]+)\]\(([^)]+)\)", options: RegexOptions.Compiled)]
    public static partial Regex MarkdownLinkValidation();

    /// <summary>
    /// Validates markdown image format.
    /// </summary>
    [GeneratedRegex(pattern: @"!\[([^\]]+)\]\(([^)]+)\)", options: RegexOptions.Compiled)]
    public static partial Regex MarkdownImageValidation();

    /// <summary>
    /// Validates Twitter handle format.
    /// </summary>
    [GeneratedRegex(pattern: @"^@[A-Za-z0-9_]{1,15}$", options: RegexOptions.Compiled)]
    public static partial Regex TwitterHandleValidation();

    /// <summary>
    /// Validates Instagram handle format.
    /// </summary>
    [GeneratedRegex(
        pattern: @"^@(?!.*\.\.)(?!.*\.$)[^\W][\w.]{0,29}$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex InstagramHandleValidation();

    /// <summary>
    /// Validates YouTube video ID format.
    /// </summary>
    [GeneratedRegex(pattern: @"^[a-zA-Z0-9_-]{11}$", options: RegexOptions.Compiled)]
    public static partial Regex YoutubeVideoIdValidation();

    /// <summary>
    /// Validates Bitcoin address format.
    /// </summary>
    [GeneratedRegex(pattern: @"^[13][a-km-zA-HJ-NP-Z1-9]{25,34}$", options: RegexOptions.Compiled)]
    public static partial Regex BitcoinAddressValidation();

    /// <summary>
    /// Validates Ethereum address format.
    /// </summary>
    [GeneratedRegex(pattern: @"^0x[a-fA-F0-9]{40}$", options: RegexOptions.Compiled)]
    public static partial Regex EthereumAddressValidation();

    /// <summary>
    /// Validates IBAN (International Bank Account Number) format.
    /// </summary>
    [GeneratedRegex(
        pattern: @"^([A-Z]{2}[ \-]?[0-9]{2})(?=(?:[ \-]?[A-Z0-9]){9,30}$)((?:[ \-]?[A-Z0-9]{3,5}){2,7})([ \-]?[A-Z0-9]{1,3})?$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex IbanValidation();

    /// <summary>
    /// Validates SWIFT/BIC code format.
    /// </summary>
    [GeneratedRegex(
        pattern: @"^[A-Z]{6}[A-Z0-9]{2}([A-Z0-9]{3})?$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex SwiftCodeValidation();
    #endregion

    #region Banking Validation
    /// <summary>
    /// Validates Account Number format (general format 10-20 digits).
    /// </summary>
    [GeneratedRegex(pattern: @"^\d{10,20}$", options: RegexOptions.Compiled)]
    public static partial Regex BankAccountNumberValidation();

    /// <summary>
    /// Validates Indonesian Bank Account Number format (10-16 digits).
    /// </summary>
    [GeneratedRegex(pattern: @"^\d{10,16}$", options: RegexOptions.Compiled)]
    public static partial Regex IndonesianBankAccountValidation();

    /// <summary>
    /// Validates Reference Number format (typically used in banking transactions).
    /// </summary>
    [GeneratedRegex(pattern: @"^[A-Z0-9]{6,20}$", options: RegexOptions.Compiled)]
    public static partial Regex ReferenceNumberValidation();

    /// <summary>
    /// Validates Transaction ID format.
    /// </summary>
    [GeneratedRegex(pattern: @"^[A-Z0-9]{8,32}$", options: RegexOptions.Compiled)]
    public static partial Regex TransactionIdValidation();

    /// <summary>
    /// Validates Indonesian Virtual Account Number format (prefix + account number).
    /// </summary>
    [GeneratedRegex(pattern: @"^\d{3}\d{8,16}$", options: RegexOptions.Compiled)]
    public static partial Regex VirtualAccountNumberValidation();

    /// <summary>
    /// Validates Indonesian Bank Code (3 digits).
    /// </summary>
    [GeneratedRegex(pattern: @"^\d{3}$", options: RegexOptions.Compiled)]
    public static partial Regex BankCodeValidation();

    /// <summary>
    /// Validates Indonesian Bank Branch Code (4 digits).
    /// </summary>
    [GeneratedRegex(pattern: @"^\d{4}$", options: RegexOptions.Compiled)]
    public static partial Regex BranchCodeValidation();

    /// <summary>
    /// Validates Indonesian Currency Amount format (with optional decimals).
    /// </summary>
    [GeneratedRegex(
        pattern: @"^Rp\s?\d{1,3}(?:\.\d{3})*(?:,\d{2})?$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex IndonesianCurrencyValidation();

    /// <summary>
    /// Validates Bill Key format (used in bill payments).
    /// </summary>
    [GeneratedRegex(pattern: @"^\d{6,20}$", options: RegexOptions.Compiled)]
    public static partial Regex BillKeyValidation();

    /// <summary>
    /// Validates Biller Code format (used in bill payments).
    /// </summary>
    [GeneratedRegex(pattern: @"^\d{3,10}$", options: RegexOptions.Compiled)]
    public static partial Regex BillerCodeValidation();

    /// <summary>
    /// Validates Customer Number format (used in utilities/bill payments).
    /// </summary>
    [GeneratedRegex(pattern: @"^\d{6,20}$", options: RegexOptions.Compiled)]
    public static partial Regex CustomerNumberValidation();

    /// <summary>
    /// Validates Indonesian Tax ID Number (NPWP) format.
    /// </summary>
    [GeneratedRegex(
        pattern: @"^\d{2}\.\d{3}\.\d{3}\.\d{1}-\d{3}\.\d{3}$",
        options: RegexOptions.Compiled
    )]
    public static partial Regex NpwpValidation();

    /// <summary>
    /// Validates Indonesian ID Card Number (KTP/NIK) format.
    /// </summary>
    [GeneratedRegex(pattern: @"^\d{16}$", options: RegexOptions.Compiled)]
    public static partial Regex KtpValidation();

    /// <summary>
    /// Validates Indonesian Passport Number format.
    /// </summary>
    [GeneratedRegex(pattern: @"^[A-Z]{1,2}\d{6,7}$", options: RegexOptions.Compiled)]
    public static partial Regex PassportNumberValidation();

    /// <summary>
    /// Validates OTP (One-Time Password) format (6 digits).
    /// </summary>
    [GeneratedRegex(pattern: @"^\d{6}$", options: RegexOptions.Compiled)]
    public static partial Regex OtpValidation();

    /// <summary>
    /// Validates Indonesian Mobile Banking PIN format (6 digits).
    /// </summary>
    [GeneratedRegex(pattern: @"^\d{6}$", options: RegexOptions.Compiled)]
    public static partial Regex MobileBankingPinValidation();

    /// <summary>
    /// Validates Card Verification Value (CVV/CVC) format (3-4 digits).
    /// </summary>
    [GeneratedRegex(pattern: @"^\d{3,4}$", options: RegexOptions.Compiled)]
    public static partial Regex CvvValidation();

    /// <summary>
    /// Validates Card Expiry Date format (MM/YY).
    /// </summary>
    [GeneratedRegex(pattern: @"^(0[1-9]|1[0-2])\/([0-9]{2})$", options: RegexOptions.Compiled)]
    public static partial Regex CardExpiryValidation();

    /// <summary>
    /// Validates Indonesian Bank Transfer Code format.
    /// </summary>
    [GeneratedRegex(pattern: @"^\d{3}\/\d{3}\/\d{5}$", options: RegexOptions.Compiled)]
    public static partial Regex BankTransferCodeValidation();

    /// <summary>
    /// Validates Indonesian E-money Card Number format.
    /// </summary>
    [GeneratedRegex(pattern: @"^\d{16}$", options: RegexOptions.Compiled)]
    public static partial Regex EmoneyCardNumberValidation();

    /// <summary>
    /// Validates Indonesian QR Payment Code format.
    /// </summary>
    [GeneratedRegex(pattern: @"^[0-9A-Z]{8,}$", options: RegexOptions.Compiled)]
    public static partial Regex QrPaymentCodeValidation();

    /// <summary>
    /// Validates Indonesian QRIS format.
    /// </summary>
    [GeneratedRegex(pattern: @"^00020101.*63[0-9]{2}$", options: RegexOptions.Compiled)]
    public static partial Regex QrisValidation();
    #endregion
}
