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
    class DialogoSalir : DialogoTextoConBotones
    {
        public DialogoSalir() : 
            base(new Size(200, 100), "Salir", "Desea Salir? (S/N)", 1, "Si", "No")
        {
        }

        public override void OnButtonPressed(GUIButton button)
        {
            if (button.Text == "Si")
                OnSi();
            else if (button.Text == "No")
                OnNo();
        }

        private void OnSi()
        {
            GameEngine.Instance.Salir = true;
            Close();
        }

        private void OnNo()
        {
            Close();
        }

        public override bool HandleEvent(GUIEvent guiEvent)
        {
            bool handled = base.HandleEvent(guiEvent);

            if (guiEvent is GUIEventKeyPressed)
            {
                GUIEventKeyPressed guiEventKey = (GUIEventKeyPressed)guiEvent;

                if (guiEventKey.key == Sdl.SDLK_s)
                {
                    OnSi();
                    handled = true;
                }
                else if (guiEventKey.key == Sdl.SDLK_n)
                {
                    OnNo();
                    handled = true;
                }
            }

            return handled;
        }
    }
}
