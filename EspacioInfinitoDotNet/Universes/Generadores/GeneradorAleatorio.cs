using System;
using System.Collections.Generic;
using System.Text;
using EspacioInfinitoDotNet.Maths;
using System.Drawing;
using System.Drawing.Imaging;
using EspacioInfinitoDotNet.Things;
using EspacioInfinitoDotNet.Graphics;

namespace EspacioInfinitoDotNet.Universes.Generadores
{
    public class GeneradorAleatorio : Generador
    {
        public const int TamanioEnSectores = 1024;
        public const int CantidadFacciones = 10;
        public const int ProbabilidadAgujeroNegro = 300;
        public const int ProbabilidadEstrella = 30;
        public const int ProbabilidadAgujeroDeGusano = 50;
        public const int ProbabilidadNebula = 100;
        public const int ProbabilidadNebulaContinua = 2;
        public const int ProbabilidadPlaneta = 10;
        public const int ProbabilidadEstacionEspacial = 50;
        public const int ProbabilidadEstacionEspacialOrbitaPlaneta = 2;

        private Galaxia galaxia;
        private System.Random rndCreacionSectores = new Random((int)DateTime.Now.Ticks);
        private List<Faccion> facciones;
        private Faccion faccionJugador;

        private Bitmap bmpUniverso;
        
        public override void Inicializar(Galaxia galaxia)
        {
            this.galaxia = galaxia;

            bmpUniverso = InicializarBitmap();
        }

        public override string Nombre()
        {
            return "GeneradorAleatorio";
        }

        public override string Descripcion()
        {
            return "Genera una galaxia al azar, es siempre distinta!";
        }

        private Bitmap InicializarBitmap()
        {
            InicializarFacciones();

            //El canal R define la faccion (31 facciones posibles)
            //Los segundos 8 bits atributos del sector
            Bitmap bmp = new Bitmap(TamanioEnSectores, TamanioEnSectores, PixelFormat.Format16bppRgb555);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
            g.Clear(Color.Black);

            foreach (Faccion faccion in facciones)
            {
                Color color = Color.FromArgb(faccion.Identificador * 8, 0, 0);

                Brush brush = new SolidBrush(color);

                int cantidadSectores = rndCreacionSectores.Next(100, 1000);

                for (int c = 0; c < cantidadSectores; c++)
                {
                    int centerX = rndCreacionSectores.Next(-TamanioEnSectores / 2 + 30, TamanioEnSectores / 2 - 30);
                    int centerY = rndCreacionSectores.Next(-TamanioEnSectores / 2 + 30, TamanioEnSectores / 2 - 30);

                    int fromX, fromY, toX, toY;

                    fromX = centerX - rndCreacionSectores.Next(1, 30);
                    fromY = centerX - rndCreacionSectores.Next(1, 30);
                    toX = centerX + rndCreacionSectores.Next(1, 30);
                    toY = centerY + rndCreacionSectores.Next(1, 30);

                    g.FillRectangle(brush, fromX, fromY, toX - fromX, toY - fromY);
                }

            }

            return bmp;
        }

        public override Faccion[] InicializarFacciones()
        {
            if (facciones == null)
            {
                facciones = new List<Faccion>();

                for (int i = 0; i < CantidadFacciones; i++)
                {
                    Faccion.TipoFaccionEnum tipoFaccion = Faccion.TipoFaccionEnum.Agresiva;

                    switch (rndCreacionSectores.Next(0, 4))
                    {
                        case 0:
                            tipoFaccion = Faccion.TipoFaccionEnum.Agresiva;
                            break;
                        case 1:
                            tipoFaccion = Faccion.TipoFaccionEnum.Neutral;
                            break;
                        case 2:
                            tipoFaccion = Faccion.TipoFaccionEnum.Pavisa;
                            break;
                        case 3:
                            tipoFaccion = Faccion.TipoFaccionEnum.Recolectora;
                            break;
                    }

                    Color color = Color.FromArgb(255,
                        rndCreacionSectores.Next(120, 256),
                        rndCreacionSectores.Next(120, 256),
                        rndCreacionSectores.Next(120, 256));
                                        
                    Faccion faccion = new Faccion("Faccion " + i, (byte)i, tipoFaccion, color);
                    
                    facciones.Add(faccion);
                }

                faccionJugador = facciones[0];
            }

            return facciones.ToArray();
        }

