using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;

namespace kAI.Editor
{
    [DataContract(Name = "kAIProject")]
    class kAIProject 
    {
        public const string kProjectFileExtension = "kAIProj";

        [DataMember()]
        public string ProjectName
        {
            get;
            set;
        }

        [DataMember()]
        public DirectoryInfo ProjectRoot
        {
            get;
            set;
        }

        [DataMember()]
        public List<FileInfo> ProjectDlls
        {
            get;
            private set;
        }

        [DataMember()]
        public DirectoryInfo XmlBehaviourRoot
        {
            get;
            set;
        }

        [DataMember()]
        public List<kAIBehaviourTemplate> Behaviours
        {
            get;
            private set;
        }

        [DataMember()]
        public List<Type> ProjectTypes
        {
            get;
            private set;
        }

        public FileInfo ProjectFile
        {
            get
            {
                return new FileInfo(ProjectRoot + "\\" + ProjectName + "." + kProjectFileExtension);
            }
        }

        

        //TODO: List<Actions>

        /// <summary>
        /// Create the default project (eg with default entries).
        /// </summary>
        public kAIProject()
        {
            ProjectName = "NewProject";
            ProjectRoot = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\" + ProjectName);
            XmlBehaviourRoot = new DirectoryInfo(ProjectRoot.FullName + "\\Behaviours\\");

            ProjectDlls = new List<FileInfo>();
            Behaviours = new List<kAIBehaviourTemplate>();
            ProjectTypes = new List<Type>();
        }
    }
}
