using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Goobies
{
    public abstract class Unit
    {
        // Global Variables
        protected Map map;
        protected int team;
        protected int health = 100;
        protected int energy = 100;
        protected int xPosition;
        protected int yPosition;
        protected int movementCost;
        private List<Vector2> movementLocations;
        private List<Vector2> cardinalLocations;
        private bool[,] movementReferenceArray;
        private int numMovements = 0;
        protected int attackRange;

        // Model Variables
        protected Model unitModel;
        private Vector3 scale;
        private Matrix world;
        private Matrix view;
        private Matrix projection;
        private Vector3 unitPosition;
        private bool modelIsLoaded;

        public Unit(Map map, int team, int x, int y)
        {
            this.map = map;
            
            this.team = team;
            this.xPosition = x;
            this.yPosition = y;
            unitPosition = new Vector3(xPosition, getUnitHeight(), yPosition);
            map.get(x, y).setGooby(this);

            scale = new Vector3(.6f, .6f, .6f);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.1f, 100f);
        }

        public bool equals(Unit unit)
        {
            if (unit.getXPosition() == xPosition && unit.getYPosition() == yPosition)
                return true;
            else
                return false;
        }

        public  List<Vector2>getMovementLocations(int movementCost)
        {
            movementLocations = new List<Vector2>();

            numMovements = energy / movementCost;
            movementReferenceArray = new bool[map.getHeight(), map.getWidth()];

            getMovementLocationsHelper(xPosition, yPosition, numMovements);

            movementReferenceArray[xPosition, yPosition] = false; // remove the current unit's position from the reference array
            for (int i = 0; i < map.getWidth(); i++)
            {
                for (int j = 0; j < map.getHeight(); j++)
                {
                    if (movementReferenceArray[i, j] == true)// && map.get(i,j).getOccupation() != true) // Dont allow movement onto a territory if a unit is already there
                        movementLocations.Add(new Vector2(i, j));
                }
            }

            return movementLocations;
        }

        public void getMovementLocationsHelper(int x, int y, int range)
        {
            elevation currentElevation = map.get(x, y).getElevationStatus();
            movementReferenceArray[x, y] = true;

            if (range <= 0)
                return;
            else
            {
                if (x > 0)
                {
                    if (isLegal(x-1,y,currentElevation))
                        getMovementLocationsHelper(x - 1, y, range - 1);
                }
                if (y < map.getHeight() - 1)
                {
                    if (isLegal(x,y+1,currentElevation))
                        getMovementLocationsHelper(x, y + 1, range - 1);
                }
                if (x < map.getWidth() - 1)
                {
                    if (isLegal(x+1,y,currentElevation))
                        getMovementLocationsHelper(x + 1, y, range - 1);
                }
                if (y > 0)
                {
                    if (isLegal(x,y-1,currentElevation))
                        getMovementLocationsHelper(x, y - 1, range - 1);
                }
            }
        }

        public List<Vector2> getCardinalLocations()
        {
            cardinalLocations = new List<Vector2>();
            elevation currentElevation = map.get(xPosition,yPosition).getElevationStatus();
            Vector2 nextVector;
            if (xPosition > 0)
            {
                nextVector = new Vector2(xPosition - 1, yPosition);
                if (movementLocations.Contains(nextVector) && isLegal(xPosition-1,yPosition,currentElevation))
                    cardinalLocations.Add(nextVector);
            }

            if (yPosition < map.getHeight() - 1)
            {
                nextVector = new Vector2(xPosition, yPosition + 1);
                if (movementLocations.Contains(nextVector) && isLegal(xPosition,yPosition+1,currentElevation))
                    cardinalLocations.Add(nextVector);
            }

            if (xPosition < map.getWidth() - 1)
            {
                nextVector = new Vector2(xPosition + 1, yPosition);
                if (movementLocations.Contains(nextVector) && isLegal(xPosition+1,yPosition,currentElevation))
                    cardinalLocations.Add(nextVector);
            }

            if (yPosition > 0)
            {
                nextVector = new Vector2(xPosition, yPosition - 1);
                if (movementLocations.Contains(nextVector) && isLegal(xPosition,yPosition-1,currentElevation))
                    cardinalLocations.Add(nextVector);
            }

            return cardinalLocations;
        }

        public bool isLegal(int x, int y, elevation current)
        {
            elevation next = map.get(x, y).getElevationStatus();

            if ((current == elevation.plain && next != elevation.mountain ||
                current == elevation.mountain && next != elevation.plain ||
                current == elevation.hill) &&
                map.get(x,y).getOccupation()== false)
                return true;
            else
                return false;

        }

        public void move(int x, int y, int movementCost)
        {
            if (energy >= movementCost) // This check should be done before entering this method
            {
                getMovementLocations(movementCost);

                if (getCardinalLocations().Contains(new Vector2(x, y)))
                {
                    //set the current position's territory to contain no goobies
                    map.get(xPosition, yPosition).setGooby(null);

                    // Set the units position variables to the new location
                    xPosition = x;
                    yPosition = y;        

                    unitPosition = new Vector3(xPosition, getUnitHeight(), yPosition); // Update unit position 
                   
                    map.get(x, y).setGooby(this);
                    energy += -movementCost;

                    if (map.get(xPosition, yPosition).getElevationStatus() == elevation.mountain)
                        map.get(xPosition, yPosition).getGooby().increaseRange();
                    else
                        resetAttackRange();
                }
            }
        }

        public bool checkMovementLocation(int x, int y)
        {
            getMovementLocations(movementCost);
            if (getCardinalLocations().Contains(new Vector2(x, y)))
                return true;
            else
                return false;
        }

        // Returns the units height depending on the elevation of the terrain
        public float getUnitHeight()
        {
            float unitHeight;
            elevation currentElevation = map.get(xPosition, yPosition).getElevationStatus();
            if (currentElevation == elevation.plain)
                unitHeight = 1.25f;
            else if (currentElevation == elevation.hill)
                unitHeight = 1.75f;
            else
                unitHeight = 2f;

            return unitHeight;
        }

        public void decreaseHealth(int damage)
        {
            health += -damage;
        }

        public void decreaseEnergy(int energyCost)
        {
            energy += -energyCost;
        }

        public void increaseRange()
        {
            attackRange += 1;
        }

        public void resetEnergy()
        {
            energy = 100;
        }

        public abstract List<Vector2> getAttackLocations();

        public abstract void attack(int x, int y);

        public abstract bool checkAttackLocation(int x, int y);

        public abstract void resetAttackRange();

        /*******************************************************************/
        /*  Model
        /*******************************************************************/
     
        public void updateCamera(Vector3 cameraPosition, Vector3 cameraTarget, ContentManager content)
        {
            if (!modelIsLoaded)
            {
                loadModel(content);
                modelIsLoaded = true;
            }
            view = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.UnitY);
            world = Matrix.CreateScale(scale) * Matrix.CreateTranslation(unitPosition);
            map.get(xPosition, yPosition).setGoobyIsLoaded(true);
        }

        public void setMovementLocationsModels(ContentManager content)
        {
            for (int i = 0; i < movementLocations.Count; i++)
            {
                Vector2 mapLocation = movementLocations.ElementAt(i);
                map.get((int)mapLocation.X, (int)mapLocation.Y).setTerritoryModel(content, "PlainBlackCardinal");
            }
        }

        public void setUnitWindowPosition()
        {
            unitPosition = new Vector3(xPosition,getUnitHeight(),yPosition);
        }
        
        public void drawModel()
        {
            foreach (ModelMesh mesh in unitModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                   // effect.EnableDefaultLighting();
                    //effect.LightingEnabled = true;
                    //effect.DirectionalLight0.DiffuseColor = new Vector3(.5f, .5f, .5f);
                    //effect.DirectionalLight0.Direction = new Vector3(-10, -1, 0);

                    //effect.DirectionalLight0.SpecularColor = new Vector3(0, 1, 0);
                    //effect.DiffuseColor= new Vector3(1,0,0);
                    //effect.AmbientLightColor = new Vector3(.9f,.9f,.9f);

                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                }

                mesh.Draw();
            }
        }

        public abstract void loadModel(ContentManager content);

       /*******************************************************************/
       /*  SETTERS AND GETTERS
       /*******************************************************************/
        
        public Map getMap()
        {
            return map;
        }
        
        public void setTeam(int team)
        {
            this.team = team;
        }
            
        public int getTeam()
        {
            return team;
        }

        public int getHealth()
        {
            return health;
        }

        public void setEnergy(int energy)
        {
            this.energy = energy;
        }

        public int getEnergy()
        {
            return energy;
        }

        public void setXPosition(int x)
        {
            xPosition = x;
        }

        public int getXPosition()
        {
            return xPosition;
        }

        public void setYPosition(int y)
        {
            yPosition = y;
        }

        public int getYPosition()
        {
            return yPosition;
        }

        public void setModel(Model model)
        {
            unitModel = model;
        }

        public Model getModel()
        {
            return unitModel;
        }

        public void setMovementCost(int movementCost)
        {
            this.movementCost = movementCost;
        }

        public int getMovementCost()
        {
            return movementCost;
        }

        public int getAttackRange()
        {
            return attackRange;
        }

        /*******************************************************************/
        /*  DEBUGGING
        /*******************************************************************/

        public void changeView(float x, float y, float z)//, float a, float b, float c,float angle)
        {
            view = Matrix.CreateLookAt(new Vector3(x, y, z), new Vector3(2, 1.25f, 2), Vector3.UnitY);
        }
    }
}
