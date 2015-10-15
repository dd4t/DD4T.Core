﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DD4T.ViewModels.Attributes;
using DD4T.Core.Contracts.ViewModels;
using DD4T.ViewModels.Reflection;
using DD4T.ViewModels;
using DD4T.ViewModels.Exceptions;
using DD4T.ContentModel;
using System.Reflection;
using System.Collections;

namespace DD4T.ViewModels.Attributes
{
    /// <summary>
    /// An embedded schema field
    /// </summary>
    public class EmbeddedSchemaFieldAttribute : NestedModelFieldAttributeBase
    {
        public override IEnumerable GetRawValues(IField field)
        {
            return field.EmbeddedValues;
        }

        public Type EmbeddedModelType
        {
            get;
            set;
        }

        protected override IModel BuildModelData(object value, IField field, ITemplate template)
        {
            return new EmbeddedFields
            {
                Fields = (IFieldSet)value,
                MetadataFields = null,
                EmbeddedSchema = field.EmbeddedSchema,
                Template = template
            };
        }

        protected override Type GetModelType(IModel data, IViewModelFactory factory, IModelProperty property)
        {
            // TODO: read type from model property
            return EmbeddedModelType;
        }

        protected override bool ReturnRawData
        {
            get { return false; }
        }
    }

    /// <summary>
    /// A Component Link Field
    /// </summary>
    /// <example>
    /// To create a multi value linked component with a custom return Type:
    ///     [LinkedComponentField(FieldName = "content", LinkedComponentTypes = new Type[] { typeof(GeneralContentViewModel) }, AllowMultipleValues = true)]
    ///     public ViewModelList&lt;GeneralContentViewModel&gt; Content { get; set; }
    ///     
    /// To create a single linked component using the default DD4T type:
    ///     [LinkedComponentField(FieldName = "internalLink")]
    ///     public IComponent InternalLink { get; set; }
    /// </example>
    /// <remarks>
    /// This requires the Property to be a concrete type with a constructor that implements ICollection&lt;T&gt; or is T[]
    /// </remarks>
    public class LinkedComponentFieldAttribute : NestedModelFieldAttributeBase
    {
        public override IEnumerable GetRawValues(IField field)
        {
            return field.LinkedComponentValues;
        }
        // TODO: remove once reading type from model property works
        public Type[] LinkedComponentTypes //Is there anyway to enforce the types passed to this?
        {
            get;
            set;
        }
        protected override IModel BuildModelData(object value, IField field, ITemplate template)
        {
            //Assuming the use of DD4T Content Model here
            var component = (Component)value;
            return new ComponentPresentation
            {
                Component = component,
                ComponentTemplate = template as ComponentTemplate
            };
        }

        protected override Type GetModelType(IModel data, IViewModelFactory factory, IModelProperty property)
        {
            if (LinkedComponentTypes.Count() == 0)
            {
                return property.ModelType;
            }
            Type result = null;
            try
            {
                result = factory.FindViewModelByAttribute<IContentModelAttribute>(data, LinkedComponentTypes);
            }
            catch (ViewModelTypeNotFoundException)
            {
                result = null;
            }
            return result;
        }

        protected override bool ReturnRawData
        {
            get
            {
                return false; // return LinkedComponentTypes == null; // If we base our return type on the type of the model property, we never have to return the raw data type
            }
        }
    }

    public class KeywordFieldAttribute : NestedModelFieldAttributeBase
    {
        public override IEnumerable GetRawValues(IField field)
        {
            return field.Keywords;
        }
        // TODO: remove once reading type from model property works
        public Type KeywordType { get; set; }
        protected override IModel BuildModelData(object value, IField field, ITemplate template)
        {
            var keyword = (IKeyword)value;
            return keyword;
        }

        protected override Type GetModelType(IModel data, IViewModelFactory factory, IModelProperty property)
        {
            return property.ModelType;
            //return KeywordType;
        }

        protected override bool ReturnRawData
        {
            get { return KeywordType == null; }
        }
    }
}
