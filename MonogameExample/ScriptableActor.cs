using CSharp_Lua_Scripting_Engine;
using Microsoft.Xna.Framework;
using NLua;

namespace MonogameExample
{
    public class ScriptableActor<T> : DrawableGameComponent where T : Script
    {
        public ScriptableActor(Game game) : base(game)
        {

        }

        private LuaTable LuaTable { get; set; }

        public IScriptingEngine<T> Engine => Game.Services.GetService<IScriptingEngine<T>>();

        public T Script { get; set; }

        public void SetScript(string source)
        {
            var engine = Engine;
            T script = null;
            if(engine.TryGetScript(this, source, out script))
            {
                script.LuaExceptionReaction = LuaExceptionReaction.Throw;
                LuaTable = script.Run<LuaTable>("LoadContent", this);
                Script = script;
            }
        }

        public void RemoveScript(string name)
        {
            var engine = Engine;
            engine.RemoveAllScripts(this, s => s.Name.Equals(name));
        }

        public override void Update(GameTime gameTime)
        {
            LuaTable["GameTime"] = gameTime;
            Script.Run("Update", LuaTable);
        }

        public override void Draw(GameTime gameTime)
        {
            LuaTable["GameTime"] = gameTime;
            Script.Run("Draw", LuaTable);
        }

        protected override void UnloadContent()
        {
            Script.Run("UnloadContent", LuaTable);
            var engine = Engine;
            engine.RemoveAllScripts(this, s => true);
        }

        public void Reload()
        {
            Script.Run("EditContent", LuaTable);
        }
    }
}
