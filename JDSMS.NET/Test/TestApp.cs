/*
 * Wavecom M1306B GSM Modem send and receive SMS C# api .
 * 建议采用wavecom M1306B（Q2406B）以获得最佳发送效果。
 * 金笛短信网 www.sendsms.cn 
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
				Console.WriteLine("\n\nReadMessages: 金笛C#短信开发包，收发测试例程.\n\n");
				Console.WriteLine("\n\n建议采用Wavecom M1306B（Q2406B）以获得最佳发送效果。\n\n");

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

				Console.WriteLine(String.Format("发送 {0} messages.\n", srv.SendMessages(sendList)));
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
			Console.WriteLine("按回车<enter>键退出...");
			Console.Read();
		}
	}
}
