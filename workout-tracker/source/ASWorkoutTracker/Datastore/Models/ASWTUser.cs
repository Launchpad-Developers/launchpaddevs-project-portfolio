using System;
using System.Collections.Generic;
using ASCommonServices.Interfaces;
using ASCommonServices.Models;
using SQLite;
using Xamarin.Forms;

namespace ASWorkoutTracker.Datastore.Models
{
    public class ASWTUser : User, IUser
    {
        public ASWTUser()
        {
            GoldMedals = new List<UserMedal>();
            SilverMedals = new List<UserMedal>();
            BronzeMedals = new List<UserMedal>();
        }

        public byte[] ProfileData { get; set; }

        [Ignore]
        public List<UserMedal> GoldMedals { get; set; }
        [Ignore]
        public List<UserMedal> SilverMedals { get; set; }
        [Ignore]
        public List<UserMedal> BronzeMedals { get; set; }
        [Ignore]
        public int GoldCount { get { return GoldMedals.Count; } }
        [Ignore]
        public int SilverCount { get { return SilverMedals.Count; } }
        [Ignore]
        public int BronzeCount { get { return BronzeMedals.Count; } }
        [Ignore]
        public Image ProfilePicture { get; set; }
    }
}
