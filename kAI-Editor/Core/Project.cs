using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;
using System.Reflection;

using kAI.Core;
using kAI.Editor.Core.Util;

namespace kAI.Editor.Core
{
    /// <summary>
    /// Represents a loaded kAIProject.
    /// </summary>
    [DataContract(Name = "kAIProject")]
    class kAIProject 
    {
        /// <summary>
        /// The extension of a project file. 
        /// </summary>
        public const string kProjectFileExtension = "kAIProj";

        /// <summary>
        /// The name of the project. 
        /// </summary>
        [DataMember()]
        public string ProjectName
        {
            get;
            set;
        }

        /// <summary>
        /// The root directory of the project. 
        /// </summary>
        [DataMember()]
        public DirectoryInfo ProjectRoot
        {
            get;
            set;
        }

        /// <summary>
        /// A list of DLL's this project uses. 
        /// </summary>
        [DataMember()]
        public List<FileInfo> ProjectDllPaths
        {
            get;
            private set;
        }

        /// <summary>
        /// The directory that contains all the XML behaviors. 
        /// </summary>
        [DataMember()]
        public DirectoryInfo XmlBehaviourRoot
        {
            get;
            set;
        }

        /// <summary>
        /// A list of the behaviors in this project (both code and XML). 
        /// </summary>
        [DataMember()]
        public List<kAIBehaviourTemplate> Behaviours
        {
            get;
            private set;
        }

        /// <summary>
        /// List of types this project uses (such as vectors).
        /// </summary>
        [DataMember()] //TODO: This won't work :P
        public List<Type> ProjectTypes
        {
            get;
            private set;
        }

        /// <summary>
        /// The locatino of the file of the project. 
        /// </summary>
        public FileInfo ProjectFile
        {
            get
            {
                // If we don't actually have a file, we supply a name based on the project root and project name
                if (mFile == null)
                {
                    return new FileInfo(ProjectRoot + "\\" + ProjectName + "." + kProjectFileExtension);
                }
                else // Otherwise we just return the project file. 
                {
                    return mFile;
                }
            }
        }

        /// <summary>
        /// The list of DLLs that have been loaded. 
        /// </summary>
        public List<Assembly> ProjectDLLs
        {
            get;
            private set;
        }

        //TODO: List<Actions>


        /// <summary>
        /// The file backing up this project. 
        /// </summary>
        FileInfo mFile;

        /// <summary>
        /// Create the default project (eg with default entries).
        /// </summary>
        public kAIProject()
        {
            ProjectName = "NewProject";
            ProjectRoot = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\" + ProjectName);
            XmlBehaviourRoot = new DirectoryInfo(ProjectRoot.FullName + "\\Behaviours\\");

            ProjectDllPaths = new List<FileInfo>();
            Behaviours = new List<kAIBehaviourTemplate>();
            ProjectTypes = new List<Type>();

            // We also initialise all the types that wouldn't have come from the XML
            // We pass null as there is no file (yet). 
            Init(null);
        }

        /// <summary>
        /// Add a DLL to this project. 
        /// </summary>
        /// <param name="lDLLPath">The path to the DLL to load.</param>
        public void AddDLL(FileInfo lDLLPath)
        {
            ProjectDllPaths.Add(lDLLPath);
            Assembly lAssembly = LoadDLL(lDLLPath);

            //Extract behaviors
            foreach (Type lType in lAssembly.GetExportedTypes())
            {
                if (lType.DoesInherit(typeof(kAIBehaviour)))
                {
                    kAIBehaviourTemplate lTemplate = new kAIBehaviourTemplate(lType);
                    Behaviours.Add(lTemplate);
                }
            }
        }

        /// <summary>
        /// Remove a DLL from this project. 
        /// </summary>
        /// <param name="lAssembly">The DLL to remove. </param>
        public void RemoveDLL(Assembly lAssembly)
        {
            // Remove the matching file
            ProjectDllPaths.RemoveAll((lFileInfo) => 
                { 
                    return lFileInfo.FullName == lAssembly.Location; 
                } );

            // And unload it from loaded DLLs
            UnloadDLL(lAssembly);

        }

        public void AddXmlBehaviour(kAIXmlBehaviour lBehaviour)
        {
            //Behaviours.Add(new kAIBehaviourTemplate())
        }


        /// <summary>
        /// Construct stuff that won't be loaded from the XML and hence won't be set up when we instantiate it. 
        /// </summary>
        /// <param name="lSouceFile">The file this project is loaded from. </param>
        private void Init(FileInfo lSouceFile)
        {
            mFile = lSouceFile;
            ProjectDLLs = new List<Assembly>();
        }

        /// <summary>
        /// Performs load operations on the project (eg the DLLs).
        /// </summary>
        private void Load()
        {
            foreach (FileInfo lDLLPath in ProjectDllPaths)
            {
                LoadDLL(lDLLPath);
            }
        }

        /// <summary>
        /// Load a specific DLL. 
        /// </summary>
        /// <param name="lDLLPath">The path of the DLL to load. </param>
        private Assembly LoadDLL(FileInfo lDLLPath)
        {
            FileStream lDLLStream = lDLLPath.OpenRead();
            byte[] lDLLArray = new byte[lDLLStream.Length];
            lDLLStream.Read(lDLLArray, 0, (int)lDLLStream.Length);
            
            Assembly lAssembly = Assembly.Load(lDLLArray);

            ProjectDLLs.Add(lAssembly);

            return lAssembly;
        }

        /// <summary>
        /// Unload a specific DLL. 
        /// </summary>
        /// <param name="lAssembly">The DLL to unload. </param>
        private void UnloadDLL(Assembly lAssembly)
        {
            List<kAIBehaviourTemplate> lTemplatesToRemove = new List<kAIBehaviourTemplate>();
            foreach (kAIBehaviourTemplate lTemplate in Behaviours)
            {
                if (lTemplate.BehaviourFlavour == kAIBehaviourTemplate.eBehaviourFlavour.BehaviourFlavour_Code)
                {
                    Type lUnderlyingType = lTemplate.BehaviourType;
                    if (lUnderlyingType.Assembly.Equals(lAssembly))
                    {
                        lTemplatesToRemove.Add(lTemplate);
                    }
                }
            }

            foreach (kAIBehaviourTemplate lTemplate in lTemplatesToRemove)
            {
                Behaviours.Remove(lTemplate);
            }

            ProjectDLLs.Remove(lAssembly);
        }

        /// <summary>
        /// Load a kaiProject from a given kAIProject XML.
        /// </summary>
        /// <param name="lProjectXml">The path to the kAIProject.</param>
        /// <returns>An instantiated kAIProject with the relevant properties. </returns>
        public static kAIProject Load(FileInfo lProjectXml)
        {
            DataContractSerializer lDeserialiser = new DataContractSerializer(typeof(kAIProject));
            FileStream lStream = lProjectXml.OpenRead();
            kAIProject lNewProject = (kAIProject)lDeserialiser.ReadObject(lStream);
            lNewProject.ProjectDLLs = new List<Assembly>();

            lStream.Close();

            lNewProject.Init(lProjectXml);
            lNewProject.Load();

            return lNewProject;
        }
    }
}
