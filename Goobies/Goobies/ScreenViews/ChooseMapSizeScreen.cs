using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Goobies.Game_Objects;

namespace Goobies.ScreenView
{
    public class ChooseMapSizeScreen : UserScreen
    {
        private GraphicsDevice graphics;
        private ContentManager content;
        private int selectedTextIndex = 0;

        private int screenCenterX;
        private int screenCenterY;

        private GamePadState prevGamePadState;
        private float thumbStickY;
        private readonly float thumbStickThreshold = .06f;

        private Map[] maps;
        private readonly int NORMAL_WIDTH = 10;
        private readonly int NORMAL_HEIGHT = 10;
        private readonly int LARGE_WIDTH = 15;
        private readonly int LARGE_HEIGHT = 15;
        private readonly int XLARGE_WIDTH = 20;
        private readonly int XLARGE_HEIGHT = 20;

        private SpriteFont titleFont;
        private String title = "Select Map";
        private Vector2 titlePos;
        private Vector2 titleOrgin;

        private SpriteFont mapOptionsFont;
        private String[] mapOptionsText = new String[] { "Normal", "Large", "X-Large" };
        private Vector2[] mapOptionPositions;

        private Camera camera;
        private CameraController cameraController;
        private MapModel[] mapModels;

        private Stack<UserScreen> screenStack;

        private readonly float TARGET_X = 5;
        private readonly float TARGET_Z = 5;

        private bool cameraChangeComplete;

        //DEBUG
        private KeyboardState oldState = Keyboard.GetState();

        public ChooseMapSizeScreen(GraphicsDevice graphics, ContentManager content, Stack<UserScreen> screenStack)
        {
            this.graphics = graphics;
            this.content = content;
            screenCenterX = graphics.Viewport.Bounds.Width / 2;
            screenCenterY = graphics.Viewport.Bounds.Height / 2;

            titleFont = content.Load<SpriteFont>("Fonts/ChooseMapScreenTitle");
            titlePos = new Vector2(screenCenterX, screenCenterY - 300);

            mapOptionsFont = content.Load<SpriteFont>("Fonts/ChooseMapScreenOptions");
            mapOptionPositions = new Vector2[] { new Vector2(screenCenterX, screenCenterY-225), new Vector2(screenCenterX, screenCenterY - 175), new Vector2(screenCenterX, screenCenterY -125) };

            maps = new Map[] {new Map(NORMAL_WIDTH,NORMAL_HEIGHT), 
                                     new Map(LARGE_WIDTH,LARGE_HEIGHT),
                                     new Map(XLARGE_WIDTH,XLARGE_HEIGHT)};

            maps[0].setMapSize(mapSize.normal);
            maps[1].setMapSize(mapSize.large);
            maps[2].setMapSize(mapSize.xLarge);

            camera = new Camera(TARGET_X, TARGET_Z, 10, 4, 1.25f);
            initializeMapModels();
            mapModels[selectedTextIndex].updateCamera(camera.getCameraPosition(), camera.getCameraTarget());
            cameraController = new CameraController(camera, mapModels[selectedTextIndex]);

            this.screenStack = screenStack;

            // DEBUG
            oldState = Keyboard.GetState();
        }

        public void initializeMapModels()
        {
            mapModels = new MapModel[maps.Count()];
            for(int i = 0; i < maps.Count();i++)
            {
                mapModels[i] = new MapModel(maps[i].getPlainMap(),content);
            }
        }

        public void drawScreen(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            titleOrgin = titleFont.MeasureString(title) / 2;
            spriteBatch.DrawString(titleFont, title, titlePos, Color.Red, 0, titleOrgin, 1.0f, SpriteEffects.None, .5f);

            for (int i = 0; i < mapOptionsText.Count(); i++)
            {
                Color textColor = Color.Red;
                if (i == selectedTextIndex)
                    textColor = Color.Blue;

                Vector2 textOrgin = mapOptionsFont.MeasureString(mapOptionsText[i]) / 2;
                spriteBatch.DrawString(mapOptionsFont, mapOptionsText[i], mapOptionPositions[i], textColor, 0, textOrgin, 1.0f, SpriteEffects.None, .5f);
            }
            spriteBatch.End();

            graphics.DepthStencilState = DepthStencilState.Default; // Fixes buffer issue when drawing 3d models
            mapModels[selectedTextIndex].drawMap();
        }

