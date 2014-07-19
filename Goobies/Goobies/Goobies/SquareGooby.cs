using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Goobies
{
    class SquareGooby : Unit
    {
        private readonly int attackCost = 30;
        private readonly int attackDistance = 1;
        private readonly int damage = 100;
        private List<Vector2> attackLocations;

        //"Shotgun"
        public SquareGooby(Map map,int team, int x, int y): base(map, team, x, y)
        {
            movementCost = 15;
            attackRange = attackDistance;
            if (map.get(xPosition, yPosition).getElevationStatus() == elevation.mountain)
                attackRange++;
        }

        // Square attacks territories that lie in cardinal directions with a range of 1
        public override List<Vector2> getAttackLocations()
        {
            attackLocations = new List<Vector2>();

            if (xPosition != 0) // add left coordinate to list
            {
                if (map.get(xPosition - attackRange, yPosition).isEnemyOccupied(team))
                    attackLocations.Add(new Vector2(xPosition - attackRange, yPosition));
            }
            if (yPosition != 0) // add up coordinate to list
            {
                if (map.get(xPosition, yPosition - attackRange).isEnemyOccupied(team))
                    attackLocations.Add(new Vector2(xPosition, yPosition - attackRange));
            }
            if (xPosition < map.getWidth() - 1) // add right coordinate to list
            {
                if (map.get(xPosition + attackRange, yPosition).isEnemyOccupied(team))
                    attackLocations.Add(new Vector2(xPosition + attackRange, yPosition));
            }
            if (yPosition < map.getHeight() - 1) // add bottom coordinate to list
            {
                if (map.get(xPosition, yPosition + attackRange).isEnemyOccupied(team))
                    attackLocations.Add(new Vector2(xPosition, yPosition + attackRange));
            }
            return attackLocations;
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
                unitModel = content.Load<Model>("Models/Goobies/Red/RedSquare");
            else
                unitModel = content.Load<Model>("Models/Goobies/Blue/BlueSquare");
        }

        /*******************************************************************/
        /*  SETTERS AND GETTERS
        /*******************************************************************/
    }
}
