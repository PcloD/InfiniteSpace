using System;
using System.Collections.Generic;
using System.Text;

namespace EspacioInfinitoDotNet.Universes
{
    public class SectorID :  IEquatable<SectorID>
    {
        #region Atributos

        private int x;
        private int y;

        public int X
        {
            get { return x; }
        }

        public int Y
        {
            get { return y; }
        }

        public SectorID[] SectoresCercanos
        {
            get
            {
                SectorID[] sectores = new SectorID[8];

                sectores[0] = new SectorID(x - 1, y - 1);
                sectores[1] = new SectorID(x - 1, y);
                sectores[2] = new SectorID(x - 1, y + 1);
                sectores[3] = new SectorID(x, y - 1);
                sectores[4] = new SectorID(x, y + 1);
                sectores[5] = new SectorID(x + 1, y - 1);
                sectores[6] = new SectorID(x + 1, y);
                sectores[7] = new SectorID(x + 1, y + 1);

                return sectores;
            }
        }

        public SectorID[] SectoresCercanosIncluyendose
        {
            get
            {
                SectorID[] sectores = new SectorID[9];

                sectores[0] = new SectorID(x - 1, y - 1);
                sectores[1] = new SectorID(x - 1, y);
                sectores[2] = new SectorID(x - 1, y + 1);
                sectores[3] = new SectorID(x, y - 1);
                sectores[4] = new SectorID(x, y);
                sectores[5] = new SectorID(x, y + 1);
                sectores[6] = new SectorID(x + 1, y - 1);
                sectores[7] = new SectorID(x + 1, y);
                sectores[8] = new SectorID(x + 1, y + 1);

                return sectores;
            }
        }

        #endregion

        public SectorID(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        #region Equals y GetHashCode

        public override bool Equals(object obj)
        {
            if (obj is SectorID)
            {
                SectorID objT = (SectorID)obj;
                return this.x == objT.x && this.y == objT.y;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (x << 16) + y;
        }

        #endregion

        #region IEquatable<SectorID> Members

        public bool Equals(SectorID other)
        {
            return other.x == this.x && other.y == this.y;
        }

        #endregion
    }
}