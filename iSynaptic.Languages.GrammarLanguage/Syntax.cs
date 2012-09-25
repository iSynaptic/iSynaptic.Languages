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
using iSynaptic.Commons;
using iSynaptic.Languages.GrammarLanguage.SyntacticModel;

using Internal = iSynaptic.Languages.GrammarLanguage.SyntacticModel.Internal;

namespace iSynaptic.Languages.GrammarLanguage
{
    public static class Syntax
    {
        #region Trivia

        public static SyntaxTrivia Whitespace(String text)
        {
            Guard.NotNullOrEmpty(text, "text");

            return new SyntaxTrivia(default(SyntaxToken), new Internal.SyntaxTrivia(SyntaxKind.WhitespaceTrivia, text));
        }

        public static SyntaxTrivia NewLine(String text)
        {
            Guard.NotNullOrEmpty(text, "text");

            return new SyntaxTrivia(default(SyntaxToken), new Internal.SyntaxTrivia(SyntaxKind.NewLineTrivia, text));
        }

        public static SyntaxTrivia SingleLineComment(String text)
        {
            Guard.NotNullOrEmpty(text, "text");

            return new SyntaxTrivia(default(SyntaxToken), new Internal.SyntaxTrivia(SyntaxKind.SingleLineCommentTrivia, text));
        }

        public static SyntaxTrivia MultiLineComment(String text)
        {
            Guard.NotNullOrEmpty(text, "text");

            return new SyntaxTrivia(default(SyntaxToken), new Internal.SyntaxTrivia(SyntaxKind.MultiLineCommentTrivia, text));
        }

        #endregion

        #region Tokens

        public static SyntaxToken Keyword(String keyword)
        {
            var kind = SyntaxFacts.GetKeywordKind(keyword);
            
            if(kind == SyntaxKind.None)
                throw new ArgumentOutOfRangeException("keyword", "Value must be a recognized keyword.");

            return Keyword(kind);
        }

        public static SyntaxToken Keyword(SyntaxKind kind)
        {
            return new SyntaxToken(new Internal.SyntaxToken(kind));
        }

        public static SyntaxToken Identifier(String text)
        {
            return new SyntaxToken(new Internal.SyntaxIdentifier(text));
        }

        #endregion

        #region Nodes

        #endregion
    }
}