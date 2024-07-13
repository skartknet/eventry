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
            var gmapDataType = await _dataTypeService.GetAsync(Constants.DataTypes.Guids.Maps) ?? throw new NullReferenceException("Map datatype not available");
            var textareaDataType = await _dataTypeService.GetAsync(Umbraco.Cms.Core.Constants.DataTypes.Guids.TextareaGuid) ?? throw new NullReferenceException("Textarea datatype not available");
            var rteDataType = await _dataTypeService.GetAsync(Umbraco.Cms.Core.Constants.DataTypes.Guids.RichtextEditorGuid) ?? throw new NullReferenceException("RTE datatype not available");
            var dateTimeDataType = await _dataTypeService.GetAsync(Umbraco.Cms.Core.Constants.DataTypes.Guids.DatePickerWithTimeGuid) ?? throw new NullReferenceException("DatePicker datatype not available");
            var contentPickerDataType = await _dataTypeService.GetAsync(Umbraco.Cms.Core.Constants.DataTypes.Guids.ContentPickerGuid) ?? throw new NullReferenceException("ContenPicker datatype not available");
            var umPriceDataType = await _dataTypeService.GetAsync(Constants.DataTypes.Guids.Price) ?? throw new NullReferenceException("Price datatype not available");
            var umStockDataType = await _dataTypeService.GetAsync(Constants.DataTypes.Guids.Stock) ?? throw new NullReferenceException("Stock datatype not available");


            IContentType? existing;
            int baseFolderContentTypeId;

            // Base folder
            existing = _contentTypeService.Get(Constants.ContentTypes.Guids.BaseFolder);
            if (existing == null)
            {
                var containerAttempt = _contentTypeService.CreateContainer(-1, Constants.ContentTypes.Guids.BaseFolder, "Eventry");
                if (containerAttempt.Success == false)
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

            var shareProps = new PropertyType[]
            {

                CreatePropertyType(textareaDataType, x =>
                {
                    x.Alias = "summary";
                    x.Name = "Summary";
                    x.Description = "";
                    x.SortOrder = 10;
                }),
                CreatePropertyType(rteDataType, x =>
                {
                    x.Alias = "description";
                    x.Name = "Description";
                    x.Description = "";
                    x.SortOrder = 20;
                }),
                CreatePropertyType(umPriceDataType, x =>
                {
                    x.Alias = "price";
                    x.Name = "Price";
                    x.Description = "";
                    x.SortOrder = 30;
                }),
                CreatePropertyType(umStockDataType, x =>
                {
                    x.Alias = "stock";
                    x.Name = "Capacity";
                    x.Description = "";
                    x.SortOrder = 40;
                }),
                CreatePropertyType(dateTimeDataType, x =>
                {
                    x.Alias = "start";
                    x.Name = "Start";
                    x.Description = "";
                    x.SortOrder = 50;
                }),
                CreatePropertyType(dateTimeDataType, x =>
                {
                    x.Alias = "end";
                    x.Name = "End";
                    x.Description = "";
                    x.SortOrder = 60;
                }),
                CreatePropertyType(contentPickerDataType, x =>
                {
                    x.Alias = "refundPolicyPage";
                    x.Name = "Refund Policy page";
                    x.Description = "";
                    x.SortOrder = 70;
                })
            };


            var physicalEventProps = new List<IPropertyType>
            {
                CreatePropertyType(gmapDataType, x =>
                {
                    x.Alias = "location";
                    x.Name = "Location";
                    x.Description = "Event physical location";
                    x.SortOrder = 10;
                })
            };
            physicalEventProps.AddRange(shareProps);

            var onlineEventProps = new List<IPropertyType>();
            onlineEventProps.AddRange(shareProps);


            var existingComposition = _contentTypeService.Get(Constants.ContentTypes.Guids.EventBaseComposition);
            if (existingComposition is null)
            {
                var contentType = CreateContentType(baseFolderContentTypeId, x =>
                {
                    x.Key = Constants.ContentTypes.Guids.EventBaseComposition;
                    x.Alias = Constants.ContentTypes.Aliases.EventBaseComposition;
                    x.Name = "Event Base Composition";
                    x.Icon = "icon-cash-register color-yellow";
                    x.IsElement = true;
                    x.PropertyGroups = new PropertyGroupCollection(new[]
                    {
                        new PropertyGroup(new PropertyTypeCollection(true, physicalEventProps))
                        {
                            Alias = "settings",
                            Name = "Settings",
                            Type = PropertyGroupType.Group,
                            SortOrder = 50
                        }
                    });
                });

                _contentTypeService.Save(contentType);

                existingComposition = contentType;

            }
            else
            {
                var saveExisting = false;
                var hasSettingsGroup = existingComposition.PropertyGroups.Contains("Settings");
                var settingsGroup = hasSettingsGroup
                    ? existingComposition.PropertyGroups["Settings"]
                    : new PropertyGroup(new PropertyTypeCollection(true, onlineEventProps))
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
                    existingComposition.PropertyGroups.Add(settingsGroup);
                    saveExisting = true;
                }

                if (saveExisting)
                {
                    _contentTypeService.Save(existing);
                }
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

                });

                var hasComposition = contentType.ContentTypeCompositionExists(Constants.ContentTypes.Aliases.EventBaseComposition);
                if (!hasComposition)
                {
                    contentType.AddContentType(existingComposition);
                }

                _contentTypeService.Save(contentType);
            }
            else
            {
                var saveExisting = false;
                var hasSettingsGroup = existing.PropertyGroups.Contains("Settings");
                var settingsGroup = hasSettingsGroup
                    ? existing.PropertyGroups["Settings"]
                    : new PropertyGroup(new PropertyTypeCollection(true, physicalEventProps))
                    {
                        Alias = "settings",
                        Name = "Settings",
                        Type = PropertyGroupType.Group,
                        SortOrder = 100
                    };

                foreach (var prop in physicalEventProps)
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

                var hasComposition = existing.ContentTypeCompositionExists(Constants.ContentTypes.Aliases.EventBaseComposition);
                if (!hasComposition)
                {
                    existing.AddContentType(existingComposition);
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

                });

                var hasComposition = contentType.ContentTypeCompositionExists(Constants.ContentTypes.Aliases.EventBaseComposition);
                if (!hasComposition)
                {
                    contentType.AddContentType(existingComposition);
                }

                _contentTypeService.Save(contentType);

            }
            else
            {
                var saveExisting = false;
                var hasSettingsGroup = existing.PropertyGroups.Contains("Settings");
                var settingsGroup = hasSettingsGroup
                    ? existing.PropertyGroups["Settings"]
                    : new PropertyGroup(new PropertyTypeCollection(true, onlineEventProps))
                    {
                        Alias = "settings",
                        Name = "Settings",
                        Type = PropertyGroupType.Group,
                        SortOrder = 100
                    };

                foreach (var prop in onlineEventProps)
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

                var hasComposition = existing.ContentTypeCompositionExists(Constants.ContentTypes.Aliases.EventBaseComposition);
                if (!hasComposition)
                {
                    existing.AddContentType(existingComposition);
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
