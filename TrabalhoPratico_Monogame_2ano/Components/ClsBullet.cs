using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrabalhoPratico_Monogame_2ano.Components
{
    internal class ClsBullet
    {
        private float _gravity = 7.5f;
        private float _scale = 0.2f;
        private Model _bulletModel;
        private Vector3 _velocity;
        private Matrix _world;

        public Vector3 Position;
        public Vector3 LastPosition;

        public ClsBullet(Model bulletModel, Vector3 tankPosition, Vector3 tankDirection)
        {
            this._bulletModel = bulletModel;
            this.Position = tankPosition;

            _velocity = tankDirection;
            _velocity.Normalize();
            _velocity *= 20f;
        }

        public void Update(GameTime gameTime)
        {
            LastPosition = Position;

            _velocity += Vector3.Down * _gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += _velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            Matrix escala = Matrix.CreateScale(_scale);
            Matrix translacao = Matrix.CreateTranslation(Position);

            _world = escala * translacao;
        }

        public void Draw()
        {
            foreach (ModelMesh mesh in _bulletModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = _world;
                    effect.View = ClsCamera.Instance.view;
                    effect.Projection = ClsCamera.Instance.projection;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }
    }
}