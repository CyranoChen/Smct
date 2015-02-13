using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Smct.SmsApp.Entity;

namespace Smct.SmsApp.Console
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private const string xmlPath = "employee.xml";
        private const string wishWord = "今天是明东员工{0}的生日，明东公司给您送上最真挚的祝福，祝您生日快乐、生活幸福。测试版";

        private void FormMain_Load(object sender, EventArgs e)
        {
            List<Employee> list = Employee.GetEmployees(xmlPath);

            if (list != null && list.Count > 0)
            {
                gvEmployee.DataSource = list;
            }
        }

        private void btnSendSms_Click(object sender, EventArgs e)
        {
            try
            {
                int _success = 0;
                int _fail = 0;

                List<Employee> list = Entity.Employee.GetEmployees(xmlPath)
                    .FindAll(delegate(Employee em)
                {
                    return DateTime.Now.Month.Equals(em.Birthday.Month)
                        && DateTime.Now.Day.Equals(em.Birthday.Day);
                });

                if (list != null & list.Count > 0)
                {
                    foreach (Employee em in list)
                    {
                        ERetCode eRetCode = SmsApiUtl.SendSms(string.Empty, em.Mobile, string.Format(wishWord, em.Name));

                        if (eRetCode.Equals(ERetCode.RETCODE_OK) || eRetCode.Equals(ERetCode.RETCODE_WAITEVENT))
                        { _success++; }
                        else
                        { _fail++; }
                    }

                    MessageBox.Show(string.Format("今天生日员工共{0}人，发送成功{1}条，发送失败{2}", list.Count.ToString(), _success.ToString(), _fail.ToString()),
                        "Sent", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    throw new Exception("没有今天生日的员工");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //ClientScript.RegisterClientScriptBlock(typeof(string), "exception", string.Format("alert('{0}')", ex.Message.ToString()), true);
            }
        }

        private void btnSmsConfig_Click(object sender, EventArgs e)
        {
            FormSms smsConfig = new FormSms();
            smsConfig.Show();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                ERetCode eRetCode = SmsApiUtl.InitConnent();

                if (eRetCode.Equals(ERetCode.RETCODE_OK))
                {
                    MessageBox.Show("打开端口成功");
                }
                else
                {
                    MessageBox.Show(eRetCode.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            ERetCode eRetCode = SmsApiUtl.SmsApi_CloseCom();

            if (eRetCode.Equals(ERetCode.RETCODE_OK))
            {
                MessageBox.Show("关闭端口成功");
            }
            else
            {
                MessageBox.Show(eRetCode.ToString());
            }
        }
    }
}
