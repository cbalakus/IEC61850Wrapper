using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace lib61850
{
    public enum ReasonForInclusion
    {
        /** the element is not included in the received report */
        REASON_NOT_INCLUDED = 0,
        /** the element is included due to a change of the data value */
        REASON_DATA_CHANGE = 1,
        /** the element is included due to a change in the quality of data */
        REASON_QUALITY_CHANGE = 2,
        /** the element is included due to an update of the data value */
        REASON_DATA_UPDATE = 3,
        /** the element is included due to a periodic integrity report task */
        REASON_INTEGRITY = 4,
        /** the element is included due to a general interrogation by the client */
        REASON_GI = 5,
        /** the reason for inclusion is unknown */
        REASON_UNKNOWN = 6
    }
    public class Reporting
    {
        const string dllPath = Connection.dllPath;
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr ClientReport_getDataSetValues(IntPtr self);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern int ClientReport_getReasonForInclusion(IntPtr self, int elementIndex);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr ClientReport_getRcbReference(IntPtr self);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr ClientReport_getDataReference(IntPtr self, int elementIndex);
        private IntPtr self,
            dataSetValues = IntPtr.Zero;
        private MmsValue values = null;
        public Reporting(IntPtr self)
        {
            this.self = self;
        }
        public MmsValue GetDataSetValues()
        {
            if (dataSetValues == IntPtr.Zero)
            {
                dataSetValues = ClientReport_getDataSetValues(self);
                if (dataSetValues == IntPtr.Zero)
                    throw new Exception("No report values available yet");
                values = new MmsValue(dataSetValues);
            }
            return values;
        }
        public ReasonForInclusion GetReasonForInclusion(int index)
        {
            if (values == null)
            {
                GetDataSetValues();
                if (values == null)
                    throw new Exception("No ReasonForInclusion available yet");
            }
            int dataSetSize = values.Size();
            if (index >= dataSetSize)
                throw new Exception("data set index out of range (count = " + dataSetSize + ")");
            return (ReasonForInclusion)ClientReport_getReasonForInclusion(self, index);
        }
        public string GetRcbReference()
        {
            IntPtr rcbRef = ClientReport_getRcbReference(self);
            return Marshal.PtrToStringAnsi(rcbRef);
        }
        public string GetDataReference(int index)
        {
            IntPtr dataRef = ClientReport_getDataReference(self, index);
            if (dataRef != IntPtr.Zero)
                return Marshal.PtrToStringAnsi(dataRef);
            else
                return null;
        }
    }
}
