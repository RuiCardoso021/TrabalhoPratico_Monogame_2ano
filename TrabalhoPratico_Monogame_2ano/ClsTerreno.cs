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
        public Vector3[,] normais;

        public ClsTerreno(GraphicsDevice device, Texture2D heightMap, Texture2D texture){
            _textureImg = heightMap;
            _effect = new BasicEffect(device);

            //efeitos luz
            _effect.EmissiveColor = new Vector3(0.0f, 0.0f, 0.0f);
            _effect.AmbientLightColor = new Vector3(0.5f, 0.5f, 0.5f);
            _effect.DirectionalLight0.Enabled = true;
            _effect.DirectionalLight0.DiffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
            _effect.DirectionalLight0.SpecularColor = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 lightDirection = new Vector3(-1.0f, 1.0f, -1.0f);
            lightDirection.Normalize();
            _effect.DirectionalLight0.Direction = lightDirection;


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
            normais = new Vector3[w, h];
            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[_vertexCount];


            for (int z = 0; z < h; z++)
            {
                for (int x = 0; x < h; x++)
                {
                    int pos = z * w + x;
                    float y = texels[pos].R * _vScale;
                    alturas[x, z] = y;
                    normais[x, z] = new Vector3(x, y, z);
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

        public Vector3 GetNormal(float x, float z)
        {
            int xA = (int)x;
            int zA = (int)z;
            int xB = xA + 1;
            int zB = zA;
            int xC = xA;
            int zC = zA + 1;
            int xD = xA + 1;
            int zD = zA + 1;

            Vector3 normalA = normais[xA, zA];
            Vector3 normalB = normais[xB, zB];
            Vector3 normalC = normais[xC, zC];
            Vector3 normalD = normais[xD, zD];

            float dA = x - xA;
            float dB = xB - x;
            float dAB = z - zA;
            float dCD = zC - z;

            Vector3 yAB = normalA * dB + normalB * dA;
            Vector3 yCD = normalC * dB + normalD * dA;

            Vector3 normal = yAB * dCD + yCD * dAB;

            return normal;
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
