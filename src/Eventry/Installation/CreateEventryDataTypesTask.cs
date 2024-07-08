using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;



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

                    var dataType = CreateDataType(editor, x =>

                    {
                        x.Key = Constants.DataTypes.Guids.Maps;
                        x.Name = "[Eventry] Maps";
                        x.DatabaseType = ValueStorageType.Nvarchar;
                    });

                    _dataTypeService.Save(dataType, -1);
                }
            }

            // Tags
            var currentEventryTags = await _dataTypeService.GetAsync(Constants.DataTypes.Guids.Tags);
            if (currentEventryTags == null)
            {
                if (_propertyEditors.TryGet(Umbraco.Cms.Core.Constants.PropertyEditors.Aliases.Tags, out IDataEditor? editor))
                {
                    var dataType = CreateDataType(editor, x =>
                    {
                        x.Key = Constants.DataTypes.Guids.Tags;
                        x.Name = "[Eventry] Tags";
                        x.DatabaseType = ValueStorageType.Nvarchar;

                        x.ConfigurationAs<TagConfiguration>()!.Group = "Eventry";
                    });

                    _dataTypeService.Save(dataType, -1);
                }
            }


        }

        private DataType CreateDataType(IDataEditor dataEditor, Action<DataType> config)
        {
            var dataType = new DataType(dataEditor, _configurationEditorJsonSerializer);

            config.Invoke(dataType);

            return dataType;
        }
    }
}