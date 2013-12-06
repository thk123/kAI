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
        /// Represents the state of a rectangle.
        /// </summary>
        class RectangleState
        {
            public bool WasHovered = false;
            public bool Hovered = false;
            public bool MousePressStartHere = false;

            public DateTime LastClickTime = DateTime.MinValue;
            public Point LastMousePressPosition = Point.Empty;

            public const double kDoubleClickDuration = 0.5;
        }

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
        kAIRTree<Tuple<kAIMouseEventResponders, RectangleState>> mMovingMouseEventListeners;

        // Stores the elements that are fixed independent of the camera. 
        kAIRTree<Tuple<kAIMouseEventResponders, RectangleState>> mFixedMouseEventListeners;

        // Is the mouse currently down. 
        bool mMouseDown;
        bool mWasMouseDown;

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
            mMovingMouseEventListeners = new kAIRTree<Tuple<kAIMouseEventResponders, RectangleState>>();
            // The fixed elements are the ones that are in a fixed position irrespective of where the camera is. 
            mFixedMouseEventListeners = new kAIRTree<Tuple<kAIMouseEventResponders, RectangleState>>();
            mEditorWindow = lEditorWindow;
            mMouseDown = false;
            mWasMouseDown = false;
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
                mFixedMouseEventListeners.AddRectangle(lArea, new Tuple<kAIMouseEventResponders, RectangleState>(lOnClickResponder, new RectangleState()));
            }
            else
            {
                mMovingMouseEventListeners.AddRectangle(lArea, new Tuple<kAIMouseEventResponders, RectangleState>(lOnClickResponder, new RectangleState()));
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
                var lRectData = mFixedMouseEventListeners.RemoveRectangle(lArea);
                if (lRectData != null)
                {
                    return lRectData.Item1;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                var lRectData = mMovingMouseEventListeners.RemoveRectangle(lArea);
                if (lRectData != null)
                {
                    return lRectData.Item1;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Clears the input rectangles stored in this input manager. 
        /// </summary>
        public void ClearInputRectangles()
        {
            mFixedMouseEventListeners.Clear();
            mMovingMouseEventListeners.Clear();
        }

       
        /// <summary>
        /// Processes a specific tree for the mouse input. 
        /// </summary>
        /// <param name="lTree">The tree (either fixed or moving) to process. </param>
        /// <param name="lPointToCheck">The point to actually check for collisions with. </param>
        /// <param name="lSender">The sender (used for calling events). </param>
        /// <param name="lEventArgs">The event args (used for calling events). </param>
        void HandleTree(kAIRTree<Tuple<kAIMouseEventResponders, RectangleState>> lTree, Point lPointToCheck, object lSender, MouseEventArgs lEventArgs)
        {
            // First we find all the elements so we can see which elements were under the mouse before and now aren't. 
            IEnumerable<Tuple<kAIMouseEventResponders, RectangleState>> lAllControls = lTree.GetAllContents();
            //List<RectangleState> lControlState = new List<RectangleState>();
            
            foreach (Tuple<kAIMouseEventResponders, RectangleState> lResponder in lAllControls)
            {
                // We store whether the mouse was over the item
                lResponder.Item2.WasHovered = lResponder.Item2.Hovered;

                // We assume not hovered at this stage
                lResponder.Item2.Hovered = false;

                // The mouse press only started here if the mouse press started here and the mouse is still down
                lResponder.Item2.MousePressStartHere = lResponder.Item2.MousePressStartHere && mWasMouseDown;
            }

            // Using the tree, get the elements under the mouse point. 
            IEnumerable<Tuple<kAIMouseEventResponders, RectangleState>> lUnderControls = lTree.GetRectsContainingPoint(lPointToCheck);

            // Go through the elements, firing events accordingly. 
            foreach (Tuple<kAIMouseEventResponders, RectangleState> lResponder in lUnderControls)
            {
                // All of the things that are under the mouse have hovered as true
                lResponder.Item2.Hovered = true;

                // We don't set pressed as this indicates that the mouse was pressed on them

                mEditorWindow.ParentControl.ContextMenu = lResponder.Item1.ContextMenu;

                // If we weren't previously hovered
                if (!lResponder.Item2.WasHovered)
                {
                    // We trigger the hover action
                    lResponder.Item1.CallAction(lResponder.Item1.OnMouseEnter, lSender, lEventArgs);
                    // This is triggererd after clicking on a node since we move the node. 
                }

                // If the mouse button is down and was up last frame then the mouse has just been pressed
                if (mMouseDown && !mWasMouseDown)
                {
                    // We trigger the on mouse down event. 
                    lResponder.Item1.CallAction(lResponder.Item1.OnMouseDown, lSender, lEventArgs);

                    // And we store that the mouse started off being pressed on this object
                    lResponder.Item2.MousePressStartHere = true;
                }
                else if (!mMouseDown && mWasMouseDown) // the mouse has just been released this frame
                {
                    // if the mouse was initially pressed on this object  so this is a click action
                    if (lResponder.Item2.MousePressStartHere)
                    {
                        // We find out the time difference between the two clicks
                        DateTime lastRectanglePress = lResponder.Item2.LastClickTime;
                        TimeSpan lTimeSpan = lastRectanglePress - DateTime.Now;

                        // If the duration is short enough && the mouse has not moved
                        if (lTimeSpan.Duration().TotalSeconds <= RectangleState.kDoubleClickDuration &&
                            lResponder.Item2.LastMousePressPosition == lEventArgs.Location)
                        {
                            lResponder.Item1.CallAction(lResponder.Item1.OnMouseDoubleClick, lSender, lEventArgs);
                        }
                        else // duration too long or the mouse has moved, therefore not a double click but a single click
                        {
                            lResponder.Item1.CallAction(lResponder.Item1.OnMouseClick, lSender, lEventArgs);
                        }

                        // Update the values in the rectangle of this press
                        lResponder.Item2.LastClickTime = DateTime.Now;
                        lResponder.Item2.LastMousePressPosition = lEventArgs.Location;
                    }
                    else // else the mouse was pressed down somewhere else so is a MouseUp event
                    {
                        lResponder.Item1.CallAction(lResponder.Item1.OnMouseUp, lSender, lEventArgs);
                    }

                    lResponder.Item2.MousePressStartHere = false;                    
                }

                // The mouse is on something. 
                MouseOnSomething = true;
            }

            foreach (Tuple<kAIMouseEventResponders, RectangleState> lControlState in lAllControls)
            {
                if (lControlState.Item2.WasHovered && !lControlState.Item2.Hovered)
                {
                    lControlState.Item1.CallAction(lControlState.Item1.OnMouseLeave, lSender, lEventArgs);
                }
            }

            if (!MouseOnSomething)
            {
                mEditorWindow.ParentControl.ContextMenu = mEditorWindow.Editor.GlobalContextMenu;
            }

            

        }

        private void UpdateTrees(kAIRelativePosition lRelativePoint, kAIAbsolutePosition lAbsolutePoint, object sender, MouseEventArgs e)
        {
            MouseOnSomething = false;

            HandleTree(mMovingMouseEventListeners, lAbsolutePoint.mPoint, sender, e);
            HandleTree(mFixedMouseEventListeners, lRelativePoint.mPoint, sender, e);

            mWasMouseDown = mMouseDown;
        }

        void lParentControl_MouseUp(object sender, MouseEventArgs e)
        {
            mWasMouseDown = mMouseDown;
            mMouseDown = false;

            kAIRelativePosition lRelativePoint = new kAIRelativePosition(e.Location); // the actual position of the mouse

            // the position of the mouse in absolute space (i.e. translated for the camera)
            kAIAbsolutePosition lAbsolutePoint = new kAIAbsolutePosition(lRelativePoint, mEditorWindow.CameraPosition, false);

            UpdateTrees(lRelativePoint, lAbsolutePoint, sender, e);

            if (OnMouseUp != null)
            {
                OnMouseUp(sender, e);
            }
        }

        void lParentControl_MouseDown(object sender, MouseEventArgs e)
        {
            mWasMouseDown = mMouseDown;
            mMouseDown = true;

            kAIRelativePosition lRelativePoint = new kAIRelativePosition(e.Location); // the actual position of the mouse

            // the position of the mouse in absolute space (i.e. translated for the camera)
            kAIAbsolutePosition lAbsolutePoint = new kAIAbsolutePosition(lRelativePoint, mEditorWindow.CameraPosition, false);

            UpdateTrees(lRelativePoint, lAbsolutePoint, sender, e);

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

            

            UpdateTrees(lRelativePoint, lAbsolutePoint, sender, e);

            if (OnMouseMove != null)
            {
                OnMouseMove(sender, e);
            }
        }
    }
}
