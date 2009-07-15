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
using System.IO;
using System.Windows.Forms;
using s3pi.Interfaces;
using System.Collections;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms.Design;

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
    [Category("Fields")]
    public class AApiVersionedFieldsCTD : AApiVersionedFields, ICustomTypeDescriptor
    {
        AApiVersionedFields s3piObject;
        public AApiVersionedFields Value { get { return s3piObject; } }

        public AApiVersionedFieldsCTD(AApiVersionedFields value) { this.s3piObject = value; }
        public override List<string> ContentFields { get { return s3piObject.ContentFields; } }
        public override int RecommendedApiVersion { get { return s3piObject.RecommendedApiVersion; } }

        #region ICustomTypeDescriptor Members

        //DisplayNameAttribute(this.Value.GetType().Name)
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
            foreach (string s in s3piObject.ContentFields)
            {
                if (s.Equals("Stream") || s.Equals("AsBytes") || s.Equals("Value")) continue;
                if (s.Equals("Count") || s.Equals("IsReadOnly"))
                    foreach (Type i in s3piObject.GetType().GetInterfaces())
                        if (i.Name.StartsWith("IList`")) goto skip;

                Attribute[] ac = null;
                if (typeof(AApiVersionedFields).IsAssignableFrom(AApiVersionedFields.GetContentFieldTypes(0, s3piObject.GetType())[s]))
                    ac = new Attribute[] { new TypeConverterAttribute(typeof(AApiVersionedFieldsCTDConverter)), };
                PropertyDescriptor pd = new PropertyDescriptor(s3piObject, s, ac);
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

            public override string DisplayName { get { return field; } }

            public override string Name { get { return field; } }

            public override string Description { get { return field; } }

            public override string Category { get { return owner.GetType().Name; } }

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
                    if (typeof(Boolset).IsAssignableFrom(t)) return typeof(BoolsetConverterCTD); // before lists
                    if (typeof(IEnumerable).IsAssignableFrom(t) && t.GetGenericArguments().Length == 1 && typeof(AApiVersionedFields).IsAssignableFrom(t.GetGenericArguments()[0]))
                        return typeof(IListCTD);
                    if (typeof(AApiVersionedFields).IsAssignableFrom(t)) return typeof(AApiVersionedFieldsCTD);
                    if (typeof(TextReader).IsAssignableFrom(t) || typeof(BinaryReader).IsAssignableFrom(t)) return typeof(ReaderCTD);
                    return t;
                }
            }

            public override object GetValue(object component)
            {
                Type t = PropertyType;
                if (t.Equals(typeof(AsHexConverterCTD))) return new AsHexConverterCTD(owner, field, component);
                if (t.Equals(typeof(BoolsetConverterCTD))) return new BoolsetConverterCTD(owner, field, component);
                if (t.Equals(typeof(IListCTD))) return new IListCTD(owner, field, component);
                if (t.Equals(typeof(AApiVersionedFieldsCTD))) return new AApiVersionedFieldsCTD((AApiVersionedFields)owner[field].Value);
                if (t.Equals(typeof(ReaderCTD))) return new ReaderCTD(owner, field, component);
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
    [Category("Fields")]
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

    [TypeConverter(typeof(BoolsetConverter))]
    [Category("Fields")]
    public class BoolsetConverterCTD : ICustomTypeDescriptor
    {
        AApiVersionedFields owner;
        string field;
        object component;
        public BoolsetConverterCTD(AApiVersionedFields owner, string field, object component) { this.owner = owner; this.field = field; this.component = component; }

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

            public override Type PropertyType { get { return typeof(BoolsetConverter); } }

            public override object GetValue(object component) { return new BoolsetConverter(); }

            public override bool IsReadOnly { get { return true; } }

            public override void SetValue(object component, object value) { throw new InvalidOperationException(); }

            public override Type ComponentType { get { return null; } }

            public override bool ShouldSerializeValue(object component) { return true; }
        }

        public class BoolsetConverter : TypeConverter
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
                        ulong asUlong = Convert.ToUInt64(asString, (asString.StartsWith("0x")) ? 16 : 10);
                        Boolset res = new Boolset(asUlong);
                        return (Boolset)((string)res).Substring(64 - ((string)owner[field]).Length);
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
                if (value as BoolsetConverterCTD != null && destinationType.Equals(typeof(string)))
                {
                    try
                    {
                        AApiVersionedFields owner = ((AApiVersionedFieldsCTD)context.Instance).Value;
                        string field = ((BoolsetConverterCTD)value).field;
                        int i = ((string)owner[field]).Length / 4;
                        string format = "0x{0:X" + i + "}";
                        return String.Format(format, (ulong)(Boolset)owner[field].Value);
                    }
                    catch { }
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }

    [Category("Lists")]
    public class IListCTD : ICustomTypeDescriptor, IList<AApiVersionedFieldsCTD>, ICollection, IEnumerable<AApiVersionedFieldsCTD>, IEnumerator<AApiVersionedFieldsCTD>
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

        public AApiVersionedFieldsCTD this[int index]
        {
            get
            {
                System.Reflection.MethodInfo m = TIList.GetMethod("get_Item", new Type[] { typeof(int) });
                object o = m.Invoke(tv.Value, new object[] { index });
                return new AApiVersionedFieldsCTD((AApiVersionedFields)o);
            }
            set
            {
                object o = ((AApiVersionedFieldsCTD)value).Value;
                System.Reflection.MethodInfo m = TIList.GetMethod("set_Item", new Type[] { typeof(int), IListT });
                m.Invoke(tv.Value, new object[] { index, o });
            }
        }
        
        public void Add(AApiVersionedFieldsCTD value)
        {
            if (!IListT.IsAssignableFrom(value.GetType())) throw new InvalidCastException();
            System.Reflection.MethodInfo m = TIList.GetMethod("Add", new Type[] { IListT });
            m.Invoke(tv.Value, new object[] { value });
            //return Count - 1;
        }

        public void Clear()
        {
            System.Reflection.MethodInfo m = TIList.GetMethod("Clear", new Type[] { });
            m.Invoke(tv.Value, new object[] { });
        }

        public bool Contains(AApiVersionedFieldsCTD value)
        {
            System.Reflection.MethodInfo m = TIList.GetMethod("Contains", new Type[] { value.GetType() });
            return (bool)m.Invoke(tv.Value, new object[] { value });
        }

        public int IndexOf(AApiVersionedFieldsCTD value)
        {
            System.Reflection.MethodInfo m = TIList.GetMethod("IndexOf", new Type[] { value.GetType() });
            return (int)m.Invoke(tv.Value, new object[] { value });
        }

        public void Insert(int index, AApiVersionedFieldsCTD value)
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

        public bool Remove(AApiVersionedFieldsCTD value)
        {
            if (!IListT.IsAssignableFrom(value.GetType())) throw new InvalidCastException();
            System.Reflection.MethodInfo m = TIList.GetMethod("Remove", new Type[] { IListT });
            return (bool)m.Invoke(tv.Value, new object[] { value });
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

        public void CopyTo(AApiVersionedFieldsCTD[] array, int index)
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
            //System.Reflection.MethodInfo m = TIList.GetMethod("GetEnumerator", new Type[] { });
            //return (IEnumerator)m.Invoke(tv.Value, new object[] { });
        }

        public IEnumerator<AApiVersionedFieldsCTD> GetEnumerator()
        {
            return this;
            //System.Reflection.MethodInfo m = TIList.GetMethod("GetEnumerator", new Type[] { });
            //return (IEnumerator)m.Invoke(tv.Value, new object[] { });
        }

        #endregion

        #region IEnumerator Members

        int enumeratorCurrent = -1;
        object IEnumerator.Current { get { return this[enumeratorCurrent]; } }
        public AApiVersionedFieldsCTD Current { get { return this[enumeratorCurrent]; } }

        public bool MoveNext() { enumeratorCurrent++; return enumeratorCurrent < Count; }

        public void Reset() { enumeratorCurrent = -1; }

        public void Dispose() { }

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

            public override AttributeCollection Attributes { get { return new AttributeCollection(new TypeConverterAttribute(typeof(AApiVersionedFieldsCTD.AApiVersionedFieldsCTDConverter))); } }

            public override bool CanResetValue(object component) { return false; }

            public override Type ComponentType { get { return typeof(IListCTD); } }

            public override string DisplayName { get { return ""; } }

            public override string Description { get { return ""; } }

            public override object GetValue(object component) { return list[index]; }

            public override bool IsReadOnly { get { return false; } }

            public override string Name { get { return "[" + index + "]"; } }

            public override Type PropertyType { get { return typeof(AApiVersionedFieldsCTD); } }

            public override void ResetValue(object component) { }

            public override bool ShouldSerializeValue(object component) { return true; }

            public override void SetValue(object component, object value) { list[index] = (AApiVersionedFieldsCTD)value; }
        }
    }

    [Editor(typeof(ReaderEditor),typeof(UITypeEditor))]
    [TypeConverter(typeof(ReaderConverter))]
    [Category("Data")]
    public class ReaderCTD : ICustomTypeDescriptor
    {
        protected AApiVersionedFields owner;
        protected string field;
        protected object component;
        public ReaderCTD(AApiVersionedFields owner, string field, object component) { this.owner = owner; this.field = field; this.component = component; }

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
            pc.Add(new PropertyDescriptor());
            return pc;
        }

        public object GetPropertyOwner(System.ComponentModel.PropertyDescriptor pd) { return this; }

        #endregion

        public class PropertyDescriptor : System.ComponentModel.PropertyDescriptor
        {
            ReaderEditor editor;
            public PropertyDescriptor() : base("Export/Import value", null) { }

            public override object GetEditor(Type editorBaseType)
            {
                if (editorBaseType == typeof(System.Drawing.Design.UITypeEditor))
                {
                    if (editor == null) editor = new ReaderEditor();
                    return editor;
                }
                return base.GetEditor(editorBaseType);
            }

            public override bool CanResetValue(object component) { return false; }

            public override void ResetValue(object component) { throw new InvalidOperationException(); }

            public override Type PropertyType { get { return null; } }

            public override object GetValue(object component) { return null; }

            public override bool IsReadOnly { get { return true; } }

            public override void SetValue(object component, object value) { throw new InvalidOperationException(); }

            public override Type ComponentType { get { return null; } }

            public override bool ShouldSerializeValue(object component) { return true; }
        }

        public class ReaderEditor : UITypeEditor
        {
            ReaderEditorPanel ui;
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.DropDown;
            }
            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (ui == null)
                    ui = new ReaderEditorPanel();

                ReaderCTD o = value as ReaderCTD;
                ui.SetField(o.owner, o.field);
                ui.EdSvc = edSvc;
                edSvc.DropDownControl(ui);
                // the ui (a) updates the value and (b) closes the dropdown

                return value;
            }
        }

        public class ReaderConverter : TypeConverter
        {
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (typeof(string).IsAssignableFrom(destinationType)) return true;
                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                if (typeof(string).IsAssignableFrom(destinationType))
                    return "Import/Export...";
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }
}
