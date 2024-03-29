﻿namespace EShop.RazorPage.Models.Roles;
public class RoleDto : BaseDto
{
    public string Title { get; set; }
    public List<Permission> Permissions { get; set; }
}
public enum Permission
{
    PanelAdmin,
    EditProfile,
    ChangePassword,

    CRUD_Banner,
    Create_Banner,
    Edit_Banner,
    Delete_Banner,
    View_Banner,

    Category_Management,
    Create_Category,
    Edit_Category,
    Delete_Category,

    CRUD_Slider,
    Create_Slider,
    Edit_Slider,
    Delete_Slider,
    View_Slider,

    User_Management,
    CRUD_User,
    Create_User,
    Edit_User,
    Delete_User,
    View_User,

    CRUD_Product,
    Create_Product,
    Edit_Product,
    Delete_Product,
    View_Product,

    AddImage_Product,
    RemoveImage_Product,

    Seller_Management,
    Seller_Panel,
    Create_Seller,
    Edit_Seller,
    Add_Inventory,
    Edit_Inventory,

    Order_Management,
    Role_Management,
    Comment_Management,

    Create_ShippingMethod,
    Edit_ShippingMethod,
    Delete_ShippingMethod,

}

