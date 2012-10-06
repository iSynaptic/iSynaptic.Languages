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
using FluentAssertions;
using NUnit.Framework;

namespace iSynaptic.Languages.GrammarLanguage.SyntacticModel.Internal
{
    [TestFixture]
    public class SyntaxNodeTests
    {
        private class TestNode : SyntaxNode
        {
            private readonly Int32 _FullWidth;

            private readonly SyntaxToken _NamespaceKeyword;
            private readonly SyntaxIdentifier _Name;

            public TestNode() 
                : base(SyntaxKind.NamespaceDeclaration)
            {
                _NamespaceKeyword = new SyntaxToken(SyntaxKind.NamespaceKeyword);
                _Name = new SyntaxIdentifier("Foo");

                _FullWidth = _NamespaceKeyword.FullWidth +
                             _Name.FullWidth;
            }

            internal override int GetSlotCount()
            {
                return 2;
            }

            internal override SyntaxNode GetSlot(int index)
            {
                if(index == 0)
                    return _NamespaceKeyword;

                if (index == 1)
                    return _Name;

                return null;
            }

            public override int FullWidth
            {
                get { return _FullWidth; }
            }

            public SyntaxToken NamespaceKeyword { get { return _NamespaceKeyword; } }
            public SyntaxIdentifier Name { get { return _Name; } }
        }

        private readonly TestNode _Node = new TestNode();

        [Test]
        public void GetFirstToken_OnNode_ReturnsNamespaceKeyword()
        {
            var keyword = _Node.GetFirstToken();

            keyword.Should().Be(_Node.NamespaceKeyword);
        }

        [Test]
        public void GetLastToken_OnNode_ReturnsNameIdentifier()
        {
            var name = _Node.GetLastToken();

            name.Should().Be(_Node.Name);
        }

        [Test]
        public void GetFirstToken_OnToken_ReturnsNull()
        {
            _Node.NamespaceKeyword.GetFirstToken()
                .Should().BeNull();
        }

        [Test]
        public void GetLastToken_OnToken_ReturnsNull()
        {
            _Node.NamespaceKeyword.GetLastToken()
                .Should().BeNull();
        }

        [Test]
        public void GetFirstToken_OnTrivia_RetunsNull()
        {
            new SyntaxTrivia(SyntaxKind.WhitespaceTrivia, " ")
                .GetFirstToken().Should().BeNull();
        }

        [Test]
        public void GetLastToken_OnTrivia_RetunsNull()
        {
            new SyntaxTrivia(SyntaxKind.WhitespaceTrivia, " ")
                .GetLastToken().Should().BeNull();
        }
    }
}
