// (c) Xavalon. All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CommandLine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Xavalon.XamlStyler.Console;
using Xavalon.XamlStyler.Options;
using Xavalon.XamlStyler.MarkupExtensions.Parser;
using Irony.Parsing;

namespace Xavalon.XamlStyler.UnitTests
{
    public class AotCompatibilityTests
    {
        [TestCase("{ /* comment */ \"attributestolerance\": \"3\", \"RootElementLineBreakRule\": \"Always\", }")]
        [TestCase("{ \"IndentWithTabs\": true, \"IndentSize\": 2 }")]
        [TestCase("{ \"IndentWithTabs\": false, \"UnknownSetting\": 42 }")]
        [TestCase("{ \"AttributeOrderingRuleGroups\": [\"A\", \"B\"] }")]
        [TestCase("{ \"FormatMarkupExtension\": \"false\", \"AttributesTolerance\": 2 }")]
        public void ConfigurationMatchesLegacyValues(string json)
        {
            string file = Path.GetTempFileName();
            try
            {
                File.WriteAllText(file, json);
                var actual = new StylerOptions(file);
                var expected = JsonConvert.DeserializeObject<StylerOptions>(json);
                expected.IndentSize = expected.IndentSize > 0 ? expected.IndentSize : 4;
                Assert.That(JToken.DeepEquals(JToken.Parse(actual.ToJson()), JToken.Parse(expected.ToJson())), Is.True);
            }
            finally { File.Delete(file); }
        }

        [Test]
        public void ClonePreservesValuesWithoutSharingRuleGroupsOrExternalPath()
        {
            var original = new StylerOptions { IndentSize = 2, AttributeOrderingRuleGroups = new[] { "A", "B" } };
            var clone = (StylerOptions)original.Clone();
            Assert.That(clone.ToJson(), Is.EqualTo(original.ToJson()));
            clone.AttributeOrderingRuleGroups[0] = "Changed";
            Assert.That(original.AttributeOrderingRuleGroups[0], Is.EqualTo("A"));
            Assert.That(clone.ConfigPath, Is.Empty);
        }

        [Test]
        public void MarkupGrammarCannotEnterReflectiveAstConstruction()
        {
            var grammar = new XamlMarkupExtensionGrammar();
            Assert.That(grammar.LanguageFlags & LanguageFlags.CreateAst, Is.EqualTo((LanguageFlags)0));
            grammar.BuildAst(null, null);
            grammar.LanguageFlags |= LanguageFlags.CreateAst;
            Assert.Throws<NotSupportedException>(() => grammar.BuildAst(null, null));
        }

        public static IEnumerable<TestCaseData> CliCases
        {
            get
            {
                var cases = new[]
                {
                    new[] { "--file", "a.xaml", "--write-to-stdout" },
                    new[] { "-f", "a.xaml,b.xaml", "-r", "-i" },
                    new[] { "--indent-size", "-1", "--indent-tabs", "false" },
                    new[] { "--attributes-tolerance=0", "--root-line-break", "Always" },
                    new[] { "--loglevel", "Debug", "--format-markup-extension", "true" },
                    new[] { "--file", "a.xaml", "b.xaml", "--config", "custom.json" },
                    new[] { "--unknown" }, new[] { "--indent-size" },
                    new[] { "--indent-size", "oops" }, new[] { "--help" },
                    new[] { "--version" }, new[] { "--write-to-stdout", "--write-to-stdout" },
                    new[] { "-ir" }, new[] { "--file" }
                };
                foreach (var args in cases) yield return new TestCaseData(new object[] { args });
                foreach (var property in typeof(CommandLineOptions).GetProperties())
                {
                    var option = property.GetCustomAttribute<OptionAttribute>();
                    if (option == null || property.Name == "File" || property.Name == "Directory" || property.Name == "Configuration") continue;
                    var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                    string value = type == typeof(bool) ? "false" : type == typeof(int) ? "2" : type.IsEnum ? "0" : "Setter";
                    if (property.PropertyType == typeof(bool))
                        yield return new TestCaseData(new object[] { new[] { "--" + option.LongName } });
                    else
                        yield return new TestCaseData(new object[] { new[] { "--" + option.LongName, value } });
                }

            }
        }

        [TestCaseSource(nameof(CliCases))]
        public void StaticCliMatchesDeclaredParser(string[] args)
        {
            using var writer = new StringWriter();
            using var parser = new CommandLine.Parser(settings => settings.HelpWriter = writer);
            var legacy = parser.ParseArguments<CommandLineOptions>(args);
            bool success = CommandLineOptions.TryParse(args, out var actual, out _);
            Assert.That(success, Is.EqualTo(legacy.Tag == ParserResultType.Parsed));
            if (!success) return;
            var expected = ((Parsed<CommandLineOptions>)legacy).Value;
            foreach (var property in typeof(CommandLineOptions).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                Assert.That(property.GetValue(actual), Is.EqualTo(property.GetValue(expected)), property.Name);
        }
    }
}
