using DD4T.ContentModel.Contracts.Resolvers;
using DD4T.Core.Contracts.DependencyInjection;
using DD4T.Core.Contracts.ViewModels;
using DD4T.Core.DD4T.Utils.Models;
using DD4T.ViewModels.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD4T.ViewModels
{
    public class DependencyMappings : IDependencyMapper
    {
        public TypeDescriptionList TypeDescriptions()
        {
            var list = new TypeDescriptionList();
            list.Add(typeof(IContextResolver), typeof(DefaultContextResolver));
            list.Add(typeof(IReflectionHelper), typeof(ReflectionOptimizer));
            list.Add(typeof(IViewModelResolver), typeof(DefaultViewModelResolver));
            list.Add(typeof(IViewModelFactory), typeof(ViewModelFactory));
            list.Add(typeof(IViewModelKeyProvider), typeof(WebConfigViewModelKeyProvider));

            return list;
        }
    }
}