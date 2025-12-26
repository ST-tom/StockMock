using System.Reflection;

namespace TS.Shared.Enumeration
{
    public abstract class IntEnumeration : IComparable
    {
        public string Name { get; private set; }

        public int Id { get; private set; }

        protected IntEnumeration(int id, string name) => (Id, Name) = (id, name);

        public override string ToString() => Name;

        public static IEnumerable<T> GetAll<T>() where T : IntEnumeration =>
            typeof(T).GetFields(BindingFlags.Public |
                                BindingFlags.Static |
                                BindingFlags.DeclaredOnly)
                     .Select(f => f.GetValue(null))
                     .Cast<T>();

        public override bool Equals(object? obj)
        {
            if (obj is not IntEnumeration otherValue)
            {
                return false;
            }

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }

        public int CompareTo(object? other) => Id.CompareTo(((IntEnumeration)other!).Id);

        public override int GetHashCode() => base.GetHashCode();

        public static Dictionary<int, string> ToDictionary<T>() where T : IntEnumeration => 
            GetAll<T>().ToDictionary(e => e.Id, e => e.Name);

        // Other utility methods ...
    }
}
