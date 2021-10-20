using System;
using System.Collections.Generic;
using System.Text;

namespace TrackingService.Exceptions
{
    public class MessageBrokerException : Exception
    {
        public MessageBrokerException(string message)
            : base(message)
        {
        }
    }
}
