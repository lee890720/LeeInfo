using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeeInfo.Data.Forex
{
    [Table("Frx_Ecs")]
    public partial class FrxEcs
    {
        [Key]
        public int Id { get; set; }

        public string EcsName { get; set; }
        public DateTime EcsTime { get; set; }
    }
}
