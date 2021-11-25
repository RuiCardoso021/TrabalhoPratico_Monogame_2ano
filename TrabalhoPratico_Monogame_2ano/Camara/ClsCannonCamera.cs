using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrabalhoPratico_Monogame_2ano.Componentes;

namespace TrabalhoPratico_Monogame_2ano.Camara
{
    internal class ClsCannonCamera : ClsCamera
    {
        private ClsTank _tank;

        public ClsCannonCamera(GraphicsDevice device, ClsTank tank) : base(device)
        {
            _tank = tank;
        }

        public override void Update(ClsTerrain terrain)
        {
            HandleMouseMovement();
            _pos = _tank._pos;
            Vector3 right = Vector3.Cross(_tank.correctedDirection, Vector3.UnitY);
            Vector3 up = Vector3.Cross(right, _tank.correctedDirection);

            if (_pos.X >= 0 && _pos.X < terrain.w - 1 && _pos.Z >= 0 && _pos.Z < terrain.h - 1)
            {
                _pos.Y = terrain.GetY(_pos.X, _pos.Z) + 6f;
            }

            Vector3 target = _pos + _tank.correctedDirection;
            view = Matrix.CreateLookAt(_pos, target, up);
        }
    }
}