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
using System.IO;
using iSynaptic.Commons;
using iSynaptic.Commons.Collections.Generic;
using iSynaptic.Commons.Linq;

namespace iSynaptic.Languages.GrammarLanguage
{
    public abstract class CodeAuthoringVisitor<TVisitor> : Visitor<TVisitor>
        where TVisitor : CodeAuthoringVisitor<TVisitor>
    {
        private readonly List<String> _usings = new List<String>();
        private readonly LazySelectionDictionary<String, CodeBlock> _codeBlocks
            = new LazySelectionDictionary<string, CodeBlock>(x => new CodeBlock(x).ToMaybe());

        private CodeBlock _currentCodeBlock;
        
        protected CodeAuthoringVisitor()
        {
            AddUsings();
        }

        protected void SetFileName(String fileName)
        {
            Guard.NotNullOrWhiteSpace(fileName, "fileName");

            _currentCodeBlock = _codeBlocks[fileName];
        }

        protected virtual void AddUsings()
        {
            AddUsing("System");
            AddUsing("System.Collections.Generic");
            AddUsing("System.ComponentModel");
            AddUsing("System.Linq");
        }

        protected void AddUsing(String identifier)
        {
            _usings.Add(identifier);
        }

        protected virtual IDisposable WithBlock(bool withStatementEnd = false)
        {
            WriteLine("{");
            var indentation = WithIndentation();

            Action onDispose = () =>
            {
                indentation.Dispose();
                Write("}");

                if (withStatementEnd)
                    WriteLine(";");
                else
                    WriteLine();
            };

            return onDispose.ToDisposable();
        }

        protected IDisposable WithIndentation()
        {
            Writer.IncreaseIndent();
            return ((Action)Writer.DecreaseIndent).ToDisposable();
        }

        protected virtual IDisposable WriteBlock(string formatString, params object[] args)
        {
            WriteLine(formatString, args);
            return WithBlock();
        }

        protected virtual void WriteUsings()
        {
            WriteLine(_usings.Delimit("\r\n", x => String.Format("using {0};", x)));
            WriteLine();
        }

        protected void Write(string text)
        {
            Writer.Write(text);
        }

        protected virtual void Write(string formatString, params object[] args)
        {
            Write(string.Format(formatString, args));
        }

        protected virtual void WriteLine()
        {
            Write(Environment.NewLine);
        }

        protected virtual void WriteLine(string text)
        {
            Write(text);
            WriteLine();
        }

        protected virtual void WriteLine(string formatString, params object[] args)
        {
            Write(string.Format(formatString, args));
            WriteLine();
        }

        protected virtual void IncreaseIndent()
        {
            Writer.IncreaseIndent();
        }

        protected virtual void DecreaseIndent()
        {
            Writer.DecreaseIndent();
        }

        protected virtual void Delimit<T>(IEnumerable<T> subjects, String delimiter)
            where T : IVisitable<TVisitor>
        {
            Delimit(subjects, (t1, t2) => Write(delimiter));
        }

        protected virtual void Delimit<T>(IEnumerable<T> subjects, Func<T, T, String> delimiter)
            where T : IVisitable<TVisitor>
        {
            Delimit(subjects, (t1, t2) => Write(delimiter(t1, t2)));
        }

        protected virtual void Delimit<T>(IEnumerable<T> subjects, Action<T, T> action)
            where T : IVisitable<TVisitor>
        {
            Dispatch(subjects, (x, a) =>
            {
                a(x.Value);
                x.Next.Run(y => action(x.Value, y));
            });
        }

        public IEnumerable<CodeBlock> CodeBlocks { get { return _codeBlocks.Values; } }
        private IndentingTextWriter Writer
        {
            get
            {
                if(_currentCodeBlock == null)
                    throw new InvalidOperationException("You must set a file name before writing text output.");

                return _currentCodeBlock.Writer;
            }
        }
    }
}
