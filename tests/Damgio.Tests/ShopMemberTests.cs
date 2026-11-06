using System.ComponentModel.DataAnnotations;
using Damgio.Core.Entities;
using Damgio.Core.Enums;
using Damgio.Web.Models.Admin;
using Xunit;

namespace Damgio.Tests;

public class ShopMemberTests
{
    private static IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var ctx = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, ctx, validationResults, true);
        return validationResults;
    }

    [Fact]
    public void ShopMemberInputModel_ValidData_PassesValidation()
    {
        var model = new ShopMemberInputModel
        {
            ShopId = 1,
            UserEmail = "kasiyer@damgio.com",
            Role = ShopRole.Cashier
        };

        var results = ValidateModel(model);
        Assert.Empty(results);
    }

    [Theory]
    [InlineData("", "Personel e-posta adresi zorunludur.")]
    [InlineData("gecersiz-email", "Geçerli bir e-posta adresi giriniz.")]
    public void ShopMemberInputModel_InvalidEmail_FailsValidation(string email, string expectedErrorMessage)
    {
        var model = new ShopMemberInputModel
        {
            ShopId = 1,
            UserEmail = email,
            Role = ShopRole.Cashier
        };

        var results = ValidateModel(model);
        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.ErrorMessage == expectedErrorMessage);
    }

    [Fact]
    public void ShopMember_Entity_StoresPropertiesCorrectly()
    {
        var member = new ShopMember
        {
            ShopId = 42,
            UserId = "user-guid-999",
            Role = ShopRole.Owner,
            CreatedAt = DateTime.UtcNow
        };

        Assert.Equal(42, member.ShopId);
        Assert.Equal("user-guid-999", member.UserId);
        Assert.Equal(ShopRole.Owner, member.Role);
    }
}
