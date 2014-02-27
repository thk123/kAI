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
    /// Handles the memory mapped files from the debugger side. 
    /// </summary>
    public class kAIDebugger : IDisposable
    {
        MemoryMappedFile mMainFile;
        ProcessSemaphore mMainFileLock;

        /// <summary>
        /// Create a debugger for a target process. 
        /// </summary>
        /// <param name="processID">
        /// The ID of the process. This can be used when mutliple instances of the game.
        /// Currently not implemented. 
        /// </param>
        public kAIDebugger(string processID)
        {
            mMainFile = MemoryMappedFile.Open(MapAccess.FileMapRead, kAIDebugServer.kMainFileID);
            mMainFileLock = new ProcessSemaphore(kAIDebugServer.kMainFileLockID, 1, 1);
        }

        /// <summary>
        /// Gets a list of AI entities currently available for debug. 
        /// </summary>
        /// <returns>A list of behaviour entries that contain IDs for their specific memory mapped files. </returns>
        public IEnumerable<kAIBehaviourEntry> GetAvaliableBehaviours()
        {
            IEnumerable<kAIBehaviourEntry> lBehaviourList;
            
            mMainFileLock.Acquire();
            {
                using (Stream inStream = mMainFile.MapView(MapAccess.FileMapRead, 0, kAIDebugServer.kMainFileSize))
                {
                    BinaryFormatter writer = new BinaryFormatter();
                    //object something = writer.Deserialize(inStream);
                    lBehaviourList = (IEnumerable<kAIBehaviourEntry>)writer.Deserialize(inStream);
                }
            }
            mMainFileLock.Release();

            return lBehaviourList;
        }

        /// <summary>
        /// Take a specific entry and load the debug info associated with it. 
        /// </summary>
        /// <param name="lEntry">The entry to load. </param>
        /// <returns>The debug info associated with that entry. </returns>
        public kAIXmlBehaviourDebugInfo LoadEntry(kAIBehaviourEntry lEntry)
        {
            kAIXmlBehaviourDebugInfo lBehaviour = null;
            ProcessSemaphore lBehaviourLock = new ProcessSemaphore(lEntry.LockID, 1, 1);
            lBehaviourLock.Acquire();
            {
                try
                {
                    MemoryMappedFile lFile = MemoryMappedFile.Open(MapAccess.FileMapRead, lEntry.FileID);
                    using (Stream inStream = lFile.MapView(MapAccess.FileMapRead, 0, kAIBehaviourDebugStore.kBehaviourFileSize))
                    {
                        BinaryFormatter writer = new BinaryFormatter();
                        //object something = writer.Deserialize(inStream);
                        lBehaviour = (kAIXmlBehaviourDebugInfo)writer.Deserialize(inStream);

                    }
                }
                catch(FileMapIOException e)
                {
                    kAIObject.GlobalLogger.LogError("IO Exception reading debug file: " + e.Message);
                }
                finally
                {
                    lBehaviourLock.Release();
                }
            }
            

            return lBehaviour;
        }

        /// <summary>
        /// Release resources associated with the main file. 
        /// </summary>
        public void Dispose()
        {
            mMainFileLock.Dispose();
            mMainFile.Dispose();
        }
    }
}
