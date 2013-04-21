using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace kAI.Editor.Core.Util
{
 
    /// <summary>
    /// Used to set a property of a list of objects (eg because Control and ToolStripMenuItem don't 
    /// have a common root).
    /// </summary>
    /// <typeparam name="PropertyType">The type of the property to be set.</typeparam>
    abstract class PropertyControllerBase<PropertyType>
    {
        /// <summary>
        /// Sets the property in this objects control. 
        /// </summary>
        /// <param name="lPropertyValue">The properties new value. </param>
        public abstract void SetProperty(PropertyType lPropertyValue);
    }

    /// <summary>
    /// Represents a control and a method to set the relevant parameter. 
    /// </summary>
    /// <typeparam name="PropertyType">The type of the property to be set. </typeparam>
    /// <typeparam name="ControlTyper">The type of the control (eg Control or ToolStripMenuItem for now).</typeparam>
    class PropertyController<PropertyType, ControlTyper> : PropertyControllerBase<PropertyType>
    {
        /// <summary>
        /// Type for function that sets the specific property on the control. 
        /// </summary>
        /// <param name="lControl">The control to set. </param>
        /// <param name="lPropertyValue">The value the property should take. </param>
        public delegate void PropertySetter(ControlTyper lControl, PropertyType lPropertyValue);

        /// <summary>
        /// Set the function to use when the property gets set. 
        /// </summary>
        public PropertySetter SetterFunction
        {
            get;
            set;
        }

        /// <summary>
        /// Create a new PropertyController with the object we want to control. 
        /// </summary>
        /// <param name="lControl">The object we want to control. </param>
        public PropertyController(ControlTyper lControl)
        {
            mControl = lControl;
        }

        /// <summary>
        /// Calls the relevant property setter method with the supplied value. 
        /// </summary>
        /// <param name="lPropertyValue">The value to set the property to. </param>
        public override void SetProperty(PropertyType lPropertyValue)
        {
            SetterFunction(mControl, lPropertyValue);
        }

        /// <summary>
        /// The control this controller is controlling. 
        /// </summary>
        ControlTyper mControl;
    }

    /// <summary>
    /// Factory methods for creating different controllers. 
    /// </summary>
    static class PropertyController
    {
        /// <summary>
        /// Creates a property controller for the enabled property in a Control. 
        /// </summary>
        /// <param name="lControl">The control we want to control. </param>
        /// <returns>The controller to control the control. </returns>
        public static PropertyController<bool, Control> CreateForEnabledControl(Control lControl)
        {
            var lController = new PropertyController<bool, Control>(lControl);
            lController.SetterFunction = (lControlToSet, lEnabled) => { lControlToSet.Enabled = lEnabled; };

            return lController;
        }

        /// <summary>
        /// Creates a property controller for the enabled property in a ToolStripMenuItem. 
        /// </summary>
        /// <param name="lControl">The ToolStripMenuItem we wish to control. </param>
        /// <returns>The controller for controlling the ToolStripMenuItem. </returns>
        public static PropertyController<bool, ToolStripMenuItem> CreateForEnabledToolStrip(ToolStripMenuItem lControl)
        {
            var lController = new PropertyController<bool, ToolStripMenuItem>(lControl);
            lController.SetterFunction = (lControlToSet, lEnabled) => { lControlToSet.Enabled = lEnabled; };

            return lController;
        }
    }
}
