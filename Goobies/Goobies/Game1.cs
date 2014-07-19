using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using NUnit.Framework;
using Goobies.Game_Types;
using Goobies.Game_Objects;
using Goobies.ScreenView;

namespace Goobies
{
    enum position {bottom, left, top, right};

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Texture2D dummyTexture;

        private UserScreen currentScreen;
        private Player player1;
        private Player player2;
        private List<Player> playerList;
        private Map gameMap;
        private MapModel map3D;
        private int mapHeight;
        private int mapWidth;
        private GoobiesGame game;

        private Stack<UserScreen> screenStack;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            Content.RootDirectory = "Content";

            // 720p-widescreen (Can scale to any tv)
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            // Call the update method before the draw method
            this.IsFixedTimeStep = false;
            //IsFixedTimeStep = true;
            //TargetElapsedTime = TimeSpan.FromSeconds(1.0f/60.0f);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Add initialization logic here 
            screenStack = new Stack<UserScreen>();
            screenStack.Push(new MainScreen(GraphicsDevice,this.Content,screenStack));
           
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

            dummyTexture = new Texture2D(GraphicsDevice, 1, 1);
            dummyTexture.SetData(new Color[] { Color.White });
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // Xbox Controller listener
            screenStack.Peek().listen(GamePad.GetState(PlayerIndex.One));

            // DEBUG --Keyboard Listener
            //KeyboardState newState = Keyboard.GetState();
            //screenStack.Peek().listenForKeyboard(newState);

            //Debug.WriteLine("IsRunningSlowly " + gameTime.IsRunningSlowly);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White); //Color.CornflowerBlue

            screenStack.Peek().drawScreen(spriteBatch);

            base.Draw(gameTime);
        }
    }
}
