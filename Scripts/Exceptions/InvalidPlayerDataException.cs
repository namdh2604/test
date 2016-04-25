namespace Voltage.Witches.Exceptions
{
    public class InvalidPlayerDataException : UserFacingException
    {
        public InvalidPlayerDataException() : base("Could not create new player")
        {
        }
    }
}

