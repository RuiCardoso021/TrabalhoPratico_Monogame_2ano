using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrabalhoPratico_Monogame_2ano.Components;

namespace TrabalhoPratico_Monogame_2ano.Camara
{
    internal class ClsThirdPersonCamera : ClsCamera
    {
        private ClsTank _tank;

        public ClsThirdPersonCamera(GraphicsDevice device, ClsTank tank) : base(device)
        {
            _tank = tank;
        }

        public override void Update(ClsTerrain terrain, GameTime gametime)
        {
            HandleMouseMovement();
            _pos = _tank._pos;
            _pos.Y = 5f;
            Vector3 right = Vector3.Cross(_tank.direction, Vector3.UnitY);
            Vector3 up = Vector3.Cross(right, _tank.direction);
            _pos = _pos - _tank.direction * 13f + _tank.normal * 5f;
            Vector3 target = _pos + _tank.direction;
            view = Matrix.CreateLookAt(_pos, target, up);
        }
    }
}