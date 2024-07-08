﻿using System;
using System.Linq;
using Eventry.Installation;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Services.Implement;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Commerce.Cms;

namespace Eventry.Composing
{
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

        public EventryComponent(IDataTypeService dataTypeService,
            IContentTypeService contentTypeService,
            IShortStringHelper shortStringHelper,
            PropertyEditorCollection propertyEditors,
            IConfigurationEditorJsonSerializer configurationEditorJsonSerializer)
        {
            _dataTypeService = dataTypeService;
            _contentTypeService = contentTypeService;
            _shortStringHelper = shortStringHelper;
            _propertyEditors = propertyEditors;
            _configurationEditorJsonSerializer = configurationEditorJsonSerializer;
        }

        public void Initialize()
        {
            var dataTypesTask = new CreateEventryDataTypesTask(_dataTypeService,
                                                               _propertyEditors,
                                                               _configurationEditorJsonSerializer);
            Task.Run(() => dataTypesTask.Execute());

            var docTypesTask = new CreateEventryDocumentTypesTask(_contentTypeService,
                                                                  _dataTypeService,
                                                                  _shortStringHelper,
                                                                  _propertyEditors);
            Task.Run(() => docTypesTask.Execute());

        }

        public void Terminate()
        {
        }
    }
}