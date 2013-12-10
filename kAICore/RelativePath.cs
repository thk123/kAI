using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;

namespace kAI.Core
{
    /// <summary>
    /// Represents a relative path to either file or folder. 
    /// </summary>
    [DataContract()]
    [System.Diagnostics.DebuggerDisplay("{mPath}")]
    public abstract class kAIRelativeObject
    {
        /// <summary>
        /// The type of path this has been stored as. 
        /// </summary>
        public enum eRelativePathType
        {
            /// <summary>
            /// The file is contained with the root directory. 
            /// </summary>
            InteriorPath,

            /// <summary>
            /// The file is not contained within the root directory, but shares some common path.
            /// </summary>
            ExteriorPath,

            /// <summary>
            /// The file is not even on the same drive so the path is just some global path. 
            /// </summary>
            SeperatePath
        }

        /// <summary>
        /// Has the path been made relative
        /// </summary>
        public eRelativePathType RelativePathType
        {
            get;
            private set;
        }

        /// <summary>
        /// The stored path of the object. 
        /// </summary>
        [DataMember(Name = "Path")]
        protected string mPath;

        /// <summary>
        /// The stored ID of the object. 
        /// </summary>
        [DataMember(Name = "RootID")]
        string mRootID;

        /// <summary>
        /// The actual root location this path is relative to. 
        /// </summary>
        protected DirectoryInfo mRootDirectory;

        /// <summary>
        /// The mapping of Directory ID's to real DirectoryPaths for this load. 
        /// </summary>
        static Dictionary<string, DirectoryInfo> sRootDirectories;

        /// <summary>
        /// Create the RootDirectories mapping. 
        /// </summary>
        static kAIRelativeObject()
        {
            sRootDirectories = new Dictionary<string, DirectoryInfo>();
        }

        /// <summary>
        /// Create the relative object to a given file. 
        /// </summary>
        /// <param name="lFilePath">The file paths. </param>
        /// <param name="lRelativeToDirectory">The directory we should store the path relative to on this machine. </param>
        /// <param name="lRootID">The ID of the directory to be used to look up the location on different machines. </param>
        protected kAIRelativeObject(FileInfo lFilePath, DirectoryInfo lRelativeToDirectory, string lRootID)
            :this(lFilePath.Directory, lRelativeToDirectory, lRootID)
        {
            mPath += @"\" + lFilePath.Name;
        }

