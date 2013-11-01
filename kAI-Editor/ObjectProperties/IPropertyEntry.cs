using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kAI.Editor.ObjectProperties
{
    /// <summary>
    /// Represents a Property class for a specific class. 
    /// </summary>
    interface kAIIPropertyEntry
    {
    }

    /// <summary>
    /// Represents a property that can be double clicked to select a different object. 
    /// </summary>
    interface kAIIClickableProperty
    {
        kAIIPropertyEntry GetSelectedProperty();
    }
}
