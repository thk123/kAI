using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using SlimDX;
using kAI.Core;

namespace kAI.Editor.Controls.DX
{
    /// <summary>
    /// Represents a point somewhere in a behaviour. The unit is pixels but 
    /// does not take in to account the position of the view port. 
    /// </summary>
    struct NodeCoordinate
    {
        Point mPosition;

        /// <summary>
        /// Create a NodeCoordinate based on an absolute pixel position relative to the centre of the behaviour. 
        /// </summary>
        /// <param name="lXPos">The x position. </param>
        /// <param name="lYPos">The y position. </param>
        public NodeCoordinate(int lXPos, int lYPos)
        {
            mPosition = new Point(lXPos, lYPos);
        }

        /// <summary>
        /// Create a node coordinate from a point on a form. 
        /// </summary>
        /// <param name="lFormPos">The position in the form.</param>
        /// <param name="lParentControl">The control the node will be in. </param>
        /// <param name="lCameraPos">The position of the camera. </param>
        public NodeCoordinate(Point lFormPos, Control lParentControl, NodeCoordinate lCameraPos)
        {
            int lWidth = lParentControl.Width;
            int lHeight = lParentControl.Height;

            // To get from a form point to a node point, we must shift the origin to the centre and move according to the camera
            mPosition = new Point(
                (lWidth / 2) - lFormPos.X + lCameraPos.mPosition.X,
                (lHeight / 2) - lFormPos.Y + lCameraPos.mPosition.X);
        }

        /// <summary>
        /// Translate the point by a specified dx/dy
        /// </summary>
        /// <param name="ldX">X offset. </param>
        /// <param name="ldY">Y offset. </param>
        public void Translate(int ldX, int ldY)
        {
            mPosition.Offset(new Point(ldX, ldY));
        }

        /// <summary>
        /// Get a position on the form of this absolute position. 
        /// </summary>
        /// <param name="lParentControl">The control containing the point. </param>
        /// <param name="lCameraPos">The current position of the camera. </param>
        /// <returns>The point on the form of this global point. </returns>
        public Point GetFormPosition(Control lParentControl, NodeCoordinate lCameraPos)
        {
            // We shift the origin to the upper left corner and then move according to the camera
            return new Point(
                mPosition.X + (lParentControl.Width / 2) - lCameraPos.mPosition.X,
                mPosition.Y + (lParentControl.Height / 2) - lCameraPos.mPosition.Y);
        }

        /// <summary>
        /// Deprecated: Get a vector2 of this point going from the global space, through the form space
        /// into the normalised space [-1, 1] x [-1, 1]
        /// </summary>
        /// <param name="lParentControl">The control the point is in. </param>
        /// <param name="lCameraPos">The current position of the camera. </param>
        /// <returns>The normalised point of the control. </returns>
        public Vector2 GetNormalisedPositionV2(Control lParentControl, NodeCoordinate lCameraPos)
        {
            return GetFormPosition(lParentControl, lCameraPos).GetNormalisedPointFromFormV2(lParentControl);
        }

        /// <summary>
        /// Deprecated: Get a vector3 of this point going from the global space, through the form space
        /// into the normalised space [-1, 1] x [-1, 1] with the Z coordinate set to 0.5f
        /// </summary>
        /// <param name="lParentControl">The control the point is in. </param>
        /// <param name="lCameraPos">The current position of the camera. </param>
        /// <returns>The normalised point of the control. </returns>
        public Vector3 GetNormalisedPositionV3(Control lParentControl, NodeCoordinate lCameraPos)
        {
            return GetFormPosition(lParentControl, lCameraPos).GetNormalisedPointFromFormV3(lParentControl);
        }

        /// <summary>
        /// Returns a string representing the point. 
        /// </summary>
        /// <returns>A string in the same format as <see cref="System.Drawing.Point"/>.  </returns>
        public override string ToString()
        {
            return mPosition.ToString();
        }
    }

    /// <summary>
    /// Deprecated: Helper functions for converting between different coordinate systems.
    /// </summary>
    static class CoordinateConversion
    {
        /// <summary>
        /// Deprecated: Convert a form point into a normalised point. 
        /// </summary>
        /// <param name="lFormPoint">The point somewhere on the form in pixels. </param>
        /// <param name="lParentControl">The control the point is in. </param>
        /// <returns>A normalised version of the point. </returns>
        public static Vector2 GetNormalisedPointFromFormV2(this Point lFormPoint, Control lParentControl)
        {
            float lHalfWidth = (float)lParentControl.Width * 0.5f;
            float lHalfHeight = (float)lParentControl.Height * 0.5f;

            // We shift the origin back to the centre and scale according to the size of the control
            return new Vector2(
                (lFormPoint.X - lHalfWidth) / lHalfWidth,
                (lFormPoint.Y - lHalfHeight) / lHalfHeight);
        }

