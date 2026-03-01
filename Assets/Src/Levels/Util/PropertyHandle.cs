using System;

namespace Levels.Util
{
    public interface IPropertyHandle<out T>
    {
        public event Action<T> Changed;

        public T Value { get; }
    }

    public class PropertyHandle<T> : IPropertyHandle<T>
    {
        public event Action<T> Changed;

        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                Changed?.Invoke(_value);
            }
        }
    }
}