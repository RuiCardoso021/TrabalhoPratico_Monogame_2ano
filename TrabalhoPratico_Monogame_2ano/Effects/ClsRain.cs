using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrabalhoPratico_Monogame_2ano.Effects
{
    class ClsRain
    {
        private List<ClsParticleRain> _particulas;
        private Random _random;
        private BasicEffect _effect;
        private GraphicsDevice _device;
        private float _altura;
        private float _raio;
        private int _particulas_por_segundo;


        public ClsRain(GraphicsDevice device)
        {
            _random = new Random();
            _particulas = new List<ClsParticleRain>();
            _device = device;
            _altura = 100;
            _raio = 230;
            _particulas_por_segundo = 20;

            _effect = new BasicEffect(device);
            _effect.VertexColorEnabled = true;
        }


        private ClsParticleRain Generate()
        {
            //generate position
            float angle = (float)_random.NextDouble() * 2 * MathF.PI;
            float d = (float)_random.NextDouble() * _raio;
            Vector3 pos = new Vector3(d * MathF.Cos(angle), _altura, d * MathF.Sin(angle));

            //random number * normal to calculate vel
            Vector3 vel = (float)_random.NextDouble() * Vector3.Down;
            vel.X = (float)_random.NextDouble();
            vel.Z = (float)_random.NextDouble();

            return new ClsParticleRain(vel, pos);
        }


        public void Update(GameTime gameTime, ClsTerrain terreno)
        {

            int particulas_a_gerar = (int)(Math.Round(_particulas_por_segundo * (float)gameTime.ElapsedGameTime.TotalMilliseconds));

            //create particula
            for (int i = 0; i < particulas_a_gerar; i++)
            {
                ClsParticleRain gerada = Generate();
                if (terreno.TerrainLimit(gerada.pos.X, gerada.pos.Z))
                    _particulas.Add(gerada);
            }

            //remove particula if position y = 0
            for (int i = _particulas.Count - 1; i >= 0; i--)
                if (_particulas[i].pos.Y < 0 || !(terreno.TerrainLimit(_particulas[i].pos.X, _particulas[i].pos.Z)))
                    _particulas.RemoveAt(i);

            //update to particulas
            foreach (ClsParticleRain particula in _particulas)
                particula.Update(gameTime);

        }

        public void Draw(Matrix view, Matrix projection)
        {
            _effect.View = view;
            _effect.Projection = projection;

            VertexPositionColor[] vertices = new VertexPositionColor[2 * _particulas.Count];

            float size = 0.5f;

            for (int i = 0; i < _particulas.Count; i++)
            {
                vertices[2 * i] = new VertexPositionColor(_particulas[i].pos, Color.White);
                vertices[2 * i + 1] = new VertexPositionColor(_particulas[i].pos + Vector3.Normalize(_particulas[i].vel) * size, Color.White);
            }
            _effect.CurrentTechnique.Passes[0].Apply();

            _device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, _particulas.Count);
        }
    }
}
