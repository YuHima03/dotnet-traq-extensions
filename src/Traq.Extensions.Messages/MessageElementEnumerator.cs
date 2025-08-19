﻿using System.Collections;

namespace Traq.Extensions.Messages
{
    public ref struct MessageElementEnumerator(ReadOnlySpan<char> source)
#if NET9_0_OR_GREATER
        : IEnumerator<MessageElement>
#else
        : IEnumerator
#endif
    {
        BaseEnumerator _base = new(source);
        MessageElement _current;

        // 0: Initial
        // 1: InUse
        // 2: PassedEnd
        // 3: Disposed
        int _state = 0;

        public readonly MessageElement Current => _current;

        readonly object? IEnumerator.Current => throw new NotSupportedException("Use the strongly-typed Current property instead.");

        public void Dispose()
        {
            if (_state != 3)
            {
                _state = 3;
                _base.Dispose();
            }
        }

        public readonly MessageElementEnumerator GetEnumerator() => (_state == 0) ? this : new(_base.Source);

        public bool MoveNext()
        {
            switch (_state)
            {
                case 0:
                {
                    if (!_base.MoveNext())
                    {
                        _state = 2;
                        return false;
                    }
                    _state = 1;
                    goto case 1;
                }
                case 1:
                {
                    var current = _base.Current;
                    if (current.Kind != MessageElementKind.NormalText)
                    {
                        if (!_base.MoveNext())
                        {
                            _state = 2;
                        }
                        _current = current;
                        return true;
                    }

                    while (_base.MoveNext())
                    {
                        var e = _base.Current;
                        if (e.Kind != MessageElementKind.NormalText)
                        {
                            _current = current;
                            return true;
                        }
                        current = new(Helpers.SpanHelper.Concat(current.RawText, e.RawText));
                    }
                    _current = current;
                    _state = 2;
                    return true;
                }
            }
            return false;
        }

        public void Reset()
        {
            if (_state != 3)
            {
                _base.Reset();
                _current = default;
                _state = 0;
            }
        }

        ref struct BaseEnumerator(ReadOnlySpan<char> source)
#if NET9_0_OR_GREATER
            : IEnumerator<MessageElement>
#else
            : IEnumerator
#endif
        {
            int _index = 0;
            MessageElement _current;
            readonly ReadOnlySpan<char> _source = source;

            public readonly MessageElement Current => _current;

            readonly object? IEnumerator.Current => throw new NotSupportedException("Use the strongly-typed Current property instead.");

            public readonly ReadOnlySpan<char> Source => _source;

            public readonly BaseEnumerator GetEnumerator() => this;

            public void Dispose() { }

            public bool MoveNext()
            {
                var begin = _index;
                var source = _source;
                if (source.Length <= begin)
                {
                    return false;
                }

                _current = MessageElement.ParseHead(source[begin..], out var charsUsed);
                _index = begin + charsUsed;
                return true;
            }

            public void Reset()
            {
                _index = 0;
            }
        }
    }
}
