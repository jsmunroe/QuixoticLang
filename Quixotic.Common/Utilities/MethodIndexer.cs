using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace Quixotic.Common.Utilities
{
    public class MethodIndexer<TDelegate, TKeyType>
        where TDelegate : Delegate
    {
        private readonly Type _targetType;
        private readonly string _methodName;
        private readonly Dictionary<Type, TDelegate> _methodMap = [];

        private readonly MethodInfo _delegateInvokeMethod;
        private readonly Type[] _delegateParameterTypes;
        private readonly Type _delegateReturnType;

        private readonly int _keyParameterIndex;

        public MethodIndexer(Type targetType, string methodName)
        {
            _targetType = targetType;
            _methodName = methodName;

            // Introspect the delegate type to understand its signature
            _delegateInvokeMethod = typeof(TDelegate).GetMethod("Invoke")
                ?? throw new InvalidOperationException($"{typeof(TDelegate).Name} is not a valid delegate type.");

            _delegateParameterTypes = [.. _delegateInvokeMethod.GetParameters().Select(p => p.ParameterType)];
            _delegateReturnType = _delegateInvokeMethod.ReturnType;

            _keyParameterIndex = _delegateParameterTypes.IndexOf(typeof(TKeyType)) - 1;

            if (_keyParameterIndex < 0)
                throw new InvalidOperationException($"No argument in the delegate '{typeof(TDelegate).Name}' isof type '{typeof(TKeyType).Name}'");

            LoadMethodMap();
        }

        public Dictionary<Type, TDelegate> MethodMap => _methodMap;

        private void LoadMethodMap()
        {
            var methods = _targetType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                                .Where(m => m.Name == _methodName);

            foreach (var method in methods)
            {
                if (TryCreateEntry(method, out var delegateInstance))
                {
                    var methodParams = method.GetParameters();

                    if (methodParams.Length > _keyParameterIndex)
                    {
                        var keyType = methodParams[_keyParameterIndex].ParameterType;
                        _methodMap[keyType] = delegateInstance;
                    }
                }
            }
        }

        private bool TryCreateEntry(MethodInfo method, [NotNullWhen(true)] out TDelegate? delegateInstance)
        {
            delegateInstance = null;

            // Check if return type matches
            if (_delegateReturnType != typeof(void) && !_delegateReturnType.IsAssignableFrom(method.ReturnType))
                return false;

            var methodParams = method.GetParameters();
            var isStatic = method.IsStatic;

            // Delegate signature: (InstanceType, MethodParam0, MethodParam1, ...) -> ReturnType
            // Instance method: MethodParam0 Methodname(MethodParam1, ...) 
            // Static method: static MethodParam0 Methodname(MethodParam1, ...)
            //
            // The first delegate parameter is ALWAYS the instance (for calling instance methods)
            // Remaining delegate parameters map 1:1 to actual method parameters

            // Delegate must have at least (InstanceType, OneMethodParameter)
            if (_delegateParameterTypes.Length < 2)
                return false;

            // First delegate parameter must be the target type
            if (_delegateParameterTypes[0] != _targetType)
                return false;

            // Check parameter count: delegate params - 1 (instance) = method params
            var expectedMethodParamCount = _delegateParameterTypes.Length - 1;
            if (methodParams.Length != expectedMethodParamCount)
                return false;

            // Build expression parameters for the delegate
            var expressionParams = new ParameterExpression[_delegateParameterTypes.Length];
            for (int i = 0; i < _delegateParameterTypes.Length; i++)
            {
                expressionParams[i] = Expression.Parameter(_delegateParameterTypes[i], $"param{i}");
            }

            var instanceParam = expressionParams[0]; // Always the first parameter
            var methodCallArgs = new Expression[methodParams.Length];

            // Map delegate parameters to method parameters
            for (int methodParamIndex = 0; methodParamIndex < methodParams.Length; methodParamIndex++)
            {
                // Delegate: [0]=instance, [1]=methodParam0, [2]=methodParam1, ...
                var delegateParamIndex = methodParamIndex + 1; // Skip the instance parameter
                var delegateParam = expressionParams[delegateParamIndex];
                var methodParamType = methodParams[methodParamIndex].ParameterType;

                // Check if we need to cast (for polymorphism: QxExpression -> QxBinaryExpression)
                if (delegateParam.Type != methodParamType)
                {
                    // delegateParam must be assignable TO methodParamType
                    // Example: QxExpression (delegate) -> QxBinaryExpression (method)
                    if (!delegateParam.Type.IsAssignableFrom(methodParamType))
                        return false;

                    methodCallArgs[methodParamIndex] = Expression.Convert(delegateParam, methodParamType);
                }
                else
                {
                    methodCallArgs[methodParamIndex] = delegateParam;
                }
            }

            // Build the method call
            // Instance method: instanceParam.Method(args)
            // Static method: TargetType.Method(args) - instanceParam ignored
            var call = Expression.Call(
                isStatic ? null : instanceParam,
                method,
                methodCallArgs);

            // Build and compile the lambda
            var lambda = Expression.Lambda<TDelegate>(call, expressionParams);
            delegateInstance = lambda.Compile();

            return true;
        }

        public bool TryGetMethod(Type key, [NotNullWhen(returnValue: true)] out TDelegate? delegateInstance)
        {
            return _methodMap.TryGetValue(key, out delegateInstance);
        }

        public bool TryGetMethod(object argument, [NotNullWhen(returnValue: true)] out TDelegate? delegateInstance)
        {
            delegateInstance = null;

            if (argument is null)
                return false;

            var argumentType = argument.GetType();

            if (_methodMap.TryGetValue(argumentType, out delegateInstance))
                return true;

            var closestDistance = double.PositiveInfinity;
            TDelegate? closestMethod = null;
            foreach (var kvp in _methodMap)
            {
                var distance = GetInheritanceDistance(argumentType, kvp.Key);

                if (distance < 0)
                    continue;

                if (closestMethod == null || distance < closestDistance)
                {
                    closestDistance = distance;
                    closestMethod = kvp.Value;
                }
            }

            delegateInstance = closestMethod;
            return delegateInstance is not null;
        }

        public static int GetInheritanceDistance(Type type, Type baseType)
        {
            if (!baseType.IsAssignableFrom(type))
                return -1;

            int distance = 0;

            while (type != baseType)
            {
                type = type.BaseType!;
                distance++;
            }

            return distance;
        }
    }
}
