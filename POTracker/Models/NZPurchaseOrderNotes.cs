//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace POTracker.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class NZPurchaseOrderNotes
    {
        public int PONotesId { get; set; }
        public int po_order_no { get; set; }
        public string po_backorder_flag { get; set; }
        public double po_l_seq { get; set; }
        public string po_note_type { get; set; }
        public double po_note_seq { get; set; }
        public string po_note_text { get; set; }
        public bool Archived { get; set; }
    }
}
