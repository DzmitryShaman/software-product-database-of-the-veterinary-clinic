//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MyPro
{
    using System;
    using System.Collections.Generic;
    
    public partial class Клиенты
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Клиенты()
        {
            this.Животные = new HashSet<Животные>();
        }
    
        public int Код_клиента { get; set; }
        public string ФИО { get; set; }
        public string Адрес { get; set; }
        public string Телефон { get; set; }
        public string Email { get; set; }
        public string Примечание { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Животные> Животные { get; set; }
    }
}
