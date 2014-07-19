using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Goobies.Game_Objects
{
    public enum axis {x,y,z};
    public enum cameraProperty {position,target};

    public class Camera
    {
        // Global Variables
        private Vector3 cameraPosition;
        private Vector3 cameraTarget;
        private bool inBirdsEye;
        private Direction cameraDirection;
        private float cameraDisplacement; // Camera distance from unit
        private float cameraHeight;
        private float targetY; // The y value of where the camera is looking

        // Constants
        private readonly int BIRDS_EYE_HEIGHT = 10;

        public Camera(float targetX, float targetZ, float cameraDisplacement, float cameraHeight, float targetY)
        {
            this.cameraDisplacement = cameraDisplacement;
            this.cameraHeight = cameraHeight;
            this.targetY = targetY;
            cameraPosition = new Vector3(targetX - cameraDisplacement,cameraHeight,targetZ - cameraDisplacement);
            cameraTarget = new Vector3(targetX,targetY,targetZ);
            cameraDirection = new Direction(compassDirection.north);
        }
        public Camera(Vector3 cameraPosition, Vector3 cameraTarget, float cameraDisplacement,compassDirection facingDirection)
        {
            this.cameraDisplacement = cameraDisplacement;
            this.cameraPosition = cameraPosition;
            this.cameraTarget = cameraTarget;

            this.cameraDirection = new Direction(facingDirection);
        }

        // Update the camera's position and target
        public void update(Vector3 cameraPosition, Vector3 cameraTarget)
        {
            this.cameraPosition = cameraPosition;
            this.cameraTarget = cameraTarget;
        }

        // Given an x and z value determine what the camera angle will be at this location
        public compassDirection getCameraAngleAt(int width, int height, int x, int z)
        {
            int xMidpoint = width / 2;
            int zMidpoint = height / 2;
            compassDirection nextCameraAngle = compassDirection.north;

            // Determine the next cursor's camera angle
            if (width % 2 == 0 && height % 2 == 0) // Even map width and height
            {
                if (x < xMidpoint && z < zMidpoint)
                    nextCameraAngle = compassDirection.north;
                else if (x >= xMidpoint && z < zMidpoint)
                    nextCameraAngle = compassDirection.east;
                else if (x >= xMidpoint && z >= zMidpoint)
                    nextCameraAngle = compassDirection.south;
                else if (x < xMidpoint && z >= zMidpoint)
                    nextCameraAngle = compassDirection.west;
            }
                /*
            // NEEDS TESTING
            else if (map.getWidth() % 2 != 0 && map.getHeight() % 2 != 0) // map with odd number of rows and columns
            {
                if (x <= xMidpoint && z < zMidpoint)
                    nextCameraAngle = 0;
                else if (x > xMidpoint && z <= zMidpoint)
                    nextCameraAngle = 1;
                else if (x >= xMidpoint && z > zMidpoint)
                    nextCameraAngle = 2;
                else if (x < xMidpoint && z >= zMidpoint)
                    nextCameraAngle = 3;

                if (x == xMidpoint && z == zMidpoint)
                    nextCameraAngle = 0;
            }
            */
            return nextCameraAngle;
        }

        /*******************************************************************/
        /*  Rotation
        /*******************************************************************/

        // Given the rotate direction determine whether the rotate increment is positive or negative
        public float determineRotateIncrement(direction rotateDirection, float rotateIncrement)
        {
            compassDirection cameraAngle = cameraDirection.getFacingDirection();
            float tempIncrement = 0;

            if (rotateDirection == direction.left)
            {
                if (cameraAngle == compassDirection.north || cameraAngle == compassDirection.east)
                    tempIncrement = Math.Abs(rotateIncrement); // moving in positive direction
                else
                    tempIncrement =  Math.Abs(rotateIncrement) * -1; // moving in negative direction
            }
            else if(rotateDirection == direction.right)
            {
                if (cameraAngle == compassDirection.north || cameraAngle == compassDirection.west)
                    tempIncrement =  Math.Abs(rotateIncrement); // moving in positive direction
                else
                    tempIncrement =  Math.Abs(rotateIncrement) * -1; // moving in negative direction
            }

            return tempIncrement;
        }

        // Given the desired rotate direction, this method will return a boolean 
        // signifying if the camera needs to move in the x direction
        public bool checkXDirection(direction rotateDirection)
        {
            compassDirection cameraAngle = cameraDirection.getFacingDirection();
            bool moveX = false;
            if (rotateDirection == direction.left)
            {
                if (cameraAngle == compassDirection.north || cameraAngle == compassDirection.south)
                    moveX = true;
            }
            else if (rotateDirection == direction.right)
            {
                if (cameraAngle == compassDirection.east || cameraAngle == compassDirection.west)
                    moveX = true;
            }
            return moveX;
        }

        // Given the rotate direction and the desired target location, 
        // this method will return the maximum distance the camera can move
        public float getMaxMove(direction rotateDirection, float target)
        {
            compassDirection cameraAngle = cameraDirection.getFacingDirection();
            float maxMove = 0;
            if (rotateDirection == direction.left)
            {
                if (cameraAngle == compassDirection.north || cameraAngle == compassDirection.east)
                    maxMove = target + cameraDisplacement;
                else if (cameraAngle == compassDirection.south || cameraAngle == compassDirection.west)
                    maxMove = target - cameraDisplacement;
            }
            else if (rotateDirection == direction.right)
            {
                if (cameraAngle == compassDirection.north || cameraAngle == compassDirection.west)
                    maxMove = target + cameraDisplacement;
                else if (cameraAngle == compassDirection.east || cameraAngle == compassDirection.south)
                    maxMove = target - cameraDisplacement;
            }

            return maxMove;
        }

        // Rotates the camera in the given direction
        public void rotate(direction direction)
        {
            if (direction == direction.left)
                cameraDirection.rotateLeft();
            else
                cameraDirection.rotateRight();

        }
        
        /*******************************************************************/
        /*  Setters and Getters
        /*******************************************************************/

        public float getHeight()
        {
            return cameraHeight;
        }

        public float getCameraDisplacement()
        {
            return cameraDisplacement;
        }

        public Vector3 getCameraPosition()
        {
            return cameraPosition;
        }

        public void setCameraPosition(Vector3 cameraPosition)
        {
            this.cameraPosition = cameraPosition;
        }

        public Vector3 getCameraTarget()
        {
            return cameraTarget;
        }

        public float getBirdsEyeHeight()
        {
            return BIRDS_EYE_HEIGHT;
        }

        public compassDirection getFacingDirection()
        {
            return cameraDirection.getFacingDirection();
        }

        public bool isInBirdsEye()
        {
            return inBirdsEye;
        }

        public void setBirdsEye(bool inBirdsEye)
        {
            this.inBirdsEye = inBirdsEye;
        }

        public void setFacingDirection(compassDirection direction)
        {
            cameraDirection.setFacingIndex(direction);
        }

        public Direction getDirection()
        {
            return cameraDirection;
        }
    }
}
