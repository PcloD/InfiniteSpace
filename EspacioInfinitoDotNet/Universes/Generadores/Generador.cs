using System;
using System.Collections.Generic;
using System.Text;
using EspacioInfinitoDotNet.Maths;
using System.Drawing;

namespace EspacioInfinitoDotNet.Universes.Generadores
{
    public abstract class Generador
    {
        public const int DistanciaMinimaEntreThingsEnSector = 400;

        public abstract Faccion[] InicializarFacciones();
        public abstract Vector2 GetPosicionInicialJugador();
        public abstract void Inicializar(Galaxia galaxia);
        public abstract Sector CrearSector(SectorID sectorID);
        public abstract Size GetTamanioEnSectores();
        public abstract Faccion GetFaccionJugador();
        public abstract string Nombre();
        public abstract string Descripcion();

        static public Generador[] GeneradoresDisponibles()
        {
            return new Generador[] { new GeneradorAleatorio(), new GeneradorTest(), new GeneradorDemo() };
        }
    }
}
