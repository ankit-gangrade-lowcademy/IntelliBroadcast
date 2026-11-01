using System.ComponentModel.DataAnnotations;
namespace IntelliBroadcast.Web.ViewModels;
public sealed class LoginViewModel
{
    [Required, EmailAddress, StringLength(320)] public string Email { get; set; } = string.Empty;
    [Required, DataType(DataType.Password), StringLength(256)] public string Password { get; set; } = string.Empty;
    public string? ReturnUrl { get; set; }
}
