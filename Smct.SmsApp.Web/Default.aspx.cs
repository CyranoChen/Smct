using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Smct.SmsApp.Entity;

namespace Smct.SmsApp.Web
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
              
            }
            catch (Exception ex)
            {
                ClientScript.RegisterClientScriptBlock(typeof(string), "exception", string.Format("alert('{0}')", ex.Message.ToString()), true);
            }
        }
    }
}