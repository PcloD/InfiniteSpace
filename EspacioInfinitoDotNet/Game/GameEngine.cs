using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Tao.OpenGl;
using Tao.Sdl;
using System.Drawing.Imaging;
using EspacioInfinitoDotNet.Things;
using EspacioInfinitoDotNet.Universes;
using EspacioInfinitoDotNet.Maths;
using EspacioInfinitoDotNet.Graphics;

namespace EspacioInfinitoDotNet.Game
{
    public class GameEngine
    {
        public const int MaximoProcesosPorSegundo = -1;
        public const float ZoomMaximo = -6000.0f;
        public const float ZoomMinimo = -200.0f;
        public const float ZoomInicial = -2000.0f;
        public const float VelocidadZoom = 2000.0f;
        public const int RadioSectoresCercanosAProcesar = 10;
        public const int ToleranciaJoytstick = 2000;

        #region Atributos

        static GameEngine instance;

        bool ignorarTeclas = false;
        bool pausa = false;
        bool reiniciar = false;
        bool enDemo = false;
        bool salir = false;

        long ultimoTicks = -1;
        float segundosTranscurridos = 0.0f;
        float segundosDemoEnElMismoThing = 0.0f;

        long ticksInicioCalculoFPS;
        float ultimoFps;
        int cuadrosDibujados = 0;
        
        float zoom = ZoomInicial;
        bool mostrarInfoDebug = false;
        bool dibujarCapaFrente = true;

        GUI.GUIEngine guiEngine;
        GUI.Controls.GUIStatic guiContadorFPS;
        GUI.Controls.GUIMapa guiMapa;
        GUI.Controls.GUIStatic guiAvisoDemo;

        bool[] teclasPresionadas = new bool[Sdl.SDLK_LAST];

        Galaxia galaxia;
        ThingNaveJugador naveJugador;
        Thing thingASeguir;

        static public GameEngine Instance
        {
            get { return instance; }
        }

        public bool IgnorarTeclas
        {
            get { return ignorarTeclas; }
            set { ignorarTeclas = value; }
        }

        public bool Pausa
        {
            get { return pausa; }
            set { pausa = value; }
        }

        public bool Salir
        {
            get { return salir; }
            set { this.salir = value; }
        }

        public bool Reiniciar
        {
            get { return reiniciar; }
            set { this.reiniciar = value; }
        }

        public Thing ThingASeguir
        {
            get { return thingASeguir; }
        }

        #endregion

        public GameEngine()
        {
            instance = this;
        }

        #region Inicialización

        private void Inicializar()
        {
            guiEngine = new GUI.GUIEngine();

            guiEngine.Init(new GUI.GUIGraphicEngine(GraphicEngine.Instance));

            //Inicializo la interfaz gráfica

            //Contador de FPS
            guiContadorFPS = new EspacioInfinitoDotNet.GUI.Controls.GUIStatic(new Size(0, 0));
            guiEngine.Root.AddChildWindow(guiContadorFPS, new Point(0, 0));

            //Mapa
            guiMapa = new GUI.Controls.GUIMapa(new Size(
                GraphicEngine.Instance.Size.Width * 3 / 10,
                GraphicEngine.Instance.Size.Height * 3 / 10));

            guiEngine.Root.AddChildWindow(guiMapa, new Point(
                GraphicEngine.Instance.Size.Width * 7 / 10 - 10,
                GraphicEngine.Instance.Size.Height * 7 / 10 - 10));

            RecargarConfiguracion();
        }