        /// <summary>
        /// Creates a relative object to a given folder. 
        /// </summary>
        /// <param name="lDirectoryPath">The location of the folder. </param>
        /// <param name="lRelativeToDirectory">The directory we should store the path relative to on this machine. </param>
        /// <param name="lRootID">The ID of the directory to be used to look up the location on different machines. </param>
        protected kAIRelativeObject(DirectoryInfo lDirectoryPath, DirectoryInfo lRelativeToDirectory, string lRootID)
        {
            DirectoryInfo lFileRootDirectory = lDirectoryPath.Root;
            DirectoryInfo lRelativeRoot = lRelativeToDirectory.Root;

            if (lFileRootDirectory.FullName == lRelativeRoot.FullName)
            {
                if (lDirectoryPath.FullName.StartsWith(lRelativeToDirectory.FullName))
                {
                    // we are a sub folder, so just remove this starting portion
                    mPath = lDirectoryPath.FullName.Remove(0, lRelativeToDirectory.FullName.Length);

                    RelativePathType = eRelativePathType.InteriorPath;
                }
                else
                {
                    int lCommonDepth;
                    Stack<DirectoryInfo> lFileChain = GetDirectoryChain(lDirectoryPath);
                    Stack<DirectoryInfo> lRelativeToChain = GetDirectoryChain(lRelativeToDirectory);
                    DirectoryInfo lCommonDir = FindLastCommonDirectory(lFileChain, lRelativeToChain, out lCommonDepth);

                    var lFilePathEnumer = lFileChain.GetEnumerator();

                    // Advance to the point the where they differ
                    for (int i = 0; i < lCommonDepth; ++i)
                    {
                        lFilePathEnumer.MoveNext();
                    }

                    StringBuilder lRelativePath = new StringBuilder();

                    for (int i = 0; i < lRelativeToChain.Count - lCommonDepth; ++i)
                    {
                        lRelativePath.Append(@"\..");
                    }

                    while (lFilePathEnumer.MoveNext())
                    {
                        lRelativePath.Append(@"\" + lFilePathEnumer.Current.Name);
                    }

                    mPath = lRelativePath.ToString() + @"\" + lDirectoryPath.Name;

                    RelativePathType = eRelativePathType.ExteriorPath;
                }

            }
            else
            {
                // Warning: no relative path exists between things in different drives
                mPath = lDirectoryPath.FullName;
                RelativePathType = eRelativePathType.SeperatePath;
            }

            mRootDirectory = lRelativeToDirectory;
            mRootID = lRootID;
        }

        /// <summary>
        /// Find a directory satisfying:
        ///  - Contained in DirectoryA's chain
        ///  - Contained in DirectoryB's chain
        ///  - There is no such directory lower in the heirachy. 
        /// </summary>
        /// <param name="lDirectoryAChain">The stack of folders for the first location (with the item at the bottom of the stack corresponding to the item lowest in the hierachy). </param>
        /// <param name="lDirectoryBChain">The stack of folders for the second location (with the item at the bottom of the stack corresponding to the item lowest in the hierachy). </param>
        /// <param name="lCommonDepth">The number of layers we were able to go from the root before the paths differed. </param>
        /// <returns>The last Directory that contains both the last elements of the two stacks. </returns>
        private DirectoryInfo FindLastCommonDirectory(Stack<DirectoryInfo> lDirectoryAChain, Stack<DirectoryInfo> lDirectoryBChain, out int lCommonDepth)
        {

            var lADirPathEnumer = lDirectoryAChain.GetEnumerator();
            var lBDirPathEnumer = lDirectoryBChain.GetEnumerator();
            lCommonDepth = 0;

            bool lADirPathAdvance = lADirPathEnumer.MoveNext();
            bool lBDirPathAdvance = lBDirPathEnumer.MoveNext();

            while (lADirPathAdvance && lBDirPathAdvance)
            {
                // We have found the first differing directory, so last directory was last that matched
                if (lADirPathEnumer.Current.FullName != lBDirPathEnumer.Current.FullName)
                {
                    return lADirPathEnumer.Current.Parent;
                }

                ++lCommonDepth;
                lADirPathAdvance = lADirPathEnumer.MoveNext();
                lBDirPathAdvance = lBDirPathEnumer.MoveNext();

            }

            if (lADirPathAdvance)
            {
                return lADirPathEnumer.Current;
            }
            else if (lBDirPathAdvance)
            {
                return lBDirPathEnumer.Current;
            }

            return null;
        }

        /// <summary>
        /// Creates a stack for a given directory where the base element is that directory, the one above its parent etc. 
        /// </summary>
        /// <param name="lDirectory">The directory to build up from. </param>
        /// <returns>A stack of the chain of directories from lDirectory up to the root. </returns>
        private Stack<DirectoryInfo> GetDirectoryChain(DirectoryInfo lDirectory)
        {
            Stack<DirectoryInfo> lDirectoryChain = new Stack<DirectoryInfo>();
            DirectoryInfo lDirCurrent = lDirectory;
            do
            {
                lDirectoryChain.Push(lDirCurrent);
                lDirCurrent = lDirCurrent.Parent;
            } while (lDirCurrent != lDirectory.Root.Parent);

            return lDirectoryChain;
        }

        /// <summary>
        /// If we haven't configured the root directory this load, we set it based on the ID. 
        /// </summary>
        protected void SetRootDirectory()
        {
            if (mRootDirectory == null)
            {
                mRootDirectory = sRootDirectories[mRootID];
            }
        }

        /// <summary>
        /// Configures the path for a specific ID on this machine. 
        /// </summary>
        /// <param name="lDirectoryID">The Directory ID this directory corresponds to. </param>
        /// <param name="lDirectory">The location of the directory on this machine.</param>
        public static void AddPathID(string lDirectoryID, DirectoryInfo lDirectory)
        {
            if (!sRootDirectories.ContainsKey(lDirectoryID))
            {
                sRootDirectories.Add(lDirectoryID, lDirectory);
            }
            else
            {
                if (sRootDirectories[lDirectoryID].FullName == lDirectory.FullName)
                {
                    // TODO: Probably don't want to keep adding it every time we load. 
                }
                else
                {
                    throw new Exception("Already set directory for ID: " + lDirectoryID);
                }
            }
        }

    }

