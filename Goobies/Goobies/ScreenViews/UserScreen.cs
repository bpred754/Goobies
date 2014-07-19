using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Goobies.ScreenView
{
    public interface UserScreen
    {
         void drawScreen(SpriteBatch spriteBatch);
         void listen(GamePadState gamePadState);

        /*******************************************************************/
        /*  DEBUG -- Keyboard listeners
        /*******************************************************************/

         void listenForKeyboard(KeyboardState newState);
    }
}
