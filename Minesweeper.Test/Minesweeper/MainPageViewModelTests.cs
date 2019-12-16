using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Minesweeper.Forms.ViewModels;
using NUnit.Framework;

namespace Minesweeper.Test
{
    [TestFixture]
    public class MainPageViewModelTests
    {
        private MainPageViewModel vm;
        private Fixture fixture;

        [SetUp]
        public void Setup()
        {
            fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization() { ConfigureMembers = true, GenerateDelegates = true });

            vm = fixture.Build<MainPageViewModel>().OmitAutoProperties().Create();
        }

        [Test]
        public void Cells_ShouldHave_100()
        {
            vm.Cells.Should().HaveCount(100);
        }

        [Test]
        public void Ctor_Passes()
        {
            Assert.Pass();
        }
    }
}