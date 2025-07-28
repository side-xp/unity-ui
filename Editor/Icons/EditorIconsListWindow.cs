/**
 * Sideways Experiments (c) 2023
 * https://sideways-experiments.com
 * Contact: dev@side-xp.com
 */

using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using SideXP.Core;
using SideXP.Core.EditorOnly;

namespace SideXP.UI.EditorOnly
{

    /// <summary>
    /// Displays the list of all available icons.
    /// </summary>
    public class EditorIconsListWindow : EditorWindow
    {

        #region Subclasses

        /// <summary>
        /// Groups informations about a displayed icon.
        /// </summary>
        private struct IconInfo
        {
            public string Category;
            public string Name;
            public Texture2D Icon;
        }

        #endregion


        #region Fields

        private const float PaginationMainFieldWidthRatio = .5f;
        private const float PaginationButtonWidthRatio = .6f;
        private const float PaginationMainFieldSeparatorWidth = 6f;

        private const string WindowTitle = "Editor Icons List";
        private const string MenuItem = EditorConstants.EditorWindowMenu + "/" + WindowTitle;

        private const int IconsCountPerPage = 20;
        private const string AllCategoriesLabel = "All";

        private static string[] s_categoriesLabels = null;
        private static string[] s_nicifiedCategoriesLabels = null;

        [SerializeField]
        [Tooltip("Filter icons by names.")]
        private string _searchString = string.Empty;

        [SerializeField]
        [Tooltip("Filter icons by catagories.")]
        private int _selectedCategoryIndex = 0;

        [SerializeField]
        [Tooltip("The current pagination value.")]
        private Pagination _pagination = new Pagination();

        [SerializeField]
        private Vector2 _scrollPosition = Vector2.zero;

        /// <summary>
        /// The list of icons that match with the current filters.
        /// </summary>
        private IconInfo[] _displayedIcons = null;

        #endregion


        #region Public API

        /// <summary>
        /// Opens this editor window.
        /// </summary>
        [MenuItem(MenuItem)]
        public static EditorIconsListWindow Open()
        {
            EditorIconsListWindow window = GetWindow<EditorIconsListWindow>(false, WindowTitle, true);
            window.Show();
            return window;
        }

        #endregion


        #region UI

        /// <summary>
        /// Draws this window GUI on screen.
        /// </summary>
        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            _selectedCategoryIndex = EditorGUILayout.Popup(EditorIcons.IconContent("search", "Category", ""), _selectedCategoryIndex, NicifiedCategoriesLabels);
            _searchString = EditorGUILayout.TextField(EditorIcons.IconContent("category", "Search", ""), _searchString);
            if (EditorGUI.EndChangeCheck())
                ReloadIcons();

            PaginationField(ref _pagination);

