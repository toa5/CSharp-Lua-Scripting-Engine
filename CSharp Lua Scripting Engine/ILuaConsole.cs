namespace CSharp_Lua_Scripting_Engine
{
    public interface ILuaConsole
    {
        /// <summary>
        /// Writes the message
        /// </summary>
        /// <param name="message"></param>
        void Write(string message);
        
        /// <summary>
        /// Deletes the message at an index
        /// </summary>
        /// <param name="index"></param>
        void DeleteAt(int index);

        /// <summary>
        /// Clears the message list
        /// </summary>
        void Clear();
    }
}
