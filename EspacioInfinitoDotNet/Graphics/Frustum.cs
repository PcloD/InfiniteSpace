using System;
using System.Collections.Generic;
using System.Text;
using EspacioInfinitoDotNet.Maths;

namespace EspacioInfinitoDotNet.Graphics
{
    public class Frustum
    {
        private float[][] frustum;

        public Frustum(float[][] frustum)
        {
            this.frustum = frustum;
        }

        public bool PointInside(Vector2 point)
        {
            float x = point.X;
            float y = point.Y;
            float z = GraphicEngine.zValue;

            for (int p = 0; p < 6; p++)
                if (frustum[p][0] * x + frustum[p][1] * y + frustum[p][2] * z + frustum[p][3] <= 0)
                    return false;

            return true;
        }

        public bool CircleInside(Vector2 center, float radius)
        {
            float x = center.X;
            float y = center.Y;
            float z = GraphicEngine.zValue;

            for (int p = 0; p < 6; p++)
                if (frustum[p][0] * x + frustum[p][1] * y + frustum[p][2] * z + frustum[p][3] <= -radius)
                    return false;

            return true;
        }

        public bool RectangleInside(Vector2 center, Vector2 size)
        {
            float x = center.X;
            float y = center.Y;
            float z = GraphicEngine.zValue;
            float sizeX = size.X;
            float sizeY = size.Y;
            float sizeZ = GraphicEngine.zValue * 2.0f;

            for (int i = 0; i < 6; i++)
            {
                //No necesito evaluar por Z, asi que lo elimino del chequeo!!
                if (frustum[i][0] * (x - sizeX) + frustum[i][1] * (y - sizeY) + frustum[i][2] * (z - sizeZ) + frustum[i][3] > 0)
                    continue;
                if (frustum[i][0] * (x + sizeX) + frustum[i][1] * (y - sizeY) + frustum[i][2] * (z - sizeZ) + frustum[i][3] > 0)
                    continue;
                if (frustum[i][0] * (x - sizeX) + frustum[i][1] * (y + sizeY) + frustum[i][2] * (z - sizeZ) + frustum[i][3] > 0)
                    continue;
                if (frustum[i][0] * (x + sizeX) + frustum[i][1] * (y + sizeY) + frustum[i][2] * (z - sizeZ) + frustum[i][3] > 0)
                    continue;
                
                if (frustum[i][0] * (x - sizeX) + frustum[i][1] * (y - sizeY) + frustum[i][2] * (z + sizeZ) + frustum[i][3] > 0)
                    continue;
                if (frustum[i][0] * (x + sizeX) + frustum[i][1] * (y - sizeY) + frustum[i][2] * (z + sizeZ) + frustum[i][3] > 0)
                    continue;
                if (frustum[i][0] * (x - sizeX) + frustum[i][1] * (y + sizeY) + frustum[i][2] * (z + sizeZ) + frustum[i][3] > 0)
                    continue;
                if (frustum[i][0] * (x + sizeX) + frustum[i][1] * (y + sizeY) + frustum[i][2] * (z + sizeZ) + frustum[i][3] > 0)
                    continue;
                
                return false;
            }

            return true;
        }
    }
}
