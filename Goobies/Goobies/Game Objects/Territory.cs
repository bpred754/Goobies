using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Goobies
{
    // Elevation Types
    public enum elevation { plain, hill, mountain };

    public class Territory
    {
        // Global Variables
        private elevation elevationType; // Elevation condition
        private bool sourceBlock; // resource condition
        private int team;
        private Unit gooby;
        private int mapLocationX; // Location in the map
        private int mapLocationY;
        protected bool goobyIsLoaded;
        protected Stack<String> modelStrings;

        // Territory Model Variable Declaration
        private ContentManager content;
        private Model territoryModel;
        private Vector3 scale;
        private Vector3 position;
        private Matrix world;
        private Matrix view;
        private Matrix projection;
        private Vector3 cameraTarget;
        private String modelType;

        // Constructor 
        public Territory(int x, int y)
        {
            elevationType = elevation.plain;
            mapLocationX = x;
            mapLocationY = y;
            team = -1; // Not owned
            modelStrings = new Stack<string>();
        }

        public bool isEnemyOccupied(int team)
        {
            if (gooby != null && this.team != team)
                return true;
            else
                return false;
        }

        // Takes territory back to its original state
        public void reset()
        {
            team = -1;
            gooby = null;
        }

        /*******************************************************************/
        /*  Model
        /*******************************************************************/

        // Called From MapModel
        public void initializeTerritoryModel(ContentManager content)
        {
            this.content = content;
            position = new Vector3(mapLocationX, 0, mapLocationY);
            
            // Pass these in when calling this method in MapModel. Allows for different sizes?
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.1f, 100f);
            scale = new Vector3(1, 1, 1);
            world = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);
  
            modelType = "";

            if (elevationType == elevation.plain)
                modelType += "Models/Terrain/Plain";
            else if (elevationType == elevation.hill)
                modelType = "Models/Terrain/Hill";
            else if (elevationType == elevation.mountain)
                modelType = "Models/Terrain/Mountain";

            String initString = modelType + "/Black/Standard";
            modelStrings.Push(initString); // Push the original model type onto stack
           
            territoryModel = content.Load<Model>(initString);
        }
         
        public void designateModel(ModelMode mode, int team)
        {
            String modelString = "";

            if (elevationType == elevation.plain)
                modelString += "Models/Terrain/Plain";
            else if (elevationType == elevation.hill)
                modelString += "Models/Terrain/Hill";
            else if (elevationType == elevation.mountain)
                modelString += "Models/Terrain/Mountain";

            if (this.team == -1)
                modelString += "/Black";
            else if (this.team == 0)
                modelString += "/Red";
            else if (this.team == 1)
                modelString += "/Blue";

            if (mode == ModelMode.cursor)
            {
                if (team == 0)
                    modelString += "/CursorRed";
                else if (team == 1)
                    modelString += "/CursorBlue";
            }
            else if (mode == ModelMode.ownership)
                modelString += "/Standard";

            else if (mode == ModelMode.movement)
                modelString += "/Highlighted";
            else if (mode == ModelMode.cardinal)
                modelString += "/Cardinal";
            else if (mode == ModelMode.attack)
                modelString += "/Attack";
             
            modelStrings.Push(modelString);
            territoryModel = content.Load<Model>(modelString);
        }
        

        public void redesignateModel()
        {
            modelStrings.Pop();
            territoryModel = content.Load<Model>(modelStrings.Peek());
        }

        public void resetStack()
        {
            // Save the original model string
            while (modelStrings.Count > 1)
            {
                Debug.WriteLine("REMOVED  AT  X:  " + mapLocationX + "  Y:  " + mapLocationY);
                modelStrings.Pop();
            }
        }

        public void updateView(Vector3 cameraPosition, Vector3 target)
        {
            view = Matrix.CreateLookAt(cameraPosition, target, Vector3.UnitY);
        }

        public void drawModel()
        {
            foreach (ModelMesh mesh in territoryModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                }

                mesh.Draw();
            }
           
            if (gooby != null && goobyIsLoaded)
                gooby.drawModel();
        }

        // Takes the territory model back to its original state
        public void resetTerritoryModel()
        {
            goobyIsLoaded = false;
            resetStack();
            territoryModel = content.Load<Model>(modelStrings.Peek());
        }
        
        /*******************************************************************/
        /*  SETTERS AND GETTERS
        /*******************************************************************/

        public void setMapLocationX(int x)
        {
            mapLocationX = x;
        }

        public int getMapLocationX()
        {
            return mapLocationX;
        }

        public int getMapLocationY()
        {
            return mapLocationY;
        }

        public void setMapLocationY(int y)
        {
            mapLocationY = y;
        }

        public void setSourceBlock(bool s)
        {
            sourceBlock = s;
        }
        
        public bool getSourceBlock()
        {
            return sourceBlock;
        }
        
        public void setElevation(elevation e)
        {
            elevationType = e;
        }

        public elevation getElevationStatus()
        {
            return elevationType;
        }

        public void setTeam(int team)
        {
            this.team = team;
        }

        public int getTeam()
        {
            return team;
        }
        
        public bool getOccupation()
        {
            if (gooby != null)
                return true;
            else
                return false;
        }
        
        public void setGooby(Unit gooby)
        {
            this.gooby = gooby;

            if(gooby != null)
                this.team = gooby.getTeam();
        }

        public Unit getGooby()
        {
            return gooby;
        }

        public void setCameraTarget(Vector3 target)
        {
            cameraTarget = target;
        }

        public void setModelType(String modelType)
        {
            this.modelType = modelType;
        }
        
        public void setGoobyIsLoaded(bool loaded)
        {
            goobyIsLoaded = loaded;
        }
        
        public void setTerritoryModel(ContentManager content, String territoryType)
        {
            territoryModel = content.Load<Model>(territoryType);
        }
       
        /*******************************************************************/
        /*  DEBUGGING
        /*******************************************************************/

        public void changeView(float x, float y, float z, float a, float b, float c)
        {
            view = Matrix.CreateLookAt(new Vector3(x, y, z), new Vector3(a, b, c), Vector3.UnitY); // 0,5,8
        }

        public void changeWorld(float angle)
        {
            world = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);
        }

        public void printElevation()
        {
            if (elevationType == elevation.plain)
            {
                if (sourceBlock == true)
                {
                    Debug.Write("====|");
                    //Console.Write("====|");
                }
                else
                {
                    Debug.Write("____|");
                    //Console.Write("____|");
                }
            }

            else if (elevationType == elevation.hill)
            {
                if (sourceBlock == true)
                {
                    Debug.Write("HILLL");
                    //Console.Write("HILLL");
                }
                else
                {
                    Debug.Write("hilll");
                    //Console.Write("hilll");
                }
            }
            else if (elevationType == elevation.mountain)
            {
                if (sourceBlock == true)
                {
                    Debug.Write("MOUNT");
                   //Console.Write("MOUNT");
                }
                else
                {
                    Debug.Write("mount");
                    //Console.Write("mount");
                }
            }
        }
    }
}
