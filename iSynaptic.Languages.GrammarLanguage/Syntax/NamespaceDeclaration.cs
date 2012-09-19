using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iSynaptic.Languages.GrammarLanguage.Syntax
{
    public class NamespaceDeclaration
    {
        public NamespaceDeclaration(string name, IEnumerable<LanguageDeclaration> languages)
        {
            Name = name;
            Languages = languages;
        }

        public string Name { get; private set; }
        public IEnumerable<LanguageDeclaration> Languages { get; private set; }
    }
}
