using Company.Models;
using Xunit;

namespace Company.Tests
{
    public class SuppliersTests
    {
        [Fact]
        public void CanCreateSupplier()
        {
            // Arrange
            var supplier = new Suppliers
            {
                ID = 1,
                NameCompany = "ABC Inc.",
                GameID = 2,
                quantity = 100
            };

            // Act

            // Assert
            Assert.Equal(1, supplier.ID);
            Assert.Equal("ABC Inc.", supplier.NameCompany);
            Assert.Equal(2, supplier.GameID);
            Assert.Equal(100, supplier.quantity);
        }
    }
}