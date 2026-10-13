// (c) Xavalon. All rights reserved.
using System;
using System.Collections.Generic;
using System.Globalization;
using Xavalon.XamlStyler.DocumentManipulation;
using Xavalon.XamlStyler.Options;

namespace Xavalon.XamlStyler.Console
{
    public sealed partial class CommandLineOptions
    {
        internal static bool TryParse(string[] args, out CommandLineOptions options, out string message)
        {
            options = new CommandLineOptions { LogLevel = LogLevel.Default, File = Array.Empty<string>() };
            var expanded = new List<string>();
            foreach (string arg in args)
            {
                if (arg.Length > 2 && arg[0] == '-' && arg[1] != '-' &&
                    Array.TrueForAll(arg.Substring(1).ToCharArray(), c => c == 'i' || c == 'r' || c == 'p'))
                {
                    foreach (char flag in arg.Substring(1)) expanded.Add("-" + flag);
                }
                else expanded.Add(arg);
            }
            args = expanded.ToArray();
            var seen = new HashSet<string>();
            message = string.Empty;
            for (int i = 0; i < args.Length; i++)
            {
                string argument = args[i];
                if (argument == "--help" || argument == "--version")
                {
                    message = argument == "--help" ? HelpText : "xstyler " + typeof(Program).Assembly.GetName().Version;
                    return false;
                }
                string name;
                string value = null;
                if (argument.StartsWith("--", StringComparison.Ordinal))
                {
                    int separator = argument.IndexOf('=');
                    name = separator < 0 ? argument.Substring(2) : argument.Substring(2, separator - 2);
                    if (separator >= 0) value = argument.Substring(separator + 1);
                }
                else if (argument.StartsWith("-", StringComparison.Ordinal) && argument.Length >= 2)
                {
                    name = argument.Substring(1, 1);
                    if (argument.Length > 2) value = argument.Substring(2).TrimStart('=');
                }
                else
                {
                    message = "Unexpected argument: " + argument;
                    return false;
                }
                try
                {
                    switch (name)
                    {
                        case "f":
                        case "file":
                            if (!seen.Add("File")) throw new FormatException("Repeated option.");
                            var files = options.File == null ? new List<string>() : new List<string>(options.File);
                            var fileValue = ReadValue(args, ref i, value);
                            if (fileValue == null) throw new FormatException("Missing file value.");
                            files.AddRange(fileValue.Split(','));
                            while (i + 1 < args.Length && !args[i + 1].StartsWith("-", StringComparison.Ordinal))
                                files.AddRange(args[++i].Split(','));
                            options.File = files;
                            break;
                        case "d":
                        case "directory":
                            if (!seen.Add("Directory")) throw new FormatException("Repeated option.");
                            options.Directory = ReadValue(args, ref i, value);
                            break;
                        case "c":
                        case "config":
                            if (!seen.Add("Configuration")) throw new FormatException("Repeated option.");
                            options.Configuration = ReadValue(args, ref i, value);
                            break;
                        case "i":
                        case "ignore":
                            if (!seen.Add("Ignore")) throw new FormatException("Repeated option.");
                            options.Ignore = ReadBoolean(args, ref i, value, true);
                            break;
                        case "r":
                        case "recursive":
                            if (!seen.Add("IsRecursive")) throw new FormatException("Repeated option.");
                            options.IsRecursive = ReadBoolean(args, ref i, value, true);
                            break;
                        case "p":
                        case "passive":
                            if (!seen.Add("IsPassive")) throw new FormatException("Repeated option.");
                            options.IsPassive = ReadBoolean(args, ref i, value, true);
                            break;
                        case "write-to-stdout":
                            if (!seen.Add("WriteToStdout")) throw new FormatException("Repeated option.");
                            options.WriteToStdout = ReadBoolean(args, ref i, value, true);
                            break;
                        case "l":
                        case "loglevel":
                            if (!Enum.TryParse<LogLevel>(ReadValue(args, ref i, value), true, out var parsedLogLevel))
                                throw new FormatException("Invalid enum value.");
                            if (!seen.Add("LogLevel")) throw new FormatException("Repeated option.");
                            options.LogLevel = parsedLogLevel;
                            break;
                        case "indent-size":
                            if (!seen.Add("IndentSize")) throw new FormatException("Repeated option.");
                            var valueIndentSize = ReadValue(args, ref i, value);
                            options.IndentSize = valueIndentSize == null ? null : int.Parse(valueIndentSize, CultureInfo.InvariantCulture);
                            break;
                        case "indent-tabs":
                            if (!seen.Add("IndentWithTabs")) throw new FormatException("Repeated option.");
                            options.IndentWithTabs = ReadBoolean(args, ref i, value, false);
                            break;
                        case "attributes-tolerance":
                            if (!seen.Add("AttributesTolerance")) throw new FormatException("Repeated option.");
                            var valueAttributesTolerance = ReadValue(args, ref i, value);
                            options.AttributesTolerance = valueAttributesTolerance == null ? null : int.Parse(valueAttributesTolerance, CultureInfo.InvariantCulture);
                            break;
                        case "attributes-same-line":
                            if (!seen.Add("KeepFirstAttributeOnSameLine")) throw new FormatException("Repeated option.");
                            options.KeepFirstAttributeOnSameLine = ReadBoolean(args, ref i, value, false);
                            break;
                        case "attributes-max-chars":
                            if (!seen.Add("MaxAttributeCharactersPerLine")) throw new FormatException("Repeated option.");
                            var valueMaxAttributeCharactersPerLine = ReadValue(args, ref i, value);
                            options.MaxAttributeCharactersPerLine = valueMaxAttributeCharactersPerLine == null ? null : int.Parse(valueMaxAttributeCharactersPerLine, CultureInfo.InvariantCulture);
                            break;
                        case "attributes-max":
                            if (!seen.Add("MaxAttributesPerLine")) throw new FormatException("Repeated option.");
                            var valueMaxAttributesPerLine = ReadValue(args, ref i, value);
                            options.MaxAttributesPerLine = valueMaxAttributesPerLine == null ? null : int.Parse(valueMaxAttributesPerLine, CultureInfo.InvariantCulture);
                            break;
                        case "no-newline-elements":
                            if (!seen.Add("NoNewLineElements")) throw new FormatException("Repeated option.");
                            options.NoNewLineElements = ReadValue(args, ref i, value);
                            break;
                        case "attributes-order-groups-newline":
                            if (!seen.Add("PutAttributeOrderRuleGroupsOnSeparateLines")) throw new FormatException("Repeated option.");
                            options.PutAttributeOrderRuleGroupsOnSeparateLines = ReadBoolean(args, ref i, value, false);
                            break;
                        case "attributes-indentation":
                            if (!seen.Add("AttributeIndentation")) throw new FormatException("Repeated option.");
                            var valueAttributeIndentation = ReadValue(args, ref i, value);
                            options.AttributeIndentation = valueAttributeIndentation == null ? null : int.Parse(valueAttributeIndentation, CultureInfo.InvariantCulture);
                            break;
                        case "attributes-indentation-style":
                            if (!Enum.TryParse<AttributeIndentationStyle>(ReadValue(args, ref i, value), true, out var parsedAttributeIndentationStyle))
                                throw new FormatException("Invalid enum value.");
                            if (!seen.Add("AttributeIndentationStyle")) throw new FormatException("Repeated option.");
                            options.AttributeIndentationStyle = parsedAttributeIndentationStyle;
                            break;
                        case "remove-design-references":
                            if (!seen.Add("RemoveDesignTimeReferences")) throw new FormatException("Repeated option.");
                            options.RemoveDesignTimeReferences = ReadBoolean(args, ref i, value, false);
                            break;
                        case "attributes-reorder":
                            if (!seen.Add("EnableAttributeReordering")) throw new FormatException("Repeated option.");
                            options.EnableAttributeReordering = ReadBoolean(args, ref i, value, false);
                            break;
                        case "attributes-first-line":
                            if (!seen.Add("FirstLineAttributes")) throw new FormatException("Repeated option.");
                            options.FirstLineAttributes = ReadValue(args, ref i, value);
                            break;
                        case "attributes-order-name":
                            if (!seen.Add("OrderAttributesByName")) throw new FormatException("Repeated option.");
                            options.OrderAttributesByName = ReadBoolean(args, ref i, value, false);
                            break;
                        case "ending-bracket-newline":
                            if (!seen.Add("PutEndingBracketOnNewLine")) throw new FormatException("Repeated option.");
                            options.PutEndingBracketOnNewLine = ReadBoolean(args, ref i, value, false);
                            break;
                        case "remove-empty-ending-tag":
                            if (!seen.Add("RemoveEndingTagOfEmptyElement")) throw new FormatException("Repeated option.");
                            options.RemoveEndingTagOfEmptyElement = ReadBoolean(args, ref i, value, false);
                            break;
                        case "space-before-closing-slash":
                            if (!seen.Add("SpaceBeforeClosingSlash")) throw new FormatException("Repeated option.");
                            options.SpaceBeforeClosingSlash = ReadBoolean(args, ref i, value, false);
                            break;
                        case "root-line-break":
                            if (!Enum.TryParse<LineBreakRule>(ReadValue(args, ref i, value), true, out var parsedRootElementLineBreakRule))
                                throw new FormatException("Invalid enum value.");
                            if (!seen.Add("RootElementLineBreakRule")) throw new FormatException("Repeated option.");
                            options.RootElementLineBreakRule = parsedRootElementLineBreakRule;
                            break;
                        case "reorder-vsm":
                            if (!Enum.TryParse<VisualStateManagerRule>(ReadValue(args, ref i, value), true, out var parsedReorderVSM))
                                throw new FormatException("Invalid enum value.");
                            if (!seen.Add("ReorderVSM")) throw new FormatException("Repeated option.");
                            options.ReorderVSM = parsedReorderVSM;
                            break;
                        case "reorder-grid-children":
                            if (!seen.Add("ReorderGridChildren")) throw new FormatException("Repeated option.");
                            options.ReorderGridChildren = ReadBoolean(args, ref i, value, false);
                            break;
                        case "reorder-canvas-children":
                            if (!seen.Add("ReorderCanvasChildren")) throw new FormatException("Repeated option.");
                            options.ReorderCanvasChildren = ReadBoolean(args, ref i, value, false);
                            break;
                        case "reorder-setters":
                            if (!Enum.TryParse<ReorderSettersBy>(ReadValue(args, ref i, value), true, out var parsedReorderSetters))
                                throw new FormatException("Invalid enum value.");
                            if (!seen.Add("ReorderSetters")) throw new FormatException("Repeated option.");
                            options.ReorderSetters = parsedReorderSetters;
                            break;
                        case "format-markup-extension":
                            if (!seen.Add("FormatMarkupExtension")) throw new FormatException("Repeated option.");
                            options.FormatMarkupExtension = ReadBoolean(args, ref i, value, false);
                            break;
                        case "no-newline-markup-extensions":
                            if (!seen.Add("NoNewLineMarkupExtensions")) throw new FormatException("Repeated option.");
                            options.NoNewLineMarkupExtensions = ReadValue(args, ref i, value);
                            break;
                        case "thickness-style":
                            if (!Enum.TryParse<ThicknessStyle>(ReadValue(args, ref i, value), true, out var parsedThicknessStyle))
                                throw new FormatException("Invalid enum value.");
                            if (!seen.Add("ThicknessStyle")) throw new FormatException("Repeated option.");
                            options.ThicknessStyle = parsedThicknessStyle;
                            break;
                        case "thickness-attributes":
                            if (!seen.Add("ThicknessAttributes")) throw new FormatException("Repeated option.");
                            options.ThicknessAttributes = ReadValue(args, ref i, value);
                            break;
                        case "comment-spaces":
                            if (!seen.Add("CommentSpaces")) throw new FormatException("Repeated option.");
                            var valueCommentSpaces = ReadValue(args, ref i, value);
                            options.CommentSpaces = valueCommentSpaces == null ? null : int.Parse(valueCommentSpaces, CultureInfo.InvariantCulture);
                            break;
                        default:
                            message = "Unknown option: " + argument;
                            return false;
                    }
                }
                catch (Exception e) when (e is FormatException || e is OverflowException || e is ArgumentException)
                {
                    message = "Invalid value for " + argument + ": " + e.Message;
                    return false;
                }
            }
            return true;
        }

