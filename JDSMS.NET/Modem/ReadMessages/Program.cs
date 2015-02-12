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

namespace ReadMessages
{
	class ReadMessages
	{		
		public void Read()
		{
			LinkedList<InboundMessage> msgList = new LinkedList<InboundMessage>();

			try
			{
				// ��ʼ�� service.
				SMSLib.Service srv = new SMSLib.Service();

				Console.WriteLine(srv.Description);
				Console.WriteLine("\n\nReadMessages: ���C#���ſ���������GSM����ʾ��.\n\n");
				Console.WriteLine("�������Wavecom M1306B��Q2406B���Ի����ѷ���Ч����\n\n");

				// ָ�����Ͷ˿�.
				srv.AddGateway(new ModemGateway("modem.com1", "COM1", 115200, "Nokia", "6310i", srv.Logger));
				
				// �趨�˿ڿ��Խ��ն���.
				srv.FindGateway("modem.com1").Inbound = true;
				
				// �趨�յ����ź��Զ�����.
				srv.FindGateway("modem.com1").callHandlers += new AGateway.CallHandler(CallNotifier);
				srv.FindGateway("modem.com1").inboundMessageHandlers += new AGateway.InboundMessageHandler(InboundMessageNotifier);
				
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

				// ��ȡ����ʾ�����յ��Ķ���.
				srv.ReadMessages(msgList, InboundMessage.MessageClasses.ALL);
				foreach (InboundMessage msg in msgList)
				{
					Console.WriteLine(msg);
					// ɾ������
					//srv.DeleteMessage(msg);
				}

				Console.WriteLine("�������н��������س�<enter>���˳�...");
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

		public void CallNotifier(AGateway gtw, string callerId)
		{
			Console.WriteLine(string.Format("�绰����: {0}.", callerId));
		}

		public void InboundMessageNotifier(AGateway gtw, Message.MessageTypes msgType, string memLoc, int memIndex)
		{
			Console.WriteLine(string.Format("�յ����� {0}:{1}.", memLoc, memIndex));
		}

		static void Main(string[] args)
		{
			ReadMessages app = new ReadMessages();
			app.Read();
		}
	}
}