        private void InicializarGalaxia(bool demo)
        {
            Universes.Generadores.Generador[] generadores = Universes.Generadores.Generador.GeneradoresDisponibles();
            Universes.Generadores.Generador generadorAUsar = null;

            if (!demo)
            {
                enDemo = false;
                guiMapa.DrawEnabled = true;
                string nombreGenerador = Properties.Settings.Default.GeneradorAUsar.Trim();

                foreach (Universes.Generadores.Generador generador in generadores)
                    if (generador.Nombre().Equals(nombreGenerador, StringComparison.InvariantCultureIgnoreCase))
                    {
                        generadorAUsar = generador;
                        break;
                    }

                if (generadorAUsar == null)
                    generadorAUsar = generadores[0];

                galaxia = Galaxia.Crear(generadorAUsar);
                naveJugador = new ThingNaveJugador(galaxia, new Vector2(100.0f, 100.0f), galaxia.PosicionInicialJugador, 0.0f, galaxia.FaccionJugador);
                thingASeguir = naveJugador;

                if (guiAvisoDemo != null)
                {
                    guiEngine.Root.RemoveChildWindow(guiAvisoDemo);
                    guiAvisoDemo = null;
                }
            }
            else
            {
                enDemo = true;
                guiMapa.DrawEnabled = true;
                generadorAUsar = new Universes.Generadores.GeneradorDemo();

                galaxia = Galaxia.Crear(generadorAUsar);
                naveJugador = null;
                thingASeguir = null;

                if (guiAvisoDemo == null)
                {
                    guiAvisoDemo = new EspacioInfinitoDotNet.GUI.Controls.GUIStatic(new Size(GraphicEngine.Instance.Size.Width, 40));
                    guiAvisoDemo.Text = "JUEGO EN MODO DEMO - PRESIONE ESCAPE Y SELECCIONE NUEVO JUEGO PARA JUGAR";
                    guiAvisoDemo.TextColor = Color.Red;
                    guiAvisoDemo.AutoFit = false;
                    guiAvisoDemo.CenterHorizontally = true;
                    guiAvisoDemo.CenterVertically = true;

                    guiEngine.Root.AddChildWindow(guiAvisoDemo, new Point(0, 100));
                }
            }
        }

        private void InicializarJoystick()
        {
            if (Sdl.SDL_NumJoysticks() > 0)
            {
                //Hay joysticks! los inicializo

                Sdl.SDL_JoystickOpen(0);
                Sdl.SDL_JoystickEventState(Sdl.SDL_ENABLE);
            }
        }

        #endregion

        #region Ciclo Principal

        private bool pasarTeclasAGUI = false;

