using console_flappy_bird.Models;
using console_flappy_bird.Services;
using Xunit;

namespace console_flappy_bird.Tests
{
    public class ConsoleServiceTests
    {
        ConsoleService _consoleService;

        public ConsoleServiceTests()
        {
            _consoleService = new ConsoleService();
        }

        [Fact]
        public void TryBackSpaceUsername_ShouldReturnFalseWhenEmpty()
        {
            Assert.False(_consoleService.TryBackspaceUsername());
        }

        [Fact]
        public void TryDeleteUsername_ShouldReturnFalseWhenEmpty()
        {
            Assert.False(_consoleService.TryDeleteUsername());
        }

        [Fact]
        public void GetBirdControllerModel_DataShouldBeTheSameAsConfig()
        {
            var config = InputOutputService.Load<ConfigModel>(@"C:\Uni\console-flappy-bird\console-flappy-bird\Resources\Config.json");
            var birdControllerModel = _consoleService.GetBirdControllerModel();

            Assert.Equal(config.ScreenHeight, birdControllerModel.ScreenHeight);
            Assert.Equal(config.RefreshInterval, birdControllerModel.RefreshInterval);
            Assert.Equal(config.GravityConstant, birdControllerModel.GravityConstant);
            Assert.Equal(config.JumpAmount, birdControllerModel.JumpAmount);
        }

    }
}
