using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace lib61850
{

    public class DataSet
    {
        const string dllPath = Connection.dllPath;
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern void ClientDataSet_destroy(IntPtr self);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr ClientDataSet_getValues(IntPtr self);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr ClientDataSet_getReference(IntPtr self);

        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern int ClientDataSet_getDataSetSize(IntPtr self);

        private IntPtr nativeObject;
        private MmsValue values = null;
        private string reference = null;

        internal DataSet(IntPtr nativeObject)
        {
            this.nativeObject = nativeObject;
        }

        /// <summary>
        /// Gets the object reference of the data set
        /// </summary>
        /// <returns>
        /// object reference.
        /// </returns>
        public string GetReference()
        {
            if (reference == null)
            {
                IntPtr nativeString = ClientDataSet_getReference(nativeObject);

                reference = Marshal.PtrToStringAnsi(nativeString);
            }

            return reference;
        }

        /// <summary>
        /// Gets the values associated with the data set object
        /// </summary>
        /// <description>This function will return the locally stored values associated with the data set.
        /// These are the values received by the last request to the server. A call to this method doesn't
        /// invoke a request to the server! </description>
        /// <returns>
        /// The locally stored values of the data set (as MmsValue instance of type MMS_ARRAY)
        /// </returns>
        public MmsValue GetValues()
        {
            if (values == null)
            {
                IntPtr nativeValues = ClientDataSet_getValues(nativeObject);

                values = new MmsValue(nativeValues, false);
            }

            return values;
        }


        /// <summary>
        /// Gets the number of elements of the data set
        /// </summary>
        /// <returns>
        /// the number of elementes (data set members)
        /// </returns>
        public int GetSize()
        {
            return ClientDataSet_getDataSetSize(nativeObject);
        }

        ~DataSet()
        {
            ClientDataSet_destroy(nativeObject);
        }

        internal IntPtr getNativeInstance()
        {
            return nativeObject;
        }
    }
}
