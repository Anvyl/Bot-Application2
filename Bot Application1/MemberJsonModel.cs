using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.TrelloTaskManagement
{
    public class MemberJsonModel
    {
        public string id { get; set; }
        public string avatarHash { get; set; }
        public string bio { get; set; }
        public object bioData { get; set; }
        public bool confirmed { get; set; }
        public string fullName { get; set; }
        public object[] idPremOrgsAdmin { get; set; }
        public string initials { get; set; }
        public string memberType { get; set; }
        public object[] products { get; set; }
        public string status { get; set; }
        public string url { get; set; }
        public string username { get; set; }
        public string avatarSource { get; set; }
        public string email { get; set; }
        public string gravatarHash { get; set; }
        public string[] idBoards { get; set; }
        public string[] idOrganizations { get; set; }
        public string[] loginTypes { get; set; }
        public object[] oneTimeMessagesDismissed { get; set; }
        public Prefs prefs { get; set; }
        public object[] trophies { get; set; }
        public object uploadedAvatarHash { get; set; }
        public object[] premiumFeatures { get; set; }
        public object idBoardsPinned { get; set; }
    }

    public class Prefs
    {
        public bool sendSummaries { get; set; }
        public int minutesBetweenSummaries { get; set; }
        public int minutesBeforeDeadlineToNotify { get; set; }
        public bool colorBlind { get; set; }
        public string locale { get; set; }
    }
}
