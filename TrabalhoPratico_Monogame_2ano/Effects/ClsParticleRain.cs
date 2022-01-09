using System;
using Microsoft.Xna.Framework;

namespace TrabalhoPratico_Monogame_2ano.Effects
{
    class ClsParticleRain
    {
        public Vector3 position;
        public Vector3 velocity;

        public ClsParticleRain(Vector3 velocity, Vector3 position)
        {
            this.position = position;
            this.velocity = velocity;
        }

        public void Update(GameTime gameTime)
        {
            Vector3 a = new Vector3(0, -20f, 0);//acelaration 
            this.velocity += a * (float)gameTime.ElapsedGameTime.TotalSeconds; //calculate gravity
            this.velocity += Vector3.Down * new Random(1).Next(1) * (float)gameTime.ElapsedGameTime.TotalSeconds; //calculate valocity

            float velocity = this.velocity.Length();
            Vector3 dir = this.velocity;
            dir.Normalize();

            //calculate next position
            position += velocity * dir * (float)gameTime.ElapsedGameTime.TotalSeconds;
            position.Z += 0.06f; //chuva diagonal
        }
    }
}
