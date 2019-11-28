using System;
using System.Globalization;
using System.Text;

namespace CoreMVVM
{
    /// <summary>
    /// A tool for parsing strings, performing resource lookup.
    /// </summary>
    public static class StringParser
    {
        public static string Format(string value, params object[] args)
        {
            string parsedValue = Parse(value);
            try
            {
                return string.Format(CultureInfo.InvariantCulture, parsedValue, args);
            }
            catch (FormatException e)
            {
                LoggerHelper.Exception("StringParser.Format", e);

                return parsedValue;
            }
        }

        /// <summary>
        /// Parses a string by expanding all ${property} values.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed value.</returns>
        public static string Parse(string value, params StringTagPair[] customTags)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            int pos = 0;
            StringBuilder output = null; // don't use StringBuilder if input is a single property
            do
            {
                int oldPos = pos;
                pos = value.IndexOf("${", pos, StringComparison.Ordinal);
                if (pos < 0)
                {
                    if (output is null)
                        return value;

                    if (oldPos < value.Length)
                    {
                        // normal text after last property
                        output.Append(value, oldPos, value.Length - oldPos);
                    }

                    return output.ToString();
                }

                if (output is null)
                {
                    if (pos == 0)
                        output = new StringBuilder();
                    else
                        output = new StringBuilder(value, 0, pos, pos + 16);
                }
                else
                {
                    if (pos > oldPos)
                    {
                        // normal text between two properties
                        output.Append(value, oldPos, pos - oldPos);
                    }
                }
                int end = value.IndexOf('}', pos + 1);
                if (end < 0)
                {
                    output.Append("${");
                    pos += 2;
                }
                else
                {
                    string property = value.Substring(pos + 2, end - pos - 2);
                    string val = GetValue(property, customTags);
                    if (val is null)
                    {
                        output.Append("${");
                        output.Append(property);
                        output.Append('}');
                    }
                    else
                    {
                        output.Append(val);
                    }
                    pos = end + 1;
                }
            } while (pos < value.Length);

            return output.ToString();
        }

        public static string GetValue(string propertyName) => GetValue(propertyName, null);

        /// <summary>
        /// Evaluates the value of a property.
        /// </summary>
        /// <param name="propertyName">The name of the property to evaluate.</param>
        /// <returns>The value that the property was evaluated to.</returns>
        public static string GetValue(string propertyName, params StringTagPair[] customTags)
        {
            if (propertyName is null) throw new ArgumentNullException(nameof(propertyName));

            if (customTags != null)
            {
                foreach (StringTagPair pair in customTags)
                {
                    if (propertyName.Equals(pair.Tag, StringComparison.OrdinalIgnoreCase))
                    {
                        return pair.Value ?? string.Empty;
                    }
                }
            }

            if (propertyName.Contains(":"))
            {
                // It's a prefixed property.

                if (propertyName.StartsWith("res:", StringComparison.OrdinalIgnoreCase))
                {
                    IResourceService resourceService = ContainerProvider.Resolve<IResourceService>();

                    string resource = resourceService.GetString(propertyName.Substring(4));
                    if (resource != null)
                        return Parse(resource, customTags);
                }
            }
            else
            {
                // It's not a prefixed property.
            }

            return $"${{{propertyName}}}";
        }

        public static string GetResource(string key)
        {
            return GetResource(key, null);
        }

        public static string GetResource(string key, params StringTagPair[] customTags)
        {
            return GetValue("res:" + key, customTags);
        }
    }

    #region StringTagPair

    public struct StringTagPair : IEquatable<StringTagPair>
    {
        public StringTagPair(string tag, string value)
        {
            Tag = tag;
            Value = value;
        }

        public StringTagPair(string tag, object value)
        {
            Tag = tag;

            if (value is IConvertible convertible)
                Value = convertible.ToString(CultureInfo.CurrentCulture);
            else
                Value = value?.ToString();
        }

        public string Tag { get; }

        public string Value { get; }

        public override bool Equals(object obj)
        {
            if (obj is StringTagPair other)
                Equals(other);

            return false;
        }

        public bool Equals(StringTagPair other)
        {
            return Tag == other.Tag &&
                   Value == other.Value;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + Tag.GetHashCode();
            hash = (hash * 7) + Value?.GetHashCode() ?? 13;

            return hash;
        }

        public static bool operator ==(StringTagPair left, StringTagPair right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(StringTagPair left, StringTagPair right)
        {
            return !(left == right);
        }
    }

    #endregion StringTagPair
}