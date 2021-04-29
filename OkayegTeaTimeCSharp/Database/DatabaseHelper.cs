﻿using OkayegTeaTimeCSharp.Database.Models;
using OkayegTeaTimeCSharp.Utils;
using System;
using System.Linq;

namespace OkayegTeaTimeCSharp.Database
{
    public static class DatabaseHelper
    {
        public static void SetAfk(this OkayegTeaTimeContext database, User user, string state)
        {
            if (state.IsMatch(@"^(t(rue)?)|(f(alse)?)$"))
            {
                User userE = database.Users.Where(userD => userD.Username == user.Username).FirstOrDefault();
                if (state.IsMatch(@"^t(rue)?$"))
                {
                    userE.IsAfk = "true";
                }
                else
                {
                    userE.IsAfk = "false";
                }
                database.SaveChanges();
            }
            else
            {
                throw new Exception("state doesn't match the required pattern");
            }
        }

        public static void AddUser(this OkayegTeaTimeContext database, string username)
        {
            database.Users.Add(new User(username));
            database.SaveChanges();
        }
    }
}
