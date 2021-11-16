using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TrabalhoPratico_Monogame_2ano.Componentes;

namespace TrabalhoPratico_Monogame_2ano.Camara
{
    internal class ClsSurfaceTank : ClsCamara
    {
        private ClsTank _tank;

        public ClsSurfaceTank(GraphicsDevice device, ClsTank tank) : base(device)
        {
            _tank = tank;
        }

        public override void Update(ClsTerreno terreno)
        {
            HandleMouseMovement();
            
            
            Matrix rotacao = Matrix.CreateFromYawPitchRoll(_yaw, _pitch, 0f);
            Vector3 direction = Vector3.Transform(-Vector3.UnitZ, rotacao);
            Vector3 right = Vector3.Cross(direction, Vector3.UnitY);
            Vector3 up = Vector3.Cross(right, direction);

            _pos = _tank._pos;
            _pos.Y += _verticalOffset;
            
            Vector3 target = _pos + direction;
            view = Matrix.CreateLookAt(_tank._pos, target, up);
            
            Mouse.SetPosition(_screenW / 2, _screenH / 2);
        }
    }
}
