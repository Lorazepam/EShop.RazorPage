﻿using EShop.RazorPage.Infrastructure.RazorUtils;
using EShop.RazorPage.Models.Categories;
using EShop.RazorPage.Services.Categories;
using EShop.RazorPage.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace EShop.RazorPage.Pages.Admin.Categories;

[BindProperties]
public class AddModel : BaseRazorPage
{
    private readonly ICategoryService _categoryService;

    public AddModel(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [Display(Name = "slug")]
    [Required(ErrorMessage = "{0} را وارد کنید")]
    public string Slug { get; set; }

    [Display(Name = "عنوان")]
    [Required(ErrorMessage = "{0} را وارد کنید")]
    public string Title { get; set; }
    public SeoDataViewModel? SeoData { get; set; }
    public void OnGet()
    {
    }
    public async Task<IActionResult> OnPost(long? parentId)
    {
        if (parentId == null)
        {
            var result = await _categoryService.CreateCategory(new CreateCategoryCommand()
            {
                Title = Title,
                Slug = Slug,
                SeoData = SeoData.MapToSeoData()
            });
            return RedirectAndShowAlert(result, RedirectToPage("Index"));

        }

        var res = await _categoryService.AddChild(new AddChildCategoryCommand()
        {
            ParentId = (long)parentId,
            Title = Title,
            SeoData = SeoData.MapToSeoData(),
            Slug = Slug
        });
        return RedirectAndShowAlert(res, RedirectToPage("Index"));

    }
}

