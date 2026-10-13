// (c) Xavalon. All rights reserved.
#if NET10_0_OR_GREATER
using System.Text.Json.Serialization;
using Xavalon.XamlStyler.DocumentManipulation;

namespace Xavalon.XamlStyler.Options
{
    // Explicit configuration contract: no runtime reflection or constructor recursion.
    internal sealed class StylerOptionsData
    {
        [JsonPropertyName("IndentSize")]
        public int IndentSize { get; set; } = -1;

        [JsonPropertyName("IndentWithTabs")]
        [JsonConverter(typeof(ConfigurationNullableBooleanConverter))]
        public bool? IndentWithTabs { get; set; } = null;

        [JsonPropertyName("AttributesTolerance")]
        public int AttributesTolerance { get; set; } = 2;

        [JsonPropertyName("KeepFirstAttributeOnSameLine")]
        [JsonConverter(typeof(ConfigurationBooleanConverter))]
        public bool KeepFirstAttributeOnSameLine { get; set; } = false;

        [JsonPropertyName("MaxAttributeCharactersPerLine")]
        public int MaxAttributeCharactersPerLine { get; set; } = 0;

        [JsonPropertyName("MaxAttributesPerLine")]
        public int MaxAttributesPerLine { get; set; } = 1;

        [JsonPropertyName("NewlineExemptionElements")]
        public string NoNewLineElements { get; set; } = "RadialGradientBrush, GradientStop, LinearGradientBrush, ScaleTransform, SkewTransform, RotateTransform, TranslateTransform, Trigger, Condition, Setter";

        [JsonPropertyName("SeparateByGroups")]
        [JsonConverter(typeof(ConfigurationBooleanConverter))]
        public bool PutAttributeOrderRuleGroupsOnSeparateLines { get; set; } = false;

        [JsonPropertyName("AttributeIndentation")]
        public int AttributeIndentation { get; set; } = 0;

        [JsonPropertyName("AttributeIndentationStyle")]
        public AttributeIndentationStyle AttributeIndentationStyle { get; set; } = AttributeIndentationStyle.Spaces;

        [JsonPropertyName("RemoveDesignTimeReferences")]
        [JsonConverter(typeof(ConfigurationBooleanConverter))]
        public bool RemoveDesignTimeReferences { get; set; } = false;

        [JsonPropertyName("EnableAttributeReordering")]
        [JsonConverter(typeof(ConfigurationBooleanConverter))]
        public bool EnableAttributeReordering { get; set; } = true;

        [JsonPropertyName("AttributeOrderingRuleGroups")]
        public string[] AttributeOrderingRuleGroups { get; set; } = new[]
        {
            "x:Class",
            "xmlns, xmlns:x",
            "xmlns:*",
            "x:Key, Key, x:Name, Name, x:Uid, Uid, Title",
            "Grid.Row, Grid.RowSpan, Grid.Column, Grid.ColumnSpan, Canvas.Left, Canvas.Top, Canvas.Right, Canvas.Bottom",
            "Width, Height, MinWidth, MinHeight, MaxWidth, MaxHeight",
            "Margin, Padding, HorizontalAlignment, VerticalAlignment, HorizontalContentAlignment, VerticalContentAlignment, Panel.ZIndex",
            "*:*, *",
            "PageSource, PageIndex, Offset, Color, TargetName, Property, Value, StartPoint, EndPoint",
            "mc:Ignorable, d:IsDataSource, d:LayoutOverrides, d:IsStaticText",
            //Storyboards, fixes #30
            "Storyboard.*, From, To, Duration",
        };

        [JsonPropertyName("FirstLineAttributes")]
        public string FirstLineAttributes { get; set; } = "";

        [JsonPropertyName("OrderAttributesByName")]
        [JsonConverter(typeof(ConfigurationBooleanConverter))]
        public bool OrderAttributesByName { get; set; } = true;

        [JsonPropertyName("PutEndingBracketOnNewLine")]
        [JsonConverter(typeof(ConfigurationBooleanConverter))]
        public bool PutEndingBracketOnNewLine { get; set; } = false;

