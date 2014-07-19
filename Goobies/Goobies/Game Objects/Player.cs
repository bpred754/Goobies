using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Goobies.Game_Objects;

namespace Goobies
{
    public enum ModelMode {cursor, ownership, movement, cardinal, attack};

    public class Player
    {
        private Map map;
        private int team;
        private Cursor cursor;
        private List<Unit> unitList;
        private int unitIndex = 0;
        private Unit selectedUnit;
        private List<Territory> possibleTerritories;
        private Player enemy;

        // Constructor
        public Player(Map map, int x, int y, int team)
        {
            this.team = team;
            this.map = map;
            unitList = new List<Unit>();
            possibleTerritories = new List<Territory>();
            cursor = new Cursor(map, x, y, team);
        }

        public void attackAtCursorPosition()
        {
            int x = cursor.getXLocation();
            int y = cursor.getYLocation();
            selectedUnit.attack(x, y);

            Unit enemyGooby = map.get(x, y).getGooby();
            if (enemyGooby.getHealth() <= 0) // If the enemy that was attacked has health below 0, remove it from the correspinding player's unitList
                enemy.removeUnit(enemyGooby, cursor.getXLocation(), cursor.getYLocation());
        }

        public void moveSelectedUnitToCursor()
        {
            int x = cursor.getXLocation();
            int y = cursor.getYLocation();
            
            selectedUnit.move(x, y, selectedUnit.getMovementCost());
        }

        // Update all the territory models that correspond to the selected unit's movement list, cardinal list, and attack list
        public void updatePossibleTerritories()
        {
            List<Vector2> movementList = selectedUnit.getMovementLocations(selectedUnit.getMovementCost());
            List<Vector2> cardinalList = selectedUnit.getCardinalLocations();
            List<Vector2> attackList = selectedUnit.getAttackLocations();

            // Mark Movement and Cardinal territories for the selected unit
            for (int i = 0; i < movementList.Count; i++)
            {
                Vector2 movement = movementList.ElementAt(i);
                Territory currentTerritory = map.get((int)movement.X, (int)movement.Y);
                bool foundCardinal = false;
                for (int j = 0; j < cardinalList.Count; j++)
                {
                   // Vector2 cardinal = cardinalList.ElementAt(j);
                    if (cardinalList.Contains(movement))
                    {
                        foundCardinal = true;
                        break;
                    }
                }
                // If the current movement location is a cardinal location, set the territory model to represent cardinality.
                if(foundCardinal)
                    currentTerritory.designateModel(ModelMode.cardinal, team);
                else // If the current movement location is NOT a cardinal location, set the territory as a possible movement location
                    currentTerritory.designateModel(ModelMode.movement, team);
                possibleTerritories.Add(currentTerritory); // Add the current territory into the list that contains territories with possible actions
            }
            
            /*  NEEDS TESTING */
            // Mark the possible attack locations for the selected unit
            for (int i = 0; i < attackList.Count; i++)
            {
                Vector2 attack = attackList.ElementAt(i);
                bool foundAttack = false;
                for (int j = 0; j < possibleTerritories.Count; j++)
                {
                    Territory possibleTerritory = possibleTerritories.ElementAt(j);
                    if (attack.X == possibleTerritory.getMapLocationX() && attack.Y == possibleTerritory.getMapLocationY())
                    {
                        possibleTerritory.redesignateModel(); // if attack territory is a movement or cardinal territory reset its model string
                        possibleTerritory.designateModel(ModelMode.attack, team); // designate model string to attack string
                        foundAttack = true;
                        break;
                    }
                }
                // If the attack location is not a movement or cardinal location set its territory to represent an attack
                if (!foundAttack)
                {
                    Territory currentTerritory = map.get((int)attack.X, (int)attack.Y);
                    currentTerritory.designateModel(ModelMode.attack, team);
                    possibleTerritories.Add(currentTerritory);
                }
            }
        }
        
        // Unselect this player's selected unit if the unit does not have any possible actions
        public void checkUnitActions()
        {
            if (possibleTerritories.Count() == 0)
                selectedUnit = null; // This player no longer has a gooby selected
        }

        // Reset the energy for all the units in this player's unitList
        public void resetEnergy()
        {
            for (int x = 0; x < unitList.Count; x++)
                unitList.ElementAt(x).resetEnergy();
        }

        // Remove the given unit from this player's unitList
        public void removeUnit(Unit gooby, int x, int y)
        {
            unitList.Remove(gooby);
            map.get(x, y).setGooby(null);
        }

        // Removes the unit at the given x and y location from this player's unitList
        public void removeUnitAt(int x, int y)
        {
            foreach(Unit goobie in unitList)
            {
                if (goobie.getXPosition() == x && goobie.getYPosition() == y)
                {
                    unitList.Remove(goobie);
                    map.get(x, y).setGooby(null);
                    break;
                }
            }
        }
        
        // Add the given unit to this player's unitList
        public void addUnit(Unit unit)
        {
            unitList.Add(unit);
        }

        // Set this player's cursor to the given position
        public void setCursorPosition(Vector2 position)
        {
            cursor.setXLocation((int)position.X);
            cursor.setYLocation((int)position.Y);
        }

        // This method returns true if this player currently has a selected unit
        public bool inSelectedState()
        {
            if (selectedUnit != null)
                return true;
            else
                return false;
        }
        
        public void incrementUnitIndex()
        {
            if (unitIndex == unitList.Count() - 1)
                unitIndex = 0;
            else
                unitIndex++;
        }

        public void decrementUnitIndex()
        {
            if (unitIndex == 0)
                unitIndex = unitList.Count() - 1;
            else
                unitIndex--;
        }

        /*******************************************************************/
        /*  SETTERS AND GETTERS
        /*******************************************************************/

        public void setEnemy(Player enemy)
        {
            this.enemy = enemy;
        }

        public void setSelectedUnit(Unit unit)
        {
            selectedUnit = unit;
        }

        public Territory getTerritory(int x, int y)
        {
            return map.get(x, y);
        }
        
        public int getUnitIndex()
        {
            return unitIndex;
        }
        
        public int getTeam()
        {
            return team;
        }

        public List<Unit> getUnitList()
        {
            return unitList;
        }

        public Unit getUnitAt(int index)
        {
            return unitList.ElementAt(index);
        }

        public List<Territory> getPossibleTerritories()
        {
            return possibleTerritories;
        }
        
        public Cursor getCursor()
        {
            return cursor;
        }

        public Unit getSelectedUnit()
        {
            return selectedUnit;
        }
    }
}
