using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TrabalhoPratico_Monogame_2ano.Camara;
using TrabalhoPratico_Monogame_2ano.Componentes;

namespace TrabalhoPratico_Monogame_2ano
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private ClsTerrain _terreno;
        private ClsCamera _camera;
        private ClsTank _tank, _tankEnemy;
        private int _viewMode = 2;

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
            _terreno = new ClsTerrain(_graphics.GraphicsDevice, Content.Load<Texture2D>("lh3d1"), Content.Load<Texture2D>("texture"));
            _tank = new ClsTank(_graphics.GraphicsDevice, Content.Load<Model>("tank"), new Vector3(50f, 0f, 40f), new Keys[] { Keys.A, Keys.W, Keys.D, Keys.S, Keys.Q, Keys.E, Keys.F, Keys.H, Keys.T, Keys.G});
            _tankEnemy = new ClsTank(_graphics.GraphicsDevice, Content.Load<Model>("tank"), new Vector3(64f, 0f, 64f), new Keys[] { Keys.J, Keys.I, Keys.L, Keys.K, Keys.O, Keys.P, Keys.Left, Keys.Right, Keys.Up, Keys.Down });
            _camera = new ClsThirdPersonCamera(_graphics.GraphicsDevice, _tank);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            HandleCamera(gameTime);
            _tank.Update(gameTime, _terreno);
            _tankEnemy.Update(gameTime, _terreno);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _terreno.Draw(_graphics.GraphicsDevice, _camera.view, _camera.projection);
            _tank.Draw(_graphics.GraphicsDevice, _camera.view, _camera.projection, Vector3.Zero);
            _tankEnemy.Draw(_graphics.GraphicsDevice, _camera.view, _camera.projection, Vector3.UnitX);
            base.Draw(gameTime);
        }

        private void HandleCamera(GameTime gameTime)
        {
            KeyboardState kb = Keyboard.GetState();

            //alteraçao da prespetiva da camara
            if (kb.IsKeyDown(Keys.F1) && _viewMode != 0)
            {
                _camera = new ClsGhostCamera(_graphics.GraphicsDevice);
                _viewMode = 0;
            }
            else if (kb.IsKeyDown(Keys.F2) && _viewMode != 1)
            {
                _camera = new ClsSurfaceFollowCamera(_graphics.GraphicsDevice);
                _viewMode = 1;
            }
            else if (kb.IsKeyDown(Keys.F3) && _viewMode != 2)
            {
                _camera = new ClsThirdPersonCamera(_graphics.GraphicsDevice, _tank);
                _viewMode = 2;
            }
            else if (kb.IsKeyDown(Keys.F4) && _viewMode != 3)
            {
                _camera = new ClsCannonCamera(_graphics.GraphicsDevice, _tank);
                _viewMode = 3;
            }

            _camera.Update(_terreno, gameTime);
        }
    }
}