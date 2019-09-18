using System;

namespace Beehive.Model
{
    public class CronParseException : Exception
    {
        public CronParseException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
