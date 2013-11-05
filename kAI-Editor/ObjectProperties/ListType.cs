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
    /// Represents a list of objects for the property grid. 
    /// </summary>
    /// <typeparam name="T">The type of object being represented. </typeparam>
    /// <typeparam name="U">The property object for T, must be constructable from T. </typeparam>
    class kAISimplePropertyList<T, U> : ICustomTypeDescriptor
        where T : kAIObject
        where U : kAIIPropertyEntry
    {
        List<kAISimpleClickableEntry<T, U>> mObjects;

        /// <summary>
        /// Create a list from a list of things. 
        /// </summary>
        /// <param name="lObjects">The things to make the list out of. </param>
        public kAISimplePropertyList(IEnumerable<T> lObjects)
        {
            mObjects = new List<kAISimpleClickableEntry<T, U>>();
            foreach (T lEntry in lObjects)
            {
                mObjects.Add(new kAISimpleClickableEntry<T, U>(lEntry));
            }
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public PropertyDescriptorCollection GetProperties(System.Attribute[] attributes)
        {
            return GetProperties();
        }

        public PropertyDescriptorCollection GetProperties()
        {
            PropertyDescriptorCollection lCollection = new PropertyDescriptorCollection(null);

            foreach (kAISimpleClickableEntry<T, U> t in mObjects)
            {
                lCollection.Add(t);
            }

            return lCollection;

        }

        public EventDescriptorCollection GetEvents(System.Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public override string ToString()
        {
            return "Size: {" + mObjects.Count + "}";
        }
    }


}
