using Quixotic.Interpret.Values;

namespace Quixotic.Interpret.Environment
{
    // I changed this to scope because C# already has a static Environment object
    // under the System namespace, and it was colliding with this class.
    public class Scope
    {
        private readonly Dictionary<string, Identifier> _values = [];

        public bool ContainsName(string name) => _values.ContainsKey(name);

        public Value this[string name]
        {
            get
            {
                var identifier = _values.GetValueOrDefault(name);
                return (identifier is null) ? new NadaValue() : identifier.Value;
            }
            set
            {
                if (_values.TryGetValue(name, out Identifier? identifier))
                    identifier.Assign(value); // Throws TypeConversionException if type of value is not type of set identifer.
                else
                    _values[name] = new Identifier(value);
            }
        }
    }
}
