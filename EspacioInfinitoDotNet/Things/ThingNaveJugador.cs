using System;
using System.Collections.Generic;
using System.Text;
using EspacioInfinitoDotNet.Universes;
using EspacioInfinitoDotNet.Maths;

namespace EspacioInfinitoDotNet.Things
{
    public class ThingNaveJugador : ThingNave
    {
        public ThingNaveJugador(Galaxia galaxia, Vector2 size, Vector2 center, float rotation, Faccion faccion)
            : base(galaxia, size, center, rotation, faccion)
        {
            SetActivo(true);
            Nombre = "Jugador";
            SetMaxVida(10000);
            SetVida(10000);
            SetColorEnMapa(System.Drawing.Color.White);
        }
    }
}
