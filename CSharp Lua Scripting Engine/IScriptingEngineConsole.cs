using System;

namespace CSharp_Lua_Scripting_Engine
{
    public interface IScriptingEngineConsole
    {
        /// <summary>
        /// Writes the message
        /// </summary>
        /// <param name="message"></param>
        void Write(string message);

        /// <summary>
        /// Writes the message
        /// </summary>
        /// <param name="message"></param>
        void Write(string message, params object[] args);

        /// <summary>
        /// Writes the message
        /// </summary>
        /// <param name="exception"></param>
        void Write(Exception exception);

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

    public interface IScriptingEngineConsole<T> : IScriptingEngineConsole
    {
        T LogLevel { get; set; }
        
        /// <summary>
        /// Writes the message
        /// </summary>
        /// <param name="message"></param>
        void Write(string message, T logLevel);

        /// <summary>
        /// Writes the message
        /// </summary>
        /// <param name="message"></param>
        void Write(T logLevel, string message, params object[] args);
    }
}
