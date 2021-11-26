using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TrabalhoPratico_Monogame_2ano.Components;

namespace TrabalhoPratico_Monogame_2ano.Camara
{
    internal class ClsSurfaceFollowCamera : ClsCamera
    {
        public ClsSurfaceFollowCamera(GraphicsDevice device) : base(device)
        {
        }

        public override void Update(GameTime gametime, ClsTerrain terrain, ClsTank tank)
        {
            HandleMouseMovement();

            Matrix rotacao = Matrix.CreateFromYawPitchRoll(_yaw, _pitch, 0f);
            Vector3 direction = Vector3.Transform(-Vector3.UnitZ, rotacao);
            Vector3 right = Vector3.Cross(direction, Vector3.UnitY);
            Vector3 up = Vector3.Cross(right, direction);
            Vector3 lastPostion = _pos;
            float speed = 20f;

            //direcao da camara
            _pos = _kbManager.MovimentWithPosition(_pos, right, speed, Keys.NumPad6, Keys.NumPad4, gametime);      //esquerda e direita
            _pos = _kbManager.MovimentWithPosition(_pos, direction, speed, Keys.NumPad8, Keys.NumPad5, gametime);  //frente e traz

            if (_pos.X >= 0 && _pos.X < terrain.w - 1 && _pos.Z >= 0 && _pos.Z < terrain.h - 1)
                _pos.Y = terrain.GetY(_pos.X, _pos.Z) + _verticalOffset;
            else _pos = lastPostion;

            Vector3 target = _pos + direction;
            view = Matrix.CreateLookAt(_pos, target, up);

            Mouse.SetPosition(_screenW / 2, _screenH / 2);
        }
    }
}