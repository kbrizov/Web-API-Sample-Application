using ForumDb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace ForumDb.Services.Models
{
    [DataContract]
    public class PostModel
    {
        [DataMember(Name="content")]
        public string Content { get; set; }

        [DataMember(Name="postDate")]
        public DateTime PostDate { get; set; }

        [DataMember(Name = "rating")]
        public string Rating { get; set; }

        [DataMember(Name = "postedBy")]
        public string PostedBy { get; set; }

        public static PostModel CreateFromPostEntity(Post entity)
        {
            PostModel postModel = new PostModel()
            {
                Content = entity.Content,
                PostDate = entity.DateCreated,
                PostedBy = entity.User.Nickname,
                Rating = ((entity.Votes.Count == 0) ? 0 : entity.Votes.Select(val => val.Value).Average()) + "/5"
            };

            return postModel;
        }
    }
}