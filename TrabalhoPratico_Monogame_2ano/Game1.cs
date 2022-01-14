using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using TrabalhoPratico_Monogame_2ano.Components;
using TrabalhoPratico_Monogame_2ano.Effects;

namespace TrabalhoPratico_Monogame_2ano
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private ClsTerrain _terrain;
        private ClsTank _tank, _tankEnemy;
        private ClsRain _effectRain;
        private ClsSoundEffect _soundRain;
        private ClsObject _pokeball;
        private List<ClsObject> _pokeballList;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            _pokeballList = new List<ClsObject>();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Mouse.SetPosition(_graphics.GraphicsDevice.Viewport.Width / 2, _graphics.GraphicsDevice.Viewport.Height / 2);
            _terrain = new ClsTerrain(_graphics.GraphicsDevice, Content.Load<Texture2D>("lh3d1"), Content.Load<Texture2D>("sand"));
            _tank = new ClsTank(_graphics.GraphicsDevice, this, Content.Load<Model>("tank"), new Vector3(50f, 0f, 40f), false, new Keys[] { Keys.A, Keys.W, Keys.D, Keys.S, Keys.Q, Keys.E, Keys.F, Keys.H, Keys.T, Keys.G, Keys.LeftShift, Keys.Space });
            _tankEnemy = new ClsTank(_graphics.GraphicsDevice, this, Content.Load<Model>("tank"), new Vector3(64f, 0f, 64f), true, new Keys[] { Keys.J, Keys.I, Keys.L, Keys.K, Keys.N, Keys.M, Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.RightShift, Keys.Enter });
            _effectRain = new ClsRain(GraphicsDevice);
            _soundRain = new ClsSoundEffect(Content.Load<SoundEffect>("SoundEffect/rain"), 0.03f);

            for (int i = 0; i < 20; i++)
            {
                _pokeball = new ClsObject(Content.Load<Model>("pokeball"), _terrain);
                _pokeballList.Add(_pokeball);
            }

            ClsCamera.CreateCamera(_graphics.GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            ClsCamera.UpdateCamera(gameTime, _graphics.GraphicsDevice, _tank, _terrain);
            _tank.Update(gameTime, _terrain, _tankEnemy);
            _tankEnemy.Update(gameTime, _terrain, _tank);
            _effectRain.Update(gameTime, _terrain);
            _soundRain.PlayWithLoop();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGray);
            _effectRain.Draw(ClsCamera.Instance.View, ClsCamera.Instance.Projection);
            _terrain.Draw(_graphics.GraphicsDevice, ClsCamera.Instance.View, ClsCamera.Instance.Projection);
            _tank.Draw(_graphics.GraphicsDevice, ClsCamera.Instance.View, ClsCamera.Instance.Projection, Vector3.Zero);
            _tankEnemy.Draw(_graphics.GraphicsDevice, ClsCamera.Instance.View, ClsCamera.Instance.Projection, Vector3.UnitX);
            foreach (ClsObject pokeball in _pokeballList)
                pokeball.Draw();

            base.Draw(gameTime);
        }
    }
}