using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portifolio.Models.ViewModels
{
    public class ProdViewModel
    {
        public IFormFile ProfileImage { get; set; }
        public Prod Prod { get; set; }

        public List<SelectListItem> CategSelect { get; set; }
    }
}
