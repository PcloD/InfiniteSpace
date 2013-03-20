using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using EspacioInfinitoDotNet.Maths;
using Tao.Sdl;

namespace EspacioInfinitoDotNet.GUI.Controls
{
    public class GUIDialog : GUIWindow
    {
        public const int GUI_DIALOG_BORDER_SIZE	= 2;
        public const int GUI_DIALOG_TITLE_SIZE = 26;

        #region Atributos

        string title;
        Color backColor = Color.FromArgb(250, Color.Black);
        Color frameColor = Color.FromArgb(200, Color.DarkGray);
        Color titleColor = Color.FromArgb(200, Color.Blue);

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public Color BackColor
        {
            get { return backColor; }
            set { backColor = value; }
        }

        public Color FrameColor
        {
            get { return frameColor; }
            set { frameColor = value; }
        }

        public Color TitleColor
        {
            get { return titleColor; }
            set { titleColor = value; }
        }

        public Rectangle InnerBounds
        {
            get
            {
                return new Rectangle(GUI_DIALOG_BORDER_SIZE,
                    GUI_DIALOG_BORDER_SIZE + GUI_DIALOG_TITLE_SIZE + 1,
                    Size.Width - GUI_DIALOG_BORDER_SIZE * 2,
                    Size.Height - (GUI_DIALOG_BORDER_SIZE * 2 + GUI_DIALOG_TITLE_SIZE + 1));
            }
        }

        #endregion

        public GUIDialog(Size size)
            : base(size)
        {
            Focusable = true;
        }

        public override void Draw(GUIGraphicEngine guiGraphicEngine)
        {
	        Rectangle rect = new Rectangle(GUI_DIALOG_BORDER_SIZE, GUI_DIALOG_BORDER_SIZE, Size.Width - GUI_DIALOG_BORDER_SIZE * 2, GUI_DIALOG_TITLE_SIZE);
        	
	        //Dibujar titulo
	        guiGraphicEngine.DrawRectangle(rect, TitleColor);

            int altoLetras = guiGraphicEngine.GetTextSizePixels("A").Height;

            guiGraphicEngine.DrawText(new Point(GUI_DIALOG_BORDER_SIZE + 4, GUI_DIALOG_TITLE_SIZE / 2 - altoLetras + 3 + GUI_DIALOG_BORDER_SIZE), title);

	        //Dibujar cuerpo
	        rect.Y += GUI_DIALOG_TITLE_SIZE - 1;
	        rect.Height = Size.Height - rect.Y - GUI_DIALOG_BORDER_SIZE;
        	
	        guiGraphicEngine.DrawRectangle(rect, BackColor);

            rect.Location = new Point(0, 0);
            rect.Size = Size;

            guiGraphicEngine.DrawRectangleFrame(rect, FrameColor, GUI_DIALOG_BORDER_SIZE);
        }

        public override bool HandleEvent(GUIEvent guiEvent)
        {
            bool handled = false;

            if (guiEvent is GUIEventKeyReleased)
            {
                //Manejo el cierre del dialogo en el KeyReleased para que no le llegue a la ventana
                //padre el cierre por error
                GUIEventKeyReleased guiEventKey = (GUIEventKeyReleased)guiEvent;

                if (guiEventKey.key == Sdl.SDLK_ESCAPE)
                {
                    Close();
                    handled = true;
                }
            }

            return handled;
        }

        public void Close()
        {
            Father.RemoveChildWindow(this);
        }

        public void AddDialogChildWindow(GUIWindow window, Point position)
        {
            AddChildWindow(window, new Point(position.X + InnerBounds.X, position.Y + InnerBounds.Y));
        }
    }
}
