using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using SlimDX;

namespace kAI.Editor.Controls.DX
{
    struct NodeCoordinate
    {
        Point mPosition;

        public NodeCoordinate(int lXPos, int lYPos)
        {
            mPosition = new Point(lXPos, lYPos);
        }

        public NodeCoordinate(Point lFormPos, Control lParentControl, NodeCoordinate lCameraPos)
        {
            int lWidth = lParentControl.Width;
            int lHeight = lParentControl.Height;

            // To get from a form point to a node point, we must shift the origin to the centre and move according to the camera
            mPosition = new Point(
                (lWidth / 2) - lFormPos.X + lCameraPos.mPosition.X,
                (lHeight / 2) - lFormPos.Y + lCameraPos.mPosition.X);
        }

        public void Translate(int ldX, int ldY)
        {
            mPosition.Offset(new Point(ldX, ldY));
        }

        public Point GetFormPosition(Control lParentControl, NodeCoordinate lCameraPos)
        {
            // We shift the origin to the upper left corner and then move according to the camera
            return new Point(
                mPosition.X + (lParentControl.Width / 2) - lCameraPos.mPosition.X,
                mPosition.Y + (lParentControl.Height / 2) - lCameraPos.mPosition.Y);
        }

        public Vector2 GetNormalisedPositionV2(Control lParentControl, NodeCoordinate lCameraPos)
        {
            return GetFormPosition(lParentControl, lCameraPos).GetNormalisedPointFromFormV2(lParentControl);
        }

        public Vector3 GetNormalisedPositionV3(Control lParentControl, NodeCoordinate lCameraPos)
        {
            return GetFormPosition(lParentControl, lCameraPos).GetNormalisedPointFromFormV3(lParentControl);
        }
    }

    static class CoordinateConversion
    {
        public static Vector2 GetNormalisedPointFromFormV2(this Point lFormPoint, Control lParentControl)
        {
            float lHalfWidth = (float)lParentControl.Width * 0.5f;
            float lHalfHeight = (float)lParentControl.Height * 0.5f;

            // We shift the origin back to the centre and scale according to the size of the control
            return new Vector2(
                (lFormPoint.X - lHalfWidth) / lHalfWidth,
                (lFormPoint.Y - lHalfHeight) / lHalfHeight);
        }

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

        public static Point GetFormPointFromNoramlised(this Vector2 lNormalisedPoint, Control lParentControl)
        {
            float lHalfWidth = (float)lParentControl.Width * 0.5f;
            float lHalfHeight = (float)lParentControl.Height * 0.5f;

            return new Point(
                (int)(lNormalisedPoint.X * lHalfWidth) + (int)lHalfWidth,
                (int)(lNormalisedPoint.Y * lHalfHeight) + (int)lHalfHeight);
        }

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
