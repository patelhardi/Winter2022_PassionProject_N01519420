using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Winter2022_PassionProject_N01519420.Models.ViewModels
{
    public class UpdateMovie
    {
        public MovieDto SelectedMovie { get; set; }
        public IEnumerable<ActorDto> KeptActors { get; set; }
    }
}