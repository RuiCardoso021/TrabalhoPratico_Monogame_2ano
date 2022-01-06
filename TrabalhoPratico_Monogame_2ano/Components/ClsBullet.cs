using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrabalhoPratico_Monogame_2ano.Components
{
    class ClsBullet
    {
        float gravidade = 7.5f;
        float scale = 0.2f;

        Model bala;
        Vector3 velocidade;
        public Vector3 posicao, posicaoAntiga;

        Matrix world;

        public ClsBullet(Model bala, Vector3 posicaoTanque, Vector3 direcaoTanque)
        {
            this.bala = bala;

            this.posicao = posicaoTanque;
            velocidade = direcaoTanque;
            velocidade.Normalize();
            velocidade *= 20f;
        }

        public void Update(GameTime gameTime)
        {
            posicaoAntiga = posicao;

            velocidade += Vector3.Down * gravidade * (float)gameTime.ElapsedGameTime.TotalSeconds;
            posicao += velocidade * (float)gameTime.ElapsedGameTime.TotalSeconds;

            Matrix escala = Matrix.CreateScale(scale);
            Matrix translacao = Matrix.CreateTranslation(posicao);

            world = escala * translacao;
        }

        public void Draw(GraphicsDevice device)
        {
            foreach (ModelMesh mesh in bala.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = ClsCamera.Instance.view;
                    effect.Projection = ClsCamera.Instance.projection;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }
    }
}
