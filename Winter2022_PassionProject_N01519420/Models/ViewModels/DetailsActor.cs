using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Winter2022_PassionProject_N01519420.Models.ViewModels
{
    public class DetailsActor
    {
        public ActorDto SelectedActor { get; set; }
        public IEnumerable<MovieDto> KeptMovies { get; set; }
    }
}