        public void listen(GamePadState gamePadState)
        {
            if (cameraController.isBusy())
            {
                cameraController.adjustCamera();
                if (!cameraController.isBusy() && cameraChangeComplete)
                    changeScreen();
            }
            listenForMovement(gamePadState);
            listenForSelection(gamePadState);
        }

        public void listenForMovement(GamePadState gamePadState)
        {
            // Listen for cursor movement and update accordingly
            thumbStickY = gamePadState.ThumbSticks.Left.Y;

            if (thumbStickY > thumbStickThreshold)
                selectedTextIndex = decrementIndex(selectedTextIndex);
            if (thumbStickY < -thumbStickThreshold)
                selectedTextIndex = incrementIndex(selectedTextIndex);

            mapModels[selectedTextIndex].updateCamera(camera.getCameraPosition(), camera.getCameraTarget());
            cameraController = new CameraController(camera, mapModels[selectedTextIndex]);
        }

        public int incrementIndex(int index)
        {
            if (index != mapOptionsText.Count() - 1)
            {
                Thread.Sleep(185);
                return ++index;
            }
            else
                return index;
        }

        public int decrementIndex(int index)
        {
            if (index != 0)
            {
                Thread.Sleep(185);
                return --index;
            }
            else
                return index;
        }

        public void listenForSelection(GamePadState gamePadState)
        {
            if (gamePadState.Buttons.A == ButtonState.Pressed && prevGamePadState.Buttons.A == ButtonState.Released)
            {
                changeScreen();
            }

            if (gamePadState.Buttons.B == ButtonState.Pressed && prevGamePadState.Buttons.B == ButtonState.Released)
            {
                screenStack.Pop();
                Thread.Sleep(200);
            }
            prevGamePadState = gamePadState;
        }

        public void changeScreen()
        {
            screenStack.Push(new ChooseMapScreen(graphics, content, maps[selectedTextIndex], screenStack));
            Thread.Sleep(200);
        }
        /*******************************************************************/
        /*  DEBUG -- Keyboard listeners
        /*******************************************************************/

        public void listenForKeyboard(KeyboardState newState)
        {
            if (cameraController.isBusy())
            {
                cameraController.adjustCamera();
                if (!cameraController.isBusy() && cameraChangeComplete)
                    changeScreen();
            }
            listenForKeyboardMovement(newState);
            listenForKeyboardSelection(newState);

            oldState = newState;
        }

        public void listenForKeyboardMovement(KeyboardState newState)
        {
            // Listen for cursor movement and update accordingly
            if (newState.IsKeyDown(Keys.Up))
            {
                if (!oldState.IsKeyDown(Keys.Up))
                {
                    selectedTextIndex = decrementIndex(selectedTextIndex);
                }
            }

            if (newState.IsKeyDown(Keys.Down))
            {
                if (!oldState.IsKeyDown(Keys.Down))
                {
                    selectedTextIndex = incrementIndex(selectedTextIndex);
                }
            }

            mapModels[selectedTextIndex].updateCamera(camera.getCameraPosition(), camera.getCameraTarget());
            cameraController = new CameraController(camera, mapModels[selectedTextIndex]);
        }

        public void listenForKeyboardSelection(KeyboardState newState)
        {
            if (newState.IsKeyDown(Keys.Enter))
            {
                if (!oldState.IsKeyDown(Keys.Enter))
                {
                    changeScreen();
                }
            }
            else if(newState.IsKeyDown(Keys.Back))
            {
                if(!oldState.IsKeyDown(Keys.Back))
                {
                    screenStack.Pop();
                    Thread.Sleep(200);
                }
            }
        }
    }
}
