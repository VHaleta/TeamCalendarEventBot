using AutoFixture;
using FluentAssertions;
using TeamCalendarEventBot.Domain.Processor.Services;
using TeamCalendarEventBot.Domain.Repositories;
using TeamCalendarEventBot.Models.Constants;
using TeamCalendarEventBot.Models.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Xunit;

namespace TeamCalendarEventBot.Tests
{
    public class UserServiceTests : IClassFixture<UserServiceFixture>, IClassFixture<LoggerFixture<UserService>>
    {
        private readonly UserService _userService;
        private readonly IUserBotRepository _userBotRepositorySubstitute;
        private readonly ITelegramBotClient _botClientSubstitute;
        private readonly Fixture _fixture;

        public UserServiceTests(UserServiceFixture userServiceFixture, LoggerFixture<UserService> loggerFixture)
        {
            _userBotRepositorySubstitute = userServiceFixture.UserBotRepository;
            _botClientSubstitute = userServiceFixture.TelegramBotClient;

            _userService = new UserService(
                userServiceFixture.UserBotRepository,
                loggerFixture.Logger,
                userServiceFixture.MenuService);
        }

        [Fact]
        public async void IsUserAuthorizedAsync_ForUnauthenticatedUser_ShouldReturnFalse()
        {
            // Arrange
            var user = new UserBot()
            {
                ChatId = 1,
                Auth = AuthenticationState.None
            };
            var message = new Message()
            {
                Text = "message"
            };

            //Act
            var result = await _userService.IsUserAuthorizedAsync(_botClientSubstitute, message, user);

            //Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async void IsUserAuthorizedAsync_ForAuthenticatedUser_ShouldReturnTrue()
        {
            // Arrange
            var user = new UserBot()
            {
                ChatId = 1,
                Auth = AuthenticationState.Approved
            };
            var message = new Message()
            {
                Text = "message"
            };

            //Act
            var result = await _userService.IsUserAuthorizedAsync(_botClientSubstitute, message, user);

            //Assert
            result.Should().BeTrue();
        }
    }
}
