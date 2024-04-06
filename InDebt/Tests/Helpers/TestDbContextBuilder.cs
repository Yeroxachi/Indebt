using Application.Helpers;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Tests.Helpers;

public class TestDbContextBuilder
{
    private readonly InDebtContext _context;
    public TestDbContextBuilder(string name)
    {
        _context = CreateContext(name);
    }

    public TestDbContextBuilder WithUsers()
    {
        var users = GenerateUsers();
        _context.Users.AddRange(users);
        _context.SaveChanges();
        return this;
    }

    public TestDbContextBuilder WithGroups()
    {
        var user = _context.Users.FirstOrDefault(x => x.Username == TestDataConstants.LoggedUsername);
        ArgumentNullException.ThrowIfNull(user);
        var groups = GenerateGroups();
        user.Groups.Add(new UserGroup
            {
                Group = groups.FirstOrDefault()
            }
        );

        _context.Groups.AddRange(groups);
        _context.SaveChanges();

        return this;
    }
        
    public TestDbContextBuilder WithGroupsForMerge()
    {
        var user = _context.Users.FirstOrDefault(x => x.Username == TestDataConstants.LoggedUsername);
        ArgumentNullException.ThrowIfNull(user);
        var groups = GenerateGroups();
        foreach (var group in groups)
        {
            user.Groups.Add(new UserGroup
            {
                Group = group
            });
        }
        _context.Groups.AddRange(groups);
        _context.SaveChanges();
        return this;
    }

    public TestDbContextBuilder WithMerge()
    {
        var firstCaseInitiator = _context.Users.FirstOrDefault(x => x.Id == Guid.Parse(TestDataConstants.TestEntity2Id));
        ArgumentNullException.ThrowIfNull(firstCaseInitiator);
        var secondCaseInitiator = _context.Users.FirstOrDefault(x => x.Id == Guid.Parse(TestDataConstants.TestEntity1Id));
        ArgumentNullException.ThrowIfNull(secondCaseInitiator);
        var group = _context.Groups.FirstOrDefault(x => x.Id == Guid.Parse(TestDataConstants.TestEntity1Id));
        ArgumentNullException.ThrowIfNull(group);
        var otherGroup = _context.Groups.FirstOrDefault(x => x.Id == Guid.Parse(TestDataConstants.TestEntity2Id));
        ArgumentNullException.ThrowIfNull(otherGroup);
        var firstCase = new MergeRequest
        {
            Id = Guid.Parse(TestDataConstants.TestEntity1Id),
            Initiator = firstCaseInitiator,
        };
        var secondCase = new MergeRequest
        {
            Id = Guid.Parse(TestDataConstants.TestEntity2Id),
            Initiator = secondCaseInitiator,
        };
        firstCaseInitiator.Merges.Add(firstCase);
        secondCaseInitiator.Merges.Add(secondCase);

        _context.Merges.AddRange(firstCase,secondCase);
        _context.SaveChanges();
        return this;
    }
        
    public TestDbContextBuilder WithMergeConfirmations()
    {
        var user = _context.Users.FirstOrDefault(x => x.Id == Guid.Parse(TestDataConstants.TestEntity1Id));
        ArgumentNullException.ThrowIfNull(user);
        var merge = _context.Merges.FirstOrDefault(x => x.Id == Guid.Parse(TestDataConstants.TestEntity1Id));
        ArgumentNullException.ThrowIfNull(merge);
        var newConfirmation = new MergeRequestApproval()
        {
            Id = Guid.Parse(TestDataConstants.TestEntity1Id),
            User = user,
            MergeRequest = merge,
            Approved = false
        };
            
        user.Approvals.Add(newConfirmation);
        _context.Approvals.Add(newConfirmation);
        _context.SaveChanges();
        return this;
    }
        
