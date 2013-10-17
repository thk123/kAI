using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using kAI.Core;

namespace TestCodeBehaviours
{
    public class TestBehaviour : kAICodeBehaviour
    {

        public TestBehaviour(kAIILogger lLogger = null)
            : base(lLogger)
        {

        }

        int i = 0;

        protected override void InternalUpdate(float lDeltaTime)
        {
            LogMessage("Hello World!");
            ++i;

            if (i > 50)
            {
                Deactivate();
            }
        }
    }
}
