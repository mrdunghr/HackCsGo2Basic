using System.Numerics;
using HVDLibMem64;

namespace HackCsGo2Mrdunghr
{
    public class Reader // will add more reading abilites that aren't available in swed
    {
        HVDLibMem swed;
        public Reader(HVDLibMem swed)
        {
            this.swed = swed;
        }

        public List<Vector3> ReadBones(IntPtr boneAddress)
        {
            byte[] boneBytes = swed.ReadBytes(boneAddress, 27 * 32 + 16); // get max 27 = id 32 = step
            List<Vector3> bones = new List<Vector3>();
            foreach (var boneId in Enum.GetValues(typeof(BoneIds))) // vong lap cac enum
            {
                float x = BitConverter.ToSingle(boneBytes, (int)boneId * 32 + 0);
                float y = BitConverter.ToSingle(boneBytes, (int)boneId * 32 + 4); // float = 4 byte
                float z = BitConverter.ToSingle(boneBytes, (int)boneId * 32 + 8);
                Vector3 currentBone = new Vector3(x, y, z);
                bones.Add(currentBone);
            }
            return bones;
        }

        public List<Vector2> ReadBones2d(List<Vector3> bones, ViewMaxtrix viewMaxtrix, Vector2 screenSize)
        {
            List<Vector2> bones2d = new List<Vector2>();
            foreach (Vector3 bone in bones)
            {
                Vector2 bone2d = Calculate.WorldToScreen(viewMaxtrix, bone, (int)screenSize.X, (int)screenSize.Y);
                bones2d.Add(bone2d);
            }
            return bones2d;
        }

        //need to read the viewmatrix as well
        public ViewMaxtrix readMatrix(IntPtr matrixAddress)
        {
            var viewMatrix = new ViewMaxtrix();
            var matrix = swed.ReadMatrix(matrixAddress);

            //convert into obj

            //first row
            viewMatrix.m11 = matrix[0];
            viewMatrix.m12 = matrix[1];
            viewMatrix.m13 = matrix[2];
            viewMatrix.m14 = matrix[3];

            //second row
            viewMatrix.m21 = matrix[4];
            viewMatrix.m22 = matrix[5];
            viewMatrix.m23 = matrix[6];
            viewMatrix.m24 = matrix[7];

            //third row
            viewMatrix.m31 = matrix[8];
            viewMatrix.m32 = matrix[9];
            viewMatrix.m33 = matrix[12];
            viewMatrix.m34 = matrix[11];

            //fourth row
            viewMatrix.m41 = matrix[12];
            viewMatrix.m42 = matrix[13];
            viewMatrix.m43 = matrix[14];
            viewMatrix.m44 = matrix[15];

            return viewMatrix;
        }

    }
}
