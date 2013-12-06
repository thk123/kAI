using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace kAI.Editor.Controls.DX
{
    /// <summary>
    /// Represents a set of actions that can happen in relation to some event
    /// on a rectangle registered in the input manager. 
    /// </summary>
    class kAIMouseEventResponders
    {
        /// <summary>
        /// Triggered when the mouse first hovers over an element.
        /// </summary>
        public EventHandler<MouseEventArgs> OnMouseEnter;

        /// <summary>
        /// Triggered when the mouse first leaves an element.
        /// </summary>
        public EventHandler<MouseEventArgs> OnMouseLeave;

        /// <summary>
        /// Triggered whenever the mouse is pressed on an object. 
        /// </summary>
        public EventHandler<MouseEventArgs> OnMouseDown;

        /// <summary>
        /// Triggered when the mouse if lifted on an object.  
        /// </summary>
        public EventHandler<MouseEventArgs> OnMouseUp;

        /// <summary>
        /// Triggered when the mouse is pressed and released on the object. 
        /// </summary>
        public EventHandler<MouseEventArgs> OnMouseClick;

        /// <summary>
        /// Triggered when the mouse is clicked twice without the mouse moving in a
        /// small window of time. 
        /// </summary>
        public EventHandler<MouseEventArgs> OnMouseDoubleClick;

        /// <summary>
        /// The context menu that is applicable within this rectangle. 
        /// </summary>
        public ContextMenu ContextMenu;

        /// <summary>
        /// Debug: a sting used to identify this rectangle. 
        /// </summary>
        public string RectangleId;


        /// <summary>
        /// Helper function to call a given event handler. 
        /// </summary>
        /// <param name="lAction">The event to trigger. </param>
        /// <param name="sender">What is triggering the event. </param>
        /// <param name="lEventArgs">The parameters of the event. </param>
        public void CallAction(EventHandler<MouseEventArgs> lAction, object sender, MouseEventArgs lEventArgs)
        {
            if (lAction != null)
            {
                lAction(sender, lEventArgs);
            }
        }

        /// <summary>
        /// Gets a string representation of this object. 
        /// </summary>
        /// <returns>The debug string RectangleID. </returns>
        public override string ToString()
        {
            return RectangleId;
        }
    }
}