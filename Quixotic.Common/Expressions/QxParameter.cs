namespace Quixotic.Common.Expressions
{
    public class QxParameter(string name, string typeName)
    {
        public string Name { get; } = name;

        public string TypeName { get; } = typeName;
    }

}
