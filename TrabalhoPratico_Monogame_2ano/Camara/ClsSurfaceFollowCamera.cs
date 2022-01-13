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
            Vector3 lastPostion = _posititon;
            float speed = 20f;

            //direcao da camara
            _posititon = _kbManager.MovimentWithPosition(_posititon, right, speed, Keys.NumPad6, Keys.NumPad4, gametime);      //esquerda e direita
            _posititon = _kbManager.MovimentWithPosition(_posititon, direction, speed, Keys.NumPad8, Keys.NumPad5, gametime);  //frente e traz

            if (_posititon.X >= 0 && _posititon.X < terrain.W - 1 && _posititon.Z >= 0 && _posititon.Z < terrain.H - 1)
                _posititon.Y = terrain.GetY(_posititon.X, _posititon.Z) + _verticalOffset;
            else _posititon = lastPostion;

            Vector3 target = _posititon + direction;
            View = Matrix.CreateLookAt(_posititon, target, up);

            Mouse.SetPosition(_screenW / 2, _screenH / 2);
        }
    }
}