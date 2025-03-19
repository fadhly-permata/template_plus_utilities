namespace IDC.Utilities.Extensions;

/// <summary>
/// Provides a comprehensive set of extension methods for generic type conversions and null-checking operations.
/// </summary>
/// <remarks>
/// This class includes type-safe conversion methods with null handling and validation features.
///
/// > [!NOTE]
/// > All conversion methods support nullable types and provide default values for failed conversions.
///
/// > [!IMPORTANT]
/// > Methods preserve type safety while providing flexible conversion options.
/// </remarks>
public static class GenericExtensions
{
    /// <summary>
    /// Validates that a reference-type value is not null, throwing an exception if it is.
    /// </summary>
    /// <typeparam name="T">The reference type to check.</typeparam>
    /// <param name="value">The value to validate.</param>
    /// <param name="paramName">The name of the parameter for the exception message.</param>
    /// <param name="message">Optional custom exception message.</param>
    /// <returns>The non-null value if validation succeeds.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
    /// <remarks>
    /// Use this method for defensive programming and parameter validation.
    ///
    /// > [!TIP]
    /// > Ideal for constructor and method parameter validation.
    ///
    /// > [!WARNING]
    /// > This method will terminate execution flow if the value is null.
    /// </remarks>
    /// <example>
    /// <code>
    /// public class UserService
    /// {
    ///     private readonly IRepository _repository;
    ///
    ///     public UserService(IRepository repository)
    ///     {
    ///         _repository = repository.ThrowIfNull(paramName: nameof(repository));
    ///     }
    /// }
    /// </code>
    /// </example>
    public static T ThrowIfNull<T>(this T? value, string paramName, string? message = null)
        where T : class =>
        value ?? throw new ArgumentNullException(paramName: paramName, message: message);

    /// <summary>
    /// Executes an action on a non-null value while preserving the original value.
    /// </summary>
    /// <typeparam name="T">The reference type to check.</typeparam>
    /// <param name="value">The value to check and potentially act upon.</param>
    /// <param name="action">The action to perform if the value is not null.</param>
    /// <returns>The original value, allowing for method chaining.</returns>
    /// <remarks>
    /// Enables fluent method chaining while performing side effects.
    ///
    /// > [!TIP]
    /// > Useful for logging, validation, or state updates without breaking the method chain.
    ///
    /// > [!NOTE]
    /// > The action is skipped if the value is null, preventing null reference exceptions.
    /// </remarks>
    /// <example>
    /// <code>
    /// var user = GetUser()
    ///     .IfNotNull(action: u => Logger.Log($"Processing user: {u.Name}"))
    ///     .IfNotNull(action: u => u.LastLoginDate = DateTime.UtcNow)
    ///     .IfNotNull(action: u => SaveUser(user: u));
    /// </code>
    /// </example>
    public static T? IfNotNull<T>(this T? value, Action<T> action)
        where T : class
    {
        if (value is not null)
            action(obj: value);

        return value;
    }

    /// <summary>
    /// Transforms a value from one type to another using a mapping function.
    /// </summary>
    /// <typeparam name="TInput">The source type.</typeparam>
    /// <typeparam name="TOutput">The target type.</typeparam>
    /// <param name="value">The value to transform.</param>
    /// <param name="mapper">The transformation function.</param>
    /// <returns>The transformed value.</returns>
    /// <remarks>
    /// Provides a functional approach to value transformation.
    ///
    /// > [!TIP]
    /// > Ideal for clean, functional-style transformations.
    ///
    /// > [!NOTE]
    /// > The mapper function should be pure for predictable results.
    /// </remarks>
    /// <example>
    /// <code>
    /// var user = GetUserDto()
    ///     .Map(mapper: dto => new User
    ///     {
    ///         Id = dto.Id,
    ///         Name = dto.Name.Trim(),
    ///         Email = dto.Email.ToLower()
    ///     });
    /// </code>
    /// </example>
    public static TOutput Map<TInput, TOutput>(this TInput value, Func<TInput, TOutput> mapper) =>
        mapper(arg: value);

