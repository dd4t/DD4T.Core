using DD4T.Core.DD4T.Utils.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD4T.Core.Contracts.DependencyInjection
{
    /// <summary>
    /// Used by DD4T.DI Packages to register default type's
    /// </summary>
    public interface IDependencyMapper
    {
        TypeDescriptionList TypeDescriptions();
    }
}