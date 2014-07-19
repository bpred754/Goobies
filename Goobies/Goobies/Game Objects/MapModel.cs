using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Goobies.Game_Objects
{
    public class MapModel
    {
        private Map map;
        private ContentManager content;

        public MapModel(Map map, ContentManager content)
        {
            this.map = map;
            this.content = content;

            // Load Models
            for (int i = 0; i < map.getWidth(); i++)
            {
                for (int j = 0; j < map.getHeight(); j++)
                    map.get(i, j).initializeTerritoryModel(content);
            }

            // Initialize all the models into memory.
            // NOTE: This takes about 30 seconds to load before game play
            // TODO: Create an array of the loaded models and create a method to pass to all territories.
            // The territories then could use the array instead of loading new objects each time the cursor moves.
            //loadModels(content);
        }

        // Load all the models into memory. A lag is caused when a cursor tries to move
        // onto a territory that it has not yet loaded
        public void loadModels(ContentManager content)
        {
            String[] elevationDirectory = new String[3] { "Models/Terrain/Plain/", "Models/Terrain/Hill/", "Models/Terrain/Mountain/" };
            String[] colorDirectory = new String[3] { "Black/", "Red/", "Blue/" };
            String[] modelFile = new String[6] { "Attack", "Cardinal", "CursorBlue", "CursorRed", "Highlighted", "Standard" };

            for (int i = 0; i < elevationDirectory.Count(); i++)
            {
                for (int j = 0; j < colorDirectory.Count(); j++)
                {
                    for (int k = 0; k < modelFile.Count(); k++)
                        content.Load<Model>(elevationDirectory[i] + colorDirectory[j] + modelFile[k]);
                }
            }
        }

        // Called in Draw method to draw models to the screen
        public void drawMap()
        {
            for (int i = 0; i < map.getWidth(); i++)
            {
                for (int j = 0; j < map.getHeight(); j++)
                    map.get(i, j).drawModel();
            }
        }

        public void updateCamera(Vector3 cameraPosition, Vector3 cameraTarget)
        {
            for (int i = 0; i < map.getWidth(); i++)
            {
                for (int j = 0; j < map.getHeight(); j++)
                {
                    map.get(i, j).updateView(cameraPosition, cameraTarget);

                    if (map.get(i, j).getGooby() != null)
                        map.get(i, j).getGooby().updateCamera(cameraPosition, cameraTarget, content);
                }
            }
        }

        // Takes map model back to its original state with no goobies and all territories not owned
        public void resetMapModel()
        {
            for (int i = 0; i < map.getWidth(); i++)
            {
                for (int j = 0; j < map.getHeight(); j++)
                    map.get(i, j).resetTerritoryModel();
            }
        }

        public Map getMap()
        {
            return map;
        }
    }
}
