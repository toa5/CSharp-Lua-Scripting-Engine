using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSharp_Lua_Scripting_Engine
{
    public class DefaultScriptingEngine<T> : IScriptingEngine<T> where T : Script
    {
        protected List<T> _scripts;

        public DefaultScriptingEngine(IScriptNameContainer container, IScriptingEngineConsole console)
        {
            _scripts = new List<T>();
            this.Console = console;
            this.ScriptSourceContainer = container;

            this.DestinationDirectory = Directory.GetCurrentDirectory();
        }

        public IEnumerable<T> AllScripts => _scripts;

        public IScriptingEngineConsole Console { get; private set; }

        public IScriptNameContainer ScriptSourceContainer { get; private set; }

        public string DestinationDirectory { get; set; }

        public T GetScript(object owner, string name, ScriptSourceType type = ScriptSourceType.File)
        {
            var list = _scripts.Where(s => s.Owner == owner && s.Name.Equals(name) && s.ScriptSourceType == type);

            T script = null;

            if (list.Count() > 0)
            {
                script = list.SingleOrDefault();
                if (script != null)
                {
                    return script;
                }
            }
           
            script = (T)Activator.CreateInstance(typeof(T), owner, name, type);
            script.Console = this.Console;
            script.NameContainer = ScriptSourceContainer;
            script.LuaExceptionReaction = LuaExceptionReaction.LogToConsole;
             _scripts.Add(script);
            return script;
        }

        public bool TryGetScript(object owner, string name, out T script, 
                            ScriptSourceType type = ScriptSourceType.File)
        {
            var list = _scripts.Where(s => s.Owner == owner && s.Name.Equals(name) && s.ScriptSourceType == type);

            if(list.Count() > 0)
            {
                script = list.SingleOrDefault();
                if (script != null)
                {
                    return false;
                }
            }
            
            script = (T)Activator.CreateInstance(typeof(T), owner, name, type);
            script.Console = this.Console;
            script.NameContainer = this.ScriptSourceContainer;
            script.LuaExceptionReaction = LuaExceptionReaction.LogToConsole;
            _scripts.Add(script);
            return true;
        }
        
        public IEnumerable<T> GetScripts(object owner, Predicate<T> predicate)
        {
            return _scripts.Where(script => script.Owner == owner && predicate(script));
        }

        public void ReloadScripts(object owner, Predicate<T> predicate)
        {
            DoReload(GetScripts(owner, predicate));
        }

        public void ReloadScript(object owner, string name)
        {
            DoReload(GetScript(owner, name));
        }
        
        public virtual void ReloadAllScripts()
        {
            foreach (string file in Directory.GetFiles(ScriptSourceContainer.SourceDirectory).Select(f => Path.GetFileName(f)))
            {
                File.Copy(Path.Combine(ScriptSourceContainer.SourceDirectory, file),
                    Path.Combine(DestinationDirectory, file), true);
            }

            foreach(var script in _scripts)
            {
                script.Reload();
            }
        }

        public void RemoveAllScripts(object owner, Predicate<T> predicate, Action<T> BeforeRemove = null, Action<T> AfterRemove = null)
        {
            foreach(var script in _scripts.Where(s => s.Owner == owner && predicate(s)).ToArray())
            {
                BeforeRemove?.Invoke(script);
                script.Dispose();
                _scripts.Remove(script);
                AfterRemove?.Invoke(script);
            }
        }

        private void DoReload(T script)
        {
            switch (script.ScriptSourceType)
            {
                case ScriptSourceType.File:
                    string file = Path.GetFileName(script.Name);
                    File.Copy(Path.Combine(ScriptSourceContainer.SourceDirectory, file),
                        Path.Combine(DestinationDirectory, file), true);
                    script.Reload();
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
