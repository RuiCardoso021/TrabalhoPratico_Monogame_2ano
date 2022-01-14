using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrabalhoPratico_Monogame_2ano.Components
{
    internal class ClsBullet
    {
        private float _gravity = 9.4f;
        private float _scale = 0.005f;
        private Model _bulletModel;
        private Vector3 _velocity;
        private Matrix _world;

        public Vector3 Position;
        public Vector3 LastPosition;

        public ClsBullet(Model bulletModel, Vector3 tankPosition, Vector3 tankDirection)
        {
            _bulletModel = bulletModel;
            Position = tankPosition;

            _velocity = tankDirection;
            _velocity.Normalize();
            _velocity *= 20f;
        }

        public void Update(GameTime gameTime)
        {
            LastPosition = Position;

            _velocity += Vector3.Down * _gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += _velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            Matrix scale = Matrix.CreateScale(_scale);
            Matrix translation = Matrix.CreateTranslation(Position);

            _world = scale * translation;
        }

        public void Draw()
        {
            foreach (ModelMesh mesh in _bulletModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = _world;
                    effect.View = ClsCamera.Instance.View;
                    effect.Projection = ClsCamera.Instance.Projection;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }
    }
}