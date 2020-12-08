using System;
using System.Text;

namespace CoreMVVM.Implementations
{
    /// <summary>
    /// Default implementation of <see cref="IStringParser"/>.
    /// </summary>
    public sealed class DefaultStringParser : IStringParser
    {
        #region Core

        /**
         * The core logic of the default string parser.
         */

        public static string Format(IResourceService resourceService, IFormatProvider formatProvider, string value, params object[] args)
        {
            string parsedValue = Parse(resourceService, value);
            try
            {
                return string.Format(formatProvider, parsedValue, args);
            }
            catch (FormatException)
            {
                return parsedValue;
            }
        }

        public static string Parse(IResourceService resourceService, string value, params StringTagPair[] args)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            int pos = 0;
            StringBuilder? output = null; // don't use StringBuilder if input is a single property
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
                        output = new();
                    else
                        output = new(value, 0, pos, pos + 16);
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
#if NETCORE
                    ReadOnlySpan<char> property = value.AsSpan(pos + 2, end - pos - 2);
                    string? val = GetValue(resourceService, property, args);
#else
                    string property = value.Substring(pos + 2, end - pos - 2);
                    string? val = GetValue(resourceService, property, args);
#endif
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

        public static string? GetValue(IResourceService resourceService, string propertyName, params StringTagPair[] args)
        {
            if (resourceService is null)
                throw new ArgumentNullException(nameof(resourceService));

            if (propertyName is null) throw new ArgumentNullException(nameof(propertyName));

            // Prioritize prefixed properties.
            if (propertyName.Contains(":"))
            {
                if (propertyName.StartsWith("res:", StringComparison.OrdinalIgnoreCase))
                {
#if NETCORE
                    var resourceKey = propertyName.AsSpan(4);
#else
                    var resourceKey = propertyName.Substring(4);
#endif

                    string? resource = resourceService.GetString(resourceKey);
                    if (resource is null)
                        return null;

                    return Parse(resourceService, resource, args);
                }
            }

            if (args != null)
            {
                foreach (var pair in args)
                {
                    if (pair.Tag == propertyName)
                        return pair.Value ?? string.Empty;
                }
            }

            return null;
        }

#if NETCORE

        public static string? GetValue(IResourceService resourceService, ReadOnlySpan<char> propertyName, params StringTagPair[] args)
        {
            if (resourceService is null)
                throw new ArgumentNullException(nameof(resourceService));

            // Prioritize prefixed properties.
            if (propertyName.IndexOf(':') != -1)
            {
                if (propertyName.StartsWith("res:", StringComparison.OrdinalIgnoreCase))
                {
                    var resourceKey = propertyName.Slice(4);
                    string? resource = resourceService.GetString(resourceKey);
                    if (resource is null)
                        return null;

                    return Parse(resourceService, resource, args);
                }
            }

            if (args != null)
            {
                foreach (var pair in args)
                {
                    if (pair.Tag == propertyName)
                        return pair.Value ?? string.Empty;
                }
            }

            return null;
        }

#endif

        #endregion Core

        private readonly IResourceService _resourceService;

        public DefaultStringParser(IResourceService resourceService)
        {
            _resourceService = resourceService;
        }

        public string Format(IFormatProvider formatProvider, string value, params object[] args)
        {
            return Format(_resourceService, formatProvider, value, args);
        }

        public string Parse(string value, params StringTagPair[] args)
        {
            return Parse(_resourceService, value, args);
        }

        public string? GetValue(string propertyName, params StringTagPair[] args)
        {
            return GetValue(_resourceService, propertyName, args);
        }

#if NETCORE

        public string? GetValue(ReadOnlySpan<char> propertyName, params StringTagPair[] args)
        {
            return GetValue(_resourceService, propertyName, args);
        }

#endif
    }
}