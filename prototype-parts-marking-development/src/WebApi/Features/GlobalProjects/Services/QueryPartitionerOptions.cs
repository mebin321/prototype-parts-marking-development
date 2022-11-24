namespace WebApi.Features.GlobalProjects.Services
{
    using System;

    public class QueryPartitionerOptions
    {
        private int partitionSize;

        public int PartitionSize
        {
            get => partitionSize;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Value must be greater than 0.");
                }

                partitionSize = value;
            }
        }
    }
}