    public TestDbContextBuilder WithConfirmationCodes()
    {
        var user = _context.Users.FirstOrDefault(x => x.Id == Guid.Parse(TestDataConstants.TestEntity1Id));
        var listOfConfirmations = new List<ConfirmationCode>
        {
            new ConfirmationCode
            {
                Id= Guid.Parse(TestDataConstants.TestEntity1Id),
                Code = TestDataConstants.CorrectConfirmationCode,
                Type = ConfirmationType.EmailConfirmation,
                UserId = Guid.Parse(TestDataConstants.TestEntity1Id),
                User = user
            },
            new ConfirmationCode
            {
                Id= Guid.Parse(TestDataConstants.TestEntity2Id),
                Code = TestDataConstants.CorrectConfirmationCode,
                Type = ConfirmationType.ResetPasswordConfirmation,
                UserId = Guid.Parse(TestDataConstants.TestEntity1Id),
                User = user
            },
            new ConfirmationCode
            {
                Id= Guid.Parse(TestDataConstants.TestEntity3Id),
                Code = TestDataConstants.CorrectConfirmationCode,
                Type = ConfirmationType.EmailConfirmation,
                UserId = Guid.Parse(TestDataConstants.TestEntity1Id),
                User = user
            }
        };
        foreach (var variableConfirmation in listOfConfirmations)
        {
            user.ConfirmationCodes.Add(variableConfirmation);
        }
        _context.ConfirmationCodes.AddRange(listOfConfirmations);
        _context.SaveChanges();
        return this;
    }
        
    public TestDbContextBuilder WithGroupInvites()
    {
        var inviterUser = _context.Users.FirstOrDefault(x => x.Id == Guid.Parse(TestDataConstants.TestEntity1Id));
        ArgumentNullException.ThrowIfNull(inviterUser);
        var invitedUser = _context.Users.FirstOrDefault(x => x.Id == Guid.Parse(TestDataConstants.TestEntity2Id));
        ArgumentNullException.ThrowIfNull(invitedUser);
        var group = _context.Groups.FirstOrDefault(x => x.Id == Guid.Parse(TestDataConstants.TestEntity1Id));
        ArgumentNullException.ThrowIfNull(group);

        var inviteForInviter = new GroupInvite
        {
            GroupId = group.Id,
            InviterId = inviterUser.Id,
            InvitedId = invitedUser.Id,
            InviteStatus = InviteStatus.Invited,
            Id = Guid.Parse(TestDataConstants.TestEntity1Id)
        };

        inviterUser.Groups.Add(new UserGroup
        {
            Group = group
        });
        _context.GroupInvites.Add(inviteForInviter);
        _context.SaveChanges();

        return this;
    }

    public TestDbContextBuilder WithCurrencies()
    {
        var currencies = GenerateCurrencies();
        _context.Currencies.AddRange(currencies);
        _context.SaveChanges();
        return this;
    }

