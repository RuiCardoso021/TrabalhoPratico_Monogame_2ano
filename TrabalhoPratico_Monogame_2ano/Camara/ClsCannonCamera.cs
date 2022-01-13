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
            _posititon = tank.CannonPosition;
            Vector3 direction = tank.CannonDirection;
            direction.Normalize();
            Vector3 right = Vector3.Cross(direction, Vector3.UnitY);
            Vector3 up = Vector3.Cross(right, direction);
            _posititon = _posititon - direction * -3f;
            Vector3 target = _posititon + direction;
            View = Matrix.CreateLookAt(_posititon, target, up);
        }
    }
}