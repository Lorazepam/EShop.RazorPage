﻿using EShop.RazorPage.Infrastructure.Utils.CustomValidation.IFormFile;
using EShop.RazorPage.Models;
using EShop.RazorPage.Models.Users;
using EShop.RazorPage.Models.Users.Commands;

namespace EShop.RazorPage.Services.Users;
public class UserService : IUserService
{
    private readonly HttpClient _client;
    private const string ModuleName = "users";
    public UserService(HttpClient client)
    {
        _client = client;
    }

    public async Task<ApiResult> CreateUser(CreateUserCommand command)
    {
        var result = await _client.PostAsJsonAsync(ModuleName, command);
        return await result.Content.ReadFromJsonAsync<ApiResult>();
    }

    public async Task<ApiResult> EditUser(EditUserCommand command)
    {
        var formData = new MultipartFormDataContent();
        formData.Add(new StringContent(command.UserId.ToString()), "UserId");
        formData.Add(new StringContent(command.PhoneNumber), "PhoneNumber");
        if (command.Avatar != null && command.Avatar.IsImage())
            formData.Add(new StreamContent(command.Avatar.OpenReadStream()), "Avatar", command.Avatar.FileName);
        formData.Add(new StringContent(command.Gender.ToString()), "Gender");
        formData.Add(new StringContent(command.Name), "Name");
        formData.Add(new StringContent(command.Family), "Family");
        formData.Add(new StringContent(command.Email), "Email");

        var result = await _client.PutAsync($"{ModuleName}", formData);
        return await result.Content.ReadFromJsonAsync<ApiResult>();
    }

    public async Task<ApiResult> EditUserCurrent(EditUserCommand command)
    {
        var formData = new MultipartFormDataContent();
        formData.Add(new StringContent(command.PhoneNumber), "PhoneNumber");

        if (command.Avatar != null)
            formData.Add(new StreamContent(command.Avatar.OpenReadStream()), "Avatar", command.Avatar.FileName);

        formData.Add(new StringContent(command.Gender.ToString()), "Gender");
        formData.Add(new StringContent(command.Name), "Name");
        formData.Add(new StringContent(command.Family), "Family");
        formData.Add(new StringContent(command.Email), "Email");

        var result = await _client.PutAsync($"{ModuleName}/current", formData);
        return await result.Content.ReadFromJsonAsync<ApiResult>();
    }

    public async Task<ApiResult> ChangePassword(ChangePasswordCommand command)
    {
        var result = await _client.PutAsJsonAsync($"{ModuleName}/ChangePassword", command);
        return await result.Content.ReadFromJsonAsync<ApiResult>();
    }
    public async Task<ApiResult> SetUserRole(SetUserRoleCommand command)
    {
        var result = await _client.PostAsJsonAsync($"{ModuleName}/SetUserRole",command);
        return await result.Content.ReadFromJsonAsync<ApiResult>();
    }

    public async Task<UserDto?> GetCurrentUser()
    {
        var result = await _client.GetFromJsonAsync<ApiResult<UserDto?>>($"{ModuleName}/current");
        return result.Data;
    }

    public async Task<UserDto?> GetUserById(long userId)
    {
        var result = await _client.GetFromJsonAsync<ApiResult<UserDto?>>($"{ModuleName}/{userId}");
        return result.Data;
    }

    public async Task<UserFilterResult> GetUsersByFilter(UserFilterParams filterParams)
    {
        //var url = filterParams.GenerateBaseFilterUrl(ModuleName) +
        //          $"email={filterParams.Email}&phoneNumber={filterParams.PhoneNumber}&id={filterParams.Id}";
        var url = $"{ModuleName}?pageId={filterParams.PageId}&take={filterParams.Take}" +
             $"&email={filterParams.Email}&phoneNumber={filterParams.PhoneNumber}";
        if (filterParams.Id != null)
            url += $"&Id={filterParams.Id}";
        var result = await _client.GetFromJsonAsync<ApiResult<UserFilterResult>>(url);
        return result.Data; throw new NotImplementedException();
    }


}

