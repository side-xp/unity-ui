using NUnit.Framework;

using UnityEngine;

using SideXP.UI.EditorOnly;

namespace SideXP.UI.Tests
{

    public class MaterialIconsMappingsUtilityTests
    {

        // Builds a TextAsset from a raw JSON string so we can exercise parsing without touching the bundled asset.
        private static TextAsset JsonAsset(string json)
        {
            return new TextAsset(json);
        }

        #region Parse - valid content

        [Test]
        public void Parse_SingleAtlasSingleIcon_ReadsEveryField()
        {
            var mappings = MaterialIconsMappingsUtility.Parse(JsonAsset(
                "{\"mappings\":[{\"iconSize\":24,\"icons\":[{\"x\":10,\"y\":20,\"name\":\"home\",\"category\":\"action\"}]}]}"));

            Assert.IsNotNull(mappings);
            Assert.AreEqual(1, mappings.mappings.Length);

            var atlas = mappings.mappings[0];
            Assert.AreEqual(24, atlas.iconSize);
            Assert.AreEqual(1, atlas.icons.Length);

            var icon = atlas.icons[0];
            Assert.AreEqual(10, icon.x);
            Assert.AreEqual(20, icon.y);
            Assert.AreEqual("home", icon.name);
            Assert.AreEqual("action", icon.category);
        }

        [Test]
        public void Parse_MultipleIcons_PreservesOrderAndCount()
        {
            var mappings = MaterialIconsMappingsUtility.Parse(JsonAsset(
                "{\"mappings\":[{\"iconSize\":24,\"icons\":[" +
                "{\"x\":0,\"y\":0,\"name\":\"a\",\"category\":\"action\"}," +
                "{\"x\":24,\"y\":0,\"name\":\"b\",\"category\":\"action\"}," +
                "{\"x\":48,\"y\":0,\"name\":\"c\",\"category\":\"content\"}]}]}"));

            var icons = mappings.mappings[0].icons;
            Assert.AreEqual(3, icons.Length);
            Assert.AreEqual("a", icons[0].name);
            Assert.AreEqual("c", icons[2].name);
            Assert.AreEqual("content", icons[2].category);
        }

        [Test]
        public void Parse_MultipleAtlases_AreAllRead()
        {
            var mappings = MaterialIconsMappingsUtility.Parse(JsonAsset(
                "{\"mappings\":[" +
                "{\"iconSize\":24,\"icons\":[{\"x\":0,\"y\":0,\"name\":\"a\",\"category\":\"action\"}]}," +
                "{\"iconSize\":48,\"icons\":[{\"x\":0,\"y\":0,\"name\":\"b\",\"category\":\"action\"}]}]}"));

            Assert.AreEqual(2, mappings.mappings.Length);
            Assert.AreEqual(24, mappings.mappings[0].iconSize);
            Assert.AreEqual(48, mappings.mappings[1].iconSize);
        }

        #endregion


        #region Parse - empty / missing content

        [Test]
        public void Parse_EmptyMappingsArray_ReturnsEmptyNonNull()
        {
            var mappings = MaterialIconsMappingsUtility.Parse(JsonAsset("{\"mappings\":[]}"));
            Assert.IsNotNull(mappings.mappings);
            Assert.AreEqual(0, mappings.mappings.Length);
        }

        [Test]
        public void Parse_EmptyObject_LeavesMappingsEmptyNonNull()
        {
            // JsonUtility.FromJsonOverwrite leaves fields that are absent from the JSON untouched, so the field
            // keeps its initialized empty-array default rather than becoming null.
            var mappings = MaterialIconsMappingsUtility.Parse(JsonAsset("{}"));
            Assert.IsNotNull(mappings.mappings);
            Assert.AreEqual(0, mappings.mappings.Length);
        }

        #endregion


        #region Round-trip

        [Test]
        public void Parse_RoundTripThroughJsonUtility_IsStable()
        {
            var source = new MaterialIconsMappings
            {
                mappings = new[]
                {
                    new MaterialIconsMappings.Atlas
                    {
                        iconSize = 24,
                        icons = new[] { new MaterialIconsMappings.Icon { x = 1, y = 2, name = "home", category = "action" } },
                    },
                },
            };

            var reparsed = MaterialIconsMappingsUtility.Parse(JsonAsset(JsonUtility.ToJson(source)));

            Assert.AreEqual(1, reparsed.mappings.Length);
            Assert.AreEqual("home", reparsed.mappings[0].icons[0].name);
            Assert.AreEqual(2, reparsed.mappings[0].icons[0].y);
        }

        #endregion

    }

}
