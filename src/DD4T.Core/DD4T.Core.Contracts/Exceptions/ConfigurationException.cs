namespace DD4T.ContentModel.Exceptions
{
    using System;

    public class ConfigurationException : Exception //ApplicationException
    {
        public ConfigurationException()
            : base()
        {
        }

        public ConfigurationException(string message)
            : base(message)
        {
        }

        public ConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}