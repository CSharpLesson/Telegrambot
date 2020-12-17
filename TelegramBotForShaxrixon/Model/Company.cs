using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TelegramBotForShaxrixon.Model
{
    [Table("company")]
    public class Company
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
        [Column("chat_id")]
        public long ChatId { get; set; }

    }
}
