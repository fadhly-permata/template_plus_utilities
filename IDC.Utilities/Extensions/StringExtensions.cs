using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace IDC.Utilities.Extensions;

/// <summary>
/// Provides extension methods for string manipulation and validation.
/// </summary>
/// <remarks>
/// Collection of thread-safe extension methods for common string operations including:
/// - Text transformation (case conversion, truncation)
/// - Character manipulation (whitespace removal, reversal)
/// - Encoding/decoding (Base64, MD5)
/// - Unicode handling (diacritics removal)
/// - Character extraction (numbers, letters)
///
/// Example usage:
/// <code>
/// string text = "Hello World";
/// string truncated = text.Truncate(maxLength: 7); // "Hello..."
/// string snakeCase = text.ToSnakeCase(); // "hello_world"
/// string base64 = text.ToBase64(); // "SGVsbG8gV29ybGQ="
/// </code>
/// </remarks>
public static class StringExtensions
{
    /// <summary>
    /// Truncates a string to a specified length and adds a suffix if truncated.
    /// </summary>
    /// <param name="value">The string to truncate.</param>
    /// <param name="maxLength">The maximum allowed length of the resulting string, excluding the suffix.</param>
    /// <param name="suffix">The string to append when truncation occurs. Defaults to "...".</param>
    /// <returns>
    /// The truncated string with suffix if truncated, or the original string if shorter than maxLength.
    /// If input is null, returns an empty string.
    /// </returns>
    /// <remarks>
    /// > [!NOTE]
    /// > The suffix is only added if the string is actually truncated.
    ///
    /// > [!IMPORTANT]
    /// > The maxLength parameter represents the length before adding the suffix.
    ///
    /// Example usage:
    /// <code>
    /// string text = "This is a very long text";
    /// string result1 = text.Truncate(maxLength: 10); // "This is..."
    /// string result2 = text.Truncate(maxLength: 10, suffix: "...more"); // "This is...more"
    /// string result3 = "Short".Truncate(maxLength: 10); // "Short"
    /// string result4 = null.Truncate(maxLength: 10); // ""
    /// </code>
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when maxLength is negative.</exception>
    public static string Truncate(this string? value, int maxLength, string suffix = "...") =>
        string.IsNullOrEmpty(value: value) || value.Length <= maxLength
            ? value ?? string.Empty
            : value[..maxLength] + suffix;

    /// <summary>
    /// Removes all whitespace characters from a string.
    /// </summary>
    /// <param name="value">The string to process.</param>
    /// <returns>
    /// A new string with all whitespace characters removed.
    /// If input is null or empty, returns an empty string.
    /// </returns>
    /// <remarks>
    /// > [!NOTE]
    /// > Whitespace includes spaces, tabs, line breaks, and other Unicode whitespace characters.
    ///
    /// Example usage:
    /// <code>
    /// string text = "Hello  World\n\t!";
    /// string result1 = text.RemoveWhitespace(); // "HelloWorld!"
    /// string result2 = "   ".RemoveWhitespace(); // ""
    /// string result3 = null.RemoveWhitespace(); // ""
    /// </code>
    /// </remarks>
    public static string RemoveWhitespace(this string? value) =>
        string.IsNullOrEmpty(value: value)
            ? string.Empty
            : new string(value: [.. value.Where(predicate: static c => !char.IsWhiteSpace(c: c))]);

    /// <summary>
    /// Converts a string to snake_case format.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>
    /// A new string in snake_case format.
    /// If input is null or empty, returns an empty string.
    /// </returns>
    /// <remarks>
    /// > [!NOTE]
    /// > Converts PascalCase or camelCase to snake_case by:
    /// > - Inserting underscore before capital letters
    /// > - Converting all characters to lowercase
    ///
    /// Example usage:
    /// <code>
    /// string text1 = "HelloWorld";
    /// string result1 = text1.ToSnakeCase(); // "hello_world"
    ///
    /// string text2 = "API_Version";
    /// string result2 = text2.ToSnakeCase(); // "api_version"
    ///
    /// string text3 = "myVariableName";
    /// string result3 = text3.ToSnakeCase(); // "my_variable_name"
    /// </code>
    /// </remarks>
    public static string ToSnakeCase(this string? value) =>
        string.IsNullOrEmpty(value: value)
            ? string.Empty
            : RegexPatternCollections
                .SnakeCase()
                .Replace(input: value, replacement: "$1_$2")
                .ToLower();