        [JsonPropertyName("RemoveEndingTagOfEmptyElement")]
        [JsonConverter(typeof(ConfigurationBooleanConverter))]
        public bool RemoveEndingTagOfEmptyElement { get; set; } = true;

        [JsonPropertyName("SpaceBeforeClosingSlash")]
        [JsonConverter(typeof(ConfigurationBooleanConverter))]
        public bool SpaceBeforeClosingSlash { get; set; } = true;

        [JsonPropertyName("RootElementLineBreakRule")]
        public LineBreakRule RootElementLineBreakRule { get; set; } = LineBreakRule.Default;

        [JsonPropertyName("ReorderVSM")]
        public VisualStateManagerRule ReorderVSM { get; set; } = VisualStateManagerRule.Last;

        [JsonPropertyName("ReorderGridChildren")]
        [JsonConverter(typeof(ConfigurationBooleanConverter))]
        public bool ReorderGridChildren { get; set; } = false;

        [JsonPropertyName("ReorderCanvasChildren")]
        [JsonConverter(typeof(ConfigurationBooleanConverter))]
        public bool ReorderCanvasChildren { get; set; } = false;

        [JsonPropertyName("ReorderSetters")]
        public ReorderSettersBy ReorderSetters { get; set; } = ReorderSettersBy.None;

        [JsonPropertyName("FormatMarkupExtension")]
        [JsonConverter(typeof(ConfigurationBooleanConverter))]
        public bool FormatMarkupExtension { get; set; } = true;

        [JsonPropertyName("NoNewLineMarkupExtensions")]
        public string NoNewLineMarkupExtensions { get; set; } = "x:Bind, Binding";

        [JsonPropertyName("ThicknessSeparator")]
        public ThicknessStyle ThicknessStyle { get; set; } = ThicknessStyle.Comma;

        [JsonPropertyName("ThicknessAttributes")]
        public string ThicknessAttributes { get; set; } = "Margin, Padding, BorderThickness, ThumbnailClipMargin";

        [JsonPropertyName("FormatOnSave")]
        [JsonConverter(typeof(ConfigurationBooleanConverter))]
        public bool FormatOnSave { get; set; } = true;

        [JsonPropertyName("SaveAndCloseOnFormat")]
        [JsonConverter(typeof(ConfigurationBooleanConverter))]
        public bool SaveAndCloseOnFormat { get; set; } = true;

        [JsonPropertyName("CommentPadding")]
        public int CommentSpaces { get; set; } = 2;

        [JsonPropertyName("IgnoreDesignTimeReferencePrefix")]
        [JsonConverter(typeof(ConfigurationBooleanConverter))]
        public bool IgnoreDesignTimeReferencePrefix { get; set; } = false;

        [JsonPropertyName("SuppressProcessing")]
        [JsonConverter(typeof(ConfigurationBooleanConverter))]
        public bool SuppressProcessing { get; set; } = false;

