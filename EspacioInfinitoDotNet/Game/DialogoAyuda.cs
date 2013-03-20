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
    class DialogoAyuda : GUIDialog
    {
        GUITextBox guiTextBox;

        public DialogoAyuda()
            : base(new Size(330, 380))
        {
            Title = "Controles";

            BackColor = Color.FromArgb(240, Color.DarkGreen);
            TitleColor = Color.FromArgb(240, Color.Green);

            guiTextBox = new GUITextBox(new Size(InnerBounds.Size.Width - 10, InnerBounds.Size.Height - 40));

            AddChildWindow(guiTextBox, new Point(InnerBounds.Location.X + 5, InnerBounds.Location.Y + 5));

            AgregarTextoAyuda("F1 - Controles");
            AgregarTextoAyuda("");
            AgregarTextoAyuda("FLECHA ARRIBA     - Avanzar");
            AgregarTextoAyuda("FLECHA ABAJO      - Retroceder");
            AgregarTextoAyuda("FLECHA DERECHA    - Girar a la Derecha");
            AgregarTextoAyuda("FLECHA IZQUIERDA  - Girar a la Izquierda");
            AgregarTextoAyuda("BARRA ESPACIADORA - Disparar");
            AgregarTextoAyuda("");
            AgregarTextoAyuda("+/- del Teclado Numerico - Zoom In / Out");
            AgregarTextoAyuda("D - No Dibujar / Dibujar separacion de Sectores");
            AgregarTextoAyuda("N - No dibujar / Dibujar la capa de las nebulosas");
            AgregarTextoAyuda("M - Ocultar / Mostrar el mini mapa");
            AgregarTextoAyuda("");
            AgregarTextoAyuda("Tambien se puede controlar con el joystick, usandolo en modo digital y con los botones 1 y 2.");
            AgregarTextoAyuda("El joystick debe estar conectado antes de iniciar el juego.");
            AgregarTextoAyuda("");
            AgregarTextoAyuda("ESCAPE - Salir o Cerrar dialogos");

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

                if (guiEventKey.key == Sdl.SDLK_F1)
                {
                    Close();
                    handled = true;
                }
            }

            return handled;
        }
    }
}
