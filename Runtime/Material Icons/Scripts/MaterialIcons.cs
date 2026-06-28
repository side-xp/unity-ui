using System.Collections.Generic;

using UnityEngine;

namespace SideXP.UI
{

    /// <summary>
    /// Contains the references to the Material Icons atlases and mappings, so you can get these icons easily.
    /// </summary>
    public class MaterialIcons : ScriptableObject
    {

        #region Enums & Subclasses

        [System.Serializable]
        private class AtlasTextures
        {
            public Texture2D IconsAtlas = null;
            public Texture2D IconsAtlas2x = null;
        }

        #endregion


        #region Fields

        /// <summary>
        /// External URL to the Material Icons browser from Google Fonts.
        /// </summary>
        public const string MaterialIconsURL = "https://fonts.google.com/icons?icon.set=Material+Icons";

        [SerializeField]
        [Tooltip("The Material Icons atlases that contain the black version of the icons.")]
        private AtlasTextures _blackIconsAtlas = null;

        [SerializeField]
        [Tooltip("The Material Icons atlases that contain the white version of the icons.")]
        private AtlasTextures _whiteIconsAtlas = null;

        [SerializeField]
        [Tooltip("The mappings json file for regular size icons.")]
        private TextAsset _mappings = null;

        [SerializeField]
        [Tooltip("The mappings json file for doubled size icons.")]
        private TextAsset _mappings2x = null;

        [SerializeField]
        [Tooltip("The size (in pixels) of the icons at regular size.")]
        private int _iconSize = 24;

        [SerializeField]
        [Tooltip("The size (in pixels) of the icons at doubled size.")]
        private int _iconSize2x = 48;

        private MaterialIconsMappings _parsedMappings = null;
        private MaterialIconsMappings _parsedMappings2x = null;

        /// <summary>
        /// Informations about the icons: keys are categories, values are icon names.
        /// </summary>
        private Dictionary<string, List<string>> _categories = null;

        /// <summary>
        /// Caches all the icons names.
        /// </summary>
        private string[] _iconNames = null;

        #endregion


        #region Public API

        /// <inheritdoc cref="_iconSize"/>
        public int IconSize => _iconSize;

        /// <inheritdoc cref="_iconSize2x"/>
        public int IconSize2x => _iconSize2x;

        /// <summary>
        /// Gets the list of all icon categories.
        /// </summary>
        public string[] IconCategories
        {
            get
            {
                if (_categories == null)
                    ReloadIconInfos();
                return new List<string>(_categories.Keys).ToArray();
            }
        }

        /// <summary>
        /// Gets all the icons names.
        /// </summary>
        public string[] IconNames
        {
            get
            {
                if (_iconNames == null)
                    ReloadIconInfos();
                return _iconNames;
            }
        }

        /// <summary>
        /// Gets a Material Icon by its name.<br/>
        /// You can browse the icons at https://fonts.google.com/icons?icon.set=Material+Icons
        /// </summary>
        /// <param name="name">The name of the icon you want to get.</param>
        /// <param name="white">Do you want to get the white version of the icon?</param>
        /// <param name="x2">Do you want the doubled size of the icon?</param>
        /// <returns>Returns the found icon.</returns>
        public Texture2D GetIcon(string name, bool white = false, bool x2 = false)
        {
            name = name.ToLower();
            MaterialIconsMappings mappings = x2 ? Mappings2x : Mappings;
            foreach (MaterialIconsMappings.Atlas atlas in mappings.mappings)
            {
                foreach (MaterialIconsMappings.Icon icon in atlas.icons)
                {
                    if (icon.name == name)
                    {
                        AtlasTextures atlasTextures = white ? _whiteIconsAtlas : _blackIconsAtlas;
                        Texture2D targetAtlasTexture = (x2 ? atlasTextures.IconsAtlas2x : atlasTextures.IconsAtlas);
                        Color[] pixels = targetAtlasTexture.GetPixels(icon.x, targetAtlasTexture.height - icon.y - atlas.iconSize, atlas.iconSize, atlas.iconSize);

                        Texture2D iconTexture = new Texture2D(atlas.iconSize, atlas.iconSize, TextureFormat.RGBA32, false);
                        iconTexture.name = icon.name;
                        iconTexture.SetPixels(pixels);
                        iconTexture.Apply();

                        return iconTexture;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets all the icon names for the named category.
        /// </summary>
        /// <param name="categoryName">The name of the category of which you want to get the icon names.</param>
        /// <returns>Returns the found icons names.</returns>
        public string[] GetIconNamesInCategory(string categoryName)
        {
            if (_categories == null)
                ReloadIconInfos();

            return _categories.TryGetValue(categoryName.ToLower(), out List<string> iconNames)
                ? iconNames.ToArray()
                : (new string[0]);
        }

        /// <summary>
        /// Gets the named icon's category.
        /// </summary>
        /// <param name="iconName">The name of the icon of which you want to get the category name.</param>
        /// <returns>Returns the found category name.</returns>
        public string GetIconCategoryName(string iconName)
        {
            iconName = iconName.ToLower();
            foreach (KeyValuePair<string, List<string>> categoryInfo in _categories)
            {
                if (categoryInfo.Value.Contains(iconName))
                    return categoryInfo.Key;
            }
            return null;
        }

        #endregion


        #region Private API

        /// <summary>
        /// Gets the Material Icons mapping data, for regular size icons.
        /// </summary>
        private MaterialIconsMappings Mappings
        {
            get
            {
                if (_parsedMappings == null)
                {
                    _parsedMappings = MaterialIconsMappingsUtility.Parse(_mappings);
                }
                return _parsedMappings;
            }
        }

        /// <summary>
        /// Gets the Material Icons mapping data, for doubled size icons.
        /// </summary>
        private MaterialIconsMappings Mappings2x
        {
            get
            {
                if (_parsedMappings2x == null)
                {
                    _parsedMappings2x = MaterialIconsMappingsUtility.Parse(_mappings2x);
                }
                return _parsedMappings2x;
            }
        }

        /// <summary>
        /// Refresh the cached informations about the icons.
        /// </summary>
        private void ReloadIconInfos()
        {
            List<string> iconNamesList = new List<string>();
            if (_categories != null)
                _categories.Clear();
            else
                _categories = new Dictionary<string, List<string>>();

            // For each atlas
            foreach (MaterialIconsMappings.Atlas atlas in Mappings.mappings)
            {
                // For each icon
                foreach (MaterialIconsMappings.Icon icon in atlas.icons)
                {
                    if (!_categories.TryGetValue(icon.category, out List<string> names))
                    {
                        names = new List<string>();
                        _categories.Add(icon.category, names);
                    }

                    names.Add(icon.name);
                    iconNamesList.Add(icon.name);
                }
            }

            _iconNames = iconNamesList.ToArray();
        }

        #endregion

    }

}