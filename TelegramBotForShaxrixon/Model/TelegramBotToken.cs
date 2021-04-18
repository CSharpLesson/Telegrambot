using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TelegramBotForShaxrixon.Model
{
    /// <summary>
    /// 
    /// </summary>
    [Table("token")]
    public class TelegramBotToken
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
    }
}
