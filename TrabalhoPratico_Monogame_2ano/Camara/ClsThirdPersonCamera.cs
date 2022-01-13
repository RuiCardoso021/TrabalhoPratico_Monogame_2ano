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
            _posititon = tank.Position;
            _posititon.Y = 5f;
            Vector3 right = Vector3.Cross(tank.Direction, Vector3.UnitY);
            Vector3 up = Vector3.Cross(right, tank.Direction);
            _posititon = _posititon - tank.Direction * 20f + tank.Normal * 5f;
            Vector3 target = _posititon + tank.Direction;
            View = Matrix.CreateLookAt(_posititon, target, up);
        }
    }
}