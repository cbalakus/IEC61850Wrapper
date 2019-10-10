using lib61850;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace read61850
{
    class Program
    {
        static void Main(string[] args)
        {
            IEC61850 _iec = new IEC61850("127.0.0.1", 102);
            string reportID = "simpleIOGenericIO/LLN0$RP$AnalogValuesRCB01";
            //string reportID = "VAMPRelay/LLN0$RP$urcbEV101";
            _iec.Subscribe(reportID, ValueHandler, TriggerOptions.INTEGRITY | TriggerOptions.GI | TriggerOptions.DATA_CHANGED);
            _iec.SendGI(reportID);
            Thread.Sleep(5000);
            _iec.SendGI(reportID);

            //Thread.Sleep(5000);
            //_iec.SendGI(reportID);


            Console.ReadKey();
        }
        static void ValueHandler(string address, double value, ReasonForInclusion reason)
        {
            Console.WriteLine(address + " : " + value + " - Reason:" + reason.ToString());
        }
    }
}
