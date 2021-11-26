using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrabalhoPratico_Monogame_2ano
{
    internal class ClsTerrain
    {
        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;
        private int _vertexCount;
        private int _indexCount;
        private BasicEffect _effect;
        private float _vScale;
        private Texture2D _textureImg;
        private VertexPositionNormalTexture[] vertices;

        public float[,] heights;
        public int w, h;
        public Vector3[,] normals;

        public ClsTerrain(GraphicsDevice device, Texture2D heightMap, Texture2D texture)
        {
            _textureImg = heightMap;
            _effect = new BasicEffect(device);

            //efeitos luz
            _effect.LightingEnabled = true;
            _effect.EmissiveColor = new Vector3(0.0f, 0.0f, 0.0f);
            _effect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);
            _effect.DirectionalLight0.Enabled = true;
            _effect.DirectionalLight0.DiffuseColor = new Vector3(0.0f, 0.0f, 0.0f);
            _effect.DirectionalLight0.SpecularColor = new Vector3(0.7f, 0.7f, 0.7f);
            _effect.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
            _effect.SpecularColor = new Vector3(0.0f, 0.0f, 0.0f);
            _effect.SpecularPower = 127;
            Vector3 lightDirection = new Vector3(1.0f, -1f, 1f);
            lightDirection.Normalize();
            _effect.DirectionalLight0.Direction = lightDirection;
            _effect.EnableDefaultLighting();
            _effect.TextureEnabled = true;
            _effect.Texture = texture;
            _effect.World = Matrix.Identity;
            CreateGeometry(device);
        }

        private void CreateGeometry(GraphicsDevice device)
        {
            w = _textureImg.Width;
            h = _textureImg.Height;
            Color[] texels = new Color[h * w];
            _textureImg.GetData<Color>(texels);
            _vertexCount = w * h;
            _vScale = 0.03f;
            heights = new float[w, h];
            normals = new Vector3[w, h];
            vertices = new VertexPositionNormalTexture[_vertexCount];

            for (int z = 0; z < h; z++)
            {
                for (int x = 0; x < h; x++)
                {
                    int pos = z * w + x;
                    float y = texels[pos].R * _vScale;
                    heights[x, z] = y;
                    normals[x, z] = Vector3.UnitY;
                    vertices[pos] = new VertexPositionNormalTexture(new Vector3(x, y, z), Vector3.UnitY, new Vector2(x % 2, z % 2));
                }
            }

            _indexCount = (w - 1) * h * 2;
            short[] indices = new short[_indexCount];
            CalculateNormal();

            for (int strip = 0; strip < w - 1; strip++)
            {
                for (int z = 0; z < w; z++)
                {
                    indices[strip * h * 2 + z * 2 + 0] = (short)(strip + z * w + 0);
                    indices[strip * h * 2 + z * 2 + 1] = (short)(strip + z * w + 1);
                }
            }
            _vertexBuffer = new VertexBuffer(device, typeof(VertexPositionNormalTexture), _vertexCount, BufferUsage.None);
            _vertexBuffer.SetData<VertexPositionNormalTexture>(vertices);

            _indexBuffer = new IndexBuffer(device, typeof(short), _indexCount, BufferUsage.None);
            _indexBuffer.SetData<short>(indices);
        }

        //calculo das normais
        public void CalculateNormal()
        {
            for (int z = 1; z < h - 1; z++)
            {
                for (int x = 1; x < w - 1; x++)
                {
                    int i;
                    i = z * w + x;

                    Vector3 p0 = vertices[i].Position;
                    Vector3 p1 = vertices[(z - 1) * w + x].Position;
                    Vector3 p2 = vertices[(z - 1) * w + (x - 1)].Position;
                    Vector3 p3 = vertices[z * w + (x - 1)].Position;
                    Vector3 p4 = vertices[(z + 1) * w + (x - 1)].Position;
                    Vector3 p5 = vertices[(z + 1) * w + x].Position;
                    Vector3 p6 = vertices[(z + 1) * w + (x + 1)].Position;
                    Vector3 p7 = vertices[z * w + (x + 1)].Position;
                    Vector3 p8 = vertices[(z - 1) * w + (x + 1)].Position;

                    Vector3 t1 = p1 - p0;
                    Vector3 t2 = p2 - p0;
                    Vector3 t3 = p3 - p0;
                    Vector3 t4 = p4 - p0;
                    Vector3 t5 = p5 - p0;
                    Vector3 t6 = p6 - p0;
                    Vector3 t7 = p7 - p0;
                    Vector3 t8 = p8 - p0;

                    Vector3 n12 = Vector3.Cross(t1, t2);
                    Vector3 n23 = Vector3.Cross(t2, t3);
                    Vector3 n34 = Vector3.Cross(t3, t4);
                    Vector3 n45 = Vector3.Cross(t4, t5);
                    Vector3 n56 = Vector3.Cross(t5, t6);
                    Vector3 n67 = Vector3.Cross(t6, t7);
                    Vector3 n78 = Vector3.Cross(t7, t8);
                    Vector3 n81 = Vector3.Cross(t8, t1);

                    n12.Normalize();
                    n23.Normalize();
                    n34.Normalize();
                    n45.Normalize();
                    n56.Normalize();
                    n67.Normalize();
                    n78.Normalize();
                    n81.Normalize();

                    Vector3 n0 = new Vector3();
                    n0 = (n12 + n23 + n34 + n45 + n56 + n67 + n78 + n81) / 8;
                    n0.Normalize();
                    normals[x, z] = n0;

                    vertices[i].Normal = n0;
                }
            }
        }

        //funcao para ir buscar o Y aproximado
        public float GetY(float x, float z)
        {
            int xA = (int)x;
            int zA = (int)z;
            int xB = xA + 1;
            int zB = zA;
            int xC = xA;
            int zC = zA + 1;
            int xD = xA + 1;
            int zD = zA + 1;

            float yA = heights[xA, zA];
            float yB = heights[xB, zB];
            float yC = heights[xC, zC];
            float yD = heights[xD, zD];

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

            Vector3 normalA = normals[xA, zA];
            Vector3 normalB = normals[xB, zB];
            Vector3 normalC = normals[xC, zC];
            Vector3 normalD = normals[xD, zD];

            float dA = x - xA;
            float dB = xB - x;
            float dAB = z - zA;
            float dCD = zC - z;

            Vector3 yAB = normalA * dB + normalB * dA;
            Vector3 yCD = normalC * dB + normalD * dA;

            Vector3 normal = yAB * dCD + yCD * dAB;

            return normal;
        }

        public void Draw(GraphicsDevice device, Matrix view, Matrix projection)
        {
            _effect.View = view;
            _effect.Projection = projection;
            _effect.CurrentTechnique.Passes[0].Apply();
            device.SetVertexBuffer(_vertexBuffer);
            device.Indices = _indexBuffer;
            w = _textureImg.Width;
            h = _textureImg.Height;
            int indexOffset = w + h;

            for (int strip = 0; strip < w - 1; strip++)
                device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, strip * indexOffset, (h - 1) * 2);
        }
    }
}