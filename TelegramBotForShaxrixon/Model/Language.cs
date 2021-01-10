using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TelegramBotForShaxrixon.Model
{
    [Table("language")]
    public class Language
    {
        /// <summary>
        /// 
        /// </summary>
        [Column("id")]
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("chatid")]
        public long ChatId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("language_id")]
        public int LanguageId { get; set; }
    }
}
