/*
 * Wavecom M1306B GSM Modem send and receive SMS C# api .
 * �������wavecom M1306B��Q2406B���Ի����ѷ���Ч����
 * ��Ѷ����� www.sendsms.cn 
 */

using System;
using System.Collections.Generic;
using System.Text;

using SMSLib;
using SMSLib.Gateway;
using SMSLib.Exceptions;

namespace Test
{
	class TestApp
	{
		public void DoIt()
		{
			LinkedList<InboundMessage> readList = new LinkedList<InboundMessage>();
			LinkedList<OutboundMessage> sendList = new LinkedList<OutboundMessage>();

			try
			{
				SMSLib.Service srv = new SMSLib.Service();

				Console.WriteLine(srv.Description);
				Console.WriteLine("\n\nReadMessages: ���C#���ſ��������շ���������.\n\n");
				Console.WriteLine("\n\n�������Wavecom M1306B��Q2406B���Ի����ѷ���Ч����\n\n");

				srv.AddGateway(new TestGateway("test.1", srv.Logger));
				srv.AddGateway(new TestGateway("test.2", srv.Logger));
				srv.FindGateway("test.1").Inbound = true;
				srv.FindGateway("test.1").Outbound = true;
				srv.FindGateway("test.2").Inbound = true;
				srv.FindGateway("test.2").Outbound = true;

				srv.StartService();
				srv.LoadBalancer = new SMSLib.Balancing.RoundRobinLoadBalancer(srv);

				//srv.ReadMessages(readList);
				//foreach (InboundMessage msg in readList)
				//	Console.WriteLine(msg);

				sendList.AddLast(new OutboundMessage("+301111", "Message #1"));
				sendList.AddLast(new OutboundMessage("+302222", "Message #2"));
				sendList.AddLast(new OutboundMessage("+303333", "Message #3"));

				Console.WriteLine(String.Format("���� {0} messages.\n", srv.SendMessages(sendList)));
				foreach (OutboundMessage msg in sendList)
					Console.WriteLine(msg);

				srv.StopService();

			}
			catch (SMSLibException e)
			{
				Console.WriteLine("JDSMSLib Exception: " + e.Message);
				Console.WriteLine(e.StackTrace);
			}
			catch (Exception e)
			{
				Console.WriteLine("Generic Exception: " + e.Message);
				Console.WriteLine(e.StackTrace);
			}
		}

		static void Main(string[] args)
		{
			TestApp app = new TestApp();
			app.DoIt();
			Console.WriteLine("���س�<enter>���˳�...");
			Console.Read();
		}
	}
}
