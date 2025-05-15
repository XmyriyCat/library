using FluentAssertions;
using Library.Application.Services.Implementations;
using Library.Contracts.Requests.Author;
using Library.Contracts.Responses.Author;
using Library.Data.Models;
using Library.Data.UnitOfWork;
using MapsterMapper;
using FluentValidation;
using Library.Application.Infrastructure.Validators;
using Library.Application.Infrastructure.Validators.Author;
using Moq;

namespace Library.Tests
{
    public class AuthorServiceTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly AuthorService _authorService;

        public AuthorServiceTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _mapperMock = new Mock<IMapper>();
            
            var authorValidator = new AuthorValidator();
            
            var pagedRequestValidator = new PagedRequestValidator();
            
            _authorService = new AuthorService(
                _repositoryWrapperMock.Object,
                _mapperMock.Object,
                authorValidator,
                pagedRequestValidator);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnCreatedAuthor()
        {
            // Arrange
            var request = new CreateAuthorRequest
            {
                Name = "New Author",
                Country = "Belarus",
                DateOfBirth = new DateTime(2005, 1, 20)
            };

            var author = new Author
            {
                Name = "New Author",
                Country = "Belarus",
                DateOfBirth = new DateTime(2005, 1, 20)
            };

            var createdAuthor = new Author
            {
                Id = Guid.Parse("FF5981FE-3D8D-4F26-ABA1-FCD3DFCBBB92"),
                Name = "New Author",
                Country = "Belarus",
                DateOfBirth = new DateTime(2005, 1, 20)
            };

            var response = new AuthorResponse
            {
                Id = createdAuthor.Id,
                Name = createdAuthor.Name,
                Country = createdAuthor.Country,
                DateOfBirth = createdAuthor.DateOfBirth
            };

            _mapperMock.Setup(m => m.Map<Author>(It.IsAny<CreateAuthorRequest>())).Returns(author);
            _repositoryWrapperMock.Setup(r => r.Authors.CreateAsync(author, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdAuthor);
            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<AuthorResponse>(createdAuthor)).Returns(response);

            // Act
            var result = await _authorService.CreateAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(Guid.Parse("FF5981FE-3D8D-4F26-ABA1-FCD3DFCBBB92"));
            result.Name.Should().Be("New Author");
            result.Country.Should().Be("Belarus");
            result.DateOfBirth.Should().Be(new DateTime(2005, 1, 20));
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowValidationException_WhenValidationFails()
        {
            // Arrange
            var request = new CreateAuthorRequest
            {
                Name = "", // Invalid
                Country = "", // Invalid
                DateOfBirth = new DateTime(2005, 1, 20)
            };

            var author = new Author();

            _mapperMock.Setup(m => m.Map<Author>(request)).Returns(author);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _authorService.CreateAsync(request));
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnAuthorResponse_WhenAuthorExists()
        {
            // Arrange
            var authorId = Guid.Parse("33035986-7749-4D30-92BB-9B09F99253FE");

            var author = new Author
            {
                Id = authorId,
                Name = "Author Name",
                Country = "Belarus",
                DateOfBirth = new DateTime(2005, 1, 20)
            };

            var response = new AuthorResponse
            {
                Id = authorId,
                Name = "Author Name",
                Country = "Belarus",
                DateOfBirth = new DateTime(2005, 1, 20)
            };

            _repositoryWrapperMock.Setup(r => r.Authors.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(author);

            _mapperMock.Setup(m => m.Map<AuthorResponse>(author)).Returns(response);

            // Act
            var result = await _authorService.GetByIdAsync(authorId);

            // Assert
            result.Should().NotBeNull();
            result?.Id.Should().Be(Guid.Parse("33035986-7749-4D30-92BB-9B09F99253FE"));
            result?.Name.Should().Be("Author Name");
            result?.Country.Should().Be("Belarus");
            result?.DateOfBirth.Should().Be(new DateTime(2005, 1, 20));
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenAuthorDoesNotExist()
        {
            // Arrange
            var authorId = Guid.Parse("E7A0CFE4-120A-48D7-BF47-F4EE68C64BD9");
            _repositoryWrapperMock.Setup(r => r.Authors.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Author?)null);

            // Act
            var result = await _authorService.GetByIdAsync(authorId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnUpdatedAuthorResponse()
        {
            // Arrange
            var authorId = Guid.Parse("BFEDF545-3FD6-477C-BE9F-075C156CE204");

            var request = new UpdateAuthorRequest
            {
                Name = "Updated Author",
                Country = "France",
                DateOfBirth = new DateTime(1995, 11, 11)
            };

            var author = new Author
            {
                Id = authorId,
                Name = "Updated Author",
                Country = "France",
                DateOfBirth = new DateTime(1995, 11, 11)
            };

            var updatedAuthor = new Author
            {
                Id = authorId,
                Name = "Updated Author",
                Country = "France",
                DateOfBirth = new DateTime(1995, 11, 11)
            };

            var response = new AuthorResponse
            {
                Id = authorId,
                Name = "Updated Author",
                Country = "France",
                DateOfBirth = new DateTime(1995, 11, 11)
            };

            _mapperMock.Setup(m => m.Map<Author>(It.IsAny<UpdateAuthorRequest>())).Returns(author);
            _repositoryWrapperMock.Setup(r => r.Authors.UpdateAsync(author, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedAuthor);
            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<AuthorResponse>(updatedAuthor)).Returns(response);

            // Act
            var result = await _authorService.UpdateAsync(authorId, request);

            // Assert
            result.Should().NotBeNull();
            result?.Id.Should().Be(Guid.Parse("BFEDF545-3FD6-477C-BE9F-075C156CE204"));
            result?.Name.Should().Be("Updated Author");
            result?.Country.Should().Be("France");
            result?.DateOfBirth.Should().Be(new DateTime(1995, 11, 11));
        }

        [Fact]
        public async Task DeleteByIdAsync_ShouldReturnTrue_WhenAuthorIsDeleted()
        {
            // Arrange
            var authorId = Guid.Parse("5BE97322-9AF7-4CC0-96FC-E8BEA51C461D");

            _repositoryWrapperMock.Setup(r => r.Authors.DeleteByIdAsync(authorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authorService.DeleteByIdAsync(authorId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteByIdAsync_ShouldReturnFalse_WhenAuthorIsNotDeleted()
        {
            // Arrange
            var authorId = Guid.Parse("43C860CB-26AE-45B7-8715-DC36A921FE5B");

            _repositoryWrapperMock.Setup(r => r.Authors.DeleteByIdAsync(authorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authorService.DeleteByIdAsync(authorId);

            // Assert
            result.Should().BeFalse();
        }
    }
}
