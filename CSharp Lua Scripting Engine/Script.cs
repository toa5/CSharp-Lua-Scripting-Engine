using NLua;
using System;

namespace CSharp_Lua_Scripting_Engine
{
    public enum LuaExceptionReaction
    {
        None,
        LogToConsole,
        LogToConsoleElseThrow,
        Throw
    }

    public enum ScriptSourceType
    {
        File,
        Code
    }

    public class Script : IDisposable
    {
        protected Lua _lua;

        private readonly string _source;
        private ScriptSourceType _scriptSourceType;

        private DateTime _lastLog = new DateTime();

        private readonly object _owner;

        /// <summary>
        /// How many times the script was load/reloaded
        /// </summary>
        public int LoadedCount { get; private set; }

        public Script(object owner, string source, ScriptSourceType scriptSourceType = ScriptSourceType.File)
        {
            _scriptSourceType = scriptSourceType;
            _owner = owner;
            _source = source;
            LoadScript();
        }

        protected virtual void LoadScript()
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

                // For loading common libraries into the lua file
                DoPreload();

                switch (_scriptSourceType)
                {
                    case ScriptSourceType.File:
                        _lua.DoFile(_source);
                        break;
                    case ScriptSourceType.Code:
                        _lua.DoString(_source);
                        break;
                    default:
                        throw new Exception(string.Format("Unhandled ScriptSourceType {0}", _scriptSourceType));
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

        protected virtual void DoPreload()
        {

        }

        public object Owner => _owner;

        public string Source => _source;

        public ScriptSourceType ScriptSourceType => _scriptSourceType;

        public IScriptingEngineConsole Console { get; set; }

        public LuaExceptionReaction LuaExceptionReaction { get; set; } = LuaExceptionReaction.LogToConsole;

        public bool NeedsReload { get; set; } = false;

        public TimeSpan LogInterval { get; set; } = TimeSpan.FromSeconds(5);

        public object this[string key]
        {
            get
            {
                return _lua[key];
            }
            set
            {
                _lua[key] = value;
            }
        }

        public bool Exists<TType>(string fullPath)
        {
            var luaObj = _lua[fullPath];
            return luaObj != null && luaObj.GetType() == typeof(TType);
        }

        public bool MethodExists(string methodName)
        {
            return (_lua[methodName] as LuaFunction) != null;
        }

        public object[] Run(string methodName, Action<Script> OnScriptLoaded, params object[] args)
        {
            if(_lua == null || NeedsReload)
            {
                LoadScript();
                OnScriptLoaded?.Invoke(this);
            }

            try
            {
                return _lua.Run(methodName, args);
            }
            catch(Exception e)
            {
                HandleException(e);
            }
            return null;
        }

        public object[] Run(string methodName, params object[] args)
        {
            return Run(methodName, null, args);
        }

        #region IDisposable Support
        private bool disposedValue = false; //To detect redundant calls

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
                    _lua?.Dispose();
                    _lua = null;
                }

                disposedValue = true;
            }
        }
        #endregion

        private void HandleException(Exception e)
        {
            switch (LuaExceptionReaction)
            {
                case LuaExceptionReaction.LogToConsole:
                    if ((DateTime.Now - _lastLog) > LogInterval)
                    {
                        Console?.Write(string.Format("Lua Error {0} {1}", e, e.InnerException));
                        _lastLog = DateTime.Now;
                    }
                    break;
                case LuaExceptionReaction.LogToConsoleElseThrow:
                    if (Console != null)
                    {
                        if ((DateTime.Now - _lastLog) > LogInterval)
                        {
                            Console.Write(string.Format("Lua Error {0} {1}", e, e.InnerException));
                            _lastLog = DateTime.Now;
                        }
                    }
                    else
                    {
                        throw e;
                    }
                    break;
                case LuaExceptionReaction.Throw:
                    throw e;
                default:
                    break;
            }
        }
    }
}
