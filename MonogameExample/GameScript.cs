using CSharp_Lua_Scripting_Engine;
using System;

namespace MonogameExample
{
    public class GameScript : Script
    {
        public string ImportString
        {
            get
            {
                if (string.IsNullOrEmpty(_importString))
                {
                    _importString = string.Join(Environment.NewLine,
                        new object[]
                        {
                            @"import ('Monogame.Framework')",
                            @"import ('System')",
                            @"import ('System', 'System.IO')",
                            @"import ('Microsoft.Xna.Framework')",
                            @"import ('Microsoft.Xna.Framework.Graphics')",
                            @"import ('Microsoft.Xna.Framework.Input')",
                            @"import ('MonogameExample')"
                        });
                }
                return _importString;
            }
        } private string _importString;

        public GameScript(object owner, string source, ScriptSourceType scriptSourceType = ScriptSourceType.File) : 
            base(owner, source, scriptSourceType)
        {
        }

        protected override void DoPreload()
        {
            _lua.DoString(ImportString);
        }
    }
}
