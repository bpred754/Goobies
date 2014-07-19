using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Goobies.Game_Objects
{
    public class CameraController
    {
        // Global Variables
        private Camera camera;
        private MapModel mapModel;
        private float[] attributes; // Represents the camera position and camera target
        private bool[] movements = new bool[5]; // Represents if each of the camera's position's or target's attributes is moving or not
        private float[] increments = new float[5]; // Represents the increment values for each of the camera position's and target's attributes
        private float[] maxMovements = new float[5]; // Represents the maximum distance each of the camera position's and target's attributes may go
        private KeyboardState oldState;

        // Rotation
        private float rotateIncrement;
        private bool complete;

        // Constants
        private const float ROTATE_SPEED_DIVISOR = 12; // Increase for slower rotate animation (Desired speed is .25) (camdistancefromunit/desired speed)
        private const float FLOAT_ERROR = .001f;
        private const int CAMERA_X = 0; // Indexes for attributes, increments, movements, and maxMovements arrays
        private const int CAMERA_Y = 1;
        private const int CAMERA_Z = 2;
        private const int TARGET_X = 3;
        private const int TARGET_Z = 4;
        
        public CameraController(Camera camera, MapModel mapModel)
        {
            this.camera = camera;
            this.mapModel = mapModel;
            rotateIncrement = (camera.getCameraDisplacement() / ROTATE_SPEED_DIVISOR);

            // Debug
            this.oldState = Keyboard.GetState();

            Vector3 cameraPosition = camera.getCameraPosition();
            Vector3 cameraTarget = camera.getCameraTarget();

            // Initialize arrays
            attributes = new float[5] {cameraPosition.X, cameraPosition.Y, cameraPosition.Z, cameraTarget.X, cameraTarget.Z};
            for (int i = 0; i < attributes.Count(); i++)
            {
                movements[i] = false;
                increments[i] = 0;
                maxMovements[i] = 0;
            }
        }

        public void listen(GamePadState gamePadState)
        {
            // Only listen for button press when the camera is not moving or rotating
            if (isBusy() == false)
                listenForRotation(gamePadState);
            else // Perform camera adjustment
                adjustCamera();
        }

        // Move camera in desired direction
        public void adjustCamera()
        {
            for (int i = 0; i < attributes.Count(); i++)
            {
                if (movements[i]) // Check if any of the attributes need to be incremented
                {
                    complete = checkComplete(increments[i], attributes[i], maxMovements[i]); // Check if the attribute that is moving has completed its movement
                    if (!complete)
                        attributes[i] += increments[i];
                }
            }

            if (complete)
                resetCameraControllerSettings();

            // Update the camera with the first 3 attributes signifying camera position and the last two signifying the target
            Vector3 newCamPosition = new Vector3(attributes[0],attributes[1],attributes[2]);
            Vector3 newCamTarget = new Vector3(attributes[3],camera.getCameraTarget().Y, attributes[4]);
            camera.update(newCamPosition, newCamTarget);
            mapModel.updateCamera(newCamPosition, newCamTarget);
        }

        // Checks if the camera controller is in the process of moving the camera
        public bool isBusy()
        {
            for (int i = 0; i < movements.Count(); i++)
            {
                if (movements[i])
                    return true;
            }
            return false;
        }

        // Checks if the camera is at its desired location
        public bool checkComplete(float increment, float position, float maxMove)
        {
             if (increment > 0)
            {
                if (position + FLOAT_ERROR >= maxMove)
                    return true;
            }
            else
            {
                if (position - FLOAT_ERROR <= maxMove)
                    return true;
            }
            return false;
        }

        // Resets all the movement flags and the complete flag
        public void resetCameraControllerSettings()
        {
            for (int i = 0; i < movements.Count(); i++)
                movements[i] = false;
            
            complete = false;
        }

        /*******************************************************************/
        /*  Camera movement between units
        /*******************************************************************/

        // Given the next camera angle determine the correct configurations to move the camera
        public void initializeCameraMovement(compassDirection nextCameraAngle, float targetStartX, float targetStartZ, float targetEndX, float targetEndZ, float frequency)
        {
            float cameraDisplacement = camera.getCameraDisplacement();

            float cameraPositionX = 0;
            if (nextCameraAngle == compassDirection.east || nextCameraAngle == compassDirection.south)
                cameraPositionX = targetEndX + cameraDisplacement;
            else
                cameraPositionX = targetEndX - cameraDisplacement;

            float cameraPositionZ = 0;
            if (nextCameraAngle == compassDirection.south || nextCameraAngle == compassDirection.west)
                cameraPositionZ = targetEndZ + cameraDisplacement;
            else
                cameraPositionZ = targetEndZ - cameraDisplacement;

            configureIncrement(attributes[CAMERA_X], cameraPositionX, axis.x, cameraProperty.position, frequency);
            configureIncrement(attributes[CAMERA_Z], cameraPositionZ, axis.z, cameraProperty.position, frequency);
            configureIncrement(targetStartX, targetEndX, axis.x, cameraProperty.target, frequency);
            configureIncrement(targetStartZ, targetEndZ, axis.z, cameraProperty.target, frequency);

            camera.setFacingDirection(nextCameraAngle);
        }

        // Given the start and end positions the corresponding movement increment will be calculated. Then depending on
        // the axis and camera property the corresponding movement flag will be set along with the maxmum distance the
        // camera can move.
        public void configureIncrement(float start, float end, axis axis, cameraProperty property, float frequency)
        {
            float distance = end - start;
            float transitionFrequency = frequency;
            if (property == cameraProperty.position)
            {
                if (axis == axis.x)
                    configureMovement(CAMERA_X,distance,transitionFrequency,end);
                else if (axis == axis.y)
                    configureMovement(CAMERA_Y, distance, transitionFrequency, end);
                else if (axis == axis.z)
                    configureMovement(CAMERA_Z, distance, transitionFrequency, end);
            }
            else
            {
                if (axis == axis.x)
                    configureMovement(TARGET_X, distance, transitionFrequency, end);
                else if (axis == axis.z)
                    configureMovement(TARGET_Z, distance, transitionFrequency, end);
            }
        }

        // Given the attribute, find the transition frequency with the distance traveled. Then set the attribute's movement
        // flag and set the max move.
        public void configureMovement(int attribute, float distance, float transitionFrequency, float end)
        {
            increments[attribute] = distance / transitionFrequency;
            maxMovements[attribute] = end;

            if (increments[attribute] != 0)
                movements[attribute] = true;
        }

        /*******************************************************************/
        /*  Rotation
        /*******************************************************************/

        // If the Left or Right bumbers are pressed during a player's turn it will rotate the camera accordingly
        public void listenForRotation(GamePadState gamePadState)
        {
            if (gamePadState.Triggers.Left > .5)
                initializeRotation(direction.left); // Activate Left rotation
            else if (gamePadState.Triggers.Right > .5)
                initializeRotation(direction.right); // Activate Right Rotation

            // Activate up or down movement
            else if (gamePadState.Buttons.Y == ButtonState.Pressed)
            {
                movements[CAMERA_Y] = true;
                if (camera.isInBirdsEye())
                {
                    maxMovements[CAMERA_Y] = camera.getHeight();
                    increments[CAMERA_Y] = Math.Abs(rotateIncrement) * -1;
                    camera.setBirdsEye(false);
                }
                else
                {
                    maxMovements[CAMERA_Y] = attributes[CAMERA_Y] + camera.getBirdsEyeHeight();
                    increments[CAMERA_Y] = Math.Abs(rotateIncrement);
                    camera.setBirdsEye(true);
                }
            }
        }

        // Given the rotate direction, determine if the camera needs to move in the x or z direction.
        // Then initialize the rotation accordingly.
        public void initializeRotation(direction rotateDirection)
        {
            // Determine positive or negative direction
            rotateIncrement = camera.determineRotateIncrement(rotateDirection, rotateIncrement);

            // Determine x or z direction and initialize the rotation
            if (camera.checkXDirection(rotateDirection))
                configureRotation(CAMERA_X, rotateIncrement, rotateDirection, TARGET_X);
            else
                configureRotation(CAMERA_Z, rotateIncrement, rotateDirection, TARGET_Z);

            // Set new camera angle
            camera.rotate(rotateDirection);
        }   

        // Givent the attribute set its movement flag, set the increment, and set the maximum move distance
        public void configureRotation(int attribute, float rotateIncrement, direction direction, int target)
        {
            movements[attribute] = true;
            increments[attribute] = rotateIncrement;
            maxMovements[attribute] = camera.getMaxMove(direction, attributes[target]);
        }

        /*******************************************************************/
        /*  Setters and Getters
        /*******************************************************************/
        public Camera getCamera()
        {
            return camera;
        }

        /*******************************************************************/
        /*  DEBUG -- Keyboard listeners
        /*******************************************************************/

        public void listenForKeyboard(KeyboardState newState)
        {
            // Only listen for button press when the camera is not moving or rotating
            if (isBusy() == false)
                listenForKeyboardRotation(newState);
            else // Perform camera adjustment
                adjustCamera();

            oldState = newState;
        }

        // If the Left or Right arrow keys are pressed during a player's turn it will rotate the camera accordingly
        public void listenForKeyboardRotation(KeyboardState newState)
        {
            if (newState.IsKeyDown(Keys.Left))
            {
                if (!oldState.IsKeyDown(Keys.Left))
                {
                    initializeRotation(direction.left); // Activate Left rotation
                }
            }
            else if (newState.IsKeyDown(Keys.Right))
            {
                if (!oldState.IsKeyDown(Keys.Right))
                {
                    initializeRotation(direction.right); // Activate Right Rotation
                }
            }

            // Activate up or down movement
            else if (newState.IsKeyDown(Keys.Up))
            {
                if (!oldState.IsKeyDown(Keys.Up))
                {
                    movements[CAMERA_Y] = true;
                    if (camera.isInBirdsEye())
                    {
                        maxMovements[CAMERA_Y] = camera.getHeight();
                        increments[CAMERA_Y] = Math.Abs(rotateIncrement) * -1;
                        camera.setBirdsEye(false);
                    }
                    else
                    {
                        maxMovements[CAMERA_Y] = attributes[CAMERA_Y] + camera.getBirdsEyeHeight();
                        increments[CAMERA_Y] = Math.Abs(rotateIncrement);
                        camera.setBirdsEye(true);
                    }
                }
            }
        }
    }
}
