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
    
    public partial class Preke
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Preke()
        {
            this.Uzsakymo_info = new HashSet<Uzsakymo_info>();
        }
    
        public int ID { get; set; }
        public string Pavadinimas { get; set; }
        public decimal Kaina { get; set; }
        public Nullable<int> Kategorija { get; set; }
        public string Gamintojas { get; set; }
        public short Garantija { get; set; }
        public int Liko { get; set; }
    
        public virtual Kategorija Kategorija1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Uzsakymo_info> Uzsakymo_info { get; set; }
    }
}