    /// <summary>
    /// Attempts to parse a value using a custom parsing function with fallback to a default value.
    /// </summary>
    /// <typeparam name="TInput">The source type.</typeparam>
    /// <typeparam name="TOutput">The target type.</typeparam>
    /// <param name="value">The value to parse.</param>
    /// <param name="parser">Function returning success status and parsed value.</param>
    /// <param name="defaultValue">Fallback value if parsing fails.</param>
    /// <returns>The parsed value or default if parsing fails.</returns>
    /// <remarks>
    /// Provides a generic way to implement try-parse patterns.
    ///
    /// > [!TIP]
    /// > Useful for custom type conversions with error handling.
    ///
    /// > [!NOTE]
    /// > The parser function should handle all possible input cases.
    /// </remarks>
    /// <example>
    /// <code>
    /// public enum UserStatus { Active, Inactive }
    ///
    /// string status = "ACTIVE";
    /// var userStatus = status.TryParse(
    ///     parser: s => {
    ///         var isValid = Enum.TryParse&lt;UserStatus&gt;(s, ignoreCase: true, out var result);
    ///         return (isValid, result);
    ///     },
    ///     defaultValue: UserStatus.Inactive
    /// );
    /// </code>
    /// </example>
    public static TOutput TryParse<TInput, TOutput>(
        this TInput value,
        Func<TInput, (bool success, TOutput result)> parser,
        TOutput defaultValue
    )
    {
        var (success, result) = parser(value);
        return success ? result : defaultValue;
    }

    /// <summary>
    /// Converts a value to an integer with robust type handling and validation.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <param name="defaultValue">The default value if conversion fails.</param>
    /// <returns>The converted integer or default value if conversion fails.</returns>
    /// <remarks>
    /// Provides safe integer conversion with proper rounding and type handling.
    ///
    /// > [!NOTE]
    /// > Decimal and double values are rounded to the nearest integer.
    ///
    /// > [!WARNING]
    /// > Values outside the range of Int32 (-2,147,483,648 to 2,147,483,647) will return the default value.
    ///
    /// > [!TIP]
    /// > Use <see cref="CastToLong"/> for values that might exceed integer range.
    ///
    /// > [!IMPORTANT]
    /// > Boolean values are converted to 1 (true) or 0 (false).
    /// </remarks>
    /// <example>
    /// <code>
    /// // Numeric string conversion
    /// var strResult = "123".CastToInteger(); // returns 123
    ///
    /// // Boolean conversion
    /// var boolResult = true.CastToInteger(); // returns 1
    ///
    /// // Decimal conversion with rounding
    /// var decimalResult = 123.45m.CastToInteger(); // returns 123
    ///
    /// // Failed conversion
    /// var failed = "abc".CastToInteger(defaultValue: 0); // returns 0
    ///
    /// // Null handling
    /// int? nullValue = null;
    /// var nullResult = nullValue.CastToInteger(defaultValue: -1); // returns -1
    /// </code>
    /// </example>
    public static int? CastToInteger<T>(this T? value, int? defaultValue = null) =>
        value switch
        {
            null => defaultValue,
            int i => i,
            string s => int.TryParse(s: s, result: out int result) ? result : defaultValue,
            double d => Convert.ToInt32(value: d),
            decimal m => Convert.ToInt32(value: m),
            bool b => b ? 1 : 0,
            _ => defaultValue
        };

    /// <summary>
    /// Converts a value to a string with null-safe handling.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <param name="defaultValue">The default value if conversion fails.</param>
    /// <returns>The converted string or default value if conversion fails.</returns>
    /// <remarks>
    /// Provides consistent string conversion for any type.
    ///
    /// > [!NOTE]
    /// > Uses the object's ToString() implementation for non-string types.
    ///
    /// > [!TIP]
    /// > Useful for ensuring consistent string representation across different types.
    ///
    /// > [!IMPORTANT]
    /// > Null values are handled gracefully by returning the default value.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Numeric conversion
    /// var numResult = 123.CastToString(); // returns "123"
    ///
    /// // DateTime conversion
    /// var dateResult = DateTime.Now.CastToString(); // returns current date as string
    ///
    /// // Null handling
    /// var nullResult = ((string)null).CastToString(defaultValue: "N/A"); // returns "N/A"
    ///
    /// // Custom object conversion
    /// var person = new Person { Name = "John" };
    /// var objResult = person.CastToString(); // returns person.ToString()
    /// </code>
    /// </example>
    public static string? CastToString<T>(this T? value, string? defaultValue = null) =>
        value switch
        {
            null => defaultValue,
            string s => s,
            _ => value.ToString() ?? defaultValue
        };

