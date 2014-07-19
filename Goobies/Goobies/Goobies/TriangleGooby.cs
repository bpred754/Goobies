using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Goobies
{
    class TriangleGooby : Unit
    {
        private readonly int attackCost = 100;
        private readonly int attackDistance = 8;
        private readonly int damage = 100;
        private List<Vector2> attackLocations;

        // "Sniper"
        public TriangleGooby(Map map, int team, int x, int y) : base(map, team, x, y)
        {
            movementCost = 50;
            attackRange = attackDistance;
            if (map.get(xPosition, yPosition).getElevationStatus() == elevation.mountain)
                attackRange++;
        }

        public override List<Vector2> getAttackLocations()
        {
            attackLocations = new List<Vector2>();
            elevation currentElevation = map.get(xPosition, yPosition).getElevationStatus();

            // Values to check if line of sight is clear
            bool clearLeft = true;
            bool clearLeftUp = true;
            bool clearUp = true;
            bool clearUpRight = true;
            bool clearRight = true;
            bool clearRightDown = true;
            bool clearDown = true;
            bool clearDownLeft = true;

            for (int i = 1; i <= attackRange; i++)
            {
                if (xPosition - i >= 0) // scan left
                {
                    clearLeft = checkAndMarkTerritory(xPosition - i, yPosition, currentElevation, clearLeft);

                    if (yPosition + i <= map.getHeight() - 1) // scan upwards
                        clearLeftUp = checkAndMarkTerritory(xPosition - i, yPosition + i, currentElevation, clearLeftUp);
                }

                if (yPosition + i <= map.getHeight() - 1) // scan upwards
                {
                    clearUp = checkAndMarkTerritory(xPosition, yPosition + i, currentElevation, clearUp);

                    if (xPosition + i <= map.getWidth() - 1) // scan right
                        clearUpRight = checkAndMarkTerritory(xPosition + i, yPosition + i, currentElevation, clearUpRight);
                }

                if (yPosition + i <= map.getWidth() - 1) // scan right
                {
                    clearRight = checkAndMarkTerritory(xPosition + i, yPosition, currentElevation, clearRight);

                    if (yPosition - i >= 0) // scan downwards
                        clearRightDown = checkAndMarkTerritory(xPosition + i, yPosition - i, currentElevation, clearRightDown);
                }

                if (yPosition - i >= 0) // scan downwards
                {
                    clearDown = checkAndMarkTerritory(xPosition, yPosition - i, currentElevation, clearDown);

                    if (xPosition - i >= 0) // scan left
                        clearDownLeft = checkAndMarkTerritory(xPosition - i, yPosition - i, currentElevation, clearDownLeft);
                }
            }

            return attackLocations;
        }

        public bool checkAndMarkTerritory(int x, int y, elevation currentElevation, bool clear)
        {
            bool clearCheck = true;

            // Cannot shoot through units
            if (map.get(x, y).isEnemyOccupied(team) && clear == true) // Add targets to the left
            {
                attackLocations.Add(new Vector2(x, y));
                clearCheck = false;
            }
            // Cannot shoot past mountains unless triangle is on top of one as well
            else if (currentElevation != elevation.mountain && map.get(x, y).getElevationStatus() == elevation.mountain)
                clearCheck = false;

            if (clearCheck == true)
                return clear;
            else
                return false;
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
                unitModel = content.Load<Model>("Models/Goobies/Red/RedTriangle");
            else
                unitModel = content.Load<Model>("Models/Goobies/Blue/BlueTriangle");
        }

        /*******************************************************************/
        /*  SETTERS AND GETTERS
        /*******************************************************************/
    }
}
