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

namespace iSynaptic.Languages.GrammarLanguage.SyntacticModel.Internal
{
    internal class SyntaxToken : SyntaxNode
    {
        private readonly Int32 _FullWidth;

        public SyntaxToken(SyntaxKind kind)
            : base(kind)
        {
            _FullWidth = SyntaxFacts.GetText(kind).Length;
        }

        public virtual String Text
        {
            get { return SyntaxFacts.GetText(Kind); }
        }

        public override int FullWidth { get { return _FullWidth; } }
    }

    internal class SyntaxIdentifier : SyntaxToken
    {
        private readonly String _Text;
        private readonly Int32 _FullWidth;

        public SyntaxIdentifier(String text)
            : base(SyntaxKind.IdentifierToken)
        {
            _Text = text;
            _FullWidth = text.Length;
        }

        public override String Text { get { return _Text; } }
        public override int FullWidth { get { return _FullWidth; } }
    }
}
