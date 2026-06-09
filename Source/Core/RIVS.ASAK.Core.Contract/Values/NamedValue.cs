namespace RIVS.ASAK.Core.Contract.Values
{
    public class NamedValue<T> : INamed
    {
        public string Name { get; set; }
        public T Value { get; set; }

        public NamedValue() { }
    }
}
