using System;
using System.Threading.Tasks;
using Moq;
using Tigerspike.Solv.Domain.Commands.Ticket;
using Tigerspike.Solv.Domain.Models;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.Validators
{
    public class TagsImportTicketCommandValidatorTests : ImportTicketCommandValidatorBase
    {

        [Fact]
        public async Task WhenTagsAreEmptyThenImportTicketCommandShouldBeValid()
        {
            Guid[] tags = null;
            var brandId = Guid.NewGuid();
            var cmd = ImportTicketCommandBuilder.Build(tags: tags, brandId: brandId);

            var result = await SystemUnderTest.ValidateAsync(cmd);
            Assert.DoesNotContain(result.Errors, x => x.PropertyName == nameof(ImportTicketCommand.Tags));
        }

        [Fact]
        public async Task WhenSomeTagsDoesNotExistThenImportTicketCommandShouldBeInvalid()
        {
            var tagOk1 = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var tagOk2 = Guid.Parse("00000000-0000-0000-0000-000000000002");
            var tagOk3 = Guid.Parse("00000000-0000-0000-0000-000000000003");
            var tagFail = Guid.Parse("00000000-ffff-0000-0000-000000000000");

            var tags = new Guid[] { tagOk1, tagFail };
            var brandId = Guid.NewGuid();
            var cmd = ImportTicketCommandBuilder.Build(tags: tags, brandId: brandId);

            Mocker.GetMock<IBrandRepository>()
                .Setup(x => x.GetTags(brandId, true, null))
                .ReturnsAsync(new Tag[] { new Tag(brandId, "ok1") { Id = tagOk1 }, new Tag(brandId, "ok2") { Id = tagOk2 }, new Tag(brandId, "ok3") { Id = tagOk3 }, });

            var result = await SystemUnderTest.ValidateAsync(cmd);
            Assert.Contains(result.Errors, x => x.PropertyName == nameof(ImportTicketCommand.Tags));
        }

        [Fact]
        public async Task WhenAllTagsExistThenImportTicketCommandShouldBeValid()
        {
            var tagOk1 = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var tagOk2 = Guid.Parse("00000000-0000-0000-0000-000000000002");
            var tagOk3 = Guid.Parse("00000000-0000-0000-0000-000000000003");
            var tagFail = Guid.Parse("00000000-ffff-0000-0000-000000000000");

            var tags = new Guid[] { tagOk1, tagOk3 };

            var brandId = Guid.NewGuid();
            var cmd = ImportTicketCommandBuilder.Build(tags: tags, brandId: brandId);

            Mocker.GetMock<IBrandRepository>()
                .Setup(x => x.GetTags(brandId, true, null))
                .ReturnsAsync(new Tag[] { new Tag(brandId, "ok1") { Id = tagOk1 }, new Tag(brandId, "ok2") { Id = tagOk2 }, new Tag(brandId, "ok3") { Id = tagOk3 }, });

            var result = await SystemUnderTest.ValidateAsync(cmd);
            Assert.DoesNotContain(result.Errors, x => x.PropertyName == nameof(ImportTicketCommand.Tags));
        }
    }
}