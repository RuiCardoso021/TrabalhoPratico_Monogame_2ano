using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TrabalhoPratico_Monogame_2ano.Componentes
{
    class ClsTank
    {
        private float speed = 5f;
        private Model _tankModel;
        private ModelBone _torreBone,
            _canhaoBone,
            _leftBackWheelBone,
            _rightBackWheelBone,
            _leftFrontWheelBone,
            _rightFrontWheelBone,
            _leftSteerBone,
            _rightSteerBone,
            _hatchBone;
        private Matrix _turretTransform,
            _cannonTransform,
            _scale,
            _leftSteerDefaultTransform,
            _rightSteerDefaultTransform,
            _leftBackWheelBoneTransform,
            _rightBackWheelBoneTransform,
            _leftFrontWheelBoneTransform,
            _rightFrontWheelBoneTransform;
        private Matrix[] _boneTransforms;
        public Vector3 _pos;
        private float _yaw, _yaw_canhao, _yaw_torre, _yaw_wheel;
        public Vector3 direcaoCorrigida;
        public Vector3 normal;


        public ClsTank(GraphicsDevice device, Model modelo, Vector3 position)
        {
            _pos = position;
            _tankModel = modelo;

            _leftBackWheelBone = _tankModel.Bones["l_back_wheel_geo"];
            _rightBackWheelBone = _tankModel.Bones["r_back_wheel_geo"];
            _leftFrontWheelBone = _tankModel.Bones["l_front_wheel_geo"];
            _rightFrontWheelBone = _tankModel.Bones["r_front_wheel_geo"];
            _leftSteerBone = _tankModel.Bones["l_steer_geo"];
            _rightSteerBone = _tankModel.Bones["r_steer_geo"];
            _torreBone = _tankModel.Bones["turret_geo"];
            _canhaoBone = _tankModel.Bones["canon_geo"];
            _hatchBone = _tankModel.Bones["hatch_geo"];

            // Read bone default transforms
            _leftBackWheelBoneTransform     = _leftBackWheelBone.Transform;
            _rightBackWheelBoneTransform    = _rightBackWheelBone.Transform;
            _leftFrontWheelBoneTransform    = _leftFrontWheelBone.Transform;
            _rightFrontWheelBoneTransform   = _rightFrontWheelBone.Transform;
            _leftSteerDefaultTransform      = _leftSteerBone.Transform;
            _rightSteerDefaultTransform     = _rightSteerBone.Transform;
            _turretTransform                = _torreBone.Transform;
            _cannonTransform                = _canhaoBone.Transform;
            

            // create array to store final bone transforms
            _boneTransforms = new Matrix[_tankModel.Bones.Count];

            _scale = Matrix.CreateScale(0.01f);
        }

        public void Update(GameTime gameTime, ClsTerreno terreno)
        {
            KeyboardState kb = Keyboard.GetState();
            Matrix steerRotation = Matrix.Identity;


            if (kb.IsKeyDown(Keys.LeftShift)) speed = 15f;
            else speed = 5f;

            if (kb.IsKeyDown(Keys.A)) //esquerda
                _yaw = _yaw + MathHelper.ToRadians(speed);
            if (kb.IsKeyDown(Keys.D)) //direita
                _yaw = _yaw - MathHelper.ToRadians(speed);


            Matrix rotacao = Matrix.CreateFromYawPitchRoll(_yaw, 0f, 0f);
            Vector3 direction = Vector3.Transform(-Vector3.UnitZ, rotacao);


            //movimento tank
            if (kb.IsKeyDown(Keys.W)){ //esquerda
                this._pos = this._pos + direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                _yaw_wheel = _yaw_wheel + MathHelper.ToRadians(speed);
            }
            if (kb.IsKeyDown(Keys.S)){ //direita
                this._pos = this._pos - direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                _yaw_wheel = _yaw_wheel - MathHelper.ToRadians(speed);
            }

            //lemitar tank no terreno
            if (this._pos.X >= 0 && this._pos.X < terreno.w - 1 && this._pos.Z >= 0 && this._pos.Z < terreno.h - 1){ 
                this._pos.Y = terreno.GetY(this._pos.X, this._pos.Z);
                normal = terreno.GetNormal(this._pos.X, this._pos.Z);
            }else
            {
                this._pos.Y = 0.0f;
                normal = Vector3.Up;
            }

            //movimento da torre
            if (kb.IsKeyDown(Keys.Left)) //up
                _yaw_torre = _yaw_torre + MathHelper.ToRadians(speed);
            if (kb.IsKeyDown(Keys.Right)) //down
                _yaw_torre = _yaw_torre - MathHelper.ToRadians(speed);
            //movimento do canhao
            if (kb.IsKeyDown(Keys.Up)) //esquerda
                _yaw_canhao = _yaw_canhao + MathHelper.ToRadians(speed);
            if (kb.IsKeyDown(Keys.Down)) //direita
                _yaw_canhao = _yaw_canhao - MathHelper.ToRadians(speed);


            Vector3 right = Vector3.Cross(direction, normal);
            direcaoCorrigida = Vector3.Cross(normal, right);

            normal.Normalize();
            direcaoCorrigida.Normalize();
            right.Normalize();

            rotacao.Up = normal;
            rotacao.Forward = direcaoCorrigida;
            rotacao.Right = right;

            Matrix translacao = Matrix.CreateTranslation(this._pos);

            //lemitar rotacao canhao
            if (_yaw_canhao > MathHelper.ToRadians(40.0f))
                _yaw_canhao = MathHelper.ToRadians(40.0f);
            if (_yaw_canhao < -MathHelper.ToRadians(40.0f))
                _yaw_canhao = -MathHelper.ToRadians(40.0f);


            //aplicar transformaçoes
            _tankModel.Root.Transform = _scale * Matrix.CreateRotationY(MathHelper.Pi) * rotacao * translacao;
            _torreBone.Transform = Matrix.CreateRotationY(MathHelper.ToRadians(45f*_yaw_torre)) * _turretTransform;
            _canhaoBone.Transform = Matrix.CreateRotationX(MathHelper.ToRadians(-45f*_yaw_canhao)) * _cannonTransform;
            _leftBackWheelBone.Transform = Matrix.CreateRotationX(MathHelper.ToRadians(45f * _yaw_wheel)) * _leftBackWheelBoneTransform;
            _rightBackWheelBone.Transform = Matrix.CreateRotationX(MathHelper.ToRadians(45f * _yaw_wheel)) * _rightBackWheelBoneTransform;
            _leftFrontWheelBone.Transform = Matrix.CreateRotationX(MathHelper.ToRadians(45f * _yaw_wheel)) * _leftFrontWheelBoneTransform;
            _rightFrontWheelBone.Transform = Matrix.CreateRotationX(MathHelper.ToRadians(45f * _yaw_wheel)) * _rightFrontWheelBoneTransform;
            _leftSteerBone.Transform = steerRotation * _leftSteerDefaultTransform;
            _rightSteerBone.Transform = steerRotation * _rightSteerDefaultTransform;

            // Appies transforms to bones in a cascade
            _tankModel.CopyAbsoluteBoneTransformsTo(_boneTransforms);
        }

            public void Draw(GraphicsDevice device, Matrix view, Matrix projection)
            {
            foreach (ModelMesh mesh in _tankModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.LightingEnabled = true; // turn on the lighting subsystem.
                    effect.EmissiveColor = new Vector3(0.0f, 0.0f, 0.0f);
                    effect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);
                    effect.DirectionalLight0.Enabled = true;
                    effect.DirectionalLight0.DiffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
                    effect.DirectionalLight0.SpecularColor = new Vector3(1.0f, 1.0f, 1.0f);
                    effect.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
                    effect.SpecularColor = new Vector3(0.0f, 0.0f, 0.0f);
                    effect.SpecularPower = 127;
                    Vector3 lightDirection = new Vector3(1.0f, -1f, 1f);
                    lightDirection.Normalize();
                    effect.DirectionalLight0.Direction = lightDirection;
                    effect.EnableDefaultLighting();
                    effect.World = _boneTransforms[mesh.ParentBone.Index];
                    effect.View = view;
                    effect.Projection = projection;
                    effect.EnableDefaultLighting();
                }
                // Draw each mesh of the model
                mesh.Draw();
            }
        }
    }
}
