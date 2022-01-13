using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrabalhoPratico_Monogame_2ano.Components
{
    public class ClsTerrain
    {
        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;
        private int _vertexCount;
        private int _indexCount;
        private BasicEffect _effect;
        private float _vScale;
        private Texture2D _textureImg;
        private VertexPositionNormalTexture[] _vertices;

        public float[,] Heights;
        public int W, H;
        public Vector3[,] Normals;

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

        //limite terreno
        public bool TerrainLimit(float x, float z)
        {
            if (x >= -0 && x < W - 2 && z >= 0 && z < H)
                return true;

            return false;
        }

        private void CreateGeometry(GraphicsDevice device)
        {
            W = _textureImg.Width;
            H = _textureImg.Height;
            Color[] texels = new Color[H * W];
            _textureImg.GetData<Color>(texels);
            _vertexCount = W * H;
            _vScale = 0.03f;
            Heights = new float[W, H];
            Normals = new Vector3[W, H];
            _vertices = new VertexPositionNormalTexture[_vertexCount];

            for (int z = 0; z < H; z++)
            {
                for (int x = 0; x < H; x++)
                {
                    int pos = z * W + x;
                    float y = texels[pos].R * _vScale;
                    Heights[x, z] = y;
                    Normals[x, z] = Vector3.UnitY;
                    _vertices[pos] = new VertexPositionNormalTexture(new Vector3(x, y, z), Vector3.UnitY, new Vector2(x % 2, z % 2));
                }
            }

            _indexCount = (W - 1) * H * 2;
            short[] indices = new short[_indexCount];
            CalculateNormal();

            for (int strip = 0; strip < W - 1; strip++)
            {
                for (int z = 0; z < W; z++)
                {
                    indices[strip * H * 2 + z * 2 + 0] = (short)(strip + z * W + 0);
                    indices[strip * H * 2 + z * 2 + 1] = (short)(strip + z * W + 1);
                }
            }
            _vertexBuffer = new VertexBuffer(device, typeof(VertexPositionNormalTexture), _vertexCount, BufferUsage.None);
            _vertexBuffer.SetData<VertexPositionNormalTexture>(_vertices);

            _indexBuffer = new IndexBuffer(device, typeof(short), _indexCount, BufferUsage.None);
            _indexBuffer.SetData<short>(indices);
        }

        //calculo das normais
        public void CalculateNormal()
        {
            for (int z = 1; z < H - 1; z++)
            {
                for (int x = 1; x < W - 1; x++)
                {
                    int i;
                    i = z * W + x;

                    Vector3 p0 = _vertices[i].Position;
                    Vector3 p1 = _vertices[(z - 1) * W + x].Position;
                    Vector3 p2 = _vertices[(z - 1) * W + (x - 1)].Position;
                    Vector3 p3 = _vertices[z * W + (x - 1)].Position;
                    Vector3 p4 = _vertices[(z + 1) * W + (x - 1)].Position;
                    Vector3 p5 = _vertices[(z + 1) * W + x].Position;
                    Vector3 p6 = _vertices[(z + 1) * W + (x + 1)].Position;
                    Vector3 p7 = _vertices[z * W + (x + 1)].Position;
                    Vector3 p8 = _vertices[(z - 1) * W + (x + 1)].Position;

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
                    Normals[x, z] = n0;

                    _vertices[i].Normal = n0;
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

            float yA = Heights[xA, zA];
            float yB = Heights[xB, zB];
            float yC = Heights[xC, zC];
            float yD = Heights[xD, zD];

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

            Vector3 normalA = Normals[xA, zA];
            Vector3 normalB = Normals[xB, zB];
            Vector3 normalC = Normals[xC, zC];
            Vector3 normalD = Normals[xD, zD];

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
            W = _textureImg.Width;
            H = _textureImg.Height;
            int indexOffset = W + H;

            for (int strip = 0; strip < W - 1; strip++)
                device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, strip * indexOffset, (H - 1) * 2);
        }
    }
}