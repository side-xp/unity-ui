using UnityEngine;

namespace SideXP.UI.EditorOnly
{

    /// <summary>
    /// Utility for parsing Material Icons' JSON mappings file.
    /// </summary>
    public static class MaterialIconsMappingsUtility
    {

        #region Public API

        /// <summary>
        /// Parses the given mappings file asset.
        /// </summary>
        /// <param name="mappingsJSONFileAsset">The mappings JSON file asset.</param>
        /// <returns>Returns the parsed data.</returns>
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