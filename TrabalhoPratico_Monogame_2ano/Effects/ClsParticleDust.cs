using Microsoft.Xna.Framework;

namespace TrabalhoPratico_Monogame_2ano.Effects
{
    public class ClsParticleDust
    {
        public Vector3 Position;
        public Vector3 Velocity;

        public ClsParticleDust(Vector3 position, Vector3 velocity)
        {
            Position = position;
            Velocity = velocity;
        }

        public void Update(GameTime gameTime, Vector3 gravity)
        {
            Velocity += gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}