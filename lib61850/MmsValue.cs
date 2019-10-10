using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace lib61850
{
    public class MmsValue : IEnumerable
    {
        const string dllPath = Connection.dllPath;
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr MmsValue_toString(IntPtr self);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern double MmsValue_toDouble(IntPtr self);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        static extern bool MmsValue_getBoolean(IntPtr self);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern int MmsValue_getBitStringSize(IntPtr self);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        static extern bool MmsValue_getBitStringBit(IntPtr self, int bitPos);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern Int64 MmsValue_toInt64(IntPtr self);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern UInt32 MmsValue_toUint32(IntPtr self);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern int MmsValue_getType(IntPtr self);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern void MmsValue_delete(IntPtr self);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr MmsValue_getElement(IntPtr complexValue, int index);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern int MmsValue_getArraySize(IntPtr self);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern ulong MmsValue_getUtcTimeInMs(IntPtr self);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr MmsValue_newBoolean(bool value);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr MmsValue_newFloat(float value);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr MmsValue_newDouble(double value);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr MmsValue_newIntegerFromInt32(Int32 integer);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr MmsValue_newUnsignedFromUint32(UInt32 integer);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr MmsValue_newIntegerFromInt64(Int64 integer);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr MmsValue_newVisibleString(string value);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern void MmsValue_setOctetString(IntPtr self, [Out] byte[] buf, int size);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern UInt16 MmsValue_getOctetStringSize(IntPtr self);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern UInt16 MmsValue_getOctetStringMaxSize(IntPtr self);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr MmsValue_getOctetStringBuffer(IntPtr self);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern ulong MmsValue_getBinaryTimeAsUtcMs(IntPtr self);
        internal IntPtr valueReference; /* reference to native MmsValue instance */

        private bool responsableForDeletion; /* if .NET wrapper is responsable for the deletion of the native MmsValue instance */

        // TODO make internal
        public MmsValue(IntPtr value)
        {
            valueReference = value;
            this.responsableForDeletion = false;
        }
        public string Reference = "";

        internal MmsValue(IntPtr value, bool responsableForDeletion)
        {
            valueReference = value;
            this.responsableForDeletion = responsableForDeletion;
        }

        public MmsValue(bool value)
        {
            valueReference = MmsValue_newBoolean(value);
        }

        public MmsValue(float value)
        {
            valueReference = MmsValue_newFloat(value);
        }

        public MmsValue(double value)
        {
            valueReference = MmsValue_newDouble(value);
        }

        public MmsValue(int value)
        {
            valueReference = MmsValue_newIntegerFromInt32(value);
        }

        public MmsValue(UInt32 value)
        {
            valueReference = MmsValue_newUnsignedFromUint32(value);
        }

        public MmsValue(long value)
        {
            valueReference = MmsValue_newIntegerFromInt64(value);
        }

        public MmsValue(string value)
        {
            valueReference = MmsValue_newVisibleString(value);
        }

        ~MmsValue()
        {
            if (responsableForDeletion)
                MmsValue_delete(valueReference);
        }

        public ulong GetBinaryTimeAsUtcMs()
        {
            if (GetType() == MmsType.MMS_BINARY_TIME)
            {
                return MmsValue_getBinaryTimeAsUtcMs(valueReference);
            }
            else
                throw new MmsValueException("Value is not a time type");
        }
        public new MmsType GetType()
        {
            return (MmsType)MmsValue_getType(valueReference);
        }
        public int Size()
        {
            if ((GetType() == MmsType.MMS_ARRAY) || (GetType() == MmsType.MMS_STRUCTURE))
            {
                return MmsValue_getArraySize(valueReference);
            }
            else if (GetType() == MmsType.MMS_BIT_STRING)
            {
                return MmsValue_getBitStringSize(valueReference);
            }
            else if (GetType() == MmsType.MMS_OCTET_STRING)
            {
                return MmsValue_getOctetStringSize(valueReference);
            }
            else
                throw new MmsValueException("Operation not supported for this type");
        }
        public int MaxSize()
        {
            if (GetType() == MmsType.MMS_OCTET_STRING)
            {
                return MmsValue_getOctetStringMaxSize(valueReference);
            }
            else
                throw new MmsValueException("Operation not supported for this type");
        }
        public byte[] getOctetString()
        {
            if (GetType() == MmsType.MMS_OCTET_STRING)
            {
                IntPtr buffer = MmsValue_getOctetStringBuffer(valueReference);
                int bufferSize = this.Size();

                byte[] octetString = new byte[bufferSize];

                Marshal.Copy(buffer, octetString, 0, bufferSize);

                return octetString;
            }
            else
                throw new MmsValueException("Operation not supported for this type");
        }
        public void setOctetString(byte[] octetString)
        {
            if (GetType() == MmsType.MMS_OCTET_STRING)
            {

                if (this.MaxSize() < octetString.Length)
                    throw new MmsValueException("octet string is to large");

                MmsValue_setOctetString(valueReference, octetString, octetString.Length);
            }
            else
                throw new MmsValueException("Operation not supported for this type");
        }
        public MmsValue GetElement(int index)
        {
            MmsType type = GetType();

            if ((type == MmsType.MMS_ARRAY) || (type == MmsType.MMS_STRUCTURE))
            {
                if ((index >= 0) && (index < Size()))
                {
                    IntPtr value = MmsValue_getElement(valueReference, index);
                    var dsa = Marshal.PtrToStringAnsi(valueReference);
                    if (value == IntPtr.Zero)
                        return null;
                    else
                        return new MmsValue(value);
                }
                else
                    throw new MmsValueException("Index out of bounds");
            }
            else
                throw new MmsValueException("Value is of wrong type");
        }
        public ulong GetUtcTimeInMs()
        {
            if (GetType() == MmsType.MMS_UTC_TIME)
                return MmsValue_getUtcTimeInMs(valueReference);
            else
                throw new MmsValueException("Value is not a time type");
        }
        public static DateTimeOffset MsTimeToDateTimeOffset(UInt64 msTime)
        {
            DateTimeOffset retVal = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
            return retVal.AddMilliseconds((double)msTime);
        }
        public DateTimeOffset GetUtcTimeAsDateTimeOffset()
        {
            if (GetType() == MmsType.MMS_UTC_TIME)
                return MsTimeToDateTimeOffset(GetUtcTimeInMs());
            else
                throw new MmsValueException("Value is not a time type");
        }
        public Int64 ToInt64()
        {
            if (GetType() != MmsType.MMS_INTEGER)
                throw new MmsValueException("Value type is not integer");

            return MmsValue_toInt64(valueReference);
        }
        public UInt32 ToUint32()
        {
            if (GetType() != MmsType.MMS_UNSIGNED)
                throw new MmsValueException("Value type is not unsigned");

            return MmsValue_toUint32(valueReference);
        }
        private string GetBitStringAsString()
        {
            if (GetType() != MmsType.MMS_BIT_STRING)
                throw new MmsValueException("Value type is not bit string");

            int size = Size();

            StringBuilder builder = new StringBuilder(size);

            for (int i = 0; i < size; i++)
            {
                if (MmsValue_getBitStringBit(valueReference, i))
                    builder.Append('1');
                else
                    builder.Append('0');
            }

            return builder.ToString();
        }
        public bool GetBoolean()
        {
            if (GetType() == MmsType.MMS_BOOLEAN)
                return MmsValue_getBoolean(valueReference);
            else
                throw new MmsValueException("Value type is not boolean");
        }
        public double ToDouble()
        {
            if (GetType() == MmsType.MMS_FLOAT)
                return MmsValue_toDouble(valueReference);
            else
                throw new MmsValueException("Value type is not float");
        }
        public override string ToString()
        {
            switch (GetType())
            {
                case MmsType.MMS_VISIBLE_STRING:
                case MmsType.MMS_STRING:
                    return Marshal.PtrToStringAnsi(MmsValue_toString(valueReference));
                case MmsType.MMS_BOOLEAN:
                    return GetBoolean().ToString();
                case MmsType.MMS_INTEGER:
                    return ToInt64().ToString();
                case MmsType.MMS_UNSIGNED:
                    return ToUint32().ToString();
                case MmsType.MMS_FLOAT:
                    return ToDouble().ToString();
                case MmsType.MMS_UTC_TIME:
                    return GetUtcTimeAsDateTimeOffset().ToString();
                case MmsType.MMS_BINARY_TIME:
                    return (MsTimeToDateTimeOffset(GetBinaryTimeAsUtcMs()).ToString());
                case MmsType.MMS_OCTET_STRING:
                    return BitConverter.ToString(getOctetString());
                case MmsType.MMS_BIT_STRING:
                    return GetBitStringAsString();
                case MmsType.MMS_STRUCTURE:
                    {
                        string retString = "{";

                        bool first = true;

                        foreach (MmsValue element in this)
                        {
                            if (first)
                            {
                                retString += element.ToString();

                                first = false;
                            }
                            else
                            {
                                retString += ", " + element.ToString();
                            }
                        }

                        retString += "}";

                        return retString;
                    }

                default:
                    return "unknown";
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new MmsValueEnumerator(this);
        }
        private class MmsValueEnumerator : IEnumerator
        {
            private MmsValue value;
            private int index = -1;
            public MmsValueEnumerator(MmsValue value)
            {
                this.value = value;
            }
            #region IEnumerator Members
            public void Reset()
            {
                index = -1;
            }
            public object Current
            {
                get { return value.GetElement(index); }
            }
            public bool MoveNext()
            {
                index++;

                if (index >= value.Size())
                    return false;
                else
                    return true;
            }
            #endregion
        }
    }
    public class MmsValueException : Exception
    {
        public MmsValueException(string message) : base(message) { }
    }
    public enum MmsType
    {
        /** array type (multiple elements of the same type) */
        MMS_ARRAY = 0,
        /** structure type (multiple elements of different types) */
        MMS_STRUCTURE = 1,
        /** boolean */
        MMS_BOOLEAN = 2,
        /** bit string */
        MMS_BIT_STRING = 3,
        /** signed integer */
        MMS_INTEGER = 4,
        /** unsigned integer */
        MMS_UNSIGNED = 5,
        /** floating point value (32 or 64 bit) */
        MMS_FLOAT = 6,
        /** octet string */
        MMS_OCTET_STRING = 7,
        /** visible string - ANSI string */
        MMS_VISIBLE_STRING = 8,
        /** Generalized time */
        MMS_GENERALIZED_TIME = 9,
        /** Binary time */
        MMS_BINARY_TIME = 10,
        /** Binary coded decimal (BCD) - not used */
        MMS_BCD = 11,
        /** object ID - not used */
        MMS_OBJ_ID = 12,
        /** Unicode string */
        MMS_STRING = 13,
        /** UTC time */
        MMS_UTC_TIME = 14,
        /** will be returned in case of an error (contains error code) */
        MMS_DATA_ACCESS_ERROR = 15
    }
    public enum MmsDataAccessError
    {
        NO_RESPONSE = -2, /* for server internal purposes only! */
        SUCCESS = -1,
        OBJECT_INVALIDATED = 0,
        HARDWARE_FAULT = 1,
        TEMPORARILY_UNAVAILABLE = 2,
        OBJECT_ACCESS_DENIED = 3,
        OBJECT_UNDEFINED = 4,
        INVALID_ADDRESS = 5,
        TYPE_UNSUPPORTED = 6,
        TYPE_INCONSISTENT = 7,
        OBJECT_ATTRIBUTE_INCONSISTENT = 8,
        OBJECT_ACCESS_UNSUPPORTED = 9,
        OBJECT_NONE_EXISTENT = 10,
        OBJECT_VALUE_INVALID = 11,
        UNKNOWN = 12,
    }
}
