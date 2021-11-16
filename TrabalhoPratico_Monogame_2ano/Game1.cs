﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TrabalhoPratico_Monogame_2ano.Camara;
using TrabalhoPratico_Monogame_2ano.Componentes;

namespace TrabalhoPratico_Monogame_2ano
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private ClsTerreno _terreno;
        private ClsCamara _camara;
        private ClsTank _tank;
        private int _viewMode = 0;

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
            _terreno = new ClsTerreno(_graphics.GraphicsDevice, Content.Load<Texture2D>("lh3d1"), Content.Load<Texture2D>("texture"));
            _camara = new ClsGhostCamera(_graphics.GraphicsDevice);
            _tank = new ClsTank(_graphics.GraphicsDevice, Content.Load<Model>("tank"), new Vector3(30f, 0f, 0f));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            HandleCamera();
            _tank.Update(gameTime, _terreno);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _terreno.Draw(_graphics.GraphicsDevice, _camara.view, _camara.projection);
            _tank.Draw(_graphics.GraphicsDevice, _camara.view, _camara.projection);
            base.Draw(gameTime);
        }
        
        private void HandleCamera()
        {
            KeyboardState kb = Keyboard.GetState();

            //alteraçao da prespetiva da camara
            if (kb.IsKeyDown(Keys.F1) && _viewMode != 0)
            {
                _camara = new ClsGhostCamera(_graphics.GraphicsDevice);
                _viewMode = 0;
            }
            else if (kb.IsKeyDown(Keys.F2) && _viewMode != 1)
            {
                _camara = new ClsSurfaceFollowCamera(_graphics.GraphicsDevice);
                _viewMode = 1;
            }
            else if (kb.IsKeyDown(Keys.F3) && _viewMode != 2)
            {
                _camara = new ClsSurfaceTank(_graphics.GraphicsDevice, _tank);
                _viewMode = 2;
            }

            _camara.Update(_terreno);
        }
    }
}