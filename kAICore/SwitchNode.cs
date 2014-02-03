using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Reflection;
using System.Linq;
using System.Text;

namespace kAI.Core
{
    abstract class kAISwitchNode : kAINodeObject
    {
        protected kAISwitchNode(kAIILogger lLogger = null)
            : base(lLogger)
        {

        }

        public static kAISwitchNode CreateSwitchNode(Type lEnumType)
        {
            Type lSwitchType = typeof(kAISwitchNode<>);
            Type lGenericSwitch = lSwitchType.MakeGenericType(lEnumType);
            ConstructorInfo lSwitchConstructor = lGenericSwitch.GetConstructor(new Type[] { typeof(kAIILogger) });

            return (kAISwitchNode)lSwitchConstructor.Invoke(new object[] { null });
        }
    }

    class kAISwitchNode<T> : kAISwitchNode where T : struct, IConvertible
    {
        [DataContract()]
        public class SerialObject : kAIINodeSerialObject
        {
            [DataMember()]
            public SerialType EnumType;

            public SerialObject(kAISwitchNode<T> lNode)
            {
                EnumType = new SerialType(typeof(T));

            }

            public string GetFriendlyName()
            {
                return EnumType.TypeName;
            }

            public eNodeFlavour GetNodeFlavour()
            {
                return eNodeFlavour.Logical;
            }

            public kAIINodeObject Instantiate(kAIXmlBehaviour.GetAssemblyByName lAssemblyResolver)
            {
                return CreateSwitchNode(EnumType.Instantiate(lAssemblyResolver));
            }

        }

        public Type EnumType
        {
            get;
            private set;
        }

        kAIDataPort<T> mSwitchDataPort;

        kAITriggerPort[] mSwitchResultPorts;

        int mLastResult;

        public kAISwitchNode(kAIILogger lLogger = null)
            :base(lLogger)
        {
            EnumType = typeof(T);
            if (!EnumType.IsEnum)
            {
                throw new Exception("Switch must be on an enum");
            }


            mSwitchDataPort = new kAIDataPort<T>("Value", kAIPort.ePortDirection.PortDirection_In);
            AddExternalPort(mSwitchDataPort);

            T[] lEnumValues = (T[])Enum.GetValues(EnumType);
            mSwitchResultPorts = new kAITriggerPort[lEnumValues.Length];
            for (int i = 0; i < lEnumValues.Length; ++i)
            {
                mSwitchResultPorts[i] = new kAITriggerPort(lEnumValues[i].ToString(), kAIPort.ePortDirection.PortDirection_Out);
                AddExternalPort(mSwitchResultPorts[i]);
            }

            mLastResult = -1;
        }

       

        public override kAIINodeSerialObject GetDataContractClass(kAIXmlBehaviour lOwningBehaviour)
        {
            return new SerialObject(this);
        }

        public override Type GetDataContractType()
        {
            return typeof(SerialObject);
        }

        public override string GetNameTemplate()
        {
            return "Switch" + EnumType.Name;
        }

        public override void Update(float lDeltaTime, object lUserData)
        {
            T lValue = mSwitchDataPort.Data;
            int lValueAsInt = lValue.ToInt32(System.Globalization.CultureInfo.InvariantCulture);

            if (lValueAsInt != mLastResult)
            {
                if (lValueAsInt >= mSwitchResultPorts.Length)
                {
                    throw new Exception("Some how the data is not within the bounds of the enum");
                }

                mSwitchResultPorts[lValueAsInt].Trigger();
            }

            mLastResult = lValueAsInt;
        }
    }
}
