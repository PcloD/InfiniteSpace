using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using EspacioInfinitoDotNet.Graphics;
using EspacioInfinitoDotNet.Universes;
using EspacioInfinitoDotNet.Maths;

namespace EspacioInfinitoDotNet.Things
{
    public class ThingOndaExpansiva : Thing
    {
        #region Atributos

        static Textura textura;

        float duracionSegundos = 5.0f;
        float duracionTranscurrida = 0.0f;
        float radioActual = 1.0f;
        float radioInicial = 1.0f;
        float radioMaximo;
        float danio = 0.0f;
        List<Thing> thingsAfectados = new List<Thing>();

        public float RadioMaximo
        {
            get { return radioMaximo; }
            set { radioMaximo = value; }
        }

        public float RadioInicial
        {
            get { return radioInicial; }
            set { radioInicial = value; }
        }

        public float DuracionSegundos
        {
            get { return duracionSegundos; }
            set { duracionSegundos = value; }
        }

        public float Danio
        {
            get { return danio; }
            set { danio = value; }
        }

        #endregion

        public ThingOndaExpansiva(Galaxia galaxia, Vector2 center)
            : base(galaxia, new Vector2(1, 1), center, 0)
        {
            SetSolido(false);
            SetActivo(true);
        }

        #region Metodos sobrecargados de Thing

        public override void Dibujar()
        {
            if (textura == null)
                textura = TexturaManager.Instance.CargarTextura(Data.NombresTexturas.OndaExpansiva);

            Color c;

            float inicioTransparencia = duracionSegundos * 2 / 3;

            //Dibujo la onda con transparencia recien sobre el 1/3 final de su vida
            if (duracionTranscurrida >= inicioTransparencia)
            {
                float alpha = 1.0f - (duracionTranscurrida - inicioTransparencia) / (duracionSegundos - inicioTransparencia);

                alpha *= alpha;

                if (alpha < 0.05f)
                    alpha = 0.05f;

                c = Color.FromArgb((int)(alpha * 255.0f), Color.White);
            }
            else
                c = Color.White;

            GraphicEngine.Instance.DrawRectangle(Centro, RotacionEnGrados, Tamanio, textura, c);
        }

        public override void Procesar(float fDeltaSegundos)
        {
            duracionTranscurrida += fDeltaSegundos;

            if (duracionTranscurrida > duracionSegundos)
            {
                Eliminar();
            }
            else
            {
                //Daño los nuevos objetos con los que impacto, para lo cual calculo el nuevo radio y solo
                //daño a quellos objetos que se encuentren a una distancia mayor a la del radio viejo, de esta
                //forma no daño 2 veces al mismo objeto! Al menos que se mueva delante de la onda expansiva, en cuyo
                //caso esta bien que lo dañe. 
                //Tambien mantengo un vector de los things que fueron afectados, asi me aseguro de afectarlos al menos una vez,
                //ya que existe la posibilidad de que el thing se mueva de forma tal que no lo afecte segun el chequeo anterior.

                float nuevoRadioActual = radioInicial + (radioMaximo - radioInicial) * duracionTranscurrida / duracionSegundos;

                Thing[] thingsImpactados = Galaxia.GetThingsEnRadio(Centro, nuevoRadioActual);

                foreach(Thing thingImpactado in thingsImpactados)
                {
                    if (thingImpactado is ThingDaniable)
                    {
                        float distancia = (thingImpactado.Centro - Centro).Length;

                        if (distancia > radioActual || !thingsAfectados.Contains(thingImpactado))
                        {
                            ((ThingDaniable)thingImpactado).ProcesarDanio(this, danio);
                            thingsAfectados.Add(thingImpactado);
                        }
                    }
                }

                radioActual = nuevoRadioActual;

                SetTamanio(new Vector2(radioActual * 2, radioActual * 2));
            }
        }

        #endregion
    }
}
