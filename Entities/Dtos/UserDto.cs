using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using WeighForce.Models;

namespace WeighForce.Dtos
{
    public class UserNameDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
    public class UserSyncRoleDto
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; }
        [JsonPropertyName("role")]
        public string Role { get; set; }
    }
    public class UserLocationDto
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; }
        [JsonPropertyName("location")]
        public string Location { get; set; }
    }
    public class IDSyncDto
    {
        [JsonPropertyName("users")]
        public List<UserDto> Users { get; set; }
        [JsonPropertyName("userRoles")]
        public List<UserSyncRoleDto> UserRoles { get; set; }
        [JsonPropertyName("userLocations")]
        public List<UserLocationDto> UserLocations { get; set; }
    }
    public class UserRoleDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public IEnumerable<OfficeReadDto> Locations { get; set; }
    }
    public class UserAlertDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> Alerts { get; set; }
    }
    public class UserDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("normalizedEmail")]
        public string NormalizedEmail { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("emailConfirmed")]
        public bool EmailConfirmed { get; set; }
        [JsonPropertyName("userName")]
        public string UserName { get; set; }
        [JsonPropertyName("normalizedUserName")]
        public string NormalizedUserName { get; set; }
        [JsonPropertyName("passwordHash")]
        public string PasswordHash { get; set; }
        [JsonPropertyName("securityStamp")]
        public string SecurityStamp { get; set; }
        [JsonPropertyName("concurrencyStamp")]
        public string ConcurrencyStamp { get; set; }
    }
    public class UserSyncDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public IEnumerable<OfficeReadDto> Locations { get; set; }
    }
    public class PasswordDto
    {
        public string Password { get; set; }
    }
    public class RoleDto
    {
        public string Name { get; set; }
    }
    public class UsersAndRolesDto
    {
        public ICollection<string> Roles { get; set; }
        public ICollection<string> Locations { get; set; }
        public ICollection<UserRoleDto> Users { get; set; }
    }
    public class UserToggleDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string CurrentPassword { get; set; }
    }
    public class UserAlertCreateDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Alert { get; set; }
        public IDto Office { get; set; }
    }
}