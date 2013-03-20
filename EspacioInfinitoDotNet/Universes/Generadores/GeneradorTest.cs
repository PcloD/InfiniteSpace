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
    public class GeneradorTest : Generador
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

        public override void Inicializar(Galaxia galaxia)
        {
            this.galaxia = galaxia;
        }

        public override string Nombre()
        {
            return "GeneradorTest";
        }

        public override string Descripcion()
        {
            return "Generador usado para test interno";
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

                    Faccion faccion = new Faccion("Faccion " + i, (byte)i, tipoFaccion, Color.White);

                    facciones.Add(faccion);
                }

                faccionJugador = facciones[0];
            }

            return facciones.ToArray();
        }

        public override EspacioInfinitoDotNet.Maths.Vector2 GetPosicionInicialJugador()
        {
            Vector2 posicionInicialJugador;

            posicionInicialJugador = new Vector2(Sector.TamanioSector * 4, Sector.TamanioSector * 4);

            return posicionInicialJugador;
        }

        public override Sector CrearSector(SectorID sectorID)
        {
            Sector sector = new Sector(galaxia, sectorID, null);

            galaxia.AgregarSector(sector);

            if (sectorID.X == 4 && sectorID.Y == 3)
            {
                ThingAgujeroDeGusano ta1 = new ThingAgujeroDeGusano(galaxia, sector.Centro +
                    new Vector2(0, Sector.TamanioSector / 2 - 1 - 300), 300);
                ThingAgujeroDeGusano ta2 = new ThingAgujeroDeGusano(galaxia, galaxia.GetSector(new SectorID(4, 5)).Centro +
                    new Vector2(0, -Sector.TamanioSector / 2 + 1 - 300), 300);

                ta1.AgujeroDestino = ta2;
                ta2.AgujeroDestino = ta1;
            }
            else if (sectorID.X == 4 && sectorID.Y == 4)
            {
                new ThingPlaneta(galaxia, sector.Centro +
                    new Vector2(Sector.TamanioSector / 2 - 1, Sector.TamanioSector / 2 - 1), 300);

                new ThingPlaneta(galaxia, sector.Centro +
                    new Vector2(-Sector.TamanioSector / 2 + 1, -Sector.TamanioSector / 2 + 1), 300);

                new ThingPlaneta(galaxia, sector.Centro +
                    new Vector2(Sector.TamanioSector / 2 - 1, -Sector.TamanioSector / 2 + 1), 300);

                new ThingPlaneta(galaxia, sector.Centro +
                    new Vector2(-Sector.TamanioSector / 2 + 1, Sector.TamanioSector / 2 - 1), 300);

                new ThingPlaneta(galaxia, sector.Centro +
                    new Vector2(-Sector.TamanioSector / 2 + 1, 0), 300);

                new ThingPlaneta(galaxia, sector.Centro +
                    new Vector2(+Sector.TamanioSector / 2 - 1, 0), 300);

                new ThingPlaneta(galaxia, sector.Centro +
                    new Vector2(0, -Sector.TamanioSector / 2 + 1), 300);

                new ThingPlaneta(galaxia, sector.Centro +
                    new Vector2(0, +Sector.TamanioSector / 2 - 1), 300);
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
