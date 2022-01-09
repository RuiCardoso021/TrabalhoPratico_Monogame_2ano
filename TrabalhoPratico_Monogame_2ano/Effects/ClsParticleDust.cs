using Microsoft.Xna.Framework;

namespace TrabalhoPratico_Monogame_2ano.Effects
{
    public class ClsParticleDust
    {
        public Vector3 position;
        public Vector3 velocity;

        public ClsParticleDust(Vector3 position, Vector3 velocity)
        {
            this.position = position;
            this.velocity = velocity;
        }

        public void Update(GameTime gameTime, Vector3 gravity)
        {
            velocity += gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}