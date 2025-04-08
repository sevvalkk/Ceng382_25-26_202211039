using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Project.Helpers
{
    public class Utils
    {
        private static readonly Lazy<Utils> _instance = new Lazy<Utils>(() => new Utils());

        public static Utils Instance => _instance.Value;

        public void ExportToJson<T>(List<T> data, string filePath, List<string> selectedColumns = null)
        {
            var filteredData = new List<Dictionary<string, object>>();

            foreach (var item in data)
            {
                var dict = new Dictionary<string, object>();

                // If no specific columns are selected, export all properties
                if (selectedColumns == null || selectedColumns.Count == 0)
                {
                    foreach (var property in item.GetType().GetProperties())
                    {
                        dict[property.Name] = property.GetValue(item);
                    }
                }
                else
                {
                    // Export only selected columns
                    foreach (var property in item.GetType().GetProperties())
                    {
                        if (selectedColumns.Contains(property.Name))
                        {
                            dict[property.Name] = property.GetValue(item);
                        }
                    }
                }

                filteredData.Add(dict);
            }

            var json = JsonSerializer.Serialize(filteredData, new JsonSerializerOptions { WriteIndented = true });

            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, json);
        }
    }
}
