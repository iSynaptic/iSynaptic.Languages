// The MIT License
// 
// Copyright (c) 2013 Jordan E. Terrell
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
using iSynaptic.Commons;
using iSynaptic.Commons.Linq;

namespace iSynaptic.Languages.GrammarLanguage
{
    public abstract class Visitor<TVisitor>
        where TVisitor : Visitor<TVisitor>
    {
        private readonly Stack<Object> _path = new Stack<Object>();

        public virtual void Dispatch<T>(T subject)
            where T : IVisitable<TVisitor>
        {
            Dispatch<T>(new []{subject});
        }

        public void Dispatch<T>(IEnumerable<T> subjects)
            where T : IVisitable<TVisitor>
        {
            Dispatch(subjects, (x, a) => a(x.Value));
        }

        public virtual void Dispatch<T>(IEnumerable<T> subjects, Action<Neighbors<T>, Action<T>> action)
            where T : IVisitable<TVisitor>
        {
            subjects
                .Unless(x => NotInterestedIn(x))
                .WithNeighbors()
                .Run(x =>
                {
                    _path.Push(x.Value);
                    action(x, s => s.Accept((TVisitor) this, AcceptMode.Self));
                    _path.Pop();
                });
        }

        protected virtual bool NotInterestedIn(IVisitable<TVisitor> subject)
        {
            return false;
        }

        public IEnumerable<Object> Path
        {
            get { return _path.Reverse(); }
        }
    }
}
