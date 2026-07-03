namespace Quixotic.Common.Types
{
    public class QxGeneric(string key) : QxType($"<{key}>")
    {
        public string Key { get; } = key;

        public static void GetKeyValues(QxType type, QxType replacement, Dictionary<string, QxType> genericTypes)
        {
            if (type is QxGeneric generic)
                genericTypes[generic.Key] = replacement;

            if (type is QxArrayType array && replacement is QxArrayType arrayReplacement)
                GetKeyValues(array.ElementType, arrayReplacement.ElementType, genericTypes);
        }

        public static QxType SetKeyValues(QxType type, Dictionary<string, QxType> keyValues)
        {
            if (type is QxGeneric generic)
            {
                if (keyValues.TryGetValue(generic.Key, out var replacement))
                    return replacement;
                else
                    return type;
            }

            if (type is QxArrayType array)
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
