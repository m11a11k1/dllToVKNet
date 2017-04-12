using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tritpo_vknet_t1
{
    class DialogInfo
    {
        /// <summary> Адрес иконки беседы(50px) </summary>
        public string DialogIconAddr { get; set; }
        /// <summary> Имя отпрвителя / название беседы </summary>
        public string Name { get; set; }
        /// <summary> Количесто непрочтенных сообщений </summary>
        public int Unread { get; set; }
        /// <summary> True если беседа, false если диалог </summary>
        public bool IsChat { get; set; }
        /// <summary> ID беседы/диалога(служебное поле) </summary>
        public long Id { get; set; }
        /// <summary> Текст последнего полученного сообщения </summary>
        public string LastReceivedMessage { get; set; }
    }
}
