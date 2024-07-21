using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;

namespace Eventry.Installation;

public class CreateEventryTemplatesTask
{
    private ITemplateService _templateService;
    private readonly IShortStringHelper _shortStringHelper;

    public CreateEventryTemplatesTask(ITemplateService templateService, IShortStringHelper shortStringHelper)
    {
        _templateService = templateService;
        _shortStringHelper = shortStringHelper;
    }

    public async Task Execute()
    {

        await CreateTemplate(Constants.Templates.Names.EventListing, Constants.Templates.Aliases.EventListing);
        await CreateTemplate(Constants.Templates.Names.OnlineEvent, Constants.Templates.Aliases.OnlineEvent);
        await CreateTemplate(Constants.Templates.Names.PhysicalEvent, Constants.Templates.Aliases.PhysicalEvent);        
     
    }

    private async Task CreateTemplate(string name, string alias)
    {
        ITemplate? currentTemplate;

        currentTemplate = await _templateService.GetAsync(alias);
        if (currentTemplate == null)
        {

            var template = new Template(_shortStringHelper, name, alias);

            //TODO: replace guid with admin's real guid
            await _templateService.CreateAsync(template, Guid.Parse("1e70f841-c261-413b-abb2-2d68cdb96094"));
        }
        
    }
}