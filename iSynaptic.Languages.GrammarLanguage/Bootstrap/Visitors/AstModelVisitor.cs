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

using System.Linq;
using iSynaptic.Commons.Linq;

namespace iSynaptic.Languages.GrammarLanguage.Bootstrap.Visitors
{
    public class AstModelVisitor : GrammarLanguageVisitor
    {
        public override void Visit(NamespaceDeclaration @namespace)
        {
            SetFileName("SyntaxKind.cs");
            @namespace.Accept(this, AcceptMode.Children);
        }

        public override void Visit(LanguageDeclaration language)
        {
            using (WriteBlock("namespace {0}.{1}.SyntacticModel", Path.OfType<NamespaceDeclaration>().Delimit(".", x => x.Name.ToString()), language.Name))
            using (WriteBlock("public enum SyntaxKind : short"))
            {
                Delimit(language.Members, ",\r\n");
                WriteLine();
            }
        }

        public override void Visit(TriviaDeclaration trivia)
        {
            Write("{0}Trivia", trivia.Name);
        }

        public override void Visit(TokenDeclaration token)
        {
            Write(!token.Name.ToString().EndsWith("Keyword") 
                      ? "{0}Token" 
                      : "{0}", token.Name);
        }

        public override void Visit(NodeDeclaration node)
        {
            Write("{0}", node.Name);
        }
    }
}