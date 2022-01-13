using Microsoft.Xna.Framework;
using System;

namespace TrabalhoPratico_Monogame_2ano.Collider
{
    internal class ClsColliderBullet
    {
        private float _radius;

        public ClsColliderBullet(float radius)
        {
            _radius = radius;
        }

        public bool Collide(Vector3 posA, Vector3 posB, Vector3 posC)
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

                if (d <= _radius)
                    return true;
            }
            return false;
        }
    }
}