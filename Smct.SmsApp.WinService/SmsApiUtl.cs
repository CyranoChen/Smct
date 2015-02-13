using System;
using System.Runtime.InteropServices;
using System.Text;

using Microsoft.Win32;

namespace Smct.SmsApp.WinService
{
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

    public static class SmsApiUtl
    {
        public static ERetCode InitConnent()
        {
            ERetCode eRetCode = ERetCode.RETCODE_ERROR;

            /// 设置工作模式
            bool bSync = true;
            eRetCode = SmsApiUtl.SmsApi_SetWorkMode(bSync);

            /// 设置读取短信后是否自动删除短信
            bool bAutoDelete = false;
            eRetCode = SmsApiUtl.SmsApi_SetAutoDelete(bAutoDelete);

            /// 设置AT指令超时
            short timeout = 15;
            eRetCode = SmsApiUtl.SmsApi_SetTimeout(timeout);

            StringBuilder cn = new StringBuilder();
            cn.Append("86");
            /// 设置国别代码
            eRetCode = SmsApiUtl.SmsApi_SetCountryCode(cn);

            short sCommIndex = 0;
            SmsApiUtl.SmsApi_GetComm(0, ref sCommIndex);

            long lCommPara = 0;/// 自动检测
            //int nCurSel = IDC_COMMPARALIST.SelectedIndex;
            //switch (nCurSel)
            //{
            //    case 1:
            //        lCommPara = 9600;
            //        break;
            //    case 2:
            //        lCommPara = 19200;
            //        break;
            //    case 3:
            //        lCommPara = 38400;
            //        break;
            //    case 4:
            //        lCommPara = 57600;
            //        break;
            //    case 5:
            //        lCommPara = 115200;
            //        break;
            //}
            /// 打开模块
            eRetCode = SmsApiUtl.SmsApi_OpenCom(sCommIndex, lCommPara);

            return eRetCode;
        }

