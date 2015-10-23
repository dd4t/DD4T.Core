using DD4T.ContentModel;
using DD4T.Core.Contracts.Resolvers;
using DD4T.Core.Contracts.ViewModels;
using DD4T.Core.Contracts.ViewModels.Binding;
using DD4T.ViewModels.Binding;
using DD4T.ViewModels.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DD4T.ViewModels
{
    public class ViewModelFactory : IViewModelFactory
    {
        private readonly IDictionary<IModelAttribute, Type> viewModels = new Dictionary<IModelAttribute, Type>();
        private readonly HashSet<Assembly> loadedAssemblies = new HashSet<Assembly>();
        private readonly IViewModelResolver _resolver;
        private readonly IViewModelKeyProvider _keyProvider;
        private readonly ILinkResolver _linkResolver;
        private readonly IRichTextResolver _richtTextResolver;
        private readonly IContextResolver _contextResolver;
        
        /// <summary>
        /// New View Model Builder
        /// </summary>
        /// <param name="keyProvider">A View Model Key provider</param>
        public ViewModelFactory(IViewModelKeyProvider keyProvider, 
                                IViewModelResolver resolver, 
                                ILinkResolver linkResolver, 
                                IRichTextResolver richTextResolver,
                                IContextResolver contextResolver)
        {
            if (keyProvider == null) throw new ArgumentNullException("keyProvider");
            if (resolver == null) throw new ArgumentNullException("resolver");
            if (linkResolver == null) throw new ArgumentNullException("linkResolver");
            if (richTextResolver == null) throw new ArgumentNullException("richTextResolver");
            if (contextResolver == null) throw new ArgumentNullException("contextResolver");

            this._keyProvider = keyProvider;
            this._resolver = resolver;
            this._linkResolver = linkResolver;
            this._richtTextResolver = richTextResolver;
            this._contextResolver = contextResolver;

            LoadViewModels(new List<Assembly> { Assembly.GetEntryAssembly() });
        }


        public IViewModelResolver ModelResolver { get { return _resolver; } }
        public ILinkResolver LinkResolver { get { return _linkResolver; } }
        public IRichTextResolver RichTextResolver { get { return _richtTextResolver; } }

        public IContextResolver ContextResolver { get { return _contextResolver; } }

        /// <summary>
        /// Loads View Model Types from an Assembly. Use minimally due to reflection overhead.
        /// </summary>
        /// <param name="assembly"></param>
        public void LoadViewModels(IEnumerable<Assembly> assemblies) //We assume we have a singleton of this instance, otherwise we incur a lot of overhead
        {
            foreach (var assembly in assemblies)
            {
                if (!loadedAssemblies.Contains(assembly))
                {
                    //Josh Einhorn - Performance Question: Should we incur the memory overhead of storing Assembly objects in the heap or allow same assembly to get processed multiple times?
                    loadedAssemblies.Add(assembly); 
                    IModelAttribute viewModelAttr;
                    foreach (var type in assembly.GetTypes())
                    {
                        viewModelAttr = _resolver.GetCustomAttribute<IModelAttribute>(type);
                        if (viewModelAttr != null && !viewModels.ContainsKey(viewModelAttr))
                        {
                            viewModels.Add(viewModelAttr, type);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loads View Model Types from an Assembly. Use minimally due to reflection overhead.
        /// </summary>
        /// <param name="assembly"></param>
        public void LoadViewModels() //We assume we have a singleton of this instance, otherwise we incur a lot of overhead
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                                        .Where(a => !a.GlobalAssemblyCache 
                                                        && !a.IsDynamic 
                                                        && !a.ReflectionOnly 
                                                        && !a.FullName.StartsWith("Tridion."));

            LoadViewModels(assemblies);
        }

        public Type FindViewModelByAttribute<T>(IModel data, Type[] typesToSearch = null) where T : IModelAttribute
        {
            //Anyway to speed this up? Better than just a straight up loop?
            typesToSearch = typesToSearch ?? viewModels.Where(x => x.Key is T).Select(x => x.Value).ToArray();
            foreach (var type in typesToSearch)
            {
                T modelAttr = _resolver.GetCustomAttribute<T>(type);
                if (modelAttr != null)
                {
                    //modelAttr.ViewModelFactory = this;
                    if (modelAttr.IsMatch(data, _keyProvider.GetViewModelKey(data)))
                    {
                        return type;
                    }
                }
            }
            throw new ViewModelTypeNotFoundException(data);
        }

        public void SetPropertyValue(object model, IModel data, IModelProperty property)
        {
            if (property == null) throw new ArgumentNullException("property");
            if (model != null && data != null && property.PropertyAttribute != null)
            {
                var propertyValue = GetPropertyValue(property,property.PropertyAttribute.GetPropertyValues(data, property, this));
                if (propertyValue != null)
                {
                    try
                    {
                        property.Set(model, propertyValue);
                    }
                    catch (Exception e)
                    {
                        if (e is TargetException || e is InvalidCastException)
                            throw new PropertyTypeMismatchException(property, property.PropertyAttribute, propertyValue);
                        else throw e;
                    }
                }
            }
        }

        public void SetPropertyValue(IViewModel model, IModelProperty property)
        {
            SetPropertyValue(model, model.ModelData, property);
        }

        public void SetPropertyValue<TModel, TProperty>(TModel model, Expression<Func<TModel, TProperty>> propertyLambda) where TModel : IViewModel
        {
            var property = _resolver.GetModelProperty(model, propertyLambda);
            SetPropertyValue(model, property);
        }

        public IViewModel BuildViewModel(IModel modelData, IContextModel contextModel = null)
        {
            return BuildViewModelByAttribute<IModelAttribute>(modelData, contextModel);
        }

        public IViewModel BuildViewModelByAttribute<T>(IModel modelData, IContextModel contextModel = null) where T : IModelAttribute
        {
            IViewModel result = null;
            Type type = FindViewModelByAttribute<T>(modelData);
            if (type != null)
            {
                result = BuildViewModel(type, modelData, contextModel);
            }
            return result;
        }
        public IViewModel BuildViewModel(Type type, IModel modelData, IContextModel contextModel = null)
        {
            IViewModel viewModel = null;
            viewModel = _resolver.ResolveModel(type, modelData);
            viewModel.ModelData = modelData;
            ProcessViewModel(viewModel, type, contextModel);
            return viewModel;
        }

       

        public T BuildViewModel<T>(IModel modelData) where T : IViewModel 
        {
            return (T)BuildViewModel(typeof(T), modelData);
        }

        public virtual object BuildMappedModel(IModel modelData, IModelMapping mapping)
        {
            var model = _resolver.ResolveInstance(mapping.ModelType);
            return BuildMappedModel(model, modelData, mapping);
        }

        public virtual T BuildMappedModel<T>(IModel modelData, IModelMapping mapping) //where T: class
        {
            T model = (T)_resolver.ResolveInstance(typeof(T));
            return BuildMappedModel<T>(model, modelData, mapping);
        }

        public virtual T BuildMappedModel<T>(T model, IModel modelData, IModelMapping mapping) //where T : class
        {
            foreach (var property in mapping.ModelProperties)
            {
                SetPropertyValue(model, modelData, property);
            }
            return model;
        }
        #region Private methods


        private void ProcessViewModel(IViewModel viewModel, Type type, IContextModel contextModel)
        {
            //PropertyInfo[] props = type.GetProperties();
            var props = _resolver.GetModelProperties(type);
            IPropertyAttribute propAttribute;
            object propertyValue = null;
            foreach (var prop in props)
            {
                propAttribute = prop.PropertyAttribute;//prop.GetCustomAttributes(typeof(FieldAttributeBase), true).FirstOrDefault() as FieldAttributeBase;
                if (propAttribute != null) // this property is an IPropertyAttribute
                {
                    IEnumerable values;
                    if(propAttribute is ILinkablePropertyAttribute)                        
                        values = ((ILinkablePropertyAttribute)propAttribute).GetPropertyValues(viewModel.ModelData, prop, this, contextModel); //delegate work to the Property Attribute object itself. Allows for custom attribute types to easily be added
                    else
                        values = propAttribute.GetPropertyValues(viewModel.ModelData, prop, this); //delegate work to the Property Attribute object itself. Allows for custom attribute types to easily be added

                    if (values != null)
                    {
                        try
                        {
                            propertyValue = GetPropertyValue(prop, values);
                            prop.Set(viewModel, propertyValue);
                        }
                        catch (Exception e)
                        {
                            if (e is TargetException || e is InvalidCastException)
                                throw new PropertyTypeMismatchException(prop, propAttribute, propertyValue);
                            else throw e;
                        }
                    }
                }
            }
        }
       
        private object GetPropertyValue(IModelProperty prop, IEnumerable values)
        {
            object result = null;
            if (prop.IsEnumerable)
            {
                result = values;
            }
            else if (prop.IsArray)
            {
                result = prop.ToArray(values);
            }
            else if (prop.IsCollection)
            {
                var tempValues = (IEnumerable)_resolver.ResolveInstance(prop.PropertyType);
                foreach (var val in values)
                {
                    prop.AddToCollection(tempValues, val);
                }
                result = tempValues;
            }
            else //it's a single value, just return the first one (should really only be one thing)
            {
                result = values.Cast<object>().FirstOrDefault();
            }
            return result;
        }
        private string[] GetViewModelKey(IModel model)
        {
            string key = _keyProvider.GetViewModelKey(model);
            return String.IsNullOrEmpty(key) ? null : new string[] { key };
        }
        #endregion





        
    }
}
