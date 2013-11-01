using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using kAI.Core;

namespace kAI.Editor.ObjectProperties
{
    /// <summary>
    /// Represents an entry in the property grid that is double-clickable. 
    /// </summary>
    /// <typeparam name="T">The object being represented. </typeparam>
    /// <typeparam name="U">The property object for T. Must be constructable from T. </typeparam>
    class kAISimpleClickableEntry<T, U> : PropertyDescriptor, kAIIClickableProperty
        where T : kAIObject
        where U : kAIIPropertyEntry
    {
        public override Type ComponentType
        {
            get { return typeof(System.String); }
        }

        public override bool IsReadOnly
        {
            get { return true; }
        }

        public override Type PropertyType
        {
            get { return typeof(T); }

        }

        T mValue;

        public kAISimpleClickableEntry(T lSource)
            : base(typeof(T).Name, null)
        {
            mValue = lSource;
        }

        /// <summary>
        /// Gets the property class of the selectable object. 
        /// </summary>
        /// <returns>An object of type U that represents the T this was made of. </returns>
        public kAIIPropertyEntry GetSelectedProperty()
        {
            ConstructorInfo lUConstructor = typeof(U).GetConstructor(new Type[] { typeof(T) });
            if (lUConstructor == null)
            {
                throw new Exception(typeof(U).Name + " must be constructable from " + typeof(T).Name);
            }
            return (kAIIPropertyEntry)lUConstructor.Invoke(new object[] { mValue });
        }

        public override void SetValue(object component, object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override object GetValue(object component)
        {
            return mValue.ToString();
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override void ResetValue(object component)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}