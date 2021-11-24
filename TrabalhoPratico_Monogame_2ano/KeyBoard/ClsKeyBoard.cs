using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TrabalhoPratico_Monogame_2ano.KeyBoard
{
    class ClsKeyBoard
    {

        private KeyboardState _kb;
        public float _yaw, _speed;

        public ClsKeyBoard(GraphicsDevice device)
        {
            _yaw = 0;
            _speed = 5f;
        }

        public void Update() {
            
        }

    }
}
