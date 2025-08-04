using System.IO;

using UnityEngine;

using SideXP.Core;

namespace SideXP.UI
{

    /// <summary>
    /// Utility for parsing Material Icons' JSON mappings file.
    /// </summary>
    public static class MaterialIconsMappingsUtility
    {

        #region Public API

        /// <summary>
        /// Parses the given mappings file.
        /// </summary>
        /// <inheritdoc cref="ParseContent(string)"/>
        /// <param name="mappingsJSONFilePath">The path to the mappings JSON file.</param>
        public static MaterialIconsMappings Parse(string mappingsJSONFilePath)
        {
            mappingsJSONFilePath = mappingsJSONFilePath.ToAbsolutePath();

            if (File.Exists(mappingsJSONFilePath))
            {
                return ParseContent(File.ReadAllText(mappingsJSONFilePath));
            }
            else
            {
                Debug.LogWarning($"No mappings JSON file found at {mappingsJSONFilePath}");
                return null;
            }
        }

        /// <inheritdoc cref="Parse(string)"/>
        /// <param name="mappingsJSONFileAsset">The mappings JSON file asset.</param>
        public static MaterialIconsMappings Parse(TextAsset mappingsJSONFileAsset)
        {
            return ParseContent(mappingsJSONFileAsset.text);
        }

        #endregion


        #region Private API

        /// <summary>
        /// Parses the given mappings.
        /// </summary>
        /// <param name="mappingsJSONAsString">The mappings to parse, as JSON string.</param>
        /// <returns>Returns the parsed data.</returns>
        private static MaterialIconsMappings ParseContent(string mappingsJSONAsString)
        {
            MaterialIconsMappings mappings = new MaterialIconsMappings();
            JsonUtility.FromJsonOverwrite(mappingsJSONAsString, mappings);
            return mappings;
        }

        #endregion

    }

}