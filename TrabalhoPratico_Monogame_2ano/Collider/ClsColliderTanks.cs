using Microsoft.Xna.Framework;
using System;

namespace TrabalhoPratico_Monogame_2ano.Collider
{
    internal class ClsColliderTanks
    {
        private float _radius;
        private float _x, _y, _z;
        private float _distance;

        public ClsColliderTanks(float radius)
        {
            _radius = radius;
        }

        public bool Collide(Vector3 position, Vector3 enimyPosition)
        {
            //calcular o ponto medio entre os dois tanks
            _x = position.X - enimyPosition.X;
            _y = position.Y - enimyPosition.Y;
            _z = position.Z - enimyPosition.Z;

            // calcular a distancia entre os dois tanks
            _distance = (float)Math.Sqrt(_x * _x + _y * _y + _z * _z);

            if (_distance <= _radius * 2)
                return false;
            else
                return true;
        }
    }
}