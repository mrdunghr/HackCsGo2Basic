using System.Numerics;

namespace HackCsGo2Mrdunghr
{
    static class Calculate
    {
        public static Vector2 CalculateAngles(Vector3 from, Vector3 to)
        {
            float yaw;
            float pitch;

            //calculate yaw
            float deltaX = to.X - from.X;
            float deltaY = to.Y - from.Y;
            yaw = (float)(Math.Atan2(deltaY, deltaX) * 180 / Math.PI); //conveter to degree = * 180 / PI

            //calculate pitch
            float deltaZ = to.Z - from.Z;
            double distance = Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
            pitch = -(float)(Math.Atan2(deltaZ, distance) * 180 / Math.PI);

            return new Vector2(yaw, pitch);
        }

        public static Vector2 WorldToScreen(ViewMaxtrix maxtrix, Vector3 pos, int width, int height)
        {
            Vector2 screenCoordinates = new Vector2();

            //get screen world
            float screenW = (maxtrix.m41 * pos.X) + (maxtrix.m42 * pos.Y) + (maxtrix.m43 * pos.Z) + maxtrix.m44;

            if (screenW > 0.001f)
            {
                // tính toán màn hình x và y
                float screenX = (maxtrix.m11 * pos.X) + (maxtrix.m12 * pos.Y) + (maxtrix.m13 * pos.Z) + maxtrix.m14;
                float screenY = (maxtrix.m21 * pos.X) + (maxtrix.m22 * pos.Y) + (maxtrix.m23 * pos.Z) + maxtrix.m24;

                // trung tâm camera
                float camX = width / 2;
                float camY = height / 2;

                //thực hiện phép tính chia phối cảnh
                float X = camX + (camX * screenX / screenW);
                float Y = camY - (camY * screenY / screenW);

                // return coords
                screenCoordinates.X = X;
                screenCoordinates.Y = Y;

                return screenCoordinates;
            } // if out of range
            else
            {
                return new Vector2(-99, -99);
            }
        }
    }
}
