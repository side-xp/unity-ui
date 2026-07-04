using NUnit.Framework;

using UnityEngine;

using SideXP.UI.EditorOnly;

namespace SideXP.UI.Tests
{

    public class MaterialIconsTests
    {

        // Names the package itself depends on (the icon browser window draws these), so they are guaranteed to
        // exist in the bundled data.
        private const string KnownIcon = "accessibility";
        private const string KnownCategory = "action";

        private MaterialIcons _asset;

        [OneTimeSetUp]
        public void LoadAsset()
        {
            _asset = EditorIcons.MaterialIconsAsset;
        }

        [SetUp]
        public void RequireAsset()
        {
            Assert.IsNotNull(_asset, "The bundled 'Material Icons' asset could not be found in the project.");
        }

        #region Metadata

        [Test]
        public void IconNames_IsNotEmpty()
        {
            Assert.Greater(_asset.IconNames.Length, 0);
        }

        [Test]
        public void IconNames_ContainsKnownIcon()
        {
            CollectionAssert.Contains(_asset.IconNames, KnownIcon);
        }

        [Test]
        public void IconCategories_ContainsKnownCategory()
        {
            CollectionAssert.Contains(_asset.IconCategories, KnownCategory);
        }

        #endregion


        #region GetIcon

        [Test]
        public void GetIcon_KnownIcon_ReturnsTextureOfExpectedSize()
        {
            Texture2D icon = _asset.GetIcon(KnownIcon);
            Assert.IsNotNull(icon);
            Assert.AreEqual(_asset.IconSize, icon.width);
            Assert.AreEqual(_asset.IconSize, icon.height);
        }

        [Test]
        public void GetIcon_DoubledSize_ReturnsLargerTexture()
        {
            Texture2D icon = _asset.GetIcon(KnownIcon, x2: true);
            Assert.IsNotNull(icon);
            Assert.AreEqual(_asset.IconSize2x, icon.width);
        }

        [Test]
        public void GetIcon_UnknownIcon_ReturnsNull()
        {
            Assert.IsNull(_asset.GetIcon("__not_a_real_icon_name__"));
        }

        [Test]
        public void GetIcon_SameArguments_ReturnsCachedInstance()
        {
            // Regression guard for the texture-leak fix: repeated calls must reuse a single texture rather than
            // allocating a fresh Texture2D each time (GetIcon is called on every editor repaint).
            Texture2D first = _asset.GetIcon(KnownIcon);
            Texture2D second = _asset.GetIcon(KnownIcon);
            Assert.AreSame(first, second);
        }

        [Test]
        public void GetIcon_IsCaseInsensitive_AndSharesCache()
        {
            Texture2D lower = _asset.GetIcon(KnownIcon);
            Texture2D upper = _asset.GetIcon(KnownIcon.ToUpper());
            Assert.AreSame(lower, upper);
        }

        [Test]
        public void GetIcon_BlackAndWhite_AreDistinctInstances()
        {
            Texture2D black = _asset.GetIcon(KnownIcon, white: false);
            Texture2D white = _asset.GetIcon(KnownIcon, white: true);
            Assert.AreNotSame(black, white);
        }

        #endregion


        #region Categories

        [Test]
        public void GetIconCategoryName_KnownIcon_ReturnsItsCategory()
        {
            // Also guards the null-check added to GetIconCategoryName: it must build the category cache on demand
            // instead of throwing when the categories were not otherwise initialized first.
            Assert.AreEqual(KnownCategory, _asset.GetIconCategoryName(KnownIcon));
        }

        [Test]
        public void GetIconNamesInCategory_KnownCategory_ContainsItsIcons()
        {
            string[] names = _asset.GetIconNamesInCategory(KnownCategory);
            Assert.Greater(names.Length, 0);
            CollectionAssert.Contains(names, KnownIcon);
        }

        [Test]
        public void GetIconNamesInCategory_UnknownCategory_ReturnsEmptyNonNull()
        {
            string[] names = _asset.GetIconNamesInCategory("__not_a_real_category__");
            Assert.IsNotNull(names);
            Assert.AreEqual(0, names.Length);
        }

        #endregion

    }

}