    public TestDbContextBuilder WithDebts()
    {
        var lender = _context.Users.FirstOrDefault(x => x.Id == Guid.Parse(TestDataConstants.TestEntity1Id));
        ArgumentNullException.ThrowIfNull(lender);
        var borrower = _context.Users.FirstOrDefault(x => x.Id == Guid.Parse(TestDataConstants.TestEntity2Id));
        ArgumentNullException.ThrowIfNull(borrower);
        var group = _context.Groups.FirstOrDefault(x => x.Id == Guid.Parse(TestDataConstants.TestEntity1Id));
        ArgumentNullException.ThrowIfNull(group);
        var currency = _context.Currencies.FirstOrDefault(x => x.Id == Guid.Parse(TestDataConstants.TestEntity1Id));
        ArgumentNullException.ThrowIfNull(currency);
        group.Users.Add(new UserGroup
        {
            User = lender
        });
        group.Users.Add(new UserGroup
        {
            User = borrower
        });

        var debt = new Debt
        {
            LenderId = lender.Id,
            BorrowerId = borrower.Id,
            GroupId = group.Id,
            Amount = TestDataConstants.TestDebtAmount,
            Remainder = TestDataConstants.TestDebtAmount,
            CreatedDate = DateTime.Parse(TestDataConstants.CreatedDate),
            Currency = currency,
            Approved = true,
            Id = Guid.Parse(TestDataConstants.TestEntity1Id)
        };
            
        var debt1 = new Debt
        {
            LenderId = borrower.Id,
            BorrowerId = lender.Id,
            GroupId = group.Id,
            Amount = TestDataConstants.TestDebtAmount,
            Remainder = TestDataConstants.TestDebtAmount,
            CreatedDate = DateTime.Parse(TestDataConstants.CreatedDate),
            Currency = currency,
            Approved = true,
            Id = Guid.Parse(TestDataConstants.TestEntity2Id)
        };
            
        var debt2 = new Debt
        {
            LenderId = Guid.Parse(TestDataConstants.TestEntity3Id),
            BorrowerId = Guid.Parse(TestDataConstants.TestEntity4Id),
            GroupId = group.Id,
            Amount = TestDataConstants.TestDebtAmount,
            Remainder = TestDataConstants.TestDebtAmount,
            CreatedDate = DateTime.Parse(TestDataConstants.CreatedDate),
            Currency = currency,
            Approved = true,
            Id = Guid.Parse(TestDataConstants.TestEntity3Id)
        };
            
        var debt3 = new Debt
        {
            LenderId = Guid.Parse(TestDataConstants.TestEntity1Id),
            BorrowerId = Guid.Parse(TestDataConstants.TestEntity4Id),
            GroupId = group.Id,
            Amount = TestDataConstants.TestDebtAmount,
            Remainder = TestDataConstants.TestDebtAmount,
            CreatedDate = DateTime.Parse(TestDataConstants.CreatedDate),
            Currency = currency,
            Approved = false,
            Id = Guid.Parse(TestDataConstants.TestEntity4Id)
        };
            
        var debt4 = new Debt
        {
            LenderId = borrower.Id,
            BorrowerId = lender.Id,
            GroupId = group.Id,
            Amount = TestDataConstants.TestDebtAmount,
            Remainder = TestDataConstants.TestDebtAmount,
            CreatedDate = DateTime.Parse(TestDataConstants.CreatedDate),
            Currency = currency,
            Approved = false,
            Id = Guid.Parse(TestDataConstants.TestEntity5Id)
        };
            
        _context.Debts.AddRange(debt, debt1, debt2, debt3, debt4);
        _context.SaveChanges();
        return this;
    }

    public TestDbContextBuilder WithTransactions()
    {
        var lender = _context.Users.FirstOrDefault(x => x.Id == Guid.Parse(TestDataConstants.TestEntity1Id));
        ArgumentNullException.ThrowIfNull(lender);
        var borrower = _context.Users.FirstOrDefault(x => x.Id == Guid.Parse(TestDataConstants.TestEntity2Id));
        ArgumentNullException.ThrowIfNull(borrower);
        var firstDebt = _context.Debts.FirstOrDefault(x => x.Id == Guid.Parse(TestDataConstants.TestEntity1Id));
        ArgumentNullException.ThrowIfNull(firstDebt);
        var secondDebt = _context.Debts.FirstOrDefault(x => x.Id == Guid.Parse(TestDataConstants.TestEntity2Id));
        ArgumentNullException.ThrowIfNull(secondDebt);
        var thirdDebt = _context.Debts.FirstOrDefault(x => x.Id == Guid.Parse(TestDataConstants.TestEntity3Id));
        ArgumentNullException.ThrowIfNull(thirdDebt);
        var currency = _context.Currencies.FirstOrDefault(x => x.Id == Guid.Parse(TestDataConstants.TestEntity1Id));
        ArgumentNullException.ThrowIfNull(currency);

        var transaction1 = new Transaction()
        {
            DebtId = firstDebt.Id,
            Amount = TestDataConstants.TestTransactionAmount,
            Id = Guid.Parse(TestDataConstants.TestEntity1Id),
        };
            
        var transaction2 = new Transaction()
        {
            DebtId = secondDebt.Id,
            Amount = TestDataConstants.TestTransactionAmount,
            Id = Guid.Parse(TestDataConstants.TestEntity2Id)
        };
            
        var transaction3 = new Transaction()
        {
            DebtId = thirdDebt.Id,
            Amount = TestDataConstants.TestTransactionAmount,
            Id = Guid.Parse(TestDataConstants.TestEntity3Id)
        };

        _context.Transactions.AddRange(transaction1,transaction2, transaction3);
        _context.SaveChanges();
        return this;
    }

