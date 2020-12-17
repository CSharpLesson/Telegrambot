using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TelegramBotForShaxrixon.Model
{
    [Table("services")]
    public class Servicess
    {

        /// <summary>
        /// 
        /// </summary>
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("price")]
        public double Price { get; set; }
    }
}
