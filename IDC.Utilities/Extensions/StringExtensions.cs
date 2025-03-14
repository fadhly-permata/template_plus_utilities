using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace IDC.Utilities.Extensions;

/// <summary>
/// Provides extension methods for string manipulation and validation.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Truncates a string to a specified length and adds an ellipsis if truncated.
    /// </summary>
    /// <param name="value">The string to truncate.</param>
    /// <param name="maxLength">The maximum length of the string.</param>
    /// <param name="suffix">The suffix to append if truncated.</param>
    /// <returns>The truncated string.</returns>
    /// <example>
    /// <code>
    /// string text = "This is a long text";
    /// string result = text.Truncate(maxLength: 10); // returns "This is..."
    /// </code>
    /// </example>
    public static string Truncate(this string? value, int maxLength, string suffix = "...") =>
        string.IsNullOrEmpty(value: value) || value.Length <= maxLength
            ? value ?? string.Empty
            : value[..maxLength] + suffix;

    /// <summary>
    /// Removes all whitespace from a string.
    /// </summary>
    /// <param name="value">The string to process.</param>
    /// <returns>String with all whitespace removed.</returns>
    /// <example>
    /// <code>
    /// string text = "Hello World";
    /// string result = text.RemoveWhitespace(); // returns "HelloWorld"
    /// </code>
    /// </example>
    public static string RemoveWhitespace(this string? value) =>
        string.IsNullOrEmpty(value: value)
            ? string.Empty
            : new string(value: [.. value.Where(predicate: static c => !char.IsWhiteSpace(c: c))]);

    /// <summary>
    /// Converts a string to snake_case.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>The snake_case string.</returns>
    /// <example>
    /// <code>
    /// string text = "HelloWorld";
    /// string result = text.ToSnakeCase(); // returns "hello_world"
    /// </code>
    /// </example>
    public static string ToSnakeCase(this string? value) =>
        string.IsNullOrEmpty(value: value)
            ? string.Empty
            : Regex
                .Replace(input: value, pattern: "([a-z])([A-Z])", replacement: "$1_$2")
                .ToLower();

    /// <summary>
    /// Converts a string to kebab-case.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>The kebab-case string.</returns>
    /// <example>
    /// <code>
    /// string text = "HelloWorld";
    /// string result = text.ToKebabCase(); // returns "hello-world"
    /// </code>
    /// </example>
    public static string ToKebabCase(this string? value) =>
        string.IsNullOrEmpty(value: value)
            ? string.Empty
            : Regex
                .Replace(input: value, pattern: "([a-z])([A-Z])", replacement: "$1-$2")
                .ToLower();

    /// <summary>
    /// Converts a string to Title Case.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>The Title Case string.</returns>
    /// <example>
    /// <code>
    /// string text = "hello world";
    /// string result = text.ToTitleCase(); // returns "Hello World"
    /// </code>
    /// </example>
    public static string ToTitleCase(this string? value) =>
        string.IsNullOrEmpty(value: value)
            ? string.Empty
            : Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(str: value.ToLower());

    /// <summary>
    /// Reverses a string.
    /// </summary>
    /// <param name="value">The string to reverse.</param>
    /// <returns>The reversed string.</returns>
    /// <example>
    /// <code>
    /// string text = "Hello";
    /// string result = text.Reverse(); // returns "olleH"
    /// </code>
    /// </example>
    public static string Reverse(this string? value) =>
        string.IsNullOrEmpty(value: value)
            ? string.Empty
            : new string(value: [.. value.ToCharArray().Reverse()]);

    /// <summary>
    /// Converts the first character of a string to uppercase.
    /// </summary>
    /// <param name="value">The string to process.</param>
    /// <returns>The string with first character in uppercase.</returns>
    /// <example>
    /// <code>
    /// string text = "hello";
    /// string result = text.FirstCharToUpper(); // returns "Hello"
    /// </code>
    /// </example>
    public static string FirstCharToUpper(this string? value) =>
        string.IsNullOrEmpty(value: value) ? string.Empty : char.ToUpper(c: value[0]) + value[1..];

    /// <summary>
    /// Converts the first character of a string to lowercase.
    /// </summary>
    /// <param name="value">The string to process.</param>
    /// <returns>The string with first character in lowercase.</returns>
    /// <example>
    /// <code>
    /// string text = "Hello";
    /// string result = text.FirstCharToLower(); // returns "hello"
    /// </code>
    /// </example>
    public static string FirstCharToLower(this string? value) =>
        string.IsNullOrEmpty(value: value) ? string.Empty : char.ToLower(c: value[0]) + value[1..];

    /// <summary>
    /// Removes diacritics from a string.
    /// </summary>
    /// <param name="value">The string to process.</param>
    /// <returns>The string without diacritics.</returns>
    /// <example>
    /// <code>
    /// string text = "résumé";
    /// string result = text.RemoveDiacritics(); // returns "resume"
    /// </code>
    /// </example>
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
    /// Extracts numbers from a string.
    /// </summary>
    /// <param name="value">The string to process.</param>
    /// <returns>A string containing only numbers.</returns>
    /// <example>
    /// <code>
    /// string text = "ABC123DEF456";
    /// string result = text.ExtractNumbers(); // returns "123456"
    /// </code>
    /// </example>
    public static string ExtractNumbers(this string? value) =>
        string.IsNullOrEmpty(value: value)
            ? string.Empty
            : new string(value: [.. value.Where(predicate: char.IsDigit)]);

    /// <summary>
    /// Extracts letters from a string.
    /// </summary>
    /// <param name="value">The string to process.</param>
    /// <returns>A string containing only letters.</returns>
    /// <example>
    /// <code>
    /// string text = "ABC123DEF456";
    /// string result = text.ExtractLetters(); // returns "ABCDEF"
    /// </code>
    /// </example>
    public static string ExtractLetters(this string? value) =>
        string.IsNullOrEmpty(value: value)
            ? string.Empty
            : new string(value: [.. value.Where(predicate: char.IsLetter)]);

    /// <summary>
    /// Converts a string to Base64.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>The Base64 encoded string.</returns>
    /// <example>
    /// <code>
    /// string text = "Hello";
    /// string result = text.ToBase64(); // returns "SGVsbG8="
    /// </code>
    /// </example>
    public static string ToBase64(this string? value) =>
        string.IsNullOrEmpty(value: value)
            ? string.Empty
            : Convert.ToBase64String(inArray: Encoding.UTF8.GetBytes(s: value));

    /// <summary>
    /// Converts a Base64 string back to a regular string.
    /// </summary>
    /// <param name="value">The Base64 string to convert.</param>
    /// <returns>The decoded string.</returns>
    /// <example>
    /// <code>
    /// string base64 = "SGVsbG8=";
    /// string result = base64.FromBase64(); // returns "Hello"
    /// </code>
    /// </example>
    public static string FromBase64(this string? value) =>
        string.IsNullOrEmpty(value: value)
            ? string.Empty
            : Encoding.UTF8.GetString(bytes: Convert.FromBase64String(s: value));

    /// <summary>
    /// Converts string to MD5 hash.
    /// </summary>
    /// <param name="value">The string to hash.</param>
    /// <returns>MD5 hash string.</returns>
    /// <example>
    /// <code>
    /// string text = "password123";
    /// string result = text.ToMD5(); // returns "482c811da5d5b4bc6d497ffa98491e38"
    /// </code>
    /// </example>
    public static string ToMD5(this string? value)
    {
        if (string.IsNullOrEmpty(value: value))
            return string.Empty;
        return Convert
            .ToHexString(inArray: MD5.HashData(source: Encoding.UTF8.GetBytes(s: value)))
            .ToLower();
    }

    /// <summary>
    /// Converts string to SHA256 hash.
    /// </summary>
    /// <param name="value">The string to hash.</param>
    /// <returns>SHA256 hash string.</returns>
    /// <example>
    /// <code>
    /// string text = "password123";
    /// string result = text.ToSHA256();
    /// </code>
    /// </example>
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
    /// <param name="startIndex">Start index of masking.</param>
    /// <param name="length">Length of characters to mask.</param>
    /// <param name="maskChar">Character to use for masking.</param>
    /// <returns>Masked string.</returns>
    /// <example>
    /// <code>
    /// string text = "1234567890";
    /// string result = text.Mask(startIndex: 4, length: 4); // returns "1234****90"
    /// </code>
    /// </example>
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
    /// Checks if string contains only numeric characters.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <returns>True if string contains only numbers.</returns>
    /// <example>
    /// <code>
    /// string text = "12345";
    /// bool result = text.IsNumeric(); // returns true
    /// </code>
    /// </example>
    public static bool IsNumeric(this string? value) =>
        !string.IsNullOrEmpty(value: value) && value.All(predicate: char.IsDigit);

    /// <summary>
    /// Checks if string contains only alphabetic characters.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <returns>True if string contains only letters.</returns>
    /// <example>
    /// <code>
    /// string text = "Hello";
    /// bool result = text.IsAlphabetic(); // returns true
    /// </code>
    /// </example>
    public static bool IsAlphabetic(this string? value) =>
        !string.IsNullOrEmpty(value: value) && value.All(predicate: char.IsLetter);

    /// <summary>
    /// Checks if string contains only alphanumeric characters.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <returns>True if string contains only letters and numbers.</returns>
    /// <example>
    /// <code>
    /// string text = "Hello123";
    /// bool result = text.IsAlphanumeric(); // returns true
    /// </code>
    /// </example>
    public static bool IsAlphanumeric(this string? value) =>
        !string.IsNullOrEmpty(value: value) && value.All(predicate: char.IsLetterOrDigit);

    /// <summary>
    /// Repeats a string a specified number of times.
    /// </summary>
    /// <param name="value">The string to repeat.</param>
    /// <param name="count">Number of times to repeat.</param>
    /// <returns>Repeated string.</returns>
    /// <example>
    /// <code>
    /// string text = "Ha";
    /// string result = text.Repeat(count: 3); // returns "HaHaHa"
    /// </code>
    /// </example>
    public static string Repeat(this string? value, int count) =>
        string.IsNullOrEmpty(value: value)
            ? string.Empty
            : string.Concat(values: Enumerable.Repeat(element: value, count: count));

    /// <summary>
    /// Checks if string matches a wildcard pattern.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="pattern">Wildcard pattern (* and ? supported).</param>
    /// <returns>True if string matches pattern.</returns>
    /// <example>
    /// <code>
    /// string text = "Hello.txt";
    /// bool result = text.MatchesWildcard(pattern: "*.txt"); // returns true
    /// </code>
    /// </example>
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
    /// Converts string to proper case (first letter of each word capitalized).
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>Proper case string.</returns>
    /// <example>
    /// <code>
    /// string text = "hello world";
    /// string result = text.ToProperCase(); // returns "Hello World"
    /// </code>
    /// </example>
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
    /// Converts string to camelCase.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>camelCase string.</returns>
    /// <example>
    /// <code>
    /// string text = "Hello World";
    /// string result = text.ToCamelCase(); // returns "helloWorld"
    /// </code>
    /// </example>
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
    /// Converts string to PascalCase.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>PascalCase string.</returns>
    /// <example>
    /// <code>
    /// string text = "hello world";
    /// string result = text.ToPascalCase(); // returns "HelloWorld"
    /// </code>
    /// </example>
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