    /// <summary>
    /// Converts a string to kebab-case format.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>
    /// A new string in kebab-case format.
    /// If input is null or empty, returns an empty string.
    /// </returns>
    /// <remarks>
    /// > [!NOTE]
    /// > Converts PascalCase or camelCase to kebab-case by:
    /// > - Inserting hyphens before capital letters
    /// > - Converting all characters to lowercase
    ///
    /// Example usage:
    /// <code>
    /// string text1 = "HelloWorld";
    /// string result1 = text1.ToKebabCase(); // "hello-world"
    ///
    /// string text2 = "API_Version";
    /// string result2 = text2.ToKebabCase(); // "api-version"
    ///
    /// string text3 = "myVariableName";
    /// string result3 = text3.ToKebabCase(); // "my-variable-name"
    /// </code>
    /// </remarks>
    public static string ToKebabCase(this string? value) =>
        string.IsNullOrEmpty(value: value)
            ? string.Empty
            : RegexPatternCollections
                .KebabCase()
                .Replace(input: value, replacement: "$1-$2")
                .ToLower();

    /// <summary>
    /// Converts a string to Title Case format.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>
    /// A new string in Title Case format.
    /// If input is null or empty, returns an empty string.
    /// </returns>
    /// <remarks>
    /// > [!NOTE]
    /// > Uses the current culture's title casing rules.
    ///
    /// > [!TIP]
    /// > For culture-specific title casing, set the current thread's culture first.
    ///
    /// Example usage:
    /// <code>
    /// string text1 = "hello world";
    /// string result1 = text1.ToTitleCase(); // "Hello World"
    ///
    /// string text2 = "THE QUICK BROWN FOX";
    /// string result2 = text2.ToTitleCase(); // "The Quick Brown Fox"
    ///
    /// string text3 = "this is a MIXED case STRING";
    /// string result3 = text3.ToTitleCase(); // "This Is A Mixed Case String"
    /// </code>
    /// </remarks>
    public static string ToTitleCase(this string? value) =>
        string.IsNullOrEmpty(value: value)
            ? string.Empty
            : Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(str: value.ToLower());

    /// <summary>
    /// Reverses the characters in a string.
    /// </summary>
    /// <param name="value">The string to reverse.</param>
    /// <returns>
    /// A new string with characters in reverse order.
    /// If input is null or empty, returns an empty string.
    /// </returns>
    /// <remarks>
    /// > [!CAUTION]
    /// > This method may not handle Unicode surrogate pairs correctly.
    /// > For proper Unicode handling, consider using a specialized text processing library.
    ///
    /// Example usage:
    /// <code>
    /// string text1 = "Hello";
    /// string result1 = text1.Reverse(); // "olleH"
    ///
    /// string text2 = "12345";
    /// string result2 = text2.Reverse(); // "54321"
    ///
    /// string text3 = "";
    /// string result3 = text3.Reverse(); // ""
    /// </code>
    /// </remarks>
    public static string Reverse(this string? value) =>
        string.IsNullOrEmpty(value: value)
            ? string.Empty
            : new string(value: [.. value.ToCharArray().Reverse()]);

    /// <summary>
    /// Converts the first character of a string to uppercase.
    /// </summary>
    /// <param name="value">The string to process.</param>
    /// <returns>
    /// A new string with the first character in uppercase.
    /// If input is null or empty, returns an empty string.
    /// </returns>
    /// <remarks>
    /// > [!NOTE]
    /// > Only the first character is modified; all other characters remain unchanged.
    ///
    /// Example usage:
    /// <code>
    /// string text1 = "hello";
    /// string result1 = text1.FirstCharToUpper(); // "Hello"
    ///
    /// string text2 = "WORLD";
    /// string result2 = text2.FirstCharToUpper(); // "WORLD"
    ///
    /// string text3 = "a";
    /// string result3 = text3.FirstCharToUpper(); // "A"
    /// </code>
    /// </remarks>
    public static string FirstCharToUpper(this string? value) =>
        string.IsNullOrEmpty(value: value) ? string.Empty : char.ToUpper(c: value[0]) + value[1..];

