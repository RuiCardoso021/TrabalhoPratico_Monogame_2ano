using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TrabalhoPratico_Monogame_2ano.KeyBoard
{
    public class ClsKeyboardManager
    {
        private KeyboardState _kb;

        public ClsKeyboardManager()
        {
            _kb = Keyboard.GetState();
        }

        public float Left_and_Right(float yaw, float speed, Keys teclaMais, Keys teclaMenos)
        {
            _kb = Keyboard.GetState();
            if (_kb.IsKeyDown(teclaMais))   //Lado Positivo
                yaw = yaw + MathHelper.ToRadians(speed);
            if (_kb.IsKeyDown(teclaMenos))  //Lado Negativo
                yaw = yaw - MathHelper.ToRadians(speed);

            return yaw;
        }

        public Vector3 MovimentWithPosition(Vector3 pos, Vector3 direction, float speed, Keys teclaMais, Keys teclaMenos, GameTime gameTime)
        {
            _kb = Keyboard.GetState();
            if (_kb.IsKeyDown(teclaMais))   //Lado Positivo
                pos = pos + direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_kb.IsKeyDown(teclaMenos))  //Lado Negativo
                pos = pos - direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            return pos;
        }

        public float LimitAngle(float yaw, float angleTop, float angleDown)
        {
            if (yaw > MathHelper.ToRadians(angleTop))
                yaw = MathHelper.ToRadians(angleTop);
            if (yaw < -MathHelper.ToRadians(angleDown))
                yaw = -MathHelper.ToRadians(angleDown);

            return yaw;
        }
    }
}