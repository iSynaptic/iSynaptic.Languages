﻿// The MIT License
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
using iSynaptic.Commons;

namespace iSynaptic.Languages.GrammarLanguage.Syntax
{
    using Parser = GrammarLanguageParser;

    [TestFixture]
    public class GrammarLanguageParserTests
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
        public void NamespaceDeclaration_CanParseWithOneLanguage()
        {
            var result = Parser.NamespaceDeclaration()(new Input("namespace Foo { language Bar { } }"));

            var ns = GetResult(result);

            ns.HasValue.Should().BeTrue();
            ns.Value.Name.Should().Be("Foo");

            ns.Value.Languages.Should().NotBeNull();
            ns.Value.Languages.Should().HaveCount(1);
            ns.Value.Languages.ElementAt(0).Name.Should().Be("Bar");
        }

        private Result<T, string> GetResult<T>(IResult<T> result)
        {
            var success = result as ISuccess<T>;
            if(success != null)
                return success.Result.ToResult();

            var failure = result as IFailure<T>;
            if(failure != null)
            {
                return Result.Failure(failure.ToString());
            }

            return Result.Failure("Unexpected result.");
        }
    }
}