    public TestDbContextBuilder WithDebtOptimization()
    {
        var userOfFirstCase = _context.Users.FirstOrDefault(x => x.Id == Guid.Parse(TestDataConstants.TestEntity1Id));
        ArgumentNullException.ThrowIfNull(userOfFirstCase);
        var userOfSecondCase = _context.Users.FirstOrDefault(x => x.Id == Guid.Parse(TestDataConstants.TestEntity2Id));
        ArgumentNullException.ThrowIfNull(userOfSecondCase);
        var group = _context.Groups.FirstOrDefault(x => x.Id == Guid.Parse(TestDataConstants.TestEntity1Id));
        ArgumentNullException.ThrowIfNull(group);
        var group4 = _context.Groups.FirstOrDefault(x => x.Id == Guid.Parse(TestDataConstants.TestEntity2Id));
        ArgumentNullException.ThrowIfNull(group4);

        var debtOptimizationRequest1 = new OptimizationRequest
        {
            Id = Guid.Parse(TestDataConstants.TestEntity1Id),
            Group = group,
            Initiator = userOfFirstCase,
        };
            
        var debtOptimizationRequest2 = new OptimizationRequest
        {
            Id = Guid.Parse(TestDataConstants.TestEntity2Id),
            Group = group,
            Initiator = userOfSecondCase,
        };
            
        var debtOptimizationRequest3 = new OptimizationRequest
        {
            Id = Guid.Parse(TestDataConstants.TestEntity3Id),
            Group = group4,
            Initiator = userOfSecondCase,
        };
            
        _context.AddRange(debtOptimizationRequest1, debtOptimizationRequest2, debtOptimizationRequest3); 
        _context.SaveChanges();
        return this;
    }
        
    public TestDbContextBuilder WithDebtOptimizationApprovals()
    {
        var userOfFirstCase = _context.Users.FirstOrDefault(x => x.Id == Guid.Parse(TestDataConstants.TestEntity1Id));
        ArgumentNullException.ThrowIfNull(userOfFirstCase);
        var userOfSecondCase = _context.Users.FirstOrDefault(x => x.Id == Guid.Parse(TestDataConstants.TestEntity2Id));
        ArgumentNullException.ThrowIfNull(userOfSecondCase);
        var optimization = _context.OptimizationRequests.FirstOrDefault(x => x.Id == Guid.Parse(TestDataConstants.TestEntity1Id));
        ArgumentNullException.ThrowIfNull(optimization);
        var approval1 = new OptimizationRequestApproval
        {
            Id = Guid.Parse(TestDataConstants.TestEntity1Id),
            OptimizationRequest = optimization,
            User = userOfFirstCase,
            Approved = false
        };
            
        var approval2 = new OptimizationRequestApproval
        {
            Id = Guid.Parse(TestDataConstants.TestEntity2Id),
            OptimizationRequest = optimization,
            User = userOfSecondCase,
            Approved = true
        };
            
        var approval3 = new OptimizationRequestApproval
        {
            Id = Guid.Parse(TestDataConstants.TestEntity3Id),
            OptimizationRequest = optimization,
            User = userOfFirstCase,
            Approved = true
        };
            
        _context.OptimizationRequestApprovals.AddRange(approval1, approval2, approval3);
        _context.SaveChanges();
        return this;
    }

