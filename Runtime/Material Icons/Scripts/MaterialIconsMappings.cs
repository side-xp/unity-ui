/**
 * Sideways Experiments (c) 2023
 * https://sideways-experiments.com
 * Contact: dev@side-xp.com
 */

namespace SideXP.UI
{

    /// <summary>
    /// The object representation of a Material Icons mappings JSON file.
    /// </summary>
    [System.Serializable]
    public class MaterialIconsMappings
    {

#pragma warning disable IDE1006 // Naming Styles

        #region Subclasses

        [System.Serializable]
        public class Atlas
        {
            public int iconSize;
            public Icon[] icons = { };

            public override string ToString()
            {
                return $"Material Icons Atlas ({icons.Length} icons, {iconSize}px)";
            }
        }

        [System.Serializable]
        public class Icon
        {
            public int x;
            public int y;
            public string name;
            public string category;

            public override string ToString()
            {
                return $"Icon \"{name}\" ({category}), mapped at ({x};{y})";
            }
        }

        #endregion


        #region Fields

        public Atlas[] mappings = { };

        #endregion

#pragma warning restore IDE1006 // Naming Styles

    }

}