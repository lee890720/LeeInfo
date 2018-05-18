using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeeInfo.Data
{
    [Table("App_Contact")]
    public partial class AppContact
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Mail { get; set; }
        public string Phone { get; set; }
        public string UserName { get; set; }
        public bool Dispose { get; set; }
    }
}
