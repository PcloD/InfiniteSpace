using System;
using System.Collections.Generic;
using System.Text;
using EspacioInfinitoDotNet.GUI.Controls;
using EspacioInfinitoDotNet.GUI;
using EspacioInfinitoDotNet.Game;
using System.Drawing;
using Tao.Sdl;

namespace EspacioInfinitoDotNet.Game
{
    class DialogoIntroduccion : GUIDialog
    {
        GUITextBox guiTextBox;

        public DialogoIntroduccion()
            : base(new Size(600, 470))
        {
            Title = "Introducción";

            BackColor = Color.FromArgb(240, Color.DarkGreen);
            TitleColor = Color.FromArgb(240, Color.Green);

            guiTextBox = new GUITextBox(new Size(InnerBounds.Size.Width - 10, InnerBounds.Size.Height - 10));

            AddChildWindow(guiTextBox, new Point(InnerBounds.Location.X + 5, InnerBounds.Location.Y + 5));

            AgregarTextoAyuda("Esto es lo que se podria considerar una versión inicial de lo que espero se convierta  en un juego al estilo NetHack y toda la familia de juegos con niveles generados al azar, y un arcade de naves.");
            AgregarTextoAyuda("Por el momento, si bien todo se genera al azar, no se salva en ningun momento, debido a lo cual es dificil continuar jugando :-), eso sin tener en cuenta que no hay \"puntos\", ni nada mas interesante que hacer que matar naves a lo loco hasta ser destruido.");
            AgregarTextoAyuda("Lo que se ve abajo a la derecha es un mapa de los sectores que rodean a la nave del jugador, en el espacio se pueden encontrar con:");
            AgregarTextoAyuda("");
            AgregarTextoAyuda("Soles (estrellas): Tienen leve atracción gravitatoria, si chocan con ellos son historia, pero los pueden destruir si disparan contra ellos, aunque aconsejo hacerlo de lejos :-)");
            AgregarTextoAyuda("");
            AgregarTextoAyuda("Agujeros Negros: Fuerte atracción gravitatoria, si te absorven te destruyen o te llevan a otro sector al azar");
            AgregarTextoAyuda("");
            AgregarTextoAyuda("Agujeros de Gusano: Comunican 2 sectors, tienen entrada y salida estable");
            AgregarTextoAyuda("");
            AgregarTextoAyuda("Planeta y Estaciones: Por el momento no hacen nada salvo estar por ahi :-)");
            AgregarTextoAyuda("");
            AgregarTextoAyuda("Otras Naves (NPC): Hay de 3 tipos, los que andan por ahi recolectando recursos de las nebulosas, los que patrullan al azar, y los que si te ven te empiezan a disparar a lo loco.. cuidado con estos.");
            AgregarTextoAyuda("");
            AgregarTextoAyuda("");
            AgregarTextoAyuda("Tienen que presionar ESCAPE para cerrar este dialogo, y con F1 pueden ver una ayuda de las teclas que controlan el juego, espero que les guste y se aceptan criticas de todo tipo :-), mandenlas a lawebdefederico@gmail.com");

            GUIButton btnAceptar = new GUIButton(new Size(60, 24));
            btnAceptar.Text = "Aceptar";
            btnAceptar.ButtonPressed += new GUIButton.ButtonPressedHandler(btnAceptar_ButtonPressed);
            AddChildWindow(btnAceptar, new Point(InnerBounds.Location.X + (InnerBounds.Width - btnAceptar.Size.Width) / 2, InnerBounds.Location.Y + (InnerBounds.Height - btnAceptar.Size.Height - 5)));

            FocusNextChild();
        }

        void btnAceptar_ButtonPressed(GUIButton button)
        {
            Close();
        }

        private void AgregarTextoAyuda(string texto)
        {
            guiTextBox.Text += texto + "\n";
        }

        public override bool HandleEvent(GUIEvent guiEvent)
        {
            bool handled = base.HandleEvent(guiEvent);

            if (guiEvent is GUIEventKeyReleased)
            {
                GUIEventKeyReleased guiEventKey = (GUIEventKeyReleased)guiEvent;

                if (guiEventKey.key == Sdl.SDLK_RETURN)
                {
                    Close();
                    handled = true;
                }
            }

            return handled;
        }
    }
}
