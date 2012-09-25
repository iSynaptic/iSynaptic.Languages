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

using System;
using FluentAssertions;
using NUnit.Framework;
using Sprache;
using iSynaptic.Commons;
using iSynaptic.Commons.Linq;
using iSynaptic.Languages.GrammarLanguage.SyntacticModel;

// ReSharper disable InconsistentNaming

namespace iSynaptic.Languages.GrammarLanguage
{
    [TestFixture]
    public class ExampleGrammarTests : ParserTestFixture
    {
        private readonly String _Input;
        private readonly Result<NamespaceDeclarationSyntax, String> _ParseResult;

        public ExampleGrammarTests()
        {
            _Input = GetEmbeddedFile("Example.grammar");
            _ParseResult = GetResult(Parser.NamespaceDeclaration()(new Input(_Input)));
        }

        [Test]
        public void Input_IsNotNullOrZeroLength()
        {
            _Input.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void ParseResult_WasSuccessful()
        {
            _ParseResult.WasSuccessful.Should().BeTrue(_ParseResult.Observations.Delimit(Environment.NewLine));
        }

        [Test]
        public void ParseResult_HasValue()
        {
            _ParseResult.HasValue.Should().BeTrue();
        }

        private NamespaceDeclarationSyntax Result
        {
            get { return _ParseResult.Value; }
        }
    }
}