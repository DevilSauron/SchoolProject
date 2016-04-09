using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SkolniProjekt.Model;

namespace SkolniProjekt
{
    class Auth
    {
        private Database db;

        public bool LoggedIn { get; private set; } = false;
        public User CurrentUser { get; private set; }
        public bool IsAdmin => CurrentUser?.Admin ?? false;

        public Auth(Database db)
        {
            this.db = db;
        }

        public bool LogIn(string username, string password)
        {
            var hash = HashHelper.Hash(password);

            var userQueryList = (from User u in db.Users
                where u.Username == username && u.PasswordHash == hash
                select u).ToList();

            if (userQueryList.Count == 0)
                return false;

            CurrentUser = userQueryList.First();
            LoggedIn = true;
            return true;
        }

        public void LogOut()
        {
            CurrentUser = null;
            LoggedIn = false;
        }

        public bool SignUp(string username, string password, bool admin = false)
        {
            var hash = HashHelper.Hash(password);

            var userQueryList = (from User u in db.Users
                                 where u.Username == username
                                 select u).ToList();

            if (userQueryList.Count != 0)
                return false;

            User newUser = new User(username, hash, admin);
            db.Users.Add(newUser);
            db.SaveUsersDb();
            return true;
        }
    }
}
