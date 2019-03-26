namespace rise.Code.Cron
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines the <see cref="CrontabFieldImpl" />
    /// </summary>
    [Serializable]
    public sealed class CrontabFieldImpl : IObjectReference
    {
        /// <summary>
        /// Defines the Minute
        /// </summary>
        public static readonly CrontabFieldImpl Minute = new CrontabFieldImpl(CrontabFieldKind.Minute, 0, 59, null);

        /// <summary>
        /// Defines the Hour
        /// </summary>
        public static readonly CrontabFieldImpl Hour = new CrontabFieldImpl(CrontabFieldKind.Hour, 0, 23, null);

        /// <summary>
        /// Defines the Day
        /// </summary>
        public static readonly CrontabFieldImpl Day = new CrontabFieldImpl(CrontabFieldKind.Day, 1, 31, null);

        /// <summary>
        /// Defines the Month
        /// </summary>
        public static readonly CrontabFieldImpl Month = new CrontabFieldImpl(CrontabFieldKind.Month, 1, 12,
            new[]
            {
                "January", "February", "March", "April",
                "May", "June", "July", "August",
                "September", "October", "November",
                "December"
            });

        /// <summary>
        /// Defines the DayOfWeek
        /// </summary>
        public static readonly CrontabFieldImpl DayOfWeek = new CrontabFieldImpl(CrontabFieldKind.DayOfWeek, 0, 6,
            new[]
            {
                "Sunday", "Monday", "Tuesday",
                "Wednesday", "Thursday", "Friday",
                "Saturday"
            });

        /// <summary>
        /// Defines the FieldByKind
        /// </summary>
        private static readonly CrontabFieldImpl[] FieldByKind = { Minute, Hour, Day, Month, DayOfWeek };

        /// <summary>
        /// Defines the Comparer
        /// </summary>
        private static readonly CompareInfo Comparer = CultureInfo.InvariantCulture.CompareInfo;

        /// <summary>
        /// Defines the Comma
        /// </summary>
        private static readonly char[] Comma = { ',' };

        /// <summary>
        /// Defines the _kind
        /// </summary>
        private readonly CrontabFieldKind _kind;

        /// <summary>
        /// Defines the _maxValue
        /// </summary>
        private readonly int _maxValue;

        /// <summary>
        /// Defines the _minValue
        /// </summary>
        private readonly int _minValue;

        /// <summary>
        /// Defines the _names
        /// </summary>
        private readonly string[] _names;

        /// <summary>
        /// Prevents a default instance of the <see cref="CrontabFieldImpl"/> class from being created.
        /// </summary>
        /// <param name="kind">The kind<see cref="CrontabFieldKind"/></param>
        /// <param name="minValue">The minValue<see cref="int"/></param>
        /// <param name="maxValue">The maxValue<see cref="int"/></param>
        /// <param name="names">The names<see cref="string[]"/></param>
        private CrontabFieldImpl(CrontabFieldKind kind, int minValue, int maxValue, string[] names)
        {
            Debug.Assert(Enum.IsDefined(typeof(CrontabFieldKind), kind));
            Debug.Assert(minValue >= 0);
            Debug.Assert(maxValue >= minValue);
            Debug.Assert(names == null || names.Length == (maxValue - minValue + 1));

            _kind = kind;
            _minValue = minValue;
            _maxValue = maxValue;
            _names = names;
        }

        /// <summary>
        /// Gets the Kind
        /// </summary>
        public CrontabFieldKind Kind
        {
            get { return _kind; }
        }

        /// <summary>
        /// Gets the MinValue
        /// </summary>
        public int MinValue
        {
            get { return _minValue; }
        }

        /// <summary>
        /// Gets the MaxValue
        /// </summary>
        public int MaxValue
        {
            get { return _maxValue; }
        }

        /// <summary>
        /// Gets the ValueCount
        /// </summary>
        public int ValueCount
        {
            get { return _maxValue - _minValue + 1; }
        }

        /// <summary>
        /// The GetRealObject
        /// </summary>
        /// <param name="context">The context<see cref="StreamingContext"/></param>
        /// <returns>The <see cref="object"/></returns>
        object IObjectReference.GetRealObject(StreamingContext context)
        {
            return FromKind(Kind);
        }

        /// <summary>
        /// The FromKind
        /// </summary>
        /// <param name="kind">The kind<see cref="CrontabFieldKind"/></param>
        /// <returns>The <see cref="CrontabFieldImpl"/></returns>
        public static CrontabFieldImpl FromKind(CrontabFieldKind kind)
        {
            if (!Enum.IsDefined(typeof(CrontabFieldKind), kind))
            {
                throw new ArgumentException(string.Format(
                    "Invalid crontab field kind. Valid values are {0}.",
                    string.Join(", ", Enum.GetNames(typeof(CrontabFieldKind)))), nameof(kind));
            }

            return FieldByKind[(int)kind];
        }

        /// <summary>
        /// The Format
        /// </summary>
        /// <param name="field">The field<see cref="CrontabField"/></param>
        /// <param name="writer">The writer<see cref="TextWriter"/></param>
        /// <param name="noNames">The noNames<see cref="bool"/></param>
        public void Format(CrontabField field, TextWriter writer, bool noNames)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            var next = field.GetFirst();
            var count = 0;

            while (next != -1)
            {
                var first = next;
                int last;

                do
                {
                    last = next;
                    next = field.Next(last + 1);
                } while (next - last == 1);

                if (count == 0
                    && first == _minValue && last == _maxValue)
                {
                    writer.Write('*');
                    return;
                }

                if (count > 0)
                    writer.Write(',');

                if (first == last)
                {
                    FormatValue(first, writer, noNames);
                }
                else
                {
                    FormatValue(first, writer, noNames);
                    writer.Write('-');
                    FormatValue(last, writer, noNames);
                }

                count++;
            }
        }

        /// <summary>
        /// The FormatValue
        /// </summary>
        /// <param name="value">The value<see cref="int"/></param>
        /// <param name="writer">The writer<see cref="TextWriter"/></param>
        /// <param name="noNames">The noNames<see cref="bool"/></param>
        private void FormatValue(int value, TextWriter writer, bool noNames)
        {
            Debug.Assert(writer != null);

            if (noNames || _names == null)
            {
                if (value >= 0 && value < 100)
                {
                    FastFormatNumericValue(value, writer);
                }
                else
                {
                    writer.Write(value.ToString(CultureInfo.InvariantCulture));
                }
            }
            else
            {
                var index = value - _minValue;
                writer.Write((string)_names[index]);
            }
        }

        /// <summary>
        /// The FastFormatNumericValue
        /// </summary>
        /// <param name="value">The value<see cref="int"/></param>
        /// <param name="writer">The writer<see cref="TextWriter"/></param>
        private static void FastFormatNumericValue(int value, TextWriter writer)
        {
            Debug.Assert(value >= 0 && value < 100);
            Debug.Assert(writer != null);

            if (value >= 10)
            {
                writer.Write((char)('0' + (value / 10)));
                writer.Write((char)('0' + (value % 10)));
            }
            else
            {
                writer.Write((char)('0' + value));
            }
        }

        /// <summary>
        /// The Parse
        /// </summary>
        /// <param name="str">The str<see cref="string"/></param>
        /// <param name="acc">The acc<see cref="CrontabFieldAccumulator"/></param>
        public void Parse(string str, CrontabFieldAccumulator acc)
        {
            if (acc == null)
                throw new ArgumentNullException(nameof(acc));

            if (string.IsNullOrEmpty(str))
                return;

            try
            {
                InternalParse(str, acc);
            }
            catch (FormatException e)
            {
                ThrowParseException(e, str);
            }
        }

        /// <summary>
        /// The ThrowParseException
        /// </summary>
        /// <param name="innerException">The innerException<see cref="Exception"/></param>
        /// <param name="str">The str<see cref="string"/></param>
        private static void ThrowParseException(Exception innerException, string str)
        {
            Debug.Assert(str != null);
            Debug.Assert(innerException != null);

            throw new FormatException(string.Format("'{0}' is not a valid crontab field expression.", str),
                innerException);
        }

        /// <summary>
        /// The InternalParse
        /// </summary>
        /// <param name="str">The str<see cref="string"/></param>
        /// <param name="acc">The acc<see cref="CrontabFieldAccumulator"/></param>
        private void InternalParse(string str, CrontabFieldAccumulator acc)
        {
            Debug.Assert(str != null);
            Debug.Assert(acc != null);

            if (str.Length == 0)
                throw new FormatException("A crontab field value cannot be empty.");

            //
            // Next, look for a list of values (e.g. 1,2,3).
            //

            var commaIndex = str.IndexOf(",", StringComparison.Ordinal);

            if (commaIndex > 0)
            {
                foreach (var token in str.Split(Comma))
                    InternalParse(token, acc);
            }
            else
            {
                var every = 1;

                //
                // Look for stepping first (e.g. */2 = every 2nd).
                //

                var slashIndex = str.IndexOf("/", StringComparison.Ordinal);

                if (slashIndex > 0)
                {
                    every = int.Parse(str.Substring(slashIndex + 1), CultureInfo.InvariantCulture);
                    str = str.Substring(0, slashIndex);
                }

                //
                // Next, look for wildcard (*).
                //

                if (str.Length == 1 && str[0] == '*')
                {
                    acc(-1, -1, every);
                    return;
                }

                //
                // Next, look for a range of values (e.g. 2-10).
                //

                var dashIndex = str.IndexOf("-", StringComparison.Ordinal);

                if (dashIndex > 0)
                {
                    var first = ParseValue(str.Substring(0, dashIndex));
                    var last = ParseValue(str.Substring(dashIndex + 1));

                    acc(first, last, every);
                    return;
                }

                //
                // Finally, handle the case where there is only one number.
                //

                var value = ParseValue(str);

                if (every == 1)
                {
                    acc(value, value, 1);
                }
                else
                {
                    Debug.Assert(every != 0);

                    acc(value, _maxValue, every);
                }
            }
        }

        /// <summary>
        /// The ParseValue
        /// </summary>
        /// <param name="str">The str<see cref="string"/></param>
        /// <returns>The <see cref="int"/></returns>
        private int ParseValue(string str)
        {
            Debug.Assert(str != null);

            if (str.Length == 0)
                throw new FormatException("A crontab field value cannot be empty.");

            var firstChar = str[0];

            if (firstChar >= '0' && firstChar <= '9')
                return int.Parse(str, CultureInfo.InvariantCulture);

            if (_names == null)
            {
                throw new FormatException(string.Format(
                    "'{0}' is not a valid value for this crontab field. It must be a numeric value between {1} and {2} (all inclusive).",
                    str, _minValue, _maxValue));
            }

            for (var i = 0; i < _names.Length; i++)
            {
                if (Comparer.IsPrefix(_names[i], str, CompareOptions.IgnoreCase))
                    return i + _minValue;
            }

            throw new FormatException(string.Format(
                "'{0}' is not a known value name. Use one of the following: {1}.",
                str, string.Join(", ", _names)));
        }
    }
}