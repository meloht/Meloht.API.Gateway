using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.API.Gateway.Utilities
{
    internal sealed class RandomFactory : IRandomFactory
    {
        /// <inheritdoc/>
        public Random CreateRandomInstance()
        {
            return Random.Shared;
        }
    }

}
