using System.Collections.Generic;

using UnityEngine;

namespace SideXP.UI.EditorOnly
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

        /// <summary>
        /// Locates an icon inside an atlas: its mapping entry and the size (in pixels) of the atlas cells.
        /// </summary>
        private struct IconLocation
        {
            public MaterialIconsMappings.Icon Icon;
            public int IconSize;
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

        /// <summary>
        /// Caches the icon textures extracted from the atlases, so a given icon is sliced only once instead of on every call.<br/>
        /// Keys are built by <see cref="MakeCacheKey(string, bool, bool)"/>.
        /// </summary>
        private Dictionary<string, Texture2D> _iconTexturesCache = new Dictionary<string, Texture2D>();

        /// <summary>
        /// Lookup of icons by name for the regular-size mappings, so we don't scan the whole atlas on each call. Built lazily by
        /// <see cref="GetIconsLookup(bool)"/>.
        /// </summary>
        private Dictionary<string, IconLocation> _iconsLookup = null;

        /// <inheritdoc cref="_iconsLookup"/>
        /// <remarks>For the doubled-size mappings.</remarks>
        private Dictionary<string, IconLocation> _iconsLookup2x = null;

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

            // Return the already-extracted texture if we have it. The cached texture may have been destroyed externally (e.g. on a
            // scene change): treat that as a cache miss and re-extract it below.
            string cacheKey = MakeCacheKey(name, white, x2);
            if (_iconTexturesCache.TryGetValue(cacheKey, out Texture2D cachedIcon) && cachedIcon != null)
                return cachedIcon;

            if (!GetIconsLookup(x2).TryGetValue(name, out IconLocation location))
                return null;

            AtlasTextures atlasTextures = white ? _whiteIconsAtlas : _blackIconsAtlas;
            Texture2D targetAtlasTexture = x2 ? atlasTextures.IconsAtlas2x : atlasTextures.IconsAtlas;

            int iconSize = location.IconSize;
            Color[] pixels = targetAtlasTexture.GetPixels(location.Icon.x, targetAtlasTexture.height - location.Icon.y - iconSize, iconSize, iconSize);

            Texture2D iconTexture = new Texture2D(iconSize, iconSize, TextureFormat.RGBA32, false);
            iconTexture.name = location.Icon.name;
            iconTexture.SetPixels(pixels);
            iconTexture.Apply();

            _iconTexturesCache[cacheKey] = iconTexture;
            return iconTexture;
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
        /// Gets the lookup of icons by name for the given size, building it lazily on first use.
        /// </summary>
        /// <param name="x2">Do you want the lookup for the doubled-size mappings?</param>
        /// <returns>Returns the icons lookup, where keys are (lowercase) icon names.</returns>
        private Dictionary<string, IconLocation> GetIconsLookup(bool x2)
        {
            if (x2)
            {
                if (_iconsLookup2x == null)
                    _iconsLookup2x = BuildIconsLookup(Mappings2x);
                return _iconsLookup2x;
            }

            if (_iconsLookup == null)
                _iconsLookup = BuildIconsLookup(Mappings);
            return _iconsLookup;
        }

        /// <summary>
        /// Builds a lookup of icons by name from the given mappings.
        /// </summary>
        /// <param name="mappings">The parsed mappings to index.</param>
        /// <returns>Returns the built lookup, where keys are icon names.</returns>
        private static Dictionary<string, IconLocation> BuildIconsLookup(MaterialIconsMappings mappings)
        {
            Dictionary<string, IconLocation> lookup = new Dictionary<string, IconLocation>();
            foreach (MaterialIconsMappings.Atlas atlas in mappings.mappings)
            {
                foreach (MaterialIconsMappings.Icon icon in atlas.icons)
                {
                    // Icon names are expected to be unique across atlases: keep the first occurrence if a duplicate appears.
                    if (!lookup.ContainsKey(icon.name))
                        lookup.Add(icon.name, new IconLocation { Icon = icon, IconSize = atlas.iconSize });
                }
            }
            return lookup;
        }

        /// <summary>
        /// Builds the key used to cache an extracted icon texture.
        /// </summary>
        /// <inheritdoc cref="GetIcon(string, bool, bool)"/>
        private static string MakeCacheKey(string name, bool white, bool x2)
        {
            return $"{name}|{(white ? 'w' : 'b')}|{(x2 ? '2' : '1')}";
        }

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