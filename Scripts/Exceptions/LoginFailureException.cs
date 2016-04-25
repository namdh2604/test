namespace Voltage.Witches.Exceptions
{
    public class LoginFailureException : UserFacingException
    {
        public LoginFailureException() : base("Could not log in to game")
        {
        }
    }
}

