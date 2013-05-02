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
    [DataContract()]
    class kAIBehaviourTemplate
    {
        

        /// <summary>
        /// The type of this template, must point to a type that inherits from kAIBehaviour
        /// </summary>
        Type mBehaviourType;
        string mBehaviourTypeNameString;

        [DataMember(Name="BehaviourType")]
        string BehaviourTypeName
        {
            get
            {
                return mBehaviourType == null ? null : mBehaviourType.AssemblyQualifiedName;
            }
            set
            {
                mBehaviourTypeNameString = value;
            }
        }


        [DataMember()]
        FileInfo mBehaviourXmlFile;

        /// <summary>
        /// The flavor of this template. 
        /// </summary>
        [DataMember()]
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
                if (value.DoesInherit(typeof(kAIBehaviourBase)))
                {
                    mBehaviourType = value;
                }
                else
                {
                    throw new Exception("Invalid behaviour type");
                }
            }
        }

        [DataMember()]
        public kAIBehaviourID BehaviourName
        {
            get;
            private set;
        }

        public void SetType(kAIProject lProject)
        {
            mBehaviourType = Type.GetType(mBehaviourTypeNameString, (lName) => // Get the type by the name provided
            {
                return lProject.ProjectDLLs.Find((lAssembly) => // Find the assembly within the loaded assemblies
                {
                    return lAssembly.FullName == lName.FullName; // Where match is a full name match
                });
            }, 
            (lAssembly, lName, lMatch) => // Find the type within the assembly
            { 
                return lAssembly.GetType(lName, lMatch); // Just get the type
            }, true); // We really want errors. 
        }

        /// <summary>
        /// Create a template from a class that inherits from kAIBehaviour, eg a code behaviour. 
        /// </summary>
        /// <param name="lBehaviourType"></param>
        public kAIBehaviourTemplate(Type lBehaviourType)
        {
            BehaviourType = lBehaviourType;
            BehaviourFlavour = eBehaviourFlavour.BehaviourFlavour_Code;
            BehaviourName = lBehaviourType.Name;

            BehaviourSourceXml = null;
        }

        /// <summary>
        /// Creates a template from an XML file representing a kAIBehaviour. 
        /// </summary>
        /// <param name="lSourceFile">The file this behaviour is based on.</param>
        public kAIBehaviourTemplate(kAIXmlBehaviour lSourceFile)
        {
            BehaviourType = typeof(kAIXmlBehaviour);
            BehaviourFlavour = eBehaviourFlavour.BehaviourFlavour_Xml;
            BehaviourName = lSourceFile.BehaviourID;
        }

        /// <summary>
        /// Create the kAIBehaviour based on this template. 
        /// </summary>
        /// <returns>An instance of the behaviour specified in this template. </returns>
        public kAIBehaviour<T> Instantiate<T>()
        {
            if(BehaviourFlavour == eBehaviourFlavour.BehaviourFlavour_Code)
            {
                //TODO: This may need more types, eg does the behaviour need to know its node ID (probably not...? What about port connexions)
                ConstructorInfo lConstructor = mBehaviourType.GetConstructor(new Type[] { typeof(kAIILogger) });

                // We found a valid constructor!
                if (lConstructor != null)
                {
                    // So we make the thing for realz. 
                    kAIBehaviour<T> lBehaviour = (kAIBehaviour<T>)lConstructor.Invoke(new Object[] { null });
                    return lBehaviour;
                }
                else
                {
                    throw new Exception("Could not instantiate!");
                }
            }
            else
            {
                return null;
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
                if (lType.DoesInherit(typeof(kAIBehaviour<>)))
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
