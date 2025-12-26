using System.Reflection;

namespace TS.Shared.Enumeration
{
    public class StringEnumeration : IComparable
    {
        public string Name { get; private set; }

        public string Key { get; private set; }

        protected StringEnumeration(string key, string name) => (Key, Name) = (key, name);

        public override string ToString() => Name;

        public static IEnumerable<T> GetAll<T>() where T : StringEnumeration =>
            typeof(T).GetFields(BindingFlags.Public |
                                BindingFlags.Static |
                                BindingFlags.DeclaredOnly)
                     .Select(f => f.GetValue(null))
                     .Cast<T>();

        public override bool Equals(object? obj)
        {
            if (obj is not StringEnumeration otherValue)
            {
                return false;
            }

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = Key.Equals(otherValue.Key);

            return typeMatches && valueMatches;
        }

        public int CompareTo(object? other) => Key.CompareTo(((StringEnumeration)other!).Key);

        public override int GetHashCode() => base.GetHashCode();

        protected static T FromName<T>(string roleString) where T : StringEnumeration =>
            GetAll<T>().Single(r => string.Equals(r.Name, roleString, StringComparison.OrdinalIgnoreCase));      

        protected static T FromKey<T>(string key) where T : StringEnumeration =>
            GetAll<T>().Single(r => r.Key == key);
        
        public static Dictionary<string, string> ToDictionary<T>() where T : StringEnumeration =>
            GetAll<T>().ToDictionary(e => e.Key, e => e.Name);
    }
}
