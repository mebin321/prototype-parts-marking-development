namespace WebApi.Common.Logging
{
    using Serilog.Core;
    using Serilog.Events;

    public class DestructurablePolicy : IDestructuringPolicy
    {
        public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue result)
        {
            switch (value)
            {
                case IDestructurable destructurable:
                    result = propertyValueFactory.CreatePropertyValue(destructurable.Destructure(), true);
                    return true;
                default:
                    result = default;
                    return false;
            }
        }
    }
}