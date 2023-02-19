using Company.Models;
using Xunit;

namespace Company.Tests
{
    public class GameTests
    {
        [Fact]
        public void CanCreateGame()
        {
            // Arrange
            var game = new Game
            {
                Title = "The Witcher 3: Wild Hunt",
                Plant = "CD Projekt Red",
                EquipmentDesc = "Minimum requirements: Nvidia GTX 660, Intel i5-2500K, 6GB RAM",
                gamekind = Game.gameKind.RPG,
                PricePerPieces = 99.99,
                quantity = 100,
                Description = "The Witcher 3: Wild Hunt is a story-driven, next-generation open world role-playing game set in a visually stunning fantasy universe full of meaningful choices and impactful consequences. In The Witcher, you play as professional monster hunter Geralt of Rivia tasked with finding a child of prophecy in a vast open world rich with merchant cities, pirate islands, dangerous mountain passes, and forgotten caverns to explore.",
                Image = "witcher3.jpg"
            };

            // Act (nothing to do here)

            // Assert
            Assert.Equal("The Witcher 3: Wild Hunt", game.Title);
            Assert.Equal("CD Projekt Red", game.Plant);
            Assert.Equal("Minimum requirements: Nvidia GTX 660, Intel i5-2500K, 6GB RAM", game.EquipmentDesc);
            Assert.Equal(Game.gameKind.RPG, game.gamekind);
            Assert.Equal(99.99, game.PricePerPieces);
            Assert.Equal(100, game.quantity);
            Assert.Equal("The Witcher 3: Wild Hunt is a story-driven, next-generation open world role-playing game set in a visually stunning fantasy universe full of meaningful choices and impactful consequences. In The Witcher, you play as professional monster hunter Geralt of Rivia tasked with finding a child of prophecy in a vast open world rich with merchant cities, pirate islands, dangerous mountain passes, and forgotten caverns to explore.", game.Description);
            Assert.Equal("witcher3.jpg", game.Image);
        }
    }
}