using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace lib61850
{
    public class Connection
    {

        //public const string dllPath = @"C:\Users\can.alakus\CMakeBuilds\69d30056-b408-a535-915d-ea1ff3edb0f3\build\x86-Debug\iec61850.dll";
        public const string dllPath = "C:/iec61850.dll";
        #region DllImports
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr IedConnection_create();
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern void IedConnection_destroy(IntPtr self);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern void IedConnection_setConnectTimeout(IntPtr self, UInt32 timeoutInMs);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern void IedConnection_connect(IntPtr self, out int error, string hostname, int tcpPort);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern void IedConnection_abort(IntPtr self, out int error);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern void IedConnection_installConnectionClosedHandler(IntPtr self, ConClosedHandler handler, IntPtr parameter);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ConClosedHandler(IntPtr parameter, IntPtr Iedconnection);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern void IedConnection_triggerGIReport(IntPtr self, out int error, string rcbReference);
        #endregion
        public IntPtr _con = IntPtr.Zero;
        public Connection(string ip, int port, uint timeOut)
        {
            _con = IedConnection_create();
            IedConnection_installConnectionClosedHandler(_con, ConClosed, _con);
            IedConnection_setConnectTimeout(_con, timeOut);
            int _err = 0;
            IedConnection_connect(_con, out _err, ip, port);
        }
        private void ConClosed(IntPtr parameter, IntPtr self)
        {
            Console.WriteLine("Connection closed.");
        }
        ~Connection()
        {
            if (_con != IntPtr.Zero)
            {
                IedConnection_destroy(_con);
                _con = IntPtr.Zero;
            }
        }
        public void SendGI(string reference)
        {
            int _err = 0;
            IedConnection_triggerGIReport(_con, out _err, reference);
            if (_err == 0)
                Console.WriteLine("GI Sended.");
            else
                Console.WriteLine("GI Result : " + ((IedError)_err).ToString());
        }
    }
}
