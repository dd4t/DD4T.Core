﻿using DD4T.ContentModel;
using DD4T.Core.Contracts.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DD4T.ViewModels.Exceptions
{
    public class ViewModelTypeNotFoundException : Exception
    {
        public ViewModelTypeNotFoundException(IComponentPresentation data)
            : base(String.Format("Could not find view model for schema '{0}' and Template '{1}' or default for schema '{0}' in loaded assemblies."
                    , data.Component.Schema.Title, data.ComponentTemplate.Title)) 
        { }

        public ViewModelTypeNotFoundException(ITemplate data)
            : base(String.Format("Could not find view model for item with Template '{0}' and ID '{1}'", data.Title, data.Id))
        { }

        //TODO: REfactor to check type and use other overloads
        public ViewModelTypeNotFoundException(IModel data)
            : base(String.Format("Could not find view model for item with Publication ID '{0}'", data))
        { }
    }

    public class PropertyTypeMismatchException : Exception
    {
        public PropertyTypeMismatchException(IModelProperty fieldProperty, IPropertyAttribute fieldAttribute, object fieldValue) : 
            base(String.Format("Type mismatch for property '{0}'. Expected type for '{1}' is {2}. Model Property is of type {3}. Field value is of type {4}."
            , fieldProperty.Name, fieldAttribute.GetType().Name, fieldAttribute.ExpectedReturnType.FullName, fieldProperty.PropertyType.FullName,
            fieldValue.GetType().FullName)) { }
    }
}
