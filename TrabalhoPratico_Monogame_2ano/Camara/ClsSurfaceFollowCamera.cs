using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TrabalhoPratico_Monogame_2ano.Camara
{
    internal class ClsSurfaceFollowCamera : ClsCamera
    {
        public ClsSurfaceFollowCamera(GraphicsDevice device) : base(device)
        {
        }

        public override void Update(ClsTerrain terreno)
        {
            HandleMouseMovement();

            Matrix rotacao = Matrix.CreateFromYawPitchRoll(_yaw, _pitch, 0f);
            Vector3 direction = Vector3.Transform(-Vector3.UnitZ, rotacao);
            Vector3 right = Vector3.Cross(direction, Vector3.UnitY);
            Vector3 up = Vector3.Cross(right, direction);

            KeyboardState kb = Keyboard.GetState();
            float speed = 0.3f;

            //direcao da camara
            if (kb.IsKeyDown(Keys.NumPad4)) //esquerda
                _pos = _pos - right * speed;
            if (kb.IsKeyDown(Keys.NumPad6)) //direita
                _pos = _pos + right * speed;
            if (kb.IsKeyDown(Keys.NumPad8)) //frente
                _pos = _pos + direction * speed;
            if (kb.IsKeyDown(Keys.NumPad2)) //trazs
                _pos = _pos - direction * speed;

            if (_pos.X >= 0 && _pos.X < terreno.w - 1 && _pos.Z >= 0 && _pos.Z < terreno.h - 1)
                _pos.Y = terreno.GetY(_pos.X, _pos.Z) + _verticalOffset;

            Vector3 target = _pos + direction;
            view = Matrix.CreateLookAt(_pos, target, up);

            Mouse.SetPosition(_screenW / 2, _screenH / 2);
        }
    }
}