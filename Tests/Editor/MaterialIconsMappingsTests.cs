using NUnit.Framework;

using SideXP.UI.EditorOnly;

namespace SideXP.UI.Tests
{

    public class MaterialIconsMappingsTests
    {

        #region Defaults

        [Test]
        public void Mappings_DefaultsToEmptyNonNullArray()
        {
            var mappings = new MaterialIconsMappings();
            Assert.IsNotNull(mappings.mappings);
            Assert.AreEqual(0, mappings.mappings.Length);
        }

        [Test]
        public void Atlas_DefaultsToEmptyNonNullIcons()
        {
            var atlas = new MaterialIconsMappings.Atlas();
            Assert.IsNotNull(atlas.icons);
            Assert.AreEqual(0, atlas.icons.Length);
        }

        #endregion


        #region ToString

        [Test]
        public void Atlas_ToString_ReportsCountAndSize()
        {
            var atlas = new MaterialIconsMappings.Atlas
            {
                iconSize = 24,
                icons = new[]
                {
                    new MaterialIconsMappings.Icon { name = "a" },
                    new MaterialIconsMappings.Icon { name = "b" },
                },
            };
            Assert.AreEqual("Material Icons Atlas (2 icons, 24px)", atlas.ToString());
        }

        [Test]
        public void Icon_ToString_ReportsNameCategoryAndPosition()
        {
            var icon = new MaterialIconsMappings.Icon { x = 48, y = 72, name = "home", category = "action" };
            Assert.AreEqual("Icon \"home\" (action), mapped at (48;72)", icon.ToString());
        }

        #endregion

    }

}
