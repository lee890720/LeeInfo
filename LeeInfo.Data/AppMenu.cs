using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeeInfo.Data
{
    [Table("App_Menu")]
    public partial class AppMenu
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Grade { get; set; }
        public int Sequence { get; set; }
        public int Follow { get; set; }
        public string Ico { get; set; }
        public string Url { get; set; }
        public string Area { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public bool Valid { get; set; }
        public string Description { get; set; }
        public string State { get; set; }
    }
}
