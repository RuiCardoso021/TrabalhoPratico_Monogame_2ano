using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrabalhoPratico_Monogame_2ano.Collider
{
    class ClsColliderBullet
    {
        float raio;

        public ClsColliderBullet(float raio)
        {
            this.raio = raio;
        }

        //formula de heron
        public bool CollidedTank(Vector3 posA, Vector3 posB, Vector3 posC)
        {
            Vector3 disA = posC - posB;
            Vector3 disB = posC - posA;
            Vector3 disC = posA - posB;


            float a = disA.Length();
            float b = disB.Length();
            float c = disC.Length();

            disA.Normalize();
            disB.Normalize();
            disC.Normalize();

            float AC = Vector3.Dot(disA, disC);
            float BC = Vector3.Dot(disB, disC);


            if (AC > 0 && BC <= 0)
            {
                float sp = (a + b + c) / 2;

                float area = (float)Math.Sqrt(sp * (sp - a) * (sp - b) * (sp - c));

                float d = 2 * area / c;

                if (d <= raio)
                {
                    return true;

                }
            }
            return false;
        }
    }
}