    public TestDbContextBuilder WithNotifications()
    {
        var notification1 = new Notification
        {
            Id = Guid.Parse(TestDataConstants.TestEntity1Id),
            DebtId = Guid.Parse(TestDataConstants.TestEntity1Id),
            Message = "Test"
        };
        var notification2 = new Notification
        {
            Id = Guid.Parse(TestDataConstants.TestEntity2Id),
            DebtId = Guid.Parse(TestDataConstants.TestEntity2Id),
            Message = "Test"
        };

        _context.Notifications.AddRange(notification1, notification2);
        _context.SaveChanges();
        return this;
    }

    public InDebtContext GetContext()
    {
        return _context;
    }

    private static InDebtContext CreateContext(string name)
    {
        var options = new DbContextOptionsBuilder<InDebtContext>()
            .UseInMemoryDatabase(name)
            .Options;
        return new InDebtContext(options);
    }

    private static List<User> GenerateUsers()
    {
        var password = UserHelper.ComputeSha256Hash("password");
        var users = new List<User>()
        {
            new User
            {
                Username = TestDataConstants.LoggedUsername,
                Name = "name",
                Surname = "surname",
                Email = "email@test.com",
                PasswordHash = password,
                Id = Guid.Parse(TestDataConstants.TestEntity1Id),
                IsConfirmedEmail = true,
            },
            new User
            {
                Username = TestDataConstants.InviterUsername,
                Name ="name",
                Surname ="surname",
                Email = "email1@test.com",
                Id = Guid.Parse(TestDataConstants.TestEntity2Id),
                IsConfirmedEmail = true
            },
            new User
            {
                Username = TestDataConstants.InvitedUsername,
                Name = "name",
                Surname = "surname",
                Email = "email2@test.com",
                Id = Guid.Parse(TestDataConstants.TestEntity3Id),
                IsConfirmedEmail = true
            },
            new User
            {
                Username = TestDataConstants.NotConfirmedUsername,
                Name = "name",
                Surname = "surname",
                Email = "email3@test.com",
                PasswordHash = password,
                Id = Guid.Parse(TestDataConstants.TestEntity4Id),
                IsConfirmedEmail = false
            }
                
        };
        return users;
    }

    private static List<Group> GenerateGroups()
    {
        var groups = new List<Group>()
        {
            new Group
            {
                Name = "name",
                Description = "description",
                Id = Guid.Parse(TestDataConstants.TestEntity1Id)
            },
            new Group
            {
                Name = "name",
                Description = "description",
                Id = Guid.Parse(TestDataConstants.TestEntity2Id)
            },
            new Group
            {
                Name = "name",
                Description = "description",
                Id = Guid.Parse(TestDataConstants.TestEntity3Id),
                Users = new List<UserGroup>
                {
                    new UserGroup
                    {
                        UserId = Guid.Parse(TestDataConstants.TestEntity1Id)
                    },
                    new UserGroup
                    {
                        UserId = Guid.Parse(TestDataConstants.TestEntity2Id)
                    }
                }
            }
                
        };

        return groups;
    }

    private static List<Currency> GenerateCurrencies()
    {
        var currencies = new List<Currency>()
        {
            new Currency
            {
                Name = "name",
                CurrencyCode = TestDataConstants.CurrencyCodeUsd,
                Id = Guid.Parse(TestDataConstants.TestEntity1Id)
            },
            new Currency
            {
                Name = "name",
                CurrencyCode = TestDataConstants.CurrencyCodeRub,
                Id = Guid.Parse(TestDataConstants.TestEntity2Id)
            },
            new Currency
            {
                Name = "name",
                CurrencyCode = TestDataConstants.CurrencyCodeKzt,
                Id = Guid.Parse(TestDataConstants.TestEntity3Id)
            }
        };

        return currencies;
    }
}