        public static ERetCode SendSms(string signName, string destTel, string smsContent)
        {
            ERetCode eRetCode = ERetCode.RETCODE_ERROR;
            if (string.IsNullOrEmpty(smsContent) || string.IsNullOrEmpty(destTel))
                return eRetCode;

            /// 设置签名
            StringBuilder szSignName = new StringBuilder();
            szSignName.Append(signName);
            eRetCode = SmsApiUtl.SmsApi_SetSignName(szSignName);

            /// 设置长短信是否自动拆分发送
            bool bAutoSplite = false;
            eRetCode = SmsApiUtl.SmsApi_SetAutoSplite(bAutoSplite);

            /// 设置短信有效期，分钟为单位
            eRetCode = SmsApiUtl.SmsApi_SetMsgValidMinute(1440);

            /// 设置发送优先级
            eRetCode = SmsApiUtl.SmsApi_SetSendPriority(16);

            /// 设置是否请求状态报告
            bool bStatusReport = false;
            eRetCode = SmsApiUtl.SmsApi_SetStatusReport(bStatusReport);

            /// 设置是否为英文短信
            bool bEnglish = false;
            eRetCode = SmsApiUtl.SmsApi_SetEnglishMsg(bEnglish);

            /// 设置是否为诺基亚手机支持的闪信
            bool bFlashSms = false;
            eRetCode = SmsApiUtl.SmsApi_SetFlashSms(bFlashSms);

            StringBuilder desttel = new StringBuilder();
            StringBuilder content = new StringBuilder();
            desttel.Append(destTel);
            content.Append(smsContent);

            eRetCode = SmsApiUtl.SmsApi_SendMsg(desttel, content);

            ulong ulMsgID = 0;

            /// 获取提交短信后生成的编号
            SmsApiUtl.SmsApi_GetMsgID(ref ulMsgID);

            return eRetCode;
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
        public static extern ERetCode SmsApi_GetCommCount(ref short pVal);

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
        public static extern ERetCode SmsApi_GetComm(short sIndex, ref short pVal);
        /**
         * @brief 设置工作模式，在初始化模块前设置对初始化模块有效，在发送短信前设置对发送短信有效
         *
         *
         * @param bSync true表示同步方式，否则异步方式
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetWorkMode)(bool bSync);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_SetWorkMode(bool bSync);
        /**
         * @brief 获取当前工作模式，未初始化模块也可以调用
         *
         *
         * @param pSync true表示同步方式，否则异步方式
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetWorkMode)(OUT bool *pSync);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_GetWorkMode(ref bool pSync);
        /**
         * @brief 设置发送短信的起始序号，未初始化模块也可以调用，为保证短信序号唯一，最好不断增加此序号大小
         *
         *
         * @param ulMsgID 短信序号
         * @return 
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetMsgID)(IN ULONG ulMsgID);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_SetMsgID(ulong ulMsgID);
        /**
         * @brief 获得当前提交发送短信的序号，未初始化模块也可以调用
         *
         *
         * @param pMsgID 短信序号，提交发送短信后调用此方法可获得提交生成的ID，这个ID会在发送结果通知中作为唯一此短信的标识
         * @return 
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetMsgID)(OUT ULONG *pMsgID);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_GetMsgID(ref  ulong pMsgID);
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
        public static extern ERetCode SmsApi_OpenCom(short sCommPort, long lCommPara);
        /**
         * @brief 获取通讯波特率，未初始化模块也可以调用
         *
         *
         * @param pCommPara 返回通讯波特率
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetCommPara)(OUT long *pCommPara);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_GetCommPara(ref long pCommPara);
        /**
         * @brief 设置通讯波特率，未初始化模块也可以调用
         *
         *
         * @param lCommPara 通讯波特率，一般是9600或115200
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetCommPara)(IN long lCommPara);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_SetCommPara(long lCommPara);
        /**
         * @brief 获得设备名称，必须初始化成功后才能调用
         *
         *
         * @param szMobileName 输入参数，预先至少分配16长度的缓存
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetMobileName)(OUT char *szMobileName);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_GetMobileName(StringBuilder szMobileName);
        /**
         * @brief 获得模块型号，必须初始化成功后才能调用
         *
         *
         * @param szMobileModel 输入参数，预先至少分配32长度的缓存
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetMobileModel)(OUT char *szMobileModel);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_GetMobileModel(StringBuilder szMobileModel);
        /**
         * @brief 获得模块版本，必须初始化成功后才能调用
         *
         *
         * @param szMobileVersion 输入参数，预先至少分配32长度的缓存
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetMobileVersion)(OUT char *szMobileVersion);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_GetMobileVersion(StringBuilder szMobileVersion);
        /**
         * @brief 获得模块IMEI，必须初始化成功后才能调用
         *
         *
         * @param szMobileIMEI 输入参数，预先至少分配32长度的缓存
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetMobileIMEI)(OUT char *szMobileIMEI);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_GetMobileIMEI(StringBuilder szMobileIMEI);
        /**
         * @brief 获得短信中心号码，CDMA无效，必须初始化成功后才能调用
         *
         *
         * @param szSMCA 返回短信中心号码，是否加国际代码如86与发送短信规则一致
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetSmsc)(OUT char *szSMCA);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_GetSmsc(StringBuilder szSMCA);
        /**
         * @brief 设置短信中心号码，CDMA无效，必须初始化成功后才能调用，一般只需要设置一次
         *
         *
         * @param szSMCA 短信中心号码，是否加国际代码如86与发送短信规则一致
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetSmsc)(IN char *szSMCA);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_SetSmsc(StringBuilder szSMCA);
        /**
         * @brief 获取AT指令执行超时时间，未初始化模块也可以调用
         *
         *
         * @param pTimeout 返回超时时间，以秒为单位
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetTimeout)(OUT short *pTimeout);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_GetTimeout(ref short pTimeout);
        /**
         * @brief 设置AT指令执行超时时间，未初始化模块也可以调用
         *
         *
         * @param sTimeout 超时时间，以秒为单位，缺省15秒
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetTimeout)(IN short sTimeout);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_SetTimeout(short sTimeout);
        /**
         * @brief 设置接收事件通知消息的窗口句柄，未初始化模块也可以调用
         *
         *
         * @param hWnd 窗口句柄，如果不设置，无法接收相应的事件通知
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetEventWnd)(IN HWND hWnd);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_SetEventWnd(IntPtr hWnd);
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
        public static extern ERetCode SmsApi_SendMsg(StringBuilder szMobileNumber, StringBuilder szMsgContent);

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
        public static extern ERetCode SmsApi_SendPushMsg(StringBuilder szMobileNumber, StringBuilder szMsgSubject, StringBuilder szMsgUrl);
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
        public static extern ERetCode SmsApi_ReadMsg(short sPosition, StringBuilder szMobileNumber, StringBuilder szTime, StringBuilder szMsgContent);
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
        public static extern ERetCode SmsApi_DeleteMsg(short sStartPosition, short sEndPosition);
        /**
         * @brief 获取读取短信后是否自动删除短信标志，未初始化模块也可以调用
         *
         *
         * @param pAutoDelete 返回是否自动删除
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetAutoDelete)(OUT bool *pAutoDelete);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_GetAutoDelete(ref bool pAutoDelete);
        /**
         * @brief 设置读取短信后是否自动删除短信，未初始化模块也可以调用
         *
         *
         * @param bAutoDeleteFlag 是否自动删除
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetAutoDelete)(IN bool bAutoDeleteFlag);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_SetAutoDelete(bool bAutoDeleteFlag);
        /**
         * @brief 清理SIM卡上所有短信，必须初始化成功后才能调用
         *
         *
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_ClearSim)();
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_ClearSim();
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
        public static extern ERetCode SmsApi_GetSimNum(ref short pUsed, ref short pTotal);
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
        public static extern ERetCode SmsApi_ATCommand(StringBuilder ATCmd, ref short pBuffLength);
        /**
         * @brief 获得AT命令执行结果，必须初始化成功后才能调用
         *
         *
         * @param pRetBuff 返回预先根据执行结果返回的长度分配缓存内容
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetATCommandReturn)(OUT char *pRetBuff);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_GetATCommandReturn(StringBuilder pRetBuff);
        /**
         * @brief 获取发送短信状态报告标记，未初始化模块也可以调用
         *
         *
         * @param pStatusReport 返回是否请求状态报告
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetStatusReport)(OUT bool *pStatusReport);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_GetStatusReport(ref bool pStatusReport);
        /**
         * @brief 设置读取短信后是否自动删除短信，未初始化模块也可以调用，此状态对后续短信都起作用
         *
         *
         * @param bStatusReport 是否请求状态报告，缺省否
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetStatusReport)(IN bool bStatusReport);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_SetStatusReport(bool bStatusReport);
        /**
         * @brief 获取是否发送闪烁文字短信，未初始化模块也可以调用
         *
         *
         * @param pFlashSms 返回是否请求状态报告
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetFlashSms)(OUT bool *pFlashSms);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_GetFlashSms(ref bool pFlashSms);
        /**
         * @brief 设置是否发送闪烁文字短信，未初始化模块也可以调用，此状态对后续短信都起作用
         *
         *
         * @param bFlashSms 是否请求状态报告，缺省否
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetFlashSms)(IN bool bFlashSms);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_SetFlashSms(bool bFlashSms);
        /**
         * @brief 获得发送短信时的签名，未初始化模块也可以调用
         *
         *
         * @param szSignName 返回自动在发送内容前添加的签名，预先至少分配32长度的缓存
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetSignName)(OUT char *szSignName);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_GetSignName(StringBuilder szSignName);
        /**
         * @brief 设置发送短信时的签名，未初始化模块也可以调用，此状态对后续短信都起作用
         *
         *
         * @param szSignName 自动在发送内容前添加签名
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetSignName)(IN char *szSignName);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_SetSignName(StringBuilder szSignName);
        /**
         * @brief 获取发送短信的有效期，未初始化模块也可以调用
         *
         *
         * @param pValidMinute 返回发送短信的有效期
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetMsgValidMinute)(OUT long *pValidMinute);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_GetMsgValidMinute(ref long pValidMinute);
        /**
         * @brief 设置发送短信的有效期，未初始化模块也可以调用
         *
         *
         * @param lValidMinute 发送短信的有效期
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetMsgValidMinute)(IN long lValidMinute);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_SetMsgValidMinute(long lValidMinute);
        /**
         * @brief 获取发送开始的时间，未初始化模块也可以调用
         *
         *
         * @param pSendStartTime 返回发送开始的时间
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetSendStartTime)(OUT DATE *pSendStartTime);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_GetSendStartTime(ref System.Double pSendStartTime);
        /**
         * @brief 设置发送开始的时间，未初始化模块也可以调用
         *
         *
         * @param sSendStartTime 发送开始的时间，缺省是0.0或与发送结束时间相同表示不限时间发送
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetSendStartTime)(IN DATE sSendStartTime);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_SetSendStartTime(System.Double sSendStartTime);
        /**
         * @brief 获取发送结束的时间，未初始化模块也可以调用
         *
         *
         * @param pSendEndTime 返回发送结束的时间
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetSendEndTime)(OUT DATE *pSendEndTime);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_GetSendEndTime(ref System.Double pSendEndTime);
        /**
         * @brief 设置发送结束的时间，未初始化模块也可以调用
         *
         *
         * @param sSendEndTime 发送结束的时间，缺省是0.0或与发送开始时间相同表示不限时间发送
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetSendEndTime)(IN DATE sSendEndTime);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_SetSendEndTime(System.Double sSendEndTime);
        /**
         * @brief 获得SIM卡序号，未初始化模块也可以调用
         *
         *
         * @param szSimCardID 返回SIM卡序号，不是手机号码，预先至少分配32长度的缓存
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetSimCardID)(OUT char *szSimCardID);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_GetSimCardID(StringBuilder szSimCardID);
        /**
         * @brief 获取发送的短信是否自动拆分发送，未初始化模块也可以调用
         *
         *
         * @param pAutoSplite 返回是否自动拆分发送
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetAutoSplite)(OUT bool *pAutoSplite);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_GetAutoSplite(ref bool pAutoSplite);
        /**
         * @brief 设置发送的短信是否自动拆分发送，未初始化模块也可以调用，此状态对后续短信都起作用
         *
         *
         * @param bAutoSplite 是否自动拆分发送
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetAutoSplite)(IN bool bAutoSplite);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_SetAutoSplite(bool bAutoSplite);
        /**
         * @brief 获得国别代码，未初始化模块也可以调用
         *
         *
         * @param szCountryCode 返回国别代码，是否加国际代码如86与发送短信规则一致
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetCountryCode)(OUT char *szCountryCode);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_GetCountryCode(StringBuilder szCountryCode);
        /**
         * @brief 设置国别代码，未初始化模块也可以调用，一般只需要设置一次，此状态对后续短信都起作用
         *
         *
         * @param szCountryCode 国别代码，缺省86代表中国
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetCountryCode)(IN char *szCountryCode);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_SetCountryCode(StringBuilder szCountryCode);
        /**
         * @brief 获取当前将要提交发送短信的优先级，未初始化模块也可以调用
         *
         *
         * @param pSendPriority 返回当前将要提交发送短信的优先级
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetSendPriority)(OUT short *pSendPriority);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_GetSendPriority(ref short pSendPriority);
        /**
         * @brief 设置当前将要提交发送短信的优先级，未初始化模块也可以调用，此状态对后续短信都起作用
         *
         *
         * @param sSendPriority 当前将要提交发送短信的优先级，1-32，数值越大，优先发送
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetSendPriority)(IN short sSendPriority);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_SetSendPriority(short sSendPriority);
        /**
         * @brief 获取将要发送英文短信，未初始化模块也可以调用
         *
         *
         * @param pEnglishMsg 返回将要发送英文短信
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetEnglishMsg)(OUT bool *pEnglishMsg);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_GetEnglishMsg(ref  bool pEnglishMsg);
        /**
         * @brief 设置将要发送英文短信，未初始化模块也可以调用，此状态对后续短信都起作用
         *
         *
         * @param bEnglishMsg 是否将要发送英文短信
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetEnglishMsg)(IN bool bEnglishMsg);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_SetEnglishMsg(bool bEnglishMsg);
        /**
         * @brief 获取设备当前信号强度，必须初始化成功后才能调用
         *
         *
         * @param pSignal 返回设备当前信号强度，小于15可能导致发送短信失败
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetSignal)(OUT short *pSignal);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_GetSignal(ref short pSignal);
        /**
         * @brief 获得呼叫转移号码，必须初始化成功后才能调用
         *
         *
         * @param szCallTransfer 返回国别代码，是否加国际代码如86与发送短信规则一致
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetCallTransfer)(OUT char *szCallTransfer);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_GetCallTransfer(StringBuilder szCallTransfer);
        /**
         * @brief 设置呼叫转移号码，必须初始化成功后才能调用，一般只需要设置一次，如果号码为空，表示取消当前所有转移号码
         *
         *
         * @param szCallTransfer 呼叫转移号码，必须SIM卡支持才能设置
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_SetCallTransfer)(IN char *szCallTransfer);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_SetCallTransfer(StringBuilder szCallTransfer);
        /**
         * @brief 获得Fireware版本，必须初始化成功后才能调用
         *
         *
         * @param szFirewareVerion 返回Fireware版本，最少分配64长度缓存
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetFirewareVerion)(OUT char *szFirewareVerion);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_GetFirewareVerion(StringBuilder szFirewareVerion);
        /**
         * @brief 获取当前排队等候发送的任务数量，必须初始化成功后才能调用
         *
         *
         * @param pWaitSend 返回当前排队等候发送的任务数量
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetWaitSend)(OUT ULONG *pWaitSend);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_GetWaitSend(ref ulong pWaitSend);
        /**
         * @brief 退出模块，当程序退出或不再需要短信服务时调用以便及时释放资源
         *
         *
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_CloseCom)();
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_CloseCom();
        /**
         * @brief 获得出错信息，当调用某个API出错时调用此方法获取出错信息
         *
         *
         * @param pErrInfo 返回内部缓冲的最近一次操作出错信息
         * @return 参照ERetCode定义
         */
        //typedef ERetCode ( WINAPIV lpSmsApi_GetErrInfo)(OUT char *pErrInfo);
        [DllImport("JindiSMSApi.dll")]
        public static extern ERetCode SmsApi_GetErrInfo(StringBuilder pErrInfo);


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

        public static string GetEventPara()
        {
            String strEventPara = "";
            RegistryKey Key;
            Key = Registry.CurrentUser;
            RegistryKey myreg = Key.OpenSubKey("Software\\JinDiSoft");

            strEventPara = myreg.GetValue("SmsEventPara").ToString();
            myreg.Close();
            return strEventPara;
        }

        public static long StrToNum(String strNumber)
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
    }
}