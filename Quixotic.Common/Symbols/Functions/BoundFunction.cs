using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Symbols;

namespace Quixotic.Common.Symbols.Functions
{
    public class BoundFunction : Function
    {
        public BoundFunction(Instance boundInstance, BindableFunction function) : base(function)
        {
            BoundInstance = boundInstance;

            Parameters.Insert(0, new Parameter("this", boundInstance.Type));
        }

        public Instance BoundInstance { get; }

        public override List<Argument> BindArguments(string name, Instance[] instances)
        {
            instances = [BoundInstance, .. instances];

            if (Parameters.Count != instances.Length)
                throw new ParameterCountException(name, Parameters.Count, instances.Length);

            List<Argument> arguments = [];

            // Push function parameters into 
            foreach (var (parameter, instance) in Parameters.Zip(instances))
            {
                if (!parameter.Type.IsAssignableFrom(instance.Type))
                    throw new TypeMismatchException(instance.Type, parameter.Type);

                arguments.Add(new Argument(parameter.Name, instance));
            }

            return arguments;
        }

        public override BoundFunction Substitute(GenericBindings bindings)
        {
            var function = base.Substitute(bindings);
            var bindableFunction = function is BindableFunction bindable ? bindable : new BindableFunction(BoundInstance.Type, function);

            return new BoundFunction(BoundInstance, bindableFunction);
        }
    }
}
