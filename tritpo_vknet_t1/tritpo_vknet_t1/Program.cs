using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.RequestParams;

namespace tritpo_vknet_t1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Login: ");
            string mylog = Console.ReadLine();
            Console.Write("Password: ");
            Interface a = new Interface(mylog, Console.ReadLine());
            var curdi = a.getDialogs(8,0);
            foreach (var s in curdi) Console.WriteLine(s.Name);
            var curme = a.getMessages(curdi.ElementAt(Int32.Parse(Console.ReadLine())),20,0);
            foreach (var s in curme) Console.WriteLine(s.DateTime+" "+s.Name+" : "+s.Body);
            var curtr = a.getTracks();
            foreach (var s in curtr) Console.WriteLine(s.Artist + " - " + s.Title);
            //a.SendMessage(curdi.Last(),Console.ReadLine());
        }
    }
}
            