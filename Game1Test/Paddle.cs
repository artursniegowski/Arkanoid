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
    public enum PaddleType
    {
        Paddle_Normal =0,
        Paddle_Long
    };

    class Paddle : GameObject
    {
        public float speed = 500;

        public Paddle(Game myGame): base(myGame)
        {
            textureName = "paddle";
        }

        public override void Update(float deltaTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            if(keyState.IsKeyDown(Keys.Left))
            {
                position.X -= speed * deltaTime;
            }
            else if (keyState.IsKeyDown(Keys.Right))
            {
                position.X += speed * deltaTime;
            }
            position.X = MathHelper.Clamp(position.X, 32 + texture.Width / 2, 992 - texture.Width / 2);
            base.Update(deltaTime);
        }

        public void Swap_paddle(PaddleType PadType)
        {
            if(PaddleType.Paddle_Long == PadType)
            textureName = "paddle_long";
            else
            textureName = "paddle";

            this.LoadContent();
        }

        
    }
}
