namespace WebApi.Common.PrototypeIdentifier
{
    using System;

    public class Base36Converter : IPrototypeCounterConverter
    {
        private const int Base = 36;
        private const int Minimum = 0;
        private const int Maximum = 1679616 - 1; // 36^4, starting from 0

        private const string Characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public string IdentifierFrom(int counter)
        {
            if (counter < Minimum || counter > Maximum)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(counter),
                    counter,
                    $"Value must be between {Minimum} and {Maximum}.");
            }

            return string.Create(4, counter, (buffer, value) =>
            {
                for (var i = 3; i >= 0; i--)
                {
                    buffer[i] = Characters[value % Base];
                    value /= Base;
                }
            });
        }
    }
}