using System;
using System.Collections.Generic;
using System.Text;

namespace lib61850
{
    public class IEC61850
    {
        private string ip = "";
        private int port = 102;
        private Connection _con;
        public IEC61850(string ip)
        {
            new IEC61850(ip, 102);
        }
        public IEC61850(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
            this._con = new Connection(this.ip, this.port, 5000);
        }
        public void Subscribe(string rptID, Report.Handler _hand, TriggerOptions _trg, uint poll)
        {
            new Report(rptID, this._con, _con._con, _hand, _trg, poll);
        }
        public void Subscribe(string rptID, Report.Handler _hand, TriggerOptions _trg)
        {
            Subscribe(rptID, _hand, _trg, 5000);
        }
        public void SendGI(string reference)
        {
            this._con.SendGI(reference);
        }
    }
}
