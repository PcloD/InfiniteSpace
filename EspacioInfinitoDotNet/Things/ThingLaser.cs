using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using EspacioInfinitoDotNet.Graphics;
using EspacioInfinitoDotNet.Universes;
using EspacioInfinitoDotNet.Maths;

namespace EspacioInfinitoDotNet.Things
{
    public class ThingLaser : Thing
    {
        #region Atributos

        static Textura Textura;
        static BoundingObject[] boundingObjects;
        float vidaSegundos = 1.0f;
        float danio = 20.0f;
        float velocidad = 3000.0f;
        Thing duenio;

        public float Danio
        {
            get { return danio; }
        }

        public Thing Duenio
        {
            get { return duenio; }
        }

        #endregion

        public ThingLaser(Galaxia galaxia, Vector2 center, float rotation, Thing duenio)
            : base(galaxia, new Vector2(40.0f, 8.0f), center, rotation)
        {
            SetActivo(true);
            this.duenio = duenio;
        }

        #region Metodos sobrecargados de Thing

        public override BoundingObject[] CrearBoundingObjects()
        {
            if (boundingObjects == null)
            {
                boundingObjects = new BoundingObject[1];

                boundingObjects[0] = new BoundingRectangle(new Vector2(0, 0), Tamanio, 0);
            }

            return boundingObjects;
        }

        public override void Dibujar()
        {
            if (Textura == null)
                Textura = TexturaManager.Instance.CargarTextura(Data.NombresTexturas.Laser);

            GraphicEngine.Instance.DrawRectangle(Centro, RotacionEnGrados, Tamanio, Textura);
        }

        public override void Procesar(float fDeltaSegundos)
        {
            vidaSegundos -= fDeltaSegundos;

            if (vidaSegundos < 0.0f)
                Eliminar();
            else
                MoverAdelante(velocidad * fDeltaSegundos, true, true);
        }

        public override void OnImpacto(Thing thing)
        {
            if (thing.Eliminado)
                return;

            if (!(thing is ThingAgujeroDeGusano))
            {
                Eliminar();

                if (thing is ThingDaniable)
                    ((ThingDaniable) thing).ProcesarDanio(this, Danio);
            }
        }

        #endregion
    }
}
