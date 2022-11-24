namespace WebApi.Common.Metadata
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class LongRunningRequestAttribute : Attribute
    {
        public LongRunningRequestAttribute(double time, MeasurementUnit unit)
        {
            if (time <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(time), time, "Value must be larger than 0.");
            }

            Threshold = unit switch
            {
                MeasurementUnit.Millisecond => TimeSpan.FromMilliseconds(time),
                MeasurementUnit.Second => TimeSpan.FromSeconds(time),
                MeasurementUnit.Minute => TimeSpan.FromMinutes(time),
                _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null),
            };
        }

        public TimeSpan Threshold { get; }
    }
}