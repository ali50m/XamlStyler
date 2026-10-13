// (c) Xavalon. All rights reserved.

using Newtonsoft.Json;
using NUnit.Framework;
using System.IO;
using System.Reflection;
using Xavalon.XamlStyler.Options;

namespace Xavalon.XamlStyler.UnitTests
{
    //Should these tests fail with a File -or Directory not found exception, disable Shadow-Copy assemblies in your
    //test runner (probably ReSharper, it searches for the config files in the wrong directory)

    [TestFixture]
    public sealed partial class Tests
    {
        [Test]
        public void TestConfigurationDefault()
        {
            var stylerOptions = new StylerOptions(config: Tests.GetConfiguration(@"TestConfigurations/Default.json"));
            this.TestConfig(stylerOptions, @"TestConfigurations/SerializedDefault.json");

            Assert.That(stylerOptions.UseVisualStudioIndentWithTabs);
        }

        [Test]
        public void TestConfigurationEmpty()
        {
            var stylerOptions = new StylerOptions(config: Tests.GetConfiguration(@"TestConfigurations/Empty.json"));
            this.TestConfig(stylerOptions, @"TestConfigurations/SerializedDefault.json");
        }

        [Test]
        public void TestConfigurationSingle()
        {
            var stylerOptions = new StylerOptions(config: Tests.GetConfiguration(@"TestConfigurations/Single.json"));
            this.TestConfig(stylerOptions, @"TestConfigurations/Single.json");
        }

        [Test]
        public void TestConfigurationBadSetting()
        {
            var stylerOptions = new StylerOptions(config: Tests.GetConfiguration(@"TestConfigurations/BadSetting.json"));
            this.TestConfig(stylerOptions, @"TestConfigurations/SerializedDefault.json");
        }

        [Test]
        public void TestConfigurationAllDifferent()
        {
            var stylerOptions = new StylerOptions(config: Tests.GetConfiguration(@"TestConfigurations/AllDifferent.json"));
            this.TestConfig(stylerOptions, @"TestConfigurations/AllDifferent.json");
        }

        [Test]
        public void TestConfigurationIndentUsingTabs()
        {
            var stylerOptions = new StylerOptions(config: Tests.GetConfiguration(@"TestConfigurations/IndentWithTabsOverride.json"));
            this.TestConfig(stylerOptions, @"TestConfigurations/IndentWithTabsOverride.json");

            Assert.That(!stylerOptions.UseVisualStudioIndentWithTabs); // IndentWithTabs is true
            Assert.That(!stylerOptions.UseVisualStudioIndentSize); // IndentSize is set
        }

        private void TestConfig(StylerOptions stylerOptions, string expectedConfiguration)
        {
            var actualOptions = JsonConvert.SerializeObject(stylerOptions);
            var expectedOptions = File.ReadAllText(Tests.GetConfiguration(expectedConfiguration));

            var actual = Newtonsoft.Json.Linq.JObject.Parse(actualOptions);
            var expected = Newtonsoft.Json.Linq.JObject.Parse(expectedOptions);
            // Default arrays compare by reference in Newtonsoft's omission logic. Verify their
            // contents rather than coupling configuration tests to that serializer detail.
            if (expected["AttributeOrderingRuleGroups"] == null && actual["AttributeOrderingRuleGroups"] != null)
            {
                using var stream = typeof(StylerOptions).Assembly.GetManifestResourceStream("Xavalon.XamlStyler.Options.DefaultSettings.json");
                using var reader = new StreamReader(stream);
                var defaults = Newtonsoft.Json.Linq.JObject.Parse(reader.ReadToEnd());
                expected["AttributeOrderingRuleGroups"] = defaults["AttributeOrderingRuleGroups"];
            }
            Assert.That(Newtonsoft.Json.Linq.JToken.DeepEquals(actual, expected), Is.True,
                $"Expected: {expected}\nActual: {actual}");
        }

        private static string GetConfiguration(string path)
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path);
        }
    }
}
