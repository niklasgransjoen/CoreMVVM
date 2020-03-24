using CoreMVVM.IOC;
using System;
using System.Linq;
using System.Text;

namespace CoreMVVM.FallbackImplementations
{
    [Scope(ComponentScope.Singleton)]
    public sealed class FallbackStringParser : IStringParser
    {
        public string Format(IResourceService resourceService, IFormatProvider formatProvider, string value, params object[] args)
        {
            string parsedValue = Parse(resourceService, value);
            try
            {
                return string.Format(formatProvider, parsedValue, args);
            }
            catch (FormatException e)
            {
                LoggerHelper.Exception("StringParser.Format", e);

                return parsedValue;
            }
        }

        public string Parse(IResourceService resourceService, string value, params StringTagPair[] args)
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
#if NETCORE
                    ReadOnlySpan<char> property = value.AsSpan(pos + 2, end - pos - 2);
                    string val = GetValue(resourceService, property, args);
#else
                        string property = value.Substring(pos + 2, end - pos - 2);
                        string val = GetValue(resourceService, property, args);
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

        public string GetValue(IResourceService resourceService, string propertyName, params StringTagPair[] args)
        {
            if (propertyName is null) throw new ArgumentNullException(nameof(propertyName));

            // Prioritize prefixed properties.
            if (propertyName.Contains(":"))
            {
                if (propertyName.StartsWith("res:", StringComparison.OrdinalIgnoreCase))
                {
#if NETCORE
                    return GetResource(resourceService, propertyName.AsSpan(4), args);
#else
                        return GetResource(resourceService, propertyName.Substring(4), args);
#endif
                }
            }
            else if (args != null)
            {
                var result = args.FirstOrDefault(pair => pair.Tag == propertyName);
                if (result != default)
                {
                    return result.Value ?? string.Empty;
                }
            }

            return null;
        }

        public string GetResource(IResourceService resourceService, string key, params StringTagPair[] args)
        {
            string resource = resourceService.GetString(key);
            if (resource is null)
                return null;

            return Parse(resourceService, resource, args);
        }

#if NETCORE

        public string GetValue(IResourceService resourceService, ReadOnlySpan<char> propertyName, params StringTagPair[] args)
        {
            // Prioritize prefixed properties.
            if (propertyName.IndexOf(':') != -1)
            {
                if (propertyName.StartsWith("res:", StringComparison.OrdinalIgnoreCase))
                {
                    return GetResource(resourceService, propertyName.Slice(4), args);
                }
            }
            else if (args != null)
            {
                foreach (var pair in args)
                {
                    if (pair.Tag == propertyName)
                        return pair.Value ?? string.Empty;
                }
            }

            return null;
        }

        public string GetResource(IResourceService resourceService, ReadOnlySpan<char> key, params StringTagPair[] args)
        {
            string resource = resourceService.GetString(key);
            if (resource is null)
                return null;

            return Parse(resourceService, resource, args);
        }

#endif
    }
}