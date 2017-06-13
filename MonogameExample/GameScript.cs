using CSharp_Lua_Scripting_Engine;
using NLua;
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

        protected override void LoadScript()
        {
            if (_lua != null && _lua.IsExecuting)
            {
                throw new Exception("Can't load script while it's executing");
            }

            _lua?.Dispose();
            try
            {
                // If the file got updated with a lua script with errors, don't crash
                // Handle the exception, then set to null and try again later
                _lua = new Lua();
                _lua.LoadCLRPackage();
                _lua.DoString(ImportString);

                switch (ScriptSourceType)
                {
                    case ScriptSourceType.File:
                        _lua.DoFile(NameContainer[Name]);
                        break;
                    case ScriptSourceType.Code:
                        _lua.DoString(NameContainer[Name]);
                        break;
                    default:
                        throw new Exception(string.Format("Unhandled ScriptSourceType {0}", ScriptSourceType));
                }
                NeedsReload = false;
                ++LoadedCount;
            }
            catch (Exception e)
            {
                HandleException(e);
                _lua?.Dispose();
                _lua = null;
            }
        }
    }
}
