namespace CSharp_Lua_Scripting_Engine
{
    public interface IScriptNameContainer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Source path of the script</returns>
        string this[string name] { get; }

        string SourceDirectory { get; }

        void Add(string name, string source);
    }
}
