using DD4T.Core.Contracts.ViewModels;
using DD4T.Core.Contracts.ViewModels.Binding;

namespace DD4T.ContentModel.Factories
{
    public interface IModelFactory
    {
        IViewModel GetModelFor(IModel domainModel);
        T GetModelFor<T>(IModel modelData) where T : IViewModel;
        object GetMappedModelFor(IModel modelData, IModelMapping modelMapping);
        T GetMappedModelFor<T>(IModel modelData, IModelMapping modelMapping);
        IViewModel GetModelByAttribute<T>(IModel modelData) where T : IModelAttribute;
    }
}
