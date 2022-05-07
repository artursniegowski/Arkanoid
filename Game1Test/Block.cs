using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1Test
{
    public enum BlockColor
    {
       Red = 0,
       Yellow,
        Blue,
        Green,
        Purple,
        GreyHi,
        Grey,
    }

    class Block : GameObject
    {
        BlockColor active_color;

        public Block(BlockColor blockcolor ,Game myGame): base(myGame)
        {
            if (blockcolor == BlockColor.Red)
            {
                textureName = "block_red";
                active_color = BlockColor.Red;
            }
            if (blockcolor == BlockColor.Yellow)
            {
                textureName = "block_yellow";
                active_color = BlockColor.Yellow;
            }
            if (blockcolor == BlockColor.Blue)
            {
                textureName = "block_blue";
                active_color = BlockColor.Blue;
            }
            if (blockcolor == BlockColor.Green)
            {
                textureName = "block_green";
                active_color = BlockColor.Green;
            }
            if (blockcolor == BlockColor.Purple)
            {
                textureName = "block_purple";
                active_color = BlockColor.Purple;
            }
            if (blockcolor == BlockColor.GreyHi)
            {
                textureName = "block_grey_hi";
                active_color = BlockColor.GreyHi;
            }
            if (blockcolor == BlockColor.Grey)
            {
                textureName = "block_grey";
                active_color = BlockColor.Grey;
            }
            if (blockcolor == (BlockColor)9)
            {
                textureName = null;
            }
            
        }

        public bool onHit()
        {
            bool toDestroy = true;
            if (active_color == BlockColor.GreyHi)
            {
                toDestroy = false;
                active_color = BlockColor.Grey;
                textureName = "block_grey";
                this.LoadContent();
            }

            return toDestroy;
        }
    }
}
