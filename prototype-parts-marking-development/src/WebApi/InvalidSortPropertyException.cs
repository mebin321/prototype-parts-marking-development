namespace WebApi
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class InvalidSortPropertyException : Exception
    {
        public InvalidSortPropertyException()
        {
        }

        public InvalidSortPropertyException(string message)
            : base(message)
        {
        }

        public InvalidSortPropertyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected InvalidSortPropertyException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}