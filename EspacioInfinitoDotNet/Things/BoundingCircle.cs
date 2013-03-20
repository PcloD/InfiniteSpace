using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using EspacioInfinitoDotNet.Maths;

namespace EspacioInfinitoDotNet.Things
{
    public class BoundingCircle : BoundingObject
    {
        private Vector2 center;
        private float radius;

        public Vector2 Center
        {
            get { return center; }
            set { center = value; }
        }

        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        public BoundingCircle(Vector2 center, float radius)
        {
            this.center = center;
            this.radius = radius;
        }

        public override bool IntersectsWith(BoundingCircle bc)
        {
            Vector2 vDiff = bc.center - this.center;
            float sumRadius = bc.radius + this.radius;

            if (vDiff.LengthSqr <= sumRadius * sumRadius)
                return true;

            return false;
        }

        public override bool IsInside(Vector2 point)
        {
            if ((point - this.center).LengthSqr <= this.radius * this.radius)
                return true;

            return false;
        }

        public override bool IntersectsWith(BoundingRectangle br)
        {
            if (br.RotacionEnGrados == 0)
            {
                //El rectangulo no esta rotado, lo trato como si fuera un AABB

                double Rad = radius;
                double Rad2 = Rad * Rad;
                double maxX, maxY, minX, minY;

                minX = br.Puntos[0].X;
                maxX = br.Puntos[1].X;
                minY = br.Puntos[0].Y;
                maxY = br.Puntos[2].Y;

                /* Translate coordinates, placing C at the origin. */
                maxX -= center.X; maxY -= center.Y;
                minX -= center.X; minY -= center.Y;

                if (maxX < 0) 			/* R to left of circle center */
                    if (maxY < 0) 		/* R in lower left corner */
                        return ((maxX * maxX + maxY * maxY) < Rad2);
                    else if (minY > 0) 	/* R in upper left corner */
                        return ((maxX * maxX + minY * minY) < Rad2);
                    else 					/* R due West of circle */
                        return (Math.Abs(maxX) < Rad);
                else if (minX > 0)  	/* R to right of circle center */
                    if (maxY < 0) 	/* R in lower right corner */
                        return ((minX * minX + maxY * maxY) < Rad2);
                    else if (minY > 0)  	/* R in upper right corner */
                        return ((minX * minX + minY * minY) < Rad2);
                    else 				/* R due East of circle */
                        return (minX < Rad);
                else				/* R on circle vertical centerline */
                    if (maxY < 0) 	/* R due South of circle */
                        return (Math.Abs(maxY) < Rad);
                    else if (minY > 0)  	/* R due North of circle */
                        return (minY < Rad);
                    else 				/* R contains circle centerpoint */
                        return (true);
            }
            else
            {
                //TODO: Esto es muy lento, optimizar!!

                //Si el centro del circulo esta dentro del rectangulo, o si el centro del rectangulo esta dentro del circulo,
                //seguro hay colision
                 if (br.IsInside(this.center) ||
                     IsInside(br.Center))
                    return true;

                //El centro del circulo no esta dentro del rectangulo, tengo que validar mas cuidadosamente..
                
                //Valido si hay interseccion entre el circulo y alguno de los 4 segmentos que componen el rectangulo rotado!

                for (int i = 0; i < 4; i++)
                    if (IntersectsWith(br.Puntos[i % 4], br.Puntos[(i + 1) % 4]))
                        return true;

                return false;
            }
        }

        public bool IntersectsWith(Vector2 p1, Vector2 p2)
        {
            //Valido si hay interseccion entre el circulo y el segmento pasado

            //Sacado de:
            //http://mathworld.wolfram.com/Circle-LineIntersection.html

            p1 = Center - p1;
            p2 = Center - p2;

            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            float dr = (float) Math.Sqrt(dx * dx + dy * dy);

            float D = p1.X * p2.Y - p2.X * p1.Y;

            float disc = radius * radius * dr * dr - D * D;

            if (disc < 0)
                return false;

            //Determino los puntos de colision

            disc = (float) Math.Sqrt(disc);

            Vector2 int1 = new Vector2(
                (D * dy + Math.Sign(dy) * dx * disc) / (dr * dr),
                (-D * dx + Math.Abs(dy) * disc) / (dr * dr));

            Vector2 int2 = new Vector2(
                (D * dy - Math.Sign(dy) * dx * disc) / (dr * dr),
                (-D * dx - Math.Abs(dy) * disc) / (dr * dr));

            //Ahora veo si los puntos de colision pertenecen a los segmentos

            float u1, u2;

            if (p2.X - p1.X != 0)
            {
                u1 = (int1.X - p1.X) / (p2.X - p1.X);
                u2 = (int2.X - p1.X) / (p2.X - p1.X);
            }
            else
            {
                u1 = (int1.Y - p1.Y) / (p2.Y - p1.Y);
                u2 = (int2.Y - p1.Y) / (p2.Y - p1.Y);
            }

            return (u1 >= 0 && u1 <= 1) || (u2 >= 0 && u2 <= 1);
        }

        public override BoundingObject RotateAndTranslate(float angleInDegrees, Vector2 distance)
        {
            return new BoundingCircle(Center.RotateByDegress(angleInDegrees) + distance, radius);
        }

    }
}
