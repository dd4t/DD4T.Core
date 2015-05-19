using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DD4T.ViewModels.Contracts;
using System.ComponentModel;
using DD4T.ContentModel;

namespace DD4T.ViewModels.Base
{

    /// <summary>
    /// Base class for all View Models
    /// </summary>
    public abstract class ViewModelBase : IViewModel
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IModel ModelData
        {
            get;
            set;
        }
    }
}
