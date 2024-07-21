using Eventry.Installation;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Commerce.Cms;

namespace Eventry.Composing;

[ComposeAfter(typeof(UmbracoCommerceComposer))]
public class EventryComposer : ComponentComposer<EventryComponent>
{
}

public class EventryComponent : IComponent
{
    private readonly IContentTypeService _contentTypeService;
    private readonly IDataTypeService _dataTypeService;
    private readonly IShortStringHelper _shortStringHelper;
    private readonly PropertyEditorCollection _propertyEditors;
    private readonly IConfigurationEditorJsonSerializer _configurationEditorJsonSerializer;
    private readonly ITemplateService _templateService;

    public EventryComponent(IDataTypeService dataTypeService,
                            IContentTypeService contentTypeService,
                            IShortStringHelper shortStringHelper,
                            PropertyEditorCollection propertyEditors,
                            IConfigurationEditorJsonSerializer configurationEditorJsonSerializer,
                            ITemplateService templateService)
    {
        _dataTypeService = dataTypeService;
        _contentTypeService = contentTypeService;
        _shortStringHelper = shortStringHelper;
        _propertyEditors = propertyEditors;
        _configurationEditorJsonSerializer = configurationEditorJsonSerializer;
        _templateService = templateService;
    }

    public void Initialize()
    {
        var dataTypesTask = new CreateEventryDataTypesTask(_dataTypeService,
                                                           _propertyEditors,
                                                           _configurationEditorJsonSerializer);
        Task.Run(dataTypesTask.Execute).GetAwaiter().GetResult();


        var templatesTask = new CreateEventryTemplatesTask(_templateService,
                                                           _shortStringHelper);

        Task.Run(templatesTask.Execute).GetAwaiter().GetResult();


        var docTypesTask = new CreateEventryDocumentTypesTask(_contentTypeService,
                                                              _dataTypeService,
                                                              _shortStringHelper,
                                                              _templateService,
                                                              _propertyEditors);
        Task.Run(docTypesTask.Execute).GetAwaiter().GetResult();

    }

    public void Terminate()
    {
    }
}
