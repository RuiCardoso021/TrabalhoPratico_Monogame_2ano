using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrabalhoPratico_Monogame_2ano.Components;

namespace TrabalhoPratico_Monogame_2ano.Camara
{
    internal class ClsCannonCamera : ClsCamera
    {
        private ClsTank _tank;

        public ClsCannonCamera(GraphicsDevice device, ClsTank tank) : base(device)
        {
            _tank = tank;
        }

        public override void Update(ClsTerrain terrain, GameTime gametime)
        {
        }
    }
}