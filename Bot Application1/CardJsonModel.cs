using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.TrelloTaskManagement
{
    public class CardJsonModel
    {
        public string id { get; set; }
        public object checkItemStates { get; set; }
        public bool closed { get; set; }
        public DateTime dateLastActivity { get; set; }
        public string desc { get; set; }
        public object descData { get; set; }
        public string idBoard { get; set; }
        public string idList { get; set; }
        public object[] idMembersVoted { get; set; }
        public int idShort { get; set; }
        public object idAttachmentCover { get; set; }
        public bool manualCoverAttachment { get; set; }
        public object[] idLabels { get; set; }
        public string name { get; set; }
        public int pos { get; set; }
        public string shortLink { get; set; }
        public Badges badges { get; set; }
        public object due { get; set; }
        public object[] idChecklists { get; set; }
        public object[] idMembers { get; set; }
        public object[] labels { get; set; }
        public string shortUrl { get; set; }
        public bool subscribed { get; set; }
        public string url { get; set; }
    }

    public class Badges
    {
        public int votes { get; set; }
        public bool viewingMemberVoted { get; set; }
        public bool subscribed { get; set; }
        public string fogbugz { get; set; }
        public int checkItems { get; set; }
        public int checkItemsChecked { get; set; }
        public int comments { get; set; }
        public int attachments { get; set; }
        public bool description { get; set; }
        public object due { get; set; }
    }
}

