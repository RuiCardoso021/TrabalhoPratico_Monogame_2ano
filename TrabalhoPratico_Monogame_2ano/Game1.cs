using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TrabalhoPratico_Monogame_2ano
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private ClsTerreno _terreno;
        private ClsCamara _camara;

        public Game1(){
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        protected override void Initialize(){
            base.Initialize();
        }

        protected override void LoadContent() {
            Mouse.SetPosition(_graphics.GraphicsDevice.Viewport.Width / 2, _graphics.GraphicsDevice.Viewport.Height / 2);
            _terreno = new ClsTerreno(_graphics.GraphicsDevice, Content.Load<Texture2D>("lh3d1"), Content.Load<Texture2D>("texture"));
            _camara = new ClsCamara(_graphics.GraphicsDevice);

        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _camara.Update(_terreno);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _terreno.Draw(_graphics.GraphicsDevice, _camara.view, _camara.projection);
            base.Draw(gameTime);
        }
    }
}
