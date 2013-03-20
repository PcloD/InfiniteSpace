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
    class DialogoReiniciar : DialogoTextoConBotones
    {
        private bool salirDelJuegoSiNoReinicia;

        public DialogoReiniciar(string motivo, bool salirDelJuegoSiNoReinicia)
            : base(new Size(300, 100), "Iniciar Nuevo Juego", motivo + " Iniciar Nuevo Juego? (S/N)", 1, "Si", "No")
        {
            this.salirDelJuegoSiNoReinicia = salirDelJuegoSiNoReinicia;
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
            GameEngine.Instance.Reiniciar = true;
            Close();
        }

        private void OnNo()
        {
            if (salirDelJuegoSiNoReinicia)
                GameEngine.Instance.Salir = true;
            Close();
        }

        public override bool HandleEvent(GUIEvent guiEvent)
        {
            bool handled = false;
            base.HandleEvent(guiEvent);

            if (guiEvent is GUIEventKeyPressed)
            {
                GUIEventKeyPressed guiEventKey = (GUIEventKeyPressed)guiEvent;

                if (guiEventKey.key == Sdl.SDLK_s)
                {
                    GameEngine.Instance.Reiniciar = true;
                    Close();
                    handled = true;
                }
                else if (guiEventKey.key == Sdl.SDLK_n)
                {
                    if (salirDelJuegoSiNoReinicia)
                        GameEngine.Instance.Salir = true;
                    Close();
                    handled = true;
                }
            }

            return handled;
        }
    }
}
