using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;


namespace Sip2RepeaterProxy
{
    public class Settings
    {
        private SipAccount accountOne = new SipAccount();
        private SipAccount accountTwo = new SipAccount();
        private static Settings instance = null;
        private Settings()
        {
            
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Settings Instance()
        {
            if(instance==null) instance = new Settings();
            return instance;
        }


        public SipAccount AccountSlotOne
        {
            get { return accountOne; };
            set { accountOne = value; } 
        }

        public SipAccount AccountSlotTwo
        {
            get { return accountTwo; }
            set { accountTwo = value; }
        }

        


        public String RepeaterIP { get; set; }
        public UInt16 PortSlot1 { get; set; }
        public UInt16 PortSlot2 { get; set; }





    }
}
