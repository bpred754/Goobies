using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Goobies
{
    class CrossGooby : Unit
    {
        private readonly int attackCost = 30;
        private readonly int attackDistance = 1;
        private readonly int damage = 100;
        private List<Vector2> attackLocations;

        // "Medic"
        public CrossGooby(Map map, int team, int x, int y) : base(map, team, x, y)
        {
            movementCost = 15;
            attackRange = attackDistance;
            if (map.get(xPosition, yPosition).getElevationStatus() == elevation.mountain)
                attackRange++;
        }

        public override List<Vector2> getAttackLocations()
        {
            attackLocations = new List<Vector2>();

            return attackLocations;
        }

        public override void attack(int x, int y)
        {
            
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
            unitModel = content.Load<Model>("Models/Goobies/practiceSphere");
        }

        /*******************************************************************/
        /*  SETTERS AND GETTERS
        /*******************************************************************/
    }
}
