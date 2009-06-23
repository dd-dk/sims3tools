/***************************************************************************
 *  Copyright (C) 2009 by Peter L Jones                                    *
 *  pljones@users.sf.net                                                   *
 *                                                                         *
 *  This file is part of the Sims 3 Package Interface (s3pi)               *
 *                                                                         *
 *  s3pi is free software: you can redistribute it and/or modify           *
 *  it under the terms of the GNU General Public License as published by   *
 *  the Free Software Foundation, either version 3 of the License, or      *
 *  (at your option) any later version.                                    *
 *                                                                         *
 *  s3pi is distributed in the hope that it will be useful,                *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of         *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
 *  GNU General Public License for more details.                           *
 *                                                                         *
 *  You should have received a copy of the GNU General Public License      *
 *  along with s3pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using s3pi.Interfaces;
using System.Collections;
using System.Drawing.Design;
using System.Reflection;

namespace S3PIDemoFE
{
    public class S3PIPropertyGrid : PropertyGrid
    {
        AApiVersionedFieldsCTD target;
        public S3PIPropertyGrid() : base() { PropertySort = PropertySort.NoSort; HelpVisible = false; ToolbarVisible = false; }

        public AApiVersionedFields s3piObject { get { return target; } set { if (value != null) target = new AApiVersionedFieldsCTD(value); SelectedObject = target; } }
    }


    // Need to convert this to a PropertyGrid
    // http://msdn.microsoft.com/en-us/library/aa302334.aspx
    // http://www.codeproject.com/KB/tabs/customizingcollectiondata.aspx?display=Print

    [TypeConverter(typeof(AApiVersionedFieldsCTDConverter))]
    public class AApiVersionedFieldsCTD : AApiVersionedFields, ICustomTypeDescriptor
    {
        AApiVersionedFields s3piObject;
        public AApiVersionedFields Value { get { return s3piObject; } }

        public AApiVersionedFieldsCTD(AApiVersionedFields value) { this.s3piObject = value; }
        public override List<string> ContentFields { get { return s3piObject.ContentFields; } }
        public override int RecommendedApiVersion { get { return s3piObject.RecommendedApiVersion; } }

        #region ICustomTypeDescriptor Members

        public AttributeCollection GetAttributes()
        {
            AttributeCollection ac = TypeDescriptor.GetAttributes(this, true);
            if (ac.Contains(new TypeConverterAttribute(typeof(AApiVersionedFieldsCTDConverter)))) return ac;
            Attribute[] newAttrs = new Attribute[ac.Count + 1];
            newAttrs[0] = new TypeConverterAttribute(typeof(AApiVersionedFieldsCTDConverter));
            ac.CopyTo(newAttrs, 1);
            return new AttributeCollection(newAttrs);
        }

        public string GetClassName() { return TypeDescriptor.GetClassName(this, true); }

        public string GetComponentName() { return TypeDescriptor.GetComponentName(this, true); }

        public TypeConverter GetConverter() { return TypeDescriptor.GetConverter(this, true); }

        public EventDescriptor GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(this, true); }

        public System.ComponentModel.PropertyDescriptor GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(this, true); }

        public object GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(this, editorBaseType, true); }

        public EventDescriptorCollection GetEvents(Attribute[] attributes) { return TypeDescriptor.GetEvents(this, attributes, true); }

        public EventDescriptorCollection GetEvents() { return TypeDescriptor.GetEvents(this, true); }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) { return GetProperties(); }

        public PropertyDescriptorCollection GetProperties()
        {
            PropertyDescriptorCollection pc = new PropertyDescriptorCollection(null);
            foreach (string s in s3piObject.ContentFields)
            {
                if (s.Equals("Stream") || s.Equals("AsBytes") || s.Equals("Value")) continue;
                if (s.Equals("Count") || s.Equals("IsReadOnly"))
                    foreach (Type i in s3piObject.GetType().GetInterfaces())
                        if (i.Name.StartsWith("IList`")) goto skip;

                PropertyDescriptor pd = new PropertyDescriptor(s3piObject, s, null);
                pc.Add(pd);
            skip: { }
            }
            return pc;
        }

        public object GetPropertyOwner(System.ComponentModel.PropertyDescriptor pd) { return this; }

        #endregion

        public class PropertyDescriptor : System.ComponentModel.PropertyDescriptor
        {
            AApiVersionedFields owner;
            string field;
            public PropertyDescriptor(AApiVersionedFields owner, string field, Attribute[] attr)
                : base(field, attr) { this.owner = owner; this.field = field; }

            public override bool CanResetValue(object component) { return false; }

            public override void ResetValue(object component) { throw new InvalidOperationException(); }

            public override Type PropertyType
            {
                get
                {
                    Type t = AApiVersionedFields.GetContentFieldTypes(0, ComponentType)[field];
                    if (t.IsEnum) return t; // Don't want these falling into the next section
                    if (t.IsPrimitive)
                        try
                        {
                            Convert.ChangeType(owner[field].Value, typeof(ulong));
                            return typeof(AsHexConverterCTD);
                        }
                        catch { }
                    if (t.IsValueType) return t; // It's a struct
                    if (t.Name.StartsWith("IList`") && typeof(AApiVersionedFields).IsAssignableFrom(t.GetGenericArguments()[0]))
                        return typeof(IListCTD);
                    if (typeof(AApiVersionedFields).IsAssignableFrom(t)) return typeof(AApiVersionedFieldsCTD);
                    //if (typeof(Boolset).IsAssignableFrom(t)) return t; Hmm...
                    return t;
                }
            }

            public override object GetValue(object component)
            {
                Type t = PropertyType;
                if (t.Equals(typeof(AsHexConverterCTD))) return new AsHexConverterCTD(owner, field, component);
                if (t.Equals(typeof(IListCTD))) return new IListCTD(owner, field, component);
                if (t.Equals(typeof(AApiVersionedFieldsCTD))) return new AApiVersionedFieldsCTD((AApiVersionedFields)owner[field].Value);
                return owner[field].Value;
            }

            public override bool IsReadOnly { get { return !ComponentType.GetProperty(field).CanWrite; } }

            public override void SetValue(object component, object value)
            {
                if (IsReadOnly) throw new InvalidOperationException();
                TypedValue tv = new TypedValue(value.GetType(), value, "X");
                owner[field] = tv;
            }

            public override Type ComponentType { get { return owner.GetType(); } }

            public override bool ShouldSerializeValue(object component) { return true; }
        }

        public class AApiVersionedFieldsCTDConverter : ExpandableObjectConverter
        {
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType.Equals(typeof(string))) return true;
                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType.Equals(typeof(string))) return "";
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }

    [TypeConverter(typeof(AsHexConverter))]
    public class AsHexConverterCTD : ICustomTypeDescriptor
    {
        AApiVersionedFields owner;
        string field;
        object component;
        public AsHexConverterCTD(AApiVersionedFields owner, string field, object component) { this.owner = owner; this.field = field; this.component = component; }

        #region ICustomTypeDescriptor Members

        public AttributeCollection GetAttributes() { return TypeDescriptor.GetAttributes(this, true); }

        public string GetClassName() { return TypeDescriptor.GetClassName(this, true); }

        public string GetComponentName() { return TypeDescriptor.GetComponentName(this, true); }

        public TypeConverter GetConverter() { return TypeDescriptor.GetConverter(this, true); }

        public EventDescriptor GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(this, true); }

        public System.ComponentModel.PropertyDescriptor GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(this, true); }

        public object GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(this, editorBaseType, true); }

        public EventDescriptorCollection GetEvents(Attribute[] attributes) { return TypeDescriptor.GetEvents(this, attributes, true); }

        public EventDescriptorCollection GetEvents() { return TypeDescriptor.GetEvents(this, true); }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) { return GetProperties(); }

        public PropertyDescriptorCollection GetProperties()
        {
            PropertyDescriptorCollection pc = new PropertyDescriptorCollection(null);
            pc.Add(new PropertyDescriptor(owner, field, component, null));
            return pc;
        }

        public object GetPropertyOwner(System.ComponentModel.PropertyDescriptor pd) { return this; }

        #endregion

        public class PropertyDescriptor : System.ComponentModel.PropertyDescriptor
        {
            AApiVersionedFields owner;
            string field;
            object component;
            public PropertyDescriptor(AApiVersionedFields owner, string field, object component, Attribute[] attr)
                : base(field, attr) { this.owner = owner; this.field = field; this.component = component; }

            public override string Name { get { return field; } }

            public override bool CanResetValue(object component) { return false; }

            public override void ResetValue(object component) { throw new InvalidOperationException(); }

            public override Type PropertyType { get { return typeof(AsHexConverter); } }

            public override object GetValue(object component) { return new AsHexConverter(); }

            public override bool IsReadOnly { get { return true; } }

            public override void SetValue(object component, object value) { throw new InvalidOperationException(); }

            public override Type ComponentType { get { return null; } }

            public override bool ShouldSerializeValue(object component) { return true; }
        }

        public class AsHexConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType.Equals(typeof(string))) return true;
                return base.CanConvertTo(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                if (value.GetType().Equals(typeof(string)))
                {
                    try
                    {
                        AApiVersionedFields owner = ((AApiVersionedFieldsCTD)context.Instance).Value;
                        string field = context.PropertyDescriptor.Name;
                        string asString = (string)value;
                        ulong res = Convert.ToUInt64(asString, (asString.StartsWith("0x")) ? 16 : 10);
                        return Convert.ChangeType(res, AApiVersionedFields.GetContentFieldTypes(0, owner.GetType())[field]);
                    }
                    catch { }
                }
                return base.ConvertFrom(context, culture, value);
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType.Equals(typeof(string))) return true;
                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                if (value as AsHexConverterCTD != null && destinationType.Equals(typeof(string)))
                {
                    try
                    {
                        AApiVersionedFields owner = ((AApiVersionedFieldsCTD)context.Instance).Value;
                        string field = ((AsHexConverterCTD)value).field;
                        int i = System.Runtime.InteropServices.Marshal.SizeOf(owner[field].Type) * 2;
                        string format = "0x{0:X" + i + "}";
                        return String.Format(format, Convert.ToUInt64(owner[field].Value));
                    }
                    catch { }
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }

    public class IListCTD : IList, ICollection, IEnumerable, IEnumerator, ICustomTypeDescriptor
    {
        protected AApiVersionedFields owner;
        protected string field;
        protected object component;

        protected TypedValue tv; // tv.Value is the list
        protected Type TIList;
        protected Type IListT;

        public IListCTD(AApiVersionedFields owner, string field, object component)
        {
            this.owner = owner; this.field = field; this.component = component;
            tv = owner[field];
            TIList = tv.Value.GetType();
            IListT = tv.Type.GetGenericArguments()[0];
        }

        #region IList Members

        public object this[int index]
        {
            get
            {
                System.Reflection.MethodInfo m = TIList.GetMethod("get_Item", new Type[] { typeof(int) });
                object o = m.Invoke(tv.Value, new object[] { index });
                return new AApiVersionedFieldsCTD((AApiVersionedFields)o);
            }
            set
            {
                object o = value;
                if (typeof(AApiVersionedFieldsCTD).IsAssignableFrom(value.GetType())) o = ((AApiVersionedFieldsCTD)value).Value;
                if (!IListT.IsAssignableFrom(value.GetType())) throw new InvalidCastException();
                System.Reflection.MethodInfo m = TIList.GetMethod("set_Item", new Type[] { typeof(int), IListT });
                m.Invoke(tv.Value, new object[] { index, value });
            }
        }

        public int Add(object value)
        {
            if (!IListT.IsAssignableFrom(value.GetType())) throw new InvalidCastException();
            System.Reflection.MethodInfo m = TIList.GetMethod("Add", new Type[] { IListT });
            m.Invoke(tv.Value, new object[] { value });
            return Count - 1;
        }

        public void Clear()
        {
            System.Reflection.MethodInfo m = TIList.GetMethod("Clear", new Type[] { });
            m.Invoke(tv.Value, new object[] { });
        }

        public bool Contains(object value)
        {
            System.Reflection.MethodInfo m = TIList.GetMethod("Contains", new Type[] { value.GetType() });
            return (bool)m.Invoke(tv.Value, new object[] { value });
        }

        public int IndexOf(object value)
        {
            System.Reflection.MethodInfo m = TIList.GetMethod("IndexOf", new Type[] { value.GetType() });
            return (int)m.Invoke(tv.Value, new object[] { value });
        }

        public void Insert(int index, object value)
        {
            System.Reflection.MethodInfo m = TIList.GetMethod("Insert", new Type[] { typeof(int), value.GetType() });
            m.Invoke(tv.Value, new object[] { index, value });
        }

        public bool IsFixedSize
        {
            get
            {
                System.Reflection.MethodInfo m = TIList.GetMethod("get_IsFixedSize", new Type[] { });
                return (bool)m.Invoke(tv.Value, new object[] { });
            }
        }

        public bool IsReadOnly { get { return false; } }

        public void Remove(object value)
        {
            if (!IListT.IsAssignableFrom(value.GetType())) throw new InvalidCastException();
            System.Reflection.MethodInfo m = TIList.GetMethod("Remove", new Type[] { IListT });
            m.Invoke(tv.Value, new object[] { value });
        }

        public void RemoveAt(int index)
        {
            System.Reflection.MethodInfo m = TIList.GetMethod("RemoveAt", new Type[] { typeof(int) });
            m.Invoke(tv.Value, new object[] { index });
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            System.Reflection.MethodInfo m = TIList.GetMethod("CopyTo", new Type[] { array.GetType(), typeof(int) });
            m.Invoke(tv.Value, new object[] { array, index });
        }

        public int Count
        {
            get
            {
                System.Reflection.MethodInfo m = TIList.GetMethod("get_Count", new Type[] { });
                return (int)m.Invoke(tv.Value, new object[] { });
            }
        }

        public bool IsSynchronized
        {
            get
            {
                System.Reflection.MethodInfo m = TIList.GetMethod("get_IsSynchronized", new Type[] { });
                return (bool)m.Invoke(tv.Value, new object[] { });
            }
        }

        public object SyncRoot
        {
            get
            {
                System.Reflection.MethodInfo m = TIList.GetMethod("get_SyncRoot", new Type[] { });
                return m.Invoke(tv.Value, new object[] { });
            }
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return this;
            //System.Reflection.MethodInfo m = TIList.GetMethod("GetEnumerator", new Type[] { });
            //return (IEnumerator)m.Invoke(tv.Value, new object[] { });
        }

        #endregion

        #region IEnumerator Members

        int enumeratorCurrent = -1;
        public object Current { get { return this[enumeratorCurrent]; } }

        public bool MoveNext() { enumeratorCurrent++; return enumeratorCurrent < Count; }

        public void Reset() { enumeratorCurrent = -1; }

        #endregion

        #region ICustomTypeDescriptor Members

        public AttributeCollection GetAttributes() { return TypeDescriptor.GetAttributes(this, true); }

        public string GetClassName() { return TypeDescriptor.GetClassName(this, true); }

        public string GetComponentName() { return TypeDescriptor.GetComponentName(this, true); }

        public TypeConverter GetConverter() { return TypeDescriptor.GetConverter(this, true); }

        public EventDescriptor GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(this, true); }

        public System.ComponentModel.PropertyDescriptor GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(this, true); }

        public object GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(this, editorBaseType, true); }

        public EventDescriptorCollection GetEvents(Attribute[] attributes) { return TypeDescriptor.GetEvents(this, attributes, true); }

        public EventDescriptorCollection GetEvents() { return TypeDescriptor.GetEvents(this, true); }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) { return GetProperties(); }

        public PropertyDescriptorCollection GetProperties()
        {
            PropertyDescriptorCollection pc = new PropertyDescriptorCollection(null);
            int count = Count;
            for (int i = 0; i < count; i++)
                pc.Add(new PropertyDescriptor(this, i));
            return pc;
        }

        public object GetPropertyOwner(System.ComponentModel.PropertyDescriptor pd) { return this; }

        #endregion

        public class PropertyDescriptor : System.ComponentModel.PropertyDescriptor
        {
            IListCTD list;
            int index;

            public PropertyDescriptor(IListCTD list, int index) : base("[" + index + "]", null) { this.list = list; this.index = index; }

            public override AttributeCollection Attributes { get { return new AttributeCollection(null); } }

            public override bool CanResetValue(object component) { return false; }

            public override Type ComponentType { get { return typeof(IListCTD); } }

            public override string DisplayName { get { return ""; } }

            public override string Description { get { return ""; } }

            public override object GetValue(object component) { return list[index]; }

            public override bool IsReadOnly { get { return false; } }

            public override string Name { get { return "[" + index + "]"; } }

            public override Type PropertyType
            {
                get
                {
                    object value = GetValue(null);
                    if (typeof(TypedValue).IsAssignableFrom(list.IListT))
                        return typeof(AApiVersionedFields).IsAssignableFrom(((TypedValue)value).Type) ? typeof(AApiVersionedFieldsCTD) : ((TypedValue)value).Type;
                    return typeof(AApiVersionedFields).IsAssignableFrom(list.IListT) ? typeof(AApiVersionedFieldsCTD) : list.IListT;
                }
            }

            public override void ResetValue(object component) { }

            public override bool ShouldSerializeValue(object component) { return true; }

            public override void SetValue(object component, object value)
            {
                object o;
                if (typeof(TypedValue).IsAssignableFrom(list.IListT))
                {
                    o = typeof(AApiVersionedFieldsCTD).IsAssignableFrom(value.GetType()) ? ((AApiVersionedFieldsCTD)value).Value : value;
                    list[index] = new TypedValue(o.GetType(), o, "X");
                }
                else
                    list[index] = value;
            }
        }
    }
}
