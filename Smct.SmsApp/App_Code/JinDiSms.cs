using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Smct.SmsApp
{
    public partial class JinDiSms
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
//        protected override void WndProc(ref Message m)
//        {
//            switch (m.Msg)
//            {
//                case SMSDLL_EVENTWND:
//                    int nCommPort = (int)m.WParam;
//                    int nEventID = (int)m.LParam;
//                    String strEventPara = GetEventPara();

//                    switch (nEventID)
//                    {
//                        case 1:
//                            {
//                                long nModemStatus = StrToNum(strEventPara);
//                                if (1 == nModemStatus)
//                                {
//                                    MessageBoxA(0, "打开端口成功!", "", 0x30);
//                                    GetPara();
//#if DEBUG
//                                    /// 设置开始发送时间和结束发送时间
//                                    DateTime CurTime = DateTime.Now;

//                                    /// 设置早8点到晚10点期间发送，缺省不设置是全天24小时发送
//                                    DateTime StartTime = new DateTime(CurTime.Year, CurTime.Month, CurTime.Day, 8, 0, 0);
//                                    DateTime EndTime = new DateTime(CurTime.Year, CurTime.Month, CurTime.Day, 22, 0, 0);
//                                    ERetCode eRetCode = ERetCode.RETCODE_ERROR;

//                                    /// 设置开始发送时间
//                                    eRetCode = SmsApi_SetSendStartTime(ConvertDateTimeDouble(StartTime));

//                                    /// 设置结束发送时间
//                                    eRetCode = SmsApi_SetSendEndTime(ConvertDateTimeDouble(EndTime));

//#endif
//                                }
//                                else
//                                    MessageBoxA(0, "打开端口失败!", "", 0x30);
//                            }
//                            break;
//                        case 2:
//                            {
//                                short nPosition = (short)StrToNum(strEventPara);
//                                /// 收到新短信，调用读取
//                                ERetCode eRetCode = ERetCode.RETCODE_ERROR;

//                                StringBuilder szMobileNumber = new StringBuilder("", 32);
//                                StringBuilder szTime = new StringBuilder("", 32);
//                                StringBuilder szContent = new StringBuilder("", 160);
//                                eRetCode = SmsApi_ReadMsg(nPosition, szMobileNumber, szTime, szContent);
//                                if (ERetCode.RETCODE_OK == eRetCode)
//                                {

//                                    MessageBoxA(0, "收到 “" + szMobileNumber.ToString() + "” 的来信，内容“" + szContent.ToString() + "”\r\n发送时间：“" + szTime.ToString() + "”，保存SIM卡位置：“" + nPosition.ToString() + "”", "", 0x30);

//                                }

//                            }
//                            break;
//                        case 3:
//                            MessageBoxA(0, strEventPara + "来电啦！", "", 0x30);
//                            break;
//                        case 4:
//                            {
//                                String strMsgID = "";
//                                String strDestTel = "";
//                                String strContent = "";
//                                String strSpliteIndex = "";
//                                String strStatus = "";
//                                int nFind = strEventPara.IndexOf("!#");
//                                if (-1 != nFind)
//                                {
//                                    strMsgID = strEventPara.Substring(0, nFind);
//                                    strEventPara.Substring(nFind + 2, strEventPara.Length - 2 - nFind);
//                                    nFind = -1;
//                                }
//                                nFind = strEventPara.IndexOf("!#");
//                                if (-1 != nFind)
//                                {
//                                    strDestTel = strEventPara.Substring(0, nFind);
//                                    strEventPara.Substring(nFind + 2, strEventPara.Length - 2 - nFind);
//                                    nFind = -1;
//                                }
//                                nFind = strEventPara.IndexOf("!#");
//                                if (-1 != nFind)
//                                {
//                                    strContent = strEventPara.Substring(0, nFind);
//                                    strEventPara.Substring(nFind + 2, strEventPara.Length - 2 - nFind);
//                                    nFind = -1;
//                                }
//                                nFind = strEventPara.IndexOf("!#");
//                                if (-1 != nFind)
//                                {
//                                    strSpliteIndex = strEventPara.Substring(0, nFind);
//                                    strEventPara.Substring(nFind + 2, strEventPara.Length - 2 - nFind);
//                                    nFind = -1;
//                                }
//                                int nStatus = (short)StrToNum(strEventPara);
//                                string strInfo;
//                                strInfo = "消息“" + strMsgID + "”发送结果通知：目标“" + strDestTel + "”内容“" + strContent + "”拆分序号“" + strSpliteIndex + "”";
//                                if (0 == nStatus)
//                                    strInfo += "发送成功！";
//                                else if (1 == nStatus)
//                                    strInfo += "发送超时！";
//                                else if (2 == nStatus)
//                                    strInfo += "发送失败！";
//                                MessageBoxA(0, strInfo, "", 0x30);
//                            }
//                            break;
//                        case 5:
//                            MessageBoxA(0, "SIM卡短信已满，无法再接收短信！", "", 0x30);
//                            break;
//                        case 6:
//                            MessageBoxA(0, "SIM卡余额已不足，请尽快充值，否则短信无法发送！", "", 0x30);
//                            break;
//                        default:
//                            break;
//                    }

//                    break;
//            }
//            base.WndProc(ref m);
//        }

        /// ERetCode,所有的API返回码定义
        public enum ERetCode
        {
            RETCODE_OK = 0,  //没有错误
            RETCODE_UNREG = 1,  //未注册
            RETCODE_PARAINVALID = 2,  //输入参数无效
            RETCODE_UNINIT = 3,  //未初始化
            RETCODE_INITREPEAT = 4,  //重复初始化
            RETCODE_WAITEVENT = 5,  //由于使用的异步工作方式，需要等待事件通知才能确定是否成功
            RETCODE_BUSY = 6,  //状态忙，通常前面的工作正在处理中，未完成前又发了新的命令才返回这个值
            RETCODE_ERROR = 7,  //发生错误
            RETCODE_TIMEOUT = 8   //超时
        }

        ///////////////////////////////// API函数定义  /////////////////////////////////
        /**
         * @brief 获取当前系统COMM串口个数，未初始化模块也可以调用
         *
         *
         * @param pVal 返回个数
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetCommCount)(OUT short *pVal);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_GetCommCount(ref short pVal);

        /**
         * @brief 获取指定编号的COMM串口序号，先调用SmsApi_GetCommCount获得个数
         *
         *
         * @param sIndex 编号，从0开始
         * @param pVal 返回COM串口序号
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetComm)(IN short sIndex,OUT short *pVal);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_GetComm(short sIndex, ref short pVal);
        /**
         * @brief 设置工作模式，在初始化模块前设置对初始化模块有效，在发送短信前设置对发送短信有效
         *
         *
         * @param bSync true表示同步方式，否则异步方式
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetWorkMode)(bool bSync);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_SetWorkMode(bool bSync);
        /**
         * @brief 获取当前工作模式，未初始化模块也可以调用
         *
         *
         * @param pSync true表示同步方式，否则异步方式
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetWorkMode)(OUT bool *pSync);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_GetWorkMode(ref bool pSync);
        /**
         * @brief 设置发送短信的起始序号，未初始化模块也可以调用，为保证短信序号唯一，最好不断增加此序号大小
         *
         *
         * @param ulMsgID 短信序号
         * @return 
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetMsgID)(IN ULONG ulMsgID);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_SetMsgID(ulong ulMsgID);
        /**
         * @brief 获得当前提交发送短信的序号，未初始化模块也可以调用
         *
         *
         * @param pMsgID 短信序号，提交发送短信后调用此方法可获得提交生成的ID，这个ID会在发送结果通知中作为唯一此短信的标识
         * @return 
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetMsgID)(OUT ULONG *pMsgID);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_GetMsgID(ref  ulong pMsgID);
        /**
         * @brief 初始化打开模块，必须初始化才能收发短信
         *
         *
         * @param sCommPort 指定端口(COM1就是1，COM2就是2，以此类推)，最大不超过256
         * @param lCommPara 指定通信波特率(0表示自动判断波特率，一般是9600或115200)
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_OpenCom)(IN short sCommPort,IN long lCommPara);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_OpenCom(short sCommPort, long lCommPara);
        /**
         * @brief 获取通讯波特率，未初始化模块也可以调用
         *
         *
         * @param pCommPara 返回通讯波特率
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetCommPara)(OUT long *pCommPara);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_GetCommPara(ref long pCommPara);
        /**
         * @brief 设置通讯波特率，未初始化模块也可以调用
         *
         *
         * @param lCommPara 通讯波特率，一般是9600或115200
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetCommPara)(IN long lCommPara);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_SetCommPara(long lCommPara);
        /**
         * @brief 获得设备名称，必须初始化成功后才能调用
         *
         *
         * @param szMobileName 输入参数，预先至少分配16长度的缓存
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetMobileName)(OUT char *szMobileName);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_GetMobileName(StringBuilder szMobileName);
        /**
         * @brief 获得模块型号，必须初始化成功后才能调用
         *
         *
         * @param szMobileModel 输入参数，预先至少分配32长度的缓存
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetMobileModel)(OUT char *szMobileModel);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_GetMobileModel(StringBuilder szMobileModel);
        /**
         * @brief 获得模块版本，必须初始化成功后才能调用
         *
         *
         * @param szMobileVersion 输入参数，预先至少分配32长度的缓存
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetMobileVersion)(OUT char *szMobileVersion);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_GetMobileVersion(StringBuilder szMobileVersion);
        /**
         * @brief 获得模块IMEI，必须初始化成功后才能调用
         *
         *
         * @param szMobileIMEI 输入参数，预先至少分配32长度的缓存
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetMobileIMEI)(OUT char *szMobileIMEI);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_GetMobileIMEI(StringBuilder szMobileIMEI);
        /**
         * @brief 获得短信中心号码，CDMA无效，必须初始化成功后才能调用
         *
         *
         * @param szSMCA 返回短信中心号码，是否加国际代码如86与发送短信规则一致
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetSmsc)(OUT char *szSMCA);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_GetSmsc(StringBuilder szSMCA);
        /**
         * @brief 设置短信中心号码，CDMA无效，必须初始化成功后才能调用，一般只需要设置一次
         *
         *
         * @param szSMCA 短信中心号码，是否加国际代码如86与发送短信规则一致
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetSmsc)(IN char *szSMCA);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_SetSmsc(StringBuilder szSMCA);
        /**
         * @brief 获取AT指令执行超时时间，未初始化模块也可以调用
         *
         *
         * @param pTimeout 返回超时时间，以秒为单位
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetTimeout)(OUT short *pTimeout);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_GetTimeout(ref short pTimeout);
        /**
         * @brief 设置AT指令执行超时时间，未初始化模块也可以调用
         *
         *
         * @param sTimeout 超时时间，以秒为单位，缺省15秒
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetTimeout)(IN short sTimeout);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_SetTimeout(short sTimeout);
        /**
         * @brief 设置接收事件通知消息的窗口句柄，未初始化模块也可以调用
         *
         *
         * @param hWnd 窗口句柄，如果不设置，无法接收相应的事件通知
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetEventWnd)(IN HWND hWnd);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_SetEventWnd(IntPtr hWnd);
        /**
         * @brief 发送文字短信，必须初始化成功后才能调用
         *
         *
         * @param szMobileNumber 目标号码，如果启用了国际代码标志，就不用加国籍代码如86，支持群发，以分号;间隔即可
         * @param szMsgContent	 发送内容，可超过70个汉字
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SendMsg)(IN char *szMobileNumber,\
        //IN char *szMsgContent);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_SendMsg(StringBuilder szMobileNumber, StringBuilder szMsgContent);

        /**
         * @brief 发送WAP PUSH短信，必须初始化成功后才能调用
         *
         *
         * @param szMobileNumber 目标号码，如果启用了国际代码标志，就不用加国籍代码如86，支持群发，以分号;间隔即可
         * @param szMsgSubject	 发送主题，尽量简短
          * @param szMsgUrl		 发送URL
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SendPushMsg)(IN char *szMobileNumber,\
        //        IN char *szMsgSubject,IN char *szMsgUrl);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_SendPushMsg(StringBuilder szMobileNumber, StringBuilder szMsgSubject, StringBuilder szMsgUrl);
        /**
         * @brief 读取指定位置短信，必须初始化成功后才能调用
         *
         *
         * @param sPosition 短信在SIM卡上的位置
         * @param szMobileNumber 返回来信号码，最少分配32长度缓存
         * @param szTime 返回对方发送时间，最少分配32长度缓存
         * @param szMsgContent 返回发送内容，最少分配140长度缓存
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_ReadMsg)(IN short sPosition,OUT char *szMobileNumber,\
        //					OUT char *szTime,OUT char *szMsgContent);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_ReadMsg(short sPosition, StringBuilder szMobileNumber, StringBuilder szTime, StringBuilder szMsgContent);
        /**
         * @brief 删除指定范围位置短信，必须初始化成功后才能调用
         *
         *
         * @param sStartPosition 短信在SIM卡上的开始位置
          * @param sEndPosition  短信在SIM卡上的结束位置，如果开始位置和结束位置一样，表示只删除开始位置短信
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_DeleteMsg)(IN short sStartPosition,IN short sEndPosition);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_DeleteMsg(short sStartPosition, short sEndPosition);
        /**
         * @brief 获取读取短信后是否自动删除短信标志，未初始化模块也可以调用
         *
         *
         * @param pAutoDelete 返回是否自动删除
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetAutoDelete)(OUT bool *pAutoDelete);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_GetAutoDelete(ref bool pAutoDelete);
        /**
         * @brief 设置读取短信后是否自动删除短信，未初始化模块也可以调用
         *
         *
         * @param bAutoDeleteFlag 是否自动删除
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetAutoDelete)(IN bool bAutoDeleteFlag);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_SetAutoDelete(bool bAutoDeleteFlag);
        /**
         * @brief 清理SIM卡上所有短信，必须初始化成功后才能调用
         *
         *
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_ClearSim)();
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_ClearSim();
        /**
         * @brief 获得SIM卡上短信数量，必须初始化成功后才能调用
         *
         *
         * @param pUsed 返回已经有的短信数量
         * @param pTotal 返回总计存储空间
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetSimNum)(OUT short *pUsed,OUT short *pTotal);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_GetSimNum(ref short pUsed, ref short pTotal);
        /**
         * @brief 执行AT命令，必须初始化成功后才能调用
         *
         *
         * @param ATCmd AT命令
         * @param pBuffLength 返回命令返回内容的长度，应用根据此长度分配对应的缓存再调用SmsApi_GetATCommandReturn获得返回字符串
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_ATCommand)(IN char *ATCmd,OUT short *pBuffLength);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_ATCommand(StringBuilder ATCmd, ref short pBuffLength);
        /**
         * @brief 获得AT命令执行结果，必须初始化成功后才能调用
         *
         *
         * @param pRetBuff 返回预先根据执行结果返回的长度分配缓存内容
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetATCommandReturn)(OUT char *pRetBuff);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_GetATCommandReturn(StringBuilder pRetBuff);
        /**
         * @brief 获取发送短信状态报告标记，未初始化模块也可以调用
         *
         *
         * @param pStatusReport 返回是否请求状态报告
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetStatusReport)(OUT bool *pStatusReport);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_GetStatusReport(ref bool pStatusReport);
        /**
         * @brief 设置读取短信后是否自动删除短信，未初始化模块也可以调用，此状态对后续短信都起作用
         *
         *
         * @param bStatusReport 是否请求状态报告，缺省否
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetStatusReport)(IN bool bStatusReport);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_SetStatusReport(bool bStatusReport);
        /**
         * @brief 获取是否发送闪烁文字短信，未初始化模块也可以调用
         *
         *
         * @param pFlashSms 返回是否请求状态报告
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetFlashSms)(OUT bool *pFlashSms);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_GetFlashSms(ref bool pFlashSms);
        /**
         * @brief 设置是否发送闪烁文字短信，未初始化模块也可以调用，此状态对后续短信都起作用
         *
         *
         * @param bFlashSms 是否请求状态报告，缺省否
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetFlashSms)(IN bool bFlashSms);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_SetFlashSms(bool bFlashSms);
        /**
         * @brief 获得发送短信时的签名，未初始化模块也可以调用
         *
         *
         * @param szSignName 返回自动在发送内容前添加的签名，预先至少分配32长度的缓存
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetSignName)(OUT char *szSignName);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_GetSignName(StringBuilder szSignName);
        /**
         * @brief 设置发送短信时的签名，未初始化模块也可以调用，此状态对后续短信都起作用
         *
         *
         * @param szSignName 自动在发送内容前添加签名
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetSignName)(IN char *szSignName);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_SetSignName(StringBuilder szSignName);
        /**
         * @brief 获取发送短信的有效期，未初始化模块也可以调用
         *
         *
         * @param pValidMinute 返回发送短信的有效期
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetMsgValidMinute)(OUT long *pValidMinute);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_GetMsgValidMinute(ref long pValidMinute);
        /**
         * @brief 设置发送短信的有效期，未初始化模块也可以调用
         *
         *
         * @param lValidMinute 发送短信的有效期
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetMsgValidMinute)(IN long lValidMinute);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_SetMsgValidMinute(long lValidMinute);
        /**
         * @brief 获取发送开始的时间，未初始化模块也可以调用
         *
         *
         * @param pSendStartTime 返回发送开始的时间
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetSendStartTime)(OUT DATE *pSendStartTime);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_GetSendStartTime(ref System.Double pSendStartTime);
        /**
         * @brief 设置发送开始的时间，未初始化模块也可以调用
         *
         *
         * @param sSendStartTime 发送开始的时间，缺省是0.0或与发送结束时间相同表示不限时间发送
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetSendStartTime)(IN DATE sSendStartTime);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_SetSendStartTime(System.Double sSendStartTime);
        /**
         * @brief 获取发送结束的时间，未初始化模块也可以调用
         *
         *
         * @param pSendEndTime 返回发送结束的时间
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetSendEndTime)(OUT DATE *pSendEndTime);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_GetSendEndTime(ref System.Double pSendEndTime);
        /**
         * @brief 设置发送结束的时间，未初始化模块也可以调用
         *
         *
         * @param sSendEndTime 发送结束的时间，缺省是0.0或与发送开始时间相同表示不限时间发送
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetSendEndTime)(IN DATE sSendEndTime);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_SetSendEndTime(System.Double sSendEndTime);
        /**
         * @brief 获得SIM卡序号，未初始化模块也可以调用
         *
         *
         * @param szSimCardID 返回SIM卡序号，不是手机号码，预先至少分配32长度的缓存
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetSimCardID)(OUT char *szSimCardID);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_GetSimCardID(StringBuilder szSimCardID);
        /**
         * @brief 获取发送的短信是否自动拆分发送，未初始化模块也可以调用
         *
         *
         * @param pAutoSplite 返回是否自动拆分发送
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetAutoSplite)(OUT bool *pAutoSplite);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_GetAutoSplite(ref bool pAutoSplite);
        /**
         * @brief 设置发送的短信是否自动拆分发送，未初始化模块也可以调用，此状态对后续短信都起作用
         *
         *
         * @param bAutoSplite 是否自动拆分发送
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetAutoSplite)(IN bool bAutoSplite);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_SetAutoSplite(bool bAutoSplite);
        /**
         * @brief 获得国别代码，未初始化模块也可以调用
         *
         *
         * @param szCountryCode 返回国别代码，是否加国际代码如86与发送短信规则一致
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetCountryCode)(OUT char *szCountryCode);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_GetCountryCode(StringBuilder szCountryCode);
        /**
         * @brief 设置国别代码，未初始化模块也可以调用，一般只需要设置一次，此状态对后续短信都起作用
         *
         *
         * @param szCountryCode 国别代码，缺省86代表中国
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetCountryCode)(IN char *szCountryCode);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_SetCountryCode(StringBuilder szCountryCode);
        /**
         * @brief 获取当前将要提交发送短信的优先级，未初始化模块也可以调用
         *
         *
         * @param pSendPriority 返回当前将要提交发送短信的优先级
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetSendPriority)(OUT short *pSendPriority);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_GetSendPriority(ref short pSendPriority);
        /**
         * @brief 设置当前将要提交发送短信的优先级，未初始化模块也可以调用，此状态对后续短信都起作用
         *
         *
         * @param sSendPriority 当前将要提交发送短信的优先级，1-32，数值越大，优先发送
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetSendPriority)(IN short sSendPriority);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_SetSendPriority(short sSendPriority);
        /**
         * @brief 获取将要发送英文短信，未初始化模块也可以调用
         *
         *
         * @param pEnglishMsg 返回将要发送英文短信
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetEnglishMsg)(OUT bool *pEnglishMsg);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_GetEnglishMsg(ref  bool pEnglishMsg);
        /**
         * @brief 设置将要发送英文短信，未初始化模块也可以调用，此状态对后续短信都起作用
         *
         *
         * @param bEnglishMsg 是否将要发送英文短信
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetEnglishMsg)(IN bool bEnglishMsg);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_SetEnglishMsg(bool bEnglishMsg);
        /**
         * @brief 获取设备当前信号强度，必须初始化成功后才能调用
         *
         *
         * @param pSignal 返回设备当前信号强度，小于15可能导致发送短信失败
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetSignal)(OUT short *pSignal);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_GetSignal(ref short pSignal);
        /**
         * @brief 获得呼叫转移号码，必须初始化成功后才能调用
         *
         *
         * @param szCallTransfer 返回国别代码，是否加国际代码如86与发送短信规则一致
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetCallTransfer)(OUT char *szCallTransfer);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_GetCallTransfer(StringBuilder szCallTransfer);
        /**
         * @brief 设置呼叫转移号码，必须初始化成功后才能调用，一般只需要设置一次，如果号码为空，表示取消当前所有转移号码
         *
         *
         * @param szCallTransfer 呼叫转移号码，必须SIM卡支持才能设置
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetCallTransfer)(IN char *szCallTransfer);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_SetCallTransfer(StringBuilder szCallTransfer);
        /**
         * @brief 获得Fireware版本，必须初始化成功后才能调用
         *
         *
         * @param szFirewareVerion 返回Fireware版本，最少分配64长度缓存
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetFirewareVerion)(OUT char *szFirewareVerion);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_GetFirewareVerion(StringBuilder szFirewareVerion);
        /**
         * @brief 获取当前排队等候发送的任务数量，必须初始化成功后才能调用
         *
         *
         * @param pWaitSend 返回当前排队等候发送的任务数量
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetWaitSend)(OUT ULONG *pWaitSend);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_GetWaitSend(ref ulong pWaitSend);
        /**
         * @brief 退出模块，当程序退出或不再需要短信服务时调用以便及时释放资源
         *
         *
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_CloseCom)();
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_CloseCom();
        /**
         * @brief 获得出错信息，当调用某个API出错时调用此方法获取出错信息
         *
         *
         * @param pErrInfo 返回内部缓冲的最近一次操作出错信息
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetErrInfo)(OUT char *pErrInfo);
        [DllImport("JindiSMSApi.dll")]
        static extern ERetCode SmsApi_GetErrInfo(StringBuilder pErrInfo);


        public static System.DateTime ConvertIntDateTime(double d)
        {
            System.DateTime time = System.DateTime.MinValue;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1899, 12, 30, 0, 0, 0));
            time = startTime.AddSeconds(d);
            return time;
        }

        /// <summary>        
        /// 将c# DateTime时间格式转换为Unix时间戳格式        
        /// </summary>        
        /// <param name="time">时间</param>        
        /// <returns>double</returns>        
        public static double ConvertDateTimeDouble(System.DateTime time)
        {
            double intResult = 0;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1899, 12, 30, 0, 0, 0));
            intResult = (time - startTime).TotalSeconds;
            return intResult;
        }

        string GetEventPara()
        {
            String strEventPara = "";
            RegistryKey Key;
            Key = Registry.CurrentUser;
            RegistryKey myreg = Key.OpenSubKey("Software\\JinDiSoft");

            strEventPara = myreg.GetValue("SmsEventPara").ToString();
            myreg.Close();
            return strEventPara;

        }
        long StrToNum(String strNumber)
        {
            long iNum = 0;
            int iLength = strNumber.Length;

            for (int j = 0; j < iLength; j++)
            {
                char c = strNumber[j];
                if (c == 0)
                    break;
                if (c <= '9' && c >= '0')
                {
                    iNum *= 10;
                    iNum = (iNum + (c - '0'));
                }
            }
            return iNum;
        }

        //public Form1()
        //{
        //    InitializeComponent();

        //}
        //protected override void OnLoad(EventArgs e)
        //{
        //    base.OnLoad(e);
        //    Init();
        //}
        //private void Init()
        //{
        //    if (this.IsHandleCreated)
        //    {
        //        /// 设置事件接收窗口句柄
        //        ERetCode eRetCode = SmsApi_SetEventWnd(this.Handle);
        //        if (ERetCode.RETCODE_OK != eRetCode)
        //        {
        //            MessageBoxA(0, "设置事件窗口句柄失败！", "", 0x30);
        //        }


        //        short sCommCount = 0;
        //        eRetCode = SmsApi_GetCommCount(ref sCommCount);

        //        for (short sIndex = 0; sIndex < sCommCount; sIndex++)
        //        {

        //            short sCommIndex = 0;
        //            eRetCode = SmsApi_GetComm(sIndex, ref sCommIndex);
        //            IDC_COMMLIST.Items.Add("COM" + sCommIndex.ToString());
        //        }

        //        if (sCommCount != 0)
        //        {
        //            IDC_COMMLIST.SelectedIndex = 0;
        //        }

        //        IDC_COMMPARALIST.Items.Add("自动检测");
        //        IDC_COMMPARALIST.Items.Add("9600");
        //        IDC_COMMPARALIST.Items.Add("19200");
        //        IDC_COMMPARALIST.Items.Add("38400");
        //        IDC_COMMPARALIST.Items.Add("57600");
        //        IDC_COMMPARALIST.Items.Add("115200");
        //        IDC_COMMPARALIST.SelectedIndex = 0;
        //        //IDC_COMMLIST.SelectedIndex = 0;
        //        IDC_EDIT_TIMEOUT.Text = "15";
        //        IDC_EDIT_MSGINDEX.Text = "1";
        //        IDC_EDIT_SIGNAL.Text = "0";
        //        IDC_EDIT_VALIDMINUTE.Text = "1440";
        //        IDC_EDIT_SENDPRI.Text = "16";
        //        IDC_CHECK_WORKMODE.Checked = false;
        //        IDC_CHECK_AUTOSPLITE.Checked = false;
        //        IDC_CHECK_ENGLISH.Checked = false;
        //        IDC_CHECK_READAUTODELETE.Checked = false;
        //        IDC_CHECK_FLASHMSG.Checked = false;
        //        IDC_CHECK_STATUSREPORT.Checked = false;
        //        IDC_EDIT_MSGCOUNT.Text = "";
        //        IDC_EDIT_MODEL.Text = "";
        //        IDC_EDIT_SMCA.Text = "";
        //        IDC_EDIT_DESTTEL.Text = "13601019694";
        //        IDC_EDIT_CONTENT.Text = "欢迎您使用本公司二次开发DLL模块。";
        //        IDC_EDIT_URL.Text = "http://wap.sohu.com";
        //        IDC_EDIT_TRANSTEL.Text = "";
        //        IDC_EDIT_ATCMDRETURN.Text = "";
        //        IDC_EDIT_ATCMD.Text = "AT+CMGL=\"1\"";
        //        IDC_EDIT_SIMCARD.Text = "";
        //        IDC_EDIT_NAME.Text = "";
        //        IDC_EDIT_VERSION.Text = "";
        //        IDC_EDIT_IMEI.Text = "";
        //        IDC_EDIT_SIGNNAME.Text = "[金笛短信]:";
        //        IDC_EDIT_READCONTENT.Text = "";
        //    }
        //}

        //[DllImport("user32.dll")]//, EntryPoint = "MessageBoxA")]
        //static extern int MessageBoxA(int hWnd, string msg, string caption, int type);

        //private void IDC_CHECK_WORKMODE_Click(object sender, EventArgs e)
        //{
        //    /// 设置工作模式
        //    bool bSync = false;
        //    if (IDC_CHECK_WORKMODE.Checked)
        //        bSync = true;
        //    ERetCode eRetCode = SmsApi_SetWorkMode(bSync);

        //}

        //private void IDC_CHECK_READAUTODELETE_Click(object sender, EventArgs e)
        //{
        //    /// 设置读取短信后是否自动删除短信
        //    bool bAutoDelete = false;
        //    if (IDC_CHECK_READAUTODELETE.Checked)
        //        bAutoDelete = true;
        //    ERetCode eRetCode = SmsApi_SetAutoDelete(bAutoDelete);
        //}

        //private void IDC_BUTTON_GETPARA_Click(object sender, EventArgs e)
        //{
        //    GetPara();

        //}

        //private void GetPara()
        //{
        //    ERetCode eRetCode = ERetCode.RETCODE_ERROR;

        //    StringBuilder szModel = new StringBuilder("", 32);
        //    /// 获得型号
        //    eRetCode = SmsApi_GetMobileModel(szModel);
        //    IDC_EDIT_MODEL.Text = szModel.ToString();

        //    StringBuilder szName = new System.Text.StringBuilder("", 16);
        //    /// 获得名称
        //    eRetCode = SmsApi_GetMobileName(szName);
        //    IDC_EDIT_NAME.Text = szName.ToString();


        //    StringBuilder szVersion = new System.Text.StringBuilder("", 64);
        //    /// 获得版本
        //    eRetCode = SmsApi_GetFirewareVerion(szVersion);
        //    IDC_EDIT_VERSION.Text = szVersion.ToString();

        //    StringBuilder szIMEI = new System.Text.StringBuilder("", 32);
        //    /// 获得IMEI
        //    eRetCode = SmsApi_GetMobileIMEI(szIMEI);
        //    IDC_EDIT_IMEI.Text = szIMEI.ToString();

        //    StringBuilder szSmca = new System.Text.StringBuilder("", 2);
        //    /// 获得短信中心号码
        //    eRetCode = SmsApi_GetSmsc(szSmca);
        //    IDC_EDIT_SMCA.Text = szSmca.ToString();

        //    StringBuilder szSimCard = new System.Text.StringBuilder("", 32);
        //    /// 获得SIM卡卡号
        //    eRetCode = SmsApi_GetSimCardID(szSimCard);
        //    IDC_EDIT_SIMCARD.Text = szSimCard.ToString();

        //    /// 获得设备信号强度
        //    short singal = 0;
        //    eRetCode = SmsApi_GetSignal(ref singal);
        //    IDC_EDIT_SIGNAL.Text = singal.ToString();
        //}

        //private void IDC_BUTTON_SETSMCA_Click(object sender, EventArgs e)
        //{
        //    if (IDC_EDIT_SMCA.Text == null || IDC_EDIT_SMCA.Text.Length == 0)
        //        return;
        //    /// 设置短信中心
        //    StringBuilder smca = new StringBuilder();
        //    smca.Append(IDC_EDIT_SMCA.Text);
        //    ERetCode eRetCode = SmsApi_SetSmsc(smca);

        //}

        //private void IDC_BUTTON_CHANGECOMMPARA_Click(object sender, EventArgs e)
        //{

        //    long lCommPara = 0;/// 自动检测
        //    int nCurSel = IDC_COMMPARALIST.SelectedIndex;
        //    switch (nCurSel)
        //    {
        //        case 1:
        //            lCommPara = 9600;
        //            break;
        //        case 2:
        //            lCommPara = 19200;
        //            break;
        //        case 3:
        //            lCommPara = 38400;
        //            break;
        //        case 4:
        //            lCommPara = 57600;
        //            break;
        //        case 5:
        //            lCommPara = 115200;
        //            break;
        //    }

        //    /// 设置通讯波特率
        //    ERetCode eRetCode = SmsApi_SetCommPara(lCommPara);
        //}

        //private void IDC_BUTTON_CONNECT_Click(object sender, EventArgs e)
        //{

        //    ERetCode eRetCode = ERetCode.RETCODE_ERROR;


        //    /// 设置工作模式
        //    bool bSync = false;
        //    if (IDC_CHECK_WORKMODE.Checked)
        //        bSync = true;
        //    eRetCode = SmsApi_SetWorkMode(bSync);

        //    /// 设置读取短信后是否自动删除短信
        //    bool bAutoDelete = false;
        //    if (IDC_CHECK_READAUTODELETE.Checked)
        //        bAutoDelete = true;
        //    eRetCode = SmsApi_SetAutoDelete(bAutoDelete);



        //    /// 设置AT指令超时
        //    short timeout = Int16.Parse(IDC_EDIT_TIMEOUT.Text);
        //    eRetCode = SmsApi_SetTimeout(timeout);

        //    StringBuilder cn = new StringBuilder();
        //    cn.Append("86");
        //    /// 设置国别代码
        //    eRetCode = SmsApi_SetCountryCode(cn);


        //    short sCommIndex = 0;
        //    SmsApi_GetComm((short)IDC_COMMLIST.SelectedIndex, ref sCommIndex);


        //    long lCommPara = 0;/// 自动检测
        //    int nCurSel = IDC_COMMPARALIST.SelectedIndex;
        //    switch (nCurSel)
        //    {
        //        case 1:
        //            lCommPara = 9600;
        //            break;
        //        case 2:
        //            lCommPara = 19200;
        //            break;
        //        case 3:
        //            lCommPara = 38400;
        //            break;
        //        case 4:
        //            lCommPara = 57600;
        //            break;
        //        case 5:
        //            lCommPara = 115200;
        //            break;
        //    }
        //    /// 打开模块
        //    eRetCode = SmsApi_OpenCom(sCommIndex, lCommPara);
        //    if (ERetCode.RETCODE_INITREPEAT == eRetCode)
        //    {
        //        MessageBoxA(0, "重复调用打开端口！", "", 0x30);
        //    }
        //    else if (ERetCode.RETCODE_ERROR == eRetCode)
        //    {

        //        StringBuilder szErrorInfo = new StringBuilder("", 256);
        //        /// 获得出错信息
        //        eRetCode = SmsApi_GetErrInfo(szErrorInfo);

        //        if (szErrorInfo != null && szErrorInfo.Length > 0)
        //            MessageBoxA(0, szErrorInfo.ToString(), "", 0x30);
        //        else
        //            MessageBoxA(0, "无法打开端口！", "", 0x30);


        //    }
        //    else if (ERetCode.RETCODE_OK == eRetCode)
        //    {
        //        MessageBoxA(0, "打开端口成功！", "", 0x30);
        //    }

        //    if (ERetCode.RETCODE_OK == eRetCode)
        //    {
        //        GetPara();
        //    }
        //}

        //private void IDC_BUTTON_CLOSE_Click(object sender, EventArgs e)
        //{

        //    /// 关闭模块
        //    ERetCode eRetCode = SmsApi_CloseCom();

        //}

        //private void IDC_BUTTON_SENDTEXTSMS_Click(object sender, EventArgs e)
        //{
        //    // TODO: 在此添加控件通知处理程序代码
        //    if (IDC_EDIT_CONTENT.Text.Length == 0 || IDC_EDIT_DESTTEL.Text.Length == 0)
        //        return;
        //    ERetCode eRetCode = ERetCode.RETCODE_ERROR;


        //    /// 设置签名
        //    StringBuilder szSignName = new StringBuilder();
        //    szSignName.Append(IDC_EDIT_SIGNNAME.Text);
        //    eRetCode = SmsApi_SetSignName(szSignName);



        //    /// 设置长短信是否自动拆分发送
        //    bool bAutoSplite = false;
        //    if (IDC_CHECK_AUTOSPLITE.Checked)
        //        bAutoSplite = true;
        //    eRetCode = SmsApi_SetAutoSplite(bAutoSplite);



        //    /// 设置短信有效期，分钟为单位
        //    eRetCode = SmsApi_SetMsgValidMinute(long.Parse(IDC_EDIT_VALIDMINUTE.Text));



        //    /// 设置发送优先级
        //    eRetCode = SmsApi_SetSendPriority(short.Parse(IDC_EDIT_SENDPRI.Text));



        //    /// 设置是否请求状态报告
        //    bool bStatusReport = false;
        //    if (IDC_CHECK_STATUSREPORT.Checked)
        //        bStatusReport = true;
        //    eRetCode = SmsApi_SetStatusReport(bStatusReport);


        //    /// 设置是否为英文短信
        //    bool bEnglish = false;
        //    if (IDC_CHECK_ENGLISH.Checked)
        //        bEnglish = true;
        //    eRetCode = SmsApi_SetEnglishMsg(bEnglish);



        //    /// 设置是否为诺基亚手机支持的闪信
        //    bool bFlashSms = false;
        //    if (IDC_CHECK_FLASHMSG.Checked)
        //        bFlashSms = true;
        //    eRetCode = SmsApi_SetFlashSms(bFlashSms);

        //    StringBuilder desttel = new StringBuilder();
        //    StringBuilder content = new StringBuilder();
        //    desttel.Append(IDC_EDIT_DESTTEL.Text);
        //    content.Append(IDC_EDIT_CONTENT.Text);

        //    eRetCode = SmsApi_SendMsg(desttel, content);

        //    ulong ulMsgID = 0;

        //    /// 获取提交短信后生成的编号
        //    SmsApi_GetMsgID(ref ulMsgID);

        //    if (ERetCode.RETCODE_OK == eRetCode)
        //        MessageBoxA(0, "发送成功！", "", 0x30);
        //    else if (ERetCode.RETCODE_WAITEVENT == eRetCode)
        //        MessageBoxA(0, "提交成功！", "", 0x30);
        //    else
        //        MessageBoxA(0, "发送失败！", "", 0x30);
        //}

        //private void IDC_BUTTON_SENDWAPPUSH_Click(object sender, EventArgs e)
        //{

        //    if (IDC_EDIT_CONTENT.Text.Length == 0 || IDC_EDIT_DESTTEL.Text.Length == 0 || IDC_EDIT_URL.Text.Length == 0)
        //        return;
        //    ERetCode eRetCode = ERetCode.RETCODE_ERROR;

        //    StringBuilder signname = new StringBuilder();
        //    signname.Append(IDC_EDIT_SIGNNAME.Text);
        //    eRetCode = SmsApi_SetSignName(signname);


        //    /// 设置短信有效期，分钟为单位
        //    eRetCode = SmsApi_SetMsgValidMinute(long.Parse(IDC_EDIT_VALIDMINUTE.Text));



        //    /// 设置是否请求状态报告
        //    bool bStatusReport = false;
        //    if (IDC_CHECK_STATUSREPORT.Checked)
        //        bStatusReport = true;
        //    eRetCode = SmsApi_SetStatusReport(bStatusReport);

        //    /// 设置发送优先级
        //    eRetCode = SmsApi_SetSendPriority(short.Parse(IDC_EDIT_SENDPRI.Text));

        //    StringBuilder desttel = new StringBuilder();
        //    StringBuilder content = new StringBuilder();
        //    StringBuilder url = new StringBuilder();
        //    desttel.Append(IDC_EDIT_DESTTEL.Text);
        //    content.Append(IDC_EDIT_CONTENT.Text);
        //    url.Append(IDC_EDIT_URL.Text);
        //    eRetCode = SmsApi_SendPushMsg(desttel, content, url);

        //    ulong ulMsgID = 0;

        //    /// 获取提交短信后生成的编号
        //    SmsApi_GetMsgID(ref ulMsgID);

        //    if (ERetCode.RETCODE_OK == eRetCode)
        //        MessageBoxA(0, "发送成功！", "", 0x30);
        //    else if (ERetCode.RETCODE_WAITEVENT == eRetCode)
        //        MessageBoxA(0, "提交到发送队列成功！", "", 0x30);
        //    else
        //        MessageBoxA(0, "提交到发送队列失败！", "", 0x30);
        //}

        //private void IDC_BUTTON_GETMSGCOUNT_Click(object sender, EventArgs e)
        //{

        //    /// 获得SIM卡上短信数量
        //    short sUsed = 0, sTotal = 0;
        //    ERetCode eRetCode = SmsApi_GetSimNum(ref sUsed, ref sTotal);

        //    IDC_EDIT_MSGCOUNT.Text = sUsed.ToString() + "/" + sTotal.ToString();


        //}

        //private void IDC_BUTTON_CLEAR_Click(object sender, EventArgs e)
        //{

        //    /// 清理SIM卡上的短信
        //    ERetCode eRetCode = SmsApi_ClearSim();

        //}

        //private void IDC_BUTTON_READMSG_Click(object sender, EventArgs e)
        //{

        //    if (short.Parse(IDC_EDIT_MSGINDEX.Text) < 1)
        //        return;
        //    ERetCode eRetCode = ERetCode.RETCODE_ERROR;

        //    StringBuilder szMobileNumber = new StringBuilder("", 32);
        //    StringBuilder szTime = new StringBuilder("", 32);
        //    StringBuilder szContent = new StringBuilder("", 140);
        //    eRetCode = SmsApi_ReadMsg(short.Parse(IDC_EDIT_MSGINDEX.Text), szMobileNumber, szTime, szContent);
        //    if (ERetCode.RETCODE_OK == eRetCode)
        //    {
        //        IDC_EDIT_READCONTENT.Text = "收到 “" + szMobileNumber.ToString() + "” 的来信，内容“" + szContent.ToString() + "”\r\n发送时间：“" + szTime.ToString() + "”，保存SIM卡位置：“" + IDC_EDIT_MSGINDEX.Text + "”";
        //    }

        //}

        //private void IDC_BUTTON_DELMSG_Click(object sender, EventArgs e)
        //{

        //    if (short.Parse(IDC_EDIT_MSGINDEX.Text) < 1)
        //        return;

        //    /// 删除指定位置的短信
        //    ERetCode eRetCode = SmsApi_DeleteMsg(short.Parse(IDC_EDIT_MSGINDEX.Text), short.Parse(IDC_EDIT_MSGINDEX.Text));

        //}

        //private void IDC_BUTTON_DOATCOMMAND_Click(object sender, EventArgs e)
        //{

        //    if (IDC_EDIT_ATCMD.Text.Length == 0)
        //        return;
        //    ERetCode eRetCode = ERetCode.RETCODE_ERROR;
        //    short sRetLen = 0;

        //    /// 执行AT指令
        //    StringBuilder atcmd = new StringBuilder();
        //    atcmd.Append(IDC_EDIT_ATCMD.Text);
        //    eRetCode = SmsApi_ATCommand(atcmd, ref sRetLen);

        //    if (sRetLen > 0)
        //    {

        //        /// 获得AT指令返回字符串
        //        StringBuilder szATCmdReturn = new StringBuilder("", sRetLen);

        //        eRetCode = SmsApi_GetATCommandReturn(szATCmdReturn);
        //        IDC_EDIT_ATCMDRETURN.Text = szATCmdReturn.ToString();


        //    }
        //}

        //private void IDC_BUTTON_READTRANSTEL_Click(object sender, EventArgs e)
        //{

        //    /// 设置呼叫转移
        //    StringBuilder szTransTel = new StringBuilder();
        //    ERetCode eRetCode = SmsApi_GetCallTransfer(szTransTel);

        //    IDC_EDIT_TRANSTEL.Text = szTransTel.ToString();

        //}

        //private void IDC_BUTTON_SETTRANSTEL_Click(object sender, EventArgs e)
        //{
        //    StringBuilder transtel = new StringBuilder();
        //    transtel.Append(IDC_EDIT_TRANSTEL.Text);
        //    ERetCode eRetCode = SmsApi_SetCallTransfer(transtel);

        //}

        //private void IDCANCEL_Click(object sender, EventArgs e)
        //{
        //    this.Close();
        //}

    }
}