using NLua;
using System;
using System.Collections.Generic;

namespace CSharp_Lua_Scripting_Engine
{
    public delegate void ScriptReloadedEvent(ScriptReloadedEventArgs args);

    public interface IScriptingEngine
    {
        /// <summary>
        /// Gets all the scripts
        /// </summary>
        /// <returns></returns>
        IEnumerable<Script> AllScripts { get; }

        /// <summary>
        /// Console for writing Script errors
        /// </summary>
        IConsole Console { get; }

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
        /// <param name="lua"></param>
        /// <returns>if true, the script was loaded, else it was pulled from a cache</returns>
        bool GetScript(IScriptable key, ScriptType type, out Script lua);

        /// <summary>
        /// Gets all scripts based on the predicate
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Sequence of Script objects</returns>
        IEnumerable<Script> GetScripts(Predicate<IScriptable> predicate);

        /// <summary>
        /// Gets all scripts based on the predicate
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Sequence of Script objects</returns>
        IEnumerable<Script> GetScripts(Predicate<ScriptType> predicate);

        /// <summary>
        /// Gets all scripts based on the predicate
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Sequence of Script objects</returns>
        IEnumerable<Script> GetScripts(Predicate<Script> predicate);
        /// <summary>
        /// Reloads all the scripts
        /// </summary>
        void ReloadAllScripts();

        /// <summary>
        /// Reloads the scripts based on the predicate
        /// </summary>
        void ReloadScripts(Predicate<ScriptType> predicate);

        /// <summary>
        /// Reloads the scripts based on the predicate
        /// </summary>
        void ReloadScripts(Predicate<IScriptable> predicate);
        /// <summary>
        /// Reloads the scripts based on the predicate
        /// </summary>
        void ReloadScripts(Predicate<Script> predicate);

        /// <summary>
        /// Updates
        /// </summary>
        void Update();
    }
    public class ScriptReloadedEventArgs : EventArgs
    {
        public ScriptReloadedEventArgs(IScriptable owner)
        {
            Owner = owner;
        }

        public IScriptable Owner { get; private set; }
    }
}
