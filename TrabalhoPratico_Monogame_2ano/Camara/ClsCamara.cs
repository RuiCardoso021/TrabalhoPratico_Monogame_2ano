using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace TrabalhoPratico_Monogame_2ano
{
    class ClsCamara
    {
        private Vector3 _pos; // sprite posicao on screen       
        private float _pitch;
        private float _yaw;
        private int _screenW, _screenH;
        private float _verticalOffset;
        private int _camaraValue;

        public Matrix view, projection;

        public ClsCamara(GraphicsDevice device)
        {
            _screenH = device.Viewport.Height;
            _screenW = device.Viewport.Width;
            float aspectRatio = (float)device.Viewport.Width / device.Viewport.Height;
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60f), aspectRatio, 0.1f, 1000f);
            _pos = new Vector3(64f, 20f, 64f);
            _pitch = 0;
            _yaw = 0;
            _verticalOffset = 2f;
            _camaraValue = 0;
        }


        public void Update(ClsTerreno terreno)
        {
            MouseState ms = Mouse.GetState();
            Vector2 mouseOffset = ms.Position.ToVector2() - new Vector2(_screenW / 2, _screenH / 2);
            float radianosPorPixel = MathHelper.ToRadians(0.1f);
            _yaw = _yaw - mouseOffset.X * radianosPorPixel;
            _pitch = _pitch + mouseOffset.Y * radianosPorPixel;

            //limitar camara para nao inverter
            if (_pitch > MathHelper.ToRadians(85f))
                _pitch = MathHelper.ToRadians(85f);
            if (_pitch < MathHelper.ToRadians(-85f))
                _pitch = MathHelper.ToRadians(-85f);


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
            if (kb.IsKeyDown(Keys.NumPad7)) //cima
                _pos = _pos + up * speed;
            if (kb.IsKeyDown(Keys.NumPad1)) //baixo
                _pos = _pos - up * speed;

            //alteraçao da prespetiva da camara
            if (kb.IsKeyDown(Keys.F1))
                _camaraValue = 0;
            else if (kb.IsKeyDown(Keys.F2))
                _camaraValue = 2;

            switch (_camaraValue) {
                case 0:
                    if (_pos.X >= 0 && _pos.X < terreno.w - 1 && _pos.Z >= 0 && _pos.Z < terreno.h - 1)
                        _pos.Y = terreno.GetY(_pos.X, _pos.Z) + _verticalOffset;
                    break;
                default:
                    break;
            }             

            Vector3 target = _pos + direction;
            view = Matrix.CreateLookAt(_pos, target, up);

            Mouse.SetPosition(_screenW / 2, _screenH / 2);
        }
    }
}