        /// <summary>
        /// Deprecated: Convert a form point into a normalised point. 
        /// </summary>
        /// <param name="lFormPoint">The point somewhere on the form in pixels. </param>
        /// <param name="lParentControl">The control the point is in. </param>
        /// <returns>A normalised version of the point with 0.5f as the Z coordinate. </returns>
        public static Vector3 GetNormalisedPointFromFormV3(this Point lFormPoint, Control lParentControl)
        {
            float lHalfWidth = (float)lParentControl.Width * 0.5f;
            float lHalfHeight = (float)lParentControl.Height * 0.5f;

            // We shift the origin back to the centre and scale according to the size of the control
            return new Vector3(
                (lFormPoint.X - lHalfWidth) / lHalfWidth,
                (lFormPoint.Y - lHalfHeight) / lHalfHeight,
                0.5f);
        }

        /// <summary>
        /// Deprecated: Convert a Vector2 into a point on a control in pixels. 
        /// </summary>
        /// <param name="lNormalisedPoint">The normalised point. </param>
        /// <param name="lParentControl">The control the point should be relative to. </param>
        /// <returns>A point (a pixel position) relative to the parent control of the normalised point. </returns>
        public static Point GetFormPointFromNormalisedV2(this Vector2 lNormalisedPoint, Control lParentControl)
        {
            float lHalfWidth = (float)lParentControl.Width * 0.5f;
            float lHalfHeight = (float)lParentControl.Height * 0.5f;

            return new Point(
                (int)(lNormalisedPoint.X * lHalfWidth) + (int)lHalfWidth,
                (int)(lNormalisedPoint.Y * lHalfHeight) + (int)lHalfHeight);
        }

        /// <summary>
        /// Deprecated: Convert a Vector3 into a point on a control in pixels. 
        /// </summary>
        /// <param name="lNormalisedPoint">The normalised point. </param>
        /// <param name="lParentControl">The control the point should be relative to. </param>
        /// <returns>A point (a pixel position) relative to the parent control of the normalised point. </returns>
        public static Point GetFormPointFromNormalisedV3(this Vector3 lNormalisedPoint, Control lParentControl)
        {
            float lHalfWidth = (float)lParentControl.Width * 0.5f;
            float lHalfHeight = (float)lParentControl.Height * 0.5f;

            return new Point(
                (int)(lNormalisedPoint.X * lHalfWidth) + (int)lHalfWidth,
                (int)(lNormalisedPoint.Y * lHalfHeight) + (int)lHalfHeight);
        }

        /// <summary>
        /// Deprecated: Convert a pixel size into a normalised size. 
        /// </summary>
        /// <param name="lFormSize">The size in pixels. </param>
        /// <param name="lParentControl">The control containing the size. </param>
        /// <returns>A normalised length of it with 0.0f as the depth. </returns>
        public static Vector3 GetNormalisedSizeFromSizeV3(this Size lFormSize, Control lParentControl)
        {
            float lHalfWidth = (float)lParentControl.Width * 0.5f;
            float lHalfHeight = (float)lParentControl.Height * 0.5f;

            // We shift the origin back to the centre and scale according to the size of the control
            return new Vector3(
                (lFormSize.Width) / lHalfWidth,
                (lFormSize.Height) / lHalfHeight,
                0.0f);
        }

        /// <summary>
        /// Deprecated: Convert a pixel size into a normalised size. 
        /// </summary>
        /// <param name="lFormSize">The size in pixels. </param>
        /// <param name="lParentControl">The control containing the size. </param>
        /// <returns>A normalised length of it. </returns>
        public static Vector2 GetNormalisedSizeFromSizeV2(this Size lFormSize, Control lParentControl)
        {
            float lHalfWidth = (float)lParentControl.Width * 0.5f;
            float lHalfHeight = (float)lParentControl.Height * 0.5f;

            return new Vector2(
                (lFormSize.Width) / lHalfWidth,
                (lFormSize.Height) / lHalfHeight);
        }
    }
}
