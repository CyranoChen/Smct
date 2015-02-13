using System;
using System.Collections.Generic;
using System.Xml;

namespace Smct.SmsApp.Entity
{
    public class Employee
    {
        public Employee() { }

        public static List<Employee> GetEmployees(string xmlPath)
        {
            List<Employee> list = new List<Employee>();

            XmlDocument xml = new XmlDocument();
            xml.Load(xmlPath);

            foreach (XmlElement xe in xml.SelectSingleNode("root").ChildNodes)
            {
                Employee em = new Employee();

                em.EmployeeID = Convert.ToInt16(xe.Attributes["id"].Value.Trim());
                em.Name = xe.Attributes["name"].Value.Trim();
                em.Mobile = xe.Attributes["mobile"].Value.Trim();
                em.Birthday = Convert.ToDateTime(xe.Attributes["birthday"].Value.Trim());
                em.Remark = xe.Attributes["remark"].Value.Trim();

                list.Add(em);
            }

            if (list.Count > 0)
            { return list; }
            else
            { return null; }
        }

        #region Members and Properties

        public int EmployeeID
        { get; set; }

        public string Name
        { get; set; }

        public string Mobile
        { get; set; }

        public DateTime Birthday
        { get; set; }

        public string Remark
        { get; set; }

        #endregion
    }
}