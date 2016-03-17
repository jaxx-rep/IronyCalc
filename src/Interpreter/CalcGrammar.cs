using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;
using IronyCalc.Interpreter.Evaluator;
using System;

namespace IronyCalc.Interpreter
{
    [Language("CalcEvaluator", "1.0", "Multi-line expression evaluator")]
    public class CalcGrammar : InterpretedLanguageGrammar
    {
        public CalcGrammar() : base(caseSensitive: false)
        {
            // 1. Terminals
            var number = new NumberLiteral("number");
            // Let's allow big integers (with unlimited number of digits):
            number.DefaultIntTypes = new TypeCode[] { TypeCode.Int32, TypeCode.Int64, NumberLiteral.TypeCodeBigInt };
            var identifier = TerminalFactory.CreateCSharpIdentifier("identifier");
            var comment = new CommentTerminal("comment", "#", "\n", "\r");
            base.NonGrammarTerminals.Add(comment);
            var comma = ToTerm(",");

            // String literal with embedded expressions
            var stringLit = new StringLiteral("string", "\"", StringOptions.AllowsAllEscapes | StringOptions.IsTemplate);
            stringLit.AddStartEnd("'", StringOptions.AllowsAllEscapes | StringOptions.IsTemplate);
            stringLit.AstConfig.NodeType = typeof(StringTemplateNode);
            var Expr = new NonTerminal("Expr");
            var templateSettings = new StringTemplateSettings();
            templateSettings.ExpressionRoot = Expr;
            this.SnippetRoots.Add(Expr);
            stringLit.AstConfig.Data = templateSettings;

            // 2. Non-terminals
            var Term = new NonTerminal("Term");
            var BinExpr = new NonTerminal("BinExpr", typeof(BinaryOperationNode));
            var ParExpr = new NonTerminal("ParExpr");
            var UnExpr = new NonTerminal("UnExpr", typeof(UnaryOperationNode));
            var TernaryIfExpr = new NonTerminal("TernaryIf", typeof(IfNode));
            var ArgList = new NonTerminal("ArgList", typeof(ExpressionListNode));
            var FunctionCall = new NonTerminal("FunctionCall", typeof(FunctionCallNode));
            var MemberAccess = new NonTerminal("MemberAccess", typeof(MemberAccessNode));
            var IndexedAccess = new NonTerminal("IndexedAccess", typeof(IndexedAccessNode));
            var ObjectRef = new NonTerminal("ObjectRef"); // foo, foo.bar or f['bar']
            var UnOp = new NonTerminal("UnOp");
            var BinOp = new NonTerminal("BinOp", "operator");
            var PrefixIncDec = new NonTerminal("PrefixIncDec", typeof(IncDecNode));
            var PostfixIncDec = new NonTerminal("PostfixIncDec", typeof(IncDecNode));
            var IncDecOp = new NonTerminal("IncDecOp");
            var AssignmentStmt = new NonTerminal("AssignmentStmt", typeof(AssignmentNode));
            var AssignmentOp = new NonTerminal("AssignmentOp", "assignment operator");
            var Statement = new NonTerminal("Statement");
            var Program = new NonTerminal("Program", typeof(StatementListNode));

            // 3. BNF rules (https://en.wikipedia.org/wiki/Backus%E2%80%93Naur_Form)
            Expr.Rule = Term | UnExpr | BinExpr | PrefixIncDec | PostfixIncDec | TernaryIfExpr;
            Term.Rule = number | ParExpr | stringLit | FunctionCall | identifier | MemberAccess | IndexedAccess;
            ParExpr.Rule = "(" + Expr + ")";
            UnExpr.Rule = UnOp + Term + ReduceHere();
            UnOp.Rule = ToTerm("+") | "-" | "!";
            BinExpr.Rule = Expr + BinOp + Expr;
            BinOp.Rule = ToTerm("+") | "-" | "*" | "/" | "%" | "^" | "**" | "==" | "<" | "<=" | ">" | ">=" | "!=" | "&&" | "||" | "&" | "|";
            PrefixIncDec.Rule = IncDecOp + identifier;
            PostfixIncDec.Rule = identifier + PreferShiftHere() + IncDecOp;
            IncDecOp.Rule = ToTerm("++") | "--";
            TernaryIfExpr.Rule = Expr + "?" + Expr + ":" + Expr;
            MemberAccess.Rule = Expr + PreferShiftHere() + "." + identifier;
            AssignmentStmt.Rule = ObjectRef + AssignmentOp + Expr;
            AssignmentOp.Rule = ToTerm("=") | "+=" | "-=" | "*=" | "/=";
            Statement.Rule = AssignmentStmt | Expr | Empty;
            ArgList.Rule = MakeStarRule(ArgList, comma, Expr);
            FunctionCall.Rule = Expr + PreferShiftHere() + "(" + ArgList + ")";
            FunctionCall.NodeCaptionTemplate = "call #{0}(...)";
            ObjectRef.Rule = identifier | MemberAccess | IndexedAccess;
            IndexedAccess.Rule = Expr + PreferShiftHere() + "[" + Expr + "]";

            Program.Rule = MakePlusRule(Program, NewLine, Statement);
            this.Root = Program;

            // 4. Operators precedence
            RegisterOperators(10, "?");
            RegisterOperators(15, "&", "&&", "|", "||");
            RegisterOperators(20, "==", "<", "<=", ">", ">=", "!=");
            RegisterOperators(30, "+", "-");
            RegisterOperators(40, "*", "/", "%");
            RegisterOperators(50, Associativity.Right, "**");
            RegisterOperators(60, "!");

            // 5. Punctuation and transient terms
            MarkPunctuation("(", ")", "?", ":", "[", "]");
            RegisterBracePair("(", ")");
            RegisterBracePair("[", "]");
            MarkTransient(Term, Expr, Statement, BinOp, UnOp, IncDecOp, AssignmentOp, ParExpr, ObjectRef);

            // 7. Syntax error reporting
            MarkNotReported("++", "--");
            AddToNoReportGroup("(", "++", "--");
            AddToNoReportGroup(NewLine);
            AddOperatorReportGroup("operator");
            AddTermsReportGroup("assignment operator", "=", "+=", "-=", "*=", "/=");

            //8. Console
            ConsoleTitle = Resources.HelpStrings.Title;
            ConsoleGreeting = Resources.HelpStrings.Greeting;
            ConsolePrompt = "? ";
            ConsolePromptMoreInput = "? ";

            //9. Language flags.
            // Automatically add NewLine before EOF so that our BNF rules work correctly when there's no final line break in source
            this.LanguageFlags = LanguageFlags.NewLineBeforeEOF | LanguageFlags.CreateAst | LanguageFlags.SupportsBigInt;
        }

        public override LanguageRuntime CreateRuntime(LanguageData language)
        {
            return new ExpressionEvaluator(language);
        }
    }
}
