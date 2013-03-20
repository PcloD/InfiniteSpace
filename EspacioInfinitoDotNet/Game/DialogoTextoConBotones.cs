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
    class DialogoTextoConBotones : GUIDialog
    {
        GUIStatic guiMensaje;

        public DialogoTextoConBotones(Size size, string titulo, string mensaje, int botonPorDefecto, params string[] textosBotones)
            : base(size)
        {
            Title = titulo;

            BackColor = Color.FromArgb(240, Color.DarkBlue);
            TitleColor = Color.FromArgb(240, Color.Blue);

            int anchoBoton = 50;
            int altoBoton = 26;

            guiMensaje = new GUIStatic(new Size(InnerBounds.Size.Width, InnerBounds.Size.Height - altoBoton));
            guiMensaje.AutoFit = false;
            guiMensaje.CenterHorizontally = true;
            guiMensaje.CenterVertically = true;

            guiMensaje.Text = mensaje;

            AddChildWindow(guiMensaje, InnerBounds.Location);

            int espacioEntreBotones = (Size.Width - (InnerBounds.X + anchoBoton * textosBotones.Length)) / (textosBotones.Length + 1);

            Point posicionBoton = new Point(
                espacioEntreBotones,
                Size.Height - GUI_DIALOG_BORDER_SIZE - altoBoton - 5);

            int n = 0;

            foreach (String textoBoton in textosBotones)
            {
                GUIButton boton = new GUIButton(new Size(anchoBoton, altoBoton));
                boton.Text = textoBoton;
                boton.ButtonPressed += new GUIButton.ButtonPressedHandler(boton_ButtonPressed);

                AddChildWindow(boton, posicionBoton);

                if (n == botonPorDefecto)
                    Focus = boton;

                posicionBoton.X += anchoBoton + espacioEntreBotones;
                n++;
            }

            if (Focus == null)
                FocusNextChild();
        }

        void boton_ButtonPressed(GUIButton button)
        {
            OnButtonPressed(button);
        }

        public virtual void OnButtonPressed(GUIButton button)
        {
            Close();
        }
    }
}
