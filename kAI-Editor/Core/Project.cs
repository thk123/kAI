using System;
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
    partial class kAIProject 
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

        DirectoryInfo mProjectRoot;

        /// <summary>
        /// The root directory of the project. 
        /// </summary>
        [DataMember(Order=0)]
        public DirectoryInfo ProjectRoot
        {
            get
            {
                return mProjectRoot;
            }
            set
            {
                mProjectRoot = value;
            }
        }

        List<kAIRelativePath> mProjectDLLPaths;

        /// <summary>
        /// A list of DLL's this project uses. 
        /// </summary>
        [DataMember(Order=1)]
        public List<kAIRelativePath> ProjectDllPaths
        {
            get
            {
                return mProjectDLLPaths;
            }
            private set
            {
                foreach (kAIRelativePath lDLLPath in value)
                {
                    LoadDLL(lDLLPath);
                }

                mProjectDLLPaths = value;
            }
        }

        /// <summary>
        /// The directory that contains all the XML behaviors.
        /// </summary>
        [DataMember(Order=0)]
        public kAIRelativeDirectory XmlBehaviourRoot
        {
            get;
            set;
        }

        /// <summary>
        /// A list of the behaviors in this project (both code and XML). 
        /// </summary>
        [DataMember(Order=1)]
        public Dictionary<string, kAIINodeSerialObject> NodeObjects
        {
            get;
            private set;
        }

        /// <summary>
        /// List of types this project uses (such as vectors).
        /// </summary>
        public List<Type> ProjectTypes
        {
            get;
            private set;
        }

        public List<MethodInfo> ProjectFunctions
        {
            get;
            private set;
        }

        [DataMember(Name="ProjectTypes", Order=1)]
        public IEnumerable<string> ProjectTypeStrings
        {
            get
            {
                return ProjectTypes.Select<Type, string>((lType) => { return lType.AssemblyQualifiedName; });
            }
            set 
            {
                foreach (string lString in value)
                {
                    ProjectTypes.Add(Type.GetType(lString, (lAssemblyName) => { return GetAssemblyByName(lAssemblyName.Name); }, null, true));

                }
            }
        }

        [DataContract()]
        public class SerialMethodInfo
        {
            /// <summary>
            /// The name of the method this function node corresponds to. 
            /// </summary>
            [DataMember()]
            public string MethodName;

            /// <summary>
            /// The decleraing type of the function. 
            /// </summary>
            [DataMember()]
            public string TypeName;

            /// <summary>
            /// The assembly of the declaring type of the function.
            /// </summary>
            [DataMember()]
            public string AssemblyName;

            public SerialMethodInfo(MethodInfo lMethod)
            {
                MethodName = lMethod.Name;
                TypeName = lMethod.DeclaringType.FullName;
                AssemblyName = lMethod.DeclaringType.Assembly.GetName().Name;
            }

            public MethodInfo Instantiate(kAIXmlBehaviour.GetAssemblyByName lAssemblyResolver)
            {
                Assembly lFunctionAssembly = lAssemblyResolver(AssemblyName);
                Type lDeclType = lFunctionAssembly.GetType(TypeName);
                return lDeclType.GetMethod(MethodName);
            }
        }

        [DataMember(Name = "ProjectFunctions", Order=1)]
        public IEnumerable<SerialMethodInfo> ProjectFunctionStrings
        {
            get
            {
                return ProjectFunctions.Select<MethodInfo, SerialMethodInfo>((lType) => { return new SerialMethodInfo(lType);  });
            }
            set
            {
                foreach (SerialMethodInfo lString in value)
                {
                    ProjectFunctions.Add(lString.Instantiate(GetAssemblyByName));

                }
            }
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
        [DataMember(Name = "AdditionalDLLPaths", Order=1)]
        Dictionary<string, kAIRelativePath> mAdditionalDllPaths
        {
            get
            {
                return mAdditionalDllPathsInt;
            }
            set
            {
                foreach (KeyValuePair<string, kAIRelativePath> lAdditionalPath in value)
                {
                    LoadDLL(lAdditionalPath.Value);
                }

                mAdditionalDllPathsInt = value;
            }
        }

        Dictionary<string, kAIRelativePath> mAdditionalDllPathsInt;

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
            ProjectDLLs = new List<Assembly>();
            ProjectFunctions = new List<MethodInfo>();

            mAdditionalDllPaths = new Dictionary<string, kAIRelativePath>();

            //NodeObjects.Add("FunctionNode", )

            // We also initialise all the types that wouldn't have come from the XML
            // We pass null as there is no file (yet). 
            Init(null);
        }

        /// <summary>
        /// Save out this project. 
        /// </summary>
        public void Save(kAIBehaviourID lOpenBehaviour)
        {
            // Settings for writing the XML file 
            XmlWriterSettings lSettings = new XmlWriterSettings();
            lSettings.Indent = true;
            lSettings.NamespaceHandling = NamespaceHandling.OmitDuplicates;

            using(XmlWriter lWriter = XmlWriter.Create(ProjectFile.FullName, lSettings))
            {
                // We serialise the project in to an XML file and save the changes
                XmlObjectSerializer lProjectSerialiser = new DataContractSerializer(typeof(kAIProject), kAINode.NodeSerialTypes);

                // Create the writer and write the file. 
                lProjectSerialiser.WriteObject(lWriter, this);
                //lWriter.Close();
            }

            // Write the meta file
            using (XmlWriter lWriter = XmlWriter.Create(ProjectFile.FullName + ".meta", lSettings))
            {
                MetaSaveFile lMetaSave = new MetaSaveFile();
                lMetaSave.OpenBehaviour = lOpenBehaviour;

                XmlObjectSerializer lMetaSerialiser = new DataContractSerializer(typeof(MetaSaveFile));
                lMetaSerialiser.WriteObject(lWriter, lMetaSave);
            }
            

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

            kAIRelativeObject.AddPathID(kBehaviourRootID, XmlBehaviourRoot.GetDirectory());
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

            AssemblyName lName = new AssemblyName(args.Name);
            return GetAssemblyByName(lName.Name);
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
            // Load using standard assembly load to ensure assembly image is all correct etc.
            Assembly lLoadedAssembly = Assembly.LoadFrom(lDllPath.GetFile().FullName);

            // Force the loading of the dll
            lLoadedAssembly.GetExportedTypes();

            return lLoadedAssembly;
        }

        [OnDeserializing]
        private void SetDefaultDllPaths(StreamingContext lStreamContext)
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            mAdditionalDllPaths = new Dictionary<string, kAIRelativePath>();
            ProjectTypes = new List<Type>();
            ProjectDLLs = new List<Assembly>();
            ProjectFunctions = new List<MethodInfo>();

        }

        /// <summary>
        /// Load a kaiProject from a given kAIProject XML.
        /// </summary>
        /// <param name="lProjectXml">The path to the kAIProject.</param>
        /// <returns>An instantiated kAIProject with the relevant properties. </returns>
        public static kAIProject Load(FileInfo lProjectXml, out kAIBehaviourID lBehaviourToLoad)
        {
            kAIRelativeDirectory.AddPathID(kProjectRootID, lProjectXml.Directory);

            DataContractSerializer lDeserialiser = new DataContractSerializer(typeof(kAIProject), kAINode.NodeSerialTypes);
            FileStream lStream = lProjectXml.OpenRead();
            kAIProject lNewProject = (kAIProject)lDeserialiser.ReadObject(lStream);

            lStream.Close();

            lNewProject.Init(lProjectXml);

            FileInfo lMetaFileInfo = new FileInfo(lProjectXml.FullName + ".meta");
            if (lMetaFileInfo.Exists)
            {
                using (FileStream lMetaStream = lMetaFileInfo.OpenRead())
                {
                    DataContractSerializer lMetaDeserialiser = new DataContractSerializer(typeof(MetaSaveFile));
                    MetaSaveFile lMetaFile = (MetaSaveFile)lMetaDeserialiser.ReadObject(lMetaStream);

                    lBehaviourToLoad = lMetaFile.OpenBehaviour;
                }
            }
            else
            {
                lBehaviourToLoad = null;
            }

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


                Assembly lFoundAssembly = ProjectDLLs.Find((lAssembly) =>
                {
                    return lAssembly.GetName().Name == lAssemblyName;
                });

                if (lFoundAssembly != null)
                {
                    return lFoundAssembly;
                }
                else
                {
                    if (mAdditionalDllPaths.ContainsKey(lAssemblyName))
                    {
                        return LoadAssemblyFromFilePath(mAdditionalDllPaths[lAssemblyName]);
                    }
                    else
                    {
                        kAIRelativePath lDllPath;
                        Assembly lLoadedAssembly = HandleMissingDll(lAssemblyName, out lDllPath);
                        if (lLoadedAssembly != null)
                        {
                            kAIObject.Assert(null, lDllPath, "Got an assembly but no corresponding path it was loaded from. ");
                            mAdditionalDllPaths.Add(lAssemblyName, lDllPath);
                        }

                        return lLoadedAssembly;
                    }

                    
                }
            }
        }


    }
}
