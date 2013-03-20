using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Tao.Sdl;

[assembly: CLSCompliant(false)]
namespace EspacioInfinitoDotNet
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Sdl.SDL_Init(Sdl.SDL_INIT_VIDEO | Sdl.SDL_INIT_JOYSTICK);

            try
            {
                Game.GameEngine ge = new Game.GameEngine();

                ge.Ejecutar();
            }
            finally
            {
                Sdl.SDL_Quit();
            }
        }
    }
}