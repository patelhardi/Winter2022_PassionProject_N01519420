using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Winter2022_PassionProject_N01519420.Models
{
    public class Actor
    {
        //primary key
        [Key]
        public int ActorID { get; set; }

        public string ActorFirstName { get; set; }

        public string ActorLastName { get; set; }

        //actor has many movies
        public ICollection<Movie> movies { get; set; }
    }
    public class ActorDto
    {
        public int ActorID { get; set; }

        public string ActorFirstName { get; set; }

        public string ActorLastName { get; set; }
    }
}