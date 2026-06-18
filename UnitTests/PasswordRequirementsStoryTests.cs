namespace UnitTests;

[TestClass]
public class PasswordRequirementsStoryTests
{
    [DataTestMethod]
    [DataRow("abc")]
    [DataRow("abcdef")]
    [DataRow("password1")]
    [DataRow("Password")]
    [DataRow("123456")]
    [DataRow("password")]
    public void PasswordRequirements_ShouldRejectWeakPasswords(string password)
    {
        var userLogic = new UserLogic();

        var result = userLogic.CheckPassword(password);

        Assert.IsFalse(result);
    }

    [DataTestMethod]
    [DataRow("Password1")]
    [DataRow("Secure123")]
    [DataRow("Welkom2026")]
    [DataRow("Admin123")]
    public void PasswordRequirements_ShouldAcceptPasswords_WithMinimumRequirements(string password)
    {
        var userLogic = new UserLogic();

        var result = userLogic.CheckPassword(password);

        Assert.IsTrue(result);
    }
}
