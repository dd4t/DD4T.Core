using DD4T.ContentModel.Factories;
using DD4T.Core.Contracts.DependencyInjection;
using DD4T.Core.DD4T.Utils.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD4T.Factories
{
    public class DependencyMappings : IDependencyMapper
    {
        public TypeDescriptionList TypeDescriptions()
        {
            var list = new TypeDescriptionList();
            list.Add(typeof(IFactoryCommonServices), typeof(FactoryCommonServices), LifeCycle.PerLifeTime);
            list.Add(typeof(IPageFactory), typeof(PageFactory), LifeCycle.PerLifeTime);
            list.Add(typeof(IComponentPresentationFactory), typeof(ComponentPresentationFactory), LifeCycle.PerLifeTime);
            list.Add(typeof(IComponentFactory), typeof(ComponentFactory), LifeCycle.PerLifeTime);
            list.Add(typeof(IBinaryFactory), typeof(BinaryFactory), LifeCycle.PerLifeTime);
            list.Add(typeof(ILinkFactory), typeof(LinkFactory), LifeCycle.PerLifeTime);

            return list;
        }
    }
}