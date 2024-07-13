using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;



namespace Eventry.Installation
{
    public class CreateEventryDataTypesTask
    {
        private readonly IDataTypeService _dataTypeService;
        private readonly PropertyEditorCollection _propertyEditors;

        private readonly IConfigurationEditorJsonSerializer _configurationEditorJsonSerializer;

        public CreateEventryDataTypesTask(IDataTypeService dataTypeService, PropertyEditorCollection propertyEditors,
            IConfigurationEditorJsonSerializer configurationEditorJsonSerializer)
        {
            _dataTypeService = dataTypeService;
            _propertyEditors = propertyEditors;
            _configurationEditorJsonSerializer = configurationEditorJsonSerializer;
        }

        public async Task Execute()
        {
            // Maps
            var currentMapEditor = await _dataTypeService.GetAsync(Constants.DataTypes.Guids.Maps);
            if (currentMapEditor == null)
            {
                if (_propertyEditors.TryGet(Bergmania.OpenStreetMap.Core.Constants.EditorAlias, out IDataEditor? editor))
                {

                    CreateDataType(editor, x =>
                    {
                        x.Key = Constants.DataTypes.Guids.Maps;
                        x.Name = "[Eventry] Maps";
                        x.EditorUiAlias = "Bergmania.PropertyEditorUi.OpenStreetMap";
                    }, new Bergmania.OpenStreetMap.Core.OpenStreetMapConfiguration());

                }
            }

            // Tags
            var currentEventryTags = await _dataTypeService.GetAsync(Constants.DataTypes.Guids.Tags);
            if (currentEventryTags == null)
            {
                if (_propertyEditors.TryGet(Umbraco.Cms.Core.Constants.PropertyEditors.Aliases.Tags, out IDataEditor? editor))
                {
                    CreateDataType(editor, x =>
                    {
                        x.Key = Constants.DataTypes.Guids.Tags;
                        x.Name = "[Eventry] Tags";
                        x.EditorUiAlias = "Umb.PropertyEditorUi.Tags";
                    },
                    new TagConfiguration
                    {
                        Group = "Eventry"
                    });


                }
            }

            var currentPriceField = await _dataTypeService.GetAsync(Constants.DataTypes.Guids.Price);
            if (currentPriceField == null)
            {
                if (_propertyEditors.TryGet(Umbraco.Commerce.Cms.Constants.PropertyEditors.Aliases.Price, out IDataEditor? editor))
                {
                    CreateDataType(editor, x =>
                    {
                        x.Key = Constants.DataTypes.Guids.Price;
                        x.Name = "[Eventry] Price";
                        x.EditorUiAlias = "Uc.PropertyEditorUi.Price";
                    });

                }
            }

            var currentStockField = await _dataTypeService.GetAsync(Constants.DataTypes.Guids.Stock);
            if (currentStockField == null)
            {
                if (_propertyEditors.TryGet(Umbraco.Commerce.Cms.Constants.PropertyEditors.Aliases.Stock, out IDataEditor? editor))
                {
                    CreateDataType(editor, x =>
                    {
                        x.Key = Constants.DataTypes.Guids.Stock;
                        x.Name = "[Eventry] Stock";
                        x.EditorUiAlias = "Uc.PropertyEditorUi.Stock";
                    });

                }
            }
        }

        private void CreateDataType(IDataEditor dataEditor, Action<DataType> config, object? typeSpecificConfig = null)
        {
            var dataType = new DataType(dataEditor, _configurationEditorJsonSerializer);

            config.Invoke(dataType);

            if (typeSpecificConfig is not null)
            {
                dataType.ConfigurationData = dataType.Editor!.GetConfigurationEditor()
                                                       .FromConfigurationObject(typeSpecificConfig, _configurationEditorJsonSerializer);
            }

            _dataTypeService.Save(dataType, -1);

        }
    }
}