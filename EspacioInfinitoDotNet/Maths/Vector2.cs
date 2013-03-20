using System;
using System.Collections.Generic;
using System.Text;

namespace EspacioInfinitoDotNet.Maths
{
    public struct Vector2
    {
        float x;
        float y;

        public float X
        {
            get { return x; }
            set { x = value; }
        }

        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2(Vector2 v)
        {
            this.x = v.x;
            this.y = v.y;
        }

        // Are these two Vector2's equal?
        static public bool operator ==(Vector2 a, Vector2 b)
        {
            return ((a.x == b.x) && (a.y == b.y));
        }
        // Are these two Vector2's not equal?
        static public Boolean operator !=(Vector2 a, Vector2 b)
        {
            return ((a.x != b.x) || (a.y != b.y));
        }
        // Negate this vector
        static public Vector2 operator -(Vector2 a)
        {
            return new Vector2(-a.x, -a.y);
        }
        // Add two Vector2's
        static public Vector2 operator +(Vector2 a, Vector2 b)
        {
            Vector2 ret = new Vector2(a.X + b.X, a.Y + b.Y);
            return ret;
        }
        // Subtract one Vector2 from another
        static public Vector2 operator -(Vector2 a, Vector2 b)
        {
            Vector2 ret = new Vector2(a.x - b.x, a.y - b.y);
            return ret;
        }
        // Multiply Vector2 by a CoordType
        static public Vector2 operator *(Vector2 v, float f)
        {
            return new Vector2(f * v.x, f * v.y);
        }
        // Multiply Vector2 by a CoordType
        static public Vector2 operator *(float f, Vector2 v)
        {
            return new Vector2(f * v.x, f * v.y);
        }
        // Divide Vector2 by a CoordType
        static public Vector2 operator /(Vector2 v, float f)
        {
            return new Vector2(v.x / f, v.y / f);
        }

        // Methods
        // Set Values
        public void Set(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        // Get length of a Vector2
        public float Length
        {
            get { return (float) Math.Sqrt(x * x + y * y); }
        }
        // Get squared length of a Vector2
        public float LengthSqr
        {
            get { return (x * x + y * y); }
        }
        // Does Vector2 equal (0, 0)?
        public bool IsZero()
        {
            return ((x == 0.0F) && (y == 0.0F));
        }
        // Normalize a Vector2
        public Vector2 Normalized()
        {
            Vector2 v = new Vector2(this);

            float m = v.Length;

            if (m > 0.0F)
                m = 1.0F / m;
            else
                m = 0.0F;
            
            v.x *= m;
            v.y *= m;

            return v;
        }

        public float Dot(Vector2 v)
        {
            return x * v.x + y * v.y;
        }

        public float AngleInDegress
        {
            get
            {
                Vector2 n = Normalized();

                double angulo = Math.Asin(n.y / 1) * 180 / Math.PI;

                if (n.x < 0)
                    angulo = 180 - angulo;

                if (angulo < 0)
                    angulo = 360 + angulo;

                return (float) angulo;
            }
        }

        public Vector2 RotateByDegress(float angleInDegrees)
        {
            float angleInRadians = (float) (angleInDegrees * Math.PI / 180);

            Vector2 v = new Vector2(
                (float) Math.Cos(angleInRadians) * x - (float) Math.Sin(angleInRadians) * y,
                (float) Math.Sin(angleInRadians) * x + (float) Math.Cos(angleInRadians) * y);

            return v;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector2)
                return ((Vector2)obj) == this;
            return false;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() + y.GetHashCode();
        }
    }
}
