//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CustomerLoyaltyManagementSystem
{
    using System;
    using System.Collections.Generic;
    
    public partial class TransactionLoyalty
    {
        public int TransactionLoyaltyID { get; set; }
        public Nullable<int> CustomerID { get; set; }
        public Nullable<int> ProgramID { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public decimal Amount { get; set; }
        public Nullable<int> PointsEarned { get; set; }
        public Nullable<int> PointsRedeemed { get; set; }
        public string TransactionType { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual LoyaltyProgram LoyaltyProgram { get; set; }
    }
}
