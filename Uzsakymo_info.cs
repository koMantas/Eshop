//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Eshop
{
    using System;
    using System.Collections.Generic;
    
    public partial class Uzsakymo_info
    {
        public int Uzsakymas_ID { get; set; }
        public int Preke_ID { get; set; }
        public int Kiekis { get; set; }
        public decimal Kaina { get; set; }
    
        public virtual Preke Preke { get; set; }
        public virtual Uzsakymas Uzsakymas { get; set; }
    }
}
