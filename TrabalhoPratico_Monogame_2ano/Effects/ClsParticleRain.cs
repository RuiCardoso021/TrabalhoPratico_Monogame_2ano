using System;
using Microsoft.Xna.Framework;

namespace TrabalhoPratico_Monogame_2ano.Effects
{
    class ClsParticleRain
    {
        public Vector3 pos;
        public Vector3 vel;

        public ClsParticleRain(Vector3 vel, Vector3 pos)
        {
            this.pos = pos;
            this.vel = vel;
        }

        public void Update(GameTime gameTime)
        {
            Vector3 a = new Vector3(0, -20f, 0);//acelaration 
            vel += a * (float)gameTime.ElapsedGameTime.TotalSeconds; //calculate gravity
            vel += Vector3.Down * new Random(1).Next(1) * (float)gameTime.ElapsedGameTime.TotalSeconds;//calculate valucity

            float velocity = vel.Length();
            Vector3 dir = vel;
            dir.Normalize();

            //calculate next position
            pos += velocity * dir * (float)gameTime.ElapsedGameTime.TotalSeconds;
            pos.Z += 0.06f; //chuva diagonal
        }
    }
}
