using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Independentsoft.Sip;
using Independentsoft.Sip.Sdp;
using Independentsoft.Sip.Methods;
using Independentsoft.Sip.Responses;


namespace Sip2RepeaterProxy
{
    public class CustomSipClient : IDisposable
    {
        private SipClient client; 
        public event Action<CallAction> CallEvent;
        private CallAction sipClientState = CallAction.CALLEND;
        private Object sipStateMutex = new object();
        private RequestResponse inviteRequestResponse = null;

        public CustomSipClient()
        {

        }


        public void Connect(String userName, String password, String serverIP, UInt16 port)
        {
            if(userName==null) throw new NullReferenceException("username is null");
            if(password==null) throw new NullReferenceException("password is null");
            if(serverIP==null) throw new NullReferenceException("server ip is null");


            client = new SipClient(serverIP, userName, password);
            client.LocalIPEndPoint = new IPEndPoint((Dns.GetHostAddresses(Dns.GetHostName()))[0],port);

            client.Connect();
            client.Register("sip:"+serverIP, "sip:" + userName + "@192.168.1.120", "sip:" +userName + "@" + client.LocalIPEndPoint.ToString());
        }


        public void CallTo(String targetUri)
        {
            inviteRequestResponse = client.Invite("sip:900@192.168.1.120", targetUri, "sip:900@" + client.LocalIPEndPoint.ToString(), GenerateSessionDescription());
            client.Ack(inviteRequestResponse);
        }


        public void AcceptCall()
        {
            OK okResponce = new OK();

            SessionDescription session = new SessionDescription();

            Owner owner = new Owner();
            owner.Username = "Bob";
            owner.SessionID = 2890844526;
            owner.Version = 2890844526;
            owner.Address = "192.168.0.2";

            session.Owner = owner;
            session.Name = "SIP Call";

            Connection connection = new Connection();
            connection.Address = "192.168.0.2";
            session.Connection = connection;

            Time time = new Time(0, 0);
            session.Time.Add(time);

            Media media1 = new Media();
            media1.Type = "audio";
            media1.Port = 49170;
            media1.TransportProtocol = "RTP/AVP";
            media1.Attributes.Add("rtpmap", "0 pcmu/8000");
            session.Media.Add(media1);
            okResponce.SessionDescription = session;
            client.SendResponse(okResponce);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }


        public void ReceiveRequestHandler(Object sender, RequestEventArgs e)
        {
            lock (sipStateMutex)
            {
                if (e.Request.Method == SipMethod.Ack)
                {
                    Console.WriteLine("ACK method");
                    client.AcceptRequest(e.Request);
                    CallEvent(CallAction.CALLSTART);
                    sipClientState = CallAction.CALLSTART;
                    return;
                }
                if (e.Request.Method == SipMethod.Cancel)
                {
                    client.AcceptRequest(e.Request);
                    CallEvent(CallAction.CALLEND);
                    return;
                }
                if (e.Request.Method == SipMethod.Bye)
                {
                    client.AcceptRequest(e.Request);
                    CallEvent(CallAction.CALLEND);
                    return;
                }
                if (e.Request.Method == SipMethod.Invite)
                {
                    OK ok = new OK();
                    Ringing ringing = new Ringing();
                    ringing.SessionDescription = GenerateSessionDescription();
                    Console.WriteLine("get invite - send ringing");
                    client.SendResponse(ringing, e.Request);
                    CallEvent(CallAction.RINGING);
                    return;
                }
             
                Console.WriteLine("get common, set accept request " + e.Request.Method.ToString());
                client.AcceptRequest(e.Request);
            }
        }


       


        private SessionDescription GenerateSessionDescription()
        {
            SessionDescription session = new SessionDescription();
            session.Version = 0;

            Owner owner = new Owner();
            owner.Username = "Bob";
            owner.SessionID = 16264;
            owner.Version = 18299;
            owner.Address = "192.168.1.16";

            session.Owner = owner;
            session.Name = "SIP Call";

            Connection connection = new Connection();
            connection.Address = "192.168.1.16";

            session.Connection = connection;

            Time time = new Time(0, 0);
            session.Time.Add(time);

            Media media1 = new Media();
            media1.Type = "audio";
            media1.Port = 25282;
            media1.TransportProtocol = "RTP/AVP";
            media1.MediaFormats.Add("0");
            media1.MediaFormats.Add("101");

            media1.Attributes.Add("rtpmap", "0 pcmu/8000");
            media1.Attributes.Add("rtpmap", "101 telephone-event/8000");
            media1.Attributes.Add("fmtp", "101 0-11");

            session.Media.Add(media1);

            return session;
        }
       


    }
}
