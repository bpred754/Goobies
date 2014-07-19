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
using Goobies.ScreenView;

namespace Goobies.Game_Objects
{
    public class SelectUnitsPlayerController
    {
        private Player player;
        private GoobiesSelectorBox selectorBox;
        private Map map;
        private int team;
        private GamePadState prevGamePadState;
        private float thumbStickX;
        private float thumbStickY;
        private readonly float thumbStickThreshold = .06f;

        private Cursor cursor;
        private Direction moveDirection;

        // DEBUG
        private KeyboardState oldState;

        public SelectUnitsPlayerController(Player player, GoobiesSelectorBox selectorBox, Map map)
        {
            this.player = player;
            this.selectorBox = selectorBox;
            this.map = map;
            team = player.getTeam();
            cursor = player.getCursor();
            moveDirection = new Direction(compassDirection.west);

            // DEBUG
            oldState = Keyboard.GetState();
        }

        public void listen(GamePadState gamePadState)
        {
            listenForCursorMovement(gamePadState);
            listenForSelectorBoxChange(gamePadState);
        }

        public void listenForCursorMovement(GamePadState gamePadState)
        {
            direction direction = getDirection(gamePadState);

            if (direction != direction.none)
            {
                cursor.redesignateCurrentTerritoryModel(); // Reset territory model back to its former model

                compassDirection desiredDirection = moveDirection.getCompassDirection(direction);
                cursor.move(desiredDirection);

                if (map.get(cursor.getXLocation(), cursor.getYLocation()).getTeam() == -1)
                    cursor.move(moveDirection.opposite(desiredDirection));

                cursor.updateTerritory(); // Update the new territory model with the player's cursor
                Thread.Sleep(150);
            }
        }

        public direction getDirection(GamePadState gamePadState)
        {
            direction direction = direction.none;

            thumbStickX = gamePadState.ThumbSticks.Left.X;
            thumbStickY = gamePadState.ThumbSticks.Left.Y;

            if ((thumbStickX == 0 && thumbStickY == 0))
                direction = direction.none;
            else // Determine the desired direction based on the thumb sticks position
            {
                if (thumbStickY > thumbStickThreshold)
                {
                    if (thumbStickX < -thumbStickThreshold)
                        direction = direction.upLeft;
                    else if (thumbStickX >= -thumbStickThreshold && thumbStickX <= thumbStickThreshold)
                        direction = direction.up;
                    else if (thumbStickX > thumbStickThreshold)
                        direction = direction.upRight;
                }
                else if (thumbStickY >= -thumbStickThreshold && thumbStickY <= thumbStickThreshold)
                {
                    if (thumbStickX < -thumbStickThreshold)
                        direction = direction.left;
                    else if (thumbStickX > thumbStickThreshold)
                        direction = direction.right;
                }
                else if (thumbStickY < -thumbStickThreshold)
                {
                    if (thumbStickX < -thumbStickThreshold)
                        direction = direction.downLeft;
                    else if (thumbStickX >= -thumbStickThreshold && thumbStickX <= thumbStickThreshold)
                        direction = direction.down;
                    else if (thumbStickX > thumbStickThreshold)
                        direction = direction.downRight;
                }
            }
            return direction;
        }

        public void listenForSelectorBoxChange(GamePadState gamePadState)
        {
            if (gamePadState.Triggers.Left > .5)
            {
                selectorBox.changeGoobieSelectorBox(direction.left);
                Thread.Sleep(150);
            }
            else if (gamePadState.Triggers.Right > .5)
            {
                selectorBox.changeGoobieSelectorBox(direction.right);
                Thread.Sleep(150);
            }

            if (gamePadState.Buttons.A == ButtonState.Pressed && prevGamePadState.Buttons.A == ButtonState.Released)
            {
                addGoobie();
            }

            prevGamePadState = gamePadState;
        }

        public void addGoobie()
        {
            Cursor cursor = player.getCursor();
            int x = cursor.getXLocation();
            int y = cursor.getYLocation();
            Unit goobie = map.get(x,y).getGooby();
            if (selectorBox.getIndex() == 0)
            {
                if (!(goobie is CircleGooby))
                {
                    Unit newGoobie = new CircleGooby(map,team, x, y);

                    if (goobie != null)
                        player.removeUnitAt(x,y);

                    player.addUnit(newGoobie);
                }
            }
            else if (selectorBox.getIndex() == 1)
            {
                if (!(goobie is SquareGooby))
                {
                    Unit newGoobie = new SquareGooby(map, team, x, y);

                    if (goobie != null)
                        player.removeUnitAt(x, y);

                    player.addUnit(newGoobie);
                }
            }
            else if (selectorBox.getIndex() == 2)
            {
                if (!(goobie is TriangleGooby))
                {
                    Unit newGoobie = new TriangleGooby(map, team, x, y);

                    if (goobie != null)
                        player.removeUnitAt(x, y);

                    player.addUnit(newGoobie);
                }
            }
            else if (selectorBox.getIndex() == 3)
            {
                if (!(goobie is DiamondGooby))
                {
                    Unit newGoobie = new DiamondGooby(map, team, x, y);

                    if (goobie != null)
                        player.removeUnitAt(x, y);

                    player.addUnit(newGoobie);
                }
            }

        }

