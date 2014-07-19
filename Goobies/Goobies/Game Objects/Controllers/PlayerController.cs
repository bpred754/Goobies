using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Goobies.Game_Objects
{
    public class PlayerController
    {
        private Player player;
        private Cursor cursor;
        private CameraController cameraController;
        private bool changeTurnFlag;

        // Controller listeners
        private GamePadState prevGamePadStateX;
        private GamePadState prevGamePadStateA;
        private GamePadState prevGamePadStateLS;
        private GamePadState prevGamePadStateRS;

        // Movement
        private float thumbStickX;
        private float thumbStickY;
        private readonly float thumbStickThreshold = .06f;
        private direction currentDirection;

        // DEBUG
        private KeyboardState oldState;
        
        public PlayerController(Player player, CameraController cameraController)
        {
            this.player = player;
            this.cursor = player.getCursor();
            this.cameraController = cameraController;

            // Mark territory ownership for starting goobies
            initializeTeamTerritories();

            //DEBUG
            oldState = Keyboard.GetState();
        }

        // Listens for Player Action
        public void listen(GamePadState gamePadState)
        {
            cameraController.listen(gamePadState); // Listen and perform camera rotations

            if (cameraController.isBusy() == false)
            {
                listenForMovement(gamePadState);
                listenForTerritorySelection(gamePadState);
                listenForUnitChange(gamePadState);
                listenForTurnChange(gamePadState);
            }
        }

        public void listenForMovement(GamePadState gamePadState)
        {
            // Listen for cursor movement and update accordingly
            currentDirection = getDirection(gamePadState);
            
            // Move the cursor in the intended direction
            if (currentDirection != direction.none)
            {
                Camera camera = cameraController.getCamera();
                float targetStartX = cursor.getXLocation();
                float targetStartZ = cursor.getYLocation();

                cursor.redesignateCurrentTerritoryModel(); // Reset territory model back to its former model
                cursor.move(camera.getDirection().getCompassDirection(currentDirection));

                cursor.updateTerritory(); // Update the new territory model with the player's cursor. **CREATES INITIAL LAG**
                cursor.findAction(player); // Determine if the new territory has a possible action
                cameraController.initializeCameraMovement(camera.getFacingDirection(), targetStartX, targetStartZ, cursor.getXLocation(), cursor.getYLocation(), 20); // Initialize the Camera movement
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

        public void listenForTurnChange(GamePadState gamePadState)
        {
            if (gamePadState.Buttons.X == ButtonState.Pressed && prevGamePadStateX.Buttons.X == ButtonState.Released)
            {
                changeTurnFlag = true; // Notify game class that this player's turn is over
                player.resetEnergy(); // Reset all of the player's units energy to their max
                deSelectUnit(); // Unselect the player's selected unit
            }

            prevGamePadStateX = gamePadState;
        }

        public void listenForTerritorySelection(GamePadState gamePadState)
        {
            // Listener for A button press -- performs action on the current territory
            if (gamePadState.Buttons.A == ButtonState.Pressed && prevGamePadStateA.Buttons.A == ButtonState.Released)
                performAction(player.getCursor().getAction());

            prevGamePadStateA = gamePadState;
        }

        public void listenForUnitChange(GamePadState gamePadState)
        {
            if (gamePadState.Buttons.LeftShoulder == ButtonState.Pressed && prevGamePadStateLS.Buttons.LeftShoulder == ButtonState.Released)
                findUnit(direction.left);
           
            else if (gamePadState.Buttons.RightShoulder == ButtonState.Pressed && prevGamePadStateRS.Buttons.RightShoulder == ButtonState.Released)
                findUnit(direction.right);

            prevGamePadStateLS = gamePadState;
            prevGamePadStateRS = gamePadState;
        }

        // Finds the currently selected unit, switches units, or finds the recently last used unit
        public void findUnit(direction direction)
        {
            Unit selectedUnit = player.getSelectedUnit();

            if (selectedUnit != null)
            {
                deSelectUnit();
                // If the cursor is on the selected unit, change units
                if (cursor.getXLocation() == selectedUnit.getXPosition() && cursor.getYLocation() == selectedUnit.getYPosition())
                {
                    if (direction == direction.left)
                        player.decrementUnitIndex();
                    else
                        player.incrementUnitIndex();
                }
            }

            moveCameraToUnit();
            selectUnit(cursor.getXLocation(), cursor.getYLocation());

            // Find new action designated for this new position
            player.getCursor().findAction(player);
        }

        // Moves the camera to the player's current unit
        public void moveCameraToUnit()
        {
            // Determine the start and end target positions
            int targetStartX = cursor.getXLocation();
            int targetStartZ = cursor.getYLocation();
            int targetEndX = player.getUnitAt(player.getUnitIndex()).getXPosition();
            int targetEndZ = player.getUnitAt(player.getUnitIndex()).getYPosition();

            // Determine the next camera angle given the desired x and y position
            compassDirection nextCameraAngle = cameraController.getCamera().getCameraAngleAt(cursor.getMap().getWidth(), cursor.getMap().getHeight(), targetEndX, targetEndZ);

            // Configure the camera movement to reach the desired unit's location
            cameraController.initializeCameraMovement(nextCameraAngle, targetStartX, targetStartZ, targetEndX, targetEndZ, 30);

            // Reset the cursor model at the current cursor location
            player.getTerritory(cursor.getXLocation(), cursor.getYLocation()).redesignateModel();

            // Update territory model to reflect the cursors new position
            player.setCursorPosition(new Vector2(targetEndX, targetEndZ));
            player.getTerritory(targetEndX, targetEndZ).designateModel(ModelMode.cursor, player.getTeam());
        }

        public void performAction(action action)
        {
            if (action == action.attack)
            {
                player.attackAtCursorPosition();

                redesignateTerritoryModels(); // clear all the model strings from all surrounding territories
                player.updatePossibleTerritories(); // Update all the territory models to match this player's possible actions
                player.checkUnitActions(); // Unselect this player's selected unit if it does not have any more possible actions
            }
            else if (action == action.move)
            {
                player.moveSelectedUnitToCursor();
                Territory currentTerritory = player.getTerritory(cursor.getXLocation(), cursor.getYLocation());
                int team = player.getTeam();

                currentTerritory.resetStack(); // Remove all model strings from stack except for the original
                currentTerritory.designateModel(ModelMode.ownership, team); // place ownership model string onto stack
                currentTerritory.designateModel(ModelMode.cursor, team); // place cursor model string onto stack
                currentTerritory.designateModel(ModelMode.cursor, team); // Extra one will be removed when redesignateTerritoryModel is called
                redesignateTerritoryModels(); // clear all the model strings from all surrounding territories
                player.updatePossibleTerritories(); // Update all the surrounding territories to match all territories with actions
                player.checkUnitActions();// Unselect this player's selected unit if it does not have any more possible actions
            }
            else if (action == action.select)
            {
                if (player.getSelectedUnit() != null) // If the player has previously selected a unit, deselect it
                    deSelectUnit();
                else // If the player has NOT previously selected a unit, select it
                    selectUnit(cursor.getXLocation(), cursor.getYLocation());
            }
            else if (action == action.deselect)
                deSelectUnit();

            cursor.findAction(player); // find a new action for the territory
        }

        public void deSelectUnit()
        {
            player.setSelectedUnit(null); // This player no longer has a gooby selected
            redesignateTerritoryModels(); // Update surrounding territories to their original states before the unit was selected
        }

        // Selects the unit at the given x and y coordinates
        public void selectUnit(int x, int y)
        {
            player.setSelectedUnit(player.getTerritory(x,y).getGooby()); // set this player's selected unit field
            player.updatePossibleTerritories(); // Update all the surrounding territories to match all territories with actions
        }

        // Initialize all the territories that contain units to show ownership
        public void initializeTeamTerritories()
        {
            List<Unit> unitList = player.getUnitList();

            for (int i = 0; i < unitList.Count; i++)
            {
                int x = unitList.ElementAt(i).getXPosition();
                int y = unitList.ElementAt(i).getYPosition();

                player.getTerritory(x, y).designateModel(ModelMode.ownership, player.getTeam());
            }
        }

        // Have all the territories with a possible action return to their original state
        public void redesignateTerritoryModels()
        {
            List<Territory> possibleTerritories = player.getPossibleTerritories();

            for (int i = 0; i < possibleTerritories.Count; i++)
                possibleTerritories.ElementAt(i).redesignateModel();

            possibleTerritories.Clear();
        }

        /*******************************************************************/
        /*  SETTERS AND GETTERS
        /*******************************************************************/

        public void setChangeTurnFlag(bool flag)
        {
            changeTurnFlag = flag;
        }

        // Check if this player's turn is over
        public bool getChangeTurnFlag()
        {
            return changeTurnFlag;
        }

        /*******************************************************************/
        /*  DEBUG -- Keyboard listeners
        /*******************************************************************/

        // Listen for player action
        public void listenForKeyboard(KeyboardState newState)
        {
            cameraController.listenForKeyboard(newState); // Listen and perform camera rotations

            if (!cameraController.isBusy())
            {
                listenForKeyboardMovement(newState);
                listenForKeyboardTerritorySelection(newState);
                listenForKeyboardUnitChange(newState);
                listenForKeyboardTurnChange(newState);
            }

            oldState = newState;
        }

        public void listenForKeyboardMovement(KeyboardState newState)
        {
            // Listen for cursor movement and update accordingly
            currentDirection = getKeyboardDirection(newState);

            // Move the cursor in the intended direction
            if (currentDirection != direction.none)
            {
                Camera camera = cameraController.getCamera();
                float targetStartX = cursor.getXLocation();
                float targetStartZ = cursor.getYLocation();

                cursor.redesignateCurrentTerritoryModel(); // Reset territory model back to its former model
                cursor.move(camera.getDirection().getCompassDirection(currentDirection));

                cursor.updateTerritory(); // Update the new territory model with the player's cursor. **CREATES INITIAL LAG**
                cursor.findAction(player); // Determine if the new territory has a possible action
                cameraController.initializeCameraMovement(camera.getFacingDirection(), targetStartX, targetStartZ, cursor.getXLocation(), cursor.getYLocation(), 20); // Initialize the Camera movement
            }
        }

        public void listenForKeyboardTerritorySelection(KeyboardState newState)
        {
            if (newState.IsKeyDown(Keys.Enter))
            {
                if (!oldState.IsKeyDown(Keys.Enter))
                {
                    performAction(player.getCursor().getAction());
                }
            }
        }

        public void listenForKeyboardUnitChange(KeyboardState newState)
        {
            if (newState.IsKeyDown(Keys.OemComma))
            {
                if (!oldState.IsKeyDown(Keys.OemComma))
                {
                    findUnit(direction.left);
                }
            }
            else if (newState.IsKeyDown(Keys.OemPeriod))
            {
                if (!oldState.IsKeyDown(Keys.OemPeriod))
                {
                    findUnit(direction.right);
                }
            }
        }

        public void listenForKeyboardTurnChange(KeyboardState newState)
        {
            if (newState.IsKeyDown(Keys.Back))
            {
                if (!oldState.IsKeyDown(Keys.Back))
                {
                    changeTurnFlag = true; // Notify game class that this player's turn is over
                    player.resetEnergy(); // Reset all of the player's units energy to their max
                    deSelectUnit(); // Unselect the player's selected unit
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

