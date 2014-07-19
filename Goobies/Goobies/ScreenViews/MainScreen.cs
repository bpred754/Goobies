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

namespace Goobies.ScreenView
{
    public class MainScreen : UserScreen
    {
        private GraphicsDevice graphics;
        private ContentManager content;
        private int selectedTextIndex = 0;

        private int screenCenterX;
        private int screenCenterY;

        private GamePadState prevGamePadState;
        private float thumbStickX;
        private float thumbStickY;
        private readonly float thumbStickThreshold = .06f;

        private Stack<UserScreen> screenStack;

        private SpriteFont titleFont;
        private String title = "Goobies";
        private Vector2 titlePos;
        private Vector2 titleOrgin;

        private SpriteFont menuOptionsFont;
        private String[] menuOptions = new String[] {"Start Game", "Getting Started"};
        private Vector2[] menuPositions;

        // DEBUG
        private KeyboardState oldState;
        
        public MainScreen(GraphicsDevice graphics, ContentManager content, Stack<UserScreen> screenStack)
        {
            this.graphics = graphics;
            this.content = content;
            screenCenterX = graphics.Viewport.Bounds.Width/2;
            screenCenterY = graphics.Viewport.Bounds.Height / 2;

            titleFont = content.Load<SpriteFont>("Fonts/MainScreenTitle");
            titlePos = new Vector2(screenCenterX, screenCenterY - 200);

            menuOptionsFont = content.Load<SpriteFont>("Fonts/MainScreenOptions");
            menuPositions = new Vector2[] {new Vector2(screenCenterX,screenCenterY), new Vector2(screenCenterX,screenCenterY+100)};

            this.screenStack = screenStack;

            // DEBUG
            oldState = Keyboard.GetState();
        }


        public void drawScreen(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            titleOrgin = titleFont.MeasureString(title) / 2;
            spriteBatch.DrawString(titleFont,title,titlePos,Color.Red,0,titleOrgin,1.0f,SpriteEffects.None,.5f);

            for (int i = 0; i < menuOptions.Count(); i++)
            {
                Color textColor = Color.Red;
                if (i == selectedTextIndex)
                    textColor = Color.Blue;

                Vector2 textOrgin = menuOptionsFont.MeasureString(menuOptions[i]) / 2;
                spriteBatch.DrawString(menuOptionsFont, menuOptions[i], menuPositions[i], textColor, 0, textOrgin, 1.0f, SpriteEffects.None, .5f);
            }
            spriteBatch.End();
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
            thumbStickY = gamePadState.ThumbSticks.Left.Y;

            if (thumbStickY > thumbStickThreshold)
                selectedTextIndex = decrementIndex(selectedTextIndex);
            if (thumbStickY < -thumbStickThreshold)
                selectedTextIndex = incrementIndex(selectedTextIndex);
        }

        public int incrementIndex(int index)
        {
            if (index != menuOptions.Count() - 1)
            {
                //Thread.Sleep(175);
                return ++index;
            }
            else
                return index;
        }

        public int decrementIndex(int index)
        {
            if (index != 0)
            {
                //Thread.Sleep(175);
                return --index;
            }
            else
                return index;
        }

        public void listenForSelection(GamePadState gamePadState)
        {
            if (gamePadState.Buttons.A == ButtonState.Pressed && prevGamePadState.Buttons.A == ButtonState.Released)
            {
                if (selectedTextIndex == 0)
                    screenStack.Push(new ChooseMapSizeScreen(graphics,content,screenStack));

                Thread.Sleep(200);
            }
            prevGamePadState = gamePadState;
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
        }

        public void listenForKeyboardSelection(KeyboardState newState)
        {
            if (newState.IsKeyDown(Keys.Enter))
            {
                if (!oldState.IsKeyDown(Keys.Enter))
                {
                    if (selectedTextIndex == 0)
                        screenStack.Push(new ChooseMapSizeScreen(graphics, content, screenStack));

                    Thread.Sleep(200);
                }
            }
        }
    }
}
