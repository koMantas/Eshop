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
    
    public partial class Pirkejas
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Pirkejas()
        {
            this.Uzsakymas = new HashSet<Uzsakymas>();
        }
    
        public int Nr { get; set; }
        public string Vardas { get; set; }
        public string Pavarde { get; set; }
        public string Tel_NR { get; set; }
        public string El_pastas { get; set; }
        public string Miestas { get; set; }
        public string Gatve { get; set; }
        public string Namas { get; set; }
        public string Butas { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Uzsakymas> Uzsakymas { get; set; }
    }
}