using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DD4T.ViewModels.Attributes;
using DD4T.ViewModels.Contracts;
using DD4T.ViewModels.Reflection;
using DD4T.Mvc.Html;
using System.Web.Mvc;
using DD4T.ViewModels;
using DD4T.ViewModels.Exceptions;
using DD4T.ContentModel;
using System.Reflection;
using System.Collections;

namespace DD4T.ViewModels.Attributes
{
    /// <summary>
    /// An Attribute for a Property representing the Link Resolved URL for a Linked or Multimedia Component
    /// </summary>
    /// <remarks>Uses the default DD4T GetResolvedUrl extension method. To override behavior you must implement
    /// your own Field Attribute. Future DD4T versions will hopefully allow for IoC of this implementation.</remarks>
    public class ResolvedUrlFieldAttribute : FieldAttributeBase
    {
        //public ResolvedUrlFieldAttribute(string fieldName) : base(fieldName) { }
        public override IEnumerable GetFieldValues(IField field, IModelProperty property, ITemplate template, IViewModelFactory factory)
        {
            return field.LinkedComponentValues
                .Select(x => x.GetResolvedUrl());
        }

        public override Type ExpectedReturnType
        {
            get { return typeof(string); }
        }
    }

    /// <summary>
    /// A Multimedia component field
    /// </summary>
    public class MultimediaFieldAttribute : FieldAttributeBase
    {
        //public MultimediaFieldAttribute(string fieldName) : base(fieldName) { }
        public override IEnumerable GetFieldValues(IField field, IModelProperty property, ITemplate template, IViewModelFactory factory)
        {
            return field.LinkedComponentValues.Select(x => x.Multimedia);
        }

        public override Type ExpectedReturnType
        {
            get { return typeof(IMultimedia); }
        }
    }
    /// <summary>
    /// A text field
    /// </summary>
    public class TextFieldAttribute : FieldAttributeBase, ICanBeBoolean
    {
        public override IEnumerable GetFieldValues(IField field, IModelProperty property, ITemplate template, IViewModelFactory factory)
        {
            IEnumerable fieldValue = null;
            var values = field.Values;
            if (IsBooleanValue)
                fieldValue = values.Select(v => { bool b; return bool.TryParse(v, out b) && b; });
            else fieldValue = values;

            return fieldValue;
        }

        /// <summary>
        /// Set to true to parse the text into a boolean value.
        /// </summary>
        public bool IsBooleanValue { get; set; }
        public override Type ExpectedReturnType
        {
            get
            {
                return IsBooleanValue ? typeof(bool) : typeof(string);
            }
        }
    }
    /// <summary>
    /// A Rich Text field. Uses the default ResolveRichText extension method.
    /// </summary>
    /// <remarks>This Attribute is dependent on a specific implementation for resolving Rich Text. 
    /// In future versions of DD4T, the rich text resolver will hopefully be abstracted to allow for IoC, 
    /// but for now, to change the behavior you must implement your own Attribute.</remarks>
    public class RichTextFieldAttribute : FieldAttributeBase
    {
        //public RichTextFieldAttribute(string fieldName) : base(fieldName) { }
        public override IEnumerable GetFieldValues(IField field, IModelProperty property, ITemplate template, IViewModelFactory factory)
        {
            return field.Values
                .Select(v => v.ResolveRichText()); //Hidden dependency on DD4T Resolve Rich Text implementation
        }

        public override Type ExpectedReturnType
        {
            get { return typeof(MvcHtmlString); }
        }
    }
    /// <summary>
    /// A Number field
    /// </summary>
    public class NumberFieldAttribute : FieldAttributeBase
    {
        //public NumberFieldAttribute(string fieldName) : base(fieldName) { }
        public override IEnumerable GetFieldValues(IField field, IModelProperty property, ITemplate template, IViewModelFactory factory)
        {
            return field.NumericValues;
        }

        public override Type ExpectedReturnType
        {
            get { return typeof(double); }
        }

    }
    /// <summary>
    /// A Date/Time field
    /// </summary>
    public class DateFieldAttribute : FieldAttributeBase
    {
        //public DateFieldAttribute(string fieldName) : base(fieldName) { }
        public override IEnumerable GetFieldValues(IField field, IModelProperty property, ITemplate template, IViewModelFactory factory)
        {
            return field.DateTimeValues;
        }

        public override Type ExpectedReturnType
        {
            get { return typeof(DateTime); }
        }
    }
    /// <summary>
    /// The Key of a Keyword field. 
    /// </summary>
    public class KeywordKeyFieldAttribute : FieldAttributeBase, ICanBeBoolean
    {
        /// <summary>
        /// The Key of a Keyword field.
        /// </summary>
        /// <param name="fieldName">Tridion schema field name</param>
        //public KeywordKeyFieldAttribute(string fieldName) : base(fieldName) { }
        public override IEnumerable GetFieldValues(IField field, IModelProperty property, ITemplate template, IViewModelFactory factory)
        {
            IEnumerable value = null;
            var values = field.Keywords;
            if (IsBooleanValue)
                value = values.Select(k => { bool b; return bool.TryParse(k.Key, out b) && b; });
            else value = values.Select(k => k.Key);
            return value;
        }

