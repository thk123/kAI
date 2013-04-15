using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kAI.Core;

namespace kAITestBehaviours
{
    public class kAITestBehaviour : kAIBehaviour
    {
        public kAITestBehaviour(kAINodeID lNodeId, kAIILogger lLogger = null)
            :base(lNodeId, lLogger)
        {
            AddPort(new kAIPort("MyPort", kAIPort.ePortDirection.PortDirection_In, null));
            AddPort(new kAIPort("MyPort", kAIPort.ePortDirection.PortDirection_Out, null));
            /*AddPort(new kAIPort("MyPort", kAIPort.ePortDirection.PortDirection_Out, null));*/

        }

        public override void Update(float lDeltaTime)
        {
            LogMessage("Delta Time: ", lDeltaTime);
        }
    }
}
