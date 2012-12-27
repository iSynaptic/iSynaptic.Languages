// The MIT License
// 
// Copyright (c) 2012 Jordan E. Terrell
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Sprache;
using iSynaptic.Commons.Linq;
using iSynaptic.Languages.GrammarLanguage.SyntacticModel;

// ReSharper disable InconsistentNaming

namespace iSynaptic.Languages.GrammarLanguage
{
    [TestFixture]
    public class ParserTests : ParserTestFixture
    {
        [Test]
        public void Identifier_CanParseSimpleIdentifier()
        {
            var identifier = Parser.Identifier()(new Input("Foo"));

            identifier.WasSuccessful.Should().BeTrue(identifier.Expectations.Delimit("\r\n"));
            identifier.Value.Kind.Should().Be(SyntaxKind.IdentifierToken);
            identifier.Value.Text.Should().Be("Foo");
        }

        [Test]
        public void Identifier_CannotParseKeywordWithoutLiteralPrefix()
        {
            var identifier = Parser.Identifier()(new Input("namespace"));

            identifier.WasSuccessful.Should().BeFalse();
        }

        [Test]
        public void Identifier_CanParseKeywordWithLiteralPrefix()
        {
            var identifier = Parser.Identifier()(new Input("@" + SyntaxFacts.NamespaceKeyword));

            identifier.WasSuccessful.Should().BeTrue(identifier.Expectations.Delimit("\r\n"));
            identifier.Value.Kind.Should().Be(SyntaxKind.IdentifierToken);
            identifier.Value.Text.Should().Be("@" + SyntaxFacts.NamespaceKeyword);
        }

        [Test]
        public void IdentifierOrKeyword_CanParseSimpleIdentifier()
        {
            var identifierOrKeyword = Parser.IdentifierOrKeyword()(new Input("Foo"));

            identifierOrKeyword.WasSuccessful.Should().BeTrue();
            identifierOrKeyword.Value.Kind.Should().Be(SyntaxKind.IdentifierToken);
            identifierOrKeyword.Value.Text.Should().Be("Foo");
        }

        [Test]
        public void NamespaceDeclaration_CanParseEmptyNamespace()
        {
            var ns = Parser.NamespaceDeclaration()(new Input("namespace Foo { }"));

            ns.WasSuccessful.Should().BeTrue();
            ns.Value.Name.Text.Should().Be("Foo");
        }

        [Test]
        public void NamespaceDeclaration_CanParseWithInnerSingleLineComment()
        {
            var ns = Parser.NamespaceDeclaration()(new Input(@"namespace Foo {
    // This is a single line comment.
}"));

            ns.WasSuccessful.Should().BeTrue(ns.Expectations.Delimit("\r\n"));
            ns.Value.Name.Text.Should().Be("Foo");
        }

        [Test]
        public void NamespaceDeclaration_CanParseWithOneLanguage()
        {
            var ns = Parser.NamespaceDeclaration()(new Input("namespace Foo { language Bar { } }"));


            ns.WasSuccessful.Should().BeTrue(ns.Expectations.Delimit("\r\n"));
            ns.Value.Name.Text.Should().Be("Foo");

            ns.Value.Languages.Should().NotBeNull();
            ns.Value.Languages.Should().HaveCount(1, "missing language");
            ns.Value.Languages.ElementAt(0).Name.Text.Should().Be("Bar");
        }

        [Test]
        public void SingleLineComment_CanParse()
        {
            String input = "// This is a test.";
            var ns = Parser.SingleLineComment()(new Input(input));

            ns.WasSuccessful.Should().BeTrue(ns.Expectations.Delimit("\r\n"));
            ns.Value.Text.Should().Be(input);
        }

        [Test]
        public void MultiLineComment_CanParse()
        {
            String input = "/*This is a test.****/";

            var ns = Parser.MultiLineComment()(new Input(input));

            ns.WasSuccessful.Should().BeTrue(ns.Expectations.Delimit("\r\n"));
            ns.Value.Text.Should().Be(input);
        }

        [Test]
        public void MultiLineComment_CanParseMultiLineComment()
        {
            String input = @"/*This is a test.
This is another test.*/";

            var ns = Parser.MultiLineComment()(new Input(input));

            ns.WasSuccessful.Should().BeTrue(ns.Expectations.Delimit("\r\n"));
            ns.Value.Text.Should().Be(input);
        }

        [Test]
        public void MultiLineComment_CanParseMultiLineCommentWithAsterisksAtTheBeginningOfEachLine()
        {
            String input = @"/*This is a test.
*This is another test.
*This is test three.*/";

            var ns = Parser.MultiLineComment()(new Input(input));


            ns.WasSuccessful.Should().BeTrue(ns.Expectations.Delimit("\r\n"));
            ns.Value.Text.Should().Be(input);
        }

        [Test]
        public void Whitespace_CanParseSomeWhitespace()
        {
            string input = "  \t    \t   ";
            var ns = Parser.Whitespace()(new Input(input));

            ns.WasSuccessful.Should().BeTrue(ns.Expectations.Delimit("\r\n"));
            ns.Value.Text.Should().Be(input);
        }

        [Test]
        public void Interleave_ShouldDiscardWhitespaceAndSingleLineComment()
        {
            string input = @"       Foo   
// This is a comment.
          ";

            var rule = Parse.String("Foo").Text().Interleave().End();

            var ns = rule(new Input(input));

            ns.WasSuccessful.Should().BeTrue(ns.Expectations.Delimit("\r\n"));
            ns.Value.Should().Be("Foo");
        }
    }
}
