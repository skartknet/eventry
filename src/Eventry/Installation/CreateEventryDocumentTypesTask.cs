using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;

namespace Eventry.Installation
{
    public class CreateEventryDocumentTypesTask
    {
        private readonly IContentTypeService _contentTypeService;
        private readonly IDataTypeService _dataTypeService;

        private readonly IShortStringHelper _shortStringHelper;
        private readonly PropertyEditorCollection _propertyEditors;

        public CreateEventryDocumentTypesTask(IContentTypeService contentTypeService,
                                                              IDataTypeService dataTypeService,
                                                              IShortStringHelper shortStringHelper,
                                                              PropertyEditorCollection propertyEditors)
        {
            _contentTypeService = contentTypeService;
            _dataTypeService = dataTypeService;
            _shortStringHelper = shortStringHelper;
            _propertyEditors = propertyEditors;
        }

        public async Task Execute()
        {

            // Setup lazy data types
            var gmapDataType = await _dataTypeService.GetAsync(Constants.DataTypes.Guids.Maps) ?? throw new NullReferenceException("Map datatype not available"); ;
            var textareaDataType = await _dataTypeService.GetAsync(Umbraco.Cms.Core.Constants.DataTypes.Guids.TextareaGuid) ?? throw new NullReferenceException("Textarea datatype not available");
            var rteDataType = await _dataTypeService.GetAsync(Umbraco.Cms.Core.Constants.DataTypes.Guids.RichtextEditorGuid);
            var dateTimeDataType = await _dataTypeService.GetAsync(Umbraco.Cms.Core.Constants.DataTypes.Guids.DatePickerWithTimeGuid);
            var contentPickerDataType = await _dataTypeService.GetAsync(Umbraco.Cms.Core.Constants.DataTypes.Guids.ContentPickerGuid);
            var umPriceDataType = await _dataTypeService.GetAsync(Constants.DataTypes.Guids.Price);
            var umStockDataType = await _dataTypeService.GetAsync(Constants.DataTypes.Guids.Stock);


            IContentType? existing;
            int baseFolderContentTypeId;


            PropertyType[] shareProps = null;
            try
            {
                shareProps =
                [

                    CreatePropertyType(gmapDataType, x =>
                    {
                        x.Alias = "location";
                        x.Name = "Location";
                        x.Description = "Event physical location";
                        x.SortOrder = 10;
                    }),
                    CreatePropertyType( textareaDataType, x =>
                    {
                        x.Alias = "summary";
                        x.Name = "Summary";
                        x.Description = "";
                        x.SortOrder = 10;
                    }),
                    CreatePropertyType( rteDataType, x =>
                    {
                        x.Alias = "description";
                        x.Name = "Description";
                        x.Description = "";
                        x.SortOrder = 20;
                    }),
                    CreatePropertyType( umPriceDataType, x =>
                    {
                        x.Alias = "price";
                        x.Name = "Price";
                        x.Description = "";
                        x.SortOrder = 30;
                    }),
                    CreatePropertyType( umStockDataType, x =>
                    {
                        x.Alias = "stock";
                        x.Name = "Capacity";
                        x.Description = "";
                        x.SortOrder = 40;
                    }),
                    CreatePropertyType( dateTimeDataType, x =>
                    {
                        x.Alias = "start";
                        x.Name = "Capacity";
                        x.Description = "";
                        x.SortOrder = 50;
                    }),
                    CreatePropertyType( dateTimeDataType, x =>
                    {
                        x.Alias = "end";
                        x.Name = "End";
                        x.Description = "";
                        x.SortOrder = 60;
                    }),
                    CreatePropertyType( contentPickerDataType, x =>
                    {
                        x.Alias = "refundPolicyPage";
                        x.Name = "Refund Policy page";
                        x.Description = "";
                        x.SortOrder = 60;
                    })
                ];
            }
            catch (Exception ex)
            {
                var e = ex;
            }

            // Base folder
            existing = _contentTypeService.Get(Constants.ContentTypes.Guids.BaseFolder);
            if (existing == null)
            {
                var containerAttempt = _contentTypeService.CreateContainer(-1, Constants.ContentTypes.Guids.BaseFolder, "Eventry");
                if(containerAttempt.Success == false)
                {
                    throw new Exception("Failed to create container");
                }
                
                var container = containerAttempt.Result?.Entity;

                baseFolderContentTypeId = container!.Id;
            }
            else
            {
                baseFolderContentTypeId = existing.Id;
            }

            // Physical event
            existing = _contentTypeService.Get(Constants.ContentTypes.Guids.PhysicalEvent);
            if (existing is null)
            {
                var contentType = CreateContentType(baseFolderContentTypeId, x =>
                {
                    x.Key = Constants.ContentTypes.Guids.PhysicalEvent;
                    x.Alias = Constants.ContentTypes.Aliases.PhysicalEvent;
                    x.Name = "Physical Event";
                    x.Icon = "icon-cash-register color-green";
                    x.PropertyGroups = new PropertyGroupCollection(new[]
                    {
                        new PropertyGroup(new PropertyTypeCollection(true, shareProps))
                        {
                            Alias = "settings",
                            Name = "Settings",
                            Type = PropertyGroupType.Group,
                            SortOrder = 50
                        }
                    });
                });
            }
            else
            {
                var saveExisting = false;
                var hasSettingsGroup = existing.PropertyGroups.Contains("Settings");
                var settingsGroup = hasSettingsGroup
                    ? existing.PropertyGroups["Settings"]
                    : new PropertyGroup(new PropertyTypeCollection(true, shareProps))
                    {
                        Alias = "settings",
                        Name = "Settings",
                        Type = PropertyGroupType.Group,
                        SortOrder = 100
                    };

                foreach (var prop in shareProps)
                {
                    if (settingsGroup.PropertyTypes is not null && !settingsGroup.PropertyTypes.Contains(prop.Alias))
                    {
                        settingsGroup.PropertyTypes.Add(prop);
                        saveExisting = true;
                    }
                }

                if (!hasSettingsGroup)
                {
                    existing.PropertyGroups.Add(settingsGroup);
                    saveExisting = true;
                }

                if (saveExisting)
                {
                    _contentTypeService.Save(existing);
                }
            }


            // Online event
            existing = _contentTypeService.Get(Constants.ContentTypes.Guids.OnlineEvent);
            if (existing is null)
            {
                var contentType = CreateContentType(baseFolderContentTypeId, x =>
                {
                    x.Key = Constants.ContentTypes.Guids.OnlineEvent;
                    x.Alias = Constants.ContentTypes.Aliases.OnlineEvent;
                    x.Name = "Online Event";
                    x.Icon = "icon-cash-register color-green";
                    x.PropertyGroups = new PropertyGroupCollection(new[]
                    {
                        new PropertyGroup(new PropertyTypeCollection(true, shareProps))
                        {
                            Alias = "settings",
                            Name = "Settings",
                            Type = PropertyGroupType.Group,
                            SortOrder = 50
                        }
                    });
                });
            }
            else
            {
                var saveExisting = false;
                var hasSettingsGroup = existing.PropertyGroups.Contains("Settings");
                var settingsGroup = hasSettingsGroup
                    ? existing.PropertyGroups["Settings"]
                    : new PropertyGroup(new PropertyTypeCollection(true, shareProps))
                    {
                        Alias = "settings",
                        Name = "Settings",
                        Type = PropertyGroupType.Group,
                        SortOrder = 100
                    };

                foreach (var prop in shareProps)
                {
                    if (settingsGroup.PropertyTypes is not null && !settingsGroup.PropertyTypes.Contains(prop.Alias))
                    {
                        settingsGroup.PropertyTypes.Add(prop);
                        saveExisting = true;
                    }
                }

                if (!hasSettingsGroup)
                {
                    existing.PropertyGroups.Add(settingsGroup);
                    saveExisting = true;
                }

                if (saveExisting)
                {
                    _contentTypeService.Save(existing);
                }
            }

        }

        private ContentType CreateContentType(int parentId, Action<ContentType> config)
        {
            var contentType = new ContentType(_shortStringHelper, parentId);

            config.Invoke(contentType);

            return contentType;
        }

        private PropertyType CreatePropertyType(IDataType dataType, Action<PropertyType> config)
        {
            var propertyType = new PropertyType(_shortStringHelper, dataType);

            config.Invoke(propertyType);

            return propertyType;
        }
    }
}
