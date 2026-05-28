using System.Collections.Generic;

namespace VelastoProductionSystem.Models
{
    public class SpsNoDocViewModel
    {
        public SpsNoDoc SpsNoDoc { get; set; } = new SpsNoDoc();
        public List<SpsItemList> ItemLists { get; set; } = new List<SpsItemList>();

        // Property pembantu untuk input multiple ItemList (saat create/edit)
        public string? ItemListInputs { get; set; }
    }
}
