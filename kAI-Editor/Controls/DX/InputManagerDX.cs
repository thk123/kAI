using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using SlimDX.RawInput;
using kAI.Editor.Controls.DX.Coordinates;

namespace kAI.Editor.Controls.DX
{
    /// <summary>
    /// Handles input for DirectX
    /// </summary>
    class kAIInputManagerDX
    {
        /// <summary>
        /// Triggered whenever the mouse moves. 
        /// </summary>
        public event EventHandler<MouseEventArgs> OnMouseMove;

        /// <summary>
        /// Triggered when the mouse button is first pressed. 
        /// </summary>
        public event EventHandler<MouseEventArgs> OnMouseDown;

        /// <summary>
        /// Triggered when the mouse button is released. 
        /// </summary>
        public event EventHandler<MouseEventArgs> OnMouseUp;

        // A reference to the editor window. 
        kAIBehaviourEditorWindowDX mEditorWindow;

        // Stores the elements that move with the camera
        kAIRTree<kAIMouseEventResponders> mMovingMouseEventListeners;

        // Stores the elements that are fixed independent of the camera. 
        kAIRTree<kAIMouseEventResponders> mFixedMouseEventListeners;

        // Is the mouse currently down. 
        bool mMouseDown;

        /// <summary>
        /// Is the mouse on some element or not. 
        /// </summary>
        public bool MouseOnSomething
        {
            get;
            private set;
        }

        /// <summary>
        /// Create a new input manager. 
        /// </summary>
        /// <param name="lParentControl">The control to detect mouse input from. </param>
        /// <param name="lEditorWindow">The parent editor window. </param>
        public kAIInputManagerDX(Control lParentControl, kAIBehaviourEditorWindowDX lEditorWindow)
        {
            // We use WinForms mouse events as I didn't have much luck with the SlimDX/DirectX stuff
            lParentControl.MouseMove += new MouseEventHandler(lParentControl_MouseMove);
            lParentControl.MouseDown += new MouseEventHandler(lParentControl_MouseDown);
            lParentControl.MouseUp += new MouseEventHandler(lParentControl_MouseUp);

            // These trees represent the various rectangles that respond to mouse events
            // The moving elements are things that move as the user drags the camera around
            mMovingMouseEventListeners = new kAIRTree<kAIMouseEventResponders>();
            // The fixed elements are the ones that are in a fixed position irrespective of where the camera is. 
            mFixedMouseEventListeners = new kAIRTree<kAIMouseEventResponders>();
            mEditorWindow = lEditorWindow;
            mMouseDown = false;
            MouseOnSomething = false;
        }

        /// <summary>
        /// Add a rectangle with a set of actions to perform under certain events. 
        /// TODO: Should throw an exception if a matching rectangle is found. 
        /// </summary>
        /// <param name="lArea">The rectangle to watch (either in absolute coordinates if not fixed, or form coordinates if fixed). </param>
        /// <param name="lOnClickResponder">Set of actions to perform in certain events. </param>
        /// <param name="lFixed">Is the element fixed to the camera. </param>
        public void AddClickListenArea(Rectangle lArea, kAIMouseEventResponders lOnClickResponder, bool lFixed)
        {
            if (lFixed)
            {
                mFixedMouseEventListeners.AddRectangle(lArea, lOnClickResponder);
            }
            else
            {
                mMovingMouseEventListeners.AddRectangle(lArea, lOnClickResponder);
            }
        }

        /// <summary>
        /// Remove a rectangle from the tree.
        /// TODO: Should throw an exception if it doesn't exist. 
        /// </summary>
        /// <param name="lArea">The rectangle to remove. </param>
        /// <param name="lFixed">Was the rectangle fixed to the camera or not. </param>
        /// <returns>The set of responders associated with that rectangle, null if it cannot be found. </returns>
        public kAIMouseEventResponders RemoveClickListenArea(Rectangle lArea, bool lFixed)
        {
            if (lFixed)
            {
                return mFixedMouseEventListeners.RemoveRectangle(lArea);
            }
            else
            {
                return mMovingMouseEventListeners.RemoveRectangle(lArea);
            }
        }

