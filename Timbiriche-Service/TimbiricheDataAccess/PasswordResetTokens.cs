//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TimbiricheDataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class PasswordResetTokens
    {
        public int idPasswordResetToken { get; set; }
        public Nullable<int> token { get; set; }
        public Nullable<System.DateTime> creationDate { get; set; }
        public Nullable<System.DateTime> expirationDate { get; set; }
        public Nullable<int> idPlayer { get; set; }
    
        public virtual Players Players { get; set; }
    }
}