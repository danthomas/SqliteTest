using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace StateMachine
{
    [TestFixture]
    public class Tests
    {
        private string template = @"public class |Name
{|{
ForEachProperty((p, f) => |
public |(f ? """","", "")|p.Type() |p.Name.CamelCase() { get; set; });
}

	public |Name({
ForEachProperty((p, f) => |(f ? """","", "")|p.Type() |p.Name.CamelCase());
}
)
	{}

}";


        [Test]
        public void Test()
        {
            Parser parser = new Parser();

            parser.Initialise("abc|def|ghi");

            parser.Step();
            Assert.That(parser.CurrChar, Is.EqualTo('a'));
            Assert.That(parser.CurrToken.TokenType, Is.EqualTo(TokenType.Text));
            Assert.That(parser.CurrToken.Text, Is.EqualTo("a"));

            parser.Step();
            Assert.That(parser.CurrChar, Is.EqualTo('b'));
            Assert.That(parser.CurrToken.TokenType, Is.EqualTo(TokenType.Text));
            Assert.That(parser.CurrToken.Text, Is.EqualTo("ab"));

            parser.Step();
            Assert.That(parser.CurrChar, Is.EqualTo('c'));
            Assert.That(parser.CurrToken.TokenType, Is.EqualTo(TokenType.Text));
            Assert.That(parser.CurrToken.Text, Is.EqualTo("abc"));

            parser.Step();
            Assert.That(parser.CurrChar, Is.EqualTo('|'));
            Assert.That(parser.CurrToken.TokenType, Is.EqualTo(TokenType.Statement));
            Assert.That(parser.CurrToken.Text, Is.EqualTo(""));

            parser.Step();
            Assert.That(parser.CurrChar, Is.EqualTo('d'));
            Assert.That(parser.CurrToken.TokenType, Is.EqualTo(TokenType.Statement));
            Assert.That(parser.CurrToken.Text, Is.EqualTo("d"));

            parser.Step();
            Assert.That(parser.CurrChar, Is.EqualTo('e'));
            Assert.That(parser.CurrToken.TokenType, Is.EqualTo(TokenType.Statement));
            Assert.That(parser.CurrToken.Text, Is.EqualTo("de"));

            parser.Step();
            Assert.That(parser.CurrChar, Is.EqualTo('f'));
            Assert.That(parser.CurrToken.TokenType, Is.EqualTo(TokenType.Statement));
            Assert.That(parser.CurrToken.Text, Is.EqualTo("def"));

            parser.Step();
            Assert.That(parser.CurrChar, Is.EqualTo('|'));
            Assert.That(parser.CurrToken.TokenType, Is.EqualTo(TokenType.Statement));
            Assert.That(parser.CurrToken.Text, Is.EqualTo(""));

            parser.Step();
            Assert.That(parser.CurrChar, Is.EqualTo('g'));
            Assert.That(parser.CurrToken.TokenType, Is.EqualTo(TokenType.Statement));
            Assert.That(parser.CurrToken.Text, Is.EqualTo("g"));

            parser.Step();
            Assert.That(parser.CurrChar, Is.EqualTo('h'));
            Assert.That(parser.CurrToken.TokenType, Is.EqualTo(TokenType.Statement));
            Assert.That(parser.CurrToken.Text, Is.EqualTo("gh"));

            parser.Step();
            Assert.That(parser.CurrChar, Is.EqualTo('i'));
            Assert.That(parser.CurrToken.TokenType, Is.EqualTo(TokenType.Statement));
            Assert.That(parser.CurrToken.Text, Is.EqualTo("ghi"));

        }

        [Test]
        public void StatementsAreIdentifiedByDelimiter()
        {
            Parser parser = new Parser();

            parser.Parse("public class |Name");

            Assert.That(parser.Tokens.Count, Is.EqualTo(2));
            
            Assert.That(parser.Tokens[0].TokenType, Is.EqualTo(TokenType.Text));
            Assert.That(parser.Tokens[0].Text, Is.EqualTo("public class "));

            Assert.That(parser.Tokens[1].TokenType, Is.EqualTo(TokenType.Statement));
            Assert.That(parser.Tokens[1].Text, Is.EqualTo("Name"));
        }

        [Test]
        public void StatementsCanFollowEachOther()
        {
            Parser parser = new Parser();

            parser.Parse("public class |Group|Name");

            Assert.That(parser.Tokens.Count, Is.EqualTo(3));

            Assert.That(parser.Tokens[0].TokenType, Is.EqualTo(TokenType.Text));
            Assert.That(parser.Tokens[0].Text, Is.EqualTo("public class "));

            Assert.That(parser.Tokens[1].TokenType, Is.EqualTo(TokenType.Statement));
            Assert.That(parser.Tokens[1].Text, Is.EqualTo("Group"));

            Assert.That(parser.Tokens[2].TokenType, Is.EqualTo(TokenType.Statement));
            Assert.That(parser.Tokens[2].Text, Is.EqualTo("Name"));
        }


        [Test]
        public void StatementsAreEndedByWhitespace()
        {
            Parser parser = new Parser();

            parser.Parse("public class |Name : IEntity");

            Assert.That(parser.Tokens.Count, Is.EqualTo(3));

            Assert.That(parser.Tokens[0].TokenType, Is.EqualTo(TokenType.Text));
            Assert.That(parser.Tokens[0].Text, Is.EqualTo("public class "));

            Assert.That(parser.Tokens[1].TokenType, Is.EqualTo(TokenType.Statement));
            Assert.That(parser.Tokens[1].Text, Is.EqualTo("Name"));

            Assert.That(parser.Tokens[2].TokenType, Is.EqualTo(TokenType.Text));
            Assert.That(parser.Tokens[2].Text, Is.EqualTo(" : IEntity"));
        }


        [Test]
        public void StatementsCanHaveDots()
        {
            Parser parser = new Parser();

            parser.Parse("public class |Entity.Name");

            Assert.That(parser.Tokens.Count, Is.EqualTo(2));

            Assert.That(parser.Tokens[0].TokenType, Is.EqualTo(TokenType.Text));
            Assert.That(parser.Tokens[0].Text, Is.EqualTo("public class "));

            Assert.That(parser.Tokens[1].TokenType, Is.EqualTo(TokenType.Statement));
            Assert.That(parser.Tokens[1].Text, Is.EqualTo("Entity.Name"));
        }

        [Test]
        public void StatementsCanHaveParens()
        {
            Parser parser = new Parser();

            parser.Parse("public class |Name()");

            Assert.That(parser.Tokens.Count, Is.EqualTo(2));

            Assert.That(parser.Tokens[0].TokenType, Is.EqualTo(TokenType.Text));
            Assert.That(parser.Tokens[0].Text, Is.EqualTo("public class "));

            Assert.That(parser.Tokens[1].TokenType, Is.EqualTo(TokenType.Statement));
            Assert.That(parser.Tokens[1].Text, Is.EqualTo("Name()"));
        }

        [Test]
        public void StatementsCanBeEnclosedInBraces()
        {
            Parser parser = new Parser();

            parser.Parse("public class |(Name)Repository");

            Assert.That(parser.Tokens.Count, Is.EqualTo(3));

            Assert.That(parser.Tokens[0].TokenType, Is.EqualTo(TokenType.Text));
            Assert.That(parser.Tokens[0].Text, Is.EqualTo("public class "));

            Assert.That(parser.Tokens[1].TokenType, Is.EqualTo(TokenType.Statement));
            Assert.That(parser.Tokens[1].Text, Is.EqualTo("(Name)"));

            Assert.That(parser.Tokens[2].TokenType, Is.EqualTo(TokenType.Text));
            Assert.That(parser.Tokens[2].Text, Is.EqualTo("Repository"));
        }
        
        [Test]
        public void StatementsCanHaveWhitespaceInParens()
        {
            Parser parser = new Parser();

            parser.Parse("public class |(f ? \"\" : \", \")");

            Assert.That(parser.Tokens.Count, Is.EqualTo(2));

            Assert.That(parser.Tokens[0].TokenType, Is.EqualTo(TokenType.Text));
            Assert.That(parser.Tokens[0].Text, Is.EqualTo("public class "));

            Assert.That(parser.Tokens[1].TokenType, Is.EqualTo(TokenType.Statement));
            Assert.That(parser.Tokens[1].Text, Is.EqualTo("(f ? \"\" : \", \")"));
        }

        [Test]
        public void StatementsCanHaveNestedParens()
        {
            Parser parser = new Parser();

            parser.Parse("public class |Name(Consts.Temp())");

            Assert.That(parser.Tokens.Count, Is.EqualTo(2));

            Assert.That(parser.Tokens[0].TokenType, Is.EqualTo(TokenType.Text));
            Assert.That(parser.Tokens[0].Text, Is.EqualTo("public class "));

            Assert.That(parser.Tokens[1].TokenType, Is.EqualTo(TokenType.Statement));
            Assert.That(parser.Tokens[1].Text, Is.EqualTo("Name(Consts.Temp())"));
        }

        [Test]
        public void BlocksAreEnclosedByDelimiterAndCurlyBrace()
        {
            Parser parser = new Parser();

            parser.Parse("text1|{block}text2");

            Assert.That(parser.Tokens.Count, Is.EqualTo(3));

            Assert.That(parser.Tokens[0].TokenType, Is.EqualTo(TokenType.Text));
            Assert.That(parser.Tokens[0].Text, Is.EqualTo("text1"));

            Assert.That(parser.Tokens[1].TokenType, Is.EqualTo(TokenType.Block));
            Assert.That(parser.Tokens[1].Text, Is.EqualTo("{block}"));

            Assert.That(parser.Tokens[2].TokenType, Is.EqualTo(TokenType.Text));
            Assert.That(parser.Tokens[2].Text, Is.EqualTo("text2"));
        }
    }
}
