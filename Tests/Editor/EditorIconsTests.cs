using NUnit.Framework;

using UnityEngine;

using SideXP.UI.EditorOnly;

namespace SideXP.UI.Tests
{

    public class EditorIconsTests
    {

        private const string KnownIcon = "accessibility";

        [SetUp]
        public void RequireAsset()
        {
            Assert.IsNotNull(EditorIcons.MaterialIconsAsset, "The bundled 'Material Icons' asset could not be found in the project.");
        }

        #region GetIcon

        [Test]
        public void GetIcon_KnownIcon_ReturnsTexture()
        {
            Assert.IsNotNull(EditorIcons.GetIcon(KnownIcon));
        }

        [Test]
        public void GetIcon_DefaultsToWhiteVariant()
        {
            // EditorIcons is drawn over dark editor skins, so its default (black:false) maps to the asset's white icon.
            Texture2D viaEditorIcons = EditorIcons.GetIcon(KnownIcon);
            Texture2D whiteFromAsset = EditorIcons.MaterialIconsAsset.GetIcon(KnownIcon, white: true);
            Assert.AreSame(whiteFromAsset, viaEditorIcons);
        }

        [Test]
        public void GetIcon_BlackTrue_MapsToBlackVariant()
        {
            Texture2D viaEditorIcons = EditorIcons.GetIcon(KnownIcon, black: true);
            Texture2D blackFromAsset = EditorIcons.MaterialIconsAsset.GetIcon(KnownIcon, white: false);
            Assert.AreSame(blackFromAsset, viaEditorIcons);
        }

        #endregion


        #region IconContent

        [Test]
        public void IconContent_TextAndTooltip_ArePopulated()
        {
            GUIContent content = EditorIcons.IconContent(KnownIcon, "Label", "Tooltip");
            Assert.AreEqual("Label", content.text);
            Assert.AreEqual("Tooltip", content.tooltip);
            Assert.IsNotNull(content.image);
        }

        [Test]
        public void IconContent_UnknownIcon_HasNullImage()
        {
            GUIContent content = EditorIcons.IconContent("__not_a_real_icon__", "Label", "Tooltip");
            Assert.AreEqual("Label", content.text);
            Assert.IsNull(content.image);
        }

        #endregion

    }

}
