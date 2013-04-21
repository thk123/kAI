using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;

using kAI.Core;
using kAI.Editor.Core;
using kAI.Editor.Core.Util;
using System.IO;

namespace kAI.Editor.Core
{
    /// <summary>
    /// Represents a template with which to create a behaviour from. 
    /// </summary>
    [DataContract(Name = "kAIBehaviourTemplate")]
    class kAIBehaviourTemplate
    {
        /// <summary>
        /// The flavour of behaviour. 
        /// </summary>
        public enum eBehaviourFlavour
        {
            /// <summary>
            /// A code behaviour - one extracted from a dll.
            /// </summary>
            BehaviourFlavour_Code, 

            /// <summary>
            /// A behaviour created within kAI-Editor, mainly the compilation of other behaviors/actions. 
            /// </summary>
            BehaviourFlavour_Xml, 
        }

        /// <summary>
        /// The type of this template, must point to a type that inherits from kAIBehaviour
        /// </summary>
        Type mBehaviourType;

        [DataMember()]
        FileInfo mBehaviourXmlFile;

        /// <summary>
        /// The flavor of this template. 
        /// </summary>
        public eBehaviourFlavour BehaviourFlavour
        {
            get;
            private set;
        }

        /// <summary>
        /// Where is the XML file representing this behaviour located. 
        /// </summary>
        public FileInfo BehaviourSourceXml
        {
            get
            {
                if (BehaviourFlavour == eBehaviourFlavour.BehaviourFlavour_Code)
                {
                    throw new Exception("No source directory for code behaviours");
                }
                else
                {
                    return mBehaviourXmlFile;
                }
            }
            private set
            {
                mBehaviourXmlFile = value;
            }
        }

        /// <summary>
        /// The underlying Type of this behaviour, eg kAIBehaviour for XML, the class for a Code behaviour. 
        /// </summary>
        public Type BehaviourType
        {
            get
            {
                return mBehaviourType;
            }
            private set
            {
                // Here we check that the type inherits from kAIBehaviour
                if (value.DoesInherit(typeof(kAIBehaviour)))
                {
                    mBehaviourType = value;
                }
                else
                {
                    throw new Exception("Invalid behaviour type");
                }
            }
        }

        /// <summary>
        /// Create a template from a class that inherits from kAIBehaviour, eg a code behaviour. 
        /// </summary>
        /// <param name="lBehaviourType"></param>
        public kAIBehaviourTemplate(Type lBehaviourType)
        {
            BehaviourType = lBehaviourType;
            BehaviourFlavour = eBehaviourFlavour.BehaviourFlavour_Code;
            BehaviourSourceXml = null;
        }

        /// <summary>
        /// Creates a template from an XML file representing a kAIBehaviour. 
        /// </summary>
        /// <param name="lSourceFile">The file this behaviour is based on.</param>
        public kAIBehaviourTemplate(FileInfo lSourceFile)
        {
            BehaviourType = typeof(kAIBehaviour); //TEMP: Replace with typeof(XmlBehaviour)
            BehaviourFlavour = eBehaviourFlavour.BehaviourFlavour_Xml;
            BehaviourSourceXml = lSourceFile;
        }

        /// <summary>
        /// Create the kAIBehaviour based on this template. 
        /// </summary>
        /// <returns>An instance of the behaviour specified in this template. </returns>
        public kAIBehaviour Instantiate()
        {
            //TODO: This may need more types, eg does the behaviour need to know its node ID (probably not...? What about port connexions)
            ConstructorInfo lConstructor = mBehaviourType.GetConstructor(new Type[] { typeof(kAINodeID), typeof(kAIILogger) });

            // We found a valid constructor!
            if (lConstructor != null)
            {
                // So we make the thing for realz. 
                kAIBehaviour lBehaviour = (kAIBehaviour)lConstructor.Invoke(new Object[] { new kAINodeID("TemporaryNode"), null } );
                return lBehaviour;
            }
            else
            {
                throw new Exception("Could not instantiate!");
            }
        }

        /// <summary>
        /// Returns a friendly name of this templated behaviour. 
        /// </summary>
        /// <returns>Name of the behaviour</returns>
        public override string ToString()
        {
            if (mBehaviourType != null)
            {
                return mBehaviourType.Name;
            }
            else
            {
                // TODO: If we are an XML type then we should find out name
                return "Unknown Behaviour";
            }
        }

        /// <summary>
        /// Read from an assembly all classes that inherit from kAIBehavour and make them into templates. 
        /// </summary>
        /// <param name="lDLLAssembly">The assembly to parse.</param>
        /// <returns>A list of templates of behaviours from this DLL. </returns>
        public static List<kAIBehaviourTemplate> ReadBehavioursFromDll(Assembly lDLLAssembly)
        {
            List<kAIBehaviourTemplate> lList = new List<kAIBehaviourTemplate>();

            // Go through all the classes
            foreach (Type lType in lDLLAssembly.GetTypes())
            {
                // Find out if they are a behaviour
                if (lType.DoesInherit(typeof(kAIBehaviour)))
                {
                    // If they are, great, we make a template from them.
                    kAIBehaviourTemplate lNewTemplate = new kAIBehaviourTemplate(lType);

                    lList.Add(lNewTemplate);
                }
            }

            return lList;
        }
    }
}
