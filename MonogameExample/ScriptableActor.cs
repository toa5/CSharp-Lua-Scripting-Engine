using CSharp_Lua_Scripting_Engine;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MonogameExample
{
    public class ScriptableActor : DrawableGameComponent
    {
        protected readonly List<string> _scriptKeys = new List<string>();

        /// <summary>
        /// Should be toggled in case script change doesn't require re-intializing
        /// </summary>
        public static bool ReInitializeOnReload { get; set; } = true;

        public ScriptableActor(Game game) : base(game)
        {

        }

        protected IEnumerable<Script> Scripts
        {
            get
            {
                var engine = Engine;
                foreach (var script in engine.GetScripts(this, s => _scriptKeys.Contains(s.Source)))
                {
                    yield return script;
                }
            }
        }

        public IScriptingEngine<GameScript> Engine => Game.Services.GetService<IScriptingEngine<GameScript>>();

        public void AddScript(string source)
        {
            var engine = Engine;
            GameScript script = null;
            if(engine.TryGetScript(this, source, out script))
            {
                script.Run("LoadContent", this);
            }
            _scriptKeys.Add(source);
        }

        public void RemoveScript(string source)
        {
            var engine = Engine;
            engine.RemoveAllScripts(this, s => s.Source.Equals(source));
            _scriptKeys.RemoveAll(s => s.Equals(source));
        }

        protected override void LoadContent()
        {
            ExecuteScripts("LoadContent", this);
        }

        public override void Update(GameTime gameTime)
        {
            ExecuteScripts("Update", this, gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            ExecuteScripts("Draw", this, gameTime);
        }

        protected override void UnloadContent()
        {
            ExecuteScripts("UnloadContent", this);
            var engine = Engine;
            engine.RemoveAllScripts(this, s => true);
        }

        protected IEnumerable<object[]> EvaluateScripts(string methodName, params object[] args)
        {
            var engine = Engine;
            foreach (var script in GetScripts())
            {
                yield return script.Run(methodName, OnScriptLoaded, args);
            }
        }

        protected void ExecuteScripts(string methodName, params object[] args)
        {
            foreach (var script in GetScripts())
            {
                script.Run(methodName, OnScriptLoaded, args);
            }
        }

        protected IEnumerable<Script> GetScripts()
        {
            var engine = Engine;
            foreach(var script in engine.GetScripts(this, s => _scriptKeys.Contains(s.Source)))
            {
                yield return script;
            }
        }

        private void OnScriptLoaded(Script script)
        {
            if (script.LoadedCount <= 1 || (script.LoadedCount > 1 && ReInitializeOnReload))
            {
                script.Run("LoadContent", this);
            }
        }
    }
}
