using System;

namespace CSharp_Lua_Scripting_Engine
{
    public class DefaultConsole : ILuaConsole
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
    }
}
