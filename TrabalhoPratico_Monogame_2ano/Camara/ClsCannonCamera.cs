using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrabalhoPratico_Monogame_2ano.Componentes;

namespace TrabalhoPratico_Monogame_2ano.Camara
{
    internal class ClsCannonCamera : ClsCamara
    {
        private ClsTank _tank;

        public ClsCannonCamera(GraphicsDevice device, ClsTank tank) : base(device)
        {
            _tank = tank;
        }

        public override void Update(ClsTerrain terreno)
        {
            HandleMouseMovement();
            _pos = _tank._pos;
            Vector3 right = Vector3.Cross(_tank.direcaoCorrigida, Vector3.UnitY);
            Vector3 up = Vector3.Cross(right, _tank.direcaoCorrigida);

            if (_pos.X >= 0 && _pos.X < terreno.w - 1 && _pos.Z >= 0 && _pos.Z < terreno.h - 1)
            {
                _pos.Y = terreno.GetY(_pos.X, _pos.Z) + 6f;
            }

            Vector3 target = _pos + _tank.direcaoCorrigida;
            view = Matrix.CreateLookAt(_pos, target, up);
        }
    }
}