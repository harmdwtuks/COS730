//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DatabaseLayer.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Message
    {
        public int Id { get; set; }
        public string Message1 { get; set; }
        public System.DateTime Timestamp { get; set; }
        public int SenderId { get; set; }
        public int TeamId { get; set; }
    
        public virtual Team Team { get; set; }
        public virtual webpages_Users webpages_Users { get; set; }
    }
}