using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Checklist
{
    public static class JsonExtensions
    {
        public static JsonElement? GetSafe(this JsonElement element, string propertyName)
            => element.TryGetProperty(propertyName, out var value) ? value : (JsonElement?)null;

        public static JsonElement? GetFirst(this JsonElement element)
            => element.ValueKind == JsonValueKind.Array && element.GetArrayLength() > 0
                ? element[0]
                : (JsonElement?)null;
    }
}