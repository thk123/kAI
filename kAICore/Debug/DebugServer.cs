using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Winterdom.IO.FileMap;
using ThreadMessaging;

using MemoryMappedFile = Winterdom.IO.FileMap.MemoryMappedFile;

namespace kAI.Core.Debug
{
    /// <summary>
    /// The manager for the overall debug file which contains what AI entities are available. 
    /// </summary>
    public static class kAIDebugServer
    {
        const int kEntrySize = 50 * 3; // 50 characters per string * 3 strings

        /// <summary>
        /// The size of the main file. 
        /// </summary>
        public const int kMainFileSize = 100 * kEntrySize ; // 100 behaviours * size of entyr

        /// <summary>
        /// The ID of the main file 
        /// </summary>
        public const string kMainFileID = "kAIDebug.MainFile";

        /// <summary>
        /// The ID of the semaphore you should use when accessing the main file. 
        /// </summary>
        public const string kMainFileLockID = "kAIDebug.MainLock";

        static MemoryMappedFile mMainFile;
        static ProcessSemaphore mMainFileLock;
        
        static Dictionary<string, kAIBehaviourEntry> behaviours;

        /// <summary>
        /// Is the Server initialised and running. 
        /// </summary>
        public static bool IsInit = false;

        /// <summary>
        /// Start up the debug server.
        /// </summary>
        /// <param name="processID">
        /// The ID of the process (used if multiple instances of your game running).
        /// Currently not implemented. 
        /// </param>
        public static void Init(string processID)
        {
            behaviours = new Dictionary<string, kAIBehaviourEntry>();

            mMainFile = MemoryMappedFile.Create(MapProtection.PageReadWrite, kMainFileSize, kMainFileID);
            mMainFileLock = new ProcessSemaphore(kMainFileLockID, 1, 1);

            IsInit = true;
        }

        /// <summary>
        /// Add a behaviour to the list of behaviours that can be debugged. 
        /// </summary>
        /// <param name="lBehaviour">The XmlBehaviour the AI entity is using. </param>
        /// <param name="objectID">The ID of the AI entity. </param>
        /// <returns>The debug store to update with new debug info. </returns>
        public static kAIBehaviourDebugStore AddBehaviour(kAIXmlBehaviour lBehaviour, string objectID)
        {
            if (behaviours.ContainsKey(objectID))
            {
                kAIObject.LogWarning(null, "Could not add behaviour as ID: " + objectID + " already exists");
                return null;
            }
            else
            {
                kAIBehaviourDebugStore lNewStore = new kAIBehaviourDebugStore(lBehaviour, objectID);
                AddToList(lNewStore);

                return lNewStore;
            }
        }

        /// <summary>
        /// Add the debug store to the dictionary and the list that will make it visible to external tools. 
        /// </summary>
        /// <param name="lStore">The debug store to add. </param>
        static void AddToList(kAIBehaviourDebugStore lStore)
        {
            kAIBehaviourEntry lEntry = new kAIBehaviourEntry { EntryID = lStore.ID, FileID = lStore.MapID, LockID = lStore.LockID };

            behaviours.Add(lStore.ID, lEntry);

            mMainFileLock.Acquire();
            {
                using (Stream lFile = mMainFile.MapView(MapAccess.FileMapWrite, 0, kMainFileSize))
                {
                    BinaryFormatter lFormatter = new BinaryFormatter();
                    lFormatter.Serialize(lFile, behaviours.Values);
                    lFile.Flush();
                }
            }
            mMainFileLock.Release();
        }

        /// <summary>
        /// Release relevant resources. 
        /// </summary>
        public static void Deinit()
        {
            mMainFile.Close();
            mMainFileLock.Dispose();
        }
    }

    /// <summary>
    /// An entry in the list of behaviours in the debug server. 
    /// Provides details of the memory mapped file corresponding to this AI entity. 
    /// </summary>
    [Serializable]
    public struct kAIBehaviourEntry
    {
        /// <summary>
        /// The ID of the AI entity. 
        /// </summary>
        public string EntryID;

        /// <summary>
        /// The ID of the sempahore you should use to gain access to the memory mapped file. 
        /// </summary>
        public string LockID;

        /// <summary>
        /// The ID of the memory mapped file which will contain a <see cref="kAIXmlBehaviourDebugInfo"/>
        /// </summary>
        public string FileID;

        /// <summary>
        /// Gets a string represenation of the object. 
        /// </summary>
        /// <returns>The ID of the agent. </returns>
        public override string ToString()
        {
            return EntryID;
        }
    }
}