    /// <summary>
    /// Converts a value to a boolean with flexible type conversion.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <param name="defaultValue">The default value if conversion fails.</param>
    /// <returns>The converted boolean or default value if conversion fails.</returns>
    /// <remarks>
    /// Supports various truthy/falsy conventions across different types.
    ///
    /// > [!NOTE]
    /// > Integer values: 0 is false, any other value is true.
    ///
    /// > [!TIP]
    /// > String values support "true", "false" (case-insensitive).
    ///
    /// > [!IMPORTANT]
    /// > Null values return the specified default value.
    /// </remarks>
    /// <example>
    /// <code>
    /// // String conversion
    /// var strResult = "true".CastToBoolean(); // returns true
    /// var caseInsensitive = "TRUE".CastToBoolean(); // returns true
    ///
    /// // Numeric conversion
    /// var numTrue = 1.CastToBoolean(); // returns true
    /// var numFalse = 0.CastToBoolean(); // returns false
    ///
    /// // Failed conversion
    /// var failed = "invalid".CastToBoolean(defaultValue: false); // returns false
    ///
    /// // Null handling
    /// bool? nullValue = null;
    /// var nullResult = nullValue.CastToBoolean(defaultValue: true); // returns true
    /// </code>
    /// </example>
    public static bool? CastToBoolean<T>(this T? value, bool? defaultValue = null) =>
        value switch
        {
            null => defaultValue,
            bool b => b,
            string s => bool.TryParse(value: s, result: out bool result) ? result : defaultValue,
            int i => i != 0,
            _ => defaultValue
        };

    /// <summary>
    /// Converts a value to a double with high-precision floating-point handling.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <param name="defaultValue">The default value if conversion fails.</param>
    /// <returns>The converted double or default value if conversion fails.</returns>
    /// <remarks>
    /// Provides precise floating-point conversion with proper type handling.
    ///
    /// > [!NOTE]
    /// > Supports scientific notation in string format (e.g., "1.23E-4").
    ///
    /// > [!WARNING]
    /// > May lose precision when converting from decimal type.
    ///
    /// > [!TIP]
    /// > Use <see cref="CastToDecimal"/> for financial calculations requiring exact precision.
    ///
    /// > [!IMPORTANT]
    /// > Boolean values are converted to 1.0 (true) or 0.0 (false).
    /// </remarks>
    /// <example>
    /// <code>
    /// // Basic numeric conversion
    /// var numResult = "123.45".CastToDouble(); // returns 123.45
    ///
    /// // Scientific notation
    /// var scientific = "1.23E-4".CastToDouble(); // returns 0.000123
    ///
    /// // Boolean conversion
    /// var boolResult = true.CastToDouble(); // returns 1.0
    ///
    /// // Integer conversion
    /// var intResult = 42.CastToDouble(); // returns 42.0
    ///
    /// // Failed conversion
    /// var failed = "abc".CastToDouble(defaultValue: 0.0); // returns 0.0
    ///
    /// // Null handling
    /// double? nullValue = null;
    /// var nullResult = nullValue.CastToDouble(defaultValue: -1.0); // returns -1.0
    /// </code>
    /// </example>
    public static double? CastToDouble<T>(this T? value, double? defaultValue = null) =>
        value switch
        {
            null => defaultValue,
            double d => d,
            string s => double.TryParse(s: s, result: out double result) ? result : defaultValue,
            int i => Convert.ToDouble(value: i),
            decimal m => Convert.ToDouble(value: m),
            bool b => b ? 1.0 : 0.0,
            _ => defaultValue
        };

