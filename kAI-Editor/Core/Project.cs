﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;
using System.Reflection;
using System.Windows.Forms;

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

        public const string kProjectRootID = "ProjectRoot";
        public const string kBehaviourRootID = "BehaviourRoot";

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
        public List<kAIRelativePath> ProjectDllPaths
        {
            get;
            private set;
        }

        /// <summary>
        /// The directory that contains all the XML behaviors.
        /// </summary>
        [DataMember()]
        public kAIRelativeDirectory XmlBehaviourRoot
        {
            get;
            set;
        }

        /// <summary>
        /// A list of the behaviors in this project (both code and XML). 
        /// </summary>
        [DataMember()]
        public Dictionary<string, kAIINodeSerialObject> NodeObjects
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
        /// The location of the file of the project. 
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

        /// <summary>
        /// The locations of additional DLLs referenced by other DLLs. 
        /// </summary>
        [OptionalField(VersionAdded = 2)]
        [DataMember(Name = "AdditionalDLLPaths")]
        Dictionary<string, kAIRelativePath> mAdditionalDllPaths;

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
            XmlBehaviourRoot = new kAIRelativeDirectory("Behaviours", ProjectRoot, kProjectRootID);

            ProjectDllPaths = new List<kAIRelativePath>();
            NodeObjects = new Dictionary<string, kAIINodeSerialObject>();
            ProjectTypes = new List<Type>();

            mAdditionalDllPaths = new Dictionary<string, kAIRelativePath>();

            // We also initialise all the types that wouldn't have come from the XML
            // We pass null as there is no file (yet). 
            Init(null);
        }

        /// <summary>
        /// Save out this project. 
        /// </summary>
        public void Save()
        {
            // We serialise the project in to an XML file and save the changes
            XmlObjectSerializer lProjectSerialiser = new DataContractSerializer(typeof(kAIProject), kAINode.NodeSerialTypes);

            // Settings for writing the XML file 
            XmlWriterSettings lSettings = new XmlWriterSettings();
            lSettings.Indent = true;
            lSettings.NamespaceHandling = NamespaceHandling.OmitDuplicates;

            // Create the writer and write the file. 
            XmlWriter lWriter = XmlWriter.Create(ProjectFile.FullName, lSettings);
            lProjectSerialiser.WriteObject(lWriter, this);
            lWriter.Close();
        }

        /// <summary>
        /// Add a DLL to this project. 
        /// </summary>
        /// <param name="lDLLPath">The path to the DLL to load.</param>
        public void AddDLL(FileInfo lDLLPath)
        {
            kAIRelativePath lDllPathRelative = new kAIRelativePath(lDLLPath, ProjectRoot, kProjectRootID);
            if (lDllPathRelative.RelativePathType != kAIRelativeObject.eRelativePathType.InteriorPath)
            {
                DialogResult lResult = MessageBox.Show("This DLL is not located inside the path, would you like to copy it in?", "Exterior DLL Detected...", MessageBoxButtons.YesNo);
                if (lResult == DialogResult.Yes)
                {
                    DirectoryInfo lDllDirectory = new DirectoryInfo(ProjectRoot.FullName + @"\" + "DLLs");
                    if (!lDllDirectory.Exists)
                    {
                        lDllDirectory.Create();
                    }

                    FileInfo lNewPath = lDLLPath.CopyTo(lDllDirectory.FullName + @"\" + lDLLPath.Name);
                    lDllPathRelative = new kAIRelativePath(lNewPath, ProjectRoot, kProjectRootID);
                }
            }
            ProjectDllPaths.Add(lDllPathRelative);
            Assembly lAssembly = LoadDLL(lDllPathRelative);
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
                return lFileInfo.GetFile().FullName == lAssembly.Location;
            });

            // And unload it from loaded DLLs
            UnloadDLL(lAssembly);

        }

        /// <summary>
        /// Add an XML behaviour to the project. 
        /// </summary>
        /// <param name="lBehaviour">The serialised version of the behaviour to add. </param>
        public void AddXmlBehaviour(kAIINodeSerialObject lBehaviour)
        {
            NodeObjects.Add(lBehaviour.GetFriendlyName(), lBehaviour);
        }

        /// <summary>
        /// Check to see if there exists a behaviour in this project with the same behaviour ID. 
        /// </summary>
        /// <param name="lBehaviourID">The behaviour ID to check. </param>
        /// <returns>A boolean indicating if the name is avaliable, where true means the name is acceptable. </returns>
        public bool CheckBehaviourName(kAIBehaviourID lBehaviourID)
        {
            foreach (kAIINodeSerialObject lTemplate in NodeObjects.Values)
            {
                if (lTemplate.GetNodeFlavour().IsBehaviourFlavour())
                {
                    if (lTemplate.GetFriendlyName() == lBehaviourID)
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// Construct stuff that won't be loaded from the XML and hence won't be set up when we instantiate it. 
        /// </summary>
        /// <param name="lSouceFile">The file this project is loaded from. </param>
        private void Init(FileInfo lSouceFile)
        {
            mFile = lSouceFile;
            ProjectDLLs = new List<Assembly>();
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            kAIRelativeObject.AddPathID(kBehaviourRootID, XmlBehaviourRoot.GetDirectory());
        }

        /// <summary>
        /// Performs load operations on the project (eg the DLLs).
        /// </summary>
        private void Load()
        {
            foreach (kAIRelativePath lDLLPath in ProjectDllPaths)
            {
                LoadDLL(lDLLPath);
            }

            /*foreach (kAIBehaviourTemplate lTemplate in NodeObjects.Values)
            {
                if (lTemplate.BehaviourFlavour == eBehaviourFlavour.BehaviourFlavour_Code)
                {
                    lTemplate.SetType(this);
                }
            }*/
        }

        /// <summary>
        /// Load a specific DLL. 
        /// </summary>
        /// <param name="lDLLPath">The path of the DLL to load. </param>
        private Assembly LoadDLL(kAIRelativePath lDLLPath)
        {           
            Assembly lAssembly = LoadAssemblyFromFilePath(lDLLPath);            
            ProjectDLLs.Add(lAssembly);

            //Extract behaviors
            foreach (Type lType in lAssembly.GetExportedTypes())
            {
                if (lType.DoesInherit(typeof(kAIBehaviour)))
                {
                    kAIINodeSerialObject lSerialObject = kAICodeBehaviour.CreateSerialObjectFromType(lType);
                    if (!NodeObjects.ContainsKey(lSerialObject.GetFriendlyName()))
                    {
                        NodeObjects.Add(lSerialObject.GetFriendlyName(), lSerialObject);
                    }
                }
            }

            return lAssembly;
        }

        
        /// <summary>
        /// Unload a specific DLL. 
        /// </summary>
        /// <param name="lAssembly">The DLL to unload. </param>
        private void UnloadDLL(Assembly lAssembly)
        {
            List<kAIINodeSerialObject> lTemplatesToRemove = new List<kAIINodeSerialObject>();
            foreach (kAIINodeSerialObject lTemplate in NodeObjects.Values)
            {
                if (lTemplate.GetNodeFlavour() == eNodeFlavour.BehaviourCode)
                {
                    //TODO: need to somehow remove??
                    /*Type lUnderlyingType = lTemplate.;
                    if (lUnderlyingType.Assembly.Equals(lAssembly))
                    {
                        lTemplatesToRemove.Add(lTemplate);
                    }*/
                }
            }

            foreach (kAIINodeSerialObject lTemplate in lTemplatesToRemove)
            {
                NodeObjects.Remove(lTemplate.GetFriendlyName());
            }

            ProjectDLLs.Remove(lAssembly);
        }

        /// <summary>
        /// This is triggered if we try to load a DLL that references a DLL that is unknown to CLR
        /// </summary>
        /// <param name="sender">Sender. </param>
        /// <param name="args">Contains, amongst other things, the FullName of the DLL we are looking for. </param>
        /// <returns>The Assembly (or null if we have still failed to find it -- this will throw an exception). </returns>
        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains(".resources"))
                return null;

            // see if it is one our our referenced dlls
            foreach (Assembly lLoadedAssembly in ProjectDLLs)
            {
                if (lLoadedAssembly.FullName == args.Name)
                {
                    return lLoadedAssembly;
                }
            }

            if (mAdditionalDllPaths.ContainsKey(args.Name))
            {
                return LoadAssemblyFromFilePath(mAdditionalDllPaths[args.Name]);
            }
            else
            {
                kAIRelativePath lDllPath;
                Assembly lLoadedAssembly = HandleMissingDll(args.Name, out lDllPath);
                if (lLoadedAssembly != null)
                {
                    kAIObject.Assert(null, lDllPath, "Got an assembly but no corresponding path it was loaded from. ");
                    mAdditionalDllPaths.Add(args.Name, lDllPath);
                }

                return lLoadedAssembly;
            }
        }

        /// <summary>
        /// When we can't find a DLL, thsi method asks the user to find it. 
        /// </summary>
        /// <param name="lAssemblyFullName">The full name of the assembly. </param>
        /// <param name="lDllPath">We will fill this with the path with the location of the assembly.</param>
        /// <returns>The loaded assembly if found, null if otherwise (e.g. the user clicks cancel). </returns>
        private Assembly HandleMissingDll(string lAssemblyFullName, out kAIRelativePath lDllPath)
        {
            OpenFileDialog lOFD = new OpenFileDialog();
            lOFD.Title = "Find missing DLL: " + lAssemblyFullName;
            if (lOFD.ShowDialog() == DialogResult.OK)
            {
                FileInfo lDllPathUnRel = new FileInfo(lOFD.FileName);
                lDllPath = new kAIRelativePath(lDllPathUnRel, ProjectRoot, kProjectRootID);
                return LoadAssemblyFromFilePath(lDllPath);
                
            }
            else
            {
                lDllPath = null;
                return null;
            }
        }

        /// <summary>
        /// Given a path to a DLL, loads it in to an Assembly. 
        /// </summary>
        /// <param name="lDllPath">The path to the DLL. </param>
        /// <returns>The loaded assembly at that position. </returns>
        private Assembly LoadAssemblyFromFilePath(kAIRelativePath lDllPath)
        {
            FileStream lDLLStream = lDllPath.GetFile().OpenRead();
            byte[] lDLLArray = new byte[lDLLStream.Length];
            lDLLStream.Read(lDLLArray, 0, (int)lDLLStream.Length);
            lDLLStream.Close();
            Assembly lLoadedAssembly = Assembly.Load(lDLLArray);

            // Force the loading of the dll
            lLoadedAssembly.GetExportedTypes();

            return lLoadedAssembly;
        }

        [OnDeserializing]
        private void SetDefaultDllPaths(StreamingContext lStreamContext)
        {
            mAdditionalDllPaths = new Dictionary<string, kAIRelativePath>();
            
        }

        /// <summary>
        /// Load a kaiProject from a given kAIProject XML.
        /// </summary>
        /// <param name="lProjectXml">The path to the kAIProject.</param>
        /// <returns>An instantiated kAIProject with the relevant properties. </returns>
        public static kAIProject Load(FileInfo lProjectXml)
        {
            kAIRelativeDirectory.AddPathID(kProjectRootID, lProjectXml.Directory);

            DataContractSerializer lDeserialiser = new DataContractSerializer(typeof(kAIProject), kAINode.NodeSerialTypes);
            FileStream lStream = lProjectXml.OpenRead();
            kAIProject lNewProject = (kAIProject)lDeserialiser.ReadObject(lStream);
            lNewProject.ProjectDLLs = new List<Assembly>();

            lStream.Close();

            lNewProject.Init(lProjectXml);
            lNewProject.Load();

            return lNewProject;
        }


        //TODO: Change this string to AssemblyName, maybe not, seemed to cause problems 
        /// <summary>
        /// Resolve an assembly name to an assembly (either loaded or referenced by the kAIProject. 
        /// </summary>
        /// <param name="lAssemblyName">The full name of the assembly</param>
        /// <returns>The assmebly if it is found somewhere. </returns>
        public Assembly GetAssemblyByName(string lAssemblyName)
        {
            if (lAssemblyName == Assembly.GetExecutingAssembly().GetName().Name)
            {
                return Assembly.GetExecutingAssembly();
            }
            else
            {
                foreach (AssemblyName lRefdAssemblyName in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
                {
                    if (lRefdAssemblyName.Name == lAssemblyName)
                    {
                        Assembly lAssembly = Assembly.Load(lRefdAssemblyName);
                        if (lAssembly != null)
                        {
                            return lAssembly;
                        }
                    }
                }


                return ProjectDLLs.Find((lAssembly) =>
                {
                    return lAssembly.GetName().Name == lAssemblyName;
                });
            }
        }


    }
}
