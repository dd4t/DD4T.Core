using DD4T.Core.Contracts.ViewModels;

namespace DD4T.ContentModel.Factories
{
    public interface IModelFactory
    {
        IViewModel GetModelFor(IModel domainModel);
    }
}