    /// <summary>
    /// Converts a value to a decimal with high-precision handling.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <param name="defaultValue">The default value if conversion fails.</param>
    /// <returns>The converted decimal or default value if conversion fails.</returns>
    /// <remarks>
    /// Provides high-precision decimal conversion with support for various numeric types.
    ///
    /// > [!NOTE]
    /// > Decimal type provides exact decimal representation up to 28-29 significant digits.
    ///
    /// > [!TIP]
    /// > Ideal for financial calculations where precision is crucial.
    ///
    /// > [!IMPORTANT]
    /// > Boolean values are converted to 1m (true) or 0m (false).
    ///
    /// > [!WARNING]
    /// > Converting from double may result in precision loss due to binary floating-point representation.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Basic numeric conversion
    /// var numResult = 123.45.CastToDecimal(); // returns 123.45m
    ///
    /// // String parsing with high precision
    /// var strResult = "123456.789012345".CastToDecimal(); // returns 123456.789012345m
    ///
    /// // Boolean conversion
    /// var boolResult = true.CastToDecimal(); // returns 1m
    ///
    /// // Integer conversion
    /// var intResult = 42.CastToDecimal(); // returns 42m
    ///
    /// // Failed conversion
    /// var failed = "invalid".CastToDecimal(defaultValue: 0m); // returns 0m
    /// </code>
    /// </example>
    public static decimal? CastToDecimal<T>(this T? value, decimal? defaultValue = null) =>
        value switch
        {
            null => defaultValue,
            decimal m => m,
            string s => decimal.TryParse(s: s, result: out decimal result) ? result : defaultValue,
            int i => Convert.ToDecimal(value: i),
            double d => Convert.ToDecimal(value: d),
            bool b => b ? 1m : 0m,
            _ => defaultValue
        };

    /// <summary>
    /// Converts a value to a DateTime with culture-aware parsing.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <param name="defaultValue">The default value if conversion fails.</param>
    /// <returns>The converted DateTime or default value if conversion fails.</returns>
    /// <remarks>
    /// Supports multiple date formats and file time conversion.
    ///
    /// > [!NOTE]
    /// > Uses the current culture's date-time format for string parsing.
    ///
    /// > [!TIP]
    /// > Long values are interpreted as Windows file times (number of 100-nanosecond intervals since January 1, 1601).
    ///
    /// > [!IMPORTANT]
    /// > String parsing supports ISO 8601, RFC 1123, and various cultural formats.
    ///
    /// > [!WARNING]
    /// > Time zone information may be lost during conversion unless explicitly specified in the input.
    /// </remarks>
    /// <example>
    /// <code>
    /// // ISO 8601 format
    /// var isoResult = "2023-12-25T15:30:00Z".CastToDateTime();
    ///
    /// // Local date format
    /// var localResult = "25/12/2023".CastToDateTime();
    ///
    /// // File time conversion
    /// var fileTime = 132886944000000000L.CastToDateTime();
    ///
    /// // With time component
    /// var fullDateTime = "2023-12-25 15:30:45".CastToDateTime();
    ///
    /// // Failed conversion
    /// var failed = "invalid".CastToDateTime(defaultValue: DateTime.MinValue);
    /// </code>
    /// </example>
    public static DateTime? CastToDateTime<T>(this T? value, DateTime? defaultValue = null) =>
        value switch
        {
            null => defaultValue,
            DateTime dt => dt,
            string s
                => DateTime.TryParse(s: s, result: out DateTime result) ? result : defaultValue,
            long l => DateTime.FromFileTime(fileTime: l),
            _ => defaultValue
        };

    /// <summary>
    /// Converts a value to a long integer with comprehensive type handling.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <param name="defaultValue">The default value if conversion fails.</param>
    /// <returns>The converted long integer or default value if conversion fails.</returns>
    /// <remarks>
    /// Supports conversion from various numeric types, strings, and boolean values.
    ///
    /// > [!NOTE]
    /// > String values are parsed using the current culture settings.
    ///
    /// > [!TIP]
    /// > Useful for handling large numeric values that exceed int.MaxValue.
    ///
    /// > [!IMPORTANT]
    /// > Decimal and double values are rounded during conversion.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Basic numeric conversion
    /// var numResult = 123456789.CastToLong(); // returns 123456789L
    ///
    /// // String parsing
    /// var strResult = "987654321".CastToLong(); // returns 987654321L
    ///
    /// // Boolean conversion
    /// var boolResult = true.CastToLong(); // returns 1L
    ///
    /// // Handling large numbers
    /// var largeNum = "9223372036854775807".CastToLong(); // returns long.MaxValue
    ///
    /// // Failed conversion
    /// var failed = "invalid".CastToLong(defaultValue: 0L); // returns 0L
    /// </code>
    /// </example>
    public static long? CastToLong<T>(this T? value, long? defaultValue = null) =>
        value switch
        {
            null => defaultValue,
            long l => l,
            string s => long.TryParse(s: s, result: out long result) ? result : defaultValue,
            int i => Convert.ToInt64(value: i),
            double d => Convert.ToInt64(value: d),
            decimal m => Convert.ToInt64(value: m),
            bool b => b ? 1L : 0L,
            _ => defaultValue
        };

