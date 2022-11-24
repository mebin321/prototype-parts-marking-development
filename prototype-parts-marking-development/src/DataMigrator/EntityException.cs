namespace DataMigrator
{
    using System;

    class EntityException : Exception
    {
        public EntityException(string message)
            : base(message)
        {
        }
    }
}
