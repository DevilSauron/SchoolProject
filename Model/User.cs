using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace SkolniProjekt.Model
{
    class User
    {
        public string Username { get; set; }
        public string PasswordHash { get; private set; }
        public bool Admin { get; private set; }

        public User(string username, string passwordHash, bool admin = false)
        {
            Username = username;
            PasswordHash = passwordHash;
            Admin = admin;
        }

        public void ChangePassword(string password)
        {
            PasswordHash = HashHelper.Hash(password);
        }
    }
}
