using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrabalhoPratico_Monogame_2ano.Components;

namespace TrabalhoPratico_Monogame_2ano.Camara
{
    internal class ClsCannonCamera : ClsCamera
    {
        public ClsCannonCamera(GraphicsDevice device) : base(device)
        {
        }

        public override void Update(GameTime gametime, ClsTerrain terrain, ClsTank tank)
        {
            HandleMouseMovement();
            _pos = tank.cannonPosition;
            Vector3 right = Vector3.Cross(tank.cannonDirection, Vector3.UnitY);
            Vector3 up = Vector3.Cross(right, tank.cannonDirection);
            _pos = _pos - tank.cannonDirection * -4f + tank.normal * 5f;
            Vector3 target = _pos + tank.direction;
            view = Matrix.CreateLookAt(_pos, target, up);
        }
    }
}