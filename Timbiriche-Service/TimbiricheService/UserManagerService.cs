﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess;
using TimbiricheDataAccess.Utils;

namespace TimbiricheService
{
    public partial class UserManagerService : IUserManager
    {
        public int AddUser(Player player)
        {
            Account auxiliarAccount = player.AccountFK;

            Accounts newAccount = new Accounts();
            newAccount.name = auxiliarAccount.Name;
            newAccount.lastName = auxiliarAccount.LastName;
            newAccount.surname = auxiliarAccount.Surname;
            newAccount.birthdate = auxiliarAccount.Birthdate;

            Players newPlayer = new Players();
            newPlayer.username = player.Username;
            newPlayer.email = player.Email;
            newPlayer.password = player.Password;
            newPlayer.coins = player.Coins;
            newPlayer.status = player.Status;
            newPlayer.salt = player.Salt;
            newPlayer.Accounts = newAccount;

            UserManagement dataAccess = new UserManagement();
            return dataAccess.AddUser(newPlayer);
        }

        public Player ValidateLoginCredentials(String username, String password)
        {
            UserManagement dataAccess = new UserManagement();
            Players playerValidated = dataAccess.ValidateLoginCredentials(username, password);
            if (playerValidated != null)
            {
                Accounts accountValidated = playerValidated.Accounts;
                Account account = new Account
                {
                    Name = accountValidated.name,
                    LastName = accountValidated.lastName,
                    Surname = accountValidated.surname,
                    Birthdate = accountValidated.birthdate
                };

                Player player = new Player
                {
                    IdPlayer = playerValidated.idPlayer,
                    Username = playerValidated.username,
                    Email = playerValidated.email,
                    Password = playerValidated.password,
                    Coins = playerValidated.coins,
                    Status = playerValidated.status,
                    Salt = playerValidated.salt,
                    AccountFK = account
                };

                return player;
            }

            return null;
        }

        public bool ValidateUniqueIdentifierUser(String identifier)
        {
            UserManagement dataAccess = new UserManagement();
            return dataAccess.ExistUserIdenitifier(identifier);
        }
    }

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public partial class UserManagerService : IManagerOnlineUsers
    {
        private static Dictionary<string, IUserManagerCallback> onlineUsers = new Dictionary<string, IUserManagerCallback>();

        public void RegisterUserToOnlineUsers(string username)
        {
            if (!onlineUsers.ContainsKey(username))
            {
                IUserManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IUserManagerCallback>();

                List<string> onlineUsernames = onlineUsers.Keys.ToList();
                currentUserCallbackChannel.NotifyOnlineUsers(onlineUsernames);

                onlineUsers.Add(username, currentUserCallbackChannel);

                foreach (var user in onlineUsers)
                {
                    if (user.Key != username)
                    {
                        user.Value.NotifyUserLoggedIn(username);
                    }
                }
            }
        }

        public void UnregisterUserToOnlineUsers(string username)
        {
            if (onlineUsers.ContainsKey(username))
            {
                onlineUsers.Remove(username);

                foreach (var user in onlineUsers)
                {
                    user.Value.NotifyUserLoggedOut(username);
                }
            }
        }

    }

    public partial class UserManagerService : IEmailManager
    {
        public string sendEmail(string addressee)
        {
            // TODO: change variables to constants
            // TODO: internationalizate
            bool isSend = false;
            string code = GenerateEmailCode();
            string sender = "timbirichethegame@gmail.com";
            string displayName = "Timbiriche THE GAME";
            string subject = "Email Confirmation";
            string body = "¡Hi, Welcome to Timbiriche!\n This is a confirmation email to create your account," +
                          " please enter the following code in the game:\n" + code;
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(sender, displayName);
                mail.To.Add(addressee);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                SmtpClient client = new SmtpClient("smtp.Gmail.com", 587);
                client.Credentials = new NetworkCredential(sender, "dusb ueav ompt pckq");
                client.EnableSsl = true;

                client.Send(mail);
                isSend = true;
            }
            catch (SmtpException ex)
            {
                isSend = false;
            }
            if (isSend)
            {
                return code;
            }
            return null;
        }

        public string GenerateEmailCode()
        {
            // TODO: Generate code
            return "aaa";
        }

    }
}