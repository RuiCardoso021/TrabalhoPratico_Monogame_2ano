using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TrabalhoPratico_Monogame_2ano.Components;
using TrabalhoPratico_Monogame_2ano.Effects;

namespace TrabalhoPratico_Monogame_2ano
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private ClsTerrain _terrain;
        public ClsTank _tank, _tankEnemy;
        private ClsRain _effectRain;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Mouse.SetPosition(_graphics.GraphicsDevice.Viewport.Width / 2, _graphics.GraphicsDevice.Viewport.Height / 2);
            _terrain = new ClsTerrain(_graphics.GraphicsDevice, Content.Load<Texture2D>("lh3d1"), Content.Load<Texture2D>("texture"));
            _tank = new ClsTank(_graphics.GraphicsDevice, Content.Load<Model>("tank"), new Vector3(50f, 0f, 40f), new Keys[] { Keys.A, Keys.W, Keys.D, Keys.S, Keys.Q, Keys.E, Keys.F, Keys.H, Keys.T, Keys.G });
            _tankEnemy = new ClsTank(_graphics.GraphicsDevice, Content.Load<Model>("tank"), new Vector3(64f, 0f, 64f), new Keys[] { Keys.J, Keys.I, Keys.L, Keys.K, Keys.O, Keys.P, Keys.Left, Keys.Right, Keys.Up, Keys.Down });
            _effectRain = new ClsRain(GraphicsDevice);

            ClsCamera.CreateCamera(_graphics.GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            ClsCamera.UpdateCamera(gameTime, _graphics.GraphicsDevice, _tank, _terrain);

            _tank.Update(gameTime, _terrain, this);
            _tankEnemy.Update(gameTime, _terrain, this);
            _effectRain.Update(gameTime, _terrain);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGray);

            _effectRain.Draw(ClsCamera.Instance.view, ClsCamera.Instance.projection);
            _terrain.Draw(_graphics.GraphicsDevice, ClsCamera.Instance.view, ClsCamera.Instance.projection);
            _tank.Draw(_graphics.GraphicsDevice, ClsCamera.Instance.view, ClsCamera.Instance.projection, Vector3.Zero);
            _tankEnemy.Draw(_graphics.GraphicsDevice, ClsCamera.Instance.view, ClsCamera.Instance.projection, Vector3.UnitX);
            base.Draw(gameTime);
        }
    }
}