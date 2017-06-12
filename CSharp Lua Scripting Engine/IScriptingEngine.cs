using System;
using System.Collections.Generic;

namespace CSharp_Lua_Scripting_Engine
{
    public interface IScriptingEngine<T> : IDisposable where T : Script
    {
        /// <summary>
        /// Gets all the scripts
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> AllScripts { get; }

        /// <summary>
        /// Console for writing T<T> errors
        /// </summary>
        IScriptingEngineConsole Console { get; }

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
        /// <param name="owner"></param>
        /// <param name="sourceName"></param>
        /// <param name="script"></param>
        /// <param name="scrptSourceType"></param>
        /// <returns>If the script had to be loaded, return true. Else, returns false</returns>
        bool TryGetScript(object owner, string sourceName, out T script, ScriptSourceType scrptSourceType = ScriptSourceType.File);

        /// <summary>
        /// Gets the script
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="sourceName"></param>
        /// <param name="scrptSourceType"></param>
        /// <returns></returns>
        T GetScript(object owner, string sourceName, ScriptSourceType scrptSourceType = ScriptSourceType.File);

        /// <summary>
        /// Gets all scripts based on the predicate
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<T> GetScripts(object owner, Predicate<T> predicate);

        /// <summary>
        /// Reloads all the scripts
        /// </summary>
        void ReloadAllScripts();

        /// <summary>
        /// Reloads the scripts based on the predicate
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="predicate"></param>
        void ReloadScripts(object owner, Predicate<T> predicate);

        /// <summary>
        /// Removes all scripts based on the predicate
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="predicate"></param>
        void RemoveAllScripts(object owner, Predicate<T> predicate);
    }
}
