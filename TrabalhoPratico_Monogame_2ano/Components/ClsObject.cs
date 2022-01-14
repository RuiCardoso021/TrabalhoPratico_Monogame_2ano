using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TrabalhoPratico_Monogame_2ano.Components
{
    internal class ClsObject
    {
        private ClsTerrain _terrain;
        private Model _texture;
        private Vector3 _position;
        private Matrix _world;

        public ClsObject(Model texture, ClsTerrain terrain)
        {
            Random r = new Random();
            _terrain = terrain;
            _texture = texture;
            _position = new Vector3(r.Next(2, 120), 0, r.Next(2, 120));
            _position.Y = _terrain.GetY(_position.X, _position.Z);

            Matrix scale = Matrix.CreateScale(0.008f);
            Matrix translation = Matrix.CreateTranslation(_position);

            _position = _terrain.GetNormal(_position.X, _position.Z);
            _position.Y = _terrain.GetY(_position.X, _position.Z);
            _world = scale * translation;
        }

        public void Draw()
        {
            foreach (ModelMesh mesh in _texture.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = _world;
                    effect.View = ClsCamera.Instance.View;
                    effect.Projection = ClsCamera.Instance.Projection;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }
    }
}