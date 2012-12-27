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
using iSynaptic.Commons.Linq;

namespace iSynaptic.Languages.GrammarLanguage.Bootstrap
{
    public abstract class NameSyntax
    {
    }

    public abstract class UnqualifiedNameSyntax : NameSyntax
    {
        public String Identifier { get; set; }
    }

    public sealed class IdentifierNameSyntax : UnqualifiedNameSyntax
    {
        public override string ToString()
        {
            return Identifier;
        }
    }

    public sealed class GenericNameSyntax : UnqualifiedNameSyntax
    {
        private NameSyntax[] _typeArguments;

        public NameSyntax[] TypeArguments { get { return _typeArguments ?? new NameSyntax[0]; } set { _typeArguments = value; } }

        public override string ToString()
        {
            return String.Format("{0}<{1}>", Identifier, TypeArguments.Delimit(", "));
        }
    }

    public sealed class QualifiedNameSyntax : NameSyntax
    {
        public NameSyntax Left { get; set; }
        public UnqualifiedNameSyntax Right { get; set; }

        public override string ToString()
        {
            if(Left != null)
                return String.Format("{0}.{1}", Left, Right);

            return Right.ToString();
        }
    }
}