        public static StylerOptionsData FromOptions(StylerOptions source)
        {
            return new StylerOptionsData
            {
                IndentSize = source.IndentSize,
                IndentWithTabs = source.IndentWithTabs,
                AttributesTolerance = source.AttributesTolerance,
                KeepFirstAttributeOnSameLine = source.KeepFirstAttributeOnSameLine,
                MaxAttributeCharactersPerLine = source.MaxAttributeCharactersPerLine,
                MaxAttributesPerLine = source.MaxAttributesPerLine,
                NoNewLineElements = source.NoNewLineElements,
                PutAttributeOrderRuleGroupsOnSeparateLines = source.PutAttributeOrderRuleGroupsOnSeparateLines,
                AttributeIndentation = source.AttributeIndentation,
                AttributeIndentationStyle = source.AttributeIndentationStyle,
                RemoveDesignTimeReferences = source.RemoveDesignTimeReferences,
                EnableAttributeReordering = source.EnableAttributeReordering,
                AttributeOrderingRuleGroups = source.AttributeOrderingRuleGroups,
                FirstLineAttributes = source.FirstLineAttributes,
                OrderAttributesByName = source.OrderAttributesByName,
                PutEndingBracketOnNewLine = source.PutEndingBracketOnNewLine,
                RemoveEndingTagOfEmptyElement = source.RemoveEndingTagOfEmptyElement,
                SpaceBeforeClosingSlash = source.SpaceBeforeClosingSlash,
                RootElementLineBreakRule = source.RootElementLineBreakRule,
                ReorderVSM = source.ReorderVSM,
                ReorderGridChildren = source.ReorderGridChildren,
                ReorderCanvasChildren = source.ReorderCanvasChildren,
                ReorderSetters = source.ReorderSetters,
                FormatMarkupExtension = source.FormatMarkupExtension,
                NoNewLineMarkupExtensions = source.NoNewLineMarkupExtensions,
                ThicknessStyle = source.ThicknessStyle,
                ThicknessAttributes = source.ThicknessAttributes,
                FormatOnSave = source.FormatOnSave,
                SaveAndCloseOnFormat = source.SaveAndCloseOnFormat,
                CommentSpaces = source.CommentSpaces,
                IgnoreDesignTimeReferencePrefix = source.IgnoreDesignTimeReferencePrefix,
                SuppressProcessing = source.SuppressProcessing,
            };
        }

        public StylerOptions ToOptions()
        {
            return new StylerOptions(isJsonConstructor: true)
            {
                IndentSize = this.IndentSize,
                IndentWithTabs = this.IndentWithTabs,
                AttributesTolerance = this.AttributesTolerance,
                KeepFirstAttributeOnSameLine = this.KeepFirstAttributeOnSameLine,
                MaxAttributeCharactersPerLine = this.MaxAttributeCharactersPerLine,
                MaxAttributesPerLine = this.MaxAttributesPerLine,
                NoNewLineElements = this.NoNewLineElements,
                PutAttributeOrderRuleGroupsOnSeparateLines = this.PutAttributeOrderRuleGroupsOnSeparateLines,
                AttributeIndentation = this.AttributeIndentation,
                AttributeIndentationStyle = this.AttributeIndentationStyle,
                RemoveDesignTimeReferences = this.RemoveDesignTimeReferences,
                EnableAttributeReordering = this.EnableAttributeReordering,
                AttributeOrderingRuleGroups = this.AttributeOrderingRuleGroups == null ? null : (string[])this.AttributeOrderingRuleGroups.Clone(),
                FirstLineAttributes = this.FirstLineAttributes,
                OrderAttributesByName = this.OrderAttributesByName,
                PutEndingBracketOnNewLine = this.PutEndingBracketOnNewLine,
                RemoveEndingTagOfEmptyElement = this.RemoveEndingTagOfEmptyElement,
                SpaceBeforeClosingSlash = this.SpaceBeforeClosingSlash,
                RootElementLineBreakRule = this.RootElementLineBreakRule,
                ReorderVSM = this.ReorderVSM,
                ReorderGridChildren = this.ReorderGridChildren,
                ReorderCanvasChildren = this.ReorderCanvasChildren,
                ReorderSetters = this.ReorderSetters,
                FormatMarkupExtension = this.FormatMarkupExtension,
                NoNewLineMarkupExtensions = this.NoNewLineMarkupExtensions,
                ThicknessStyle = this.ThicknessStyle,
                ThicknessAttributes = this.ThicknessAttributes,
                FormatOnSave = this.FormatOnSave,
                SaveAndCloseOnFormat = this.SaveAndCloseOnFormat,
                CommentSpaces = this.CommentSpaces,
                IgnoreDesignTimeReferencePrefix = this.IgnoreDesignTimeReferencePrefix,
                SuppressProcessing = this.SuppressProcessing,
            };
        }
    }

    [JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true, ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip,
        NumberHandling = JsonNumberHandling.AllowReadingFromString, UseStringEnumConverter = true)]
    [JsonSerializable(typeof(StylerOptionsData))]
    [JsonSerializable(typeof(string[]))]
    internal partial class StylerJsonContext : JsonSerializerContext { }
}
#endif
