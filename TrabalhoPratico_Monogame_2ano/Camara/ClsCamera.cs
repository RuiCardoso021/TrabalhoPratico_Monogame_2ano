using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TrabalhoPratico_Monogame_2ano.Camara;
using TrabalhoPratico_Monogame_2ano.Components;
using TrabalhoPratico_Monogame_2ano.KeyBoard;

namespace TrabalhoPratico_Monogame_2ano
{
    internal abstract class ClsCamera
    {
        protected Vector3 _posititon; // sprite posicao on screen
        protected int _screenW, _screenH;
        protected float _pitch;
        protected float _yaw;
        protected float _verticalOffset;
        protected int _cameraValue;
        protected ClsKeyboardManager _kbManager;

        public static ClsCamera Instance;
        public Matrix View, Projection;

        public ClsCamera(GraphicsDevice device)
        {
            _kbManager = new ClsKeyboardManager();
            _screenH = device.Viewport.Height;
            _screenW = device.Viewport.Width;
            float aspectRatio = (float)device.Viewport.Width / device.Viewport.Height;
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60f), aspectRatio, 0.1f, 1000f);
            _posititon = new Vector3(64f, 20f, 64f);
            _pitch = 0;
            _yaw = 0;
            _verticalOffset = 5f;
            _cameraValue = 0;

            Instance = this;
        }

        public abstract void Update(GameTime gametime, ClsTerrain terrain, ClsTank tank);

        protected void HandleMouseMovement()
        {
            MouseState ms = Mouse.GetState();
            Vector2 mouseOffset = ms.Position.ToVector2() - new Vector2(_screenW / 2, _screenH / 2);
            float radiansPerPixel = MathHelper.ToRadians(0.1f);
            _yaw = _yaw - mouseOffset.X * radiansPerPixel;
            _pitch = _pitch + mouseOffset.Y * radiansPerPixel;

            //limitar camara para nao inverter
            _pitch = _kbManager.LimitAngle(_pitch, 85f, 85f);
        }

        internal static void CreateCamera(GraphicsDevice graphicsDevice)
        {
            Instance = new ClsThirdPersonCamera(graphicsDevice);
        }

        internal static void UpdateCamera(GameTime gameTime, GraphicsDevice graphicsDevice, ClsTank tank, ClsTerrain terrain)
        {
            KeyboardState kb = Keyboard.GetState();

            if (kb.IsKeyDown(Keys.F3) && !(Instance is ClsThirdPersonCamera) || Instance == null)
                Instance = new ClsThirdPersonCamera(graphicsDevice);
            else if (kb.IsKeyDown(Keys.F1) && !(Instance is ClsGhostCamera))
                Instance = new ClsGhostCamera(graphicsDevice);
            else if (kb.IsKeyDown(Keys.F2) && !(Instance is ClsSurfaceFollowCamera))
                Instance = new ClsSurfaceFollowCamera(graphicsDevice);
            else if (kb.IsKeyDown(Keys.F4) && !(Instance is ClsCannonCamera))
                Instance = new ClsCannonCamera(graphicsDevice);

            Instance.Update(gameTime, terrain, tank);
        }
    }
}