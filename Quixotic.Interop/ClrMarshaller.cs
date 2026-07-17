using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Symbols;
using Quixotic.Common.TypeSystem.Types;
using Quixotic.Interop.TypeSystem.Types;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Quixotic.Interop
{
    public class ClrMarshaller(ClrTypeRegistry registry)
    {
        private bool TryCreate(object value, [NotNullWhen(returnValue: true)] out Instance? instance)
        {
            instance = null;

            if (TryCreate(value.GetType(), out var qxType) && qxType is ClrType clrType)
            {
                instance = clrType.Construct(value);
                return true;
            }

            return false;
        }

        public bool TryWrap(object? value, [NotNullWhen(returnValue: true)] out Instance? instance)
        {
            instance = value switch
            {
                null => QxType.Nada,
                int number => QxType.Number.Construct(number),
                double number => QxType.Number.Construct(number),
                string text => QxType.String.Construct(text),
                bool boolean => QxType.Boolean.Construct(boolean),
                _ when registry.TryResolve(value.GetType(), out var type) && type is ClrType clrType => clrType.Construct(value),
                _ => null,
            };

            if (instance is not null)
                return true;

            return TryCreate(value!, out instance);  // If value where null, the method would have already return QxType.Nada.
        }

        public Instance Wrap(object? value)
        {
            return TryWrap(value, out var instance) ? instance : throw new NotSupportedException($"Type '{value!.GetType().Name}' is not supported.");
        }

        public IEnumerable<Instance> Wrap(IEnumerable<object?> values)
        {
            foreach (var value in values)
                yield return Wrap(value);
        }

        private bool TryCreate(Type type, [NotNullWhen(returnValue: true)] out QxType? qxType)
        {
            var clrType = new ClrType(type);

            registry.Register(type, clrType);

            var typeImporter = new TypeImporter(this);
            if (typeImporter.Import(clrType))
            {
                qxType = clrType;
                return true;
            }

            qxType = null;
            return false;
        }

        public bool TryWrap(Type type, [NotNullWhen(returnValue: true)] out QxType? qxType)
        {
            qxType = type switch
            {
                _ when type == typeof(int) => QxType.Number,
                _ when type == typeof(double) => QxType.Number,
                _ when type == typeof(string) => QxType.String,
                _ when type == typeof(bool) => QxType.Boolean,
                _ when type == typeof(void) => QxType.Void.Type,
                _ when registry.TryResolve(type, out var type2) && type2 is ClrType clrType => clrType,
                _ => null,
            };

            if (qxType is not null)
                return true;

            return TryCreate(type, out qxType);
        }

        public QxType Wrap(Type type)
        {
            return TryWrap(type, out var instance) ? instance : throw new NotSupportedException($"Type '{type.Name} is not supported.");
        }

        public IEnumerable<QxType> Wrap(IEnumerable<Type> types)
        {
            foreach (var type in types)
                yield return Wrap(type);
        }

        public IEnumerable<Parameter> Wrap(IEnumerable<ParameterInfo> parameters)
        {
            var index = 0;

            foreach (var parameter in parameters)
            {
                var name = parameter.Name ?? $"param{index++}";
                var type = parameter.ParameterType;

                if (!TryWrap(type, out var qxType))
                    throw new NotSupportedException($"Type '{type.Name} is not supported.");

                yield return new Parameter(name, qxType);
            }
        }

        public IEnumerable<object?> Unwrap(IEnumerable<Instance?> instances)
        {
            foreach (var instance in instances)
            {
                if (!TryUnwrap(instance, out var value))
                    throw new NotSupportedException($"Type '{instance!.Type}' to is not supported.");

                yield return value;
            }
        }

        public bool TryUnwrap(Instance? instance, [MaybeNullWhen(returnValue: true)] out object? value)
        {
            if (instance is null || instance.IsNada)
            {
                value = null;
                return true;
            }

            if (instance.Type.Equals(QxType.Number))
            {
                value = QxType.Number.Get(instance);
                return true;
            }

            if (instance.Type.Equals(QxType.String))
            {
                value = QxType.String.Get(instance);
                return true;
            }

            if (instance.Type.Equals(QxType.Boolean))
            {
                value = QxType.Boolean.Get(instance);
                return true;
            }

            if (instance.Type is ClrType)
            {
                value = instance["instance"];
                return true;
            }

            value = null;
            return false;
        }

        public IEnumerable<Type> UnWrap(IEnumerable<QxType> qxTypes)
        {
            foreach (var qxType in qxTypes)
            {
                if (!TryUnwrap(qxType, out var type))
                    throw new NotSupportedException($"Type '{qxType}' to is not supported.");

                yield return type;
            }
        }

        public bool TryUnwrap(QxType qxType, [NotNullWhen(returnValue: true)] out Type? type)
        {
            if (qxType.Equals(QxType.Number))
            {
                type = typeof(double);
                return true;
            }

            if (qxType.Equals(QxType.String))
            {
                type = typeof(string);
                return true;
            }

            if (qxType.Equals(QxType.Boolean))
            {
                type = typeof(bool);
                return true;
            }

            if (qxType is ClrType clrType)
            {
                type = clrType.ExternalType;
                return true;
            }

            type = null;
            return false;
        }
    }
}
