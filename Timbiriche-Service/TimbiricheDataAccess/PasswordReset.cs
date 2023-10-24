using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess;

namespace TimbiricheDataAccess
{
    public class PasswordReset
    {
        public bool AddToken(PasswordResetTokens passwordResetToken)
        {
            int rowsAffected = 0;
            if(passwordResetToken == null)
            {
                using (var context = new TimbiricheDBEntities())
                {
                    var newPasswordResetToken = context.PasswordResetTokens.Add(passwordResetToken);
                    try
                    {
                        rowsAffected = context.SaveChanges();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        foreach (var entityValidationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in entityValidationErrors.ValidationErrors)
                            {
                                Console.WriteLine($"Entity: {entityValidationErrors.Entry.Entity.GetType().Name}, Field: {validationError.PropertyName}, Error: {validationError.ErrorMessage}");
                            }
                        }
                    }
                }   
            }
            return rowsAffected > 0;
        }
    }
}