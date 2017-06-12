using NLua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSharp_Lua_Scripting_Engine
{
    public class DefaultScriptingEngine<T> : IScriptingEngine<T>, IDisposable
    {
        private static readonly Func<IScriptable, T, Lua> _scriptLoader = (key, scriptType) =>
         {
             Lua newLua = new Lua();
             switch (key.ScriptableType)
             {
                 case ScriptableType.Code:
                     newLua.DoString(key.GetScriptSource(scriptType));
                     break;
                 case ScriptableType.File:
                     newLua.DoFile(key.GetScriptSource(scriptType));
                     break;
             }
             return newLua;
         };

        private Dictionary<IScriptable, Script<T>> _scripts;

        public DefaultScriptingEngine(ILuaConsole console)
        {
            _scripts = new Dictionary<IScriptable, Script<T>>();
            this.Console = console;

            this.DestinationDirectory = Directory.GetCurrentDirectory();
            this.SourceDirectory = Directory.GetCurrentDirectory();
        }

        public IEnumerable<Script<T>> AllScripts => _scripts.Select(pair => pair.Value);

        public ILuaConsole Console { get; private set; }

        public string DestinationDirectory { get; set; }

        public string SourceDirectory { get; set; }

        public bool GetScript(IScriptable key, T scriptType, out Script<T> lua)
        {
            bool loaded = false;
            if (_scripts.ContainsKey(key) == false)
            {
                Script<T> script = new Script<T>(() => _scriptLoader(key, scriptType));
                script.Console = this.Console;
                script.LuaExceptionReaction = LuaExceptionReaction.LogToConsole;
                _scripts.Add(key, script);
                loaded = true;
            }

            lua = _scripts[key];
            return loaded;
        }

        public Script<T> GetScript(IScriptable key, T scriptType)
        {
            if (_scripts.ContainsKey(key) == false)
            {
                Script<T> script = new Script<T>(() => _scriptLoader(key, scriptType));
                script.Console = this.Console;
                script.LuaExceptionReaction = LuaExceptionReaction.LogToConsole;
                _scripts.Add(key, script);
            }

            return _scripts[key];
        }

        public IEnumerable<Script<T>> GetScripts(Predicate<IScriptable> predicate)
        {
            return _scripts.Where(pair => predicate(pair.Key)).Select(pair => pair.Value);
        }

        public IEnumerable<Script<T>> GetScripts(Predicate<T> predicate)
        {
            return _scripts.Where(pair => predicate(pair.Value.ScriptType)).Select(pair => pair.Value);
        }

        public IEnumerable<Script<T>> GetScripts(Predicate<Script<T>> predicate)
        {
            return _scripts.Where(pair => predicate(pair.Value)).Select(pair => pair.Value);
        }

        public virtual void ReloadAllScripts()
        {
            DoReload(_scripts);
        }

        public virtual void ReloadScripts(Predicate<T> predicate)
        {
            DoReload(_scripts.Where(pair => predicate(pair.Value.ScriptType)));
        }

        public virtual void ReloadScripts(Predicate<IScriptable> predicate)
        {
            DoReload(_scripts.Where(pair => predicate(pair.Key)));
        }

        public virtual void ReloadScripts(Predicate<Script<T>> predicate)
        {
            DoReload(_scripts.Where(pair => predicate(pair.Value)));
        }

        public virtual void Update()
        {
            foreach (var pair in _scripts)
            {
                pair.Value.Update();
            }
        }

        private void DoReload(IEnumerable<KeyValuePair<IScriptable, Script<T>>> pairs)
        {
            foreach(var pair in pairs)
            {
                IScriptable scriptable = pair.Key;
                Script<T> script = pair.Value;
                script.NeedsReload = true;
                if (scriptable.ScriptableType == ScriptableType.File)
                {
                    string file = Path.GetFileName(scriptable.GetScriptSource(script.ScriptType));
                    File.Copy(Path.Combine(SourceDirectory, file), 
                        Path.Combine(DestinationDirectory, file), true);
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach(var pair in _scripts)
                    {
                        pair.Value.Dispose();
                    }
                    _scripts.Clear();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }
        #endregion
    }
}
