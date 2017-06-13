using CSharp_Lua_Scripting_Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MonogameExample
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MonogameExample : Game, IScriptingEngineConsole
    {
        List<string> _logs = new List<string>();
        DefaultScriptingEngine<GameScript> engine;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        KeyboardState oldState;

        public SpriteBatch SpriteBatch => spriteBatch;
        public SpriteFont SpriteFont => spriteFont;

        public MonogameExample()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        public Texture2D LoadTexture(string assetName)
        {
            return Content.Load<Texture2D>(assetName);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            spriteFont = Content.Load<SpriteFont>("font");

            engine = new DefaultScriptingEngine<GameScript>(
                new DefaultScriptNameContainer(
                     @"C:\Users\Temdog007\Documents\GitHub\CSharp-Lua-Scripting-Engine\MonogameExample\Scripts"),
                this)
            {
                DestinationDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Scripts")
            };
            Services.AddService<IScriptingEngine<GameScript>>(engine);
        }

        private void LoadObjects()
        {
            ScriptableActor<GameScript> fpsCounter = new ScriptableActor<GameScript>(this);
            fpsCounter.SetScript("FPSCounter");
            Components.Add(fpsCounter);

            ScriptableActor<GameScript> circle = new ScriptableActor<GameScript>(this);
            circle.SetScript("Circle");
            Components.Add(circle);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            engine.Dispose();
        }

        protected override bool BeginDraw()
        {
            spriteBatch.Begin();
            return base.BeginDraw();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            base.Draw(gameTime);

            Vector2 pos = new Vector2();
            int width = GraphicsDevice.Viewport.Width;
            float size = spriteFont.MeasureString("X").Y;
            foreach (var log in _logs)
            {
                var goodLog = new string(log.Where(c => spriteFont.Characters.Contains(c)).ToArray());
                foreach (var str in goodLog.Wrap(spriteFont, width).ToArray())
                {
                    spriteBatch.DrawString(spriteFont, str, pos, Color.White);
                    pos.Y += size;
                }
            }
        }

        protected override void EndDraw()
        {
            spriteBatch.End();
            base.EndDraw();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            var state = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || state.IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            else if (state.IsKeyDown(Keys.C) && oldState.IsKeyUp(Keys.C))
            {
                Clear();
                Write("Cleared Logs");
            }
            else if (state.IsKeyDown(Keys.R) && oldState.IsKeyUp(Keys.R))
            {
                var engine = Services.GetService<IScriptingEngine<GameScript>>();
                engine.ReloadAllScripts();
                Clear();
                Write("Reloaded Scripts!");
            }
            else if (state.IsKeyDown(Keys.E) && oldState.IsKeyUp(Keys.E))
            {
                foreach (var actor in Components.
                                Where(c => c is ScriptableActor<GameScript>).
                                Select(c => c as ScriptableActor<GameScript>))
                {
                    actor.Reload();
                }
                Write("Edited content!");
            }
            else if (state.IsKeyDown(Keys.Delete) && oldState.IsKeyUp(Keys.Delete))
            {
                while (Components.Count > 0)
                {
                    Components.RemoveAt(0);
                }
                LoadObjects();
                Write("Reloaded game");
            }
            oldState = state;

            base.Update(gameTime);
        }

        #region Console

        public void Clear()
        {
            _logs.Clear();
        }

        public void DeleteAt(int index)
        {
            if (index >= 0 && index < _logs.Count)
            {
                _logs.RemoveAt(index);
            }
        }

        public void Write(string message)
        {
            _logs.Add(message);
        }

        public void Write(string message, params object[] args)
        {
            _logs.Add(string.Format(message, args));
        }

        #endregion Console
    }
}
