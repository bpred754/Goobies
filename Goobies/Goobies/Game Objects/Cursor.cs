using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Goobies.Game_Objects;

namespace Goobies.Game_Objects
{
    public enum action { attack, move, select, deselect, none };
    public enum direction { left, upLeft, up, upRight, right, downRight, down, downLeft, none };

    public class Cursor
    {
        // Global Variables
        private int team;
        private Map map;
        private int xLocation;
        private int yLocation;
        private action action;

        public Cursor(Map map, int x, int y, int team)
        {
            this.map = map;
            this.xLocation = x;
            this.yLocation = y;
            this.team = team;
        }

        // Find the possible action the player can do at this cursor's position
        public void findAction(Player player)
        {
            bool playerSelectedState = player.inSelectedState();
            Unit tempUnit = map.get(xLocation,yLocation).getGooby();

            if (playerSelectedState) //If the player has previously made a unit selection.
            {
                if (tempUnit != null) //There is a Gooby in the Cursor's Territory
                {
                    if (team == tempUnit.getTeam()) //Unit in Territory is the Players Unit
                    {
                        if (tempUnit.equals(player.getSelectedUnit()) == true) //Unit is the Player's Selected Unit
                            action = action.deselect;
                        else //Unit is NOT the Player's Selected Unit
                            action = action.select;
                    }
                    else //Unit in Territory is NOT the Players Unit
                    {
                        if (player.getSelectedUnit().checkAttackLocation(xLocation, yLocation)) //If enemy Unit is within attack range
                            action = action.attack;
                    }
                }
                else //There is NOT a Gooby in the Cursor's Territory
                {
                    if (player.getSelectedUnit().checkMovementLocation(xLocation, yLocation)) //Valid Move Location
                        action = action.move;
                    else //Invalid Move Location
                        action = action.none;
                }
            }
            else //If the player has NOT previously made a unit selection.
            {
                if (tempUnit != null) //There is a Gooby in the Cursor's Territory
                {
                    if (team == tempUnit.getTeam()) //Unit in Territory is the Players Unit
                        action = action.select;
                    else //Unit in Territory is NOT the Players Unit
                        action = action.none;
                }
                else //There is NOT a Gooby in the Cursor's Territory
                    action = action.none;
            }
        }

        public void move(compassDirection direction)
        {
            if (direction == compassDirection.west)
                moveWest();
            else if (direction == compassDirection.northWest)
                moveNorthWest();
            else if (direction == compassDirection.north)
                moveNorth();
            else if (direction == compassDirection.northEast)
                moveNorthEast();
            else if (direction == compassDirection.east)
                moveEast();
            else if (direction == compassDirection.southEast)
                moveSouthEast();
            else if (direction == compassDirection.south)
                moveSouth();
            else if (direction == compassDirection.southWest)
                moveSouthWest();
        }

        public void moveWest()
        {
            if (xLocation < map.getWidth() - 1 && yLocation > 0)
            {
                xLocation += 1;
                yLocation += -1;
            }
        }

        public void moveNorthWest()
        {
            if (xLocation < map.getWidth() - 1)
                xLocation += 1;
        }

        public void moveNorth()
        {
            if (xLocation < map.getWidth() - 1 && yLocation < map.getHeight() - 1)
            {
                xLocation += 1;
                yLocation += 1;
            }
        }

        public void moveNorthEast()
        {
            if (yLocation < map.getHeight() - 1)
                yLocation += 1;
        }

        public void moveEast()
        {
            if (xLocation > 0 && yLocation < map.getHeight() - 1)
            {
                xLocation += -1;
                yLocation += 1;
            }
        }

        public void moveSouthEast()
        {
            if (xLocation > 0)
                xLocation += -1;
        }

        public void moveSouth()
        {
            if (xLocation > 0 && yLocation > 0)
            {
                xLocation += -1;
                yLocation += -1;
            }
        }

        public void moveSouthWest()
        {
            if (yLocation > 0)
                yLocation += -1;
        }
        
        // Change the territory model depending on the owner and the elevation
        public void updateTerritory()
        {
            map.get(xLocation,yLocation).designateModel(ModelMode.cursor, team);
        }
        
        public void redesignateCurrentTerritoryModel()
        {
            map.get(xLocation, yLocation).redesignateModel();
        }

        /*******************************************************************/
        /*  Setters and Getters
        /*******************************************************************/
        
        public void setXLocation(int x)
        {
            this.xLocation = x;
        }

        public int getXLocation()
        {
            return xLocation;
        }

        public void setYLocation(int y)
        {
            this.yLocation = y;
        }

        public int getYLocation()
        {
            return yLocation;
        }

        public int getTeam()
        {
            return team;
        }

        public action getAction()
        {
            return action;
        }

        public Map getMap()
        {
            return map;
        }
    }
}
