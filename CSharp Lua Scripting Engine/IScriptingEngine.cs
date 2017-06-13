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
        /// Mapping script names to source paths
        /// </summary>
        IScriptNameContainer ScriptSourceContainer { get; }

        /// <summary>
        /// The place where the updated scripts will be saved when reloading
        /// </summary>
        string DestinationDirectory { get; set; }

        /// <summary>
        /// Gets the script
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="nameOrSource"></param>
        /// <param name="script"></param>
        /// <param name="scrptSourceType"></param>
        /// <returns>If the script had to be loaded, return true. Else, returns false</returns>
        bool TryGetScript(object owner, string name, out T script, ScriptSourceType scrptSourceType = ScriptSourceType.File);

        /// <summary>
        /// Gets the script
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="nameOrSource"></param>
        /// <param name="scrptSourceType"></param>
        /// <returns></returns>
        T GetScript(object owner, string name, ScriptSourceType scrptSourceType = ScriptSourceType.File);

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
        /// <param name="BeforeRemove"></param>
        /// <param name="AfterRemove"></param>
        void RemoveAllScripts(object owner, Predicate<T> predicate, Action<T> BeforeRemove = null, Action<T> AfterRemove = null);
    }
}
