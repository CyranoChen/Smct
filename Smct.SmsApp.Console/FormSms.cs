using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;

using Smct.SmsApp.Entity;

namespace Smct.SmsApp.Console
{
    public partial class FormSms : Form
    {
        //接收事件窗口消息ID定义
        //#define SMSDLL_EVENTWND	WM_USER + 777
        /*@  窗口消息参数定义
          WPARAM 端口序号
          LPARAM 事件通知类型：1为异步连接短信猫返回结果通知；2为新短信通知；3、有来电通知；4、发送结果通知；5、SIM卡已满通知；6、SIM卡余额不足
          事件参数，从注册表固定位置(HKEY_CURRENT_USER\Software\JinDiSoft)读取字符串键值SmsEventPara。
          参数格式，如果是多个参数以!#分割，下面是针对每个事件的参数定义
          事件1：参数只有1个返回连接设备的状态，0表示连接失败，1表示连接成功，连接成功后才能进行发送短信等操作
          事件2：参数只有1个，是新短信在SIM卡中保存的序号，得到此序号后可以调用读取短信获取来信其它信息
          事件3：参数只有1个，是来电号码，可以不处理
          事件5：没有参数，收到此事件时，需要尽快读取短信后删除，否则无法接收新的短信
          事件6：没有参数，收到此事件时，需要尽快给SIM卡充值，否则无法再继续发送短信
          事件4：一共6个参数，第一个参数是发送短信的ID编号，第二个参数是发送的目标号码，第三个参数是发送的内容；第四个参数是发送时内容拆分的序号；第五个参数时发送的状态（0表示成功，1表示超时，2表示发送失败）
        */
        private const int WM_USER = 0x0400;
        private const int SMSDLL_EVENTWND = WM_USER + 777;
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case SMSDLL_EVENTWND:
                    int nCommPort = (int)m.WParam;
                    int nEventID = (int)m.LParam;
                    String strEventPara = SmsApiUtl.GetEventPara();

                    switch (nEventID)
                    {
                        case 1:
                            {
                                long nModemStatus = SmsApiUtl.StrToNum(strEventPara);
                                if (1 == nModemStatus)
                                {
                                    MessageBox.Show("打开端口成功!");
                                    GetPara();
#if DEBUG
                                    /// 设置开始发送时间和结束发送时间
                                    DateTime CurTime = DateTime.Now;

                                    /// 设置早8点到晚10点期间发送，缺省不设置是全天24小时发送
                                    DateTime StartTime = new DateTime(CurTime.Year, CurTime.Month, CurTime.Day, 8, 0, 0);
                                    DateTime EndTime = new DateTime(CurTime.Year, CurTime.Month, CurTime.Day, 22, 0, 0);
                                    ERetCode eRetCode = ERetCode.RETCODE_ERROR;

                                    /// 设置开始发送时间
                                    eRetCode = SmsApiUtl.SmsApi_SetSendStartTime(SmsApiUtl.ConvertDateTimeDouble(StartTime));

                                    /// 设置结束发送时间
                                    eRetCode = SmsApiUtl.SmsApi_SetSendEndTime(SmsApiUtl.ConvertDateTimeDouble(EndTime));

#endif
                                }
                                else
                                    MessageBoxA(0, "打开端口失败!", "", 0x30);
                            }
                            break;
                        case 2:
                            {
                                short nPosition = (short)SmsApiUtl.StrToNum(strEventPara);
                                /// 收到新短信，调用读取
                                ERetCode eRetCode = ERetCode.RETCODE_ERROR;

                                StringBuilder szMobileNumber = new StringBuilder("", 32);
                                StringBuilder szTime = new StringBuilder("", 32);
                                StringBuilder szContent = new StringBuilder("", 160);
                                eRetCode = SmsApiUtl.SmsApi_ReadMsg(nPosition, szMobileNumber, szTime, szContent);
                                if (ERetCode.RETCODE_OK == eRetCode)
                                {

                                    MessageBoxA(0, "收到 “" + szMobileNumber.ToString() + "” 的来信，内容“" + szContent.ToString() + "”\r\n发送时间：“" + szTime.ToString() + "”，保存SIM卡位置：“" + nPosition.ToString() + "”", "", 0x30);

                                }

                            }
                            break;
                        case 3:
                            MessageBoxA(0, strEventPara + "来电啦！", "", 0x30);
                            break;
                        case 4:
                            {
                                String strMsgID = "";
                                String strDestTel = "";
                                String strContent = "";
                                String strSpliteIndex = "";
                                //String strStatus = "";
                                int nFind = strEventPara.IndexOf("!#");
                                if (-1 != nFind)
                                {
                                    strMsgID = strEventPara.Substring(0, nFind);
                                    strEventPara.Substring(nFind + 2, strEventPara.Length - 2 - nFind);
                                    nFind = -1;
                                }
                                nFind = strEventPara.IndexOf("!#");
                                if (-1 != nFind)
                                {
                                    strDestTel = strEventPara.Substring(0, nFind);
                                    strEventPara.Substring(nFind + 2, strEventPara.Length - 2 - nFind);
                                    nFind = -1;
                                }
                                nFind = strEventPara.IndexOf("!#");
                                if (-1 != nFind)
                                {
                                    strContent = strEventPara.Substring(0, nFind);
                                    strEventPara.Substring(nFind + 2, strEventPara.Length - 2 - nFind);
                                    nFind = -1;
                                }
                                nFind = strEventPara.IndexOf("!#");
                                if (-1 != nFind)
                                {
                                    strSpliteIndex = strEventPara.Substring(0, nFind);
                                    strEventPara.Substring(nFind + 2, strEventPara.Length - 2 - nFind);
                                    nFind = -1;
                                }
                                int nStatus = (short)SmsApiUtl.StrToNum(strEventPara);
                                string strInfo;
                                strInfo = "消息“" + strMsgID + "”发送结果通知：目标“" + strDestTel + "”内容“" + strContent + "”拆分序号“" + strSpliteIndex + "”";
                                if (0 == nStatus)
                                    strInfo += "发送成功！";
                                else if (1 == nStatus)
                                    strInfo += "发送超时！";
                                else if (2 == nStatus)
                                    strInfo += "发送失败！";
                                MessageBoxA(0, strInfo, "", 0x30);
                            }
                            break;
                        case 5:
                            MessageBoxA(0, "SIM卡短信已满，无法再接收短信！", "", 0x30);
                            break;
                        case 6:
                            MessageBoxA(0, "SIM卡余额已不足，请尽快充值，否则短信无法发送！", "", 0x30);
                            break;
                        default:
                            break;
                    }

