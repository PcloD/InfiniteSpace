using System;
using System.Collections.Generic;
using System.Text;
using EspacioInfinitoDotNet.Maths;

namespace EspacioInfinitoDotNet.Things
{
    class BoundingTriangle
    {
        Vector2 p1, p2, p3;

        public BoundingTriangle(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            this.p1 = p1; this.p2 = p2; this.p3 = p3;
        }

        public bool IsInside(Vector2 p)
        {
            float fAB = (p.Y-p1.Y)*(p2.X-p1.X) - (p.X-p1.X)*(p2.Y-p1.Y);
            float fBC = (p.Y-p2.Y)*(p3.X-p2.X) - (p.X-p2.X)*(p3.Y-p2.Y);
            float fCA = (p.Y-p3.Y)*(p1.X-p3.X) - (p.X-p3.X)*(p1.Y-p3.Y);

            return (fAB * fBC >= 0 && fBC * fCA >= 0);
        }
    }
}
