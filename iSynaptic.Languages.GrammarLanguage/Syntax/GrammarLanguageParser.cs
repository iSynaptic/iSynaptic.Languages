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
using System.Globalization;
using Sprache;
using iSynaptic.Commons;
using System.Linq;

namespace iSynaptic.Languages.GrammarLanguage.Syntax
{
    internal static class ParserExtensions
    {
        public static Parser<V> SelectMany<T, U, V>(this Parser<T> source, Func<T, Parser<U>> selector, Func<T,U,V> projector)
        {
            return Parse.SelectMany(source.Token(), selector, projector);
        }
    }

    public class GrammarLanguageParser
    {
        public GrammarLanguageParser()
        {
            BlockStart = Parse.String("{").Text();
            BlockEnd = Parse.String("}").Text();

            NamespaceKeyword = Token("namespace");
            LanguageKeyword = Token("language");

            LetterCharacter = CharacterByUnicodeCategory(
                UnicodeCategory.UppercaseLetter,
                UnicodeCategory.LowercaseLetter,
                UnicodeCategory.TitlecaseLetter,
                UnicodeCategory.ModifierLetter,
                UnicodeCategory.OtherLetter,
                UnicodeCategory.LetterNumber);

            DecimalDigitCharacter = CharacterByUnicodeCategory(
                UnicodeCategory.DecimalDigitNumber);

            ConnectingCharacter = CharacterByUnicodeCategory(
                UnicodeCategory.ConnectorPunctuation);

            CombiningCharacter = CharacterByUnicodeCategory(
                UnicodeCategory.NonSpacingMark, 
                UnicodeCategory.SpacingCombiningMark);

            FormattingCharacter = CharacterByUnicodeCategory(
                UnicodeCategory.Format);

            IdentifierPartCharacter = 
                    LetterCharacter
                .Or(DecimalDigitCharacter)
                .Or(ConnectingCharacter)
                .Or(CombiningCharacter)
                .Or(FormattingCharacter);

            IdentifierStartCharacter = LetterCharacter
                .Or(Parse.Char('_'));

            IdentifierOrKeyword = IdentifierStartCharacter
                .Once()
                .Concat(IdentifierPartCharacter.Many())
                .Text();

            LanguageDeclaration = BlockedUnitDeclaration(
                LanguageKeyword,
                Parse.Return(new Unit()),
                (id, u) => new LanguageDeclaration(id));

            NamespaceDeclaration = BlockedUnitDeclaration(
                NamespaceKeyword, 
                LanguageDeclaration.Many(), 
                (id, langs) => new NamespaceDeclaration(id, langs));
        }

        public Parser<String> NamespaceKeyword { get; private set; }
        public Parser<String> LanguageKeyword { get; private set; }

        public Parser<NamespaceDeclaration> NamespaceDeclaration { get; private set; }
        public Parser<LanguageDeclaration> LanguageDeclaration { get; private set; }

        public Parser<Char> LetterCharacter { get; private set;}
        public Parser<Char> DecimalDigitCharacter { get; private set; }
        public Parser<Char> ConnectingCharacter { get; private set; }
        public Parser<Char> CombiningCharacter { get; private set; }
        public Parser<Char> FormattingCharacter { get; private set; }

        public Parser<Char> IdentifierPartCharacter { get; private set; }
        public Parser<Char> IdentifierStartCharacter { get; private set; }

        public Parser<String> IdentifierOrKeyword { get; private set; }

        public Parser<String> BlockStart { get; private set; }
        public Parser<String> BlockEnd { get; private set; }

        public Parser<T> BlockedUnitDeclaration<T, TBody>(Parser<String> keyword, Parser<TBody> body, Func<String, TBody, T> selector)
        {
            return from k in keyword
                   from id in IdentifierOrKeyword
                   from blockStart in BlockStart
                   from b in body
                   from blockEnd in BlockEnd
                   select selector(id, b);
        }

        private Parser<Char> CharacterByUnicodeCategory(params UnicodeCategory[] categories)
        {
            Guard.NotNull(categories, "categories");

            return Parse.Char(c => categories.Contains(Char.GetUnicodeCategory(c)), "characterByUnicodeCategory");
        }

        private Parser<String> Token(String literal)
        {
            return Parse.String(literal).Text();
        }
   }
}
