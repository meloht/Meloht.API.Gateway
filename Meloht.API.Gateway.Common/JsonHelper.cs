using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Meloht.API.Gateway.Common
{
    public static class JsonHelper
    {
        public static ValueTask<T?> ReadJsonAsync<T>(Stream stream)
           => JsonSerializer.DeserializeAsync<T>(stream);
    }
}
