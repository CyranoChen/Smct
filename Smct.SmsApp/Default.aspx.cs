using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Smct.SmsApp.Entity;
using SMSLib;

namespace Smct.SmsApp
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                List<Employee> list = Entity.Employee.GetEmployees(XmlPath);

                if (list != null && list.Count > 0)
                {
                    gvEmployee.DataSource = list;
                    gvEmployee.DataBind();
                }
            }
        }

        private string XmlPath
        {
            get
            {
                // Read Xml File Path from web.config
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["EmployeeXmlPath"].ToString()))
                {
                    return Server.MapPath(ConfigurationManager.AppSettings["EmployeeXmlPath"].ToString());
                }
                {
                    throw new Exception("web.config缺少配置项[EmployeeXmlPath]");
                }
            }
        }

        protected void btnSendSmsBirthday_Click(object sender, EventArgs e)
        {
            try
            {
                List<Employee> list = Entity.Employee.GetEmployees(XmlPath).FindAll(delegate(Employee em)
                {
                    return DateTime.Now.Month.Equals(em.Birthday.Month)
                        && DateTime.Now.Day.Equals(em.Birthday.Day);
                });

                if (list != null & list.Count > 0)
                {
                    if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["WishWord"].ToString()))
                    { throw new Exception("web.config缺少配置项[WishWord]"); }

                    string _wishWord = ConfigurationManager.AppSettings["WishWord"].ToString();
                    LinkedList<OutboundMessage> sendList = new LinkedList<OutboundMessage>();

                    foreach (Employee em in list)
                    {
                        sendList.AddLast(new OutboundMessage(em.Mobile,
                            string.Format(_wishWord, em.Name)));
                    }

                    SmsApiUtl.BulkSend(sendList);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterClientScriptBlock(typeof(string), "exception", string.Format("alert('{0}')", ex.Message.ToString()), true);
            }
        }
    }
}