    /// <summary>
    /// A path for some file relative to some file.  
    /// </summary>
    [DataContract(Name="FilePath")]
    public class kAIRelativePath : kAIRelativeObject
    {
        /// <summary>
        /// Create a new relative path to a file. 
        /// </summary>
        /// <param name="lPath">The file we want to store the path for. </param>
        /// <param name="lRelativeToDirectory">The root directory we want the path to be relative to. </param>
        /// <param name="lRootID">The ID of the directory to be used to look up the location on different machines. </param>
        public kAIRelativePath(FileInfo lPath, DirectoryInfo lRelativeToDirectory, string lRootID)
            :base(lPath,lRelativeToDirectory, lRootID)
        {}

        /// <summary>
        /// Create a new relative path to a file that is in the relative to directory. 
        /// </summary>
        /// <param name="lFileName">The name of the file.</param>
        /// <param name="lRelativeToDirectory">The directory the file is in and should be relative to. </param>
        /// <param name="lRootID">The ID of the directory to be used to look up the location on different machines. </param>
        public kAIRelativePath(string lFileName, DirectoryInfo lRelativeToDirectory, string lRootID)
            : this(new FileInfo(lRelativeToDirectory.FullName + @"\" + lFileName), lRelativeToDirectory, lRootID)
        {}

        /// <summary>
        /// Get the actual location of the file. 
        /// </summary>
        /// <returns>The FileInfo corresponding to the file. </returns>
        public FileInfo GetFile()
        {
            SetRootDirectory();
            if (RelativePathType == eRelativePathType.SeperatePath)
            {
                return new FileInfo(mPath);
            }
            else
            {
                return new FileInfo(mRootDirectory.FullName + mPath);
            }
        }
    }

    /// <summary>
    /// A path for some file relative to some directory. 
    /// </summary>
    [DataContract(Name = "DirectoryPath")]
    public class kAIRelativeDirectory : kAIRelativeObject
    {
        /// <summary>
        /// Create a new relative path to a directory. 
        /// </summary>
        /// <param name="lDirPath">The directory we want to store the path for. </param>
        /// <param name="lRelativeToDirectory">The root directory we want the path to be relative to. </param>
        /// <param name="lRootID">The ID of the directory to be used to look up the location on different machines. </param>
        public kAIRelativeDirectory(DirectoryInfo lDirPath, DirectoryInfo lRelativeToDirectory, string lRootID)
            : base(lDirPath, lRelativeToDirectory, lRootID)
        {}

        /// <summary>
        /// Create a new relative path to a directory. 
        /// </summary>
        /// <param name="lDirName">The name of the directory. </param>
        /// <param name="lRelativeToDirectory">The location of the directory and what the path should be relative to. </param>
        /// <param name="lRootID">The ID of the directory to be used to look up the location on different machines. </param>
        public kAIRelativeDirectory(string lDirName, DirectoryInfo lRelativeToDirectory, string lRootID)
            : this(new DirectoryInfo(lRelativeToDirectory.FullName + @"\" + lDirName), lRelativeToDirectory, lRootID)
        {}

        /// <summary>
        /// Get the actual location of the directory. . 
        /// </summary>
        /// <returns>The DirectoryInfo corresponding to the directory. </returns>
        public DirectoryInfo GetDirectory()
        {
            SetRootDirectory();
            if (RelativePathType == eRelativePathType.SeperatePath)
            {
                return new DirectoryInfo(mPath);
            }
            else
            {
                return new DirectoryInfo(mRootDirectory.FullName + mPath);
            }
        }
    }
}
