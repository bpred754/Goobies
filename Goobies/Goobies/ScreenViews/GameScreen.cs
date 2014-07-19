using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Goobies.Game_Types;
using Goobies.Game_Objects;

namespace Goobies.ScreenView
{
    public class GameScreen : UserScreen
    {
        private GraphicsDevice graphics;
        private ContentManager content;

        private MapModel mapModel;
        private GoobiesGame game;

        public GameScreen(GraphicsDevice graphics, ContentManager content, MapModel mapModel, GoobiesGame game)
        {
            this.graphics = graphics;
            this.content = content;

            this.mapModel = mapModel;
            this.game = game;

        }

        public void drawScreen(SpriteBatch spriteBatch)
        {
            mapModel.drawMap();
        }

        public void listen(GamePadState gamePadState)
        {
            game.listen(gamePadState);
        }

        /*******************************************************************/
        /*  DEBUG -- Keyboard listeners
        /*******************************************************************/

        public void listenForKeyboard(KeyboardState newState)
        {
            game.listenForKeyboard(newState);
        }
    }
}
