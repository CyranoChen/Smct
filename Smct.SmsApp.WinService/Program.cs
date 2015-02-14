using System;
using System.Collections.Generic;
using System.Text;

using Smct.SmsApp.Entity;
using System.Threading;

namespace Smct.SmsApp.WinService
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("--- 上海明东生日短信发送开始 （" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "） ---");

                Console.WriteLine("- 读取员工信息 -");

                int _success = 0;
                int _fail = 0;
                ERetCode eRetCode;
                StringBuilder szErrorInfo = new StringBuilder("", 256);

                List<Employee> list = Entity.Employee.GetEmployees(xmlPath)
                    .FindAll(delegate(Employee em)
                    {
                        return DateTime.Now.Month.Equals(em.Birthday.Month)
                            && DateTime.Now.Day.Equals(em.Birthday.Day);
                    });

                if (list != null & list.Count > 0)
                {
                    Console.WriteLine(string.Format("今天[{0}]生日员工共{1}名", DateTime.Now.ToString("yyyy-MM-dd"), list.Count.ToString()));

                    Console.WriteLine("- 打开短信猫端口 - ");

                    eRetCode = SmsApiUtl.InitConnent();

                    if (eRetCode.Equals(ERetCode.RETCODE_OK) || eRetCode.Equals(ERetCode.RETCODE_WAITEVENT))
                    {
                        Console.WriteLine("打开端口成功 -- 等待5秒后发送");

                        Thread.Sleep(5000);
                    }
                    else
                    {
                        Console.WriteLine(eRetCode.ToString());
                    }

                    Console.WriteLine("- 发送生日短信 -");

                    foreach (Employee em in list)
                    {
                        eRetCode = SmsApiUtl.SendSms(string.Empty, em.Mobile, string.Format(wishWord, em.Name));

                        if (eRetCode.Equals(ERetCode.RETCODE_OK) || eRetCode.Equals(ERetCode.RETCODE_WAITEVENT))
                        {
                            _success++;

                            Console.WriteLine(string.Format("发送成功 - {0}", em.Name));
                        }
                        else
                        {
                            _fail++;

                            Console.WriteLine(string.Format("发送失败 - {0}, error: {1}", em.Name, eRetCode.ToString()));

                            while (eRetCode.Equals(ERetCode.RETCODE_ERROR))
                            {
                                Console.WriteLine("等待3秒后重试");

                                Thread.Sleep(3000);

                                eRetCode = SmsApiUtl.SendSms(string.Empty, em.Mobile, string.Format(wishWord, em.Name));

                                Console.WriteLine("重试结果 - {0}, result: {1}", em.Name, eRetCode.ToString());

                                if (eRetCode.Equals(ERetCode.RETCODE_ERROR))
                                {
                                    /// 获得出错信息
                                    eRetCode = SmsApiUtl.SmsApi_GetErrInfo(szErrorInfo);

                                    if (szErrorInfo != null && szErrorInfo.Length > 0)
                                    { Console.WriteLine(szErrorInfo.ToString()); }
                                }
                            }
                        }
                    }

                    Console.WriteLine(string.Format("今天生日员工共{0}人，发送成功{1}条，发送失败{2}条", list.Count.ToString(), _success.ToString(), _fail.ToString()));

                    //Console.WriteLine("- 关闭短信猫端口 - \n");

                    //eRetCode = SmsApiUtl.SmsApi_CloseCom();

                    //if (eRetCode.Equals(ERetCode.RETCODE_OK))
                    //{
                    //    Console.WriteLine("关闭端口成功 \n\n");
                    //}
                    //else
                    //{
                    //    Console.WriteLine(eRetCode.ToString());
                    //}

                    Console.WriteLine("--- 上海明东生日短信发送结束 （" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "） ---");

                }
                else
                {
                    Console.WriteLine("- 没有今天生日的员工 -");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                //Console.Read();
            }
        }

        private const string xmlPath = "employee.xml";
        private const string wishWord = "今天是明东员工{0}的生日，明东公司给您送上最真挚的祝福，祝您生日快乐、生活幸福。测试用";
    }
}
