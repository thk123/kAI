using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kAI.Core;

namespace kAITestBehaviours
{
    public class kAITestBehaviour : kAIBehaviour
    {
        public kAITestBehaviour()
        {
            AddPort(new kAIPort("MyPort", kAIPort.ePortDirection.PortDirection_In, null));
            AddPort(new kAIPort("MyOutPort", kAIPort.ePortDirection.PortDirection_Out, null));
        }
    }
}
