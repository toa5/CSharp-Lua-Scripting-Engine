namespace CSharp_Lua_Scripting_Engine
{
    /// <summary>
    /// For objects that contain one script
    /// </summary>
    public interface IScriptable
    {
        string GetScriptSource(ScriptType type);

        /// <summary>
        /// Either gets Lua from a file or returns actual code
        /// </summary>
        ScriptableType ScriptableType { get; }
    }

    public enum ScriptableType
    {
        File,
        Code
    }
}