        public override EspacioInfinitoDotNet.Maths.Vector2 GetPosicionInicialJugador()
        {
            Vector2 posicionInicialJugador;

            do
            {
                posicionInicialJugador = galaxia.GetSectorEnPosicion(new Vector2(
                    (float)rndCreacionSectores.Next((int)-galaxia.Tamanio.X / 2, (int)galaxia.Tamanio.Y / 2),
                    (float)rndCreacionSectores.Next((int)-galaxia.Tamanio.Y / 2, (int)galaxia.Tamanio.Y / 2))).BuscarEspacioVacio(300);

            } while (galaxia.GetSectorEnPosicion(posicionInicialJugador).Faccion != null &&
                    galaxia.GetSectorEnPosicion(posicionInicialJugador).Faccion.GetRelacion(faccionJugador) == Faccion.RelacionConOtraFaccionEnum.Agresiva);

            return posicionInicialJugador;
        }

        public override Sector CrearSector(SectorID sectorID)
        {
            Faccion faccion = null;

            if (sectorID.X > -TamanioEnSectores / 2 &&
                sectorID.X < TamanioEnSectores / 2 &&
                sectorID.Y > -TamanioEnSectores / 2 &&
                sectorID.Y < TamanioEnSectores / 2)
            {
                int x = sectorID.X + TamanioEnSectores / 2;
                int y = sectorID.Y + TamanioEnSectores / 2;

                Color color = bmpUniverso.GetPixel(x, y);

                faccion = facciones[color.R / 8];
            }

            Sector sector = new Sector(galaxia, sectorID, faccion);

            galaxia.AgregarSector(sector);

            bool puedeSerNebula = true;
            bool puedeTenerNaves = false;

            if (rndCreacionSectores.Next(0, ProbabilidadAgujeroNegro) == 0)
            {
                //Agrego un agujero negro

                puedeSerNebula = false; //Los sectores con agujeros negros no deben contener nebulas

                float diametro = rndCreacionSectores.Next(100, 500);
                float radioAlcance = rndCreacionSectores.Next(1000, 3000);
                float velocidadRotacion = rndCreacionSectores.Next(90, 360);

                Vector2 posicion = sector.BuscarEspacioVacio(diametro / 2 + DistanciaMinimaEntreThingsEnSector);

                ThingAgujeroNegro t = new ThingAgujeroNegro(galaxia, posicion, diametro);

                t.RadioAccion = radioAlcance;
                t.VelocidadRotacion = velocidadRotacion;
            }
            else if (rndCreacionSectores.Next(0, ProbabilidadEstrella) == 0)
            {
                //Agrego una estrella

                float diametro = rndCreacionSectores.Next(100, 500);
                float radioAlcance = rndCreacionSectores.Next(1000, 3000);

                Vector2 posicion = sector.BuscarEspacioVacio(diametro / 2 + DistanciaMinimaEntreThingsEnSector);

                ThingEstrella t = new ThingEstrella(galaxia, posicion, diametro);

                t.RadioAccion = radioAlcance;
            }
            else if (rndCreacionSectores.Next(0, ProbabilidadPlaneta) == 0)
            {
                //Agrego un planeta, para lo cual primero determino si va a tener o no estacion espacial,
                //ya que de tenerla, tengo que buscar en el sector un espacio lo suficientemente grande como para
                //que entre el planeta y la estacion espacial en orbita
                puedeTenerNaves = true;

                float diametroPlaneta = rndCreacionSectores.Next(100, 500);

                Vector2 tamanioEstacionEspacial = new Vector2(200, 200);
                bool tieneEstacionEspacial = (rndCreacionSectores.Next(0, ProbabilidadEstacionEspacialOrbitaPlaneta) == 0);
                float radioOrbitaEstacionEspacial = diametroPlaneta / 2.0f + tamanioEstacionEspacial.X / 2.0f + (float)rndCreacionSectores.Next(100, 1000);

                Vector2 posicionPlaneta;

                if (tieneEstacionEspacial)
                    posicionPlaneta = sector.BuscarEspacioVacio(diametroPlaneta / 2 + radioOrbitaEstacionEspacial + tamanioEstacionEspacial.X / 2 + DistanciaMinimaEntreThingsEnSector);
                else
                    posicionPlaneta = sector.BuscarEspacioVacio(diametroPlaneta / 2 + DistanciaMinimaEntreThingsEnSector);

                ThingPlaneta planeta = new ThingPlaneta(galaxia, posicionPlaneta, diametroPlaneta);

                if (tieneEstacionEspacial)
                {
                    //Agrego una estación espacial en orbita!
                    float velocidadRotacionOrbita = (float)rndCreacionSectores.Next(100, 1000) / 100.0f;

                    Vector2 posicionEstacion = planeta.Centro + new Vector2(radioOrbitaEstacionEspacial, 0);

                    ThingEstacionEspacial estacionEspacial = new ThingEstacionEspacial(galaxia, posicionEstacion, tamanioEstacionEspacial);

                    estacionEspacial.EnOrbita = true;
                    estacionEspacial.ThingCentroOrbita = planeta;
                    estacionEspacial.RadioOrbita = radioOrbitaEstacionEspacial;
                    estacionEspacial.VelocidadRotacionOrbita = velocidadRotacionOrbita;
                }
            }
            else if (rndCreacionSectores.Next(0, ProbabilidadEstacionEspacial) == 0)
            {
                //Agrego una estación espacial

                puedeTenerNaves = true;

                Vector2 posicion = sector.BuscarEspacioVacio(100 + DistanciaMinimaEntreThingsEnSector);

                ThingEstacionEspacial t = new ThingEstacionEspacial(galaxia, posicion, new Vector2(200, 200));
            }
            else if (rndCreacionSectores.Next(0, ProbabilidadAgujeroDeGusano) == 0)
            {
                //Agrego agujeros de gusano

                float diametro = rndCreacionSectores.Next(100, 500);
                float velocidadRotacion = rndCreacionSectores.Next(90, 360);

                //Busco posiciones que sean 2 veces el diametro para que quede espacio alrededor
                //del agujero de salida para que se mueva el objeto que llegue
                Vector2 posicion = sector.BuscarEspacioVacio(diametro / 2 + DistanciaMinimaEntreThingsEnSector);

                ThingAgujeroDeGusano a1 = new ThingAgujeroDeGusano(galaxia, posicion, diametro);
                Sector sectorDestino;

                a1.VelocidadRotacion = velocidadRotacion;

                while (true)
                {
                    SectorID sectorIDDestino = new SectorID(
                        rndCreacionSectores.Next(-TamanioEnSectores / 2, TamanioEnSectores / 2),
                        rndCreacionSectores.Next(-TamanioEnSectores / 2, TamanioEnSectores / 2));

                    if (galaxia.SectorCargado(sectorIDDestino) == false)
                    {
                        sectorDestino = galaxia.GetSector(sectorIDDestino);
                        break;
                    }
                }

                posicion = sectorDestino.BuscarEspacioVacio(diametro / 2 + DistanciaMinimaEntreThingsEnSector);

                ThingAgujeroDeGusano a2 = new ThingAgujeroDeGusano(galaxia, posicion, diametro);

                a1.AgujeroDestino = a2;
                a2.AgujeroDestino = a1;
            }

            if (puedeSerNebula)
            {
                int probabilidadNebula = ProbabilidadNebula;

                //Si un sector tiene una nebula cerca, tiene mas probabilidad de ser nebula

                foreach (SectorID sIDCercano in sectorID.SectoresCercanos)
                    if (galaxia.SectorCargado(sIDCercano) &&
                        galaxia.GetSector(sIDCercano).TieneNebulosa)
                    {
                        probabilidadNebula = ProbabilidadNebulaContinua;
                        break;
                    }

                if (rndCreacionSectores.Next(0, probabilidadNebula) == 0)
                    sector.CrearNebulosa();
            }

            if (puedeTenerNaves && sector.Faccion != null)
            {
                int cantidadNaves = rndCreacionSectores.Next(0, 2);

                for (int i = 0; i < cantidadNaves; i++)
                {
                    Vector2 posicion = sector.BuscarEspacioVacio(300);

                    Things.NPC.ThingNaveNPC thingNaveNPC = new Things.NPC.ThingNaveNPC(galaxia, new Vector2(100, 100), posicion, 0, sector.Faccion);
                }
            }

            return sector;
        }

        public override Size GetTamanioEnSectores()
        {
            return new Size(TamanioEnSectores, TamanioEnSectores);
        }

        public override Faccion GetFaccionJugador()
        {
            InicializarFacciones();

            return faccionJugador;
        }
    }
}
