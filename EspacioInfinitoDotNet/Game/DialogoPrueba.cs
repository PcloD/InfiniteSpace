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
    class DialogoPrueba : GUIDialog
    {
        public DialogoPrueba()
            : base(new Size(500, 550))
        {
            Title = "Prueba";

            BackColor = Color.FromArgb(240, Color.DarkGreen);
            TitleColor = Color.FromArgb(240, Color.Green);

            GUICheckbox guiChk = new GUICheckbox(new Size(100, 24));
            guiChk.Text = "Checkbox";
            AddChildWindow(guiChk, new Point(5, 30));

            GUIHorizontalComboBox guiHCB = new GUIHorizontalComboBox(new Size(100, 24), new object[] { "Opcion A", "Opcion B", "Opcion C" });
            AddChildWindow(guiHCB, new Point(5, 50));

            Focus = guiHCB;

            GUIButton guiBtn1 = new GUIButton(new Size(30, 24));
            guiBtn1.Text = "BTN1";
            AddChildWindow(guiBtn1, new Point(5, 80));

            GUIButton guiBtn2 = new GUIButton(new Size(30, 24));
            guiBtn2.Text = "BTN2";
            AddChildWindow(guiBtn2, new Point(40, 80));

            guiBtn1.ButtonPressed += new GUIButton.ButtonPressedHandler(guiBtn1_ButtonPressed);
        }

        void guiBtn1_ButtonPressed(GUIButton button)
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
