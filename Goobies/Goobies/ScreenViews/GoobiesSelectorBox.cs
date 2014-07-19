using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Goobies.Game_Objects;

namespace Goobies.ScreenView
{
    public class GoobiesSelectorBox
    {
        private GraphicsDevice graphics;

        private Texture2D frameTexture;
        private Rectangle[] frames;

        private Texture2D boxTexture;
        private Rectangle[] boxes;

        private Texture2D selectedBoxTexture;
        private Rectangle[] selectedBoxes;

        private int selectorBoxIndex = 0;

        private Texture2D[] goobyImages;

        private SpriteFont font;
        private String LT = "LT";
        private Vector2 LTPos;
        private Vector2 LTOrgin;
        private String RT = "RT";
        private Vector2 RTPos;
        private Vector2 RTOrgin;

        // Constants
        private readonly int FRAME_WIDTH = 75;
        private readonly int FRAME_HEIGHT = 75;
        private readonly int BOX_WIDTH = 65;
        private readonly int BOX_HEIGHT = 65;
        private readonly int OFFSET = 5;
        private readonly int LEFT_OFFSET = 35;
        private readonly int RIGHT_OFFSET = 20;

        public GoobiesSelectorBox(GraphicsDevice graphics, ContentManager content, int x, int y, Texture2D[] goobyImages)
        {
            this.graphics = graphics;
            this.goobyImages = goobyImages;

            frameTexture = new Texture2D(graphics, 1, 1);
            frameTexture.SetData(new Color[] { Color.Black });

            boxTexture = new Texture2D(graphics, 1, 1);
            boxTexture.SetData(new Color[] { Color.White });

            selectedBoxTexture = new Texture2D(graphics, 1, 1);
            selectedBoxTexture.SetData(new Color[] { Color.LightYellow });

            //redCircleGoobyTexture = content.Load<Texture2D>("RedCircleGoobie2D");

            frames = new Rectangle[4];
            boxes = new Rectangle[4];
            selectedBoxes = new Rectangle[4];
            for (int i = 0; i < frames.Count(); i++)
            {
                frames[i] = new Rectangle(x + (i*(FRAME_WIDTH-OFFSET)), y, FRAME_WIDTH, FRAME_HEIGHT);
                boxes[i] = new Rectangle(x + (i * (FRAME_WIDTH - OFFSET)) + OFFSET, y + OFFSET, BOX_WIDTH, BOX_HEIGHT);
            }

            font = content.Load<SpriteFont>("Fonts/SelectorBox");
            LTPos = new Vector2(x-35, y+35);
            RTPos = new Vector2(x + FRAME_WIDTH*frames.Count()+ 20, y + 35);
        }

        public void draw(SpriteBatch spriteBatch)
        {
            Color selectedColor = Color.LightYellow;
            Color notSelectedColor = Color.White;

            for (int i = 0; i < frames.Count(); i++)
            {
                spriteBatch.Draw(frameTexture, frames[i], Color.Black);
                if(i == selectorBoxIndex)
                    spriteBatch.Draw(goobyImages[i],boxes[i],selectedColor);
                else
                    spriteBatch.Draw(goobyImages[i],boxes[i],notSelectedColor);
            }

            
            //spriteBatch.Draw(selectedBoxTexture, boxes[selectorBoxIndex], Color.LightYellow);

            LTOrgin = font.MeasureString(LT) / 2;
            RTOrgin = font.MeasureString(RT) / 2;
            spriteBatch.DrawString(font, LT, LTPos, Color.Black, 0, LTOrgin, 1.0f, SpriteEffects.None, .5f);
            spriteBatch.DrawString(font, RT, RTPos, Color.Black, 0, RTOrgin, 1.0f, SpriteEffects.None, .5f);
        }

        public void changeGoobieSelectorBox(direction direction)
        {
            if (direction == direction.left)
                selectorBoxIndex = decrementIndex(selectorBoxIndex);
            else if (direction == direction.right)
                selectorBoxIndex = incrementIndex(selectorBoxIndex);
        }

        public int incrementIndex(int index)
        {
            if (index != frames.Count() - 1)
                return ++index;
            else
                return index;
        }

        public int decrementIndex(int index)
        {
            if (index != 0)
            {
                return --index;
            }
            else
                return index;
        }

        public float getWidth()
        {
            return LEFT_OFFSET + BOX_WIDTH * 4 + OFFSET * 5 + RIGHT_OFFSET + font.MeasureString(RT).X;
        }

        public int getIndex()
        {
            return selectorBoxIndex;
        }
    }
}
