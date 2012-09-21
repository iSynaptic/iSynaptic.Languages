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

using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Sprache;
using iSynaptic.Commons.Linq;

namespace iSynaptic.Languages.GrammarLanguage.Syntax
{
    using Parser = GrammarLanguageParser;

    [TestFixture]
    public class GrammarLanguageParserTests : ParserTestFixture
    {
        [Test]
        public void Identifier_CanParseSimpleIdentifier()
        {
            var result = Parser.Identifier()(new Input("Foo"));

            var identifier = GetResult(result);

            identifier.HasValue.Should().BeTrue();
            identifier.Value.Should().Be("Foo");
        }

        [Test]
        public void Identifier_CannotParseKeywordWithoutLiteralPrefix()
        {
            var result = Parser.Identifier()(new Input("namespace"));

            var identifier = GetResult(result);

            identifier.HasValue.Should().BeFalse();
            identifier.WasSuccessful.Should().BeFalse();
        }

        [Test]
        public void Identifier_CanParseKeywordWithiteralPrefix()
        {
            var result = Parser.Identifier()(new Input("@namespace"));

            var identifier = GetResult(result);

            identifier.HasValue.Should().BeTrue();
            identifier.Value.Should().Be("namespace");
        }

        [Test]
        public void IdentifierOrKeyword_CanParseSimpleIdentifier()
        {
            var result = Parser.IdentifierOrKeyword()(new Input("Foo"));

            var identifierOrKeyword = GetResult(result);

            identifierOrKeyword.HasValue.Should().BeTrue();
            identifierOrKeyword.Value.Should().Be("Foo");
        }

        [Test]
        public void NamespaceDeclaration_CanParseEmptyNamespace()
        {
            var result = Parser.NamespaceDeclaration()(new Input("namespace Foo { }"));

            var ns = GetResult(result);

            ns.HasValue.Should().BeTrue();
            ns.Value.Name.Should().Be("Foo");
        }

        [Test]
        public void NamespaceDeclaration_CanParseWithInnerSingleLineComment()
        {
            var result = Parser.NamespaceDeclaration()(new Input(@"namespace Foo {
    // This is a single line comment.
}"));

            var ns = GetResult(result);

            ns.WasSuccessful.Should().BeTrue(ns.Observations.Delimit("\r\n"));
            ns.HasValue.Should().BeTrue("no value");
            ns.Value.Name.Should().Be("Foo");
        }

        [Test]
        public void NamespaceDeclaration_CanParseWithOneLanguage()
        {
            var result = Parser.NamespaceDeclaration()(new Input("namespace Foo { language Bar { } }"));

            var ns = GetResult(result);

            ns.WasSuccessful.Should().BeTrue(ns.Observations.Delimit("\r\n"));
            ns.HasValue.Should().BeTrue("no value");
            ns.Value.Name.Should().Be("Foo");

            ns.Value.Languages.Should().NotBeNull();
            ns.Value.Languages.Should().HaveCount(1, "missing language");
            ns.Value.Languages.ElementAt(0).Name.Should().Be("Bar");
        }

        [Test]
        public void SingleLineComment_CanParse()
        {
            var result = Parser.SingleLineComment()(new Input("//This is a test."));

            var ns = GetResult(result);

            ns.WasSuccessful.Should().BeTrue(ns.Observations.Delimit("\r\n"));
            ns.HasValue.Should().BeTrue("no value");
            ns.Value.Should().Be("This is a test.");
        }

        [Test]
        public void DelimitedComment_CanParse()
        {
            var result = Parser.DelimitedComment()(new Input("/*This is a test.*/"));

            var ns = GetResult(result);

            ns.WasSuccessful.Should().BeTrue(ns.Observations.Delimit("\r\n"));
            ns.HasValue.Should().BeTrue("no value");
            ns.Value.Should().Be("This is a test.");
        }

        [Test]
        public void DelimitedComment_CanParseMultiLineComment()
        {
            var result = Parser.DelimitedComment()(new Input(@"/*This is a test.
This is another test.*/"));

            var ns = GetResult(result);

            ns.WasSuccessful.Should().BeTrue(ns.Observations.Delimit("\r\n"));
            ns.HasValue.Should().BeTrue("no value");
            ns.Value.Should().Be("This is a test.\r\nThis is another test.");
        }

        [Test]
        public void DelimitedComment_CanParseMultiLineCommentWithAsterisksAtTheBeginningOfEachLine()
        {
            var result = Parser.DelimitedComment()(new Input(@"/*This is a test.
*This is another test.
*This is test three.*/"));

            var ns = GetResult(result);

            ns.WasSuccessful.Should().BeTrue(ns.Observations.Delimit("\r\n"));
            ns.HasValue.Should().BeTrue("no value");
            ns.Value.Should().Be("This is a test.\r\nThis is another test.\r\nThis is test three.");
        }

        [Test]
        public void WhiteSpace_CanParseSomeWhiteSpace()
        {
            string input = "  \t    \t   ";
            var result = Parser.WhiteSpace()(new Input(input));

            var ns = GetResult(result);

            ns.WasSuccessful.Should().BeTrue(ns.Observations.Delimit("\r\n"));
            ns.HasValue.Should().BeTrue("no value");
            ns.Value.Should().Be(input);
        }

        [Test]
        public void Interleave_ShouldDiscardWhiteSpaceAndSingleLineComment()
        {
            string input = @"       Foo   
// This is a comment.
          ";

            var rule = Parse.String("Foo").Text().Interleave().End();

            var result = rule(new Input(input));

            var ns = GetResult(result);

            ns.WasSuccessful.Should().BeTrue(ns.Observations.Delimit("\r\n"));
            ns.HasValue.Should().BeTrue("no value");
            ns.Value.Should().Be("Foo");
        }
    }
}
