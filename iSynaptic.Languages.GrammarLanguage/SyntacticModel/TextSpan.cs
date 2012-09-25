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

namespace iSynaptic.Languages.GrammarLanguage.SyntacticModel
{
    public struct TextSpan
    {
        private readonly Int32 _Start;
        private readonly Int32 _Length;

        public TextSpan(Int32 start, Int32 length)
        {
            if(start < 0) throw new ArgumentOutOfRangeException("start", "Text span start must be greater than or equal to zero (0).");
            if(length < 0) throw new ArgumentOutOfRangeException("length", "Text span length must be greater than or equal to zero (0).");

            _Start = start;
            _Length = length;
        }

        public Int32 Start { get { return _Start; } }
        public Int32 Length { get { return _Length; } }

        public Int32 End { get { return Start + Length; }}
    }
}
