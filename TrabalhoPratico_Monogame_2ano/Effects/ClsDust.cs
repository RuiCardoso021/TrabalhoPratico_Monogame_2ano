using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrabalhoPratico_Monogame_2ano.Effects
{
    class ClsDust
    {

        BasicEffect effect;
        Random random;

        public List<ClsParticleDust> listaParticulasFumo;

        float diametro;

        public ClsDust(GraphicsDevice device)
        {

            effect = new BasicEffect(device);

            diametro = 1f;
            random = new Random();
            listaParticulasFumo = new List<ClsParticleDust>();

            effect.LightingEnabled = false;
            effect.VertexColorEnabled = true;
        }

        public void Update(Vector3 posicaoRoda, GameTime gameTime, Vector3 gravidade, ClsTerrain terrain)
        {
            Vector3 posicaoTemp = new Vector3(posicaoRoda.X + (diametro * (float)random.NextDouble() - (diametro / 2)), posicaoRoda.Y, posicaoRoda.Z + (diametro * (float)random.NextDouble() - (diametro / 2)));
            ClsParticleDust particula = new ClsParticleDust(posicaoTemp, new Vector3(diametro * (float)random.NextDouble() - (diametro / 2), 10f, diametro * (float)random.NextDouble() - (diametro / 2)));
            listaParticulasFumo.Add(particula);

            foreach (ClsParticleDust particulas in listaParticulasFumo)
            {
                particulas.Update(gameTime, gravidade);
            }

            foreach (ClsParticleDust particulas in listaParticulasFumo.ToArray())
            {
                //if (particulas.position.Y <= terrain.GetY(particulas.position.X, particulas.position.Z))
                if (particulas.position.Y <= 0)
                    listaParticulasFumo.Remove(particulas);
            }
        }

        public void Draw(GraphicsDevice graphicsDevice)
        {
            if (listaParticulasFumo.Count > 0)
            {
                effect.View = ClsCamera.Instance.view;
                effect.Projection = ClsCamera.Instance.projection; ;
                effect.World = Matrix.Identity;

                VertexPositionColor[] particulasVertices;
                particulasVertices = new VertexPositionColor[2 * listaParticulasFumo.Count];

                for (int i = 0; i < listaParticulasFumo.Count; i++)
                {
                    particulasVertices[2 * i] = new VertexPositionColor(listaParticulasFumo[i].position, Color.Black);
                    particulasVertices[2 * i + 1] = new VertexPositionColor(listaParticulasFumo[i].position - new Vector3(0f, 0.1f, 0f), Color.Black);
                }

                effect.CurrentTechnique.Passes[0].Apply();
                graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, particulasVertices, 0, listaParticulasFumo.Count);
            }
        }
    }
}
