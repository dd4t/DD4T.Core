using DD4T.ContentModel;
using DD4T.ContentModel.Contracts.Logging;
using DD4T.ContentModel.Factories;
using DD4T.Core.Contracts.ViewModels;
using DD4T.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD4T.Factories
{
    public class ModelFactory : IModelFactory
    {
        private readonly ILogger _logger;

        public ModelFactory(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");

            _logger = logger;

            _logger.Debug("About to load all models defined in all assemblies loaded into the appdmain.");

            ViewModelDefaults.Factory.LoadViewModels();
        }

        public IViewModel GetModelFor(IModel domainModel)
        {
            var viewModel = ViewModelDefaults.Factory.BuildViewModel(domainModel);
            return viewModel;
        }

    }
}
