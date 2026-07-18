using Quixotic.Common.Syntax;
using Quixotic.Common.Syntax.Casing;
using Quixotic.Common.TypeSystem;
using Quixotic.Common.Utilities;
using Quixotic.Interop.Attributes;
using Quixotic.Interop.TypeSystem.Types;
using System.Diagnostics;
using System.Reflection;

namespace Quixotic.Interop
{
    public class TypeImporter(ClrMarshaller marshaller)
    {
        private readonly static MethodIndexer<Action<TypeImporter, ClrType, MemberInfo>, MemberInfo> _imports = new(typeof(TypeImporter), "Import");

        public bool Import(ClrType clrType)
        {
            var type = clrType.ExternalType;

            foreach (var member in type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                if (member.GetCustomAttribute<QuixoticIgnoreAttribute>() is not null)
                    continue;

                if (!_imports.TryGetMethod(member, out var action))
                {
                    Debug.WriteLine($"Could not find Import handler for {member.GetType().Name}.");
                    return false;
                }

                try
                {
                    action(this, clrType, member);
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        protected void Import(ClrType clrType, ConstructorInfo constructorInfo)
        {
            Instance callConstructor(Instance[] arguments)
            {
                var instance = arguments[0];
                arguments = arguments[1..];

                var args = marshaller.Unwrap(arguments);

                var clrInstance = constructorInfo.Invoke([.. args]);

                instance["instance"] = clrInstance;

                return instance;
            }

            clrType.RegisterConstructor(callConstructor, [.. marshaller.Wrap(constructorInfo.GetParameters())]);
        }

        protected void Import(ClrType _clrType, EventInfo _eventInfo)
        {

        }

        protected void Import(ClrType clrType, FieldInfo fieldInfo)
        {
            var name = CaseRule.Current.LocalNames.Recase(fieldInfo.Name);
            var isStatic = fieldInfo.IsStatic;

            var type = marshaller.Wrap(fieldInfo.FieldType);

            Instance callGetter(params Instance[] arguments)
            {
                var instance = isStatic ? null : arguments[0];

                var clrInstance = marshaller.Unwrap(instance);

                var value = fieldInfo.GetValue(clrInstance);

                return marshaller.Wrap(value);
            }

            Instance callSetter(params Instance[] arguments)
            {
                var instance = isStatic ? null : arguments[0];
                var value = isStatic ? arguments[0] : arguments[1];

                var clrInstance = marshaller.Unwrap(instance);

                fieldInfo.SetValue(clrInstance, marshaller.Unwrap(value));

                return value;
            }

            if (fieldInfo.IsStatic)
            {
                if (fieldInfo.IsInitOnly)
                    clrType.RegisterStaticProperty(name, type, callGetter);
                else
                    clrType.RegisterStaticProperty(name, type, callGetter, callSetter);
            }
            else
            {
                if (fieldInfo.IsInitOnly)
                    clrType.RegisterProperty(name, type, callGetter);
                else
                    clrType.RegisterProperty(name, type, callGetter, callSetter);
            }
        }

        protected void Import(ClrType clrType, MethodInfo method)
        {
            var name = method.Name;

            if (method.IsSpecialName)
            {
                if (!ClrNames.Current.TryResolve(name, out var qxName))
                    return;

                name = qxName;
            }

            name = CaseRule.Current.MethodNames.Recase(name);

            var isStatic = method.IsStatic;

            Instance callMethod(Instance[] arguments)
            {
                var instance = isStatic ? null : arguments[0];
                arguments = isStatic ? arguments : arguments[1..];

                var args = marshaller.Unwrap(arguments);

                var clrInstance = marshaller.Unwrap(instance);

                var returnValue = method.Invoke(clrInstance, [.. args]);

                return marshaller.Wrap(returnValue);
            }

            if (method.IsStatic)
                clrType.RegisterStaticMethod(name, callMethod, marshaller.Wrap(method.ReturnType), [.. marshaller.Wrap(method.GetParameters())]);
            else
                clrType.RegisterMethod(name, callMethod, marshaller.Wrap(method.ReturnType), [.. marshaller.Wrap(method.GetParameters())]);
        }

        protected void Import(ClrType clrType, PropertyInfo property)
        {
            var name = CaseRule.Current.PropertyNames.Recase(property.Name);

            var type = marshaller.Wrap(property.PropertyType);

            var isStatic = property.GetMethod?.IsStatic ?? property.SetMethod?.IsStatic ?? false;

            Instance callGetter(params Instance[] arguments)
            {
                var instance = isStatic ? null : arguments[0];

                var clrInstance = marshaller.Unwrap(instance);

                var value = property.GetValue(clrInstance);

                return marshaller.Wrap(value);
            }

            Instance callSetter(params Instance[] arguments)
            {
                var instance = isStatic ? null : arguments[0];
                var value = isStatic ? arguments[0] : arguments[1];

                var clrInstance = marshaller.Unwrap(instance);

                property.SetValue(clrInstance, marshaller.Unwrap(value));

                return value;
            }

            if (property.SetMethod is null)
                clrType.RegisterProperty(name, type, callGetter);
            else
                clrType.RegisterProperty(name, type, callGetter, callSetter);
        }
    }
}
