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
    class ChooseMapScreen : UserScreen
    {
        private GraphicsDevice graphics;
        private ContentManager content;
        private Stack<UserScreen> screenStack;

        private String title = "Select Map";
        private SpriteFont titleFont;
        private Vector2 titlePos;
        private Vector2 titleOrgin;

        private String leftArrow = "<";
        private String rightArrow = ">";
        private SpriteFont arrowFont;
        private Vector2 leftArrowPosition;
        private Vector2 rightArrowPosition;
        private Vector2 leftArrowOrgin;
        private Vector2 rightArrowOrgin;
        private Color leftArrowColor = Color.Red;
        private Color rightArrowColor = Color.Red;

        private int screenCenterX;
        private int screenCenterY;
        private int selectedMapIndex = 1;

        private GamePadState prevGamePadState;
        private float thumbStickX;
        private readonly float thumbStickThreshold = .25f;

        private Camera camera;
        private MapModel[] mapModels;

        private Map[] maps;

        // DEBUG
        private KeyboardState oldState;

        public ChooseMapScreen(GraphicsDevice graphics, ContentManager content, Map map, Stack<UserScreen> screenStack)
        {
            this.graphics = graphics;
            this.content = content;
            screenCenterX = graphics.Viewport.Bounds.Width / 2;
            screenCenterY = graphics.Viewport.Bounds.Height / 2;

            titleFont = content.Load<SpriteFont>("Fonts/ChooseMapScreenTitle");
            titlePos = new Vector2(screenCenterX, screenCenterY - 300);

            arrowFont = content.Load<SpriteFont>("Fonts/MainScreenTitle");
            leftArrowPosition = new Vector2(50, screenCenterY-125);
            rightArrowPosition = new Vector2(graphics.Viewport.Bounds.Width-50,screenCenterY-125);

            maps = new Map[3];
            for (int i = 0; i < 3; i++)
            {
                maps[i] = new Map(map.getWidth(), map.getHeight());
                maps[i].setMapSize(map.getMapSize());
                Thread.Sleep(100);
            }

            initializeCamera(map);
            initializeMapModels();
            mapModels[selectedMapIndex].updateCamera(camera.getCameraPosition(), camera.getCameraTarget());

            this.screenStack = screenStack;

            // DEBUG
            oldState = Keyboard.GetState();
        }

        public void initializeCamera(Map map)
        {
            if (map.getMapSize() == mapSize.normal)
                camera = new Camera(new Vector3(-5, 6, map.getHeight() + 4), new Vector3(3, 1.25f, map.getHeight() - 4), 3, compassDirection.west);
            else if (map.getMapSize() == mapSize.large)
                camera = new Camera(new Vector3(-5, 9, map.getHeight() + 4), new Vector3(5, 1.25f, map.getHeight() - 6), 3, compassDirection.west);
            else if (map.getMapSize() == mapSize.xLarge)
                camera = new Camera(new Vector3(-5, 12, map.getHeight() + 4), new Vector3(8, 1.25f, map.getHeight() - 9), 3, compassDirection.west);
        }

        public void initializeMapModels()
        {
            mapModels = new MapModel[maps.Count()];
            for (int i = 0; i < maps.Count(); i++)
            {
                mapModels[i] = new MapModel(maps[i], content);
            }
        }

        public void listen(GamePadState gamePadState)
        {
            listenForMovement(gamePadState);
            listenForSelection(gamePadState);
        }

        public void listenForMovement(GamePadState gamePadState)
        {
            // Listen for cursor movement and update accordingly
            thumbStickX = gamePadState.ThumbSticks.Left.X;

            if (thumbStickX > thumbStickThreshold)
                selectedMapIndex = incrementIndex(selectedMapIndex);
            if (thumbStickX < -thumbStickThreshold)
                selectedMapIndex = decrementIndex(selectedMapIndex);
            mapModels[selectedMapIndex].updateCamera(camera.getCameraPosition(), camera.getCameraTarget());
        }

        public int incrementIndex(int index)
        {
            if (index != maps.Count() - 1)
            {
                Thread.Sleep(225);
                return ++index;
            }
            else
                return index;
        }

        public int decrementIndex(int index)
        {
            if (index != 0)
            {
                Thread.Sleep(225);
                return --index;
            }
            else
                return index;
        }

        public void listenForSelection(GamePadState gamePadState)
        {
            if (gamePadState.Buttons.A == ButtonState.Pressed && prevGamePadState.Buttons.A == ButtonState.Released)
            {
                screenStack.Push(new SelectUnitsScreen(graphics, content, mapModels[selectedMapIndex],screenStack, camera));
                Thread.Sleep(200);
            }

            if (gamePadState.Buttons.B == ButtonState.Pressed && prevGamePadState.Buttons.B == ButtonState.Released)
            {
                screenStack.Pop();
                Thread.Sleep(200);
            }

            prevGamePadState = gamePadState;
        }

        public void drawScreen(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            titleOrgin = titleFont.MeasureString(title) / 2;
            leftArrowOrgin = arrowFont.MeasureString(leftArrow) / 2;
            rightArrowOrgin = arrowFont.MeasureString(rightArrow) / 2;

            spriteBatch.DrawString(titleFont, title, titlePos, Color.Red, 0, titleOrgin, 1.0f, SpriteEffects.None, .5f);

            if (selectedMapIndex == 0)
                leftArrowColor = Color.White;
            else
                leftArrowColor = Color.Red;

            if (selectedMapIndex == maps.Count() - 1)
                rightArrowColor = Color.White;
            else
                rightArrowColor = Color.Red;

            spriteBatch.DrawString(arrowFont, leftArrow, leftArrowPosition, leftArrowColor, 0, leftArrowOrgin, 1.0f, SpriteEffects.None, .5f);
            spriteBatch.DrawString(arrowFont, rightArrow, rightArrowPosition, rightArrowColor, 0, rightArrowOrgin, 1.0f, SpriteEffects.None, .5f);

            spriteBatch.End();

            graphics.DepthStencilState = DepthStencilState.Default; // Fixes buffer issue when drawing 3d models
            mapModels[selectedMapIndex].drawMap();
        }

        /*******************************************************************/
        /*  DEBUG -- Keyboard listeners
        /*******************************************************************/

        public void listenForKeyboard(KeyboardState newState)
        {
            listenForKeyboardMovement(newState);
            listenForKeyboardSelection(newState);

            oldState = newState;
        }

        public void listenForKeyboardMovement(KeyboardState newState)
        {
            // Listen for cursor movement and update accordingly
            if (newState.IsKeyDown(Keys.Right))
            {
                if (!oldState.IsKeyDown(Keys.Right))
                {
                    selectedMapIndex = incrementIndex(selectedMapIndex);
                }
            }

            if (newState.IsKeyDown(Keys.Left))
            {
                if (!oldState.IsKeyDown(Keys.Left))
                {
                    selectedMapIndex = decrementIndex(selectedMapIndex);
                }
            }

            mapModels[selectedMapIndex].updateCamera(camera.getCameraPosition(), camera.getCameraTarget());
        }

        public void listenForKeyboardSelection(KeyboardState newState)
        {
            if (newState.IsKeyDown(Keys.Enter))
            {
                if (!oldState.IsKeyDown(Keys.Enter))
                {
                    screenStack.Push(new SelectUnitsScreen(graphics, content, mapModels[selectedMapIndex], screenStack, camera));
                    Thread.Sleep(200);
                }
            }
            else if (newState.IsKeyDown(Keys.Back))
            {
                if (!oldState.IsKeyDown(Keys.Back))
                {
                    screenStack.Pop();
                    Thread.Sleep(200);
                }
            }
        }
    }
}
