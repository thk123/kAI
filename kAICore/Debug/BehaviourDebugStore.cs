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
    /// Represents a specific behaviours debug store.
    /// Manages access to the memory mapped file for a specific behaviour. 
    /// </summary>
    public class kAIBehaviourDebugStore
    {
        /// <summary>
        /// The size of the memory mapped file for a specific behaviour. 
        /// </summary>
        public const int kBehaviourFileSize = 32 * 1024;

        kAIXmlBehaviour mBehaviour;

        /// <summary>
        /// The unique ID of the object
        /// </summary>
        public string ID
        {
            get;
            private set;
        }

        MemoryMappedFile mBehaviourFile;
        ProcessSemaphore mBehaviourLock;

        /// <summary>
        /// The name of the memory mapped file. 
        /// </summary>
        public string MapID
        {
            get
            {
                return "kAIDebug.BehaviourMap." + ID;
            }
        }

        /// <summary>
        /// The name of the semaphore that controls access to the memory mapped file. 
        /// </summary>
        public string LockID
        {
            get
            {
                return "kAIDebug.BehaviourLock." + ID;
            }
        }

        /// <summary>
        /// Create a debug store to manage the memory mapped file for this behaviour.
        /// </summary>
        /// <param name="lBehaviour">The behaviour we are tracking. </param>
        /// <param name="lObjectID">The unqiue ID to identify this amgonst other AI entities. </param>
        public kAIBehaviourDebugStore(kAIXmlBehaviour lBehaviour, string lObjectID)
        {
            ID = lObjectID;
            mBehaviour = lBehaviour;
            mBehaviourFile = MemoryMappedFile.Create(MapProtection.PageReadWrite, kBehaviourFileSize, MapID);
            mBehaviourLock = new ProcessSemaphore(LockID, 1, 1);
        }

        /// <summary>
        /// Update the contents of the memory mapped file with the latest debug info from the behaviour.
        /// </summary>
        public void Update()
        {
            kAIXmlBehaviourDebugInfo lDebugInfo = (kAIXmlBehaviourDebugInfo)mBehaviour.GenerateDebugInfo();
            mBehaviourLock.Acquire();
            {
                using (Stream lFile = mBehaviourFile.MapView(MapAccess.FileMapWrite, 0, kBehaviourFileSize))
                {
                    BinaryFormatter lFormatter = new BinaryFormatter();
                    lFormatter.Serialize(lFile, lDebugInfo);
                    lFile.Flush();
                }
            }
            mBehaviourLock.Release();
        }

        /// <summary>
        /// Release all file and locks.
        /// </summary>
        internal void Deinit()
        {
            mBehaviourFile.Close();
            mBehaviourLock.Dispose();
        }
    }
}
