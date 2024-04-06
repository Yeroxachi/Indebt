namespace Tests.Helpers;

public static class TestDataConstants
{
    public const string TestEntity1Id = "00000000-0000-0000-0000-000000000001";
    public const string TestEntity2Id = "00000000-0000-0000-0000-000000000002";
    public const string TestEntity3Id = "00000000-0000-0000-0000-000000000003";
    public const string TestEntity4Id = "00000000-0000-0000-0000-000000000004";
    public const string TestEntity5Id = "00000000-0000-0000-0000-000000000005";
    public const string IncorrectTestEntity1Id = "00000000-0000-0000-0000-000000000006";
    public const string IncorrectTestEntity2Id = "00000000-0000-0000-0000-000000000007";

    public const string LoggedUsername = "username";
    public const string InviterUsername = "inviter";
    public const string InvitedUsername = "invited";
    public const string IncorrectEntityUsername = "";
    public const string NotConfirmedUsername = "NotConfirmed";

    public const string CreatedDate = "2023-03-24";
    public const string EndDate = "2023-04-24";
    public const string ReminderDate = "2023-04-20";
    public const string IncorrectDate = "1000-01-01";

    public const string CurrencyCodeUsd = "USD";
    public const string CurrencyCodeRub = "RUB";
    public const string CurrencyCodeKzt = "KZT";
    public const int ConversionRate = 1;
        
    public const string CorrectConfirmationCode =
        "whmq4WtLgm85zIkeoh9ivzFXn8pTRiY40IZ7lBCZPITr7DNIhafYR1yuDgDlc7l3U8tr88UJJdzGAV4jEoSk6AvxvG1UA5G2V6lToPwbfMrWdEYPcCiqEKR4z03mFTYrJAxE0w4dZIGgls1eJrWnzezHv2PUS81oQgRn4oSEIXoH7ynaG3L3wERDaXwPFjCLDwmqA138iBcxYsk1ATnrlD9lCldlh7936FYc6A7gEykwihSFEymqbVrdedfm46E";
    public const string IncorrectConfirmationCode =
        "whmq4WtLgm85zIkeoh9ivzFXn8pTRiY40IZ7lBCZPITr7DNIhafYR1yuDgDlc7l3U8tr88UJJdzGAV4jEoSk6AvxvG1UA5G2V6lToPwbfMrWdEYPcCiqEKR4z03mFTYrJAxE0w4dZIGgls1eJrWnzezHv2PUS81oQgRn4oSEIXoH7ynaG3L3wERDaXwPFjCLDwmqA138iBcxYsk1ATnrlD9lCldlh7936FYc6A7gEykwihSFEymqbVrdedfm464";

    public const string DefaultEmail = "email@test.com";
    public const string NotConfirmedEmail = "email3@test.com";
    public const string IncorrectEmail = "email65@test.com";
    //Auth Configs
    public const string AuthAudience = "TestAudience";
    public const string AuthIssuer = "TestIssuer";
    public const string AuthKey = "Ayaquekmfdlkfnbldmblkdnbkdnlbndflbnrinldnblfnbldnbdlfjnbdlmbndlfnbdlfbnmcnbldfjknbldfnblfdnjlbndfn";

    public const int TestDebtAmount = 100;
    public const int TestTransactionAmount = 50;
    public const int IncorrectTransactionAmount = 500;
    //EmailServiceConfigs
    public const string Host = "smtp.yandex.com";
    public const int Port = 465;
    public const string SenderName = "Administration InDebt";
    public const string SenderAddress = "indebttest@yandex.ru";
    public const string Password = "da24b366";
    public const bool SslState = true;
}