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
using System.Linq;
using Sprache;
using iSynaptic.Commons;

namespace iSynaptic.Languages.GrammarLanguage.Bootstrap
{
    public static class Parser
    {
        public static Parser<NamespaceDeclaration> Namespace()
        {
            return from keyword in Keyword("namespace")
                   from name in Name()
                   from members in Blocked(NamespaceMember().Many())
                   select new NamespaceDeclaration
                   {
                       Name = name,
                       Members = members.ToList()
                   };
        }

        public static Parser<INamespaceMember> NamespaceMember()
        {
            return ((Parser<INamespaceMember>)Namespace()).Or(Language());
        }

        public static Parser<LanguageDeclaration> Language()
        {
            return from keyword in Keyword("language")
                   from name in IdentifierName()
                   from members in Blocked(Interleave())
                   select new LanguageDeclaration
                   {
                       Name = name
                   };
        }

        public static Parser<NameSyntax> Name()
        {
            return ((Parser<NameSyntax>)QualifiedName()).Or(UnqualifiedName());
        }

        public static Parser<QualifiedNameSyntax> QualifiedName()
        {
            return Delimit(UnqualifiedName(), Period(), 1)
                .Select(x => x.Aggregate((QualifiedNameSyntax)null, (l, r) => new QualifiedNameSyntax { Left = l, Right = r }));
        }

        public static Parser<UnqualifiedNameSyntax> UnqualifiedName()
        {
            return ((Parser<UnqualifiedNameSyntax>) GenericName()).Or(IdentifierName());
        }

        public static Parser<GenericNameSyntax> GenericName()
        {
            return from id in IdentifierOrKeyword()
                   from start in AngleStart()
                   from types in Delimit(Name(), Comma(), 0)
                   from end in AngleEnd()
                   select new GenericNameSyntax
                   {
                       Identifier = id,
                       TypeArguments = types.ToArray()
                   };
        }

        public static Parser<IdentifierNameSyntax> IdentifierName()
        {
            return IdentifierOrKeyword().Select(x => new IdentifierNameSyntax {Identifier = x});
        }

        public static Parser<String> InheritsOperator() { return Parse.String(":").Text(); }

        public static Parser<String> BlockStart() { return Parse.String("{").Text().Named("block start"); }
        public static Parser<String> BlockEnd() { return Parse.String("}").Text().Named("block end"); }

        public static Parser<String> AngleStart() { return Parse.String("<").Text(); }
        public static Parser<String> AngleEnd() { return Parse.String(">").Text(); }

        public static Parser<String> Period() { return Parse.String(".").Text(); }
        public static Parser<String> Comma() { return Parse.String(",").Text(); }

        public static Parser<String> StatementEnd() { return Parse.String(";").Text(); }

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
            var identifier = IdentifierStartCharacter()
                .Once()
                .Concat(IdentifierPartCharacter().Many())
                .Text();

            return identifier
                .Or(Parse.Char('@').Then(_ => identifier.Select(x => "@" + x)));
        }

        public static Parser<String> Keyword(String keyword)
        {
            return IdentifierOrKeyword()
                .Where(x => x == keyword);
        }

        public static Parser<String> SingleLineComment()
        {
            return Parse.String("//")
                .Then(_ => Parse.AnyChar.Except(NewLineCharacter()).Many().Text())
                .Select(txt => "//" + txt);
        }

        public static Parser<String> MultiLineComment()
        {
            return Parse.String("/*")
                .Then(_ => Parse.AnyChar.Until(Parse.String("*/")).Text())
                .Select(txt => "/*" + txt + "*/");
        }

        public static Parser<String> NewLine()
        {
            return Parse.String("\u000D") // Carriage return character
                .Or(Parse.String("\u000A")) // Line feed character
                .Or(Parse.String("\u000D\u000A")) // Carriage return character followed by line feed character
                .Or(Parse.String("\u0085")) // Next line character
                .Or(Parse.String("\u2028")) // Line separator character
                .Or(Parse.String("\u2029")) // Paragraph separator character
                .Text();
        }

        public static Parser<Char> NewLineCharacter()
        {
            return Parse.Char('\u000D') // Carriage return character
                .Or(Parse.Char('\u000A')) // Line feed character
                .Or(Parse.Char('\u0085')) // Next line character
                .Or(Parse.Char('\u2028')) // Line separator character
                .Or(Parse.Char('\u2029')); // Paragraph separator character
        }

        public static Parser<Char> WhitespaceCharacter()
        {
            return CharacterByUnicodeCategory(UnicodeCategory.SpaceSeparator)
                .Or(Parse.Char('\u0009')) // Horizontal tab character
                .Or(Parse.Char('\u000B')) // Vertical tab character
                .Or(Parse.Char('\u000C')); // Form feed character
        }

        public static Parser<String> Whitespace()
        {
            return WhitespaceCharacter().AtLeastOnce().Text();
        }

        public static Parser<T> Concept<T, TDefinition>(String keyword, Parser<TDefinition> definition, Func<String, TDefinition, T> selector)
        {
            return ConceptCore(keyword, false, definition, (id, @base, def) => selector(id, def));
        }

        public static Parser<T> Concept<T, TDefinition>(String keyword, Parser<TDefinition> definition, Func<String, Maybe<String>, TDefinition, T> selector)
        {
            return ConceptCore(keyword, true, definition, selector);
        }

        private static Parser<T> ConceptCore<T, TDefinition>(String keyword, bool canInherit, Parser<TDefinition> definition, Func<String, Maybe<String>, TDefinition, T> selector)
        {
            return from k in Keyword(keyword)
                   from id in IdentifierOrKeyword()

                   from @base in canInherit
                        ? InheritsOperator().Interleave().Then(_ => IdentifierOrKeyword().Select(x => x.ToMaybe()))
                            .Or(Parse.Return(Maybe<String>.NoValue))
                        : Parse.Return(Maybe<String>.NoValue)

                   from def in definition
                   select selector(id, @base, def);
        }

        private static Parser<T> Blocked<T>(this Parser<T> body)
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

        private static Parser<IEnumerable<T>> Delimit<T, TDelimiter>(this Parser<T> parser, Parser<TDelimiter> delimiter, int minimumRepeats)
        {
            return from result in parser
                   from remaining in (from d in delimiter from r in parser select r).Many()
                   where remaining.Count() >= minimumRepeats
                   select new[]{result}.Concat(remaining);
        }

        public static Parser<IEnumerable<String>> Interleave()
        {
            return (SingleLineComment()
                .Or(MultiLineComment())
                .Or(NewLine())
                .Or(Whitespace()))
                .Many();
        }

        public static Parser<T> Interleaved<T>(this Parser<T> body)
        {
            var interleave = Interleave();

            return
                Parse.SelectMany(
                    Parse.SelectMany(interleave, _ => body, (_, b) => b), b => interleave, (b, _) => b);
        }

        public static Parser<V> SelectMany<T, U, V>(this Parser<T> source, Func<T, Parser<U>> selector, Func<T, U, V> projector)
        {
            return Parse.SelectMany(source.Interleaved(), selector, projector);
        }

    }
}