        // Initialize all the territories that contain units to show ownership
        public void initializeTeamTerritories(Vector2[] startingLocations)
        {
            for (int i = 0; i < startingLocations.Count(); i++)
            {
                int x = (int)startingLocations[i].X;
                int y = (int)startingLocations[i].Y;
                Territory territory = player.getTerritory(x,y);

                territory.setTeam(team);
                territory.designateModel(ModelMode.ownership, team);
            }
        }

        /*******************************************************************/
        /*  DEBUG -- Keyboard listeners
        /*******************************************************************/

        public void listenForKeyboard(KeyboardState newState)
        {
            listenForKeyboardCursorMovement(newState);
            listenForKeyboardSelectorBoxChange(newState);

            oldState = newState;
        }

        public void listenForKeyboardCursorMovement(KeyboardState newState)
        {
            direction direction = getKeyboardDirection(newState);

            if (direction != direction.none)
            {
                cursor.redesignateCurrentTerritoryModel(); // Reset territory model back to its former model

                compassDirection desiredDirection = moveDirection.getCompassDirection(direction);
                cursor.move(desiredDirection);

                if (map.get(cursor.getXLocation(), cursor.getYLocation()).getTeam() == -1)
                    cursor.move(moveDirection.opposite(desiredDirection));

                cursor.updateTerritory(); // Update the new territory model with the player's cursor
                Thread.Sleep(150);
            }
        }

        public void listenForKeyboardSelectorBoxChange(KeyboardState newState)
        {
            if (newState.IsKeyDown(Keys.Left))
            {
                if (!oldState.IsKeyDown(Keys.Left))
                {
                    selectorBox.changeGoobieSelectorBox(direction.left);
                    Thread.Sleep(150);
                }
            }
            else if (newState.IsKeyDown(Keys.Right))
            {
                if (!oldState.IsKeyDown(Keys.Right))
                {
                    selectorBox.changeGoobieSelectorBox(direction.right);
                    Thread.Sleep(150);
                }
            }
            else if (newState.IsKeyDown(Keys.Space))
            {
                if (!oldState.IsKeyDown(Keys.Space))
                {
                    addGoobie();
                }
            }
        }

        public direction getKeyboardDirection(KeyboardState newState)
        {
            direction direction = direction.none;

            if (newState.IsKeyDown(Keys.NumPad7))
            {
                if (!oldState.IsKeyDown(Keys.NumPad7))
                {
                    direction = direction.upLeft;
                }
            }
            else if (newState.IsKeyDown(Keys.NumPad8))
            {
                if (!oldState.IsKeyDown(Keys.NumPad8))
                {
                    direction = direction.up;
                }
            }
            else if (newState.IsKeyDown(Keys.NumPad9))
            {
                if (!oldState.IsKeyDown(Keys.NumPad9))
                {
                    direction = direction.upRight;
                }
            }
            else if (newState.IsKeyDown(Keys.NumPad4))
            {
                if (!oldState.IsKeyDown(Keys.NumPad4))
                {
                    direction = direction.left;
                }
            }
            else if (newState.IsKeyDown(Keys.NumPad5))
            {
                if (!oldState.IsKeyDown(Keys.NumPad5))
                {
                    direction = direction.none;
                }
            }
            else if (newState.IsKeyDown(Keys.NumPad6))
            {
                if (!oldState.IsKeyDown(Keys.NumPad6))
                {
                    direction = direction.right;
                }
            }
            else if (newState.IsKeyDown(Keys.NumPad1))
            {
                if (!oldState.IsKeyDown(Keys.NumPad1))
                {
                    direction = direction.downLeft;
                }
            }
            else if (newState.IsKeyDown(Keys.NumPad2))
            {
                if (!oldState.IsKeyDown(Keys.NumPad2))
                {
                    direction = direction.down;
                }
            }
            else if (newState.IsKeyDown(Keys.NumPad3))
            {
                if (!oldState.IsKeyDown(Keys.NumPad3))
                {
                    direction = direction.downRight;
                }
            }

            return direction;
        }
    }
}
