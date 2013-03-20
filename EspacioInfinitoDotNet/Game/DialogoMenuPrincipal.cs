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
    class DialogoMenuPricipal : GUIDialog
    {
        public DialogoMenuPricipal()
            : base(new Size(170, 290))
        {
            Title = "Menú Principal";

            BackColor = Color.FromArgb(240, Color.DarkGray);
            TitleColor = Color.FromArgb(240, Color.Gray);

            AddMenuEntry("Iniciar Nuevo Juego", new GUIButton.ButtonPressedHandler(reiniciar_Pressed));
            AddMenuEntry("Introducción", new GUIButton.ButtonPressedHandler(introduccion_Pressed));
            AddMenuEntry("Controles", new GUIButton.ButtonPressedHandler(ayuda_Pressed));
            AddMenuEntry("Configuración", new GUIButton.ButtonPressedHandler(configuracion_Pressed));
            AddMenuEntry("Cerrar Menú", new GUIButton.ButtonPressedHandler(cerrarMenu_Pressed));
            offsetY += 30;
            AddMenuEntry("Salir", new GUIButton.ButtonPressedHandler(salir_Pressed));

            FocusNextChild();
        }

        private int offsetY = 6;

        private void AddMenuEntry(string texto, GUIButton.ButtonPressedHandler handler)
        {
            GUIButton btn = new GUIButton(new Size(InnerBounds.Width - 30, 30));
            btn.Text = texto;
            AddChildWindow(btn, new Point(InnerBounds.Location.X + 15, InnerBounds.Location.Y + offsetY));
            btn.ButtonPressed += handler;

            offsetY += 36;
        }

        void cerrarMenu_Pressed(GUIButton btn)
        {
            Close();
        }

        void reiniciar_Pressed(GUIButton btn)
        {
            DialogoReiniciar guiDialogoReiniciar = new DialogoReiniciar(null, false);

            Father.AddChildWindow(
                guiDialogoReiniciar,
                new Point(
                    (Father.Size.Width - guiDialogoReiniciar.Size.Width) / 2,
                    (Father.Size.Height - guiDialogoReiniciar.Size.Height) / 2));

            Father.Focus = guiDialogoReiniciar;

            Close();
        }

        void configuracion_Pressed(GUIButton btn)
        {
            GUI.Controls.GUIDialog guiConfiguracion = new DialogoConfiguracion();

            Father.AddChildWindow(
                guiConfiguracion,
                new Point(
                    (Father.Size.Width - guiConfiguracion.Size.Width) / 2,
                    (Father.Size.Height - guiConfiguracion.Size.Height) / 2));

            Father.Focus = guiConfiguracion;
        }

        void ayuda_Pressed(GUIButton btn)
        {
            GUI.Controls.GUIDialog guiAyuda = new DialogoAyuda();

            Father.AddChildWindow(
                guiAyuda,
                new Point(
                    (Father.Size.Width - guiAyuda.Size.Width) / 2,
                    (Father.Size.Height - guiAyuda.Size.Height) / 2));

            Father.Focus = guiAyuda;
        }

        void introduccion_Pressed(GUIButton btn)
        {
            GUI.Controls.GUIDialog guiIntroduccion = new DialogoIntroduccion();

            Father.AddChildWindow(
                guiIntroduccion,
                new Point(
                    (Father.Size.Width - guiIntroduccion.Size.Width) / 2,
                    (Father.Size.Height - guiIntroduccion.Size.Height) / 2));

            Father.Focus = guiIntroduccion;
        }

        void salir_Pressed(GUIButton btn)
        {
            GUI.Controls.GUIDialog guiDialogoSalir = new DialogoSalir();

            Father.AddChildWindow(
                guiDialogoSalir,
                new Point(
                    (Father.Size.Width - guiDialogoSalir.Size.Width) / 2,
                    (Father.Size.Height - guiDialogoSalir.Size.Height) / 2));

            Father.Focus = guiDialogoSalir;
        }

        public override bool HandleEvent(GUIEvent guiEvent)
        {
            bool handled = base.HandleEvent(guiEvent);

            if (guiEvent is GUIEventKeyReleased)
            {
                GUIEventKeyReleased guiEventKey = (GUIEventKeyReleased)guiEvent;

                if (guiEventKey.key == Sdl.SDLK_ESCAPE)
                {
                    Close();
                    handled = true;
                }
            }

            return handled;
        }
    }
}
