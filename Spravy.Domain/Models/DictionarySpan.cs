using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Spravy.Domain.Models;

public readonly ref struct DictionarySpan<TKey, TValue> where TKey : notnull
{
    public readonly Span<KeyValuePair<TKey, TValue>> Span;
    public readonly int Count;
    public readonly Span<TKey> Keys;
    public readonly Span<TValue> Values;

    public DictionarySpan(IReadOnlyDictionary<TKey, TValue> dictionary) : this(dictionary.ToArray())
    {
    }

    private DictionarySpan(Span<KeyValuePair<TKey, TValue>> span)
    {
        Span = span;
        Count = span.Length;
        var keys = new TKey[span.Length];
        var values = new TValue[span.Length];

        for (var index = 0; index < span.Length; index++)
        {
            keys[index] = span[index].Key;
            values[index] = span[index].Value;
        }

        Keys = keys;
        Values = values;
    }

    public static implicit operator DictionarySpan<TKey, TValue>(
        Dictionary<TKey, TValue> dictionary
    )
    {
        return new (dictionary);
    }

    public Enumerator GetEnumerator()
    {
        return new (Span);
    }

    public bool ContainsKey(TKey key)
    {
        foreach (var item in Span)
        {
            if (item.Equals(key))
            {
                return true;
            }
        }

        return false;
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        foreach (var item in Span)
        {
            if (item.Key.Equals(key))
            {
                value = item.Value;

                return true;
            }
        }

        value = default;

        return false;
    }

    public TValue this[TKey key] => throw new NotImplementedException();

    public ref struct Enumerator
    {
        private readonly Span<KeyValuePair<TKey, TValue>> span;
        private int index;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator(Span<KeyValuePair<TKey, TValue>> span)
        {
            this.span = span;
            index = -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            var num = index + 1;

            if (num >= span.Length)
            {
                return false;
            }

            index = num;

            return true;
        }

        public ref KeyValuePair<TKey, TValue> Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref span[index];
        }
    }
}