using System.Collections.Generic;
using System.IO;

namespace CSharp_Lua_Scripting_Engine
{
    public class DefaultScriptNameContainer : Dictionary<string, string>, IScriptNameContainer
    {
        public DefaultScriptNameContainer(string sourceDirectory)
        {
            SourceDirectory = sourceDirectory;
            LoadFiles(sourceDirectory);
        }

        private void LoadFiles(string directory)
        {
            foreach(var dir in Directory.GetDirectories(directory))
            {
                LoadFiles(dir);
            }

            foreach(var file in Directory.GetFiles(directory))
            {
                Add(file);
            }
        }

        public string SourceDirectory { get; private set; }

        public void Add(string source)
        {
            Add(Path.GetFileNameWithoutExtension(source), source);
        }

        public bool HasScript(string name)
        {
            return ContainsKey(name);
        }
    }
}
