using System;
using System.Collections.Generic;
using System.Text;
using EspacioInfinitoDotNet.Maths;

namespace EspacioInfinitoDotNet.Things
{
    public abstract class BoundingObject
    {
        public abstract bool IsInside(Vector2 point);
        public abstract bool IntersectsWith(BoundingRectangle rectangle);
        public abstract bool IntersectsWith(BoundingCircle circle);
        public abstract BoundingObject RotateAndTranslate(float angleInDegrees, Vector2 distance);

        public bool IntersectsWith(BoundingObject bo)
        {
            if (bo is BoundingCircle)
                return IntersectsWith((BoundingCircle)bo);
            else if (bo is BoundingRectangle)
                return IntersectsWith((BoundingRectangle)bo);

            throw new Exception("Tipo de objeto no colisionable!");
        }
    }
}
