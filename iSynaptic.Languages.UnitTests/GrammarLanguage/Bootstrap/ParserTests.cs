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
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using iSynaptic.Commons;
using iSynaptic.Commons.Linq;

namespace iSynaptic.Languages.GrammarLanguage.Bootstrap
{
    [TestFixture]
    public class ParserTests : ParserTestFixture
    {
        [Test]
        public void FullGrammarParseTest()
        {
            string input = GetEmbeddedFile(typeof (Parser).Assembly, "GrammarLanguage.grammar");
            var value = Parser.Start().SuccessfullyParse(input);
            value.Should().NotBeNull();
        }

        [Test]
        public void Namespace_WithEmptyBody_ReturnsCorrectly()
        {
            var value = Parser.Namespace().SuccessfullyParse("namespace Foo.Bar.Baz { }");
            value.Name.ToString().Should().Be("Foo.Bar.Baz");
        }

        [Test]
        public void Namespace_WithNestedNamespace_ReturnsCorrectly()
        {
            var value = Parser.Namespace().SuccessfullyParse("namespace Foo.Bar { namespace Baz { } }");
            value.Members[0].Name.ToString().Should().Be("Baz");
        }

        [Test]
        public void Namespace_WithLanguage()
        {
            var value = Parser.Namespace().SuccessfullyParse("namespace Foo { language Bar { } }");
            value.Members[0].Name.ToString().Should().Be("Bar");
        }

        [Test]
        public void Namespace_WithLanguageInNestedNamespace()
        {
            var value = Parser.Namespace().SuccessfullyParse("namespace Foo { namespace Bar { language Baz { } } }");
            
            value
                .Members[0]
                .As<NamespaceDeclaration>()
                .Members[0]
                .Name.ToString().Should().Be("Baz");
        }

        [Test]
        public void Language_ReturnsCorrectly()
        {
            var value = Parser.Language().SuccessfullyParse("language Foo { }");
            value.Name.ToString().Should().Be("Foo");
        }

        [Test]
        public void Language_WithLiteralTokenMember_ReturnsCorrectly()
        {
            var value = Parser.Language().SuccessfullyParse("language Foo { token Bar = \"Baz\"; }");

            var token = value
                .Members[0]
                .As<TokenDeclaration>();

            token.Name.ToString().Should().Be("Bar");
            token.Expression.As<LiteralTokenExpression>().Literal.Should().Be("Baz");
        }
        
        [Test]
        public void NodeMember_ReturnsCorrectly()
        {
            var cardinalities = new Dictionary<TypeCardinality, String>
            {
                {TypeCardinality.One, ""},
                {TypeCardinality.OneOrMore, "+"},
                {TypeCardinality.ZeroOrMore, "*"},
                {TypeCardinality.ZeroOrOne, "?"}
            };

            var combinations = from cardinality in cardinalities.Keys
                               from hasName in new[] {true, false}
                               let value = Parser.NodeMember().SuccessfullyParse(
                                   String.Format("Identifier{0}{1}", cardinalities[cardinality], hasName ? " name" : ""))
                               select new {Value = value, Cardinality = cardinality, HasName = hasName};


            foreach (var combo in combinations)
            {
                combo.Value.Type.Name.ToString().Should().Be("Identifier");
                combo.Value.Type.Cardinality.Should().Be(combo.Cardinality);
                combo.Value.Name.HasValue.Should().Be(combo.HasName);

                if (combo.HasName)
                    combo.Value.Name.Value.ToString().Should().Be("name");
            }
        }

        [Test]
        public void IdentifierName_ReturnsCorrectly()
        {
            var value = Parser.IdentifierName().SuccessfullyParse("testIdentifier");
            value.Identifier.Should().Be("testIdentifier");
        }

        [Test]
        public void GenericName_ReturnsCorrectly()
        {
            var name = Parser.GenericName().SuccessfullyParse("TestGeneric<One, Two>");
            
            name.Identifier.Should().Be("TestGeneric");
            name.TypeArguments[0].As<IdentifierNameSyntax>().Identifier.Should().Be("One");
            name.TypeArguments[1].As<IdentifierNameSyntax>().Identifier.Should().Be("Two");
        }

        [Test]
        public void GenericName_WithRecursiveGenericNames_ReturnsCorrectly()
        {
            var name = Parser.GenericName().SuccessfullyParse("TestGeneric<One<Foo>, Two<Bar, Baz>>");
            
            name.Identifier.Should().Be("TestGeneric");

            var one = name.TypeArguments[0].As<GenericNameSyntax>();
            one.Identifier.Should().Be("One");
            one.TypeArguments[0].As<IdentifierNameSyntax>().Identifier.Should().Be("Foo");

            var two = name.TypeArguments[1].As<GenericNameSyntax>();
            two.Identifier.Should().Be("Two");
            two.TypeArguments[0].As<IdentifierNameSyntax>().Identifier.Should().Be("Bar");
            two.TypeArguments[1].As<IdentifierNameSyntax>().Identifier.Should().Be("Baz");
        }

        [Test]
        public void UnqualifiedName_WithIdentifierNameAsInput_ReturnsCorrectly()
        {
            var value = Parser.UnqualifiedName().SuccessfullyParse("testIdentifier");
            value.As<IdentifierNameSyntax>().Identifier.Should().Be("testIdentifier");
        }

        [Test]
        public void Name_WithIdentifierNameAsInput_ReturnsCorrectly()
        {
            var value = Parser.Name().SuccessfullyParse("testIdentifier");
            value.As<IdentifierNameSyntax>().Identifier.Should().Be("testIdentifier");
        }

        [Test]
        public void QualifiedName_WithIdentifierNamesAsInput_ReturnsCorrectly()
        {
            var value = Parser.QualifiedName().SuccessfullyParse("Foo.Bar.Baz");
            value.Recurse(x => x.Left.ToMaybe().OfType<QualifiedNameSyntax>())
                   .Select(x => x.Right)
                   .Select(x => x.Identifier)
                   .Reverse()
                   .SequenceEqual(new[] {"Foo", "Bar", "Baz"})
                   .Should().BeTrue();
        }

        [Test]
        public void Name_WithComplexInput_WasSuccessful()
        {
            string input = "NS1.NS2.NS3.Type1<NS1.NS2.NS3.Type2<NS1.NS2.NS3.Type4>, NS1.NS2.NS3.Type3<Type5, NS1.Type6>>";

            var value = Parser.Name().SuccessfullyParse(input);
            value.ToString().Should().Be(input);
        }
    }
}
