using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMDT.Models;

namespace TMDT.MauThietKe
{
    public class EmployeeSingleton
    {
        public static EmployeeSingleton instance { get; } = new EmployeeSingleton();
        public List<Employees> lsEmployee { get; } = new List<Employees>();

        public void Init(TMDTThucAnNhanhEntities context)
        {
            if(lsEmployee.Count == 0) {
                var employee = context.Employees
                    .AsEnumerable().ToList();
                foreach (var item in employee) {
                    lsEmployee.Add(item);
                }
            }
        }

        public void Update(TMDTThucAnNhanhEntities context)
        {
            lsEmployee.Clear();
            Init(context);
        }
    }
}