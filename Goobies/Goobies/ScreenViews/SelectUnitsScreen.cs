using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Goobies.Game_Objects;
using Goobies.Game_Types;

namespace Goobies.ScreenView
{
    class SelectUnitsScreen : UserScreen
    {
        private Stack<UserScreen> screenStack;
        private GraphicsDevice graphics;
        private ContentManager content;
        private Map map;
        private MapModel mapModel;
        private GamePadState prevGamePadState;
        private Camera camera;

        private String title = "Select Goobies";
        private SpriteFont titleFont;
        private Vector2 titlePos;
        private Vector2 titleOrgin;

        private int screenCenterX;
        private int screenCenterY;

        private GoobiesSelectorBox selectorBox1;
        private GoobiesSelectorBox selectorBox2;

        private List<Player> playerList;
        private Player player1;
        private Player player2;

        private SelectUnitsPlayerController player1Controller;
        private SelectUnitsPlayerController player2Controller;

        private Vector2[] team1StartLocations;
        private Vector2[] team2StartLocations;

        private Texture2D[] redGoobyImages;
        private Texture2D[] blueGoobyImages;

        // DEBUG
        private KeyboardState oldState;

        public SelectUnitsScreen(GraphicsDevice graphics, ContentManager content, MapModel mapModel,Stack<UserScreen> screenStack, Camera camera)
        {
            this.graphics = graphics;
            this.content = content;
            this.mapModel = mapModel;
            this.map = mapModel.getMap();
            this.screenStack = screenStack;
            screenCenterX = graphics.Viewport.Bounds.Width / 2;
            screenCenterY = graphics.Viewport.Bounds.Height / 2;

            this.camera = camera;
            //initializeCamera();
            mapModel.updateCamera(camera.getCameraPosition(), camera.getCameraTarget());

            titleFont = content.Load<SpriteFont>("Fonts/ChooseMapScreenTitle");
            titlePos = new Vector2(screenCenterX, screenCenterY - 300);

            redGoobyImages = new Texture2D[] {content.Load<Texture2D>("GoobyImages/RedCircleGooby2D"),
                                              content.Load<Texture2D>("GoobyImages/RedSquareGooby2D"),
                                              content.Load<Texture2D>("GoobyImages/RedTriangleGooby2D"),
                                              content.Load<Texture2D>("GoobyImages/RedDiamondGooby2D")};
            blueGoobyImages = new Texture2D[] {content.Load<Texture2D>("GoobyImages/BlueCircleGooby2D"),
                                               content.Load<Texture2D>("GoobyImages/BlueSquareGooby2D"),
                                               content.Load<Texture2D>("GoobyImages/BlueTriangleGooby2D"),
                                               content.Load<Texture2D>("GoobyImages/BlueDiamondGooby2D")};

            selectorBox1 = new GoobiesSelectorBox(graphics,content, 90, 125, redGoobyImages);
            selectorBox2 = new GoobiesSelectorBox(graphics, content, graphics.Viewport.Bounds.Width - (int)selectorBox1.getWidth(), 125, blueGoobyImages);

            player1 = new Player(map, 1, 1, 0);
            player2 = new Player(map, map.getWidth()-2, map.getHeight()-2, 1);
            player1.setEnemy(player2);
            player2.setEnemy(player1);
            initializePlayer2();

            playerList = new List<Player>();
            playerList.Add(player1);
            playerList.Add(player2);

            player1Controller = new SelectUnitsPlayerController(player1,selectorBox1,map);
            //player2Controller = new SelectUnitsPlayerController(player2, selectorBox2, map);

            int width = map.getWidth() - 1;
            int height = map.getHeight() - 1;

            team1StartLocations = new Vector2[] { new Vector2(0, 0), new Vector2(0, 2), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(2, 0) };
            //team2StartLocations = new Vector2[] { new Vector2(width,height), new Vector2(width, height-1), 
                                  //new Vector2(width, height-2), new Vector2(width-1,height-1), new Vector2(width-2,height), new Vector2(width-1, height)};

            player1Controller.initializeTeamTerritories(team1StartLocations);
            //player2Controller.initializeTeamTerritories(team2StartLocations);

            player1.getCursor().updateTerritory();
            player2.getCursor().updateTerritory();
        }

        

        public void drawScreen(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            titleOrgin = titleFont.MeasureString(title) / 2;
            spriteBatch.DrawString(titleFont, title, titlePos, Color.Red, 0, titleOrgin, 1.0f, SpriteEffects.None, .5f);
            selectorBox1.draw(spriteBatch);
            selectorBox2.draw(spriteBatch);
            spriteBatch.End();

            graphics.DepthStencilState = DepthStencilState.Default; // Fixes buffer issue when drawing 3d models
            mapModel.drawMap();

            mapModel.updateCamera(camera.getCameraPosition(), camera.getCameraTarget());
        }

        public void listen(GamePadState gamePadState)
        {
            player1Controller.listen(gamePadState);
            //player2Controller.listen(gamePadState);
            if (gamePadState.Buttons.Start == ButtonState.Pressed && prevGamePadState.Buttons.Start == ButtonState.Released)
            {
                GoobiesGame game = new GoobiesGame(mapModel, playerList);
                screenStack.Push(new GameScreen(graphics, content, mapModel, game));
            }

            if (gamePadState.Buttons.B == ButtonState.Pressed && prevGamePadState.Buttons.B == ButtonState.Released)
            {
                mapModel.resetMapModel();
                map.resetMap();
                screenStack.Pop();
                Thread.Sleep(200);
            }

            prevGamePadState = gamePadState;
        }

        public UserScreen getNextScreen()
        {
            return screenStack.Peek();
        }

        /*******************************************************************/
        /*  DEBUGGING
        /*******************************************************************/

        public void initializePlayer2()
        {
            int mapWidth = map.getWidth();
            int mapHeight = map.getHeight();

            Unit player2Circle = new CircleGooby(map, 1, mapWidth - 2, mapHeight - 2);
            Unit player2Circle1 = new CircleGooby(map, 1, mapWidth - 2, mapHeight - 1);
            Unit player2Circle3 = new CircleGooby(map, 1, mapWidth - 3, mapHeight - 1);
            Unit player2Circle2 = new CircleGooby(map, 1, mapWidth - 1, mapHeight - 2);
            Unit player2Circle4 = new CircleGooby(map, 1, mapWidth - 1, mapHeight - 3);
            player2.addUnit(player2Circle);
            player2.addUnit(player2Circle1);
            player2.addUnit(player2Circle2);
            player2.addUnit(player2Circle3);
            player2.addUnit(player2Circle4);
        }

        /*******************************************************************/
        /*  DEBUG -- Keyboard listeners
        /*******************************************************************/

        public void listenForKeyboard(KeyboardState newState)
        {
            player1Controller.listenForKeyboard(newState);
            if (newState.IsKeyDown(Keys.Enter))
            {
                if (!oldState.IsKeyDown(Keys.Enter))
                {
                    GoobiesGame game = new GoobiesGame(mapModel, playerList);
                    screenStack.Push(new GameScreen(graphics, content, mapModel, game));
                }
            }

            // TODO:First space switches player, second space starts game

            if (newState.IsKeyDown(Keys.Back))
            {
                if (!oldState.IsKeyDown(Keys.Back))
                {
                    mapModel.resetMapModel();
                    map.resetMap();
                    screenStack.Pop();
                    Thread.Sleep(200);
                }
            }

            oldState = newState;
        }
    }
}
