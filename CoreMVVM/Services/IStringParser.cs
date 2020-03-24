using CoreMVVM.IOC;
using System;

namespace CoreMVVM
{
    /// <summary>
    /// Service providing functionality to the <see cref="StringParser"/> singleton service.
    /// </summary>
    [FallbackImplementation(typeof(FallbackImplementations.FallbackStringParser))]
    public interface IStringParser
    {
        /// <summary>
        /// Parser the given value, then formats it with the given arguments.
        /// </summary>
        string Format(IResourceService resourceService, IFormatProvider formatProvider, string value, params object[] args);

        /// <summary>
        /// Parses the given string.
        /// </summary>
        string Parse(IResourceService resourceService, string value, params StringTagPair[] args);

        /// <summary>
        /// Attempts to resolve the given property. Returns null if no match is found.
        /// </summary>
        string GetValue(IResourceService resourceService, string propertyName, params StringTagPair[] args);

        /// <summary>
        /// Resolves a resource. The resource is then parsed, before it's removed. Returns null if not match is found.
        /// </summary>
        string GetResource(IResourceService resourceService, string key, params StringTagPair[] args);

#if NETCORE

        /// <summary>
        /// Attempts to resolve the given property. Returns null if no match is found.
        /// </summary>
        string GetValue(IResourceService resourceService, ReadOnlySpan<char> propertyName, params StringTagPair[] args);

        /// <summary>
        /// Resolves a resource. The resource is then parsed, before it's removed. Returns null if not match is found.
        /// </summary>
        string GetResource(IResourceService resourceService, ReadOnlySpan<char> key, params StringTagPair[] args);

#endif
    }

    #region StringTagPair

    public readonly struct StringTagPair : IEquatable<StringTagPair>
    {
        public StringTagPair(string tag, string value)
        {
            Tag = tag;
            Value = value;
        }

        public StringTagPair(string tag, object value)
        {
            Tag = tag;
            Value = value?.ToString() ?? string.Empty;
        }

        public string Tag { get; }

        public string Value { get; }

        public override bool Equals(object obj)
        {
            return obj is StringTagPair other && Equals(other);
        }

        public bool Equals(StringTagPair other)
        {
            return Tag == other.Tag &&
                   Value == other.Value;
        }

        public override int GetHashCode()
        {
#if NETCORE
            return HashCode.Combine(Tag, Value);
#else
            int hash = 13;
            hash = (hash * 7) + Tag.GetHashCode();
            hash = (hash * 7) + Value?.GetHashCode() ?? 13;

            return hash;
#endif
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