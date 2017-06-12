using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSharp_Lua_Scripting_Engine
{
    public class DefaultScriptingEngine<T> : IScriptingEngine<T> where T : Script
    {
        protected List<T> _scripts;

        public DefaultScriptingEngine(IScriptingEngineConsole console)
        {
            _scripts = new List<T>();
            this.Console = console;

            this.DestinationDirectory = Directory.GetCurrentDirectory();
            this.SourceDirectory = Directory.GetCurrentDirectory();
        }

        public IEnumerable<T> AllScripts => _scripts;

        public IScriptingEngineConsole Console { get; private set; }

        public string DestinationDirectory { get; set; }

        public string SourceDirectory { get; set; }

        public T GetScript(object owner, string source, ScriptSourceType type = ScriptSourceType.File)
        {
            var list = _scripts.Where(s => s.Source.Equals(source) && s.ScriptSourceType == type);

            T script = null;

            if (list.Count() > 0)
            {
                script = list.SingleOrDefault();
                if (script != null)
                {
                    return script;
                }
            }

            script = (T)Activator.CreateInstance(typeof(T), owner, source, type);
            script.Console = this.Console;
            script.LuaExceptionReaction = LuaExceptionReaction.LogToConsole;
            _scripts.Add(script);
            return script;

        }
        public bool TryGetScript(object owner, string source, out T script, ScriptSourceType type = ScriptSourceType.File)
        {
            var list = _scripts.Where(s => s.Source.Equals(source) && s.ScriptSourceType == type);

            if(list.Count() > 0)
            {
                script = list.SingleOrDefault();
                if (script != null)
                {
                    return false;
                }
            }
            
            script = (T)Activator.CreateInstance(typeof(T), owner, source, type);
            script.Console = this.Console;
            script.LuaExceptionReaction = LuaExceptionReaction.LogToConsole;
            _scripts.Add(script);
            return true;
        }

        public T GetScript(object owner, string sourceName)
        {
            return _scripts.Where(script => script.Owner == owner && script.Source.Equals(sourceName)).FirstOrDefault();
        }

        public IEnumerable<T> GetScripts(object owner, Predicate<T> predicate)
        {
            return _scripts.Where(script => script.Owner == owner && predicate(script));
        }

        public void ReloadScripts(object owner, Predicate<T> predicate)
        {
            DoReload(GetScripts(owner, predicate));
        }

        public void ReloadScript(object owner, string sourceName)
        {
            DoReload(GetScript(owner, sourceName));
        }
        
        public virtual void ReloadAllScripts()
        {
            DoReload(_scripts);
        }

        public void RemoveAllScripts(object owner, Predicate<T> predicate)
        {
            foreach(var script in _scripts.Where(s => s.Owner == owner && predicate(s)).ToArray())
            {
                script.Dispose();
                _scripts.Remove(script);
            }
        }

        private void DoReload(T script)
        {
            script.NeedsReload = true;
            switch (script.ScriptSourceType)
            {
                case ScriptSourceType.File:
                    string file = Path.GetFileName(script.Source);
                    File.Copy(Path.Combine(SourceDirectory, file),
                        Path.Combine(DestinationDirectory, file), true);
                    break;
            }
        }
        private void DoReload(IEnumerable<T> scripts)
        {
            foreach (var script in scripts)
            {
                DoReload(script);
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
                    foreach(var script in _scripts)
                    {
                        script.Dispose();
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
