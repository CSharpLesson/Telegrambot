using System.ComponentModel.DataAnnotations.Schema;

namespace TelegramBotForShaxrixon.Model
{
    [Table("client")]
    public class Client
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
        public long? ChatId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("phone")]
        public string Phone { get; set; }

    }
}
