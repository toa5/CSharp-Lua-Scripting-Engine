using System;
using System.Collections.Generic;

namespace CSharp_Lua_Scripting_Engine
{
    public interface IScriptingEngine : IDisposable
    {
        /// <summary>
        /// Mapping script names to source paths
        /// </summary>
        IScriptNameContainer ScriptSourceContainer { get; }

        /// <summary>
        /// The place where the updated scripts will be saved when reloading
        /// </summary>
        string DestinationDirectory { get; set; }

        /// <summary>
        /// Reloads all the scripts
        /// </summary>
        void ReloadAllScripts();
    }

    public interface IScriptingEngine<T> : IScriptingEngine where T : Script
    {
        /// <summary>
        /// Gets all the scripts
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> AllScripts { get; }

        /// <summary>
        /// Gets the script
        /// </summary>
        /// <param name="nameOrSource"></param>
        /// <param name="script"></param>
        /// <param name="scrptSourceType"></param>
        /// <returns>If the script had to be loaded, return true. Else, returns false</returns>
        bool TryGetScript(string name, out T script, ScriptSourceType scrptSourceType = ScriptSourceType.File);

        /// <summary>
        /// Gets the script
        /// </summary>
        /// <param name="nameOrSource"></param>
        /// <param name="scrptSourceType"></param>
        /// <returns></returns>
        T GetScript(string name, ScriptSourceType scrptSourceType = ScriptSourceType.File);

        /// <summary>
        /// Gets all scripts based on the predicate
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<T> GetScripts(Predicate<T> predicate);

        /// <summary>
        /// Reloads the scripts based on the predicate
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="predicate"></param>
        void ReloadScripts(Predicate<T> predicate);

        /// <summary>
        /// Removes all scripts based on the predicate
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="predicate"></param>
        /// <param name="BeforeRemove"></param>
        /// <param name="AfterRemove"></param>
        void RemoveAllScripts(Predicate<T> predicate, Action<T> BeforeRemove = null, Action<T> AfterRemove = null);
    }

    public interface IScriptingEngine<T, K> : IScriptingEngine<T> where T : Script
    {
        /// <summary>
        /// Console for writing T<T> errors
        /// </summary>
        IScriptingEngineConsole<K> Console { get; }
    }
}
