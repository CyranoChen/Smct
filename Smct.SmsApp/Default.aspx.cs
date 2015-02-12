using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Smct.SmsApp.Entity;

namespace Smct.SmsApp
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Read Xml File Path from web.config
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["EmployeeXmlPath"].ToString()))
                { throw new Exception("web.config缺少配置项[EmployeeXmlPath]"); }

                string _xmlPath = Server.MapPath(ConfigurationManager.AppSettings["EmployeeXmlPath"].ToString());

                List<Employee> list = Entity.Employee.GetEmployees(_xmlPath);

                if (list != null)
                {
                    gvEmployee.DataSource = list;
                    gvEmployee.DataBind();
                }
            }
        }
    }
}