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
using System.IO;
using System.Text;
using iSynaptic.Commons;

namespace iSynaptic.Languages.GrammarLanguage
{
    public class IndentingTextWriter : TextWriter
    {
        private readonly TextWriter _underlying;
        private readonly string _indentWith;

        private int _matchIndex;
        private int _indentationLevel;

        public IndentingTextWriter(TextWriter underlying, string indentWith)
        {
            _underlying = Guard.NotNull(underlying, "underlying");
            _indentWith = Guard.NotNullOrEmpty(indentWith, "indentWith");
        }

        public override void Write(char value)
        {
            if (_matchIndex == Environment.NewLine.Length)
            {
                _matchIndex = 0;
                WriteIndentation();
            }

            if (Environment.NewLine[_matchIndex] == value)
                _matchIndex++;
            else
                _matchIndex = 0;

            _underlying.Write(value);
        }

        private void WriteIndentation()
        {
            for (int i = 0; i < _indentationLevel; i++)
            {
                _underlying.Write(_indentWith);
            }
        }

        public void IncreaseIndent()
        {
            if (_indentationLevel < Int32.MaxValue)
                _indentationLevel++;
        }

        public void DecreaseIndent()
        {
            if (_indentationLevel > 0)
                _indentationLevel--;
        }

        public override Encoding Encoding
        {
            get { return _underlying.Encoding; }
        }
    }
}
