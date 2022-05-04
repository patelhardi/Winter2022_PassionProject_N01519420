using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Winter2022_PassionProject_N01519420.Models
{
    public class Actor
    {
        //primary key
        [Key]
        public int ActorID { get; set; }
        [Required]
        public string ActorFirstName { get; set; }
        [Required]
        public string ActorLastName { get; set; }
        [AllowHtml]
        public string Bio { get; set; }
        public bool ActorHasPic { get; set; }
        public string PicExtension { get; set; }

        //actor has many movies
        public ICollection<Movie> movies { get; set; }
    }
    public class ActorDto
    {
        public int ActorID { get; set; }
        [Required(ErrorMessage = "Please Enter a First Name.")]
        public string ActorFirstName { get; set; }
        [Required(ErrorMessage = "Please Enter a Last Name.")]
        public string ActorLastName { get; set; }
        [AllowHtml]
        public string Bio { get; set; }
        public bool ActorHasPic { get; set; }
        public string PicExtension { get; set; }
    }
}