    /// <summary>
    /// Converts the first character of a string to lowercase.
    /// </summary>
    /// <param name="value">The string to process.</param>
    /// <returns>
    /// A new string with the first character in lowercase.
    /// If input is null or empty, returns an empty string.
    /// </returns>
    /// <remarks>
    /// > [!NOTE]
    /// > Only the first character is modified; all other characters remain unchanged.
    ///
    /// Example usage:
    /// <code>
    /// string text1 = "Hello";
    /// string result1 = text1.FirstCharToLower(); // "hello"
    ///
    /// string text2 = "WORLD";
    /// string result2 = text2.FirstCharToLower(); // "wORLD"
    ///
    /// string text3 = "A";
    /// string result3 = text3.FirstCharToLower(); // "a"
    /// </code>
    /// </remarks>
    public static string FirstCharToLower(this string? value) =>
        string.IsNullOrEmpty(value: value) ? string.Empty : char.ToLower(c: value[0]) + value[1..];

    /// <summary>
    /// Removes diacritical marks from characters in a string.
    /// </summary>
    /// <param name="value">The string to process.</param>
    /// <returns>
    /// A new string with diacritical marks removed.
    /// If input is null or empty, returns an empty string.
    /// </returns>
    /// <remarks>
    /// > [!NOTE]
    /// > This method:
    /// > - Normalizes the string to decomposed form (FormD)
    /// > - Removes combining diacritical marks
    /// > - Normalizes back to composed form (FormC)
    ///
    /// Example usage:
    /// <code>
    /// string text1 = "résumé";
    /// string result1 = text1.RemoveDiacritics(); // "resume"
    ///
    /// string text2 = "über";
    /// string result2 = text2.RemoveDiacritics(); // "uber"
    ///
    /// string text3 = "piñata";
    /// string result3 = text3.RemoveDiacritics(); // "pinata"
    /// </code>
    /// </remarks>
    public static string RemoveDiacritics(this string? value)
    {
        if (string.IsNullOrEmpty(value: value))
            return string.Empty;

        var normalizedString = value.Normalize(normalizationForm: NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
            if (CharUnicodeInfo.GetUnicodeCategory(ch: c) != UnicodeCategory.NonSpacingMark)
                stringBuilder.Append(value: c);

        return stringBuilder.ToString().Normalize(normalizationForm: NormalizationForm.FormC);
    }

    /// <summary>
    /// Extracts all numeric characters from a string.
    /// </summary>
    /// <param name="value">The string to process.</param>
    /// <returns>
    /// A new string containing only numeric characters.
    /// If input is null or empty, returns an empty string.
    /// </returns>
    /// <remarks>
    /// > [!NOTE]
    /// > Only extracts 0-9 digits. Does not handle:
    /// > - Decimal points
    /// > - Negative signs
    /// > - Scientific notation
    /// > - Unicode digits from other scripts
    ///
    /// Example usage:
    /// <code>
    /// string text1 = "ABC123DEF456";
    /// string result1 = text1.ExtractNumbers(); // "123456"
    ///
    /// string text2 = "Phone: +1-555-0123";
    /// string result2 = text2.ExtractNumbers(); // "15550123"
    ///
    /// string text3 = "No numbers here";
    /// string result3 = text3.ExtractNumbers(); // ""
    /// </code>
    /// </remarks>
    public static string ExtractNumbers(this string? value) =>
        string.IsNullOrEmpty(value: value)
            ? string.Empty
            : new string(value: [.. value.Where(predicate: char.IsDigit)]);

    /// <summary>
    /// Extracts all letter characters from a string.
    /// </summary>
    /// <param name="value">The string to process.</param>
    /// <returns>
    /// A new string containing only letter characters.
    /// If input is null or empty, returns an empty string.
    /// </returns>
    /// <remarks>
    /// > [!NOTE]
    /// > Uses Unicode letter categories to determine what constitutes a letter.
    /// > Includes letters from all scripts, not just Latin alphabet.
    ///
    /// Example usage:
    /// <code>
    /// string text1 = "ABC123DEF456";
    /// string result1 = text1.ExtractLetters(); // "ABCDEF"
    ///
    /// string text2 = "Hello, World! 123";
    /// string result2 = text2.ExtractLetters(); // "HelloWorld"
    ///
    /// string text3 = "12345";
    /// string result3 = text3.ExtractLetters(); // ""
    /// </code>
    /// </remarks>
    public static string ExtractLetters(this string? value) =>
        string.IsNullOrEmpty(value: value)
            ? string.Empty
            : new string(value: [.. value.Where(predicate: char.IsLetter)]);

    /// <summary>
    /// Converts a string to its Base64 encoded representation.
    /// </summary>
    /// <param name="value">The string to encode.</param>
    /// <returns>
    /// A Base64 encoded string.
    /// If input is null or empty, returns an empty string.
    /// </returns>
    /// <remarks>
    /// > [!NOTE]
    /// > Uses UTF-8 encoding for the string before Base64 conversion.
    ///
    /// > [!IMPORTANT]
    /// > The output string length will be approximately 33% larger than the input.
    ///
    /// Example usage:
    /// <code>
    /// string text1 = "Hello";
    /// string result1 = text1.ToBase64(); // "SGVsbG8="
    ///
    /// string text2 = "Hello World";
    /// string result2 = text2.ToBase64(); // "SGVsbG8gV29ybGQ="
    ///
    /// string text3 = "";
    /// string result3 = text3.ToBase64(); // ""
    /// </code>
    /// </remarks>
    public static string ToBase64(this string? value) =>
        string.IsNullOrEmpty(value: value)
            ? string.Empty
            : Convert.ToBase64String(inArray: Encoding.UTF8.GetBytes(s: value));

    /// <summary>
    /// Decodes a Base64 encoded string back to its original form.
    /// </summary>
    /// <param name="value">The Base64 encoded string to decode.</param>
    /// <returns>
    /// The decoded string.
    /// If input is null or empty, returns an empty string.
    /// </returns>
    /// <remarks>
    /// > [!NOTE]
    /// > Uses UTF-8 encoding for the decoded bytes.
    ///
    /// > [!WARNING]
    /// > Will throw FormatException if the input is not valid Base64.
    ///
    /// Example usage:
    /// <code>
    /// string base64_1 = "SGVsbG8=";
    /// string result1 = base64_1.FromBase64(); // "Hello"
    ///
    /// string base64_2 = "SGVsbG8gV29ybGQ=";
    /// string result2 = base64_2.FromBase64(); // "Hello World"
    ///
    /// string base64_3 = "";
    /// string result3 = base64_3.FromBase64(); // ""
    /// </code>
    /// </remarks>
    /// <exception cref="FormatException">Thrown when input is not a valid Base64 string.</exception>
    public static string FromBase64(this string? value) =>
        string.IsNullOrEmpty(value: value)
            ? string.Empty
            : Encoding.UTF8.GetString(bytes: Convert.FromBase64String(s: value));

    /// <summary>
    /// Converts a string to its MD5 hash representation.
    /// </summary>
    /// <param name="value">The string to be hashed.</param>
    /// <returns>
    /// A lowercase hexadecimal string representing the MD5 hash.
    /// If input is null or empty, returns an empty string.
    /// </returns>
    /// <remarks>
    /// > [!IMPORTANT]
    /// > MD5 is cryptographically broken and should not be used for secure hashing.
    /// > Consider using <see cref="ToSHA256"/> for secure hashing needs.
    ///
    /// > [!NOTE]
    /// > - Uses UTF-8 encoding for string conversion
    /// > - Returns lowercase hexadecimal string
    /// > - Output length is always 32 characters
    ///
    /// Example usage:
    /// <code>
    /// string text1 = "password123";
    /// string hash1 = text1.ToMD5(); // "482c811da5d5b4bc6d497ffa98491e38"
    ///
    /// string text2 = "";
    /// string hash2 = text2.ToMD5(); // ""
    /// </code>
    /// </remarks>
    /// <seealso cref="ToSHA256"/>
    public static string ToMD5(this string? value)
    {
        if (string.IsNullOrEmpty(value: value))
            return string.Empty;
        return Convert
            .ToHexString(inArray: MD5.HashData(source: Encoding.UTF8.GetBytes(s: value)))
            .ToLower();
    }

    /// <summary>
    /// Converts a string to its SHA256 hash representation.
    /// </summary>
    /// <param name="value">The string to be hashed.</param>
    /// <returns>
    /// A lowercase hexadecimal string representing the SHA256 hash.
    /// If input is null or empty, returns an empty string.
    /// </returns>
    /// <remarks>
    /// > [!NOTE]
    /// > - Uses UTF-8 encoding for string conversion
    /// > - Returns lowercase hexadecimal string
    /// > - Output length is always 64 characters
    /// > - Suitable for secure hashing needs
    ///
    /// Example usage:
    /// <code>
    /// string text1 = "password123";
    /// string hash1 = text1.ToSHA256(); // "ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f"
    ///
    /// string text2 = "";
    /// string hash2 = text2.ToSHA256(); // ""
    /// </code>
    /// </remarks>
    /// <seealso cref="ToMD5"/>
    public static string ToSHA256(this string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;
        return Convert
            .ToHexString(inArray: SHA256.HashData(source: Encoding.UTF8.GetBytes(s: value)))
            .ToLower();
    }

    /// <summary>
    /// Masks a portion of the string with a specified character.
    /// </summary>
    /// <param name="value">The string to mask.</param>
    /// <param name="startIndex">Zero-based starting position of the masking.</param>
    /// <param name="length">Number of characters to mask.</param>
    /// <param name="maskChar">Character to use for masking. Defaults to '*'.</param>
    /// <returns>
    /// A new string with specified portion masked.
    /// If input is null or empty, returns an empty string.
    /// If indices are invalid, returns original string.
    /// </returns>
    /// <remarks>
    /// > [!NOTE]
    /// > - Preserves original string length
    /// > - Returns original string if indices are invalid
    /// > - Thread-safe operation
    ///
    /// Example usage:
    /// <code>
    /// string text1 = "1234567890";
    /// string result1 = text1.Mask(startIndex: 4, length: 4); // "1234****90"
    ///
    /// string text2 = "password";
    /// string result2 = text2.Mask(startIndex: 2, length: 4, maskChar: '#'); // "pa####rd"
    ///
    /// string text3 = "short";
    /// string result3 = text3.Mask(startIndex: 10, length: 2); // "short" (invalid index)
    /// </code>
    /// </remarks>
    public static string Mask(this string? value, int startIndex, int length, char maskChar = '*')
    {
        if (string.IsNullOrEmpty(value: value))
            return string.Empty;

        if (startIndex < 0 || length < 0 || startIndex + length > value.Length)
            return value;

        var chars = value.ToCharArray();
        for (int i = startIndex; i < startIndex + length; i++)
            chars[i] = maskChar;

        return new string(value: chars);
    }

    /// <summary>
    /// Determines if a string contains only numeric characters (0-9).
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <returns>
    /// true if string contains only numeric characters; otherwise, false.
    /// Returns false if string is null or empty.
    /// </returns>
    /// <remarks>
    /// > [!NOTE]
    /// > - Checks only basic digits (0-9)
    /// > - Does not allow decimal points or signs
    /// > - Returns false for empty strings
    ///
    /// Example usage:
    /// <code>
    /// string text1 = "12345";
    /// bool result1 = text1.IsNumeric(); // true
    ///
    /// string text2 = "12.34";
    /// bool result2 = text2.IsNumeric(); // false (contains decimal point)
    ///
    /// string text3 = "-123";
    /// bool result3 = text3.IsNumeric(); // false (contains minus sign)
    /// </code>
    /// </remarks>
    /// <seealso cref="IsAlphabetic"/>
    /// <seealso cref="IsAlphanumeric"/>
    public static bool IsNumeric(this string? value) =>
        !string.IsNullOrEmpty(value: value) && value.All(predicate: char.IsDigit);

    /// <summary>
    /// Determines if a string contains only alphabetic characters (A-Z, a-z).
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <returns>
    /// true if string contains only alphabetic characters; otherwise, false.
    /// Returns false if string is null or empty.
    /// </returns>
    /// <remarks>
    /// > [!NOTE]
    /// > - Includes both uppercase and lowercase letters
    /// > - Supports Unicode letters from all languages
    /// > - Returns false for empty strings
    ///
    /// Example usage:
    /// <code>
    /// string text1 = "Hello";
    /// bool result1 = text1.IsAlphabetic(); // true
    ///
    /// string text2 = "Hello123";
    /// bool result2 = text2.IsAlphabetic(); // false (contains numbers)
    ///
    /// string text3 = "Hello World";
    /// bool result3 = text3.IsAlphabetic(); // false (contains space)
    /// </code>
    /// </remarks>
    /// <seealso cref="IsNumeric"/>
    /// <seealso cref="IsAlphanumeric"/>
    public static bool IsAlphabetic(this string? value) =>
        !string.IsNullOrEmpty(value: value) && value.All(predicate: char.IsLetter);

    /// <summary>
    /// Determines if a string contains only alphanumeric characters (A-Z, a-z, 0-9).
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <returns>
    /// true if string contains only alphanumeric characters; otherwise, false.
    /// Returns false if string is null or empty.
    /// </returns>
    /// <remarks>
    /// > [!NOTE]
    /// > - Includes letters and numbers
    /// > - Supports Unicode letters from all languages
    /// > - Returns false for empty strings
    ///
    /// Example usage:
    /// <code>
    /// string text1 = "Hello123";
    /// bool result1 = text1.IsAlphanumeric(); // true
    ///
    /// string text2 = "Hello 123";
    /// bool result2 = text2.IsAlphanumeric(); // false (contains space)
    ///
    /// string text3 = "Hello-123";
    /// bool result3 = text3.IsAlphanumeric(); // false (contains hyphen)
    /// </code>
    /// </remarks>
    /// <seealso cref="IsNumeric"/>
    /// <seealso cref="IsAlphabetic"/>
    public static bool IsAlphanumeric(this string? value) =>
        !string.IsNullOrEmpty(value: value) && value.All(predicate: char.IsLetterOrDigit);

    /// <summary>
    /// Repeats a string a specified number of times.
    /// </summary>
    /// <param name="value">The string to repeat.</param>
    /// <param name="count">Number of times to repeat the string.</param>
    /// <returns>
    /// A new string containing the input string repeated the specified number of times.
    /// If input is null or empty, returns an empty string.
    /// </returns>
    /// <remarks>
    /// > [!NOTE]
    /// > - Returns empty string for null/empty input
    /// > - Memory efficient implementation
    /// > - Preserves original string
    ///
    /// > [!WARNING]
    /// > Large count values may cause memory issues
    ///
    /// Example usage:
    /// <code>
    /// string text1 = "Ha";
    /// string result1 = text1.Repeat(count: 3); // "HaHaHa"
    ///
    /// string text2 = "ABC";
    /// string result2 = text2.Repeat(count: 2); // "ABCABC"
    ///
    /// string text3 = "";
    /// string result3 = text3.Repeat(count: 5); // ""
    /// </code>
    /// </remarks>
    public static string Repeat(this string? value, int count) =>
        string.IsNullOrEmpty(value: value)
            ? string.Empty
            : string.Concat(values: Enumerable.Repeat(element: value, count: count));

    /// <summary>
    /// Checks if a string matches a wildcard pattern.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="pattern">The wildcard pattern to match against.</param>
    /// <returns>
    /// true if the string matches the pattern; otherwise, false.
    /// Returns false if string is null or empty.
    /// </returns>
    /// <remarks>
    /// > [!NOTE]
    /// > Supports following wildcards:
    /// > - '*' matches zero or more characters
    /// > - '?' matches exactly one character
    ///
    /// > [!TIP]
    /// > Pattern matching is case-sensitive
    ///
    /// Example usage:
    /// <code>
    /// string text1 = "Hello.txt";
    /// bool result1 = text1.MatchesWildcard("*.txt"); // true
    ///
    /// string text2 = "config.json";
    /// bool result2 = text2.MatchesWildcard("config.*"); // true
    ///
    /// string text3 = "test.dat";
    /// bool result3 = text3.MatchesWildcard("test.???"); // true
    /// </code>
    /// </remarks>
    public static bool MatchesWildcard(this string? value, string pattern)
    {
        if (string.IsNullOrEmpty(value: value))
            return false;

        return Regex.IsMatch(
            input: value,
            pattern: $"^{Regex.Escape(str: pattern).Replace(oldValue: "\\*", newValue: ".*").Replace(oldValue: "\\?", newValue: ".")}$"
        );
    }

    /// <summary>
    /// Converts a string to proper case (first letter of each word capitalized).
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>
    /// A new string in proper case format.
    /// If input is null or empty, returns an empty string.
    /// </returns>
    /// <remarks>
    /// > [!NOTE]
    /// > - Capitalizes first letter of each word
    /// > - Converts remaining letters to lowercase
    /// > - Preserves existing spaces
    ///
    /// Example usage:
    /// <code>
    /// string text1 = "hello world";
    /// string result1 = text1.ToProperCase(); // "Hello World"
    ///
    /// string text2 = "JOHN DOE";
    /// string result2 = text2.ToProperCase(); // "John Doe"
    ///
    /// string text3 = "the QUICK brown FOX";
    /// string result3 = text3.ToProperCase(); // "The Quick Brown Fox"
    /// </code>
    /// </remarks>
    /// <seealso cref="ToCamelCase"/>
    /// <seealso cref="ToPascalCase"/>
    public static string ToProperCase(this string? value) =>
        string.IsNullOrEmpty(value: value)
            ? string.Empty
            : string.Join(
                separator: " ",
                values: value
                    .Split(separator: ' ')
                    .Select(selector: static word =>
                        word.Length > 0 ? char.ToUpper(c: word[0]) + word[1..].ToLower() : word
                    )
            );

    /// <summary>
    /// Converts a string to camelCase format.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>
    /// A new string in camelCase format.
    /// If input is null or empty, returns an empty string.
    /// </returns>
    /// <remarks>
    /// > [!NOTE]
    /// > - First word starts with lowercase
    /// > - Subsequent words start with uppercase
    /// > - Removes spaces, underscores, and hyphens
    ///
    /// Example usage:
    /// <code>
    /// string text1 = "Hello World";
    /// string result1 = text1.ToCamelCase(); // "helloWorld"
    ///
    /// string text2 = "user_first_name";
    /// string result2 = text2.ToCamelCase(); // "userFirstName"
    ///
    /// string text3 = "API-Version";
    /// string result3 = text3.ToCamelCase(); // "apiVersion"
    /// </code>
    /// </remarks>
    /// <seealso cref="ToProperCase"/>
    /// <seealso cref="ToPascalCase"/>
    public static string ToCamelCase(this string? value)
    {
        if (string.IsNullOrEmpty(value: value))
            return string.Empty;

        var words = value.Split(
            separator: [' ', '_', '-'],
            options: StringSplitOptions.RemoveEmptyEntries
        );

        var result = words[0].ToLower();
        for (int i = 1; i < words.Length; i++)
            result += char.ToUpper(c: words[i][0]) + words[i][1..].ToLower();
        return result;
    }

    /// <summary>
    /// Converts a string to PascalCase format.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>
    /// A new string in PascalCase format.
    /// If input is null or empty, returns an empty string.
    /// </returns>
    /// <remarks>
    /// > [!NOTE]
    /// > - Every word starts with uppercase
    /// > - Removes spaces, underscores, and hyphens
    /// > - Remaining letters are lowercase
    ///
    /// Example usage:
    /// <code>
    /// string text1 = "hello world";
    /// string result1 = text1.ToPascalCase(); // "HelloWorld"
    ///
    /// string text2 = "user_first_name";
    /// string result2 = text2.ToPascalCase(); // "UserFirstName"
    ///
    /// string text3 = "api-version";
    /// string result3 = text3.ToPascalCase(); // "ApiVersion"
    /// </code>
    /// </remarks>
    /// <seealso cref="ToProperCase"/>
    /// <seealso cref="ToCamelCase"/>
    public static string ToPascalCase(this string? value)
    {
        if (string.IsNullOrEmpty(value: value))
            return string.Empty;

        return string.Join(
            separator: "",
            values: value
                .Split(separator: [' ', '_', '-'], options: StringSplitOptions.RemoveEmptyEntries)
                .Select(selector: static word => char.ToUpper(c: word[0]) + word[1..].ToLower())
        );
    }
}
