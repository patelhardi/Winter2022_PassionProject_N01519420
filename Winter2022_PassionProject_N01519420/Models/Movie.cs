using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Winter2022_PassionProject_N01519420.Models
{
    public class Movie
    {
        [Key]
        public int MovieID { get; set; }

        public string MovieName { get; set; }

        public int ReleaseYear { get; set; }

        public bool MovieHasPic { get; set; }
        public string PicExtension { get; set; }

        //many actors in one movie
        public ICollection<Actor> actors { get; set; }
    }
    public class MovieDto
    {
        public int MovieID { get; set; }

        public string MovieName { get; set; }

        public int ReleaseYear { get; set; }
        public bool MovieHasPic { get; set; }
        public string PicExtension { get; set; }
    }
}