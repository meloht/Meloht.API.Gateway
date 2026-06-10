using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.Utilities
{
    public interface IRandomFactory
    {
        /// <summary>
        /// Create a instance of random class.
        /// </summary>
        Random CreateRandomInstance();
    }
}
