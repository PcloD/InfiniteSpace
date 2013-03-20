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
    class DialogoConfiguracion : GUIDialog
    {
        private class Resolucion
        {
            public Size size;

            public Resolucion(Size size)
            {
                this.size = size;
            }

            public override string ToString()
            {
                return size.Width.ToString() + " x " + size.Height.ToString();
            }
        }

        GUIHorizontalComboBox guiResolucion;
        GUIHorizontalComboBox guiBitsDeColor;
        GUICheckbox guiPantallaCompleta;
        GUIHorizontalComboBox guiFormaDeControl;
        GUINumeric guiVelocidadRotacionInicial;
        GUINumeric guiVelocidadRotacionFinal;
        GUINumeric guiTiempoAceleracionRotacion;

        public DialogoConfiguracion()
            : base(new Size(500, 400))
        {
            Title = "Configuracion";

            BackColor = Color.FromArgb(240, Color.DarkBlue);
            TitleColor = Color.FromArgb(240, Color.Blue);

            GUIStatic st;

            int y = 5;

            //Resolucion
            st = new GUIStatic(new Size(1, 1));
            st.Text = "--------------------------- Pantalla ---------------------------";
            AddDialogChildWindow(st, new Point(5, y));

            y += 30;
           
            //Resolucion
            st = new GUIStatic(new Size(1, 1));
            st.Text = "Resolución: ";
            AddDialogChildWindow(st, new Point(5, y));

            Sdl.SDL_Rect[] resoluciones = Sdl.SDL_ListModes(IntPtr.Zero, Sdl.SDL_HWSURFACE | Sdl.SDL_DOUBLEBUF | Sdl.SDL_OPENGL | Sdl.SDL_FULLSCREEN);
            System.Collections.ArrayList arrResoluciones = new System.Collections.ArrayList();

            foreach (Sdl.SDL_Rect res in resoluciones)
                arrResoluciones.Add(new Resolucion(new Size(res.w, res.h)));

            guiResolucion = new GUIHorizontalComboBox(new Size(InnerBounds.Width / 2 - 10, 24),
                (object[]) arrResoluciones.ToArray());

            AddDialogChildWindow(guiResolucion, new Point(5 + InnerBounds.Width / 2, y));

            y += 30;

            //Profunidad Bits
            st = new GUIStatic(new Size(1, 1));
            st.Text = "Bits de Color: ";
            AddDialogChildWindow(st, new Point(5, y));

            guiBitsDeColor = new GUIHorizontalComboBox(new Size(InnerBounds.Width / 2 - 10, 24),
                new object[] { 16, 32 } );
            AddDialogChildWindow(guiBitsDeColor, new Point(5 + InnerBounds.Width / 2, y));

            y += 30;

            //Pantalla completa
            st = new GUIStatic(new Size(1, 1));
            st.Text = "Pantalla Completa: ";
            AddDialogChildWindow(st, new Point(5, y));

            guiPantallaCompleta = new GUICheckbox(new Size(InnerBounds.Width / 2 - 10, 24));
            AddDialogChildWindow(guiPantallaCompleta, new Point(5 + InnerBounds.Width / 2, y));

            y += 30;

            //Controles
            st = new GUIStatic(new Size(1, 1));
            st.Text = "--------------------------- Controles --------------------------";
            AddDialogChildWindow(st, new Point(5, y));

            y += 30;

            //Forma de Control
            st = new GUIStatic(new Size(1, 1));
            st.Text = "Forma de Control: ";
            AddDialogChildWindow(st, new Point(5, y));

            guiFormaDeControl = new GUIHorizontalComboBox(new Size(InnerBounds.Width / 2 - 10, 24),
                new object[] { 
                    "Realista",
                    "Arcade" });
            AddDialogChildWindow(guiFormaDeControl, new Point(5 + InnerBounds.Width / 2, y));

            y += 30;

            //Velocidad Rotación Inicial
            st = new GUIStatic(new Size(1, 1));
            st.Text = "Vel. Rotación Inicial (grados/segundo): ";
            AddDialogChildWindow(st, new Point(5, y));

            guiVelocidadRotacionInicial = new GUINumeric(new Size(InnerBounds.Width / 2 - 10, 24));
            guiVelocidadRotacionInicial.MaxValue = 720.0f;
            guiVelocidadRotacionInicial.MinValue = 10.0f;
            guiVelocidadRotacionInicial.StepValue = 10.0f;
            AddDialogChildWindow(guiVelocidadRotacionInicial, new Point(5 + InnerBounds.Width / 2, y));

            y += 30;

            //Velocidad Rotación Final
            st = new GUIStatic(new Size(1, 1));
            st.Text = "Vel. Rotación Final (grados/segundo): ";
            AddDialogChildWindow(st, new Point(5, y));

            guiVelocidadRotacionFinal = new GUINumeric(new Size(InnerBounds.Width / 2 - 10, 24));
            guiVelocidadRotacionFinal.MaxValue = 720.0f;
            guiVelocidadRotacionFinal.MinValue = 10.0f;
            guiVelocidadRotacionFinal.StepValue = 10.0f;
            AddDialogChildWindow(guiVelocidadRotacionFinal, new Point(5 + InnerBounds.Width / 2, y));

            y += 30;

            //Tiempo Aceleración Rotación
            st = new GUIStatic(new Size(1, 1));
            st.Text = "Tiempo Aceleración Rotacion (segundos): ";
            AddDialogChildWindow(st, new Point(5, y));

            guiTiempoAceleracionRotacion = new GUINumeric(new Size(InnerBounds.Width / 2 - 10, 24));
            guiTiempoAceleracionRotacion.MaxValue = 2.0f;
            guiTiempoAceleracionRotacion.MinValue = 0.0f;
            guiTiempoAceleracionRotacion.StepValue = 0.1f;
            AddDialogChildWindow(guiTiempoAceleracionRotacion, new Point(5 + InnerBounds.Width / 2, y));

            y += 30;

            //Botones
            y = InnerBounds.Height - 30;

            GUIButton btnAceptar = new GUIButton(new Size(60, 24));
            btnAceptar.Text = "Guardar";
            btnAceptar.ButtonPressed += new GUIButton.ButtonPressedHandler(btnAceptar_ButtonPressed);
            AddDialogChildWindow(btnAceptar, new Point(5, y));

            GUIButton btnCancelar = new GUIButton(new Size(60, 24));
            btnCancelar.Text = "Cancelar";
            btnCancelar.ButtonPressed += new GUIButton.ButtonPressedHandler(btnCancelar_ButtonPressed);
            AddDialogChildWindow(btnCancelar, new Point(InnerBounds.Width - btnCancelar.Size.Width - 5, y));

            FocusNextChild();

            //Cargo la configuración en la pantalla

            guiPantallaCompleta.Checked = Properties.Settings.Default.PantallaCompleta;
            guiVelocidadRotacionInicial.Value = Properties.Settings.Default.VelocidadRotacionInicial;
            guiVelocidadRotacionFinal.Value = Properties.Settings.Default.VelocidadRotacionFinal;
            guiTiempoAceleracionRotacion.Value = Properties.Settings.Default.TiempoAceleracionRotacion;

            foreach(Resolucion res in guiResolucion.Items)
                if (res.size == Properties.Settings.Default.Resolucion)
                {
                    guiResolucion.SelectedItem = res;
                    break;
                }

            foreach (int bits in guiBitsDeColor.Items)
                if (bits == Properties.Settings.Default.ProfundidadBits)
                {
                    guiBitsDeColor.SelectedItem = bits;
                    break;
                }

            foreach(String s in guiFormaDeControl.Items)
                if (s.Equals(Properties.Settings.Default.FormaDeControl))
                {
                    guiFormaDeControl.SelectedItem = s;
                    break;
                }
        }

        void btnAceptar_ButtonPressed(GUIButton button)
        {
            Properties.Settings.Default.PantallaCompleta = guiPantallaCompleta.Checked;
            Properties.Settings.Default.Resolucion = ((Resolucion) guiResolucion.SelectedItem).size;
            Properties.Settings.Default.ProfundidadBits = (int)guiBitsDeColor.SelectedItem;
            Properties.Settings.Default.VelocidadRotacionInicial = guiVelocidadRotacionInicial.Value;
            Properties.Settings.Default.VelocidadRotacionFinal = guiVelocidadRotacionFinal.Value;
            Properties.Settings.Default.TiempoAceleracionRotacion = guiTiempoAceleracionRotacion.Value;
            Properties.Settings.Default.FormaDeControl = (string) guiFormaDeControl.SelectedItem;
            Properties.Settings.Default.Save();

            GameEngine.Instance.RecargarConfiguracion();

            GUIDialog dialogoAviso = new DialogoTextoConBotones(new Size(500, 100), "Cambio de Pantalla", "Los cambios a los modos de pantalla solo cobran efecto despues de reiniciar el juego", 0, "Aceptar");
            Father.AddChildWindow(
                dialogoAviso,
                new Point(
                    (Father.Size.Width - dialogoAviso.Size.Width) / 2,
                    (Father.Size.Height - dialogoAviso.Size.Height) / 2)); 
            
            Father.Focus = dialogoAviso;

            Close();
        }

        void btnCancelar_ButtonPressed(GUIButton button)
        {
            Close();
        }

        public override bool HandleEvent(GUIEvent guiEvent)
        {
            bool handled = base.HandleEvent(guiEvent);

            return handled;
        }
    }
}
