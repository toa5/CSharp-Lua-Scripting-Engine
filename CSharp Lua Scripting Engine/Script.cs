using NLua;
using System;
using System.IO;

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

        private readonly object _owner;
        private readonly string _name;
        private DateTime _lastLog = new DateTime();
        private ScriptSourceType _scriptSourceType;

        public Script(object owner, string name, ScriptSourceType scriptSourceType = ScriptSourceType.File)
        {
            _scriptSourceType = scriptSourceType;
            _owner = owner;
            _name = name;
        }

        public IScriptingEngineConsole Console { get; set; }

        public IScriptNameContainer NameContainer { get; set; }

        /// <summary>
        /// How many times the script was load/reloaded
        /// </summary>
        public int LoadedCount { get; protected set; }
        public TimeSpan LogInterval { get; set; } = TimeSpan.FromSeconds(5);

        public LuaExceptionReaction LuaExceptionReaction { get; set; } = LuaExceptionReaction.LogToConsole;

        public bool NeedsReload { get; set; } = false;

        public object Owner => _owner;

        public ScriptSourceType ScriptSourceType => _scriptSourceType;

        public string Name => _name;

        public bool Exists<TType>(string fullPath)
        {
            if (_lua == null || NeedsReload)
            {
                LoadScript();
            }
            var luaObj = _lua[fullPath];
            return luaObj != null && luaObj.GetType() == typeof(TType);
        }

        public void HandleException(Exception e)
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

        public bool MethodExists(string methodName)
        {
            if (_lua == null || NeedsReload)
            {
                LoadScript();
            }
            return (_lua[methodName] as LuaFunction) != null;
        }

        public object[] Run(string methodName, params object[] args)
        {
            if (_lua == null || NeedsReload)
            {
                LoadScript();
            }

            return DoRun(methodName, args);
        }

        protected object[] DoRun(string methodName, params object[] args)
        {
            try
            {
                return _lua.Run(methodName, args);
            }
            catch (Exception e)
            {
                HandleException(e);
            }
            return null;
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

                switch (_scriptSourceType)
                {
                    case ScriptSourceType.File:
                        _lua.DoFile(NameContainer[Name]);
                        break;
                    case ScriptSourceType.Code:
                        _lua.DoString(NameContainer[Name]);
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
                    if (_lua != null && _lua.IsExecuting)
                    {
                        throw new Exception("Can't dispose while lua script is executing");
                    }

                    _lua?.Dispose();
                    _lua = null;
                }

                disposedValue = true;
            }
        }
        #endregion
    }
}
