
using NLua;
using System;

namespace CSharp_Lua_Scripting_Engine
{
    public class Script : IDisposable
    {
        private readonly Func<Lua> _getLua;
        private Lua _lua;

        public Script(Func<Lua> getLua)
        {
            _getLua = getLua;
            _lua = getLua();
        }

        public bool NeedsReload { get; set; } = false;
        public ScriptType ScriptType { get; set; }
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

        public object[] Run(string methodName, params object[] args)
        {
            var func = _lua[methodName] as LuaFunction;
            return func?.Call(args);
        }

        public void Update()
        {
            if(_lua == null || NeedsReload)
            {
                _lua?.Dispose();
                _lua = _getLua();
                NeedsReload = false;
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
                    _lua.Dispose();
                    _lua = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Script() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }
        #endregion
    }
}