        /// <summary>
        /// Processes a specific tree for the mouse input. 
        /// </summary>
        /// <param name="lTree">The tree (either fixed or moving) to process. </param>
        /// <param name="lPointToCheck">The point to actually check for collisions with. </param>
        /// <param name="lSender">The sender (used for calling events). </param>
        /// <param name="lEventArgs">The event args (used for calling events). </param>
        void HandleTree(kAIRTree<kAIMouseEventResponders> lTree, Point lPointToCheck, object lSender, MouseEventArgs lEventArgs)
        {
            // First we find all the elements so we can see which elements were under the mouse before and now aren't. 
            IEnumerable<kAIMouseEventResponders> lAllControls = lTree.GetAllContents();
            List<bool> lControlState = new List<bool>();

            foreach (kAIMouseEventResponders lResponder in lAllControls)
            {
                lControlState.Add(lResponder.Hovered);
                lResponder.Hovered = false;
                //lResponder.JustPressed = false;
            }

            // Using the tree, get the elements under the mouse point. 
            IEnumerable<kAIMouseEventResponders> lUnderControls = lTree.GetRectsContainingPoint(lPointToCheck);

            // Go through the elements, firing events accordingly. 
            foreach (kAIMouseEventResponders lResponder in lUnderControls)
            {
                // If we weren't previously hovered
                if (!lResponder.Hovered)
                {
                    // We trigger the hover action
                    lResponder.CallAction(lResponder.OnMouseHover, lSender, lEventArgs);
                    lResponder.Hovered = true;
                }

                // If the mouse button is down
                if (mMouseDown)
                {
                    // We trigger the on mouse down event. 
                    lResponder.CallAction(lResponder.OnMouseDown, lSender, lEventArgs);
                }

                // The mouse is on something. 
                MouseOnSomething = true;
            }

            // We loop through all the controls and compare their current hover state with their previous hover state. 
            var lControlEnumerator = lAllControls.GetEnumerator();
            var lControlStateEnumerator = lControlState.GetEnumerator();

            while (lControlEnumerator.MoveNext() && lControlStateEnumerator.MoveNext())
            {
                if (!lControlEnumerator.Current.Hovered)
                {
                    // is now now hovered

                    if (lControlStateEnumerator.Current)
                    {
                        // Was hovered before, therefore the mouse just moved off it
                        lControlEnumerator.Current.CallAction(lControlEnumerator.Current.OnMouseLeave, lSender, lEventArgs);
                    }
                }
            }
        }

        void lParentControl_MouseUp(object sender, MouseEventArgs e)
        {
            mMouseDown = false;
            if (OnMouseUp != null)
            {
                OnMouseUp(sender, e);
            }
        }

        void lParentControl_MouseDown(object sender, MouseEventArgs e)
        {
            mMouseDown = true;
            if (OnMouseDown != null)
            {
                OnMouseDown(sender, e);
            }
        }

        void lParentControl_MouseMove(object sender, MouseEventArgs e)
        {
            kAIRelativePosition lRelativePoint= new kAIRelativePosition(e.Location); // the actual position of the mouse

            // the position of the mouse in absolute space (i.e. translated for the camera)
            kAIAbsolutePosition lAbsolutePoint = new kAIAbsolutePosition(lRelativePoint, mEditorWindow.CameraPosition, false);

            MouseOnSomething = false;

            HandleTree(mMovingMouseEventListeners, lAbsolutePoint.mPoint, sender, e);
            HandleTree(mFixedMouseEventListeners, lRelativePoint.mPoint, sender, e);
            if (OnMouseMove != null)
            {
                OnMouseMove(sender, e);
            }
        }
    }
}
