namespace CSharp_Lua_Scripting_Engine
{
    public interface IScriptable
    {
        string GetScriptSource(object scriptType);

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
