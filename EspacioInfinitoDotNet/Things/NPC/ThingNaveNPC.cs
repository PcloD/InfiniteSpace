using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using EspacioInfinitoDotNet.Graphics;
using EspacioInfinitoDotNet.Universes;
using EspacioInfinitoDotNet.Maths;

namespace EspacioInfinitoDotNet.Things.NPC
{
    public class ThingNaveNPC : ThingNave
    {
        #region Atributos
        
        private Estados.EstadoNPC estadoNPC;
        private Sector sectorNativo;

        public Sector SectorNativo
        {
            get { return sectorNativo; }
        }

        public Estados.EstadoNPC EstadoNPC
        {
            get { return estadoNPC; }
        }

        #endregion
        
        public ThingNaveNPC(Galaxia galaxia, Vector2 size, Vector2 center, float rotation, Faccion faccion)
            : base(galaxia, size, center, rotation, faccion)
        {
            this.estadoNPC = new Estados.EstadoNPCPlanificar(this);
            Nombre = "NPC " + GetHashCode();
            sectorNativo = Galaxia.GetSectorEnPosicion(Centro);
            velocidad = 1200.0f;
            //SetColorEnMapa(Color.FromArgb(200, Color.Green));
            SetColorEnMapa(Faccion.Color);
        }

        public override void ProcesarDanio(Thing delThing, float danio)
        {
            base.ProcesarDanio(delThing, danio);

            if (delThing is ThingLaser)
            {
                ThingLaser tl = (ThingLaser)delThing;

                if (tl.Duenio is ThingNave)
                {
                    ThingNave nave = (ThingNave) tl.Duenio;

                    if (nave.Faccion != Faccion)
                    {
                        if (Eliminado)
                        {
                            Faccion.InformarDestruccionDePropiedad(nave.Faccion);
                        }
                        else
                        {
                            Faccion.InformarAtaque(nave.Faccion);
                            estadoNPC = estadoNPC.OnDaniadoPor(nave);
                        }
                    }
                }
            }
        }

        public override void Dibujar()
        {
            base.Dibujar();

            //Accion que esta realizando
            Graphics.GraphicEngine.Instance.DrawTextStrokeCenteredX(Centro.X, Centro.Y + Tamanio.Y / 2.0f - 100.0f, Color.White, 50.0f, estadoNPC.Describir());
        }


        public override void Procesar(float fDeltaSegundos)
        {
            base.Procesar(fDeltaSegundos);

            estadoNPC = estadoNPC.Procesar(fDeltaSegundos);
        }
    }
}
