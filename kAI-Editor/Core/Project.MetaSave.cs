using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using kAI.Core;

namespace kAI.Editor.Core
{
    partial class kAIProject 
    {
        /// <summary>
        /// Additional information about the project such as which file was last open. 
        /// </summary>
        [DataContract(Name="ProjectMetaFile")]
        class MetaSaveFile
        {
            /// <summary>
            /// The ID of the last open behaviour. 
            /// </summary>
            [DataMember()]
            public kAIBehaviourID OpenBehaviour;
        }
    }
}
