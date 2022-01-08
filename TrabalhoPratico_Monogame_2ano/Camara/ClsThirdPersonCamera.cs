using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrabalhoPratico_Monogame_2ano.Components;

namespace TrabalhoPratico_Monogame_2ano.Camara
{
    internal class ClsThirdPersonCamera : ClsCamera
    {
        public ClsThirdPersonCamera(GraphicsDevice device) : base(device)
        {
        }

        public override void Update(GameTime gametime, ClsTerrain terrain, ClsTank tank)
        {
            HandleMouseMovement();
            _pos = tank._pos;
            _pos.Y = 5f;
            Vector3 right = Vector3.Cross(tank.direction, Vector3.UnitY);
            Vector3 up = Vector3.Cross(right, tank.direction);
            _pos = _pos - tank.direction * 20f + tank.normal * 5f;
            Vector3 target = _pos + tank.direction;
            view = Matrix.CreateLookAt(_pos, target, up);
        }
    }
}