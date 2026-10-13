// (c) Xavalon. All rights reserved.

using Irony.Parsing;

namespace Xavalon.XamlStyler.MarkupExtensions.Parser
{
    // Grammar for XAML markup extension: See https://msdn.microsoft.com/en-us/library/ee200269.aspx
    // MarkupExtension = "{" TYPENAME 0*1Arguments "}"
    // Arguments       = (NamedArgs / (PositionalArgs 0*1("," NamedArgs))
    // NamedArgs       = NamedArg*("," NamedArg)
    // NamedArg        = MEMBERNAME "=" STRING
    // PositionalArgs  = NamedArg / (STRING 0*1( "," PositionalArgs))
    [Language("XamlMarkupExtension", "1.0", "Xaml Markup Extension")]
    public class XamlMarkupExtensionGrammar : Grammar
    {
        public const string MarkupExtensionTerm = "MarkupExtension";
        public const string TypeNameTerm = "TYPENAME";
        public const string MemberNameTerm = "MEMBERNAME";
        public const string NamedArgumentTerm = "NamedArg";
        public const string StringTerm = "STRING";

        public XamlMarkupExtensionGrammar()
        {
            // Non Terminals
            var markupExtension = new NonTerminal(MarkupExtensionTerm);
            var arguments = new NonTerminal("Arguments");
            var namedArgs = new NonTerminal("NamedArgs");
            var namedArg = new NonTerminal(NamedArgumentTerm);
            var positionalArgs = new NonTerminal("PositionalArgs");
            var argument = new NonTerminal("Argument");

            // Terminals
            var typeName = new TypeNameTerminal(TypeNameTerm);
            var memberName = new MemberNameTerminal(MemberNameTerm);
            var @string = new StringTerminal(StringTerm);
            var startExtension = this.ToTransientTerm("{");
            var endExtension = this.ToTransientTerm("}");
            var namedArgumentSeparator = this.ToTransientTerm("=");
            var argumentSeparator = this.ToTransientTerm(",");

            // Setup rules
            markupExtension.Rule = (startExtension + typeName + endExtension)
                | (startExtension + typeName + arguments + endExtension);

            arguments.Rule = namedArgs | positionalArgs | (positionalArgs + argumentSeparator + namedArgs);
            namedArgs.Rule = namedArg | (namedArg + argumentSeparator + namedArgs);
            namedArg.Rule = (memberName + namedArgumentSeparator + argument);
            positionalArgs.Rule = namedArgs | argument | (argument + argumentSeparator + positionalArgs);
            argument.Rule = markupExtension | @string;

            this.Root = markupExtension;
            this.MarkTransient(arguments, argument);
        }

        public override void BuildAst(LanguageData language, ParseTree parseTree)
        {
            // This grammar is consumed as a ParseTree by MarkupExtension.Create. It does not
            // define Irony AST node types, so never enter the reflective AST factory.
            if ((this.LanguageFlags & LanguageFlags.CreateAst) != 0)
            {
                throw new System.NotSupportedException("Markup extension parsing uses parse trees, not Irony AST nodes.");
            }
        }

        private BnfTerm ToTransientTerm(string text)
        {
            var term = ToTerm(text);
            term.Flags |= TermFlags.IsTransient;
            return term;
        }
    }
}