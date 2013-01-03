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

using System.Collections.Generic;

namespace iSynaptic.Languages.GrammarLanguage.Bootstrap
{
    public class SyntaxTree : IVisitable<GrammarLanguageVisitor>
    {
        private List<UsingStatement> _usings;
        private List<NamespaceDeclaration> _namespaces;

        public List<UsingStatement> Usings { get { return _usings ?? (_usings = new List<UsingStatement>()); } set { _usings = value; } }
        public List<NamespaceDeclaration> Namespaces { get { return _namespaces ?? (_namespaces = new List<NamespaceDeclaration>()); } set { _namespaces = value; } }

        public void Accept(GrammarLanguageVisitor visitor, AcceptMode mode)
        {
            if(mode == AcceptMode.Self)
                visitor.Visit(this);

            if(mode == AcceptMode.Children)
                visitor.Dispatch<NamespaceDeclaration>(Namespaces);
        }
    }
}
