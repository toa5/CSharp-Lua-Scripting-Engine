using NLua;
using System;
using System.Collections.Generic;

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
        protected readonly Lua _lua;

        private readonly string _name;
        private DateTime _lastLog = new DateTime();
        private ScriptSourceType _scriptSourceType;

        public Script(string name, ScriptSourceType scriptSourceType = ScriptSourceType.File)
        {
            _scriptSourceType = scriptSourceType;
            _name = name;
            
            // If the file got updated with a lua script with errors, don't crash
            // Handle the exception, then set to null and try again later
            _lua = new Lua();
            _lua.LoadCLRPackage();
        }

        public IScriptingEngineConsole Console { get; set; }

        public bool IsExecuting => _lua == null ? false : _lua.IsExecuting;
        public TimeSpan LogInterval { get; set; } = TimeSpan.FromSeconds(5);
        public LuaExceptionReaction LuaExceptionReaction { get; set; } = LuaExceptionReaction.LogToConsole;
        public string Name => _name;

        public IScriptNameContainer NameContainer
        {
            get
            {
                return _container;
            }
            set
            {
                _container = value;
                Reload();
            }
        } private IScriptNameContainer _container;

        public ScriptSourceType ScriptSourceType => _scriptSourceType;

        public virtual void Reload()
        {
            try
            {
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
            }
            catch(Exception e)
            {
                HandleException(e);
            }
        }

        public bool Exists<F>(string fullPath) where F : class
        {
            return (_lua?[fullPath] as F) != null;
        }
        
        public Dictionary<object, object> GetTableDict(LuaTable table)
        {
            return _lua.GetTableDict(table);
        }

        public void NewTable(string fullPath = "")
        {
            _lua.NewTable(fullPath);
        }

        public virtual void HandleException(Exception e)
        {
            switch (LuaExceptionReaction)
            {
                case LuaExceptionReaction.LogToConsole:
                    if ((DateTime.Now - _lastLog) > LogInterval)
                    {
                        Console?.Write(e);
                        _lastLog = DateTime.Now;
                    }
                    break;
                case LuaExceptionReaction.LogToConsoleElseThrow:
                    if (Console != null)
                    {
                        if ((DateTime.Now - _lastLog) > LogInterval)
                        {
                            Console.Write(e);
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

        public object[] Run(string methodName, params object[] args)
        {
            try
            {
                return _lua?.Run(methodName, args);
            }
            catch (Exception e)
            {
                HandleException(e);
            }
            return null;
        }

        public T Run<T>(string methodName, params object[] args) where T : class
        {
            try
            {
                return (T)(_lua.Run(methodName, args)[0]);
            }
            catch (Exception e)
            {
                HandleException(e);
            }
            return null;
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
                }

                disposedValue = true;
            }
        }
        #endregion
    }
}