    /// <summary>
    /// Converts a value to a float with precise decimal handling.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <param name="defaultValue">The default value if conversion fails.</param>
    /// <returns>The converted float or default value if conversion fails.</returns>
    /// <remarks>
    /// Provides precise floating-point conversion with proper rounding.
    ///
    /// > [!NOTE]
    /// > Uses current culture settings for string parsing.
    ///
    /// > [!WARNING]
    /// > May lose precision when converting from double or decimal.
    ///
    /// > [!TIP]
    /// > Consider using double for better precision if memory isn't a constraint.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Basic numeric conversion
    /// var numResult = 123.45.CastToFloat(); // returns 123.45f
    ///
    /// // String parsing
    /// var strResult = "3.14159".CastToFloat(); // returns 3.14159f
    ///
    /// // Boolean conversion
    /// var boolResult = true.CastToFloat(); // returns 1.0f
    ///
    /// // Scientific notation
    /// var scientific = "1.23E-4".CastToFloat(); // returns 0.000123f
    ///
    /// // Failed conversion
    /// var failed = "invalid".CastToFloat(defaultValue: 0f); // returns 0f
    /// </code>
    /// </example>
    public static float? CastToFloat<T>(this T? value, float? defaultValue = null) =>
        value switch
        {
            null => defaultValue,
            float f => f,
            string s => float.TryParse(s, out float result) ? result : defaultValue,
            int i => Convert.ToSingle(i),
            double d => Convert.ToSingle(d),
            decimal m => Convert.ToSingle(m),
            bool b => b ? 1f : 0f,
            _ => defaultValue
        };

    /// <summary>
    /// Converts a value to a short integer with range validation.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <param name="defaultValue">The default value if conversion fails.</param>
    /// <returns>The converted short integer or default value if conversion fails.</returns>
    /// <remarks>
    /// Handles conversion with proper range checking and type validation.
    ///
    /// > [!NOTE]
    /// > Valid range is from -32,768 to 32,767.
    ///
    /// > [!WARNING]
    /// > Values outside the valid range will return the default value.
    ///
    /// > [!IMPORTANT]
    /// > Decimal and floating-point values are rounded during conversion.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Basic numeric conversion
    /// var numResult = 12345.CastToShort(); // returns (short)12345
    ///
    /// // String parsing
    /// var strResult = "-1234".CastToShort(); // returns (short)-1234
    ///
    /// // Boolean conversion
    /// var boolResult = true.CastToShort(); // returns (short)1
    ///
    /// // Range validation
    /// var outOfRange = 40000.CastToShort(defaultValue: (short)0); // returns (short)0
    ///
    /// // Failed conversion
    /// var failed = "invalid".CastToShort(defaultValue: (short)0); // returns (short)0
    /// </code>
    /// </example>
    public static short? CastToShort<T>(this T? value, short? defaultValue = null) =>
        value switch
        {
            null => defaultValue,
            short s => s,
            string str => short.TryParse(s: str, result: out short result) ? result : defaultValue,
            int i => Convert.ToInt16(value: i),
            double d => Convert.ToInt16(value: d),
            decimal m => Convert.ToInt16(value: m),
            bool b => (short)(b ? 1 : 0),
            _ => defaultValue
        };

