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
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using iSynaptic.Commons;
using iSynaptic.Commons.Linq;

namespace iSynaptic.Languages.GrammarLanguage.SyntacticModel.Internal
{
    [TestFixture]
    public class DesignTests
    {
        [Test]
        public void AllTypesMustBeInternal()
        {

            var outcome = GetInternalTypes()
                .Select(x => Outcome.FailIf(x.IsPublic, x.FullName))
                .Combine();

            if(!outcome.WasSuccessful)
            {
                Assert.Fail("The following types are public:\r\n{0}", outcome.Observations.Delimit(Environment.NewLine));
            }
        }

        [Test]
        public void AllFieldsMustBeReadOnly()
        {
            var writableFields = GetInternalTypes()
                .SelectMany(x => x.GetFields(BindingFlags.Public | BindingFlags.NonPublic))
                .Where(f => !f.IsInitOnly)
                .Select(x => String.Format("{0}.{1}", x.DeclaringType.FullName, x.Name))
                .ToArray();

            if (writableFields.Length > 0)
            {
                Assert.Fail("The following fields are writable:\r\n{0}", writableFields.Delimit(Environment.NewLine));
            }
        }

        private IEnumerable<Type> GetInternalTypes()
        {
            const String internalNamespace = "iSynaptic.Languages.GrammarLanguage.SyntacticModel.Internal";

            return typeof (SyntaxNode)
                .Assembly
                .GetTypes()
                .Where(x => x.Namespace != null && x.Namespace.StartsWith(internalNamespace));
        }
    }
}
