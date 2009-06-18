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
                    if (typeof(AApiVersionedFields).IsAssignableFrom(t)) return typeof(AApiVersionedFieldsCTD);
                    if (t.IsPrimitive)
                        try
                        {
                            Convert.ChangeType(owner[field].Value, typeof(ulong));
                            return typeof(AsHexConverterCTD);
                        }
                        catch { }
                    return t;
                }
            }

            public override object GetValue(object component)
            {
                Type t = PropertyType;
                if (t.Equals(typeof(AApiVersionedFieldsCTD))) return new AApiVersionedFieldsCTD((AApiVersionedFields)owner[field].Value);
                if (t.Equals(typeof(AsHexConverterCTD))) return new AsHexConverterCTD(owner, field, component);
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

    [TypeConverter(typeof(TGIBlockCTDConverter))]
    public class TGIBlockCTD : AApiVersionedFieldsCTD
    {
        public TGIBlockCTD(AApiVersionedFieldsCTD t) : base(t) { }

        public class TGIBlockCTDConverter : ExpandableObjectConverter
        {
            AApiVersionedFieldsCTD t;
            TGIBlockCTDConverter(AApiVersionedFieldsCTD t) { this.t = t; }

            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType.Equals(typeof(string))) return true;
                return base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                if (value.GetType().Equals(typeof(string)))
                {
                    try
                    {
                        s3pi.Extensions.TGIN tgin = new s3pi.Extensions.TGIN();
                        tgin = (string)value;
                        t["ResourceType"] = new TypedValue(typeof(uint), (uint)tgin.ResType, "X");
                        t["ResourceGroup"] = new TypedValue(typeof(uint), (uint)tgin.ResGroup, "X");
                        t["Instance"] = new TypedValue(typeof(ulong), (ulong)tgin.ResInstance, "X");
                    }
                    catch { }
                    return t.Value;
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
                if (destinationType.Equals(typeof(string))) return "" + t.Value;
                return base.ConvertTo(context, culture, value, destinationType);
            }

            public override bool GetPropertiesSupported(ITypeDescriptorContext context) { return base.GetPropertiesSupported(context); }
            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) { return base.GetProperties(context, value, attributes); }
            public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) { return base.GetCreateInstanceSupported(context); }
            public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues) { return base.CreateInstance(context, propertyValues); }
            public override bool IsValid(ITypeDescriptorContext context, object value) { return base.IsValid(context, value); }
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
}
