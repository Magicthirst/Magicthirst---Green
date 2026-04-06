namespace Levels.Util
{
    public class V<T>
    {
        public T Value { get; set; }

        public V(T value)
        {
            Value = value;
        }
    }
}