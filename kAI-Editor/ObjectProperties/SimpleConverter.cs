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
    /// A simple converter to show any object as a string. 
    /// </summary>
    /// <typeparam name="T">The type of object. </typeparam>
    /// <typeparam name="U">The type of the property representing the object. </typeparam>
    class kAISimpleConverter<T, U> : TypeConverter
        where T : kAIObject
        where U : kAIIPropertyEntry
    {

        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(kAISimpleClickableEntry<T, U>))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(String) && value is kAISimpleClickableEntry<T, U>)
            {
                return ((kAISimpleClickableEntry<T, U>)value).GetValue(null);
            }
            else
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }


    }

    /// <summary>
    /// A simple converter for an expandable object representing its root as just a string. 
    /// </summary>
    /// <typeparam name="T">The type of the object. </typeparam>
    class kAISimpleExpandableConverter<T> : ExpandableObjectConverter
    {
        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(T))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(String) && value is T)
            {
                return value.ToString();
            }
            else
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }
}
