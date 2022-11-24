namespace WebApi
{
    public static class Validations
    {
        public static bool BeNumeric(string value)
        {
            if (value is null)
            {
                return false;
            }

            foreach (var character in value)
            {
                if (!char.IsNumber(character))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool NotContainWhitespace(string value)
        {
            if (value is null)
            {
                return false;
            }

            foreach (var character in value)
            {
                if (char.IsWhiteSpace(character))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
