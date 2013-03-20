using System;
using System.Collections.Generic;
using System.Text;
using EspacioInfinitoDotNet.Graphics;
using EspacioInfinitoDotNet.Maths;
using System.Drawing;

namespace EspacioInfinitoDotNet.GUI
{
    public abstract class GUIWindow
    {
        #region Atributos

        GUIWindow father;
        GUIWindow focus;
        Rectangle bounds;
        bool drawEnabled = true;
        bool focusable = false;

        List<GUIWindow> childs = new List<GUIWindow>();

        public GUIWindow Father
        {
            get { return father; }
        }

        public Rectangle Bounds
        {
            get { return bounds; }
        }

        public Point Position
        {
            get { return bounds.Location; }
            set { bounds.Location = value; }
        }

        public Size Size
        {
            get { return bounds.Size; }
            set { bounds.Size = value; }
        }

        public List<GUIWindow> Childs
        {
            get { return childs; }
        }

        public bool DrawEnabled
        {
            get { return drawEnabled; }
            set { this.drawEnabled = value; }
        }

        public GUIWindow Focus
        {
            get { return focus; }
            set { this.focus = value; }
        }

        public bool Focusable
        {
            get { return focusable; }
            set { this.focusable = value; }
        }

        #endregion

        #region Metodos internos

	    protected Point PrvGetAbsolutePosition()
        {
            Point pos = Position;

            GUIWindow father = this.father;

            while (father != null)
            {
                pos.X += father.Position.X;
                pos.Y += father.Position.Y;

                father = father.father;
            }

            return pos;
        }

        protected GUIWindow PrvGetChildWindowAtPoint(Point point)
        {
            foreach(GUIWindow child in childs)
            {
                Rectangle pos = child.bounds;

                if (point.X >= pos.Left &&
                    point.Y >= pos.Top &&
                    point.X <= pos.Right &&
                    point.Y <= pos.Bottom)
                {
                    return child;
                }
            }

            return null;
        }

	    internal bool PrvHandleEvent(GUIEvent guiEvent)
        {
            if (guiEvent is GUIEventMouseButtonPressed)
            {
                ((GUIEventMouseButtonPressed) guiEvent).position.X -= Position.X;
                ((GUIEventMouseButtonPressed) guiEvent).position.Y -= Position.Y;
            }
            else if (guiEvent is GUIEventMouseButtonReleased)
            {
                ((GUIEventMouseButtonReleased) guiEvent).position.X -= Position.X;
                ((GUIEventMouseButtonReleased) guiEvent).position.Y -= Position.Y;
            }
            else if (guiEvent is GUIEventMouseMoved)
            {
                ((GUIEventMouseMoved) guiEvent).position.X -= Position.X;
                ((GUIEventMouseMoved) guiEvent).position.Y -= Position.Y;
            }

	        if (guiEvent is GUIEventMouseButtonPressed)
	        {
		        GUIWindow newFocus = null;

                Point point = ((GUIEventMouseButtonPressed) guiEvent).position;

		        newFocus = PrvGetChildWindowAtPoint(point);

		        if (newFocus != this.focus)
		        {
			        if (this.focus != null)
				        this.focus.PrvHandleEvent(new GUIEventFocusLost());
        			
			        this.focus = newFocus;

			        if (this.focus != null)
			        {
				        this.focus.PrvHandleEvent(new GUIEventFocusGain());

				        SetTopChildWindow(this.focus);
			        }
		        }
	        }

            if (this.focus == null)
                return HandleEvent(guiEvent);
            else
            {
                if (focus.PrvHandleEvent(guiEvent))
                    return true;
                else
                    return HandleEvent(guiEvent);
            }
        }

        internal void PrvDraw(GUIGraphicEngine guiGraphicEngine)
        {
            if (drawEnabled)
            {
                guiGraphicEngine.PushBounds(Bounds);

                Draw(guiGraphicEngine);

                foreach (GUIWindow child in childs)
                    child.PrvDraw(guiGraphicEngine);

                guiGraphicEngine.PopBounds();
            }
        }

        #endregion

        public GUIWindow(Size size)
        {
            bounds.Size = size;
        }

        #region Manejo de Ventanas

        public void AddChildWindow(GUIWindow window, Point position)
        {
            childs.Add(window);
            window.father = this;
            window.Position = position;
        }

        public void RemoveChildWindow(GUIWindow window)
        {
            childs.Remove(window);

            if (focus == window)
            {
                focus = null;
                FocusNextChild();
            }
        }

        public void SetTopChildWindow(GUIWindow window)
        {
            if (childs.IndexOf(window) != 0)
            {
                childs.Remove(window);
                childs.Insert(0, window);
            }
        }

        public void SetBottomChildWindow(GUIWindow window)
        {
            if (childs.IndexOf(window) != childs.Count - 1)
            {
                childs.Remove(window);
                childs.Add(window);
            }
        }

        public void FocusNextChild()
        {
            if (Focus == null)
            {
                foreach (GUIWindow g in childs)
                    if (g.Focusable)
                    {
                        Focus = g;
                        break;
                    }
            }
            else
            {
                bool next = false;
                GUIWindow nextFocus = null;

                foreach (GUIWindow g in childs)
                {
                    if (g == Focus)
                        next = true;
                    else
                        if (next && g.Focusable)
                        {
                            nextFocus = g;
                            break;
                        }
                }

                if (nextFocus != null)
                {
                    Focus = nextFocus;
                }
                else
                {
                    foreach (GUIWindow g in childs)
                        if (g.Focusable)
                        {
                            Focus = g;
                            break;
                        }
                }
            }
        }

        public void FocusPreviusChild()
        {
            if (Focus == null)
            {
                foreach (GUIWindow g in childs)
                    if (g.Focusable)
                        Focus = g;
            }
            else
            {
                GUIWindow nextFocus = null;

                foreach (GUIWindow g in childs)
                {
                    if (g == Focus)
                        break;

                    if (g.Focusable)
                        nextFocus = g;
                }

                if (nextFocus != null && nextFocus != Focus)
                {
                    Focus = nextFocus;
                }
                else
                {
                    foreach (GUIWindow g in childs)
                        if (g.Focusable)
                            Focus = g;
                }
            }
        }

        #endregion

        public abstract bool HandleEvent(GUIEvent guiEvent);

        public abstract void Draw(GUIGraphicEngine guiGraphicEngine);
    }
}
