namespace CSharp_Lua_Scripting_Engine
{
    public interface IConsole
    {
        /// <summary>
        /// Writes the message
        /// </summary>
        /// <param name="message"></param>
        void Write(string message);

        /// <summary>
        /// Writes the message as a line
        /// </summary>
        /// <param name="message"></param>
        void WriteLine(string message);

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
