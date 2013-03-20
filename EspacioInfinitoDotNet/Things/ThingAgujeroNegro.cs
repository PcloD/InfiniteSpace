using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using EspacioInfinitoDotNet.Graphics;
using EspacioInfinitoDotNet.Universes;
using EspacioInfinitoDotNet.Maths;

namespace EspacioInfinitoDotNet.Things
{
    class ThingAgujeroNegro : ThingRedondo
    {
        #region Atributos

        static Textura Textura;
        float velocidadAtraccion = 500.0f;
        float radioAccion = 1000.0f;
        float velocidadRotacion = 180.0f;

        public float VelocidadAtraccion
        {
            get { return velocidadAtraccion; }
            set { this.velocidadAtraccion = value; }
        }

        public float RadioAccion
        {
            get { return radioAccion; }
            set { this.radioAccion = value; }
        }

        public float VelocidadRotacion
        {
            get { return velocidadRotacion; }
            set { this.velocidadRotacion = value; }
        }

        #endregion

        public ThingAgujeroNegro(Galaxia galaxia, Vector2 centro, float diametro)
            : base(galaxia, new Vector2(diametro, diametro), centro, 0)
        {
            SetMovible(false);
            SetMaxVida(1000000);
            SetVida(1000000);
            SetColorEnMapa(Color.Violet);
        }

        #region Metodos sobrecargados de Thing

        public override void Dibujar()
        {
            if (Textura == null)
                Textura = TexturaManager.Instance.CargarTextura(Data.NombresTexturas.AgujeroNegro);

            GraphicEngine.Instance.DrawRectangle(Centro, RotacionEnGrados, Tamanio, Textura);
        }

        public override void Procesar(float fDeltaSegundos)
        {
            Rotar(velocidadRotacion * fDeltaSegundos, false, false);

            Thing[] thingsAfectados = Galaxia.GetThingsEnRadio(Centro, radioAccion);

            foreach (Thing thing in thingsAfectados)
                if (thing.Movible)
                {
                    Vector2 distancia = thing.Centro - this.Centro;

                    float velocidad = -velocidadAtraccion * (radioAccion - distancia.Length) / radioAccion;

                    thing.MoverA(distancia.Normalized() * velocidad * fDeltaSegundos, true, true);

                    float anguloDistancia = distancia.AngleInDegress + 180.0f;
                    float anguloThing = thing.RotacionEnGrados;

                    anguloDistancia = anguloDistancia % 360.0f;
                    if (anguloDistancia < 0)
                        anguloDistancia += 360.0f;

                    float separacionRotacion = anguloDistancia - anguloThing;
                    if (separacionRotacion < 0)
                        separacionRotacion += 360.0f;

                    if (separacionRotacion > 180.0f)
                        separacionRotacion = -(separacionRotacion - 180.0f);

                    float velocidadRotacionAngulo = (radioAccion - distancia.Length) / radioAccion;

                    separacionRotacion *= velocidadRotacionAngulo * fDeltaSegundos;

                    thing.Rotar(separacionRotacion, true, true);
                }
        }

        public override void OnImpacto(Thing thing)
        {
            if (thing.Eliminado)
                return;

            Random rnd = new Random((int) DateTime.Now.Ticks);

            if (rnd.Next(0, 2) == 1)
            {
                if (thing is ThingDaniable)
                    ((ThingDaniable) thing).ProcesarDanio(this, 1000.0f);
            }
            else
            {
                while (true)
                {
                    SectorID sectorIDDestino = new SectorID(
                        rnd.Next(-Galaxia.TamanioEnSectores.Width / 2, Galaxia.TamanioEnSectores.Width / 2),
                        rnd.Next(-Galaxia.TamanioEnSectores.Height / 2, Galaxia.TamanioEnSectores.Height / 2));

                    Sector sectorDestino = Galaxia.GetSector(sectorIDDestino);

                    Vector2 posicionEnSector = new Vector2(
                        rnd.Next(-Sector.TamanioSector / 2, -Sector.TamanioSector / 2),
                        rnd.Next(-Sector.TamanioSector / 2, -Sector.TamanioSector / 2));

                    Vector2 posicionFinal = sectorDestino.Centro + posicionEnSector;

                    if (Galaxia.ColisionaConThing(thing, posicionFinal, thing.Tamanio, thing.RotacionEnGrados) == null)
                    {
                        Galaxia.ActualizarCentroThing(thing, posicionFinal);
                        break;
                    }
                }
            }
        }

        #endregion
    }
}
