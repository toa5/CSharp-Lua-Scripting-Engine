using NLua;
using System;
using System.Collections.Generic;

namespace CSharp_Lua_Scripting_Engine
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">The script type</typeparam>
    public interface IScriptingEngine<T>
    {
        /// <summary>
        /// Gets all the scripts
        /// </summary>
        /// <returns></returns>
        IEnumerable<Script<T>> AllScripts { get; }

        /// <summary>
        /// Console for writing Script<T> errors
        /// </summary>
        ILuaConsole Console { get; }

        /// <summary>
        /// The place where the updated scripts will be saved when reloading
        /// </summary>
        string DestinationDirectory { get; set; }

        /// <summary>
        /// The location of the scripts to be copied to the destination directory
        /// </summary>
        string SourceDirectory { get; set; }

        /// <summary>
        /// Gets the script
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <param name="script"></param>
        /// <returns>if true, the script was loaded, else it was pulled from a cache</returns>
        bool GetScript(IScriptable key, T scriptType, out Script<T> script);

        /// <summary>
        /// Gets the script
        /// </summary>
        /// <param name="key"></param>
        /// <param name="scriptType"></param>
        /// <returns></returns>
        Script<T> GetScript(IScriptable key, T scriptType);

        /// <summary>
        /// Gets all scripts based on the predicate
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Sequence of Script<T> objects</returns>
        IEnumerable<Script<T>> GetScripts(Predicate<IScriptable> predicate);

        /// <summary>
        /// Gets all scripts based on the predicate
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Sequence of Script<T> objects</returns>
        IEnumerable<Script<T>> GetScripts(Predicate<T> predicate);

        /// <summary>
        /// Gets all scripts based on the predicate
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Sequence of Script<T> objects</returns>
        IEnumerable<Script<T>> GetScripts(Predicate<Script<T>> predicate);
        /// <summary>
        /// Reloads all the scripts
        /// </summary>
        void ReloadAllScripts();

        /// <summary>
        /// Reloads the scripts based on the predicate
        /// </summary>
        void ReloadScripts(Predicate<T> predicate);

        /// <summary>
        /// Reloads the scripts based on the predicate
        /// </summary>
        void ReloadScripts(Predicate<IScriptable> predicate);
        /// <summary>
        /// Reloads the scripts based on the predicate
        /// </summary>
        void ReloadScripts(Predicate<Script<T>> predicate);

        /// <summary>
        /// Updates
        /// </summary>
        void Update();
    }
}
