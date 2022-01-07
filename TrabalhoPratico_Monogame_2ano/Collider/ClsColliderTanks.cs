using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrabalhoPratico_Monogame_2ano.Collider
{
    class ClsColliderTanks
    {
        float raio;
        float x, y, z;
        float distancia;


        public ClsColliderTanks(float raio)
        {
            this.raio = raio;
        }

        public bool CollidedTank(Vector3 posicaoTank, Vector3 posicaoInimigo)
        {
            //calcular o ponto medio entre os dois tanks
            x = posicaoTank.X - posicaoInimigo.X;
            y = posicaoTank.Y - posicaoInimigo.Y;
            z = posicaoTank.Z - posicaoInimigo.Z;


            // calcular a distancia entre os dois tanks
            distancia = (float)Math.Sqrt(x * x + y * y + z * z);


            if (distancia <= raio * 2)
            {
                return false;
            }
            else
            {
                // significa que os tanks colidiram 
                return true;
            }
        }
    }
}
