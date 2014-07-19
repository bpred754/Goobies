using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Goobies.Game_Objects;

namespace Goobies.Game_Types
{
    /** Make this an abstract super class when making other game types **/

    public class GoobiesGame
    {
        private Map map;
        private Camera camera;
        private CameraController cameraController;
        private List<Player> playerList;
        private PlayerController[] playerControllers;
        private Player currentPlayer;
        private Player nextPlayer;
        private int currentPlayerIndex = 0;
        private int nextPlayerIndex = 1;

        public GoobiesGame(MapModel mapModel, List<Player> playerList)
        {
            this.map = mapModel.getMap();
            this.playerList = playerList;
            currentPlayer = playerList.ElementAt(currentPlayerIndex);
            nextPlayer = playerList.ElementAt(nextPlayerIndex);
            
            Cursor playerCursor = currentPlayer.getCursor();
            camera = new Camera(playerCursor.getXLocation(), playerCursor.getYLocation(), 3, 3, 1.25f);
            cameraController = new CameraController(camera,mapModel);

            // TODO: Use Factory Pattern to make player controller objects since there will be more than one type
            // Place startingLocations into the correct playerController factory

            // Initialize Player Controllers for each player
            playerControllers = new PlayerController[playerList.Count()];
            for (int i = 0; i < playerControllers.Count(); i++)
                playerControllers[i] = new PlayerController(playerList.ElementAt(i), cameraController);

            // Update the current territory to show the player's cursor
            playerCursor.updateTerritory();

            // Find the first players initial action
            playerCursor.findAction(currentPlayer);

            // Initial Game View
            mapModel.updateCamera(camera.getCameraPosition(), camera.getCameraTarget());
        }

        public void listen(GamePadState gamePadState)
        {
             listenForTurnChange();
             playerControllers.ElementAt(currentPlayerIndex).listen(gamePadState);  // Only the current player's player controller should be listening
        }

        // When the current player sets his change turn flag, find the new next player and move the camera to their position
        public void listenForTurnChange()
        {
            if (cameraController.isBusy() == false)
            {
                if (playerControllers.ElementAt(currentPlayerIndex).getChangeTurnFlag() == true)
                {
                    playerControllers.ElementAt(currentPlayerIndex).setChangeTurnFlag(false);
                    playerControllers.ElementAt(currentPlayerIndex).deSelectUnit();

                    // Determine the start and end target positions
                    int targetStartX = currentPlayer.getCursor().getXLocation();
                    int targetStartZ = currentPlayer.getCursor().getYLocation();
                    int targetEndX = nextPlayer.getUnitAt(0).getXPosition();
                    int targetEndZ = nextPlayer.getUnitAt(0).getYPosition();
                    
                    // Determine the next camera angle given the desired x and y position
                    compassDirection nextCameraAngle = camera.getCameraAngleAt(map.getWidth(), map.getHeight(),targetEndX,targetEndZ);

                    // Initialize the camera movement to reach the desired unit's location
                    cameraController.initializeCameraMovement(nextCameraAngle, targetStartX, targetStartZ, targetEndX, targetEndZ, 60);

                    // Reset the current territory's model to its original state
                    map.get(targetStartX, targetStartZ).redesignateModel();

                    // Determine the new current and next player
                    updatePlayerTurns();

                    // Update territory model to reflect cursor
                    currentPlayer.setCursorPosition(new Vector2(targetEndX,targetEndZ));
                    map.get(targetEndX, targetEndZ).designateModel(ModelMode.cursor, currentPlayer.getTeam());

                    // Find new action designated for this new position
                    currentPlayer.getCursor().findAction(currentPlayer);
                }
            }
        }

        // The current player is set equal to the previous next player reference, and a new next player is determined
        public void updatePlayerTurns()
        {
            if (currentPlayerIndex == playerList.Count() - 1)
                currentPlayerIndex = 0;
            else
                currentPlayerIndex++;

            if (currentPlayerIndex == playerList.Count() - 1)
                nextPlayerIndex = 0;
            else
                nextPlayerIndex = currentPlayerIndex + 1;

            currentPlayer = playerList.ElementAt(currentPlayerIndex);
            nextPlayer = playerList.ElementAt(nextPlayerIndex);
        }

        /*******************************************************************/
        /*  DEBUG -- Keyboard listeners
        /*******************************************************************/

        public void listenForKeyboard(KeyboardState newState)
        {
            listenForTurnChange();
            playerControllers.ElementAt(currentPlayerIndex).listenForKeyboard(newState);
        }

    }
}
