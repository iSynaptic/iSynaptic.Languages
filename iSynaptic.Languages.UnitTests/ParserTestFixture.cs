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
using System.Linq;
using System.Reflection;
using Sprache;
using iSynaptic.Commons;

namespace iSynaptic.Languages
{
    public abstract class ParserTestFixture
    {
        protected String GetEmbeddedFile(String file)
        {
            return GetEmbeddedFiles(file)
                .FirstOrDefault();
        }

        protected IEnumerable<String> GetEmbeddedFiles(params String[] files)
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            return files
                .Select(f => asm.GetManifestResourceStream(String.Format("{0}.{1}", GetType().Namespace, f)))
                .Select(s => new StreamReader(s).ReadToEnd());
        }

        protected Result<T, string> GetResult<T>(IResult<T> result)
        {
            var success = result as ISuccess<T>;
            if (success != null)
                return success.Result.ToResult();

            var failure = result as IFailure<T>;
            if (failure != null)
            {
                return Result.Failure(failure.ToString());
            }

            return Result.Failure("Unexpected result.");
        }
    }
}
