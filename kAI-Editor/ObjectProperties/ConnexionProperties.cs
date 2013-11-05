using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using kAI.Core;

namespace kAI.Editor.ObjectProperties
{
    /// <summary>
    /// Represents the properties visible for a connexion.
    /// </summary>
    class kAIConnexionProperties : kAIIPropertyEntry
    {
        /// <summary>
        /// The start port. 
        /// </summary>
        [TypeConverterAttribute(typeof(kAISimpleConverter<kAIPort, kAIPortProperties>))]
        [DescriptionAttribute("The port this connexion starts at. ")]
        public kAISimpleClickableEntry<kAIPort, kAIPortProperties> StartPort
        {
            get
            {
                return new kAISimpleClickableEntry<kAIPort, kAIPortProperties>(mConnexion.StartPort);
            }
        }

        /// <summary>
        /// The end port. 
        /// </summary>
        [TypeConverterAttribute(typeof(kAISimpleConverter<kAIPort, kAIPortProperties>))]
        [DescriptionAttribute("The port this connexion ends at. ")]
        public kAISimpleClickableEntry<kAIPort, kAIPortProperties> EndPort
        {
            get
            {
                return new kAISimpleClickableEntry<kAIPort, kAIPortProperties>(mConnexion.EndPort);
            }
        }

        /// <summary>
        /// The connexion being represented here. 
        /// </summary>
        kAIPort.kAIConnexion mConnexion;

        /// <summary>
        /// Create a ConnexionProperties basic off a specific connexion. 
        /// </summary>
        /// <param name="lConnexion">The connexion to show the properties for. </param>
        public kAIConnexionProperties(kAIPort.kAIConnexion lConnexion)
        {
            mConnexion = lConnexion;
        }
    }
}
