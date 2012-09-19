using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iSynaptic.Languages.GrammarLanguage.Syntax
{
    public class LanguageDeclaration
    {
        public LanguageDeclaration(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}
