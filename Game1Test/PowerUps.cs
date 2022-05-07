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
    public enum PowerUpType
    {
       Ball_Catch = 0,
        Multi_Ball,
        Paddle_Size
    };

    class PowerUps : GameObject
    {
        public float speed = 50;
        public bool ToRemove = false;
        public PowerUpType powType;

        public PowerUps(PowerUpType Power_UP_Type,Game myGame)
            : base(myGame)
        {
            powType = Power_UP_Type;

            if (Power_UP_Type == PowerUpType.Ball_Catch)
            {
                textureName = "ball_catch";
            }
            if (Power_UP_Type == PowerUpType.Multi_Ball)
            {
                textureName = "Multi_Ball";
            }
            if (Power_UP_Type == PowerUpType.Paddle_Size)
            {
                textureName = "Paddle_Size";
            }
        }


        public override void Update(float deltaTime)
        {
            position.Y +=  speed * deltaTime;
            base.Update(deltaTime);

            if (position.Y > (768 + this.height))
            {
                ToRemove = true;
            }
        }
        
    }
}
