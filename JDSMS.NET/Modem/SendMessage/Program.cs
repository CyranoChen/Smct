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

namespace SendMessage
{
	class SendMessage
	{
		public void Send()
		{
			try
			{
				// 初始化.
				SMSLib.Service srv = new SMSLib.Service();

				Console.WriteLine(srv.Description);
				Console.WriteLine("\n\nSendMessage: 金笛C#短信开发包，发送短信示例.\n\n");
				Console.WriteLine("建议采用Wavecom M1306B（Q2406B）以获得最佳发送效果。\n\n");

				// 指定发送端口.
				srv.AddGateway(new ModemGateway("modem.com1", "COM1", 9600, "Nokia", "6310i", srv.Logger));

				// 设定端口可以发送短信.
				srv.FindGateway("modem.com1").Outbound = true;
				
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

				
				// 发送一条短信. 如果发送多条短信，此处可以设定一个循环。
				OutboundMessage msg = new OutboundMessage("15665885670", "nuibisshshs ");
				
				// 设置短信编码格式。发送中文一定要设置为UCS2
				msg.Encoding = Message.MessageEncodings.ENCUCS2;
				
				// 设置状态报告
				msg.StatusReport = true;
				
				// 设置短信有效时间（小时）
				msg.ValidityPeriod = 48;
				if (srv.SendMessage(msg)) Console.WriteLine(msg);

				Console.WriteLine("短信发送结束，按回车<enter>键退出...");
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

		static void Main(string[] args)
		{
			SendMessage app = new SendMessage();
			app.Send();
		}
	}
}
