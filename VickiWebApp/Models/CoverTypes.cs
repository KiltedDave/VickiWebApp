using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VickiWebApp.Models
{
    //public class CoverTypes
    //{
    //    public List<SelectListItem> CoverType { get; } = new List<SelectListItem>
    //    {
    //        new SelectListItem { Value = "Hard", Text = "Hard Back" },
    //        new SelectListItem { Value = "Soft", Text = "SoftBack" },
    //    };
    //}
    public enum CoverEnum
    {
        [Display(Name = "Hard Back")]
        hardBack,
        [Display(Name = "Soft Back")]
        softback
    }
}