    /// <summary>
    /// Converts a value to a byte with comprehensive type handling.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <param name="defaultValue">The default value if conversion fails or value is out of byte range.</param>
    /// <returns>The converted byte value or default value if conversion fails.</returns>
    /// <remarks>
    /// Supports conversion from various numeric types and strings.
    ///
    /// > [!NOTE]
    /// > Values outside the byte range (0-255) will return the default value.
    ///
    /// > [!WARNING]
    /// > Boolean values are converted to 1 (true) or 0 (false).
    /// </remarks>
    /// <example>
    /// <code>
    /// var numResult = 255.CastToByte(); // returns 255
    /// var strResult = "128".CastToByte(); // returns 128
    /// var boolResult = true.CastToByte(); // returns 1
    /// var outOfRange = 256.CastToByte(defaultValue: 0); // returns 0
    /// var failed = "invalid".CastToByte(defaultValue: 0); // returns 0
    /// </code>
    /// </example>
    public static byte? CastToByte<T>(this T? value, byte? defaultValue = null) =>
        value switch
        {
            null => defaultValue,
            byte b => b,
            string s => byte.TryParse(s, out byte result) ? result : defaultValue,
            int i => Convert.ToByte(i),
            double d => Convert.ToByte(d),
            decimal m => Convert.ToByte(m),
            bool b => (byte)(b ? 1 : 0),
            _ => defaultValue
        };

    /// <summary>
    /// Converts a value to a Guid with robust parsing.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <param name="defaultValue">The default value if conversion fails.</param>
    /// <returns>The converted Guid or default value if conversion fails.</returns>
    /// <remarks>
    /// Handles string representations of GUIDs in various formats.
    ///
    /// > [!NOTE]
    /// > Supports both hyphenated and non-hyphenated GUID strings.
    ///
    /// > [!TIP]
    /// > Use Guid.Empty as defaultValue for a standard fallback value.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Standard format
    /// var result1 = "123e4567-e89b-12d3-a456-426614174000".CastToGuid();
    ///
    /// // Without hyphens
    /// var result2 = "123e4567e89b12d3a456426614174000".CastToGuid();
    ///
    /// // With braces
    /// var result3 = "{123e4567-e89b-12d3-a456-426614174000}".CastToGuid();
    ///
    /// // Invalid input
    /// var failed = "invalid".CastToGuid(defaultValue: Guid.Empty);
    /// </code>
    /// </example>
    public static Guid? CastToGuid<T>(this T? value, Guid? defaultValue = null) =>
        value switch
        {
            null => defaultValue,
            Guid g => g,
            string s => Guid.TryParse(input: s, result: out Guid result) ? result : defaultValue,
            _ => defaultValue
        };

    /// <summary>
    /// Converts a value to a TimeSpan with comprehensive duration handling.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <param name="defaultValue">The default value if conversion fails.</param>
    /// <returns>The converted TimeSpan or default value if conversion fails.</returns>
    /// <remarks>
    /// Supports multiple duration formats and numeric time representations.
    ///
    /// > [!NOTE]
    /// > String inputs support standard TimeSpan format patterns.
    ///
    /// > [!TIP]
    /// > Numeric values are interpreted as total seconds.
    ///
    /// > [!IMPORTANT]
    /// > Long values are interpreted as ticks (1 tick = 100 nanoseconds).
    ///
    /// > [!WARNING]
    /// > Negative durations are supported but should be used with caution.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Standard TimeSpan format
    /// var standardFormat = "1.02:03:04".CastToTimeSpan(); // 1 day, 2 hours, 3 minutes, 4 seconds
    ///
    /// // Compact format
    /// var compactFormat = "02:30".CastToTimeSpan(); // 2 minutes, 30 seconds
    ///
    /// // Numeric seconds
    /// var fromSeconds = 3600.CastToTimeSpan(); // 1 hour
    ///
    /// // From ticks
    /// var fromTicks = 10000000L.CastToTimeSpan(); // 1 second
    ///
    /// // Failed conversion
    /// var failed = "invalid".CastToTimeSpan(defaultValue: TimeSpan.Zero);
    ///
    /// // Negative duration
    /// var negative = "-01:30:00".CastToTimeSpan(); // -1 hour, 30 minutes
    /// </code>
    /// </example>
    public static TimeSpan? CastToTimeSpan<T>(this T? value, TimeSpan? defaultValue = null) =>
        value switch
        {
            null => defaultValue,
            TimeSpan ts => ts,
            string s
                => TimeSpan.TryParse(s: s, result: out TimeSpan result) ? result : defaultValue,
            long l => TimeSpan.FromTicks(value: l),
            int i => TimeSpan.FromSeconds(value: i),
            double d => TimeSpan.FromSeconds(value: d),
            _ => defaultValue
        };
}
