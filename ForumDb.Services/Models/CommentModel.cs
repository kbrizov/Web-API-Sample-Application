using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace ForumDb.Services.Models
{
    [DataContract]
    public class CommentModel
    {
        [DataMember(Name="content")]
        public string Content { get; set; }

        [DataMember(Name="commentDate")]
        public DateTime CommentDate { get; set; }
    }
}