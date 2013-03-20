using System;
using System.Collections.Generic;
using System.Text;
using EspacioInfinitoDotNet.Maths;

namespace EspacioInfinitoDotNet.Things
{
    public class BoundingRectangle : BoundingObject
    {
        /** Corners of the rotated box, where 0 is the lower left. */
        Vector2[] corner = new Vector2[4];

        /** Two edges of the box extended away from corner[0]. */
        Vector2[] axis = new Vector2[2];

        /** origin[a] = corner[0].dot(axis[a]); */
        float[] origin = new float[2];

        float anguloOriginal;
        Vector2 centroOriginal;
        Vector2 tamanioOriginal;

        public Vector2[] Puntos
        {
            get { return corner; }
        }

        public float RotacionEnGrados
        {
            get { return anguloOriginal; }
        }

        public Vector2 Center
        {
            get { return (corner[0] + corner[1] + corner[2] + corner[3]) * 0.25f; }
        }

        /** Returns true if other overlaps one dimension of this. */
        bool overlaps1Way(BoundingRectangle other)
        {
            for (int a = 0; a < 2; ++a)
            {
                float t = other.corner[0].Dot(axis[a]);

                // Find the extent of box 2 on axis a
                float tMin = t;
                float tMax = t;

                for (int c = 1; c < 4; ++c)
                {
                    t = other.corner[c].Dot(axis[a]);

                    if (t < tMin)
                        tMin = t;
                    else if (t > tMax)
                        tMax = t;
                }

                // We have to subtract off the origin

                // See if [tMin, tMax] intersects [0, 1]
                if ((tMin > 1 + origin[a]) || (tMax < origin[a]))
                {
                    // There was no intersection along this dimension;
                    // the boxes cannot possibly overlap.
                    return false;
                }
            }

            // There was no dimension along which there is no intersection.
            // Therefore the boxes overlap.
            return true;
        }


        /** Updates the axes after the corners move.  Assumes the
            corners actually form a rectangle. */
        void computeAxes()
        {
            axis[0] = corner[1] - corner[0];
            axis[1] = corner[3] - corner[0];

            // Make the length of each axis 1/edge length so we know any
            // dot product must be less than 1 to fall within the edge.

            for (int a = 0; a < 2; ++a)
            {
                axis[a] /= axis[a].LengthSqr;
                origin[a] = corner[0].Dot(axis[a]);
            }
        }

        public BoundingRectangle(Vector2 center, Vector2 tamanio, float angleInDegrees)
        {
            centroOriginal = center;
            tamanioOriginal = tamanio;

            this.anguloOriginal = angleInDegrees;
            float anguloEnRadianes = (float) (angleInDegrees * Math.PI / 180);
            
            Vector2 X = new Vector2((float)Math.Cos(anguloEnRadianes), (float)Math.Sin(anguloEnRadianes));
            Vector2 Y = new Vector2((float)-Math.Sin(anguloEnRadianes), (float)Math.Cos(anguloEnRadianes));

            X  = X * tamanio.X / 2;
            Y  = Y * tamanio.Y / 2;

            corner[0] = center - X - Y;
            corner[1] = center + X - Y;
            corner[2] = center + X + Y;
            corner[3] = center - X + Y;

            computeAxes();
        }


        /** Returns true if the intersection of the boxes is non-empty. */
        public override bool IntersectsWith(BoundingRectangle other)
        {
            return overlaps1Way(other) && other.overlaps1Way(this);
        }

        public override bool IntersectsWith(BoundingCircle bc)
        {
            return bc.IntersectsWith(this);
        }

        public override bool IsInside(Vector2 point)
        {
            if (RotacionEnGrados == 0)
            {
                //Lo trato como un AABB
                float minX = Puntos[0].X;
                float maxX = Puntos[1].X;
                float minY = Puntos[0].Y;
                float maxY = Puntos[2].Y;

                return !(point.X < minX ||
                    point.X > maxX ||
                    point.Y < minY ||
                    point.Y > maxY);
            }
            else
            {
                //Genero los 2 triangulos que componen el rectangulo rotado y valido si el punto
                //esta adentro de alguno de ellos
                return new BoundingTriangle(corner[0], corner[1], corner[2]).IsInside(point) ||
                    new BoundingTriangle(corner[0], corner[2], corner[3]).IsInside(point);
            }
        }

        protected BoundingRectangle(Vector2 center, Vector2 tamanio, float angleInDegrees, Vector2[] corner)
        {
            centroOriginal = center;
            tamanioOriginal = tamanio;
            this.anguloOriginal = angleInDegrees;

            for (int i =0; i < 4; i++)
                this.corner[i] = corner[i];

            computeAxes();
        }

        public override BoundingObject RotateAndTranslate(float angleInDegrees, Vector2 distance)
        {
            if (angleInDegrees != 0)
            {
                Vector2[] newCorners = new Vector2[4];

                for (int i = 0; i < 4; i++)
                    newCorners[i] = corner[i].RotateByDegress(angleInDegrees) + distance;

                return new BoundingRectangle(centroOriginal.RotateByDegress(angleInDegrees) + distance, tamanioOriginal, anguloOriginal + angleInDegrees, newCorners);
            }
            else
            {
                return new BoundingRectangle(centroOriginal.RotateByDegress(angleInDegrees) + distance, tamanioOriginal, anguloOriginal);
            }
        }
    }
}
