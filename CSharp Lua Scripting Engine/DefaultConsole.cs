using System;

namespace CSharp_Lua_Scripting_Engine
{
    public class DefaultConsole : IScriptingEngineConsole
    {
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
    }
}
