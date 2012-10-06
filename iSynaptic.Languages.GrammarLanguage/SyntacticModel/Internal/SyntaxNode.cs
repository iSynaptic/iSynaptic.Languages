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
    internal abstract class SyntaxNode
    {
        public readonly SyntaxKind Kind;

        protected SyntaxNode(SyntaxKind kind)
        {
            Kind = kind;
        }

        internal virtual Int32 GetSlotCount()
        {
            return 0;
        }

        internal virtual SyntaxNode GetSlot(Int32 index)
        {
            return null;
        }

        public abstract Int32 FullWidth { get; }

        public virtual Int32 LeadingWidth
        {
            get 
            { 
                var token = GetFirstToken(true);
                return token != null 
                    ? token.LeadingWidth 
                    : 0;
            }
        }

        public virtual Int32 TrailingWidth
        {
            get
            {
                var token = GetLastToken(true);
                return token != null
                    ? token.TrailingWidth
                    : 0;
            }
        }

        public virtual Int32 Width
        {
            get { return FullWidth - LeadingWidth - TrailingWidth; }
        }

        public SyntaxToken GetFirstToken()
        {
            return GetFirstToken(false);
        }

        public SyntaxToken GetFirstToken(bool includeZeroWidth)
        {
            Int32 slotCount = GetSlotCount();
            Int32 index = 0;

            while (index < slotCount)
            {
                var token = GetSlot(index) as SyntaxToken;
                if(token != null && (includeZeroWidth || token.FullWidth != 0))
                    return token;

                index++;
            }

            return null;
        }

        public SyntaxToken GetLastToken()
        {
            return GetLastToken(false);
        }

        public SyntaxToken GetLastToken(bool includeZeroWidth)
        {
            Int32 slotCount = GetSlotCount();
            Int32 index = slotCount - 1;

            while (index >= 0)
            {
                var token = GetSlot(index) as SyntaxToken;
                if (token != null && (includeZeroWidth || token.FullWidth != 0))
                    return token;

                index--;
            }

            return null;
        }
    }
}
