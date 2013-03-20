using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using EspacioInfinitoDotNet.Graphics;
using EspacioInfinitoDotNet.Universes;
using EspacioInfinitoDotNet.Maths;

namespace EspacioInfinitoDotNet.Things
{
    public class ThingEstacionEspacial : ThingDaniable
    {
        #region Atributos

        BoundingObject[] boundingObjects;
        Textura textura;

        bool enOrbita = false;
        Vector2 centroOrbita; //Puede esta  r en orbita alrededor de un centro determinado
        Thing thingCentroOrbita; //O alrededor de un objeto determinado
        float radioOrbita;
        float velocidadRotacionOrbita; //En grados por segundo
        float rotacionOrbita = 0.0f;

        public bool EnOrbita
        {
            get { return enOrbita; }
            set { enOrbita = value; }
        }

        public Vector2 CentroOrbita
        {
            get { return centroOrbita; }
            set { centroOrbita = value; }
        }

        public Thing ThingCentroOrbita
        {
            get { return thingCentroOrbita; }
            set { thingCentroOrbita = value; }
        }

        public float RadioOrbita
        {
            get { return radioOrbita; }
            set { radioOrbita = value; }
        }

        public float VelocidadRotacionOrbita
        {
            get { return velocidadRotacionOrbita; }
            set { velocidadRotacionOrbita = value; }
        }

        public float RotacionOrbita
        {
            get { return rotacionOrbita; }
            set { rotacionOrbita = value; }
        }
        
        
        #endregion

        public ThingEstacionEspacial(Galaxia galaxia, Vector2 centro, Vector2 tamanio)
            : base(galaxia, tamanio, centro, 0)
        {
            SetMovible(false);
            SetVida(1000000);
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
            if (textura == null)
                textura = TexturaManager.Instance.CargarTextura(Data.NombresTexturas.EstacionEspacial);

            GraphicEngine.Instance.DrawRectangle(Centro, RotacionEnGrados, Tamanio, textura);
        }

        public override void Procesar(float fDeltaSegundos)
        {
            //TODO: Actualziar estado interno? recursos?

            if (enOrbita)
            {
                if (thingCentroOrbita != null)
                    centroOrbita = thingCentroOrbita.Centro;

                Vector2 nuevaPosicion = new Vector2(radioOrbita, 0);
                nuevaPosicion = nuevaPosicion.RotateByDegress(rotacionOrbita);
                nuevaPosicion += centroOrbita;                

                MoverA(nuevaPosicion - Centro, true, false);

                rotacionOrbita += velocidadRotacionOrbita * fDeltaSegundos;

                if (rotacionOrbita > 360.0f)
                    rotacionOrbita -= 360.0f;
            }
        }

        public override void OnImpacto(Thing thing)
        {
            //TODO: Dockear? Similar a planeta?
        }

        #endregion
    }
}
