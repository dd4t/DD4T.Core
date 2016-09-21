namespace DD4T.ContentModel.Exceptions
{
    using System;

    public class ModelNotCreatedException : Exception  //ApplicationException
    {
        public ModelNotCreatedException(string viewName)
            : base(string.Format("model for view '{0}' is not created", viewName))
        {
        }

        public ModelNotCreatedException(string viewName, Exception innerException)
            : base(string.Format("model for view '{0}' is not created", viewName), innerException)
        {
        }
    }
}