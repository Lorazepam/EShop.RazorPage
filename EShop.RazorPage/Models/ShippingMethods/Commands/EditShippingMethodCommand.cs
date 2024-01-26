﻿using System.ComponentModel.DataAnnotations;

namespace EShop.RazorPage.Models.ShippingMethods.Commands;
public class EditShippingMethodCommand
{
    public long Id { get; set; }

    [Display(Name = "روش ارسال")]
    [Required(ErrorMessage = "{0} را وارد کنید")]
    public string Title { get; set; }

    [Display(Name = "هزینه")]
    [Required(ErrorMessage = "{0} را وارد کنید")]
    public int Cost { get; set; }
}