        private static string ReadValue(string[] args, ref int index, string value)
        {
            if (value != null) return value;
            if (index + 1 >= args.Length || args[index + 1].StartsWith("--", StringComparison.Ordinal)) return null;
            return args[++index];
        }

        private static bool ReadBoolean(string[] args, ref int index, string value, bool flag)
        {
            if (value != null) return bool.Parse(value);
            if (index + 1 < args.Length && bool.TryParse(args[index + 1], out bool parsed))
            {
                index++;
                return parsed;
            }
            if (flag) return true;
            return bool.Parse(ReadValue(args, ref index, value));
        }

        private const string HelpText = "xstyler - format XAML files\n" +
            "--file  XAML file to process (supports comma-separated list).\n" +
            "--directory  Directory to process XAML files in.\n" +
            "--config  JSON file containing XAML Styler settings configuration.\n" +
            "--ignore  Ignore XAML file type check and process all files.\n" +
            "--recursive  Recursively process specified directory.\n" +
            "--passive  Check files follow proper formatting without making any modifications. Returns error status if files fail the check.\n" +
            "--write-to-stdout  Instead of modifying the file, write to stdout. In this mode, logs are printed to stderr. Must specify exactly one file. Cannot be combined with --passive.\n" +
            "--loglevel  Levels in order of increasing detail: None, Minimal, Default, Verbose, Debug\n" +
            "--indent-size  Override: indent size.\n" +
            "--indent-tabs  Override: indent with tabs.\n" +
            "--attributes-tolerance  Override: attributes tolerance.\n" +
            "--attributes-same-line  Override: keep first attribute on the same line.\n" +
            "--attributes-max-chars  Override: max attribute characters per line.\n" +
            "--attributes-max  Override: max attributes per line.\n" +
            "--no-newline-elements  Override: no newline elements.\n" +
            "--attributes-order-groups-newline  Override: put attribute order rule groups on separate lines.\n" +
            "--attributes-indentation  Override: attribute indentation.\n" +
            "--attributes-indentation-style  Override: attribute indentation style.\n" +
            "--remove-design-references  Override: remove design time references.\n" +
            "--attributes-reorder  Override: enable attribute reordering.\n" +
            "--attributes-first-line  Override: first line attributes.\n" +
            "--attributes-order-name  Override: order attributes by name.\n" +
            "--ending-bracket-newline  Override: put ending bracket on new line.\n" +
            "--remove-empty-ending-tag  Override: remove ending tag of empty element.\n" +
            "--space-before-closing-slash  Override: space before closing slash.\n" +
            "--root-line-break  Override: root element line break rule.\n" +
            "--reorder-vsm  Override: reorder visual state manager rule.\n" +
            "--reorder-grid-children  Override: reorder grid children.\n" +
            "--reorder-canvas-children  Override: reorder canvas children.\n" +
            "--reorder-setters  Override: reorder setters.\n" +
            "--format-markup-extension  Override: format markup extension.\n" +
            "--no-newline-markup-extensions  Override: no newline markup extensions.\n" +
            "--thickness-style  Override: thickness style.\n" +
            "--thickness-attributes  Override: thickness attributes.\n" +
            "--comment-spaces  Override: comment spaces.\n";
    }
}
