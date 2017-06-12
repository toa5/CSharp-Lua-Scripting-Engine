using NLua;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharp_Lua_Scripting_Engine
{
    public static class LuaExtensions
    {
        public static object[] Run(this Lua lua, string methodName, params object[] args)
        {
            var func = lua[methodName] as LuaFunction;
            return func?.Call(args);
        }

        public static IEnumerable<Tuple<object, object>> ToEnumerable(this LuaTable table)
        {
            foreach (var key in table.Keys)
            {
                yield return Tuple.Create(key, table[key]);
            }
        }

        public static Dictionary<object, object> ToDictionary(this LuaTable table)
        {
            return table.ToEnumerable().ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);
        }
    }
}
