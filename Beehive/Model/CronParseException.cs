using System;
using System.Collections.Generic;
using System.Text;

namespace Beehive.Model
{
    public class CronParseException : Exception
    {
        public CronParseException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
