using NLua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSharp_Lua_Scripting_Engine
{
    public class DefaultScriptingEngine : IScriptingEngine, IDisposable
    {
        private static readonly Func<IScriptable, ScriptType, Lua> _scriptLoader = (key, type) =>
         {
             Lua newLua = new Lua();
             switch (key.ScriptableType)
             {
                 case ScriptableType.Code:
                     newLua.DoString(key.GetScriptSource(type));
                     break;
                 case ScriptableType.File:
                     newLua.DoFile(key.GetScriptSource(type));
                     break;
             }
             return newLua;
         };

        private bool _needsLoad = false;

        private Dictionary<IScriptable, Script> _scripts;

        public DefaultScriptingEngine(IConsole console)
        {
            _scripts = new Dictionary<IScriptable, Script>();
            Console = console;

            DestinationDirectory = Directory.GetCurrentDirectory();
            SourceDirectory = Directory.GetCurrentDirectory();
        }

        public IEnumerable<Script> AllScripts => _scripts.Select(pair => pair.Value);

        public IConsole Console { get; private set; }

        public string DestinationDirectory { get; set; }

        public string SourceDirectory { get; set; }

        public bool GetScript(IScriptable key, ScriptType type, out Script lua)
        {
            bool loaded = false;
            if (_scripts.ContainsKey(key) == false)
            {
                Script script = new Script(() => _scriptLoader(key, type));
                _scripts.Add(key, script);
            }

            lua = _scripts[key];
            return loaded;
        }

        public IEnumerable<Script> GetScripts(Predicate<IScriptable> predicate)
        {
            return _scripts.Where(pair => predicate(pair.Key)).Select(pair => pair.Value);
        }

        public IEnumerable<Script> GetScripts(Predicate<ScriptType> predicate)
        {
            return _scripts.Where(pair => predicate(pair.Value.ScriptType)).Select(pair => pair.Value);
        }

        public IEnumerable<Script> GetScripts(Predicate<Script> predicate)
        {
            return _scripts.Where(pair => predicate(pair.Value)).Select(pair => pair.Value);
        }

        public virtual void ReloadAllScripts()
        {
            DoReload(_scripts.Select(pair => pair.Value));
        }

        public virtual void ReloadScripts(Predicate<ScriptType> predicate)
        {
            DoReload(_scripts.Where(pair => predicate(pair.Value.ScriptType)).Select(pair => pair.Value));
        }

        public virtual void ReloadScripts(Predicate<IScriptable> predicate)
        {
            DoReload(_scripts.Where(pair => predicate(pair.Key)).Select(pair => pair.Value));
        }

        public virtual void ReloadScripts(Predicate<Script> predicate)
        {
            DoReload(_scripts.Where(pair => predicate(pair.Value)).Select(pair => pair.Value));
        }

        public virtual void Update()
        {
            foreach (var pair in _scripts)
            {
                pair.Value.Update();
            }
        }

        private void DoReload(IEnumerable<Script> scripts)
        {
            foreach(var script in scripts)
            {
                script.NeedsReload = true;
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
