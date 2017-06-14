using System;

namespace CSharp_Lua_Scripting_Engine
{
    public class DefaultConsole<T> : IScriptingEngineConsole<T>
    {
        public T LogLevel { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Clear()
        {
            Console.Clear();
        }

        public void DeleteAt(int index)
        {
            
        }

        public void Write(string message)
        {
            Console.WriteLine(message);
        }

        public void Write(string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }

        public void Write(string message, T logLevel)
        {
            Write(message);
        }

        public void Write(T logLevel, string message, params object[] args)
        {
            Write(message, args);
        }

        public void Write(Exception exception)
        {
            Write(string.Format("{0}", exception));
        }
    }
}
