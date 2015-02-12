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

namespace ReadMessages
{
	class ReadMessages
	{		
		public void Read()
		{
			LinkedList<InboundMessage> msgList = new LinkedList<InboundMessage>();

			try
			{
				// 初始化 service.
				SMSLib.Service srv = new SMSLib.Service();

				Console.WriteLine(srv.Description);
				Console.WriteLine("\n\nReadMessages: 金笛C#短信开发包：读GSM短信示例.\n\n");
				Console.WriteLine("建议采用Wavecom M1306B（Q2406B）以获得最佳发送效果。\n\n");

				// 指定发送端口.
				srv.AddGateway(new ModemGateway("modem.com1", "COM1", 115200, "Nokia", "6310i", srv.Logger));
				
				// 设定端口可以接收短信.
				srv.FindGateway("modem.com1").Inbound = true;
				
				// 设定收到短信后自动提醒.
				srv.FindGateway("modem.com1").callHandlers += new AGateway.CallHandler(CallNotifier);
				srv.FindGateway("modem.com1").inboundMessageHandlers += new AGateway.InboundMessageHandler(InboundMessageNotifier);
				
				// 启动服务...
				srv.StartService();

				// 获取GSM Modem设备信息.
				Console.WriteLine("GSM Modem:");
				Console.WriteLine("	芯片制造商    : {0}", srv.FindGateway("modem.com1").Manufacturer());
				Console.WriteLine("	型号          : {0}", srv.FindGateway("modem.com1").Model());
				Console.WriteLine("	SerialNo      : {0}", srv.FindGateway("modem.com1").SerialNo());
				Console.WriteLine("	IMSI          : {0}", srv.FindGateway("modem.com1").Imsi());
				Console.WriteLine("	S/W Version   : {0}", srv.FindGateway("modem.com1").SWVersion());
				Console.WriteLine("	电量          : {0}%", srv.FindGateway("modem.com1").BatteryLevel());
				Console.WriteLine("	信号强度      : {0}%", srv.FindGateway("modem.com1").SignalLevel());
				Console.WriteLine();

				// 读取并显示所有收到的短信.
				srv.ReadMessages(msgList, InboundMessage.MessageClasses.ALL);
				foreach (InboundMessage msg in msgList)
				{
					Console.WriteLine(msg);
					// 删除短信
					//srv.DeleteMessage(msg);
				}

				Console.WriteLine("程序运行结束，按回车<enter>键退出...");
				Console.Read();

				// 停止服务
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

		public void CallNotifier(AGateway gtw, string callerId)
		{
			Console.WriteLine(string.Format("电话打入: {0}.", callerId));
		}

		public void InboundMessageNotifier(AGateway gtw, Message.MessageTypes msgType, string memLoc, int memIndex)
		{
			Console.WriteLine(string.Format("收到短信 {0}:{1}.", memLoc, memIndex));
		}

		static void Main(string[] args)
		{
			ReadMessages app = new ReadMessages();
			app.Read();
		}
	}
}
