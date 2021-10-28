using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TrabalhoPratico_Monogame_2ano
{
    class ClsTerreno
    {
        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;
        private int _vertexCount;
        private int _indexCount;
        private BasicEffect _effect;
        private float _vScale;
        private Texture2D _textureImg;

        public float[,] alturas;
        public int w, h;

        public ClsTerreno(GraphicsDevice device, Texture2D heightMap, Texture2D texture){
            _textureImg = heightMap;
            _effect = new BasicEffect(device);
            _effect.LightingEnabled = false;
            _effect.VertexColorEnabled = false;
            _effect.TextureEnabled = true;
            _effect.Texture = texture;
            _effect.World = Matrix.Identity;
            CreateGeometry(device);
        }

        private void CreateGeometry(GraphicsDevice device){
            
            w = _textureImg.Width;
            h = _textureImg.Height;
            Color[] texels = new Color[h * w];
            _textureImg.GetData<Color>(texels);
            _vertexCount = w * h;
            _vScale = 0.03f;
            alturas = new float[w, h];
            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[_vertexCount];


            for (int z = 0; z < h; z++)
            {
                for (int x = 0; x < h; x++)
                {
                    int pos = z * w + x;
                    float y = texels[pos].R * _vScale;
                    alturas[x, z] = y;
                    vertices[pos] = new VertexPositionNormalTexture(new Vector3(x, y, z), Vector3.UnitY, new Vector2(x % 2, z % 2));
                }

            }

            _vertexBuffer = new VertexBuffer(device, typeof(VertexPositionNormalTexture), _vertexCount, BufferUsage.None);
            _vertexBuffer.SetData<VertexPositionNormalTexture>(vertices);

            _indexCount = (w - 1) * h * 2;
            short[] indices = new short[_indexCount];

            for (int strip = 0; strip < w - 1; strip++)
            {
                for (int z = 0; z < w; z++)
                {
                    indices[strip * h * 2 + z * 2 + 0] = (short)(strip + z * w + 0);
                    indices[strip * h * 2 + z * 2 + 1] = (short)(strip + z * w + 1);
                }
            }

            _indexBuffer = new IndexBuffer(device, typeof(short), _indexCount, BufferUsage.None);
            _indexBuffer.SetData<short>(indices);
        }

        //funcao para ir buscar o Y aproximado
        public float GetY(float x, float z) {
            int xA = (int)x;
            int zA = (int)z;
            int xB = xA + 1;
            int zB = zA;
            int xC = xA;
            int zC = zA + 1;
            int xD = xA + 1;
            int zD = zA + 1;

            float yA = alturas[xA, zA];
            float yB = alturas[xB, zB];
            float yC = alturas[xC, zC];
            float yD = alturas[xD, zD];

            float dA = x - xA;
            float dB = xB - x;
            float dAB = z - zA;
            float dCD = zC - z;

            float yAB = yA * dB + yB * dA;
            float yCD = yC * dB + yD * dA;

            float y = yAB * dCD + yCD * dAB;

            return y;
        }

        public void Draw(GraphicsDevice device, Matrix view, Matrix projection) {
            _effect.View = view;
            _effect.Projection = projection;
            _effect.CurrentTechnique.Passes[0].Apply();
            device.SetVertexBuffer(_vertexBuffer);
            device.Indices = _indexBuffer;
            w = _textureImg.Width;
            h = _textureImg.Height;
            int indexOffset = w+h;

            for (int strip = 0; strip < w - 1; strip++)
                device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, strip * indexOffset, (h - 1) * 2);
            

        }
    }
}
