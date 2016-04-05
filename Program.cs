using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Eshop.UI;
using Eshop.Data;

namespace Eshop
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbManeger = new DbManager();
            var UI = new ConsoleUI(dbManeger);
            UI.RunUI();
        }
    }
}
