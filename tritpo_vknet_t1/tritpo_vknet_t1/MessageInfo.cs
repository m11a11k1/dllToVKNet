using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tritpo_vknet_t1
{
    class MessageInfo
    {
        /// <summary> Адрес иконки отправителя(50px) </summary>
        public string SenderIconAddr { get; set; }
        /// <summary> Имя+Фамилия отправителя </summary>
        public string Name { get; set; }
        /// <summary> true если сообщение прочтено, false в обратном случае </summary>
        public bool Unread { get; set; }
        /// <summary> Текст сообщения </summary>
        public string Body { get; set; }
        /// <summary> Дата и время отправки сообщения </summary>
        public DateTime DateTime { get; set; }
        /// <summary> Пересланное сообщение </summary>
        public List<MessageInfo> FwdMessage { get; set; }
        /// <summary> Id сообщения </summary>
        public ulong Id { get; set; } 
    }
}
