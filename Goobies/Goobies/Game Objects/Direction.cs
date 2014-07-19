using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Goobies.Game_Objects
{
    public enum compassDirection {west = 0, northWest = 1, north = 2, northEast = 3, east = 4, southEast = 5, south = 6, southWest =7};

    public class Direction
    {
        private int facingIndex; 
        private compassDirection[] compassDirections = new compassDirection[8]{compassDirection.west,
                                                                               compassDirection.northWest,
                                                                               compassDirection.north,
                                                                               compassDirection.northEast,
                                                                               compassDirection.east,
                                                                               compassDirection.southEast,
                                                                               compassDirection.south,
                                                                               compassDirection.southWest};
        public Direction(compassDirection compassDirection)
        {
            setFacingIndex(compassDirection);
        }

        // Rotate camera to the left by getting the direction 2 indexes ahead of the current index in compassDirections array                                                                          
        public void rotateLeft()
        {
            facingIndex = getIndex(facingIndex, 2);
        }

        // Rotate camera to the left by getting the direction 2 indexes behind the current index in compassDirections array    
        public void rotateRight()
        {
            facingIndex = getIndex(facingIndex, -2);
        }

        // Return the compass direction for the given direction
        public compassDirection getCompassDirection(direction direction)
        {
            int index = -1;
            if (direction == direction.left)
                index = getIndex(facingIndex, -2);
            else if (direction == direction.upLeft)
                index = getIndex(facingIndex, -1);
            else if (direction == direction.up)
                index = facingIndex;
            else if (direction == direction.upRight)
                index = getIndex(facingIndex, 1);
            else if (direction == direction.right)
                index = getIndex(facingIndex, 2);
            else if (direction == direction.downRight)
                index = getIndex(facingIndex, 3);
            else if (direction == direction.down)
                index = getIndex(facingIndex, 4);
            else if (direction == direction.downLeft)
                index = getIndex(facingIndex, -3);

            return compassDirections[index];
        }

        // Create a circular array that returns the facing index given a certain rotation
        public int getIndex(int index, int rotate)
        {
            int numCompassDirections = compassDirections.Count();
            index += rotate;

            if (index > numCompassDirections-1)
                index += -numCompassDirections;
            else if(index < 0)
                index += numCompassDirections;

            return index;
        }

        // Given the facing direction convert it to an index of type int
        public void setFacingIndex(compassDirection direction)
        {
            if (direction == compassDirection.north)
                facingIndex = 2;
            else if (direction == compassDirection.east)
                facingIndex = 4;
            else if (direction == compassDirection.south)
                facingIndex = 6;
            else if (direction == compassDirection.west)
                facingIndex = 0;
        }

        public compassDirection getFacingDirection()
        {
            return compassDirections[facingIndex];
        }

        public compassDirection opposite(compassDirection direction)
        {
            int index = getIndex((int)direction, -4);
            return compassDirections[index];
        }
    }
}
