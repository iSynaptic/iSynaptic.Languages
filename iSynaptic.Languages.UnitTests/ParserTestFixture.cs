﻿// The MIT License
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
using System.Linq;
using System.Reflection;

namespace iSynaptic.Languages
{
    public abstract class ParserTestFixture
    {
        protected String GetEmbeddedFile(String file)
        {
            return GetEmbeddedFiles(file)
                .FirstOrDefault();
        }

        protected String GetEmbeddedFile(Assembly assembly, String file)
        {
            return GetEmbeddedFiles(assembly, file)
                .FirstOrDefault();
        }

        protected IEnumerable<String> GetEmbeddedFiles(params String[] files)
        {
            return GetEmbeddedFiles(Assembly.GetExecutingAssembly(), files);
        }

        protected IEnumerable<String> GetEmbeddedFiles(Assembly assembly, params String[] files)
        {
            return GetEmbeddedFiles(assembly, x => files.Any(x.EndsWith));
        }

        protected IEnumerable<String> GetEmbeddedFiles(Assembly assembly, Func<String, Boolean> predicate)
        {
            return assembly.GetManifestResourceNames()
                .Where(predicate)
                .Select(assembly.GetManifestResourceStream)
                .Select(s =>
                {
                    using(s)
                    using(var r = new StreamReader(s))
                    {
                        return r.ReadToEnd();
                    }
                });
        }
    }
}
