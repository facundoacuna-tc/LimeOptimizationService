﻿using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LOS.Common.Constants;


namespace LOS.Common.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public bool IsSysAdmin { get; set; }
        public DateTime? LastLogin { get; set; }
        public int? ExpiresIn { get; set; }
        public long? ExpiresOn { get; set; }
        public bool Deleted { get; set; }

        public void Login(long ts)
        {
            AccessToken = Guid.NewGuid().ToString();
            RefreshToken = Guid.NewGuid().ToString();
            ExpiresIn = Constants.Constants.ONE_DAY_IN_SECONDS;
            ExpiresOn = ts + ExpiresIn;
            LastLogin = DateTime.Now;
        }

        public void Logout()
        {
            AccessToken = null;
            RefreshToken = null;
            ExpiresIn = null;
            ExpiresOn = null;
        }
    }
}
