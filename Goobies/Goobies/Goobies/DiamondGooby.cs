using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Goobies
{
    class DiamondGooby : Unit
    {
        private readonly int attackCost = 100;
        private readonly int attackDistance = 5;
        private readonly int damage = 50;
        private List<Vector2> attackLocations;
        private bool[,] attackReferenceArray;

        // "Mortar"
        public DiamondGooby(Map map, int team, int x, int y): base(map, team, x, y)
        {
            movementCost = 100;
            attackRange = attackDistance;
            if (map.get(xPosition, yPosition).getElevationStatus() == elevation.mountain)
                attackRange++;
        }

        public override List<Vector2> getAttackLocations()
        {
            attackLocations = new List<Vector2>();

            attackReferenceArray = new bool[map.getHeight(), map.getWidth()];
            attackLocationsHelper(xPosition, yPosition, attackRange);

            // Traverse through attackReferenceArray and add vectors of possible attack locations
            for (int i = 0; i < map.getWidth(); i++)
            {
                for (int j = 0; j < map.getHeight(); j++)
                {
                    if (attackReferenceArray[j, i] == true)
                        attackLocations.Add(new Vector2(j, i));
                }
            }

            return attackLocations;
        }

        public void attackLocationsHelper(int x, int y, int range)
        {
            if (map.get(x, y).isEnemyOccupied(team))
                attackReferenceArray[x, y] = true;

            if (range == 0) // Base Case
                return;
            else
            {
                if (x > 0)
                    attackLocationsHelper(x - 1, y, range - 1);
                if (y < map.getHeight() - 1)
                    attackLocationsHelper(x, y + 1, range - 1);
                if (x < map.getWidth() - 1)
                    attackLocationsHelper(x + 1, y, range - 1);
                if (y > 0)
                    attackLocationsHelper(x, y - 1, range - 1);
            }
        }

        public override void attack(int x, int y)
        {
            if (getAttackLocations().Contains(new Vector2(x, y)))
            {
                map.get(x, y).getGooby().decreaseHealth(damage);
                decreaseEnergy(attackCost);

                if (x > 0)//check left
                {
                    if(map.get(x-1,y).isEnemyOccupied(team))
                        map.get(x-1, y).getGooby().decreaseHealth(damage);
                }
                if(y < map.getHeight()-1)// check up
                {
                    if(map.get(x,y+1).isEnemyOccupied(team))
                        map.get(x, y+1).getGooby().decreaseHealth(damage);
                }
                if (x < map.getWidth() - 1) // check right
                {
                    if (map.get(x + 1, y).isEnemyOccupied(team))
                        map.get(x + 1, y).getGooby().decreaseHealth(damage);
                }
                if (y > 0) // check down
                {
                    if (map.get(x, y - 1).isEnemyOccupied(team))
                        map.get(x, y - 1).getGooby().decreaseHealth(damage);
                }
            } 
        }

        public override bool checkAttackLocation(int x, int y)
        {
            if (getAttackLocations().Contains(new Vector2(x, y)))
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
            if (team == 0)
                unitModel = content.Load<Model>("Models/Goobies/Red/RedDiamond");
            else
                unitModel = content.Load<Model>("Models/Goobies/Blue/BlueDiamond");
        }

        /*******************************************************************/
        /*  SETTERS AND GETTERS
        /*******************************************************************/
    }
}
