using FinTracker.Assets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FinTracker
{
    public class Storage
    {
        private static Storage _storage;

        public List<User> Users;
        public AbstractAsset actualAsset;
        public User actualUser;
        public Transaction actualTransaction;
        public Loan actualLoan;

        [Flags]
        public enum sign
        {
            spend,
            income
        }

        [Flags]
        public enum period
        {
            День = 1,
            Неделя = 7,
            Месяц = 30,
            Год = 360
        }

        [Flags]
        public enum DateRange
        {
            Месяц,
            Полгода,
            Год
        }

        private readonly string path = @".\QQQ.txt";

        Storage()
        {
            Users = new List<User>();
        }

        public static Storage GetStorage()
        {
            if (_storage == null)
            {
                _storage = new Storage();
            }
            return _storage;
        }

        public User GetUserByName(string name)
        {
            foreach (User user in Users)
            {
                if (user.Name == name)
                {
                    return user;
                }
            }
            //throw new Exception();
            return null;
        }

        public Loan GetLoanById(int id)
        {
            Storage storage = Storage.GetStorage();
            foreach (Loan loan in storage.actualUser.Loans)
            {
                if (loan.Id == id)
                {
                    return loan;
                }
            }
            return null;
        }


        public void DeleteUser(string name)
        {
            User user = GetUserByName(name);
            Users.Remove(user);
        }

        public bool IsUniqeUser(string name)
        {
            bool uniq = true;
            foreach (User user in Users)
            {
                if (name == user.Name)
                {
                    uniq = false;
                }
            }
            return uniq;
        }

        public void Save()
        {
            string str = JsonConvert.SerializeObject(Users, Formatting.Indented, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            using (StreamWriter sw = new StreamWriter(path, false, Encoding.Default))
            {
                sw.WriteLine(str);
                sw.Close();
            }

        }

        public void GetSave()
        {
            string result = "";
            try
            {
                using (StreamReader sr = new StreamReader(path, Encoding.Default))
                {
                    result = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch (FileNotFoundException)
            {

            }
            List<User>? newClients = JsonConvert.DeserializeObject<List<User>>(result, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            if (newClients is not null)
            {
                Users = newClients;
            }
        }
    }
}
