using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TrabalhoPratico_Monogame_2ano
{
    abstract class ClsCamara
    {
        protected Vector3 _pos; // sprite posicao on screen       
        protected int _screenW, _screenH;
        protected float _pitch;
        protected float _yaw;
        protected float _verticalOffset;
        protected int _camaraValue;

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

        public abstract void Update(ClsTerreno terreno);

        protected void HandleMouseMovement()
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
        }
    }
}
