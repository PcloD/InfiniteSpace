using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using EspacioInfinitoDotNet.Graphics;
using EspacioInfinitoDotNet.Universes;
using EspacioInfinitoDotNet.Maths;

namespace EspacioInfinitoDotNet.Things
{
    public class ThingResto : Thing
    {
        static Random rnd = new Random();

        float vida;
        float velocidad; 

        public ThingResto(Galaxia galaxia, Vector2 center)
            : base(galaxia, new Vector2(rnd.Next(3000, 5000) / 1000.0f, rnd.Next(3000, 5000) / 1000.0f), center, (float)rnd.Next(0, 360))
        {
            SetSolido(false);
            SetActivo(true);
            vida = rnd.Next(1000, 3000) / 1000.0f;
            velocidad = (float) rnd.Next(500, 2000);
        }

        #region Metodos sobrecargados de Thing

        public override void Dibujar()
        {
            GraphicEngine.Instance.DrawRectangle(Centro, RotacionEnGrados, Tamanio, Color.Gray);
        }

        public override void Procesar(float fDeltaSegundos)
        {
            vida -= fDeltaSegundos;

            if (vida < 0.0f)
                Eliminar();
            else
                MoverAdelante(velocidad * fDeltaSegundos, true, true);
        }

        public override void OnImpacto(Thing thing)
        {
            if (thing.Eliminado)
                return;

            //RemoveFromSector();
        }

        #endregion
    }
}
