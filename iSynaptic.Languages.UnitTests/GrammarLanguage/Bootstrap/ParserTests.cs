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
using iSynaptic.Commons;
using iSynaptic.Commons.Linq;

namespace iSynaptic.Languages.GrammarLanguage.Bootstrap
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void Namespace_WithEmptyBody_ReturnsCorrectly()
        {
            var results = Parser.Namespace()(new Input("namespace Foo.Bar.Baz { }"));
            results.WasSuccessful.Should().BeTrue(results.Message);

            results.Value.Name.ToString().Should().Be("Foo.Bar.Baz");
        }

        [Test]
        public void Namespace_WithNestedNamespace_ReturnsCorrectly()
        {
            var results = Parser.Namespace()(new Input("namespace Foo.Bar { namespace Baz { } }"));
            results.WasSuccessful.Should().BeTrue(results.Message);

            results.Value.Members[0].Name.ToString().Should().Be("Baz");
        }

        [Test]
        public void Namespace_WithLanguage()
        {
            var results = Parser.Namespace()(new Input("namespace Foo { language Bar { } }"));
            results.WasSuccessful.Should().BeTrue(results.Message);

            results.Value.Members[0].Name.ToString().Should().Be("Bar");
        }

        [Test]
        public void Namespace_WithLanguageInNestedNamespace()
        {
            var results = Parser.Namespace()(new Input("namespace Foo { namespace Bar { language Baz { } } }"));
            results.WasSuccessful.Should().BeTrue(results.Message);

            results.Value
                .Members[0]
                .As<NamespaceDeclaration>()
                .Members[0]
                .Name.ToString().Should().Be("Baz");
        }

        [Test]
        public void IdentifierName_ReturnsCorrectly()
        {
            var results = Parser.IdentifierName()(new Input("testIdentifier"));
            results.WasSuccessful.Should().BeTrue(results.Message);

            results.Value.Identifier.Should().Be("testIdentifier");
        }

        [Test]
        public void GenericName_ReturnsCorrectly()
        {
            var results = Parser.GenericName()(new Input("TestGeneric<One, Two>"));
            results.WasSuccessful.Should().BeTrue(results.Message);

            var name = results.Value;
            name.Identifier.Should().Be("TestGeneric");
            name.TypeArguments[0].As<IdentifierNameSyntax>().Identifier.Should().Be("One");
            name.TypeArguments[1].As<IdentifierNameSyntax>().Identifier.Should().Be("Two");
        }

        [Test]
        public void GenericName_WithRecursiveGenericNames_ReturnsCorrectly()
        {
            var results = Parser.GenericName()(new Input("TestGeneric<One<Foo>, Two<Bar, Baz>>"));
            results.WasSuccessful.Should().BeTrue(results.Message);

            var name = results.Value;
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
            var results = Parser.UnqualifiedName()(new Input("testIdentifier"));
            results.WasSuccessful.Should().BeTrue(results.Message);

            results.Value.As<IdentifierNameSyntax>().Identifier.Should().Be("testIdentifier");
        }

        [Test]
        public void Name_WithIdentifierNameAsInput_ReturnsCorrectly()
        {
            var results = Parser.Name()(new Input("testIdentifier"));
            results.WasSuccessful.Should().BeTrue(results.Message);

            results.Value.As<IdentifierNameSyntax>().Identifier.Should().Be("testIdentifier");
        }

        [Test]
        public void QualifiedName_WithIdentifierNamesAsInput_ReturnsCorrectly()
        {
            var results = Parser.QualifiedName()(new Input("Foo.Bar.Baz"));
            results.WasSuccessful.Should().BeTrue(results.Message);

            results.Value.Recurse(x => x.Left.ToMaybe().OfType<QualifiedNameSyntax>())
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

            var results = Parser.Name()(new Input(input));
            results.WasSuccessful.Should().BeTrue(results.Message);

            results.Value.ToString().Should().Be(input);
        }
    }
}
