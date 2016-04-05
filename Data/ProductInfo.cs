using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eshop.Data
{
    public class ProductInfo
    {
        public int ID { get; set; }
        public string Pavadinimas { get; set; }
        public decimal Kaina { get; set; }
        public decimal KainaSuNuolaida { get; set; }
        public int Nuolaida { get; set; }
        public string Kategorija { get; set; }
        public string Gamintojas { get; set; }
        public short Garantija { get; set; }
        public int Liko { get; set; }
    }
}
