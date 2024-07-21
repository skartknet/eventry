
using Eventry.Installation;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Tests.Common.Testing;
using Umbraco.Cms.Tests.Integration.Testing;

namespace Eventry.Tests
{

    [TestFixture]
    [UmbracoTest(Database = UmbracoTestOptions.Database.NewSchemaPerTest)]
    public class TypesCreationTests : UmbracoIntegrationTest
    {


        [Test]
        public async Task Should_Create_Eventry_DataTypes()
        {
            var dataTypeService = GetRequiredService<IDataTypeService>();
            var propertyEditors = GetRequiredService<PropertyEditorCollection>();
            var configurationEditorJsonSerializer = GetRequiredService<IConfigurationEditorJsonSerializer>();

            var dataTypesTask = new CreateEventryDataTypesTask(dataTypeService,
                                    propertyEditors,
            configurationEditorJsonSerializer);

            // SUT
            await dataTypesTask.Execute();


            var currentMapEditor = await dataTypeService.GetAsync(Constants.DataTypes.Guids.Maps);
            var currentEventryTags = await dataTypeService.GetAsync(Constants.DataTypes.Guids.Tags);
            var currentPriceField = await dataTypeService.GetAsync(Constants.DataTypes.Guids.Price);
            var currentStockField = await dataTypeService.GetAsync(Constants.DataTypes.Guids.Stock);

            Assert.Multiple(() =>
            {
                Assert.That(currentMapEditor is not null, "map editor is null");
                Assert.That(currentEventryTags is not null, "tags editor is null");
                Assert.That(currentPriceField is not null, "price editor is null");
                Assert.That(currentStockField is not null, "stock editor is null");
                Assert.That(currentMapEditor?.ConfigurationData.Count > 0);
                Assert.That(currentEventryTags?.ConfigurationData.Count > 0);
            });



        }

        [Test]
        public async Task Should_Create_Eventry_ContentTypes()
        {
            var dataTypeService = GetRequiredService<IDataTypeService>();
            var contentTypeService = GetRequiredService<IContentTypeService>();
            var propertyEditors = GetRequiredService<PropertyEditorCollection>();
            var stringHelper = GetRequiredService<IShortStringHelper>();
            var configurationEditorJsonSerializer = GetRequiredService<IConfigurationEditorJsonSerializer>();
            var templateService = GetRequiredService<ITemplateService>();

            var dataTypesTask = new CreateEventryDataTypesTask(dataTypeService,
                                 propertyEditors,
                                 configurationEditorJsonSerializer);

            // SUT
            await dataTypesTask.Execute();

            var contentTypesTask = new CreateEventryDocumentTypesTask(contentTypeService,
                                    dataTypeService,
                                    stringHelper,
                                    templateService,
                                    propertyEditors);

            // SUT
            await contentTypesTask.Execute();


            var existingContainer = contentTypeService.GetContainer(Constants.ContentTypes.Guids.BaseFolder);
            var physicalEvent = contentTypeService.Get(Constants.ContentTypes.Guids.PhysicalEvent);
            var onlineEvent = contentTypeService.Get(Constants.ContentTypes.Guids.OnlineEvent);
            var eventsListing = contentTypeService.Get(Constants.ContentTypes.Guids.EventsListing);

            var physicalEventCompositionExists = physicalEvent?.ContentTypeCompositionExists(Constants.ContentTypes.Aliases.EventBaseComposition);
            var onlineEventCompositionExists = onlineEvent?.ContentTypeCompositionExists(Constants.ContentTypes.Aliases.EventBaseComposition);

            Assert.Multiple(() =>
            {
                Assert.That(existingContainer is not null, "Base folder doesn't exist");
                Assert.That(physicalEvent is not null, "Physical event doesn't exist");
                Assert.That(onlineEvent is not null, "Online event doesn't exist");
                Assert.That(eventsListing is not null, "Events listing doesn't exist");

                Assert.That(physicalEventCompositionExists == true, "Physical Event doesn't contain the Event Base composition");
                Assert.That(onlineEventCompositionExists == true, "Online Event doesn't contain the Event Base composition");
            });
        }

        [Test]
        public async Task Should_Create_Eventry_Templates()
        {
            var templateService = GetRequiredService<ITemplateService>();
            var stringHelper = GetRequiredService<IShortStringHelper>();

            var templatesTask = new CreateEventryTemplatesTask(templateService,
                                                        stringHelper);

            // SUT
            await templatesTask.Execute();

            var listing = await templateService.GetAsync(Constants.Templates.Aliases.EventListing);
            var onlineEvent = await templateService.GetAsync(Constants.Templates.Aliases.OnlineEvent);
            var physicalEvent = await templateService.GetAsync(Constants.Templates.Aliases.PhysicalEvent);

            Assert.Multiple(() =>
            {
                Assert.That(listing is not null, "Layout template doesn't exist");
                Assert.That(onlineEvent is not null, "OnlineEvent template doesn't exist");
                Assert.That(physicalEvent is not null, "PhysicalEvent template doesn't exist");
            });

        }
    }
}