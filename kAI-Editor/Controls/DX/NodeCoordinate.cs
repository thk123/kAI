using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using SlimDX;
using kAI.Core;

namespace kAI.Editor.Controls.DX.Coordinates
{
    /// <summary>
    /// Represents the actual position of something wrt to nothing. 
    /// </summary>
    struct kAIAbsolutePosition
    {
        internal Point mPoint;

        internal bool mIsFixed;

        public kAIAbsolutePosition(int lX, int lY, bool lIsFixed)
        {
            mPoint = new Point(lX, lY);
            mIsFixed = lIsFixed;
        }

        public kAIAbsolutePosition(kAIRelativePosition lPos, kAIAbsolutePosition lCameraPos, bool lIsFixed)
        {
            mPoint = lPos.mPoint;
            if (!lIsFixed)
            {
                mPoint.Offset(lCameraPos.mPoint);
            }
            mIsFixed = lIsFixed;
        }

        public static kAIAbsolutePosition MidYFirstX(kAIAbsolutePosition lFirst, kAIAbsolutePosition lSecond)
        {
            return new kAIAbsolutePosition(lFirst.mPoint.X, (int)(0.5f * (lFirst.mPoint.Y + lSecond.mPoint.Y)), lFirst.mIsFixed);
        }

        public static kAIAbsolutePosition MidYSecondX(kAIAbsolutePosition lFirst, kAIAbsolutePosition lSecond)
        {
            return new kAIAbsolutePosition(lSecond.mPoint.X, (int)(0.5f * (lFirst.mPoint.Y + lSecond.mPoint.Y)), lFirst.mIsFixed);
        }

        public kAIAbsolutePosition Translate(int ldX, int ldY)
        {
            return new kAIAbsolutePosition(mPoint.X + ldX, mPoint.Y + ldY, mIsFixed);
        }

        public kAIAbsolutePosition Add(kAIAbsolutePosition lOtherAbs)
        {
            kAIObject.Assert(null, lOtherAbs.mIsFixed == mIsFixed);
            return Translate(lOtherAbs.mPoint.X, lOtherAbs.mPoint.Y);
        }

        public kAIAbsolutePosition Subtract(kAIAbsolutePosition lOtherAbs)
        {
            kAIObject.Assert(null, lOtherAbs.mIsFixed == mIsFixed);
            return Translate(-lOtherAbs.mPoint.X, -lOtherAbs.mPoint.Y);
        }

        public kAIRelativePosition GetAsRelative(kAIAbsolutePosition lCameraPos)
        {
            return new kAIRelativePosition(this, lCameraPos);
        }
    }

    /// <summary>
    /// Represents the absolute size of something. 
    /// </summary>
    struct kAIAbsoluteSize
    {
        internal Size mSize;
        public kAIAbsoluteSize(int lWidth, int lHeight)
        {
            mSize = new Size(lWidth, lHeight);
        }

        public kAIAbsoluteSize(kAIRelativeSize lSize)
        {
            mSize = lSize.mSize;
        }

        public kAIAbsoluteSize ChangeHeight(int lDelta)
        {
            return new kAIAbsoluteSize(mSize.Width, mSize.Height + lDelta);
        }

        public kAIAbsoluteSize ChangeWidth(int lDelta)
        {
            return new kAIAbsoluteSize(mSize.Width + lDelta, mSize.Height);
        }
    }

    /// <summary>
    /// Represents the position of something wrt the position of the camera. 
    /// </summary>
    struct kAIRelativePosition
    {
        internal Point mPoint;

        public kAIRelativePosition(kAIAbsolutePosition lPos, kAIAbsolutePosition lCameraPos)
        {
            mPoint = lPos.mPoint;
            if (!lPos.mIsFixed)
            {
                mPoint.Offset(-lCameraPos.mPoint.X, -lCameraPos.mPoint.Y);
            }
        }

        public kAIRelativePosition(Point lFormPos)
        {
            mPoint = lFormPos;
        }
    }

    /// <summary>
    /// Represents the relative size of something wrt to the position of the camera.
    /// At the moment, this is the same as the absolute size, but I suppose if we 
    /// add a zoom to the camera, this will change. 
    /// </summary>
    struct kAIRelativeSize
    {
        internal Size mSize;

        public kAIRelativeSize(kAIAbsoluteSize lSize)
        {
            mSize = lSize.mSize;
        }
    }

    /// <summary>
    /// Represents a position in normalised space [-1, 1] x [-1, 1] so dependent on 
    /// the position of the camera and the size of the control being used. 
    /// </summary>
    struct kAINormalisedPosition
    {
        Vector3 mPos;

        public kAINormalisedPosition(kAIAbsolutePosition lPos, kAIAbsolutePosition lCameraPos, Control lContainer)
            :this(new kAIRelativePosition(lPos, lCameraPos), lContainer)
        {

        }

        public kAINormalisedPosition(kAIRelativePosition lPos, Control lContainer)
        {
            int lHalfWidth = lContainer.Width / 2;
            int lHalfHeight = lContainer.Height / 2;

            Point lRelativePoint = lPos.mPoint;
            lRelativePoint.Offset(-lHalfWidth, -lHalfHeight);

            mPos = new Vector3
            {
                X = ((float)lRelativePoint.X / (float)lHalfWidth),
                Y = ((float)lRelativePoint.Y / (float)lHalfHeight) * -1, // * -1 since in normalised space +1 is at the top, -1 is at the bottom
                Z = 0.5f
            };
        }

        
        public Vector3 GetAsV3()
        {
            return mPos;
        }
    }

    /// <summary>
    /// Represents a size in normalised space ([-1, 1] x [-1, 1]) so is depdent on
    /// the size of the control being used and presumably the camera if the camera 
    /// has a zoom function. 
    /// </summary>
    struct kAINormalisedSize
    {
        Vector2 mSize;

        public kAINormalisedSize(kAIAbsoluteSize lSize, Control lParentalControl)
            : this(new kAIRelativeSize(lSize), lParentalControl)
        {}

        public kAINormalisedSize(kAIRelativeSize lSize, Control lContainer)
        {
            int lHalfWidth = lContainer.Width / 2;
            int lHalfHeight = lContainer.Height / 2;

            mSize = new Vector2
            {
                X = lSize.mSize.Width / lHalfWidth,
                Y = lSize.mSize.Height / lHalfHeight
            };
        }

        public Vector3 GetAsV3()
        {
            return new Vector3(mSize, 0.0f);
        }
    }
}