        /// <summary>
        /// Set to true to parse the Keyword Key into a boolean value.
        /// </summary>
        public bool IsBooleanValue { get; set; }
        public override Type ExpectedReturnType
        {
            get
            {
                return IsBooleanValue ? typeof(bool) : typeof(string);
            }
        }
    }
    /// <summary>
    /// The Key of a Keyword as a number
    /// </summary>
    public class NumericKeywordKeyFieldAttribute : FieldAttributeBase
    {
        //public NumericKeywordKeyFieldAttribute(string fieldName) : base(fieldName) { }
        public override IEnumerable GetFieldValues(IField field, IModelProperty property, ITemplate template, IViewModelFactory factory)
        {
            return field.Keywords
                .Select(k => { double i; double.TryParse(k.Key, out i); return i; });
        }

        public override Type ExpectedReturnType
        {
            get
            {
                return typeof(double);
            }
        }
    }
    /// <summary>
    /// The URL of the Multimedia data of the view model
    /// </summary>
    public class MultimediaUrlAttribute : ComponentAttributeBase
    {
        public override IEnumerable GetPropertyValues(IComponent component, IModelProperty property, ITemplate template, IViewModelFactory factory)
        {
            IEnumerable result = null;
            if (component != null && component.Multimedia != null)
            {
                result = new string[] { component.Multimedia.Url };
            }
            return result;
        }

        public override Type ExpectedReturnType
        {
            get { return typeof(String); }
        }
    }
    /// <summary>
    /// The Multimedia data of the view model
    /// </summary>
    public class MultimediaAttribute : ComponentAttributeBase
    {
        public override IEnumerable GetPropertyValues(IComponent component, IModelProperty property, ITemplate template, IViewModelFactory factory)
        {
            IMultimedia result = null;
            if (component != null && component.Multimedia != null)
            {
                result = component.Multimedia;
            }
            return new IMultimedia[] { result };
        }

        public override Type ExpectedReturnType
        {
            get { return typeof(IMultimedia); }
        }
    }
    /// <summary>
    /// The title of the Component (if the view model represents a Component)
    /// </summary>
    public class ComponentTitleAttribute : ComponentAttributeBase
    {
        public override IEnumerable GetPropertyValues(IComponent component, IModelProperty property, ITemplate template, IViewModelFactory factory)
        {
            return component == null ? null : new string[] { component.Title };
        }
        public override Type ExpectedReturnType
        {
            get { return typeof(String); }
        }
    }
    /// <summary>
    /// A DD4T IMultimedia object representing the multimedia data of the model
    /// </summary>
    public class DD4TMultimediaAttribute : ComponentAttributeBase
    {
        //Example of using the BaseData object

        public override IEnumerable GetPropertyValues(IComponent component, IModelProperty property, ITemplate template, IViewModelFactory factory)
        {
            IMultimedia[] result = null;
            if (component != null && component.Multimedia != null)
            {
                result = new IMultimedia[] { component.Multimedia };
            }
            return result;
        }

        public override Type ExpectedReturnType
        {
            get { return typeof(IMultimedia); }
        }
    }
    /// <summary>
    /// Component Presentations filtered by the DD4T CT Metadata "view" field
    /// </summary>
    public class PresentationsByViewAttribute : ComponentPresentationsAttributeBase
    {
        public override System.Collections.IEnumerable GetPresentationValues(IList<IComponentPresentation> cps, IModelProperty property, IViewModelFactory factory)
        {
            return cps.Where(cp =>
                    {
                        bool result = false;
                        if (cp.ComponentTemplate != null && cp.ComponentTemplate.MetadataFields != null
                            && cp.ComponentTemplate.MetadataFields.ContainsKey("view"))
                        {
                            var view = cp.ComponentTemplate.MetadataFields["view"].Values.FirstOrDefault();
                            if (view != null && view.StartsWith(ViewPrefix))
                            {
                                result = true;
                            }
                        }
                        return result;
                    })
                    .Select(cp =>
                        {
                            object model = null;
                            if (ComplexTypeMapping != null)
                            {
                                model = factory.BuildMappedModel(cp, ComplexTypeMapping);
                            }
                            else model = factory.BuildViewModel((cp));
                            return model;
                        });
        }
       
        public string ViewPrefix { get; set; }
        public override Type ExpectedReturnType
        {
            get { return typeof(IList<IViewModel>); }
        }
    }



    public class KeywordDataAttribute : ModelPropertyAttributeBase
    {
        public override IEnumerable GetPropertyValues(IModel modelData, IModelProperty property, IViewModelFactory factory)
        {
            IEnumerable result = null;
            if (modelData != null && modelData is IKeyword)
            {
                result = new IKeyword[] { modelData as IKeyword };
            }
            return result;
        }

        public override Type ExpectedReturnType
        {
            get { return typeof(IKeyword); }
        }
    }

    /// <summary>
    /// Field that is parsed into an Enum. Must be a Text field (not Keyword).
    /// </summary>
    public class EnumFieldAttribute : FieldAttributeBase
    {  
        public override IEnumerable GetFieldValues(IField field, IModelProperty property, ITemplate template, IViewModelFactory factory)
        {
            var result = new List<object>();
            foreach (var value in field.Values)
            {
                object parsed;
                if (EnumTryParse(property.ModelType, value, out parsed))
                {
                    result.Add(parsed);
                }
            }
            return result;
        }

        public override Type ExpectedReturnType
        {
            get { return typeof(Enum); }
        }

        private bool EnumTryParse(Type enumType, object value, out object parsedEnum)
        {
            bool result = false;
            parsedEnum = null;
            if (value != null)
            {
                try
                {
                    parsedEnum = Enum.Parse(enumType, value.ToString());
                    result = true;
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return result;
        }
    }
}
