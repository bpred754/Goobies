using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Goobies
{
    public enum mapSize {normal,large,xLarge};

    public class Map
    {
        private Territory[,] map;
        private int height;
        private int width;
        private int heightWithBorder;
        private int widthWithBorder;
        private bool mapChanged = false;
        private mapSize mapSize;

        public int numMountains;
        public int numHills;
        private int numMountainsAfter;
        private int numHillsAfter;
        private int calls;

        // Constants
        private const int BORDER_SIZE = 2;
        private const int PERCENT_HILL = 5;
        private const int PERCENT_MOUNTAIN = 96; // 100 - PERCENT_MOUNTAIN = actual percent mountain

        // Constructor for random generated map
        public Map(int w, int h)
        {
            // Add 2 to height and width to allow algorithm to be applied more accurately to map edges
            height = h;
            width = w;
            heightWithBorder = h + 2 * BORDER_SIZE;
            widthWithBorder = w + 2 * BORDER_SIZE;

            // Initialize bordered map as a large plain
            Territory[,] borderedMap = new Territory[widthWithBorder, heightWithBorder];
            for (int i = 0; i < widthWithBorder; i++)
            {
                for (int j = 0; j < heightWithBorder; j++)
                    borderedMap[i, j] = new Territory(i,j);
            }

            borderedMap = buildBorderedMap(borderedMap);

            map = new Territory[width, height];
            map = transformMap(borderedMap);
            assignSource(map);
        }
        
        // Constructor for custom made map
        public Map(Territory[,] mapGrid)
        {
            width = mapGrid.GetLength(0);
            height = mapGrid.GetLength(1);

            map = new Territory[width, height];
            map = mapGrid;
        }

        public Map getPlainMap()
        {
            Territory[,] mapGrid = new Territory[width, height];

            // Create map with all plains
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    mapGrid[i, j] = new Territory(i, j);
                }
            }
            Map plainMap = new Map(mapGrid);

            return plainMap;
        }

        // Build map with dimensions height + BORDER_SIZE and width + BORDER_SIZE to give actual map a feel of realism
        private Territory[,] buildBorderedMap(Territory[,] m)
        {
            Territory[,] tempMap = m;

            // Logic to change tempMap (original Creation)
            var random = new Random();
            for (int i = 0; i < widthWithBorder; i++)
            {
                for (int j = 0; j < heightWithBorder; j++)
                {
                    int randomVal = random.Next(1,101);

                    if (randomVal <= PERCENT_HILL)
                    {
                        tempMap[i, j].setElevation(elevation.hill);
                        numHills++;
                    }
                    else if (randomVal > PERCENT_MOUNTAIN)
                    {
                        tempMap[i, j].setElevation(elevation.mountain);
                        numMountains++;
                    }
                }
            }
            return buildMapHelper(m);
        }

        // Recursive function for buildMap
        public Territory[,] buildMapHelper(Territory[,] m)
        {
            Territory[,] tempMap = m;
            calls++;
     
            mapChanged = false;
            Random random = new Random();
            for (int i = BORDER_SIZE; i < width+BORDER_SIZE; i++)
            {
                for (int j = BORDER_SIZE; j < height+BORDER_SIZE; j++)
                {
                    // if current territory is a mountain
                    if (m[i, j].getElevationStatus() == elevation.mountain)
                    {
                        //check for planes in cardinal directions
                        if (m[i, j - 1].getElevationStatus() == elevation.plain) // Check left
                            updateElevation(tempMap[i, j - 1], getSurroundings(m, i, j - 1), random.Next(1, 101), random.Next(1, 101));
                        if (m[i + 1, j].getElevationStatus() == elevation.plain) // Check up
                            updateElevation(tempMap[i + 1, j], getSurroundings(m, i + 1, j), random.Next(1, 101), random.Next(1, 101));
                        if (m[i, j + 1].getElevationStatus() == elevation.plain) // Check right
                            updateElevation(tempMap[i, j + 1], getSurroundings(m, i, j + 1), random.Next(1, 101), random.Next(1, 101));
                        if (m[i - 1, j].getElevationStatus() == elevation.plain) // Check Down
                            updateElevation(tempMap[i - 1, j], getSurroundings(m, i - 1, j), random.Next(1, 101), random.Next(1, 101));
                    }
                }
            }

            if (mapChanged) // if temp map was changed call buildHelper again
                m = buildMapHelper(tempMap);
            
            return m;
        }

        //Check surroundings of the location at m[row,col] for mountains. If there is a mountain increment counter.
        //When all surroundings are checked return the number of surrounding mountains
        public int getSurroundings(Territory[,] m, int row, int col)
        {
            int numMountains = 0;

            if (m[col-1,row].getElevationStatus() == elevation.mountain || // Check left
                m[col,row+1].getElevationStatus() == elevation.mountain || // Check up
                m[col+1,row].getElevationStatus() == elevation.mountain || // Check right
                m[col,row-1].getElevationStatus() == elevation.mountain)   // Check down
                numMountains++;

            return numMountains;
        }

        // Set the elevation of territory t depending on the number of mountains surrounding it
        public void updateElevation(Territory t, int numMountains,int r1, int r2)
        {
            elevation e;
            bool setElevation = false;
            Random random = new Random();
            int hillMountainRandom = r1;
            int elevateRandom = r2;
            int degrade = (calls - 1) * 2;

            if(hillMountainRandom < 50)
                e = elevation.hill;
            else
                e = elevation.mountain;

            if (numMountains == 1)
            {
                if (elevateRandom < 20 - degrade)
                    setElevation = true;
            }
            else if (numMountains == 2)
            {
                if (elevateRandom < 30 - degrade)
                    setElevation = true;
            }
            else if (numMountains == 3)
            {
                if (elevateRandom < 50 - degrade)
                    setElevation = true;
            }
            else if (numMountains == 4)
                setElevation = true;

            if (setElevation)
            {
                t.setElevation(e);

                if (e == elevation.hill)
                    numHillsAfter++;

                // recurse only if a plain turns to mountain
                else if (e == elevation.mountain)
                {
                    mapChanged = true;
                    numMountainsAfter++;
                }
            }
        }

        // Convert Bordered map to a map with desired size
        public Territory[,] transformMap(Territory[,] m)
        {
            Territory[,] transformedMap = new Territory[height, width];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    transformedMap[i, j] = m[i + BORDER_SIZE, j + BORDER_SIZE];
                    transformedMap[i, j].setMapLocationX(i);
                    transformedMap[i, j].setMapLocationY(j);
                }
            }
            return transformedMap;
        }

        // Randomly assign source blocks to territories
        public void assignSource(Territory[,] m)
        {
            Random random = new Random();
            int r = random.Next(1, 6);
            
            for(int i = 0; i < r; i++)
            {
                int x = random.Next(0, width);
                int y = random.Next(0, height);

                if (m[x, y].getSourceBlock() == true)
                    r += 1;
                else
                    m[x, y].setSourceBlock(true);
            }            
        }

        // Takes map back to its original state with no goobies and all territories not owned
        public void resetMap()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                    map[i, j].reset();
            }
        }

        /*******************************************************************/
        /*  SETTERS AND GETTERS
        /*******************************************************************/

        public int getHeight()
        {
            return height;
        }

        public int getWidth()
        {
            return width;
        }

        public Territory get(int x, int y)
        {
            return map[x, y];
        }

        public void setMapSize(mapSize mapSize)
        {
            this.mapSize = mapSize;
        }

        public mapSize getMapSize()
        {
            return mapSize;
        }

        /*******************************************************************/
        /*  DEBUGGING
        /*******************************************************************/

        public void printMap()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    map[i, j].printElevation();
                    Debug.Write(" ");
                 //   Console.Write(" ");
                }
                Debug.WriteLine("");
               // Console.WriteLine(" ");
            }
            /*
            Debug.WriteLine("Mountains Before "+numMountains);
            Debug.WriteLine("Hills Before " + numHills);
           
            Debug.WriteLine("Calls " + calls);
            Debug.WriteLine("Mountains After " + numMountainsAfter);
            Debug.WriteLine("Hills After " + numHillsAfter);
             */
        }
    }
}