        public void Ejecutar()
        {
            Size resolucion = Properties.Settings.Default.Resolucion;
            int profunidadBits = Properties.Settings.Default.ProfundidadBits;
            bool pantallaCompleta = Properties.Settings.Default.PantallaCompleta;

            if (GraphicEngine.Instance.Inicializar(
                    "Espacio Infinito",
                    resolucion.Width, resolucion.Height,
                    profunidadBits,
                    pantallaCompleta))
            {
                Inicializar();

                InicializarJoystick();

                InicializarGalaxia(true);

                DialogoIntroduccion guiDialogoInfo = new DialogoIntroduccion();

                guiEngine.Root.AddChildWindow(
                    guiDialogoInfo,
                    new Point(
                        (guiEngine.Root.Size.Width - guiDialogoInfo.Size.Width) / 2,
                        (guiEngine.Root.Size.Height - guiDialogoInfo.Size.Height) / 2));

                guiEngine.Root.Focus = guiDialogoInfo;

                Sdl.SDL_Event evt;

                Sdl.SDL_EnableKeyRepeat(Sdl.SDL_DEFAULT_REPEAT_INTERVAL, Sdl.SDL_DEFAULT_REPEAT_DELAY); 

                while (!salir)
                {
                    if (reiniciar)
                    {
                        InicializarGalaxia(false);

                        reiniciar = false;
                    }

                    //Primero consumo todos los eventos pendientes en la cola de eventos de SDL
                    Sdl.SDL_PollEvent(out evt);

                    do
                    {
                        //Determino si pasarle los eventos a la GUI o al GameEngine

                        bool hayDialogosEnGUI = false;

                        foreach (GUI.GUIWindow wnd in guiEngine.Root.Childs)
                            if (wnd is GUI.Controls.GUIDialog)
                            {
                                hayDialogosEnGUI = true;
                                break;
                            }

                        if (hayDialogosEnGUI)
                        {
                            pasarTeclasAGUI = true;
                            if (!enDemo)
                                Pausa = true;
                            else
                                Pausa = false;
                        }
                        else
                        {
                            pasarTeclasAGUI = false;
                            Pausa = false;
                        }

                        //Despacho los eventos
                        switch (evt.type)
                        {
                            case Sdl.SDL_QUIT:
                                salir = true;
                                break;

                            case Sdl.SDL_KEYDOWN:
                            case Sdl.SDL_KEYUP:
                                ProcesarEventoKey(evt.key.keysym.sym, evt.key.state == Sdl.SDL_PRESSED);
                                break;

                            case Sdl.SDL_JOYBUTTONDOWN:
                            case Sdl.SDL_JOYBUTTONUP:
                                if (!pasarTeclasAGUI)
                                {
                                    if (evt.jbutton.button == 0)
                                        ProcesarEventoKey(Sdl.SDLK_SPACE, evt.jbutton.state == Sdl.SDL_PRESSED);
                                    else if (evt.jbutton.button == 1)
                                        ProcesarEventoKey(Sdl.SDLK_ESCAPE, evt.jbutton.state == Sdl.SDL_PRESSED);
                                }
                                else
                                {
                                    if (evt.jbutton.button == 0)
                                        ProcesarEventoKey(Sdl.SDLK_RETURN, evt.jbutton.state == Sdl.SDL_PRESSED);
                                    else if (evt.jbutton.button == 1)
                                        ProcesarEventoKey(Sdl.SDLK_ESCAPE, evt.jbutton.state == Sdl.SDL_PRESSED);
                                }
                                break;

                            case Sdl.SDL_JOYAXISMOTION:
                                if (evt.jaxis.axis == 0)
                                {
                                    //Movimiento en X

                                    if (evt.jaxis.val > ToleranciaJoytstick)
                                    {
                                        ProcesarEventoKey(Sdl.SDLK_LEFT, false);
                                        ProcesarEventoKey(Sdl.SDLK_RIGHT, true);
                                    }
                                    else if (evt.jaxis.val < -ToleranciaJoytstick)
                                    {
                                        ProcesarEventoKey(Sdl.SDLK_LEFT, true);
                                        ProcesarEventoKey(Sdl.SDLK_RIGHT, false);
                                        
                                    }
                                    else
                                    {
                                        ProcesarEventoKey(Sdl.SDLK_LEFT, false);
                                        ProcesarEventoKey(Sdl.SDLK_RIGHT, false);
                                    }
                                }
                                else if (evt.jaxis.axis == 1)
                                {
                                    //Movimiento en Y

                                    if (evt.jaxis.val > ToleranciaJoytstick)
                                    {
                                        ProcesarEventoKey(Sdl.SDLK_DOWN, true);
                                        ProcesarEventoKey(Sdl.SDLK_UP, false);
                                    }
                                    else if (evt.jaxis.val < -ToleranciaJoytstick)
                                    {
                                        ProcesarEventoKey(Sdl.SDLK_DOWN, false);
                                        ProcesarEventoKey(Sdl.SDLK_UP, true);
                                    }
                                    else
                                    {
                                        ProcesarEventoKey(Sdl.SDLK_DOWN, false);
                                        ProcesarEventoKey(Sdl.SDLK_UP, false);
                                    }
                                }

                                break;
                        }
                    } while (Sdl.SDL_PollEvent(out evt) == 1);

                    //Ahora realizo el proceso del Game (Dibujar, etc..)
                    Procesar();
                }

                GraphicEngine.Instance.Finalizar();
            }
            else
            {
                Properties.Settings.Default.Reset();
                Properties.Settings.Default.Save();
                MessageBox.Show("No se pudo iniciar el modo gráfico.\nLa configuración fue reseteada, pruebe iniciar el juego nuevamente", "Fallo al iniciar el modo gráfico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProcesarEventoKey(int tecla, bool presionado)
        {
            if (teclasPresionadas[tecla] != presionado)
            {
                teclasPresionadas[tecla] = presionado;

                if (!pasarTeclasAGUI)
                {
                    if (presionado)
                        OnTeclaPresionada(tecla);
                    else
                        OnTeclaSoltada(tecla);
                }
                else
                {
                    if (presionado)
                    {
                        GUI.GUIEventKeyPressed guiEvent = new EspacioInfinitoDotNet.GUI.GUIEventKeyPressed();
                        guiEvent.key = tecla;
                        guiEngine.AddEvent(guiEvent);
                    }
                    else
                    {
                        GUI.GUIEventKeyReleased guiEvent = new EspacioInfinitoDotNet.GUI.GUIEventKeyReleased();
                        guiEvent.key = tecla;
                        guiEngine.AddEvent(guiEvent);
                    }
                }
            }
        }

        private void Procesar()
        {
            Dibujar();

            if (ActualizarTimers())
            {
                if (!Pausa)
                    ProcesarThings();

                if (!IgnorarTeclas)
                    ProcesarTeclas();
            }
        }

        #endregion

        #region Dibujar

        private void Dibujar()
        {
            GraphicEngine.Instance.IniciarDibujado();

            if (thingASeguir != null)
            {
                Gl.glTranslatef(-thingASeguir.Centro.X, -thingASeguir.Centro.Y, zoom);

                Graphics.Frustum frustum = Graphics.GraphicEngine.Instance.GetFrustum();

                Sector[] sectoresVisibles = galaxia.GetSectoresVisibles(thingASeguir.Centro, frustum);

                foreach (Sector sector in sectoresVisibles)
                    sector.DibujarFondo();

                foreach (Sector sector in sectoresVisibles)
                    foreach (Thing thing in sector.GetThingsVisibles(frustum))
                    {
                        thing.Dibujar();

                        if (mostrarInfoDebug && thing.Solido)
                            DibujarBoundingObjects(thing);
                    }

                if (dibujarCapaFrente)
                    foreach (Sector sector in sectoresVisibles)
                        sector.DibujarFrente();

                if (mostrarInfoDebug)
                    foreach (Sector sector in sectoresVisibles)
                        GraphicEngine.Instance.DrawRectangleBorder(sector.Centro, 0, sector.Tamanio, Color.White);
            }

            DibujarGUI();

            cuadrosDibujados++;

            GraphicEngine.Instance.FinalizarDibujado();
        }

        private void DibujarBoundingObjects(Thing thing)
        {
            BoundingObject[] boundingObjects = (BoundingObject[])thing.CrearBoundingObjects().Clone();

            for (int i = 0; i < boundingObjects.Length; i++)
                boundingObjects[i] = boundingObjects[i].RotateAndTranslate(thing.RotacionEnGrados, thing.Centro);

            foreach (BoundingObject bo in boundingObjects)
            {
                if (bo is BoundingCircle)
                {
                    BoundingCircle boc = (BoundingCircle)bo;

                    Gl.glDisable(Gl.GL_TEXTURE_2D);

                    Gl.glColor4f(1.0f, 1.0f, 1.0f, 1.0f);

                    Gl.glTranslatef(boc.Center.X, boc.Center.Y, GraphicEngine.zValue);

                    Gl.glBegin(Gl.GL_LINE_STRIP);

                    Gl.glVertex3f(0, 0, 0);

                    for (int i = 0; i <= 360; i += 30)
                    {
                        float degInRad = (float) (i * Math.PI / 180);
                        Gl.glVertex3f((float) Math.Cos(degInRad) * boc.Radius, (float) Math.Sin(degInRad) * boc.Radius, GraphicEngine.zValue);
                    }

                    Gl.glEnd();

                    Gl.glTranslatef(-boc.Center.X, -boc.Center.Y, -GraphicEngine.zValue);

                    Gl.glEnable(Gl.GL_TEXTURE_2D);
                }
                else if (bo is BoundingRectangle)
                {
                    BoundingRectangle bor = (BoundingRectangle)bo;

                    Gl.glDisable(Gl.GL_TEXTURE_2D);

                    Gl.glColor4f(1.0f, 1.0f, 1.0f, 1.0f);

                    Gl.glBegin(Gl.GL_LINE_STRIP);

                    for (int i = 0; i < 5; i++)
                        Gl.glVertex3f(bor.Puntos[i % 4].X, bor.Puntos[i % 4].Y, GraphicEngine.zValue);

                    Gl.glEnd();

                    Gl.glEnable(Gl.GL_TEXTURE_2D);
                }
            }
        }

        private void DibujarGUI()
        {
            guiContadorFPS.Text = "FPS: " + ultimoFps.ToString("0.00");

            guiEngine.Draw();
        }

        #endregion

        #region Manejo de Timers

        private bool ActualizarTimers()
        {
            bool procesar = false;

            if (ultimoTicks == -1)
            {
                ultimoTicks = DateTime.Now.Ticks;
                segundosTranscurridos = 0.0f;

                ticksInicioCalculoFPS = ultimoTicks;
                cuadrosDibujados = 0;
                ultimoFps = 0.0f;
            }
            else
            {
                long newTicks = DateTime.Now.Ticks;

                if (MaximoProcesosPorSegundo == -1)
                {
                    //No se limita la cantidad de procesamientos por segundo
                    segundosTranscurridos = (newTicks - ultimoTicks) / 10000000.0f;
                    ultimoTicks = newTicks;
                    procesar = true;
                }
                else
                {
                    //Se limita la cantidad de procesamientos por segundo
                    if (newTicks - ultimoTicks > 10000000 / MaximoProcesosPorSegundo)
                    {
                        segundosTranscurridos =  1.0f / MaximoProcesosPorSegundo;
                        ultimoTicks = newTicks;
                        procesar = true;
                    }
                }

                if (newTicks - ticksInicioCalculoFPS > 10000000)
                {
                    ultimoFps = cuadrosDibujados / ((newTicks - ticksInicioCalculoFPS) / 10000000.0f);
                    ticksInicioCalculoFPS = newTicks;
                    cuadrosDibujados = 0;
                    //this.Text = "FPS: " + lastFps.ToString("0.00");
                }
            }

            if (segundosTranscurridos > 0.1f)
                segundosTranscurridos = 0.1f;

            return procesar;
        }

        #endregion

        #region Actualización de objetos

        private void ProcesarThings()
        {
            if (thingASeguir != null)
                galaxia.Procesar(segundosTranscurridos, thingASeguir.Centro, RadioSectoresCercanosAProcesar);

            if (!enDemo)
            {
                if (naveJugador.Eliminado == true)
                {
                    DialogoReiniciar guiDialogoReiniciar = new DialogoReiniciar("Fuiste destruido.", true);

                    guiEngine.Root.AddChildWindow(
                        guiDialogoReiniciar,
                        new Point(
                            (guiEngine.Root.Size.Width - guiDialogoReiniciar.Size.Width) / 2,
                            (guiEngine.Root.Size.Height - guiDialogoReiniciar.Size.Height) / 2));

                    guiEngine.Root.Focus = guiDialogoReiniciar;
                }
            }
            else
            {
                segundosDemoEnElMismoThing += segundosTranscurridos;

                if (segundosDemoEnElMismoThing > 30.0f || thingASeguir == null || thingASeguir.Eliminado)
                {
                    segundosDemoEnElMismoThing = 0.0f;

                    //Busco un nuevo thing a seguir al azar

                    System.Random rnd = new System.Random((int)DateTime.Now.Ticks);
                    thingASeguir = null;

                    int intentos = 30;

                    while (thingASeguir == null && intentos > 0)
                    {
                        int x = rnd.Next(-galaxia.TamanioEnSectores.Width / 2, galaxia.TamanioEnSectores.Width / 2);
                        int y = rnd.Next(-galaxia.TamanioEnSectores.Height / 2, galaxia.TamanioEnSectores.Height / 2);

                        Sector sector = galaxia.GetSector(new SectorID(x, y));

                        foreach (Thing thing in sector.Things)
                        {
                            if (thing is ThingNave)
                            {
                                thingASeguir = thing;
                                break;
                            }
                        }

                        intentos--;
                    }
                }
            }
        }

        #endregion

        #region Manejo del teclado

        private bool TeclaPresionada(int key)
        {
            return teclasPresionadas[key];
        }

        float velocidadRotacionInicial = 180.0f;
        float velocidadRotacionFinal = 360.0f;
        float velocidadRotacion = 180.0f;
        float tiempoAceleracion = 0.5f;
        float tiempoRestanteAceleracion = 0.5f;
        bool formaControlRealista = true;

        public void RecargarConfiguracion()
        {
            velocidadRotacionInicial = velocidadRotacion = Properties.Settings.Default.VelocidadRotacionInicial;
            velocidadRotacionFinal = Properties.Settings.Default.VelocidadRotacionFinal;
            tiempoAceleracion = tiempoRestanteAceleracion = Properties.Settings.Default.TiempoAceleracionRotacion;
            formaControlRealista = Properties.Settings.Default.FormaDeControl.Equals("Realista");
        }

        private void ProcesarTeclas()
        {
            if (!pausa && !enDemo)
            {
                //Solo proceso las teclas de movimiento de jugador cuando el juego no esta
                //en pausa

                if (formaControlRealista)
                {
                    if (TeclaPresionada(Sdl.SDLK_LEFT) ||
                        TeclaPresionada(Sdl.SDLK_RIGHT))
                    {
                        if (tiempoRestanteAceleracion > 0.0f)
                        {
                            tiempoRestanteAceleracion -= segundosTranscurridos;

                            if (tiempoRestanteAceleracion <= 0.0f)
                            {
                                tiempoRestanteAceleracion = 0.0f;
                                velocidadRotacion = velocidadRotacionFinal;
                            }
                            else
                            {
                                float porcentajeRestante = (tiempoAceleracion - tiempoRestanteAceleracion) / tiempoAceleracion;

                                //Uso una función exponencial para calcular la aceleración de la rotacion, se siene mas natural y es mas práctico para
                                //apuntar de lejos
                                float porcentajeAceleracion = (float)Math.Pow(10, -(1.0f - porcentajeRestante));
                                //float porcentajeAceleracion = porcentajeRestante;

                                velocidadRotacion = velocidadRotacionInicial + (velocidadRotacionFinal - velocidadRotacionInicial) * porcentajeAceleracion;
                            }
                        }
                        else
                        {
                            velocidadRotacion = velocidadRotacionFinal;
                        }

                        if (TeclaPresionada(Sdl.SDLK_LEFT))
                            naveJugador.Rotar(velocidadRotacion * segundosTranscurridos, true, true);
                        else if (TeclaPresionada(Sdl.SDLK_RIGHT))
                            naveJugador.Rotar(-velocidadRotacion * segundosTranscurridos, true, true);
                    }
                    else
                    {
                        velocidadRotacion = velocidadRotacionInicial;
                        tiempoRestanteAceleracion = tiempoAceleracion;
                    }

                    if (TeclaPresionada(Sdl.SDLK_UP))
                    {
                        naveJugador.MoverAdelante(naveJugador.Velocidad * segundosTranscurridos, true, true);
                    }

                    if (TeclaPresionada(Sdl.SDLK_DOWN))
                    {
                        naveJugador.MoverAdelante(-naveJugador.Velocidad * segundosTranscurridos, true, true);
                    }
                }
                else
                {
                    //Proceso los controles tipo arcade

                    if (TeclaPresionada(Sdl.SDLK_LEFT) ||
                        TeclaPresionada(Sdl.SDLK_RIGHT) ||
                        TeclaPresionada(Sdl.SDLK_UP) ||
                        TeclaPresionada(Sdl.SDLK_DOWN))
                    {
                        float anguloRotacion = -1;

                        if (TeclaPresionada(Sdl.SDLK_UP) && !TeclaPresionada(Sdl.SDLK_DOWN))
                        {
                            if (!TeclaPresionada(Sdl.SDLK_LEFT) && !TeclaPresionada(Sdl.SDLK_RIGHT))
                                anguloRotacion = 90; //Arriba
                            else if (TeclaPresionada(Sdl.SDLK_LEFT) && !TeclaPresionada(Sdl.SDLK_RIGHT))
                                anguloRotacion = 135;//Arriba Izquierda
                            else if (!TeclaPresionada(Sdl.SDLK_LEFT) && TeclaPresionada(Sdl.SDLK_RIGHT))
                                anguloRotacion = 45;//Arriba Derecha
                        }
                        else if (!TeclaPresionada(Sdl.SDLK_UP) && TeclaPresionada(Sdl.SDLK_DOWN))
                        {
                            if (!TeclaPresionada(Sdl.SDLK_LEFT) && !TeclaPresionada(Sdl.SDLK_RIGHT))
                                anguloRotacion = 270; //Abajo
                            else if (TeclaPresionada(Sdl.SDLK_LEFT) && !TeclaPresionada(Sdl.SDLK_RIGHT))
                                anguloRotacion = 225;//Abajo izquierda
                            else if (!TeclaPresionada(Sdl.SDLK_LEFT) && TeclaPresionada(Sdl.SDLK_RIGHT))
                                anguloRotacion = 315;//Abajo derecha
                        }
                        else if (!TeclaPresionada(Sdl.SDLK_UP) && !TeclaPresionada(Sdl.SDLK_DOWN) &&
                            !TeclaPresionada(Sdl.SDLK_LEFT) && TeclaPresionada(Sdl.SDLK_RIGHT))
                        {
                            anguloRotacion = 0; //Derecha
                        }
                        else if (!TeclaPresionada(Sdl.SDLK_UP) && !TeclaPresionada(Sdl.SDLK_DOWN) &&
                            TeclaPresionada(Sdl.SDLK_LEFT) && !TeclaPresionada(Sdl.SDLK_RIGHT))
                        {
                            anguloRotacion = 180; //Izquierda
                        }

                        if (anguloRotacion != -1)
                        {
                            naveJugador.Rotar(anguloRotacion - naveJugador.RotacionEnGrados, true, true);
                            naveJugador.MoverAdelante(naveJugador.Velocidad * segundosTranscurridos, true, true);
                        }
                    }
                }

                if (TeclaPresionada(Sdl.SDLK_SPACE))
                {
                    naveJugador.Shoot();
                }
            }

            if (TeclaPresionada(Sdl.SDLK_PLUS) ||
                TeclaPresionada(Sdl.SDLK_KP_PLUS))
            {
                zoom += VelocidadZoom * segundosTranscurridos;

                if (zoom > ZoomMinimo)
                    zoom = ZoomMinimo;
            }

            if (TeclaPresionada(Sdl.SDLK_MINUS) ||
                TeclaPresionada(Sdl.SDLK_KP_MINUS))
            {
                zoom -= VelocidadZoom * segundosTranscurridos;

                if (zoom < ZoomMaximo)
                    zoom = ZoomMaximo;
            }
        }

        private void OnTeclaPresionada(int tecla)
        {
            if (tecla == Sdl.SDLK_d)
                mostrarInfoDebug = !mostrarInfoDebug;

            if (tecla == Sdl.SDLK_n)
                dibujarCapaFrente = !dibujarCapaFrente;

            if (tecla == Sdl.SDLK_m)
                guiMapa.DrawEnabled = !guiMapa.DrawEnabled;

            if (tecla == Sdl.SDLK_p)
                pausa = !pausa;
        }

        private void OnTeclaSoltada(int tecla)
        {
            if (tecla == Sdl.SDLK_ESCAPE)
            {
                GUI.Controls.GUIDialog guiDialogo = new DialogoMenuPricipal();

                guiEngine.Root.AddChildWindow(
                    guiDialogo,
                    new Point(
                        (guiEngine.Root.Size.Width - guiDialogo.Size.Width) / 2,
                        (guiEngine.Root.Size.Height - guiDialogo.Size.Height) / 2));

                guiEngine.Root.Focus = guiDialogo;
            }

            if (tecla == Sdl.SDLK_F1)
            {
                DialogoAyuda guiDialogo = new DialogoAyuda();

                guiEngine.Root.AddChildWindow(
                    guiDialogo,
                    new Point(
                        (guiEngine.Root.Size.Width - guiDialogo.Size.Width) / 2,
                        (guiEngine.Root.Size.Height - guiDialogo.Size.Height) / 2));

                guiEngine.Root.Focus = guiDialogo;
            }
        }

        #endregion
    }
}
