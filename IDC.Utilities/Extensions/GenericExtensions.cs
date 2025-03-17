namespace IDC.Utilities.Extensions;

/// <summary>
/// Provides extension methods for generic types.
/// </summary>
public static class GenericExtensions
{
    /// <summary>
    /// Throws an exception if the value is null.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="paramName">The name of the parameter being checked.</param>
    /// <param name="message">Optional custom error message.</param>
    /// <returns>The non-null value.</returns>
    /// <exception cref="ArgumentNullException">Thrown when value is null.</exception>
    /// <example>
    /// <code>
    /// string? name = null;
    /// string result = name.ThrowIfNull(paramName: "name"); // throws ArgumentNullException
    /// </code>
    /// </example>
    public static T ThrowIfNull<T>(this T? value, string paramName, string? message = null)
        where T : class =>
        value ?? throw new ArgumentNullException(paramName: paramName, message: message);

    /// <summary>
    /// Performs an action on the value if it's not null.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="action">The action to perform if value is not null.</param>
    /// <returns>The original value.</returns>
    /// <example>
    /// <code>
    /// string name = "John";
    /// name.IfNotNull(action: x => Console.WriteLine(x)); // prints "John"
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
    /// Maps a value to a new type using the specified mapping function.
    /// </summary>
    /// <typeparam name="TInput">The type of the input value.</typeparam>
    /// <typeparam name="TOutput">The type of the output value.</typeparam>
    /// <param name="value">The value to map.</param>
    /// <param name="mapper">The mapping function.</param>
    /// <returns>The mapped value.</returns>
    /// <example>
    /// <code>
    /// int number = 42;
    /// string result = number.Map(mapper: x => x.ToString()); // returns "42"
    /// </code>
    /// </example>
    public static TOutput Map<TInput, TOutput>(this TInput value, Func<TInput, TOutput> mapper) =>
        mapper(arg: value);

    /// <summary>
    /// Converts a value to a different type using a try-parse pattern.
    /// </summary>
    /// <typeparam name="TInput">The type of the input value.</typeparam>
    /// <typeparam name="TOutput">The type of the output value.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <param name="parser">The parsing function that returns a tuple of (bool success, TOutput result).</param>
    /// <param name="defaultValue">The default value to return if parsing fails.</param>
    /// <returns>The parsed value if successful, otherwise the default value.</returns>
    /// <example>
    /// <code>
    /// string number = "42";
    /// int result = number.TryParse(
    ///     parser: s => (int.TryParse(s, out int n), n),
    ///     defaultValue: 0
    /// ); // returns 42
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
    /// Converts a value to an integer.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <param name="defaultValue">The default value if conversion fails.</param>
    /// <returns>The converted integer or default value.</returns>
    /// <example>
    /// <code>
    /// var result = "123".CastToInteger(); // returns 123
    /// var boolResult = true.CastToInteger(); // returns 1
    /// var failed = "abc".CastToInteger(defaultValue: 0); // returns 0
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
    /// Converts a value to a string.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <param name="defaultValue">The default value if conversion fails.</param>
    /// <returns>The converted string or default value.</returns>
    /// <example>
    /// <code>
    /// var result = 123.CastToString(); // returns "123"
    /// var nullResult = null.CastToString(defaultValue: "N/A"); // returns "N/A"
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
    /// Converts a value to a boolean.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <param name="defaultValue">The default value if conversion fails.</param>
    /// <returns>The converted boolean or default value.</returns>
    /// <example>
    /// <code>
    /// var result = "true".CastToBoolean(); // returns true
    /// var numResult = 1.CastToBoolean(); // returns true
    /// var failed = "invalid".CastToBoolean(defaultValue: false); // returns false
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
    /// Converts a value to a double.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <param name="defaultValue">The default value if conversion fails.</param>
    /// <returns>The converted double or default value.</returns>
    /// <example>
    /// <code>
    /// var result = "123.45".CastToDouble(); // returns 123.45
    /// var boolResult = true.CastToDouble(); // returns 1.0
    /// var failed = "abc".CastToDouble(defaultValue: 0.0); // returns 0.0
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
    /// Converts a value to a decimal.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <param name="defaultValue">The default value if conversion fails.</param>
    /// <returns>The converted decimal or default value.</returns>
    /// <example>
    /// <code>
    /// var result = "123.45".CastToDecimal(); // returns 123.45m
    /// var boolResult = true.CastToDecimal(); // returns 1m
    /// var failed = "abc".CastToDecimal(defaultValue: 0m); // returns 0m
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
    /// Converts a value to a DateTime.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <param name="defaultValue">The default value if conversion fails.</param>
    /// <returns>The converted DateTime or default value.</returns>
    /// <example>
    /// <code>
    /// var result = "2023-12-25".CastToDateTime(); // returns DateTime(2023,12,25)
    /// var failed = "invalid".CastToDateTime(defaultValue: DateTime.MinValue); // returns DateTime.MinValue
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
    /// Converts a value to a long.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <param name="defaultValue">The default value if conversion fails.</param>
    /// <returns>The converted long or default value.</returns>
    /// <example>
    /// <code>
    /// var result = "123".CastToLong(); // returns 123L
    /// var boolResult = true.CastToLong(); // returns 1L
    /// var failed = "abc".CastToLong(defaultValue: 0L); // returns 0L
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
    /// Converts a value to a float.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <param name="defaultValue">The default value if conversion fails.</param>
    /// <returns>The converted float or default value.</returns>
    /// <example>
    /// <code>
    /// var result = "123.45".CastToFloat(); // returns 123.45f
    /// var boolResult = true.CastToFloat(); // returns 1f
    /// var failed = "abc".CastToFloat(defaultValue: 0f); // returns 0f
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
    /// Converts a value to a short.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <param name="defaultValue">The default value if conversion fails.</param>
    /// <returns>The converted short or default value.</returns>
    /// <example>
    /// <code>
    /// var result = "123".CastToShort(); // returns (short)123
    /// var boolResult = true.CastToShort(); // returns (short)1
    /// var failed = "abc".CastToShort(defaultValue: (short)0); // returns (short)0
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
    /// Converts a value to a byte.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <param name="defaultValue">The default value if conversion fails.</param>
    /// <returns>The converted byte or default value.</returns>
    /// <example>
    /// <code>
    /// var result = "123".CastToByte(); // returns (byte)123
    /// var boolResult = true.CastToByte(); // returns (byte)1
    /// var failed = "abc".CastToByte(defaultValue: (byte)0); // returns (byte)0
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
    /// Converts a value to a Guid.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <param name="defaultValue">The default value if conversion fails.</param>
    /// <returns>The converted Guid or default value.</returns>
    /// <example>
    /// <code>
    /// var result = "123e4567-e89b-12d3-a456-426614174000".CastToGuid(); // returns parsed Guid
    /// var failed = "invalid".CastToGuid(defaultValue: Guid.Empty); // returns Guid.Empty
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
    /// Converts a value to a TimeSpan.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <param name="defaultValue">The default value if conversion fails.</param>
    /// <returns>The converted TimeSpan or default value.</returns>
    /// <example>
    /// <code>
    /// var result = "01:30:00".CastToTimeSpan(); // returns TimeSpan of 1.5 hours
    /// var numResult = 90.CastToTimeSpan(); // returns TimeSpan of 90 seconds
    /// var failed = "invalid".CastToTimeSpan(defaultValue: TimeSpan.Zero); // returns TimeSpan.Zero
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
