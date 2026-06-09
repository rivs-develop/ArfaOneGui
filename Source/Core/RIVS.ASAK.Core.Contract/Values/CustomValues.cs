using System;

namespace RIVS.ASAK.Core.Contract.Values
{
    /// <summary>
    /// Represents the Value item from the Report (text value, number value, datetime value, ...)
    /// </summary>
    public abstract class CustomValue
    {
        public enum ValueType { Text, Number, Date, Bool, Object }

        public abstract ValueType Type { get; }
        public abstract object Value { get; }

        public abstract object GetValueObject();

        static public implicit operator string(CustomValue value)
        {
            if (value == null)
            {
                return "";
            }

            var text = value as CustomValueText;
            if (text != null)
            {
                return text._value;
            }

            return value.ToString();
        }

        static public implicit operator CustomValue(string value)
        {
            return new CustomValueText(value);
        }

        static public implicit operator decimal(CustomValue value)
        {
            var number = Convert(value, ValueType.Number) as CustomValueNumber;
            if (number != null)
            {
                return number._value;
            }

            throw new ArgumentException("Can't convert Value to Decimal");
        }

        static public implicit operator CustomValue(decimal value)
        { return new CustomValueNumber(value); }

        static public implicit operator CustomValue(double value)
        { return new CustomValueNumber((decimal)value); }

        static public implicit operator CustomValue(int value)
        { return new CustomValueNumber(value); }

        static public implicit operator CustomValue(DateTime value)
        { return new CustomValueDate(value); }

        static public implicit operator DateTime(CustomValue value)
        {
            var number = CustomValue.Convert(value, ValueType.Date) as CustomValueDate;
            if (number != null)
            {
                return number._value;
            }

            throw new ArgumentException("Can't convert Value to DateTime");
        }

        static public implicit operator CustomValue(bool value)
        { return new CustomValueBool(value); }

        static public implicit operator bool(CustomValue value)
        {
            var flag = CustomValue.Convert(value, ValueType.Bool) as CustomValueBool;
            if (flag != null)
            {
                return flag._value;
            }

            throw new ArgumentException("Can't convert Value to Boolean");
        }

        static public ValueType ParseValueType(string input)
        {
            switch (input.Trim().ToLower())
            {
                case "date":
                case "datetime":
                    return ValueType.Date;

                case "bool":
                case "boolean":
                    return ValueType.Date;

                case "number":
                case "int":
                case "double":
                    return ValueType.Number;

                case "object":
                case "class":
                case "struct":
                    return ValueType.Object;

                default:
                    return ValueType.Text;
            }
        }

        static public CustomValue Convert(CustomValue value, ValueType type)
        {
            if (value == null)
            {
                return null;
            }

            if (value.Type == type)
            {
                return value;
            }

            switch (type)
            {
                case ValueType.Number:
                    return double.Parse(value.ToString());
                case ValueType.Date:
                    return DateTime.ParseExact(value.ToString(), CustomValueDate.DEFAULT_FORMAT, null);
                case ValueType.Bool:
                    var val = value.ToString().ToLower().Trim();
                    if (val.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return CustomValueBool.True;
                    }

                    if (val.Equals("false", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return CustomValueBool.False;
                    }

                    if (val == "1")
                    {
                        return true;
                    }

                    return false;
                default:
                    return value.ToString();
            }
        }
    }

    /// <summary>
    /// Represents the Object Value item 
    /// </summary>
    public class CustomValue<T> : CustomValue
    {
        protected internal T _value;

        public override ValueType Type => ValueType.Object;
        public override object Value => _value;

        public override object GetValueObject() => _value;

        public CustomValue(T value) { _value = value; }

        public override string ToString() => _value?.ToString() ?? "";

        static public implicit operator T(CustomValue<T> value)
        { return value._value; }

        static public implicit operator CustomValue<T>(T value)
        { return new CustomValue<T>(value); }
    }

    /// <summary>
    /// Represents the Boolean Value item
    /// </summary>
    public class CustomValueBool : CustomValue<bool>
    {
        public override ValueType Type => ValueType.Bool;

        public CustomValueBool(bool value)
            : base(value) { }

        public override string ToString() => _value ? "true" : "false";

        static public implicit operator CustomValueBool(bool flag)
        { return new CustomValueBool(flag); }

        static public implicit operator bool(CustomValueBool value)
        { return value._value; }

        static public CustomValueBool True { get { return new CustomValueBool(true); } }
        static public CustomValueBool False { get { return new CustomValueBool(false); } }
    }

    /// <summary>
    /// Represents the DateTime Value item from the Report 
    /// </summary>
    public class CustomValueDate : CustomValue<DateTime>
    {
        public const string DEFAULT_FORMAT = "dd.MM.yyyy";

        protected string _format;

        public override ValueType Type => ValueType.Date;

        public CustomValueDate(DateTime value, string format = DEFAULT_FORMAT)
            : base(value) { _format = format; }

        public override string ToString() => _value.ToString(_format);

        static public implicit operator CustomValueDate(DateTime date)
        { return new CustomValueDate(date); }

        static public implicit operator DateTime(CustomValueDate date)
        { return date._value; }
    }

    /// <summary>
    /// Represents the Numeric Value item from the Report 
    /// </summary>
    public class CustomValueNumber : CustomValue<decimal>
    {
        public override ValueType Type => ValueType.Number;

        public CustomValueNumber(decimal number)
            : base(number) { }

        public override string ToString() => _value.ToString();

        static public implicit operator double(CustomValueNumber number)
        { return (double)number._value; }

        static public implicit operator CustomValueNumber(decimal number)
        { return new CustomValueNumber(number); }

        static public implicit operator CustomValueNumber(double number)
        { return new CustomValueNumber((decimal)number); }

        static public implicit operator CustomValueNumber(int number)
        { return new CustomValueNumber(number); }
    }

    /// <summary>
    /// Represents the Text Value item from the Report 
    /// </summary>
    public class CustomValueText : CustomValue<string>
    {
        public override ValueType Type => ValueType.Text;

        public CustomValueText(string text)
            : base(text) { }

        public override string ToString() => _value;

        static public implicit operator string(CustomValueText text)
        { return text._value; }

        static public implicit operator CustomValueText(string text)
        { return new CustomValueText(text); }
    }
}
