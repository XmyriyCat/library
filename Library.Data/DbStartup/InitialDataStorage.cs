using Library.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace Library.Data.DbStartup
{
    public class InitialDataStorage
    {
        public readonly List<Guid> BookGuidList =
        [
            Guid.Parse("a94571e1-7150-490b-bd4b-909e06d39d33"), // Carrie
            Guid.Parse("056527af-0a82-4b4b-99f8-7f3ac7b0f0be"), // The Shining
            Guid.Parse("4f2f47f2-4beb-479d-9255-d449165fdbee"), // Night Shift
            Guid.Parse("705452a0-2d44-48d6-936d-22f6ea11a747"), // Misery
            Guid.Parse("eb5b64b7-ed91-4454-8124-def388dcb60b"), // CLR via Csharp
            Guid.Parse("f3967bfa-c178-4d88-873d-866d34176aa4"), // Clean Code
            Guid.Parse("d9676151-46c1-4a64-967f-c96b05be752a")  // Clean Architecture
        ];
        
        public readonly List<Guid> UserGuidList =
        [
            Guid.Parse("287f1ab7-670e-444f-9899-e2064bc6731a"), // Paulito (Admin)
            Guid.Parse("8e484b22-a82b-4536-a2f2-80c73aa0bdfc"), // Ivan (Manager)
            Guid.Parse("740107c6-714d-4209-97de-90bb929ef2e9"), // Alex (Manager)
            Guid.Parse("8DBA59DC-8BB4-4F07-83D5-71F66938E565")  // Tom (no role)
        ];
        
        private readonly IdentityDataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public InitialDataStorage(IdentityDataContext context, UserManager<User> userManager, 
            RoleManager<Role> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public List<Role> Roles
            => GenerateRoles();

        public List<User> Users
            => GenerateUsers();

        public List<Author> Authors
            => GenerateAuthors();

        public List<Book> Books
            => GenerateBooks();
        
        public List<UserBook> UserBooks
            => GenerateUserBooks();

        private List<User> GenerateUsers()
        {
            var users = new List<User>
            {
                new User
                {
                    Id = UserGuidList[0],
                    SecurityStamp = "66f022d3-a390-45e4-8526-553dee50b4e1",
                    UserName = "Paulito",
                    Email = "Paulito@gmail.com",
                    Roles = [_context.Roles.First(x => x.Name == Variables.Roles.Admin)]
                },
                new User
                {
                    Id = UserGuidList[1],
                    SecurityStamp = "bdd0864b-daea-45b5-9ca1-33c073888ec6",
                    UserName = "Ivan",
                    Email = "Ivan228@gmail.com",
                    Roles = [_context.Roles.First(x => x.Name == Variables.Roles.Manager)]
                },
                new User
                {
                    Id = UserGuidList[2],
                    SecurityStamp = "42b19a3e-9273-4127-aad8-2caf3fe5293d",
                    UserName = "Alex",
                    Email = "AlexAlex22@gmail.com",
                    Roles = [_context.Roles.First(x => x.Name == Variables.Roles.Manager)]
                },
                new User
                {
                    Id = UserGuidList[3],
                    SecurityStamp = "7CF18BB6-F42F-4769-A505-66BDF43690E1",
                    UserName = "Tom",
                    Email = "tom@gmail.com"
                }
            };

            return users;
        }

        private List<Role> GenerateRoles()
        {
            var roles = new List<Role>
            {
                new Role
                {
                    Name = Variables.Roles.Admin,
                    
                },
                new Role
                {
                    Name = Variables.Roles.Manager
                }
            };

            return roles;
        }

        private List<Author> GenerateAuthors()
        {
            var authors = new List<Author>
            {
                new Author()
                {
                    Id = Guid.Parse("cfa075ea-0243-4771-a948-357d64dd3d3c"),
                    Name = "Stephen King",
                    Country = "USA",
                    DateOfBirth = new DateTime(1947, 09, 21)
                },
                new Author()
                {
                    Id = Guid.Parse("7b671b2a-14e5-4c49-b846-6d960e55c31b"),
                    Name = "Robert Martin",
                    Country = "USA",
                    DateOfBirth = new DateTime(1952, 11, 19)
                },
                new Author()
                {
                    Id = Guid.Parse("11c72779-7cf5-4083-8b8a-f0921aaa37db"),
                    Name = "Jeffrey Richter",
                    Country = "USA",
                    DateOfBirth = new DateTime(1964, 07, 27)
                }
            };

            return authors;
        }

        private List<Book> GenerateBooks()
        {
            var books = new List<Book>()
            {
                new Book()
                {
                    Id = BookGuidList[0],
                    Isbn = "345-24223-8-595",
                    Title = "Carrie",
                    Genre = "Horror",
                    Description = "No description...",
                    ImageName = BookGuidList[0] + ".png",
                    Author = _context.Authors
                        .FirstOrDefault(x => x.Id == Guid.Parse("cfa075ea-0243-4771-a948-357d64dd3d3c"))!
                },
                new Book()
                {
                    Id = BookGuidList[1],
                    Isbn = "785-1-123-15255-3",
                    Title = "The Shining",
                    Genre = "Novel",
                    Description = "No description...",
                    ImageName = BookGuidList[1] + ".png",
                    Author = _context.Authors
                        .FirstOrDefault(x => x.Id == Guid.Parse("cfa075ea-0243-4771-a948-357d64dd3d3c"))!
                },
                new Book()
                {
                    Id = BookGuidList[2],
                    Isbn = "741-1-153-13548-8",
                    Title = "Night Shift",
                    Genre = "Horror",
                    Description = "No description...",
                    ImageName = BookGuidList[2] + ".png",
                    Author = _context.Authors
                        .FirstOrDefault(x => x.Id == Guid.Parse("cfa075ea-0243-4771-a948-357d64dd3d3c"))!
                },
                new Book()
                {
                    Id = BookGuidList[3],
                    Isbn = "978-0-385-12167-5",
                    Title = "Misery",
                    Genre = "Fantasy",
                    Description = "No description...",
                    ImageName = BookGuidList[3] + ".png",
                    Author = _context.Authors
                        .FirstOrDefault(x => x.Id == Guid.Parse("cfa075ea-0243-4771-a948-357d64dd3d3c"))!
                },
                new Book()
                {
                    Id = BookGuidList[4],
                    Isbn = "745-1-735-66745-7",
                    Title = "CLR via Csharp",
                    Genre = "Technical",
                    Description = "No description...",
                    ImageName = BookGuidList[4] + ".png",
                    Author = _context.Authors
                        .FirstOrDefault(x => x.Id == Guid.Parse("11c72779-7cf5-4083-8b8a-f0921aaa37db"))!
                },
                new Book()
                {
                    Id = BookGuidList[5],
                    Isbn = "988-1-654-45621-9",
                    Title = "Clean Code",
                    Genre = "Technical",
                    Description = "No description...",
                    ImageName = BookGuidList[5] + ".png",
                    Author = _context.Authors
                        .FirstOrDefault(x => x.Id == Guid.Parse("7b671b2a-14e5-4c49-b846-6d960e55c31b"))!
                },
                new Book()
                {
                    Id = BookGuidList[6],
                    Isbn = "778-1-656-12583-4",
                    Title = "Clean Architecture",
                    Genre = "Technical",
                    Description = "No description...",
                    ImageName = BookGuidList[6] + ".png",
                    Author = _context.Authors
                        .FirstOrDefault(x => x.Id == Guid.Parse("7b671b2a-14e5-4c49-b846-6d960e55c31b"))!
                }
            };

            return books;
        }

        private List<UserBook> GenerateUserBooks()
        {
            var userBooks = new List<UserBook>
            {
                new UserBook
                {
                    BookId = BookGuidList[0],
                    UserId = UserGuidList[0],
                    TakenDate = DateTime.UtcNow.AddDays(-3),
                    ReturnDate = DateTime.UtcNow.AddDays(4)
                },
                new UserBook
                {
                    BookId = BookGuidList[2],
                    UserId = UserGuidList[0],
                    TakenDate = DateTime.UtcNow.AddMonths(-10),
                    ReturnDate = DateTime.UtcNow.AddMonths(-5)
                },
                new UserBook
                {
                    BookId = BookGuidList[5],
                    UserId = UserGuidList[3],
                    TakenDate = DateTime.UtcNow.AddDays(-1),
                    ReturnDate = DateTime.UtcNow.AddDays(6)
                }
            };

            return userBooks;
        }
    }
}