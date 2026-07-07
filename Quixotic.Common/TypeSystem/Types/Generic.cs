namespace Quixotic.Common.TypeSystem.Types
{
    public class Generic(string key) : QxType($"<{key}>")
    {
        public string Key { get; } = key;

        public static void GetKeyValues(QxType type, QxType replacement, Dictionary<string, QxType> genericTypes)
        {
            if (type is Generic generic)
                genericTypes[generic.Key] = replacement;

            if (type is ArrayType array && replacement is ArrayType arrayReplacement)
                GetKeyValues(array.ElementType, arrayReplacement.ElementType, genericTypes);
        }

        public static QxType SetKeyValues(QxType type, Dictionary<string, QxType> keyValues)
        {
            if (type is Generic generic)
            {
                if (keyValues.TryGetValue(generic.Key, out var replacement))
                    return replacement;
                else
                    return type;
            }

            if (type is ArrayType array)
            {
                var newElementType = SetKeyValues(array.ElementType, keyValues);
                if (newElementType != array.ElementType)
                    return Array(newElementType);
                else
                    return type;
            }

            return type;
        }
    }
}
