/**
 * Sideways Experiments (c) 2023
 * https://sideways-experiments.com
 * Contact: dev@side-xp.com
 */

using UnityEngine;
using UnityEditor;

using SideXP.Core.EditorOnly;

namespace SideXP.UI.EditorOnly
{

    /// <summary>
    /// Utility for getting icon resources.
    /// </summary>
    public static class EditorIcons
    {

        #region Fields

        private static MaterialIcons s_materialIconsAsset = null;

        #endregion


        #region Public API
        
        /// <summary>
        /// Gets the <see cref="MaterialIcons"/> asset used by this utility.
        /// </summary>
        public static MaterialIcons MaterialIconsAsset
        {
            get
            {
                if (s_materialIconsAsset == null)
                {
                    s_materialIconsAsset = ObjectUtility.FindAsset<MaterialIcons>();
                }
                return s_materialIconsAsset;
            }
        }

        /// <summary>
        /// Gets the texture of a named icon alone.
        /// </summary>
        /// <param name="name">The name of the icon you want to get, as defined from the Material Icons list
        /// (<see href="https://fonts.google.com/icons?icon.set=Material+Icons"/>).</param>
        /// <param name="black">Do you want to get the black version of the icon?</param>
        /// <param name="x2">Do you want the doubled size of the icon?</param>
        /// <returns>Returns the found icon.</returns>
        public static Texture2D GetIcon(string name, bool black = false, bool x2 = false)
        {
            if (MaterialIconsAsset != null)
            {
                return MaterialIconsAsset.GetIcon(name, !black, x2);
            }
            return null;
        }

        /// <summary>
        /// Creates a <see cref="GUIContent"/>, using the named icon as content's icon.
        /// </summary>
        /// <param name="iconName"><inheritdoc cref="GetIcon(string, bool, bool)" path="/param[@name='name']"/></param>
        /// <param name="text">The text of the output <see cref="GUIContent"/>.</param>
        /// <param name="tooltip">The tooltip of the output <see cref="GUIContent"/>.</param>
        /// <returns>Returns the generated <see cref="GUIContent"/>.</returns>
        /// <inheritdoc cref="GetIcon(string, bool, bool)"/>
        public static GUIContent IconContent(string iconName, string text, string tooltip, bool black = false, bool x2 = false)
        {
            return new GUIContent(text, GetIcon(iconName, black, x2), tooltip);
        }

        /// <inheritdoc cref="IconContent(string, string, string, bool, bool)"/>
        public static GUIContent IconContent(string iconName, string tooltip, bool black = false, bool x2 = false)
        {
            return new GUIContent(GetIcon(iconName, black, x2), tooltip);
        }

        /// <inheritdoc cref="IconContent(string, string, string, bool, bool)"/>
        public static GUIContent IconContent(string iconName, bool black = false, bool x2 = false)
        {
            return new GUIContent(GetIcon(iconName, black, x2));
        }

        /// <summary>
        /// Draws a button on GUI with the named icon in the label.
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the button.</param>
        /// <param name="label">Text and tooltip for this button.</param>
        /// <returns>Returns true if the user clicks the button.</returns>
        /// <inheritdoc cref="IconContent(string, string, string, bool, bool)"/>
        public static bool IconButton(Rect position, string iconName, GUIContent label)
        {
            if (label == null || label == GUIContent.none)
                label = new GUIContent();

            // Add a space to make sure the icon doesn't appear too close from the label
            if (!string.IsNullOrWhiteSpace(label.text) && !label.text.StartsWith(' '))
                label.text = ' ' + label.text;

            label.image = GetIcon(iconName);
            return GUI.Button(position, label, MoreEditorGUI.IconButtonStyle);
        }

        /// <param name="label">Text for this button</param>
        /// <param name="tooltip">Tooltip for this button.</param>
        /// <inheritdoc cref="IconButton(Rect, string, GUIContent)"/>
        public static bool IconButton(Rect position, string iconName, string label, string tooltip)
        {
            return IconButton(position, iconName, new GUIContent(label, tooltip));
        }

        /// <inheritdoc cref="IconButton(Rect, string, string, string)"/>
        public static bool IconButton(Rect position, string iconName, string label)
        {
            return IconButton(position, iconName, new GUIContent(label));
        }

        /// <inheritdoc cref="IconButton(Rect, string, GUIContent)"/>
        public static bool IconButton(Rect position, string iconName)
        {
            return IconButton(position, iconName, GUIContent.none);
        }

        /// <param name="options">Options for the rectangle on the screen to use for the button.</param>
        /// <remarks>Uses layout GUI.</remarks>
        /// <inheritdoc cref="IconButton(Rect, string, GUIContent)"/>
        public static bool IconButton(string iconName, GUIContent label, params GUILayoutOption[] options)
        {
            Rect position = EditorGUILayout.GetControlRect(false, options);
            return IconButton(position, iconName, label);
        }

        /// <inheritdoc cref="IconButton(Rect, string, string, string)"/>
        /// <inheritdoc cref="IconButton(string, GUIContent, GUILayoutOption[])"/>
        public static bool IconButton(string iconName, string label, string tooltip, params GUILayoutOption[] options)
        {
            return IconButton(iconName, new GUIContent(label, tooltip), options);
        }

        /// <inheritdoc cref="IconButton(string, string, string, GUILayoutOption[])"/>
        public static bool IconButton(string iconName, string label, params GUILayoutOption[] options)
        {
            return IconButton(iconName, new GUIContent(label), options);
        }

        /// <inheritdoc cref="IconButton(string, string, string, GUILayoutOption[])"/>
        public static bool IconButton(string iconName, params GUILayoutOption[] options)
        {
            return IconButton(iconName, GUIContent.none, options);
        }

        #endregion

    }

}