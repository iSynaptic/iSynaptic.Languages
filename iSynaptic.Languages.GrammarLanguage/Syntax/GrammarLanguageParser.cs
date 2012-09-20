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
using System.Globalization;
using Sprache;
using iSynaptic.Commons;
using System.Linq;

namespace iSynaptic.Languages.GrammarLanguage.Syntax
{
    public static class GrammarLanguageParser
    {
        public static readonly String BaseKeyword = "base";
        public static readonly String UsingKeyword = "using";

        public static readonly String NamespaceKeyword = "namespace";
        public static readonly String LanguageKeyword = "language";
        public static readonly String InterleaveKeyword = "interleave";

        public static IEnumerable<String> Keywords()
        {
            return new[]
            {
                BaseKeyword,
                UsingKeyword,
                NamespaceKeyword,
                LanguageKeyword,
                InterleaveKeyword
            };
        }

        public static Parser<String> InheritsOperator() { return Parse.String(":").Text(); }

        public static Parser<String> BlockStart() { return Parse.String("{").Text().Named("block start"); }
        public static Parser<String> BlockEnd() { return Parse.String("}").Text().Named("block end"); }
        public static Parser<String> StatementEnd() { return Parse.String(";").Text(); }

        public static Parser<NamespaceDeclaration> NamespaceDeclaration()
        {
            return BlockedConcept(
                NamespaceKeyword,
                LanguageDeclaration().Many(),
                (id, langs) => new NamespaceDeclaration(id, langs));
        }

        public static Parser<LanguageDeclaration> LanguageDeclaration()
        {
            return BlockedConceptWithInheritance(
                LanguageKeyword,
                Parse.Return(new Unit()),
                (id, baseLanguage, u) => new LanguageDeclaration(id))
                .Named("language");
        }

        public static Parser<Char> LetterCharacter()
        {
            return CharacterByUnicodeCategory(
                UnicodeCategory.UppercaseLetter,
                UnicodeCategory.LowercaseLetter,
                UnicodeCategory.TitlecaseLetter,
                UnicodeCategory.ModifierLetter,
                UnicodeCategory.OtherLetter,
                UnicodeCategory.LetterNumber);
        }

        public static Parser<Char> DecimalDigitCharacter()
        {
            return CharacterByUnicodeCategory(
                UnicodeCategory.DecimalDigitNumber);
        }

        public static Parser<Char> ConnectingCharacter()
        {
            return CharacterByUnicodeCategory(
                UnicodeCategory.ConnectorPunctuation);
        }

        public static Parser<Char> CombiningCharacter()
        {
            return CharacterByUnicodeCategory(
                UnicodeCategory.NonSpacingMark,
                UnicodeCategory.SpacingCombiningMark);
        }

        public static Parser<Char> FormattingCharacter()
        {
            return CharacterByUnicodeCategory(
                UnicodeCategory.Format);
        }

        public static Parser<Char> IdentifierPartCharacter()
        {
            return LetterCharacter()
                .Or(DecimalDigitCharacter())
                .Or(ConnectingCharacter())
                .Or(CombiningCharacter())
                .Or(FormattingCharacter());
        }

        public static Parser<Char> IdentifierStartCharacter()
        {
            return LetterCharacter()
                .Or(Parse.Char('_'));
        }

        public static Parser<String> IdentifierOrKeyword()
        {
            return IdentifierStartCharacter()
                .Once()
                .Concat(IdentifierPartCharacter().Many())
                .Text();
        }

        public static Parser<String> Keyword()
        {
            return IdentifierOrKeyword()
                .Where(x => Keywords().Contains(x));
        }

        public static Parser<String> AvailableIdentifier()
        {
            return IdentifierOrKeyword()
                .Where(x => !Keywords().Contains(x));
        }

        public static Parser<String> Identifier()
        {
            return AvailableIdentifier()
                .Or(Parse.Char('@').Then(_ => IdentifierOrKeyword()));
        }

        private static Parser<T> Concept<T>(String keyword, Func<String, T> selector)
        {
            return from k in Parse.String(keyword)
                   from id in Identifier()
                   from statementEnd in StatementEnd()
                   select selector(id);
        }

        private static Parser<T> ConceptWithInheritance<T>(String keyword, Func<String, Maybe<String>, T> selector)
        {
            return from k in Parse.String(keyword)
                   from id in Identifier()
                   from @base in InheritsOperator().Interleave().Then(_ => Identifier().Select(x => x.ToMaybe()))
                        .Or(Parse.Return(Maybe<String>.NoValue))
                   from statementEnd in StatementEnd()
                   select selector(id, @base);
        }

        private static Parser<T> BlockedConcept<T, TBody>(String keyword, Parser<TBody> body, Func<String, TBody, T> selector)
        {
            return from k in Parse.String(keyword)
                   from id in Identifier()
                   from b in Blocked(body)
                   select selector(id, b);
        }

        private static Parser<T> BlockedConceptWithInheritance<T, TBody>(String keyword, Parser<TBody> body, Func<String, Maybe<String>, TBody, T> selector)
        {
            return from k in Parse.String(keyword)
                   from id in Identifier()
                   from @base in InheritsOperator().Interleave().Then(_ => Identifier().Select(x => x.ToMaybe()))
                        .Or(Parse.Return(Maybe<String>.NoValue))
                   from b in Blocked(body)
                   select selector(id, @base, b);
        }

        private static Parser<T> Blocked<T>(Parser<T> body)
        {
            return from blockStart in BlockStart()
                   from b in body
                   from blockEnd in BlockEnd()
                   select b;
        }

        private static Parser<Char> CharacterByUnicodeCategory(params UnicodeCategory[] categories)
        {
            Guard.NotNull(categories, "categories");

            return Parse.Char(c => categories.Contains(Char.GetUnicodeCategory(c)), "characterByUnicodeCategory");
        }

        private static Parser<T> Interleave<T>(this Parser<T> source )
        {
            return source.Token();
        }

        public static Parser<V> SelectMany<T, U, V>(this Parser<T> source, Func<T, Parser<U>> selector, Func<T, U, V> projector)
        {
            return Parse.SelectMany(source.Interleave(), selector, projector);
        }

   }
}