﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TrabalhoPratico_Monogame_2ano.Camara;
using TrabalhoPratico_Monogame_2ano.KeyBoard;

namespace TrabalhoPratico_Monogame_2ano
{
    internal abstract class ClsCamera
    {
        protected Vector3 _pos; // sprite posicao on screen
        protected int _screenW, _screenH;
        protected float _pitch;
        protected float _yaw;
        protected float _verticalOffset;
        protected int _camaraValue;
        protected ClsKeyboardManager _kb;

        public Matrix view, projection;

        public ClsCamera(GraphicsDevice device)
        {
            _kb = new ClsKeyboardManager();
            _screenH = device.Viewport.Height;
            _screenW = device.Viewport.Width;
            float aspectRatio = (float)device.Viewport.Width / device.Viewport.Height;
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60f), aspectRatio, 0.1f, 1000f);
            _pos = new Vector3(64f, 20f, 64f);
            _pitch = 0;
            _yaw = 0;
            _verticalOffset = 5f;
            _camaraValue = 0;
        }

        public abstract void Update(ClsTerrain terrain, GameTime gametime);

        protected void HandleMouseMovement()
        {
            MouseState ms = Mouse.GetState();
            Vector2 mouseOffset = ms.Position.ToVector2() - new Vector2(_screenW / 2, _screenH / 2);
            float radianosPorPixel = MathHelper.ToRadians(0.1f);
            _yaw = _yaw - mouseOffset.X * radianosPorPixel;
            _pitch = _pitch + mouseOffset.Y * radianosPorPixel;

            //limitar camara para nao inverter
            _pitch = _kb.LimitAngle(_pitch, 85f, 85f);
        }
    }
}