                    break;
            }
            base.WndProc(ref m);
        }

        public FormSms()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Init();
        }

        private void Init()
        {
            if (this.IsHandleCreated)
            {
                try
                {
                    /// 设置事件接收窗口句柄
                    ERetCode eRetCode = SmsApiUtl.SmsApi_SetEventWnd(this.Handle);

                    if (ERetCode.RETCODE_OK != eRetCode)
                    {
                        MessageBoxA(0, "设置事件窗口句柄失败！", "", 0x30);
                    }

                    short sCommCount = 0;
                    eRetCode = SmsApiUtl.SmsApi_GetCommCount(ref sCommCount);

                    for (short sIndex = 0; sIndex < sCommCount; sIndex++)
                    {
                        short sCommIndex = 0;
                        eRetCode = SmsApiUtl.SmsApi_GetComm(sIndex, ref sCommIndex);
                        IDC_COMMLIST.Items.Add("COM" + sCommIndex.ToString());
                    }

                    if (sCommCount != 0)
                    {
                        IDC_COMMLIST.SelectedIndex = 0;
                    }

                    IDC_COMMPARALIST.Items.Add("自动检测");
                    IDC_COMMPARALIST.Items.Add("9600");
                    IDC_COMMPARALIST.Items.Add("19200");
                    IDC_COMMPARALIST.Items.Add("38400");
                    IDC_COMMPARALIST.Items.Add("57600");
                    IDC_COMMPARALIST.Items.Add("115200");
                    IDC_COMMPARALIST.SelectedIndex = 0;
                    //IDC_COMMLIST.SelectedIndex = 0;
                    IDC_EDIT_TIMEOUT.Text = "15";
                    IDC_EDIT_MSGINDEX.Text = "1";
                    IDC_EDIT_SIGNAL.Text = "0";
                    IDC_EDIT_VALIDMINUTE.Text = "1440";
                    IDC_EDIT_SENDPRI.Text = "16";
                    IDC_CHECK_WORKMODE.Checked = false;
                    IDC_CHECK_AUTOSPLITE.Checked = false;
                    IDC_CHECK_ENGLISH.Checked = false;
                    IDC_CHECK_READAUTODELETE.Checked = false;
                    IDC_CHECK_FLASHMSG.Checked = false;
                    IDC_CHECK_STATUSREPORT.Checked = false;
                    IDC_EDIT_MSGCOUNT.Text = "";
                    IDC_EDIT_MODEL.Text = "";
                    IDC_EDIT_SMCA.Text = "";
                    IDC_EDIT_DESTTEL.Text = "13601019694";
                    IDC_EDIT_CONTENT.Text = "欢迎您使用本公司二次开发DLL模块。";
                    IDC_EDIT_URL.Text = "http://wap.sohu.com";
                    IDC_EDIT_TRANSTEL.Text = "";
                    IDC_EDIT_ATCMDRETURN.Text = "";
                    IDC_EDIT_ATCMD.Text = "AT+CMGL=\"1\"";
                    IDC_EDIT_SIMCARD.Text = "";
                    IDC_EDIT_NAME.Text = "";
                    IDC_EDIT_VERSION.Text = "";
                    IDC_EDIT_IMEI.Text = "";
                    IDC_EDIT_SIGNNAME.Text = "[金笛短信]:";
                    IDC_EDIT_READCONTENT.Text = "";
                }
                catch { }
            }
        }

        [DllImport("user32.dll")]//, EntryPoint = "MessageBoxA")]
        public static extern int MessageBoxA(int hWnd, string msg, string caption, int type);

        public void IDC_CHECK_WORKMODE_Click(object sender, EventArgs e)
        {
            /// 设置工作模式
            bool bSync = false;
            if (IDC_CHECK_WORKMODE.Checked)
                bSync = true;
            ERetCode eRetCode = SmsApiUtl.SmsApi_SetWorkMode(bSync);

        }

        private void IDC_CHECK_READAUTODELETE_Click(object sender, EventArgs e)
        {
            /// 设置读取短信后是否自动删除短信
            bool bAutoDelete = false;
            if (IDC_CHECK_READAUTODELETE.Checked)
                bAutoDelete = true;
            ERetCode eRetCode = SmsApiUtl.SmsApi_SetAutoDelete(bAutoDelete);
        }

        private void IDC_BUTTON_GETPARA_Click(object sender, EventArgs e)
        {
            GetPara();
        }

        private void GetPara()
        {
            ERetCode eRetCode = ERetCode.RETCODE_ERROR;

            StringBuilder szModel = new StringBuilder("", 32);
            /// 获得型号
            eRetCode = SmsApiUtl.SmsApi_GetMobileModel(szModel);
            IDC_EDIT_MODEL.Text = szModel.ToString();

            StringBuilder szName = new System.Text.StringBuilder("", 16);
            /// 获得名称
            eRetCode = SmsApiUtl.SmsApi_GetMobileName(szName);
            IDC_EDIT_NAME.Text = szName.ToString();


            StringBuilder szVersion = new System.Text.StringBuilder("", 64);
            /// 获得版本
            eRetCode = SmsApiUtl.SmsApi_GetFirewareVerion(szVersion);
            IDC_EDIT_VERSION.Text = szVersion.ToString();

            StringBuilder szIMEI = new System.Text.StringBuilder("", 32);
            /// 获得IMEI
            eRetCode = SmsApiUtl.SmsApi_GetMobileIMEI(szIMEI);
            IDC_EDIT_IMEI.Text = szIMEI.ToString();

            StringBuilder szSmca = new System.Text.StringBuilder("", 32);
            /// 获得短信中心号码
            eRetCode = SmsApiUtl.SmsApi_GetSmsc(szSmca);
            IDC_EDIT_SMCA.Text = szSmca.ToString();

            StringBuilder szSimCard = new System.Text.StringBuilder("", 32);
            /// 获得SIM卡卡号
            eRetCode = SmsApiUtl.SmsApi_GetSimCardID(szSimCard);
            IDC_EDIT_SIMCARD.Text = szSimCard.ToString();

            /// 获得设备信号强度
            short singal = 0;
            eRetCode = SmsApiUtl.SmsApi_GetSignal(ref singal);
            IDC_EDIT_SIGNAL.Text = singal.ToString();
        }

        private void IDC_BUTTON_SETSMCA_Click(object sender, EventArgs e)
        {
            if (IDC_EDIT_SMCA.Text == null || IDC_EDIT_SMCA.Text.Length == 0)
                return;
            /// 设置短信中心
            StringBuilder smca = new StringBuilder();
            smca.Append(IDC_EDIT_SMCA.Text);
            ERetCode eRetCode = SmsApiUtl.SmsApi_SetSmsc(smca);

        }

        private void IDC_BUTTON_CHANGECOMMPARA_Click(object sender, EventArgs e)
        {

            long lCommPara = 0;/// 自动检测
            int nCurSel = IDC_COMMPARALIST.SelectedIndex;
            switch (nCurSel)
            {
                case 1:
                    lCommPara = 9600;
                    break;
                case 2:
                    lCommPara = 19200;
                    break;
                case 3:
                    lCommPara = 38400;
                    break;
                case 4:
                    lCommPara = 57600;
                    break;
                case 5:
                    lCommPara = 115200;
                    break;
            }

            /// 设置通讯波特率
            ERetCode eRetCode = SmsApiUtl.SmsApi_SetCommPara(lCommPara);
        }

        private void IDC_BUTTON_CONNECT_Click(object sender, EventArgs e)
        {
            ERetCode eRetCode = ERetCode.RETCODE_ERROR;

            /// 设置工作模式
            bool bSync = false;
            if (IDC_CHECK_WORKMODE.Checked)
                bSync = true;
            eRetCode = SmsApiUtl.SmsApi_SetWorkMode(bSync);

            /// 设置读取短信后是否自动删除短信
            bool bAutoDelete = false;
            if (IDC_CHECK_READAUTODELETE.Checked)
                bAutoDelete = true;
            eRetCode = SmsApiUtl.SmsApi_SetAutoDelete(bAutoDelete);



            /// 设置AT指令超时
            short timeout = Int16.Parse(IDC_EDIT_TIMEOUT.Text);
            eRetCode = SmsApiUtl.SmsApi_SetTimeout(timeout);

            StringBuilder cn = new StringBuilder();
            cn.Append("86");
            /// 设置国别代码
            eRetCode = SmsApiUtl.SmsApi_SetCountryCode(cn);


            short sCommIndex = 0;
            SmsApiUtl.SmsApi_GetComm((short)IDC_COMMLIST.SelectedIndex, ref sCommIndex);


            long lCommPara = 0;/// 自动检测
            int nCurSel = IDC_COMMPARALIST.SelectedIndex;
            switch (nCurSel)
            {
                case 1:
                    lCommPara = 9600;
                    break;
                case 2:
                    lCommPara = 19200;
                    break;
                case 3:
                    lCommPara = 38400;
                    break;
                case 4:
                    lCommPara = 57600;
                    break;
                case 5:
                    lCommPara = 115200;
                    break;
            }
            /// 打开模块
            eRetCode = SmsApiUtl.SmsApi_OpenCom(sCommIndex, lCommPara);
            if (ERetCode.RETCODE_INITREPEAT == eRetCode)
            {
                MessageBoxA(0, "重复调用打开端口！", "", 0x30);
            }
            else if (ERetCode.RETCODE_ERROR == eRetCode)
            {

                StringBuilder szErrorInfo = new StringBuilder("", 256);
                /// 获得出错信息
                eRetCode = SmsApiUtl.SmsApi_GetErrInfo(szErrorInfo);

                if (szErrorInfo != null && szErrorInfo.Length > 0)
                    MessageBoxA(0, szErrorInfo.ToString(), "", 0x30);
                else
                    MessageBoxA(0, "无法打开端口！", "", 0x30);


            }
            else if (ERetCode.RETCODE_OK == eRetCode)
            {
                MessageBoxA(0, "打开端口成功！", "", 0x30);
            }

            if (ERetCode.RETCODE_OK == eRetCode)
            {
                GetPara();
            }
        }

        private void IDC_BUTTON_CLOSE_Click(object sender, EventArgs e)
        {

            /// 关闭模块
            ERetCode eRetCode = SmsApiUtl.SmsApi_CloseCom();

        }

        private void IDC_BUTTON_SENDTEXTSMS_Click(object sender, EventArgs e)
        {
            // TODO: 在此添加控件通知处理程序代码
            if (IDC_EDIT_CONTENT.Text.Length == 0 || IDC_EDIT_DESTTEL.Text.Length == 0)
                return;
            ERetCode eRetCode = ERetCode.RETCODE_ERROR;


            /// 设置签名
            StringBuilder szSignName = new StringBuilder();
            szSignName.Append(IDC_EDIT_SIGNNAME.Text);
            eRetCode = SmsApiUtl.SmsApi_SetSignName(szSignName);



            /// 设置长短信是否自动拆分发送
            bool bAutoSplite = false;
            if (IDC_CHECK_AUTOSPLITE.Checked)
                bAutoSplite = true;
            eRetCode = SmsApiUtl.SmsApi_SetAutoSplite(bAutoSplite);



            /// 设置短信有效期，分钟为单位
            eRetCode = SmsApiUtl.SmsApi_SetMsgValidMinute(long.Parse(IDC_EDIT_VALIDMINUTE.Text));



            /// 设置发送优先级
            eRetCode = SmsApiUtl.SmsApi_SetSendPriority(short.Parse(IDC_EDIT_SENDPRI.Text));



            /// 设置是否请求状态报告
            bool bStatusReport = false;
            if (IDC_CHECK_STATUSREPORT.Checked)
                bStatusReport = true;
            eRetCode = SmsApiUtl.SmsApi_SetStatusReport(bStatusReport);


            /// 设置是否为英文短信
            bool bEnglish = false;
            if (IDC_CHECK_ENGLISH.Checked)
                bEnglish = true;
            eRetCode = SmsApiUtl.SmsApi_SetEnglishMsg(bEnglish);



            /// 设置是否为诺基亚手机支持的闪信
            bool bFlashSms = false;
            if (IDC_CHECK_FLASHMSG.Checked)
                bFlashSms = true;
            eRetCode = SmsApiUtl.SmsApi_SetFlashSms(bFlashSms);

            StringBuilder desttel = new StringBuilder();
            StringBuilder content = new StringBuilder();
            desttel.Append(IDC_EDIT_DESTTEL.Text);
            content.Append(IDC_EDIT_CONTENT.Text);

            eRetCode = SmsApiUtl.SmsApi_SendMsg(desttel, content);

            ulong ulMsgID = 0;

            /// 获取提交短信后生成的编号
            SmsApiUtl.SmsApi_GetMsgID(ref ulMsgID);

            if (ERetCode.RETCODE_OK == eRetCode)
                MessageBoxA(0, "发送成功！", "", 0x30);
            else if (ERetCode.RETCODE_WAITEVENT == eRetCode)
                MessageBoxA(0, "提交成功！", "", 0x30);
            else
                MessageBoxA(0, "发送失败！", "", 0x30);
        }

        private void IDC_BUTTON_SENDWAPPUSH_Click(object sender, EventArgs e)
        {

            if (IDC_EDIT_CONTENT.Text.Length == 0 || IDC_EDIT_DESTTEL.Text.Length == 0 || IDC_EDIT_URL.Text.Length == 0)
                return;
            ERetCode eRetCode = ERetCode.RETCODE_ERROR;

            StringBuilder signname = new StringBuilder();
            signname.Append(IDC_EDIT_SIGNNAME.Text);
            eRetCode = SmsApiUtl.SmsApi_SetSignName(signname);


            /// 设置短信有效期，分钟为单位
            eRetCode = SmsApiUtl.SmsApi_SetMsgValidMinute(long.Parse(IDC_EDIT_VALIDMINUTE.Text));



            /// 设置是否请求状态报告
            bool bStatusReport = false;
            if (IDC_CHECK_STATUSREPORT.Checked)
                bStatusReport = true;
            eRetCode = SmsApiUtl.SmsApi_SetStatusReport(bStatusReport);

            /// 设置发送优先级
            eRetCode = SmsApiUtl.SmsApi_SetSendPriority(short.Parse(IDC_EDIT_SENDPRI.Text));

            StringBuilder desttel = new StringBuilder();
            StringBuilder content = new StringBuilder();
            StringBuilder url = new StringBuilder();
            desttel.Append(IDC_EDIT_DESTTEL.Text);
            content.Append(IDC_EDIT_CONTENT.Text);
            url.Append(IDC_EDIT_URL.Text);
            eRetCode = SmsApiUtl.SmsApi_SendPushMsg(desttel, content, url);

            ulong ulMsgID = 0;

            /// 获取提交短信后生成的编号
            SmsApiUtl.SmsApi_GetMsgID(ref ulMsgID);

            if (ERetCode.RETCODE_OK == eRetCode)
                MessageBoxA(0, "发送成功！", "", 0x30);
            else if (ERetCode.RETCODE_WAITEVENT == eRetCode)
                MessageBoxA(0, "提交到发送队列成功！", "", 0x30);
            else
                MessageBoxA(0, "提交到发送队列失败！", "", 0x30);
        }

        private void IDC_BUTTON_GETMSGCOUNT_Click(object sender, EventArgs e)
        {

            /// 获得SIM卡上短信数量
            short sUsed = 0, sTotal = 0;
            ERetCode eRetCode = SmsApiUtl.SmsApi_GetSimNum(ref sUsed, ref sTotal);

            IDC_EDIT_MSGCOUNT.Text = sUsed.ToString() + "/" + sTotal.ToString();


        }

        private void IDC_BUTTON_CLEAR_Click(object sender, EventArgs e)
        {

            /// 清理SIM卡上的短信
            ERetCode eRetCode = SmsApiUtl.SmsApi_ClearSim();

        }

        private void IDC_BUTTON_READMSG_Click(object sender, EventArgs e)
        {

            if (short.Parse(IDC_EDIT_MSGINDEX.Text) < 1)
                return;
            ERetCode eRetCode = ERetCode.RETCODE_ERROR;

            StringBuilder szMobileNumber = new StringBuilder("", 32);
            StringBuilder szTime = new StringBuilder("", 32);
            StringBuilder szContent = new StringBuilder("", 140);
            eRetCode = SmsApiUtl.SmsApi_ReadMsg(short.Parse(IDC_EDIT_MSGINDEX.Text), szMobileNumber, szTime, szContent);
            if (ERetCode.RETCODE_OK == eRetCode)
            {
                IDC_EDIT_READCONTENT.Text = "收到 “" + szMobileNumber.ToString() + "” 的来信，内容“" + szContent.ToString() + "”\r\n发送时间：“" + szTime.ToString() + "”，保存SIM卡位置：“" + IDC_EDIT_MSGINDEX.Text + "”";
            }

        }

        private void IDC_BUTTON_DELMSG_Click(object sender, EventArgs e)
        {

            if (short.Parse(IDC_EDIT_MSGINDEX.Text) < 1)
                return;

            /// 删除指定位置的短信
            ERetCode eRetCode = SmsApiUtl.SmsApi_DeleteMsg(short.Parse(IDC_EDIT_MSGINDEX.Text), short.Parse(IDC_EDIT_MSGINDEX.Text));

        }

        private void IDC_BUTTON_DOATCOMMAND_Click(object sender, EventArgs e)
        {

            if (IDC_EDIT_ATCMD.Text.Length == 0)
                return;
            ERetCode eRetCode = ERetCode.RETCODE_ERROR;
            short sRetLen = 0;

            /// 执行AT指令
            StringBuilder atcmd = new StringBuilder();
            atcmd.Append(IDC_EDIT_ATCMD.Text);
            eRetCode = SmsApiUtl.SmsApi_ATCommand(atcmd, ref sRetLen);

            if (sRetLen > 0)
            {

                /// 获得AT指令返回字符串
                StringBuilder szATCmdReturn = new StringBuilder("", sRetLen);

                eRetCode = SmsApiUtl.SmsApi_GetATCommandReturn(szATCmdReturn);
                IDC_EDIT_ATCMDRETURN.Text = szATCmdReturn.ToString();


            }
        }

        private void IDC_BUTTON_READTRANSTEL_Click(object sender, EventArgs e)
        {

            /// 设置呼叫转移
            StringBuilder szTransTel = new StringBuilder();
            ERetCode eRetCode = SmsApiUtl.SmsApi_GetCallTransfer(szTransTel);

            IDC_EDIT_TRANSTEL.Text = szTransTel.ToString();

        }

        private void IDC_BUTTON_SETTRANSTEL_Click(object sender, EventArgs e)
        {
            StringBuilder transtel = new StringBuilder();
            transtel.Append(IDC_EDIT_TRANSTEL.Text);
            ERetCode eRetCode = SmsApiUtl.SmsApi_SetCallTransfer(transtel);

        }

        private void IDCANCEL_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
