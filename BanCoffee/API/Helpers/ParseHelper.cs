using System;
using System.Collections.Generic;
using System.Text.Json;

namespace API.Helpers
{
    public static class ParseHelper
    {
        public static string GetString(Dictionary<string, object> formData, string key, string defaultValue = "")
        {
            if (formData != null && formData.TryGetValue(key, out var value) && value != null)
            {
                if (value is JsonElement element)
                {
                    return element.ValueKind != JsonValueKind.Null ? element.ToString() : defaultValue;
                }
                return Convert.ToString(value) ?? defaultValue;
            }
            return defaultValue;
        }

        public static int GetInt(Dictionary<string, object> formData, string key, int defaultValue = 0)
        {
            if (formData != null && formData.TryGetValue(key, out var value) && value != null)
            {
                if (value is JsonElement element)
                {
                    if (element.ValueKind == JsonValueKind.Number && element.TryGetInt32(out int i)) return i;
                    if (element.ValueKind == JsonValueKind.String && int.TryParse(element.GetString(), out int j)) return j;
                    return defaultValue;
                }
                try { return Convert.ToInt32(value); } catch { return defaultValue; }
            }
            return defaultValue;
        }

        public static DateTime? GetDateTime(Dictionary<string, object> formData, string key)
        {
            if (formData != null && formData.TryGetValue(key, out var value) && value != null)
            {
                if (value is JsonElement element)
                {
                    if (element.ValueKind == JsonValueKind.String && DateTime.TryParse(element.GetString(), out DateTime dt)) return dt;
                    return null;
                }
                try { return Convert.ToDateTime(value); } catch { return null; }
            }
            return null;
        }

        public static double GetDouble(Dictionary<string, object> formData, string key, double defaultValue = 0)
        {
            if (formData != null && formData.TryGetValue(key, out var value) && value != null)
            {
                if (value is JsonElement element)
                {
                    if (element.ValueKind == JsonValueKind.Number && element.TryGetDouble(out double d)) return d;
                    if (element.ValueKind == JsonValueKind.String && double.TryParse(element.GetString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double s)) return s;
                    return defaultValue;
                }
                try { return Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture); } catch { return defaultValue; }
            }
            return defaultValue;
        }
    }
}
