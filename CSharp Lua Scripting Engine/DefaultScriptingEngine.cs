using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSharp_Lua_Scripting_Engine
{
    public class DefaultScriptingEngine<T,K> : IScriptingEngine<T, K> where T : Script
    {
        protected List<T> _scripts;

        public DefaultScriptingEngine(IScriptNameContainer container, IScriptingEngineConsole<K> console)
        {
            _scripts = new List<T>();
            this.Console = console;
            this.ScriptSourceContainer = container;

            this.DestinationDirectory = Directory.GetCurrentDirectory();
        }

        public IEnumerable<T> AllScripts => _scripts;

        public IScriptingEngineConsole<K> Console { get; private set; }

        public IScriptNameContainer ScriptSourceContainer { get; private set; }

        public string DestinationDirectory { get; set; }

        public T GetScript(string name, ScriptSourceType type = ScriptSourceType.File)
        {
            var list = _scripts.Where(s => s.Name.Equals(name) && s.ScriptSourceType == type);

            T script = null;

            if (list.Count() > 0)
            {
                script = list.SingleOrDefault();
                if (script != null)
                {
                    return script;
                }
            }
           
            script = (T)Activator.CreateInstance(typeof(T), name, type);
            script.Console = this.Console;
            script.NameContainer = ScriptSourceContainer;
            script.LuaExceptionReaction = LuaExceptionReaction.LogToConsole;
             _scripts.Add(script);
            return script;
        }

        public bool TryGetScript(string name, out T script, 
                            ScriptSourceType type = ScriptSourceType.File)
        {
            var list = _scripts.Where(s => s.Name.Equals(name) && s.ScriptSourceType == type);

            if(list.Count() > 0)
            {
                script = list.SingleOrDefault();
                if (script != null)
                {
                    return false;
                }
            }
            
            script = (T)Activator.CreateInstance(typeof(T), name, type);
            script.Console = this.Console;
            script.NameContainer = this.ScriptSourceContainer;
            script.LuaExceptionReaction = LuaExceptionReaction.LogToConsole;
            _scripts.Add(script);
            return true;
        }
        
        public IEnumerable<T> GetScripts(Predicate<T> predicate)
        {
            return _scripts.Where(script => predicate(script));
        }

        public void ReloadScripts(Predicate<T> predicate)
        {
            DoReload(GetScripts(predicate));
        }

        public void ReloadScript(string name)
        {
            DoReload(GetScript(name));
        }
        
        public virtual void ReloadAllScripts()
        {
            if(DestinationDirectory.Last() != '\\')
            {
                DestinationDirectory += '\\';
            }

            foreach (string file in Directory.GetFiles
                (ScriptSourceContainer.SourceDirectory, "*", SearchOption.AllDirectories))
            {
                File.Copy(file, file.Replace(ScriptSourceContainer.SourceDirectory, DestinationDirectory), true);
            }

            foreach(var script in _scripts)
            {
                script.Reload();
            }
        }

        public void RemoveAllScripts(Predicate<T> predicate, Action<T> BeforeRemove = null, Action<T> AfterRemove = null)
        {
            foreach(var script in _scripts.Where(s => predicate(s)).ToArray())
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
