﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace TelegramBotForShaxrixon.Model
{
    [Table("orders")]
    public class Orders
    {
        /// <summary>
        /// 
        /// </summary>
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("chat_id")]
        public long? ChatId { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [Column("longitude")]
        public string Longitude { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("lotetude")]
        public string Lotetude { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("dateorder")]
        public DateTime? DateOrder { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("position")]
        public int? Position { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("suxoypar")]
        public double? SuxoyPar { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("count")]
        public int Count { get; set; }

        /// <summary>
        /// 
        /// </summary>        
        [Column("service_id")]
        [ForeignKey("ServiceModel")]
        public int? ServiceId { get; set; }

        [IgnoreDataMember]
        public virtual Servicess ServiceModel { get; set; }

    }
}
