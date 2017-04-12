using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tritpo_vknet_t1
{
    class TrackInfo
    {
        /// <summary> Исполнитель </summary>
        public string Artist { get; set; }
        /// <summary> Название </summary>
        public string Title { get; set; }
        /// <summary> URL-Адрес </summary>
        public Uri Path { get; set; }
        /// <summary> Длительность в секундах </summary>
        public int Duration { get; set; }
    }
}
