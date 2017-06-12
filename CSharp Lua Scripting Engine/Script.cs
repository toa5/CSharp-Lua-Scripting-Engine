
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

    public class Script<T> : IDisposable
    {
        private readonly Func<Lua> _getLua;
        private Lua _lua;

        

        public Script(Func<Lua> getLua)
        {
            _getLua = getLua;
            _lua = getLua();
        }

        public ILuaConsole Console { get; set; }

        public LuaExceptionReaction LuaExceptionReaction { get; set; } = LuaExceptionReaction.LogToConsole;

        public bool NeedsReload { get; set; } = false;

        public T ScriptType { get; set; }

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

       

        public object[] Run(string methodName, params T[] args)
        {
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

        public void Update()
        {
            if(_lua == null || NeedsReload)
            {
                _lua?.Dispose();
                try
                {
                    // If the file got updated with a lua script with errors, don't crash
                    // Handle the exception, then set to null and try again later
                    _lua = _getLua();
                    NeedsReload = false;
                }
                catch(Exception e)
                {
                    HandleException(e);
                    _lua = null;
                }
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
                    _lua.Dispose();
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
                    Console?.Write(string.Format("Lua Error {0}", e));
                    break;
                case LuaExceptionReaction.LogToConsoleElseThrow:
                    if (Console != null)
                    {
                        Console.Write(string.Format("Lua Error {0}", e));
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
