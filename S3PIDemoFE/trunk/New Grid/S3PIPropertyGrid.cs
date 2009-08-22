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
        public S3PIPropertyGrid() : base() { PropertySort = PropertySort.Alphabetical; HelpVisible = false; ToolbarVisible = false; }

        public AApiVersionedFields s3piObject { set { if (value != null) target = new AApiVersionedFieldsCTD(value); SelectedObject = target; } }
    }


    // Need to convert this to a PropertyGrid
    // http://msdn.microsoft.com/en-us/library/aa302334.aspx
    // http://www.codeproject.com/KB/tabs/customizingcollectiondata.aspx?display=Print

    [TypeConverter(typeof(AApiVersionedFieldsCTDConverter))]
    public class AApiVersionedFieldsCTD : ICustomTypeDescriptor
    {
        AApiVersionedFields owner;
        string field;
        object component;
        AApiVersionedFields value;
        public AApiVersionedFieldsCTD(AApiVersionedFields value) { this.value = value; }
        public AApiVersionedFieldsCTD(AApiVersionedFields owner, string field, object component) { this.owner = owner; this.field = field; this.component = component; }

        #region ICustomTypeDescriptor Members

        public AttributeCollection GetAttributes() { return TypeDescriptor.GetAttributes(this, true); }

        public string GetClassName() { return "!" + TypeDescriptor.GetClassName(this, true); }

        public string GetComponentName() { return "\"" + TypeDescriptor.GetComponentName(this, true); }

        public TypeConverter GetConverter() { return TypeDescriptor.GetConverter(this, true); }

        public EventDescriptor GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(this, true); }

        public PropertyDescriptor GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(this, true); }

        public object GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(this, editorBaseType, true); }

        public EventDescriptorCollection GetEvents(Attribute[] attributes) { return TypeDescriptor.GetEvents(this, attributes, true); }

        public EventDescriptorCollection GetEvents() { return TypeDescriptor.GetEvents(this, true); }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            if (value == null) value = (AApiVersionedFields)owner[field].Value;

            List<string> filter = new List<string>(new string[] { "Stream", "AsBytes", "Value", });
            List<string> contentFields = value.ContentFields;
            PropertyDescriptorCollection pdc = new PropertyDescriptorCollection(null);
            if (typeof(IDictionary).IsAssignableFrom(value.GetType())) { pdc.Add(new IDictionaryPropertyDescriptor((IDictionary)value, "(this)", new Attribute[] { new CategoryAttribute("Lists") })); }
            foreach (string f in contentFields)
            {
                if (filter.Contains(f)) continue;
                TypedValuePropertyDescriptor tvpd = new TypedValuePropertyDescriptor(value, f, null);
                pdc.Add(new TypedValuePropertyDescriptor(value, f, new Attribute[] { new CategoryAttribute(tvpdToCategory(tvpd.PropertyType)) }));
            }
            return pdc;
        }
        string tvpdToCategory(Type t)
        {
            if (t.Equals(typeof(EnumChooserCTD))) return "Values";
            if (t.Equals(typeof(EnumFlagsCTD))) return "Fields";
            if (t.Equals(typeof(AsHexCTD))) return "Values";
            if (t.Equals(typeof(ArrayAsHexCTD))) return "Lists";
            if (t.Equals(typeof(AApiVersionedFieldsCTD))) return "Fields";
            if (t.Equals(typeof(ICollectionAApiVersionedFieldsCTD))) return "Lists";
            if (t.Equals(typeof(IDictionaryCTD))) return "Lists";
            if (t.Equals(typeof(ReaderCTD))) return "Readers";
            return "Values";
        }

        public PropertyDescriptorCollection GetProperties() { return GetProperties(new Attribute[] { }); }

        public object GetPropertyOwner(PropertyDescriptor pd) { return this; }

        #endregion

        public class IDictionaryPropertyDescriptor : PropertyDescriptor
        {
            IDictionary value;
            public IDictionaryPropertyDescriptor(IDictionary value, string field, Attribute[] attrs) : base(field, attrs) { this.value = value; }

            public override bool CanResetValue(object component) { return false; }

            public override Type ComponentType { get { throw new NotImplementedException(); } }

            public override object GetValue(object component)
            {
                return new IDictionaryCTD(value);
            }

            public override bool IsReadOnly { get { return false; } }

            public override Type PropertyType { get { return typeof(IDictionaryCTD); } }

            public override void ResetValue(object component) { throw new NotImplementedException(); }

            public override void SetValue(object component, object value) { }

            public override bool ShouldSerializeValue(object component) { return true; }
        }

        public class TypedValuePropertyDescriptor : PropertyDescriptor
        {
            AApiVersionedFields owner;
            Type fieldType;
            public TypedValuePropertyDescriptor(AApiVersionedFields owner, string field, Attribute[] attrs)
                : base(field, attrs)
            {
                this.owner = owner;
                if (typeof(AsHex).Equals(owner.GetType())) fieldType = ((AsHex)owner).ElementType;
                else if (typeof(AsKVP).Equals(owner.GetType())) fieldType = ((AsKVP)owner).GetType(field);
                else fieldType = AApiVersionedFields.GetContentFieldTypes(0, owner.GetType())[Name];
            }

            public override bool CanResetValue(object component) { return false; }

            public override Type ComponentType { get { throw new NotImplementedException(); } }

            public override object GetValue(object component)
            {
                Type t = PropertyType;
                if (t.Equals(typeof(EnumChooserCTD))) return new EnumChooserCTD(owner, Name, component);
                if (t.Equals(typeof(EnumFlagsCTD))) return new EnumFlagsCTD(owner, Name, component);
                if (t.Equals(typeof(AsHexCTD))) return new AsHexCTD(owner, Name, component);
                if (t.Equals(typeof(ArrayAsHexCTD))) return new ArrayAsHexCTD(owner, Name, component);
                if (t.Equals(typeof(AApiVersionedFieldsCTD))) return new AApiVersionedFieldsCTD(owner, Name, component);
                if (t.Equals(typeof(ICollectionAApiVersionedFieldsCTD))) return new ICollectionAApiVersionedFieldsCTD(owner, Name, component);
                if (t.Equals(typeof(IDictionaryCTD))) return new IDictionaryCTD(owner, Name, component);
                if (t.Equals(typeof(ReaderCTD))) return new ReaderCTD(owner, Name, component);
                return owner[Name].Value;
            }

            public override bool IsReadOnly
            {
                get
                {
                    if (owner.GetType().Equals(typeof(AsHex))) return false;
                    if (owner.GetType().Equals(typeof(AsKVP))) return false;
                    return !owner.GetType().GetProperty(Name).CanWrite; 
                }
            }


            bool isCollection(Type fieldType)
            {
                if (!typeof(ICollection).IsAssignableFrom(fieldType)) return false;
                Type baseType;
                if (fieldType.GetGenericArguments().Length == 1) baseType = fieldType.GetGenericArguments()[0];
                else if (fieldType.BaseType.GetGenericArguments().Length == 1) baseType = fieldType.BaseType.GetGenericArguments()[0];
                else return false;
                return typeof(AApiVersionedFields).IsAssignableFrom(baseType);
            }
            /*
             *  * Boolset (as hex)
             *  * value types naturally convertible to ulong (as hex)
             *  * AApiVersionedFields (recurse)
             *  * IEnumerable (including arrays but not strings or Boolsets)
             *  * Text/BinaryReader (special editor)
             */
            public override Type PropertyType
            {
                get
                {
                    // Must test these before IConvertible
                    List<Type> simpleTypes = new List<Type>(new Type[] { typeof(bool), typeof(DateTime), typeof(decimal), typeof(double), typeof(float), typeof(string), });
                    if (simpleTypes.Contains(fieldType)) return fieldType;

                    // Must test enum before IConvertible
                    if (typeof(Enum).IsAssignableFrom(fieldType) && fieldType.GetCustomAttributes(typeof(FlagsAttribute), true).Length == 0) return typeof(EnumChooserCTD);
                    if (typeof(Enum).IsAssignableFrom(fieldType) && fieldType.GetCustomAttributes(typeof(FlagsAttribute), true).Length == 1) return typeof(EnumFlagsCTD);

                    if (typeof(IConvertible).IsAssignableFrom(fieldType) || typeof(Boolset).Equals(fieldType)) return typeof(AsHexCTD);

                    if (typeof(AApiVersionedFields).IsAssignableFrom(fieldType)) return typeof(AApiVersionedFieldsCTD);

                    
                    // More complex stuff

                    // Arrays
                    if (fieldType.HasElementType
                        && (typeof(IConvertible).IsAssignableFrom(fieldType.GetElementType()) || typeof(Boolset).Equals(fieldType.GetElementType())))
                        return typeof(ArrayAsHexCTD);

                    // Collections of AApiVersionedFields (AResource.DependentList<T> where T is AHandlerElement)
                    if (isCollection(fieldType))
                        return typeof(ICollectionAApiVersionedFieldsCTD);

                    if (typeof(IDictionary).IsAssignableFrom(fieldType)) return typeof(IDictionaryCTD);

                    if (typeof(BinaryReader).IsAssignableFrom(fieldType) || typeof(TextReader).IsAssignableFrom(fieldType))
                        return typeof(ReaderCTD);

                    return fieldType;
                }
            }

            public override void ResetValue(object component) { throw new NotImplementedException(); }

            public override void SetValue(object component, object value) { owner[Name] = new TypedValue(value.GetType(), value); }

            public override bool ShouldSerializeValue(object component) { return true; }

            public Type FieldType { get { return fieldType; } }
        }

        public class AApiVersionedFieldsCTDConverter : ExpandableObjectConverter
        {
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (typeof(string).Equals(destinationType)) return true;
                return base.CanConvertTo(context, destinationType);
            }
            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                if (typeof(string).Equals(destinationType)) return "";
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }

    [TypeConverter(typeof(AsHexConverter))]
    public class AsHexCTD : ICustomTypeDescriptor
    {
        protected AApiVersionedFields owner;
        protected string field;
        protected object component;
        public AsHexCTD(AApiVersionedFields owner, string field, object component) { this.owner = owner; this.field = field; this.component = component; }

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

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) { return new PropertyDescriptorCollection(new PropertyDescriptor[] { new AsHexPropertyDescriptor(owner, field, component, null), }); }

        public PropertyDescriptorCollection GetProperties() { return GetProperties(null); }

        public object GetPropertyOwner(System.ComponentModel.PropertyDescriptor pd) { return this; }

        #endregion

        public class AsHexPropertyDescriptor : System.ComponentModel.PropertyDescriptor
        {
            AApiVersionedFields owner;
            string field;
            object component;
            public AsHexPropertyDescriptor(AApiVersionedFields owner, string field, object component, Attribute[] attr)
                : base(field, attr) { this.owner = owner; this.field = field; this.component = component; }

            public override string Name { get { return field; } }

            public override bool CanResetValue(object component) { throw new InvalidOperationException(); }

            public override void ResetValue(object component) { throw new InvalidOperationException(); }

            public override Type PropertyType { get { throw new InvalidOperationException(); } }

            public override object GetValue(object component) { throw new InvalidOperationException(); }

            public override bool IsReadOnly { get { throw new InvalidOperationException(); } }

            public override void SetValue(object component, object value) { throw new InvalidOperationException(); }

            public override Type ComponentType { get { throw new InvalidOperationException(); } }

            public override bool ShouldSerializeValue(object component) { throw new InvalidOperationException(); }
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
                    string str = (string)value;
                    try
                    {
                        AApiVersionedFieldsCTD.TypedValuePropertyDescriptor pd = (AApiVersionedFieldsCTD.TypedValuePropertyDescriptor)context.PropertyDescriptor;
                        if (typeof(Boolset).Equals(pd.FieldType))
                            return new Boolset(str);
                        ulong num = Convert.ToUInt64(str, str.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase) ? 16 : 10);
                        return Convert.ChangeType(num, pd.FieldType);
                    }
                    catch (Exception ex) { throw new NotSupportedException("Invalid data: " + str, ex); }
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
                if (value as AsHexCTD != null && destinationType.Equals(typeof(string)))
                {
                    try
                    {
                        AsHexCTD ctd = (AsHexCTD)value;
                        return "" + ctd.owner[ctd.field];
                    }
                    catch { }
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }

    [Editor(typeof(EnumChooserEditor), typeof(UITypeEditor))]
    [TypeConverter(typeof(EnumChooserConverter))]
    public class EnumChooserCTD : ICustomTypeDescriptor
    {
        protected AApiVersionedFields owner;
        protected string field;
        protected object component;
        public EnumChooserCTD(AApiVersionedFields owner, string field, object component) { this.owner = owner; this.field = field; this.component = component; }

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

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) { return new PropertyDescriptorCollection(new PropertyDescriptor[] { new EnumChooserPropertyDescriptor(owner, field, component, null), }); }

        public PropertyDescriptorCollection GetProperties() { return GetProperties(null); }

        public object GetPropertyOwner(System.ComponentModel.PropertyDescriptor pd) { return this; }

        #endregion

        public class EnumChooserPropertyDescriptor : System.ComponentModel.PropertyDescriptor
        {
            AApiVersionedFields owner;
            string field;
            object component;
            public EnumChooserPropertyDescriptor(AApiVersionedFields owner, string field, object component, Attribute[] attr)
                : base(field, attr) { this.owner = owner; this.field = field; this.component = component; }

            public override string Name { get { return field; } }

            public override bool CanResetValue(object component) { throw new InvalidOperationException(); }

            public override void ResetValue(object component) { throw new InvalidOperationException(); }

            public override Type PropertyType { get { throw new InvalidOperationException(); } }

            public override object GetValue(object component) { throw new InvalidOperationException(); }

            public override bool IsReadOnly { get { throw new InvalidOperationException(); } }

            public override void SetValue(object component, object value) { throw new InvalidOperationException(); }

            public override Type ComponentType { get { throw new InvalidOperationException(); } }

            public override bool ShouldSerializeValue(object component) { throw new InvalidOperationException(); }
        }

        public class EnumChooserConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType.Equals(typeof(string))) return true;
                return base.CanConvertFrom(context, sourceType);
            }
            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                if (value.GetType().Equals(typeof(string)))
                {
                    string[] content = ((string)value).Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    string str = content[0];
                    try
                    {
                        AApiVersionedFieldsCTD.TypedValuePropertyDescriptor pd = (AApiVersionedFieldsCTD.TypedValuePropertyDescriptor)context.PropertyDescriptor;
                        ulong num = Convert.ToUInt64(str, str.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase) ? 16 : 10);
                        return Enum.ToObject(pd.FieldType, num);
                    }
                    catch (Exception ex) { throw new NotSupportedException("Invalid data: " + str, ex); }
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
                if (value as EnumChooserCTD != null && destinationType.Equals(typeof(string)))
                {
                    try
                    {
                        EnumChooserCTD ctd = (EnumChooserCTD)value;
                        return "" + ctd.owner[ctd.field];
                    }
                    catch { }
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        public class EnumChooserEditor : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) { return UITypeEditorEditStyle.DropDown; }

            public override bool IsDropDownResizable { get { return true; } }

            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                EnumChooserCTD ctd = (EnumChooserCTD)value;
                TypedValue tv = ctd.owner[ctd.field];

                List<string> enumValues = new List<string>();
                int index = -1;
                int i = 0;
                foreach (Enum e in Enum.GetValues(tv.Type))
                {
                    if (e.Equals((Enum)tv.Value)) index = i;
                    enumValues.Add(new TypedValue(e.GetType(), e, "X"));
                    i++;
                }

                ListBox lb = new ListBox();
                lb.Items.AddRange(enumValues.ToArray());
                if (index >= 0) { lb.SelectedIndices.Add(index); }
                lb.SelectedIndexChanged += new EventHandler(lb_SelectedIndexChanged);
                lb.Tag = edSvc;
                lb.Height = lb.Items.Count * lb.ItemHeight;
                lb.IntegralHeight = false;
                edSvc.DropDownControl(lb);

                ctd.owner[ctd.field] = new TypedValue(tv.Type,
                    (Enum)new EnumChooserConverter().ConvertFrom(context, System.Globalization.CultureInfo.CurrentCulture, lb.SelectedItem));

                return value;
            }

            void lb_SelectedIndexChanged(object sender, EventArgs e) { ((sender as ListBox).Tag as IWindowsFormsEditorService).CloseDropDown(); }
        }
    }

    [TypeConverter(typeof(EnumFlagsConverter))]
    public class EnumFlagsCTD : ICustomTypeDescriptor
    {
        protected AApiVersionedFields owner;
        protected string field;
        protected object component;
        public EnumFlagsCTD(AApiVersionedFields owner, string field, object component) { this.owner = owner; this.field = field; this.component = component; }

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

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) { return new PropertyDescriptorCollection(new PropertyDescriptor[] { new EnumFlagsPropertyDescriptor(owner, field, component, null), }); }

        public PropertyDescriptorCollection GetProperties() { return GetProperties(null); }

        public object GetPropertyOwner(System.ComponentModel.PropertyDescriptor pd) { return this; }

        #endregion

        public class EnumFlagsPropertyDescriptor : PropertyDescriptor
        {
            AApiVersionedFields owner;
            string field;
            object component;
            public EnumFlagsPropertyDescriptor(AApiVersionedFields owner, string field, object component, Attribute[] attr)
                : base(field, attr) { this.owner = owner; this.field = field; this.component = component; }

            public override string Name { get { return field; } }

            public override bool CanResetValue(object component) { throw new InvalidOperationException(); }

            public override void ResetValue(object component) { throw new InvalidOperationException(); }

            public override Type PropertyType { get { throw new InvalidOperationException(); } }

            public override object GetValue(object component) { throw new InvalidOperationException(); }

            public override bool IsReadOnly { get { throw new InvalidOperationException(); } }

            public override void SetValue(object component, object value) { throw new InvalidOperationException(); }

            public override Type ComponentType { get { throw new InvalidOperationException(); } }

            public override bool ShouldSerializeValue(object component) { throw new InvalidOperationException(); }
        }

        public class EnumFlagsConverter : ExpandableObjectConverter
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
                    string[] content = ((string)value).Split(new char[] {' '}, 2, StringSplitOptions.RemoveEmptyEntries);
                    string str = content[0];
                    try
                    {
                        AApiVersionedFieldsCTD.TypedValuePropertyDescriptor pd = (AApiVersionedFieldsCTD.TypedValuePropertyDescriptor)context.PropertyDescriptor;
                        ulong num = Convert.ToUInt64(str, str.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase) ? 16 : 10);
                        return Enum.ToObject(pd.FieldType, num);
                    }
                    catch (Exception ex) { throw new NotSupportedException("Invalid data: " + str, ex); }
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
                if (value as EnumFlagsCTD != null && destinationType.Equals(typeof(string)))
                {
                    try
                    {
                        EnumFlagsCTD ctd = (EnumFlagsCTD)value;
                        TypedValue tv = ctd.owner[ctd.field];
                        return "0x" + Enum.Format(tv.Type, tv.Value, "X");
                    }
                    catch { }
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }

            static int underlyingTypeToBits(Type t)
            {
                if (t.Equals(typeof(byte))) return 8;
                if (t.Equals(typeof(ushort))) return 16;
                if (t.Equals(typeof(uint))) return 32;
                if (t.Equals(typeof(ulong))) return 64;
                return -1;
            }
            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
            {
                EnumFlagsCTD ctd = (EnumFlagsCTD)value;
                Type enumType = AApiVersionedFields.GetContentFieldTypes(0, ctd.owner.GetType())[ctd.field];
                int bits = underlyingTypeToBits(Enum.GetUnderlyingType(enumType));
                EnumFlagPropertyDescriptor[] enumFlags = new EnumFlagPropertyDescriptor[bits];
                string fmt = "[{0:X" + bits.ToString("X").Length + "}] ";
                for (int i = 0; i < bits; i++)
                {
                    ulong u = (ulong)1 << i;
                    string s = (Enum)Enum.ToObject(enumType, u) + "";
                    if (s == u.ToString()) s = "-";
                    s = String.Format(fmt, i) + s;
                    enumFlags[i] = new EnumFlagPropertyDescriptor(ctd.owner, ctd.field, u, s);
                }
                return new PropertyDescriptorCollection(enumFlags);
            }

            public class EnumFlagPropertyDescriptor : PropertyDescriptor
            {
                AApiVersionedFields owner;
                string field;
                ulong mask;
                public EnumFlagPropertyDescriptor(AApiVersionedFields owner, string field, ulong mask, string value) : base(value, null) { this.owner = owner; this.field = field; this.mask = mask; }
                public override bool CanResetValue(object component) { return true; }

                public override Type ComponentType { get { throw new NotImplementedException(); } }

                ulong getFlags(AApiVersionedFields owner, string field)
                {
                    TypedValue tv = owner[field];
                    object o = Convert.ChangeType(tv.Value, Enum.GetUnderlyingType(tv.Type));
                    if (o.GetType().Equals(typeof(byte))) return (byte)o;
                    if (o.GetType().Equals(typeof(ushort))) return (ushort)o;
                    if (o.GetType().Equals(typeof(uint))) return (uint)o;
                    return (ulong)o;
                }
                public override object GetValue(object component)
                {
                    ulong old = getFlags(owner, field);
                    return (old & mask) != 0;
                }

                public override bool IsReadOnly { get { return !owner.GetType().GetProperty(field).CanWrite; } }

                public override Type PropertyType { get { return typeof(Boolean); } }

                public override void ResetValue(object component) { SetValue(component, false); }

                void setFlags(AApiVersionedFields owner, string field, ulong mask, bool value)
                {
                    ulong old = getFlags(owner, field);
                    ulong res = old & ~mask;
                    if (value) res |= mask;
                    Type t = AApiVersionedFields.GetContentFieldTypes(0, owner.GetType())[field];
                    TypedValue tv = new TypedValue(t, Enum.ToObject(t, res));
                    owner[field] = tv;
                }
                public override void SetValue(object component, object value)
                {
                    setFlags(owner, field, mask, (bool)value);
                }

                public override bool ShouldSerializeValue(object component) { return false; }
            }
        }
    }

    [Editor(typeof(ICollectionAApiVersionedFieldsEditor), typeof(UITypeEditor))]
    [TypeConverter(typeof(ICollectionAApiVersionedConverter))]
    public class ICollectionAApiVersionedFieldsCTD : ICustomTypeDescriptor
    {
        protected AApiVersionedFields owner;
        protected string field;
        protected object component;
        public ICollectionAApiVersionedFieldsCTD(AApiVersionedFields owner, string field, object component) { this.owner = owner; this.field = field; this.component = component; }

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

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) { return new PropertyDescriptorCollection(new PropertyDescriptor[] { new ICollectionAApiVersionedFieldsPropertyDescriptor(owner, field, component, null), }); }

        public PropertyDescriptorCollection GetProperties() { return GetProperties(null); }

        public object GetPropertyOwner(System.ComponentModel.PropertyDescriptor pd) { return this; }

        #endregion

        public class ICollectionAApiVersionedFieldsPropertyDescriptor : PropertyDescriptor
        {
            AApiVersionedFields owner;
            string field;
            object component;
            public ICollectionAApiVersionedFieldsPropertyDescriptor(AApiVersionedFields owner, string field, object component, Attribute[] attr)
                : base(field, attr) { this.owner = owner; this.field = field; this.component = component; }

            public override bool CanResetValue(object component) { throw new InvalidOperationException(); }

            public override void ResetValue(object component) { throw new InvalidOperationException(); }

            public override Type PropertyType { get { throw new InvalidOperationException(); } }

            public override object GetValue(object component) { throw new InvalidOperationException(); }

            public override bool IsReadOnly { get { throw new InvalidOperationException(); } }

            public override void SetValue(object component, object value) { throw new InvalidOperationException(); }

            public override Type ComponentType { get { return component.GetType(); } }

            public override bool ShouldSerializeValue(object component) { return true; }
        }

        public class ICollectionAApiVersionedFieldsEditor : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) { return UITypeEditorEditStyle.Modal; }
            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                ICollectionAApiVersionedFieldsCTD field = (ICollectionAApiVersionedFieldsCTD)value;
                ICollection coll = (ICollection)field.owner[field.field].Value;
                List<AApiVersionedFields> list = new List<AApiVersionedFields>();
                foreach (AApiVersionedFields f in coll) list.Add(f);

                NewGridForm ui = new NewGridForm(list);
                edSvc.ShowDialog(ui);

                return value;
            }
        }

        public class ICollectionAApiVersionedConverter : TypeConverter
        {
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (typeof(string).Equals(destinationType)) return true;
                return base.CanConvertTo(context, destinationType);
            }
            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                ICollectionAApiVersionedFieldsCTD ctd = value as ICollectionAApiVersionedFieldsCTD;
                ICollection ic = (ICollection)ctd.owner[ctd.field].Value;
                if (ic == null) return "(null)";
                if (typeof(string).Equals(destinationType)) return "(Collection: " + ic.Count + ")";
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }

    [TypeConverter(typeof(ArrayAsHexConverter))]
    public class ArrayAsHexCTD : ICustomTypeDescriptor
    {
        protected AApiVersionedFields owner;
        protected string field;
        protected object component;
        public ArrayAsHexCTD(AApiVersionedFields owner, string field, object component) { this.owner = owner; this.field = field; this.component = component; }

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

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return new PropertyDescriptorCollection(new PropertyDescriptor[] { new IListAsHexPropertyDescriptor(owner, field, component, null), });
        }

        public PropertyDescriptorCollection GetProperties() { return GetProperties(null); }

        public object GetPropertyOwner(System.ComponentModel.PropertyDescriptor pd) { return this; }

        #endregion

        public class IListAsHexPropertyDescriptor : PropertyDescriptor
        {
            AApiVersionedFields owner;
            string field;
            object component;
            public IListAsHexPropertyDescriptor(AApiVersionedFields owner, string field, object component, Attribute[] attr)
                : base(field, attr) { this.owner = owner; this.field = field; this.component = component; }

            public override string Name { get { return field; } }

            public override bool CanResetValue(object component) { throw new InvalidOperationException(); }

            public override void ResetValue(object component) { throw new InvalidOperationException(); }

            public override Type PropertyType { get { throw new InvalidOperationException(); } }

            public override object GetValue(object component) { throw new InvalidOperationException(); }

            public override bool IsReadOnly { get { throw new InvalidOperationException(); } }

            public override void SetValue(object component, object value) { throw new InvalidOperationException(); }

            public override Type ComponentType { get { throw new InvalidOperationException(); } }

            public override bool ShouldSerializeValue(object component) { throw new InvalidOperationException(); }
        }

        public class IListAsHexEditor : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) { return UITypeEditorEditStyle.Modal; }
            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                ArrayAsHexCTD field = (ArrayAsHexCTD)value;
                NewGridForm ui = new NewGridForm(new AsHex(field.owner, field.field));
                edSvc.ShowDialog(ui);

                return value;
            }
        }

        public class ArrayAsHexConverter : ExpandableObjectConverter
        {
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (typeof(string).Equals(destinationType)) return true;
                return base.CanConvertTo(context, destinationType);
            }
            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                ArrayAsHexCTD ctd = value as ArrayAsHexCTD;
                Array ary = (Array)ctd.owner[ctd.field].Value;
                if (ary == null) return "(null)";
                if (typeof(string).Equals(destinationType)) return "(Array: " + ary.Length + ")";
                return base.ConvertTo(context, culture, value, destinationType);
            }

            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
            {
                ArrayAsHexCTD ctd = value as ArrayAsHexCTD;
                AsHex ah = new AsHex(ctd.owner, ctd.field);
                Array ary = ctd.owner[ctd.field].Value as Array;
                AApiVersionedFieldsCTD.TypedValuePropertyDescriptor[] pds = new AApiVersionedFieldsCTD.TypedValuePropertyDescriptor[ary.Length];
                string fmt = "[{0:X" + ary.Length.ToString("X").Length + "}] " + ary.GetType().GetElementType().Name;
                for (int i = 0; i < ary.Length; i++)
                    pds[i] = new AApiVersionedFieldsCTD.TypedValuePropertyDescriptor(ah, String.Format(fmt, i), new Attribute[] { });
                return new PropertyDescriptorCollection(pds);
            }
        }
    }

    public class AsHex : AApiVersionedFields
    {
        AApiVersionedFields owner;
        string field;
        TypedValue tv;
        IList list;
        public AsHex(AApiVersionedFields owner, string field) { this.owner = owner; this.field = field; tv = owner[field]; list = (IList)tv.Value; }

        public Type ElementType { get { return tv.Type.HasElementType ? tv.Type.GetElementType() : list.Count > 0 ? list[0].GetType() : typeof(object); } }

        static System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"^\[([\dA-F]+)\].*$");
        public override TypedValue this[string index]
        {
            get
            {
                if (!regex.IsMatch(index))
                    throw new ArgumentOutOfRangeException();
                int i = Convert.ToInt32("0x" + regex.Match(index).Groups[1].Value, 16);

                return new TypedValue(ElementType, ((IList)tv.Value)[i], "X");
            }
            set
            {
                if (!regex.IsMatch(index))
                    throw new ArgumentOutOfRangeException();
                int i = Convert.ToInt32("0x" + regex.Match(index).Groups[1].Value, 16);

                //list[i] = value.Value; <-- BYPASSES "new" set method in AHandlerList<T>
                //Lists
                PropertyInfo p = list.GetType().GetProperty("Item");
                if (p != null)
                    p.SetValue(list, value.Value, new object[] { i, });
                else
                {
                    //Arrays
                    MethodInfo m = list.GetType().GetMethod("Set");
                    if (m != null)
                        m.Invoke(list, new object[] { i, value.Value });
                    owner[field] = new TypedValue(tv.Type, list);
                }
            }
        }

        public override List<string> ContentFields
        {
            get
            {
                List<string> res = new List<string>();
                string fmt = "[{0:X" + list.Count.ToString("X").Length + "}] {1}";
                for (int i = 0; i < list.Count; i++)
                    res.Add(String.Format(fmt, i, field));
                return res;
            }
        }

        public override int RecommendedApiVersion { get { return 0; } }
    }

    [Editor(typeof(IDictionaryEditor), typeof(UITypeEditor))]
    [TypeConverter(typeof(IDictionaryConverter))]
    public class IDictionaryCTD : ICustomTypeDescriptor
    {
        protected AApiVersionedFields owner;
        protected string field;
        protected object component;
        IDictionary value;
        public IDictionaryCTD(AApiVersionedFields owner, string field, object component) { this.owner = owner; this.field = field; this.component = component; }
        public IDictionaryCTD(IDictionary value) { this.value = value; }

        public IDictionary Value { get { if (value == null) value = (IDictionary)owner[field].Value; return value; } }

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

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return new PropertyDescriptorCollection(new PropertyDescriptor[] { new IDictionaryPropertyDescriptor(owner, field, component, null), });
        }

        public PropertyDescriptorCollection GetProperties() { return GetProperties(null); }

        public object GetPropertyOwner(System.ComponentModel.PropertyDescriptor pd) { return this; }

        #endregion

        public class IDictionaryPropertyDescriptor : PropertyDescriptor
        {
            AApiVersionedFields owner;
            string field;
            object component;
            public IDictionaryPropertyDescriptor(AApiVersionedFields owner, string field, object component, Attribute[] attr)
                : base(field, attr) { this.owner = owner; this.field = field; this.component = component; }

            public override string Name { get { return field; } }

            public override bool CanResetValue(object component) { throw new InvalidOperationException(); }

            public override void ResetValue(object component) { throw new InvalidOperationException(); }

            public override Type PropertyType { get { throw new InvalidOperationException(); } }

            public override object GetValue(object component) { throw new InvalidOperationException(); }

            public override bool IsReadOnly { get { throw new InvalidOperationException(); } }

            public override void SetValue(object component, object value) { throw new InvalidOperationException(); }

            public override Type ComponentType { get { throw new InvalidOperationException(); } }

            public override bool ShouldSerializeValue(object component) { throw new InvalidOperationException(); }
        }

        public class IDictionaryEditor : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) { return UITypeEditorEditStyle.Modal; }
            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                IDictionaryCTD field = (IDictionaryCTD)value;
                if (field.Value == null) return value;

                List<AApiVersionedFields> list = new List<AApiVersionedFields>();
                List<object> oldKeys = new List<object>();
                foreach (var k in field.Value.Keys) { list.Add(new AsKVP(new DictionaryEntry(k, field.Value[k]))); oldKeys.Add(k); }

                NewGridForm ui = new NewGridForm(list);
                edSvc.ShowDialog(ui);

                List<object> newKeys = new List<object>();
                foreach (AsKVP kvp in list)
                    newKeys.Add(kvp["Key"].Value);

                List<object> delete = new List<object>();
                foreach (var k in field.Value.Keys) if (!newKeys.Contains(k)) delete.Add(k);
                foreach (object k in delete) field.Value.Remove(k);

                foreach (AsKVP kvp in list)
                    if (oldKeys.Contains(kvp["Key"].Value)) field.Value[kvp["Key"].Value] = kvp["Val"].Value;
                    else field.Value.Add(kvp["Key"].Value, kvp["Val"].Value);

                return value;
            }
        }

        public class IDictionaryConverter : TypeConverter
        {
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (typeof(string).Equals(destinationType)) return true;
                return base.CanConvertTo(context, destinationType);
            }
            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                IDictionaryCTD ctd = value as IDictionaryCTD;
                IDictionary id = (IDictionary)ctd.Value;
                if (id == null) return "(null)";
                if (typeof(string).Equals(destinationType)) return "(Dictionary: " + id.Count + ")";
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }

    public class AsKVP : AApiVersionedFields
    {
        DictionaryEntry entry;
        List<string> contentFields;
        public AsKVP(DictionaryEntry entry) { this.entry = entry; contentFields = new List<string>(new string[] { "Key", "Val", }); }

        public override List<string> ContentFields { get { return contentFields; } }
        public override int RecommendedApiVersion { get { return 0; } }
        
        public override TypedValue this[string index]
        {
            get
            {
                switch (contentFields.IndexOf(index))
                {
                    case 0: return new TypedValue(entry.Key.GetType(), entry.Key, "X");
                    case 1: return new TypedValue(entry.Value.GetType(), entry.Value, "X");
                    default: throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (contentFields.IndexOf(index))
                {
                    case 0: entry.Key = value.Value; break;
                    case 1: entry.Value = value.Value; break;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        public Type GetType(string index)
        {
            switch (contentFields.IndexOf(index))
            {
                case 0: return entry.Key.GetType();
                case 1: return entry.Value.GetType();
                default: throw new IndexOutOfRangeException();
            }
        }
    }

    // Fake up an "AsTGI" AApiVersionFields that formats the Type with the Tag, like the new ResourceInfo dialog

    [Editor(typeof(ReaderEditor), typeof(UITypeEditor))]
    [TypeConverter(typeof(ReaderConverter))]
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

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return new PropertyDescriptorCollection(new PropertyDescriptor[] { new ReaderPropertyDescriptor() });
        }

        public PropertyDescriptorCollection GetProperties() { return GetProperties(null); }

        public object GetPropertyOwner(System.ComponentModel.PropertyDescriptor pd) { return this; }

        #endregion

        public class ReaderPropertyDescriptor : PropertyDescriptor
        {
            ReaderEditor editor;
            public ReaderPropertyDescriptor() : base("Export/Import value", null) { }

            public override object GetEditor(Type editorBaseType)
            {
                if (editorBaseType == typeof(System.Drawing.Design.UITypeEditor))
                {
                    if (editor == null) editor = new ReaderEditor();
                    return editor;
                }
                return base.GetEditor(editorBaseType);
            }

            public override bool CanResetValue(object component) { throw new InvalidOperationException(); }

            public override void ResetValue(object component) { throw new InvalidOperationException(); }

            public override Type PropertyType { get { throw new InvalidOperationException(); } }

            public override object GetValue(object component) { throw new InvalidOperationException(); }

            public override bool IsReadOnly { get { throw new InvalidOperationException(); } }

            public override void SetValue(object component, object value) { throw new InvalidOperationException(); }

            public override Type ComponentType { get { throw new InvalidOperationException(); } }

            public override bool ShouldSerializeValue(object component) { throw new InvalidOperationException(); }
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