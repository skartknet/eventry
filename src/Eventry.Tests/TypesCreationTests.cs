
using Eventry.Installation;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Services.Implement;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Tests.Common.Testing;
using Umbraco.Cms.Tests.Integration.Testing;

namespace Eventry.Tests
{

    [TestFixture]
    [UmbracoTest(Database = UmbracoTestOptions.Database.NewSchemaPerFixture)]
    public class TypesCreationTests : UmbracoIntegrationTest
    {


        [Test]
        public async Task It_Should_Create_Eventry_DataTypes()
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
                Assert.IsNotNull(currentMapEditor, "map editor is null");
                Assert.IsNotNull(currentEventryTags, "tags editor is null");
                Assert.IsNotNull(currentPriceField, "price editor is null");
                Assert.IsNotNull(currentStockField, "stock editor is null");
                Assert.True(currentMapEditor?.ConfigurationData.Count > 0);
                Assert.True(currentEventryTags?.ConfigurationData.Count > 0);
            });



        }

        [Test]
        public async Task Should_Create_Eventry_ContentTypes()
        {
            var dataTypeService = GetRequiredService<IDataTypeService>();
            var contentTypeService = GetRequiredService<IContentTypeService>();
            var propertyEditors = GetRequiredService<PropertyEditorCollection>();
            var stringHelper = GetRequiredService<IShortStringHelper>();

            var contentTypesTask = new CreateEventryDocumentTypesTask(contentTypeService,
                                    dataTypeService,
                                    stringHelper,
                                    propertyEditors);

            // SUT
            await contentTypesTask.Execute();


            var existingContainer = contentTypeService.GetContainer(Constants.ContentTypes.Guids.BaseFolder);

            Assert.IsNotNull(existingContainer);
        }
    }
}