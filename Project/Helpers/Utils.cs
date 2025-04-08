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



/*
AI-----------------------------------------
PROMPT_____________________________
JSON Export Feature (New Task)
• Add a new button to export the data to JSON. There should be two modes:
o Unfiltered export (exports the entire data)
o Filtered export (exports only the currently filtered rows)
• Also add the ability to select specific columns for export:
o If no column is selected, export all columns.
o If certain columns (e.g., 1st and 4th) are selected, export only those columns.
o The selected columns should visually change color to indicate selection.
o The exported JSON should contain only the selected column data.
4. Utility Class for JSON Export
• Create a new C# class file named Utils.cs.
• Inside it, implement a generic method that can export any class to JSON.
• The method should work with any model class.
• This class must be implemented as a singleton, so it can be accessed from anywhere in the
project.
5. Folder Structure Reminder (MVP)
Since your project follows the MVP structure in a Razor Pages application:
• Place the ClassInformationTable and related data models in the Models folder.
• Place the Utils.cs class in a separate folder called Helpers or Utilities.
• Place pagination logic, filtering logic, and UI-related code in the appropriate Pages folder.

the given task is like that



RESPONSE_____________________________
To implement the JSON export feature as described, we will follow a few steps. The task requires the creation of a utility class (Utils.cs), setting up logic for filtered/unfiltered exports, handling column selection, and organizing the file structure. Here's how we will break down the task:

Utility Class for JSON Export (Utils.cs)
First, we will create a utility class that will handle the JSON export logic.

Create a new folder called Helpers (or Utilities) in your project.

Add a new class file named Utils.cs inside the Helpers folder.

Implement a generic method that can export any model class to JSON.

Code for Utils.cs (in Helpers folder):
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

        // Generic method to export any list of data to JSON
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

            // Serialize the filtered data to JSON
            var json = JsonSerializer.Serialize(filteredData, new JsonSerializerOptions { WriteIndented = true });

            // Ensure the directory exists
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Write the JSON data to the specified file
            File.WriteAllText(filePath, json);
        }
    }
}

Explanation:
Singleton Pattern: The class is implemented as a singleton to be easily accessible throughout the project.

ExportToJson Method: A generic method that can export any data list (List<T>) to a JSON file.

It checks if selectedColumns is provided; if not, it exports all columns.

If selectedColumns is provided, it only includes those columns in the export.

The selectedColumns parameter is a list of column names to export (e.g., ["Id", "ClassName"]).

The data is serialized to JSON, and it ensures the directory exists before saving.
*/
