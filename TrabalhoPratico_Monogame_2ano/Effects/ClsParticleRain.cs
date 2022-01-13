using System;
using Microsoft.Xna.Framework;

namespace TrabalhoPratico_Monogame_2ano.Effects
{
    class ClsParticleRain
    {
        public Vector3 Position;
        public Vector3 Velocity;

        public ClsParticleRain(Vector3 velocity, Vector3 position)
        {
            Position = position;
            Velocity = velocity;
        }

        public void Update(GameTime gameTime)
        {
            Vector3 a = new Vector3(0, -20f, 0);//acelaration 
            Velocity += a * (float)gameTime.ElapsedGameTime.TotalSeconds; //calculate gravity
            Velocity += Vector3.Down * new Random(1).Next(1) * (float)gameTime.ElapsedGameTime.TotalSeconds; //calculate valocity

            float velocity = Velocity.Length();
            Vector3 dir = Velocity;
            dir.Normalize();

            //calculate next position
            Position += velocity * dir * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position.Z += 0.06f; //chuva diagonal
        }
    }
}
