using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RIVS.ASAK.Core.Contract.Values
{
    public class ParamValue : INamed
    {
        public string Name { get; set; }
        public CustomValue Value { get; set; }

        protected ParamValue() { }

        public ParamValue(string name, CustomValue value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString() => $"{Name ?? ""} = {Value?.ToString() ?? "NULL"}";

        public override int GetHashCode() => ToString().GetHashCode();

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(obj, this))
            {
                return true;
            }

            if (!(obj is ParamValue item))
            {
                return false;
            }

            return Name.Equals(item.Name) && Value.Equals(item.Value);
        }
    }

    public class ParamValueCollection : NamedCollection<ParamValue>, /*IValueProvider, */ITextByNameProvider
    {
        public const string TEXT_PARAM_VALUE_VARIABLE_PATTERN = @"(#|(?<=\s)@|^@)(?<paramname>[\w\-\.\d]+)(?<!\.)(#)?";

        private string _prefix = "";

        public string Prefix
        {
            get { return _prefix; }
            protected set { _prefix = string.IsNullOrEmpty(value) ? "" : value.Trim('.'); }
        }

        public ParamValueCollection()
            : base()
        {
        }

        public ParamValueCollection(string prefix)
            : base()
        {
            Prefix = prefix ?? "";
        }

        public ParamValueCollection(IEnumerable<ParamValue> collection, string prefix = "")
            : base(collection)
        {
            Prefix = prefix ?? "";
        }

        public CustomValue GetValue(string name)
        {
            return Get(name)?.Value;
        }

        //public T? GetValue<T>(string name)
        //{
        //    var value = this.GetValue(name);
        //    if (value == null)
        //        return null;

        //    Type type = typeof(T);
        //    if (value.Type == CustomValue.ValueType.Number && type == typeof(decimal) || type == typeof(int) || type == typeof(float) || type == typeof(long) || type == typeof(short) || type == typeof(byte))
        //        return (T)(value as CustomValueNumber)._value;

        //}

        public string GetText(string name)
        {
            return Get(name)?.Value?.ToString() ?? "";
        }

        public decimal? GetNumber(string name)
        {
            var value = GetValue(name);
            return value == null ? null : (value as CustomValueNumber)?._value;
        }

        public DateTime? GetDate(string name)
        {
            var value = GetValue(name);
            return value == null ? null : (value as CustomValueDate)?._value;
        }

        public bool? GetBool(string name)
        {
            var value = GetValue(name);
            return value == null ? null : (value as CustomValueBool)?._value;
        }

        public void Add(string name, CustomValue value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Имя параметра не может быть пусто");
            }

            Add(new ParamValue((Prefix + "." + name).Trim('.'), value));
        }

        public void Add(IEnumerable<ParamValue> values)
        {
            if (values == null)
            {
                return;
            }

            foreach (var value in values)
            {
                Add(value.Name, value.Value);
            }
        }

        public void Add(string prefix, IParamValueContainer container)
        {
            if (container != null)
            {
                Add(container.GetParamValues(prefix));
            }
        }

        public override bool Set(ParamValue item)
        {
            if (item == null)
            {
                return false;
            }

            //this[item.Name] = item.Value;

            var current = Get(item.Name);
            if (current != null)
            {
                Remove(current);
            }

            Add(new ParamValue(item.Name, item.Value));

            return true;
        }

        public void Set(string name, CustomValue value)
        {
            if (value == null)
            {
                return;
            }

            var current = Get(name);
            if (current != null)
            {
                Remove(current);
            }

            Add(new ParamValue(name, value));
        }

        public string Replace(string text, string replaceMissingParams = null)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            //var result = Regex.Replace(text, @"[#|@](?<paramname>[\w\-\.\d]+)(?<!\.)", match =>
            var result = Regex.Replace(text, TEXT_PARAM_VALUE_VARIABLE_PATTERN, match =>
            {
                var found = match.ToString();
                var paramname = match.Groups["paramname"].Value; //.Trim('.');
                return GetText(paramname) ?? replaceMissingParams ?? found;
                //var paramvalue = this.GetValue(paramname);
                //return paramvalue == null ? "" : paramvalue.ToString();
            });

            return result;
        }

        public List<KeyValuePair<string, CustomValue>> GetKeyValuePairs()
        {
            return this.Select(i => new KeyValuePair<string, CustomValue>(i.Name, i.Value)).ToList();
        }

        public List<KeyValuePair<string, string>> GetKeyStringPairs()
        {
            return this.Select(i => new KeyValuePair<string, string>(i.Name, i.ToString())).ToList();
        }

        static public ParamValueCollection GetObjectProperties(object obj, string prefix = "")
        {
            var result = new ParamValueCollection(prefix);

            if (obj == null)
            {
                return result;
            }

            var properties = obj.GetType().GetProperties();
            foreach (var p in properties)
            {
                var name = p.Name;
                var value = p.GetValue(obj, null);

                if (value == null)
                {
                    continue;
                }

                result.Add(GetParamValues(name, value));
            }

            return result;
        }

        static public ParamValueCollection GetParamValues(string name, object value)
        {
            var result = new ParamValueCollection();

            if (value == null)
            {
                return result;
            }

            var type = value.GetType();

            if (type == typeof(string))
            {
                result.Add(name, value.ToString());
            }
            else if (type == typeof(DateTime))
            {
                result.Add(name, (DateTime)value);
            }
            else if (type == typeof(DateTimeOffset))
            {
                result.Add(name, ((DateTimeOffset)value).DateTime);
            }
            else if (type.IsClass)
            {
                if (!typeof(IEnumerable).IsAssignableFrom(type) && !type.IsArray)
                {
                    result.Add(GetObjectProperties(value, name));
                }
            }
            else
            {
                result.Add(name, value.ToString());
            }

            return result;
        }

        public static ParamValueCollection Split(string text, params char[] separators)
        {
            var result = new ParamValueCollection();

            if (string.IsNullOrWhiteSpace(text))
            {
                return result;
            }

            var items = SplitFormatted(text, separators);
            foreach (var item in items)
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }

                var split = item.Split('=');
                if (split == null || split.Length != 2)
                {
                    throw new FormatException("Param's Name and Value should be separated by '='");
                }

                var name = split[0].Trim();
                if (name.Length > 2 && name[0] == name[name.Length - 1] && separators.Contains(name[0]))
                {
                    name = name.Substring(1, name.Length - 2);
                }

                var value = split[1].Trim();
                if (value.Length > 2 && value[0] == value[value.Length - 1] && separators.Contains(value[0]))
                {
                    value = value.Substring(1, value.Length - 2);
                }

                result.Add(name, value);
            }

            return result;
        }

        public static ParamValueCollection Split(string input)
        {
            return Split(input, ';');
        }



        public static IEnumerable<string> SplitFormatted(string input, params char[] separators)
        {
            var sb = new StringBuilder();
            bool quoted = false;
            bool apostrophed = false;

            foreach (char c in input)
            {
                if (quoted || apostrophed)
                {
                    if (quoted && c == '"')
                    {
                        quoted = false;
                    }
                    else if (apostrophed && c == '\'')
                    {
                        apostrophed = false;
                    }

                    sb.Append(c);
                }
                else
                {
                    if (separators.Contains(c))
                    {
                        yield return sb.ToString();
                        sb.Length = 0;
                        continue;
                    }

                    if (c == '"')
                    {
                        quoted = true;
                    }
                    else if (c == '\'')
                    {
                        apostrophed = true;
                    }

                    sb.Append(c);
                }
            }

            if (quoted || apostrophed)
            {
                throw new ArgumentException("Unterminated quotation mark.");
            }

            yield return sb.ToString();
        }

    }
}
