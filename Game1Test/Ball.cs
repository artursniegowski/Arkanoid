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
    class Ball : GameObject
    {
        public float speed = 400;
        public Vector2 direction = new Vector2(0.707f, -0.707f);
        public bool caught = false;
        public bool ToRemove = false;
        float prev_pos_x = 0.0f;
        bool One_cycle = false;

        public Ball(Game myGame): base(myGame)
        {
            textureName = "ball";
        }

        public override void Update(float deltaTime)
        {
            if (caught == false)
            {
                One_cycle = true;
                position += direction * speed * deltaTime;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                caught = false;
            }
            base.Update(deltaTime);
        }

        public void Check_catch(Vector2 pos)
        {
            if (One_cycle)
            {
                prev_pos_x = pos.X;
                One_cycle = false;
            }

            if(caught)
            {
                position.X +=  (pos.X - prev_pos_x); 
               // position.X = position.X + (pos.X - position.X);// + (pos.X-position.X);  
                prev_pos_x = pos.X;
            }
        }

    }
}
