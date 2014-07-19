using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Goobies
{
    class CircleGooby : Unit
    {
        private readonly int attackCost = 50; // Can shoot a maximum of 2 times
        
        private readonly int damage = 50; // Takes 2 shots to kill enemy
        private readonly int attackDistance = 4;
        private List<Vector2> attackLocations;
        private bool[,] attackReferenceArray;

        // "Infantry"
        public CircleGooby(Map map, int team, int x, int y) : base(map, team, x, y)
        {
            movementCost = 25;
            attackRange = attackDistance;
            if (map.get(xPosition, yPosition).getElevationStatus() == elevation.mountain)
                attackRange++;
        }

        public override List<Vector2> getAttackLocations()
        {
            attackLocations = new List<Vector2>();
            attackReferenceArray = new bool[map.getHeight(), map.getWidth()];

            if (getEnergy() >= attackCost)
            {
                attackLocationsHelper(xPosition, yPosition, attackRange);

                // Update attackReferenceArray to remove all objstructed territories from units or mountains
                updateAttackReferenceArray();

                // Traverse through attackReferenceArray and add vectors of possible attack locations
                for (int i = 0; i < map.getWidth(); i++)
                {
                    for (int j = 0; j < map.getHeight(); j++)
                    {
                        if (attackReferenceArray[j, i] == true)
                            attackLocations.Add(new Vector2(j, i));
                    }
                }
            }

            return attackLocations;
        }

        public void attackLocationsHelper(int x, int y, int range)
        {
            elevation currentElevation = map.get(x, y).getElevationStatus();

            if (map.get(x, y).isEnemyOccupied(team))
                attackReferenceArray[x, y] = true;

            // If the current territory is a mountain, return (except if the starting location is a mountain). 
            // A circle cannot shoot past a mountain, however they can still hit whoever is on top
            if (range == 0)
                return;
            else
            {
                if (x > 0)
                    attackLocationsHelper(x - 1, y, range - 1);
                if (y < map.getHeight() - 1)
                    attackLocationsHelper(x, y+1, range - 1);
                if (x < map.getWidth() - 1)
                    attackLocationsHelper(x+1, y, range - 1);
                if (y > 0)
                    attackLocationsHelper(x, y-1, range - 1);
            }
        }

        public void updateAttackReferenceArray()
        {
            elevation currentElevation = map.get(xPosition, yPosition).getElevationStatus();

            for (int i = 0; i < map.getWidth(); i++)
            {
                for (int j = 0; j < map.getHeight(); j++)
                {
                    if (attackReferenceArray[i, j] == true || (currentElevation != elevation.mountain && map.get(i,j).getElevationStatus() == elevation.mountain))
                    {
                        bool isMountain = false;
                        if (map.get(i, j).getElevationStatus() == elevation.mountain)
                            isMountain = true;

                        if (i < xPosition) // check left
                        {
                            if (j > yPosition) // check diagonal left and up
                            {
                                for (int a = 0; a <= i; a++)
                                {
                                    for (int b = j; b < map.getHeight(); b++)
                                        attackReferenceArray[a, b] = false;
                                }
                                if(!isMountain)
                                    attackReferenceArray[i, j] = true; // reset to true if only a unit was in the territory since array above set to false
                            }
                            else if (j < yPosition) // check diagonal left and down
                            {
                                for (int a = 0; a <= i; a++)
                                {
                                    for (int b = 0; b <= j; b++)
                                        attackReferenceArray[a, b] = false;
                                }
                                if (!isMountain)
                                    attackReferenceArray[i, j] = true; // reset to true if only a unit was in the territory since array above set to false
                            }
                            else // j = yLocation (Direct Line Logic to the Left)
                            {
                                for (int a = 0; a < i; a++)
                                    attackReferenceArray[a, j] = false;
                            }
                        }
                        else if (i > xPosition)// check right
                        {
                            if (j > yPosition) // check diagonal up and right
                            {
                                for (int a = i; a < map.getWidth(); a++)
                                {
                                    for (int b = j; b < map.getHeight(); b++)
                                        attackReferenceArray[a, b] = false;
                                }
                                if (!isMountain)
                                    attackReferenceArray[i, j] = true; // reset to true if only a unit was in the territory since array above set to false
                            }

                            else if (j < yPosition) // check diagonal down and right
                            {
                                for (int a = i; a < map.getWidth(); a++)
                                {
                                    for (int b = 0; b <= j; b++)
                                        attackReferenceArray[a, b] = false;
                                }
                                if (!isMountain)
                                    attackReferenceArray[i, j] = true; // reset to true if only a unit was in the territory since array above set to false
                            }
                            else // j = yLocation (Direct Line Logic to the Right)
                            {
                                for (int a = i + 1; a < map.getWidth(); a++)
                                    attackReferenceArray[a, j] = false;
                            }
                        }
                        else // i = xLocation (Direct Line Logic Up and and down)
                        {
                            if (j > yPosition) // check Up
                            {
                                for (int b = j + 1; b < map.getHeight(); b++)
                                    attackReferenceArray[i, b] = false;
                            }
                            else // Check Down
                            {
                                for (int b = 0; b < j; b++)
                                    attackReferenceArray[i, b] = false;
                            }
                        }
                    }
                }
            }
        }

        public override void attack(int x, int y)
        {
            if (getAttackLocations().Contains(new Vector2(x, y)))
            {
                map.get(x, y).getGooby().decreaseHealth(damage);
                decreaseEnergy(attackCost);
            }
        }
        public override bool checkAttackLocation(int x, int y)
        {
            if (getAttackLocations().Contains(new Vector2(x, y)) && getEnergy() >= attackCost)
            {
                return true;
            }
            else
                return false;
        }
        public override void resetAttackRange()
        {
            attackRange = attackDistance;
        }

        /*******************************************************************/
        /*  Model
        /*******************************************************************/

        public override void loadModel(ContentManager content)
        {
            if(team == 0)
                unitModel = content.Load<Model>("Models/Goobies/Red/RedCircle");
            else
                unitModel = content.Load<Model>("Models/Goobies/Blue/BlueCircle");
        }

        /*******************************************************************/
        /*  SETTERS AND GETTERS
        /*******************************************************************/

        public int getAttackCost()
        {
            return attackCost;
        }
    }
}
