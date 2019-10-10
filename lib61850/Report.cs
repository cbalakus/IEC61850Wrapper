using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace lib61850
{
    public class Report
    {
        const string dllPath = Connection.dllPath;
        #region DllImports
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr ClientReportControlBlock_create(string dataAttributeReference);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr IedConnection_getRCBValues(IntPtr connection, out int error, string rcbReference, IntPtr updateRcb);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern void IedConnection_setRCBValues(IntPtr connection, out int error, IntPtr rcb, UInt32 parametersMask, bool singleRequest);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern void IedConnection_installReportHandler(IntPtr connection, string rcbReference, string rptId, InternalReportHandler handler, IntPtr handlerParameter);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void InternalReportHandler(IntPtr parameter, IntPtr report);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr ClientReportControlBlock_getRptId(IntPtr self);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern void ClientReportControlBlock_setTrgOps(IntPtr self, int trgOps);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern void ClientReportControlBlock_setRptEna(IntPtr self, bool rptEna);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern void ClientReportControlBlock_setIntgPd(IntPtr self, UInt32 intgPd);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr IedConnection_readDataSetValues(IntPtr self, out int error, [MarshalAs(UnmanagedType.LPStr)] string dataSetReference, IntPtr dataSet);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern void ClientReportControlBlock_setOptFlds(IntPtr self, int optFlds);
        #endregion

        private IntPtr _rpt = IntPtr.Zero;
        public delegate void Handler(string address, double value, ReasonForInclusion reason);
        private Handler _handler = null;


        private bool flagRptId = false;
        private bool flagRptEna = false;
        private bool flagResv = false;
        private bool flagDataSetReference = false;
        private bool flagConfRev = false;
        private bool flagOptFlds = false;
        private bool flagBufTm = false;
        private bool flagSqNum = false;
        private bool flagTrgOps = false;
        private bool flagIntgPd = false;
        private bool flagGI = false;
        private bool flagPurgeBuf = false;
        private bool flagResvTms = false;
        private bool flagEntryId = false;
        private void resetFlags()
        {
            flagRptId = false;
            flagRptEna = false;
            flagResv = false;
            flagDataSetReference = false;
            flagConfRev = false;
            flagOptFlds = false;
            flagBufTm = false;
            flagSqNum = false;
            flagTrgOps = false;
            flagIntgPd = false;
            flagGI = false;
            flagPurgeBuf = false;
            flagResvTms = false;
            flagEntryId = false;
        }
        private uint GetMask()
        {
            uint mask = 0;
            if (flagRptId)
                mask += 1;
            if (flagRptEna)
                mask += 2;
            if (flagResv)
                mask += 4;
            if (flagDataSetReference)
                mask += 8;
            if (flagConfRev)
                mask += 16;
            if (flagOptFlds)
                mask += 32;
            if (flagBufTm)
                mask += 64;
            if (flagSqNum)
                mask += 128;
            if (flagTrgOps)
                mask += 256;
            if (flagIntgPd)
                mask += 512;
            if (flagGI)
                mask += 1024;
            if (flagPurgeBuf)
                mask += 2048;
            if (flagEntryId)
                mask += 4096;
            if (flagResvTms)
                mask += 16384;
            return mask;
        }

        public Report(string rptID, Connection _con, IntPtr _conPtr, Handler _handler, TriggerOptions _trg, uint _poll)
        {
            this._handler = _handler;
            int _err = 0;
            _rpt = ClientReportControlBlock_create(rptID);
            _err = 0;
            IedConnection_getRCBValues(_conPtr, out _err, rptID, _rpt);
            if (_err != 0)
            {
                Console.WriteLine(((IedError)_err).ToString());
                return;
            }
            string _repID = Marshal.PtrToStringAnsi(ClientReportControlBlock_getRptId(_rpt));
            IedConnection_installReportHandler(_conPtr, rptID, _repID, ReportHandler, IntPtr.Zero);
            ClientReportControlBlock_setTrgOps(_rpt, (int)_trg);
            flagTrgOps = true;
            if ((_trg & TriggerOptions.INTEGRITY) == TriggerOptions.INTEGRITY)
            {
                ClientReportControlBlock_setIntgPd(_rpt, _poll);
                flagIntgPd = true;
            }
            ClientReportControlBlock_setOptFlds(_rpt, (int)(ReportOptions.DATA_REFERENCE | ReportOptions.REASON_FOR_INCLUSION));
            flagOptFlds = true;
            ClientReportControlBlock_setRptEna(_rpt, true);
            flagRptEna = true;
            _err = 0;
            IedConnection_setRCBValues(_conPtr, out _err, _rpt, GetMask(), true);
            resetFlags();
            Console.WriteLine(((IedError)_err).ToString());
        }

        Reporting _rep = null;
        private void ReportHandler(IntPtr parameter, IntPtr report)
        {
            try
            {
                if (_rep == null)
                    _rep = new Reporting(report);
                MmsValue values = _rep.GetDataSetValues();
                for (int i = 0; i < values.Size(); i++)
                {
                    var reason = _rep.GetReasonForInclusion(i);
                    if (reason != ReasonForInclusion.REASON_NOT_INCLUDED)
                    {
                        var dos = _rep.GetDataReference(i);
                        WriteValue(values.GetElement(i), dos == null ? i + "" : dos, reason);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void WriteValue(MmsValue m, string dos, ReasonForInclusion reason)
        {
            try
            {
                switch (m.GetType())
                {
                    case MmsType.MMS_ARRAY:
                    case MmsType.MMS_STRUCTURE:
                        var size = m.Size();
                        foreach (MmsValue e in m)
                            WriteValue(e, dos, reason);
                        break;
                    case MmsType.MMS_BOOLEAN:
                        _handler?.Invoke(dos, m.GetBoolean() ? 1 : 0, reason);
                        break;
                    case MmsType.MMS_INTEGER:
                        _handler?.Invoke(dos, m.ToInt64(), reason);
                        break;
                    case MmsType.MMS_FLOAT:
                        _handler?.Invoke(dos, m.ToDouble(), reason);
                        break;
                    case MmsType.MMS_UNSIGNED:
                        _handler?.Invoke(dos, m.ToUint32(), reason);
                        break;
                    case MmsType.MMS_UTC_TIME:
                        //Console.WriteLine(dos + " : " + new DateTime(1970, 1, 1).AddMilliseconds(m.GetUtcTimeInMs()).ToString());
                        break;
                    case MmsType.MMS_BIT_STRING:
                        //Console.WriteLine(dos + " : " + m + " (Bit)");
                        break;
                    case MmsType.MMS_STRING:
                    case MmsType.MMS_OCTET_STRING:
                    case MmsType.MMS_VISIBLE_STRING:
                    case MmsType.MMS_GENERALIZED_TIME:
                    case MmsType.MMS_BINARY_TIME:
                    case MmsType.MMS_BCD:
                    case MmsType.MMS_OBJ_ID:
                    case MmsType.MMS_DATA_ACCESS_ERROR:
                    default:
                        Console.WriteLine(dos + " : " + m + " - " + m.GetType());
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