            EditorGUILayout.Space();
            MoreEditorGUI.HorizontalSeparator();
            EditorGUILayout.Space();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            {
                foreach (IconInfo i in _pagination.Paginate(DisplayedIcons))
                {
                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            Rect rect = EditorGUILayout.GetControlRect(GUILayout.Width(i.Icon.width), GUILayout.Height(i.Icon.height));
                            EditorGUI.DrawTextureTransparent(rect, i.Icon);

                            using (new EditorGUILayout.VerticalScope())
                            {
                                EditorGUILayout.LabelField(i.Name, EditorStyles.boldLabel);
                                EditorGUILayout.LabelField(i.Category, EditorStyles.largeLabel.FontStyle(FontStyle.Italic).FontSize(10));
                            }
                        }
                    }
                }
            }
            EditorGUILayout.EndScrollView();
        }

        #endregion


        #region Private API

        /// <inheritdoc cref="s_categoriesLabels"/>
        private static string[] CategoriesLabels
        {
            get
            {
                if (s_categoriesLabels == null)
                {
                    List<string> categoriesLabelsList = new List<string>();

                    categoriesLabelsList.Add(AllCategoriesLabel);
                    categoriesLabelsList.AddRange(EditorIcons.MaterialIconsAsset.IconCategories);

                    s_categoriesLabels = categoriesLabelsList.ToArray();
                }
                return s_categoriesLabels;
            }
        }

        /// <inheritdoc cref="s_nicifiedCategoriesLabels"/>
        private static string[] NicifiedCategoriesLabels
        {
            get
            {
                if (s_nicifiedCategoriesLabels == null)
                {
                    List<string> nicifiedCategoriesLabelsList = new List<string>();
                    foreach (string cat in CategoriesLabels)
                        nicifiedCategoriesLabelsList.Add(ObjectNames.NicifyVariableName(cat));
                    s_nicifiedCategoriesLabels = nicifiedCategoriesLabelsList.ToArray();
                }
                return s_nicifiedCategoriesLabels;
            }
        }

        /// <inheritdoc cref="_displayedIcons"/>
        private IconInfo[] DisplayedIcons
        {
            get
            {
                if (_displayedIcons == null)
                    ReloadIcons();
                return _displayedIcons;
            }
        }

        /// <summary>
        /// Reloads the list of icons to display, based on the active filters and pagination value.
        /// </summary>
        private void ReloadIcons()
        {
            List<string> iconNames = new List<string>();
            if (_selectedCategoryIndex > 0)
                iconNames.AddRange(EditorIcons.MaterialIconsAsset.GetIconNamesInCategory(CategoriesLabels[_selectedCategoryIndex]));
            else
                iconNames.AddRange(EditorIcons.MaterialIconsAsset.IconNames);

            if (!string.IsNullOrEmpty(_searchString))
            {
                string search = _searchString.ToLower().Trim();
                iconNames.RemoveAll(name => !name.ToLower().Contains(search));
            }

            List<IconInfo> iconInfos = new List<IconInfo>();
            foreach (string iconName in iconNames)
            {
                iconInfos.Add(new IconInfo
                {
                    Name = iconName,
                    Category = EditorIcons.MaterialIconsAsset.GetIconCategoryName(iconName),
                    Icon = EditorIcons.MaterialIconsAsset.GetIcon(iconName, true, true)
                });
            }

            _displayedIcons = iconInfos.ToArray();
            _pagination = new Pagination(0, IconsCountPerPage, _displayedIcons.Length);
        }

        /// <remarks>This is a copy of <see cref="MoreEditorGUI.PaginationField(Rect, GUIContent, ref Pagination)"/>, using actual Material
        /// Icons for the page controls.</remarks>
        /// <inheritdoc cref="PaginationField(Rect, ref Pagination)"/>
        public static void PaginationField(ref Pagination pagination)
        {
            PaginationField(EditorGUILayout.GetControlRect(false), ref pagination);
        }

        /// <inheritdoc cref="MoreEditorGUI.PaginationField(Rect, GUIContent, ref Pagination)"/>
        public static void PaginationField(Rect position, ref Pagination pagination)
        {
            float mainFieldWidth = position.width * PaginationMainFieldWidthRatio;
            float controlsAvailableWidth = (position.width - mainFieldWidth - MoreGUI.HMargin * 2) / 2;
            float largeButtonWidth = controlsAvailableWidth * PaginationButtonWidthRatio;
            float smallButtonWidth = controlsAvailableWidth - largeButtonWidth - MoreGUI.HMargin;

            Rect rect = new Rect(position);

            // Draw "previous" controls
            using (new EnabledScope(pagination.Page > 0))
            {
                rect.width = smallButtonWidth;
                if (EditorIcons.IconButton(rect, "first_page"))
                    pagination.Page = 0;

                rect.x += rect.width + MoreGUI.HMargin;
                rect.width = largeButtonWidth;
                if (EditorIcons.IconButton(rect, "chevron_left"))
                    pagination.Page--;
            }

            // Draw page index
            int indexPlus1 = pagination.Page + 1;
            rect.x += rect.width + MoreGUI.HMargin;
            rect.width = mainFieldWidth / 2;
            indexPlus1 = EditorGUI.IntField(rect, indexPlus1, EditorStyles.textField.TextAlignment(TextAnchor.MiddleCenter));
            pagination.Page = indexPlus1 - 1;

            // Draw separator
            rect.x += rect.width;
            rect.width = PaginationMainFieldSeparatorWidth;
            EditorGUI.LabelField(rect, "/");

            // Draw pages count
            rect.x += rect.width + MoreGUI.HMargin;
            rect.width = mainFieldWidth / 2 - MoreGUI.HMargin - PaginationMainFieldSeparatorWidth;
            EditorGUI.LabelField(rect, pagination.PagesCount.ToString(), EditorStyles.label.TextAlignment(TextAnchor.MiddleCenter));

            rect.x += rect.width + MoreGUI.HMargin;
            // Draw "next" controls
            using (new EnabledScope(pagination.Page < pagination.PagesCount - 1))
            {
                rect.width = largeButtonWidth;
                if (EditorIcons.IconButton(rect, "chevron_right"))
                    pagination.Page++;

                rect.x += rect.width + MoreGUI.HMargin;
                rect.width = smallButtonWidth;
                if (EditorIcons.IconButton(rect, "last_page"))
                    pagination.Page = pagination.PagesCount;
            }
        }

        #endregion

    }

}