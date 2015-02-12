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

namespace SendMessage
{
	class SendMessage
	{
		public void Send()
		{
			try
			{
				// ��ʼ��.
				SMSLib.Service srv = new SMSLib.Service();

				Console.WriteLine(srv.Description);
				Console.WriteLine("\n\nSendMessage: ���C#���ſ����������Ͷ���ʾ��.\n\n");
				Console.WriteLine("�������Wavecom M1306B��Q2406B���Ի����ѷ���Ч����\n\n");

				// ָ�����Ͷ˿�.
				srv.AddGateway(new ModemGateway("modem.com1", "COM1", 9600, "Nokia", "6310i", srv.Logger));

				// �趨�˿ڿ��Է��Ͷ���.
				srv.FindGateway("modem.com1").Outbound = true;
				
				// ��������...
				srv.StartService();

				// ��ȡGSM Modem�豸��Ϣ.
				Console.WriteLine("GSM Modem:");
				Console.WriteLine("	оƬ������    : {0}", srv.FindGateway("modem.com1").Manufacturer());
				Console.WriteLine("	�ͺ�          : {0}", srv.FindGateway("modem.com1").Model());
				Console.WriteLine("	SerialNo      : {0}", srv.FindGateway("modem.com1").SerialNo());
				Console.WriteLine("	IMSI          : {0}", srv.FindGateway("modem.com1").Imsi());
				Console.WriteLine("	S/W Version   : {0}", srv.FindGateway("modem.com1").SWVersion());
				Console.WriteLine("	����          : {0}%", srv.FindGateway("modem.com1").BatteryLevel());
				Console.WriteLine("	�ź�ǿ��      : {0}%", srv.FindGateway("modem.com1").SignalLevel());
				Console.WriteLine();

				
				// ����һ������. ������Ͷ������ţ��˴������趨һ��ѭ����
				OutboundMessage msg = new OutboundMessage("15665885670", "nuibisshshs ");
				
				// ���ö��ű����ʽ����������һ��Ҫ����ΪUCS2
				msg.Encoding = Message.MessageEncodings.ENCUCS2;
				
				// ����״̬����
				msg.StatusReport = true;
				
				// ���ö�����Чʱ�䣨Сʱ��
				msg.ValidityPeriod = 48;
				if (srv.SendMessage(msg)) Console.WriteLine(msg);

				Console.WriteLine("���ŷ��ͽ��������س�<enter>���˳�...");
				Console.Read();



				// ֹͣ����
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
