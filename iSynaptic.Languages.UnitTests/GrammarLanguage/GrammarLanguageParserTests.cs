using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Sprache;
using iSynaptic.Languages.GrammarLanguage.Syntax;

namespace iSynaptic.Languages.GrammarLanguage
{
    [TestFixture]
    public class GrammarLanguageParserTests
    {
        private static readonly GrammarLanguageParser Parser = new GrammarLanguageParser();

        [Test]
        public void IdentifierOrKeyword_CanParseSimpleIdentifier()
        {
            var result = Parser.IdentifierOrKeyword(new Input("Foo"));

            var identifierOrKeyword = GetValue(result);

            identifierOrKeyword.Should().Be("Foo");
        }

        [Test]
        public void NamespaceDeclaration_CanParseEmptyNamespace()
        {
            var result = Parser.NamespaceDeclaration(new Input("namespace Foo { }"));

            var ns = GetValue(result);

            ns.Should().NotBeNull();
            ns.Name.Should().Be("Foo");
        }

        [Test]
        public void NamespaceDeclaration_CanParseWithOneLanguage()
        {
            var result = Parser.NamespaceDeclaration(new Input("namespace Foo { language Bar { } }"));

            var ns = GetValue(result);

            ns.Should().NotBeNull();
            ns.Name.Should().Be("Foo");

            ns.Languages.Should().NotBeNull();
            ns.Languages.Should().HaveCount(1);
            ns.Languages.ElementAt(0).Name.Should().Be("Bar");
        }

        private T GetValue<T>(IResult<T> result)
        {
            var success = result as ISuccess<T>;
            if(success != null)
                return success.Result;

            var failure = result as IFailure<T>;
            if(failure != null)
            {
                throw new InvalidOperationException(failure.ToString());
            }

            throw new InvalidOperationException("Unexpected result.");
        }
    }
}
