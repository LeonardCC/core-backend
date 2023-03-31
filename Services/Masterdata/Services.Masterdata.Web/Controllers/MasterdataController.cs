using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Lens.Core.Lib.Models;
using Lens.Services.Masterdata.Models;
using Lens.Services.Masterdata.Services;
using System.Net;

namespace Services.Masterdata.Web.Controllers;

[ApiController]
[Route("[controller]")]
[AllowAnonymous]
public class MasterdataController : ControllerBase
{

    private readonly IMasterdataService _masterdataService;

    public MasterdataController(IMasterdataService masterdataService)
    {
        _masterdataService = masterdataService;
    }

    #region HttpGet
    /// <summary>
    /// List all masterdata types.
    /// </summary>
    /// <param name="queryModel">The settings for paging, sorting and filtering.</param>
    /// <returns>A list of masterdata types.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ResultPagedListModel<MasterdataTypeListModel>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ResultPagedListModel<MasterdataTypeListModel>>> Get([FromQuery] QueryModel queryModel)
    {
        var result = await _masterdataService.GetMasterdataTypes(queryModel);
        return this.Ok(result);
    }

    /// <summary>
    /// Show the details of a masterdata type.
    /// </summary>
    /// <param name="masterdataType">The masterdata type (Id or Code).</param>
    /// <param name="domain">Application domain.</param>
    /// <returns>The details for a masterdata type.</returns>
    [HttpGet("{masterdataType}/details")]
    [ProducesResponseType(typeof(MasterdataTypeModel), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<MasterdataTypeModel?>> GetMasterdataType(string masterdataType, [FromHeader(Name = "masterdata-domain")]string? domain)
    {
        var result = await _masterdataService.GetMasterdataType(masterdataType, domain);
        return this.Ok(result);
    }

    /// <summary>
    /// List all masterdatas belonging to a specific masterdata type.
    /// </summary>
    /// <param name="masterdataType">The masterdata type (Id or Code).</param>
    /// <param name="queryModel">The settings for paging, sorting and filtering.</param>
    /// <returns>A list of masterdatas belonging to a specific masterdata type.</returns>
    [HttpGet("{masterdataType}")]
    [ProducesResponseType(typeof(ResultPagedListModel<MasterdataModel>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ResultPagedListModel<MasterdataModel>>> GetMasterdata(string masterdataType, [FromQuery] MasterdataQueryModel queryModel)
    {
        var result = await _masterdataService.GetMasterdata(masterdataType, queryModel);
        return this.Ok(result);
    }

    /// <summary>
    /// Show the details of a masterdata item.
    /// </summary>
    /// <param name="masterdataType">The masterdata type.</param>
    /// <param name="value">The masterdata item identifier (Id or Key).</param>
    /// <returns>The details for a masterdata item belonging to a specific masterdata type.</returns>
    [HttpGet("{masterdataType}/{value}")]
    [ProducesResponseType(typeof(MasterdataModel), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<MasterdataModel?>> GetMasterdata(string masterdataType, string value)
    {
        var result = await _masterdataService.GetMasterdata(masterdataType, value);
        return this.Ok(result);
    }

    /// <summary>
    /// List all alternative keys belonging to a masterdata item.
    /// </summary>
    /// <param name="masterdataType">The masterdata type.</param>
    /// <param name="value">The masterdata item identifier (Id or Key).</param>
    /// <param name="queryModel">The settings for paging, sorting and filtering.</param>
    /// <returns>A list of alternative keys belonging to a masterdata item.</returns>
    [HttpGet("{masterdataType}/{value}/keys")]
    [ProducesResponseType(typeof(ResultPagedListModel<MasterdataKeyModel>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ResultPagedListModel<MasterdataKeyModel>>> GetMasterdataKeys(string masterdataType, string value, [FromQuery] QueryModel queryModel)
    {
        var result = await _masterdataService.GetMasterdataKeys(masterdataType, value, queryModel);
        return this.Ok(result);
    }

    /// <summary>
    /// List all domains from all alternative keys associated with all masterdata items beloging to a masterdata type.
    /// </summary>
    /// <param name="masterdataType">The masterdata type (Id or Code).</param>
    /// <param name="value">The masterdata item identifier (Id or Key).</param>
    /// <param name="queryModel">The settings for paging, sorting and filtering.</param>
    /// <returns>A list of all domains from all alternative keys associated with all masterdata items beloging to a masterdata type.</returns>
    [HttpGet("{masterdataType}/{value}/keys/domains")]
    [ProducesResponseType(typeof(ResultPagedListModel<string>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ResultPagedListModel<string>>> GetDomains(string masterdataType, string value, [FromQuery] QueryModel queryModel)
    {
        var result = await _masterdataService.GetDomains(masterdataType, value, queryModel);
        return this.Ok(result);
    }

    /// <summary>
    /// List all tags associated with a specific masterdata type.
    /// </summary>
    /// <param name="masterdataType">The masterdata type (Id or Code).</param>
    /// <param name="queryModel">The settings for paging, sorting and filtering.</param>
    /// <returns>A list of tags associated with a specific masterdata type.</returns>
    [HttpGet("{masterdataType}/tags")]
    [ProducesResponseType(typeof(ResultPagedListModel<string>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ResultPagedListModel<string>>> GetTags(string masterdataType, [FromQuery] QueryModel queryModel)
    {
        var result = await _masterdataService.GetTags(masterdataType, queryModel);
        return this.Ok(result);
    }

    [HttpGet("{masterdataType}/{masterdata}/related/{relatedMasterdataType?}")]
    [ProducesResponseType(typeof(ResultListModel<MasterdataModel>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ResultListModel<MasterdataModel>>> GetRelated(string masterdataType, string masterdata, string? relatedMasterdataType = null, [FromQuery]bool includeDescendants = false)
    {
        var result = await _masterdataService.GetMasterdataRelated(masterdataType, masterdata, relatedMasterdataType, includeDescendants);
        return this.Ok(result);
    }
    #endregion

    #region HttpPost
    [HttpPost]
    [ProducesResponseType(typeof(MasterdataTypeModel), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<MasterdataTypeModel>> Post(MasterdataTypeCreateModel model)
    {
        var result = await _masterdataService.AddMasterdataType(model);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPost("{masterdataType}")]
    [ProducesResponseType(typeof(MasterdataModel), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<MasterdataModel>> Post(string masterdataType, MasterdataCreateModel model)
    {
        var result = await _masterdataService.AddMasterdata(masterdataType, model);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPost("{masterdataType}/{masterdata}/keys")]
    [ProducesResponseType(typeof(ICollection<MasterdataKeyModel>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ICollection<MasterdataKeyModel>>> PostKeys(string masterdataType, string masterdata, [FromBody] ICollection<MasterdataKeyCreateModel> model)
    {
        var result = await _masterdataService.AddMasterdataKeys(masterdataType, masterdata, model);
        return this.Ok(result);
    }

    [HttpPost("{masterdataType}/{masterdata}/related")]
    [ProducesResponseType(typeof(ICollection<MasterdataRelatedModel>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ICollection<MasterdataRelatedModel>>> PostRelated(string masterdataType, string masterdata, [FromBody] ICollection<MasterdataRelatedCreateModel> model)
    {
        var result = await _masterdataService.AddMasterdataRelated(masterdataType, masterdata, model);
        return this.Ok(result);
    }
    #endregion

    #region HttpPut
    [HttpPut("{masterdataType}/details")]
    [ProducesResponseType(typeof(MasterdataTypeModel), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<MasterdataTypeModel>> Put(string masterdataType, MasterdataTypeUpdateModel model)
    {
        var result = await _masterdataService.UpdateMasterdataType(masterdataType, model);
        return this.Ok(result);
    }

    [HttpPut("{masterdataType}/{masterdata}")]
    [ProducesResponseType(typeof(MasterdataModel), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<MasterdataModel>> Put(string masterdataType, string masterdata, MasterdataUpdateModel model)
    {
        var result = await _masterdataService.UpdateMasterdata(masterdataType, masterdata, model);
        return AcceptedAtAction(nameof(Get), new { id = result.Id }, result);
    }
    #endregion

    #region HttpDelete
    [HttpDelete("{masterdataType}/details")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<ActionResult> DeleteType(string masterdataType)
    {
        await _masterdataService.DeleteMasterdataType(masterdataType);
        return this.NoContent();
    }

    [HttpDelete("{masterdataType}/{masterdata}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<ActionResult> Delete(string masterdataType, string masterdata)
    {
        await _masterdataService.DeleteMasterdata(masterdataType, masterdata);
        return this.NoContent();
    }

    [HttpDelete("{masterdataType}/{masterdata}/keys")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<ActionResult> DeleteMasterdataKeys(string masterdataType, string masterdata)
    {
        await _masterdataService.DeleteMasterdataKeys(masterdataType, masterdata);
        return this.NoContent();
    }

    [HttpDelete("{masterdataType}/{masterdata}/keys/{alternativeKeyId}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<ActionResult> DeleteMasterdataKeys(string masterdataType, string masterdata, Guid alternativeKeyId)
    {
        await _masterdataService.DeleteMasterdataKeys(masterdataType, masterdata, alternativeKeyId);
        return this.NoContent();
    }

    [HttpDelete("{masterdataType}/{masterdata}/related")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<ActionResult> DeleteRelated(string masterdataType, string masterdata, [FromBody]List<Guid> relatedMasterdataIds)
    {
        await _masterdataService.DeleteMasterdataRelated(masterdataType, masterdata, relatedMasterdataIds);
        return this.NoContent();
    }
    #endregion

    #region Others
    [HttpPost("import")]
    [ProducesResponseType(typeof(MasterdataTypeModel), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<MasterdataTypeModel>> Import(MasterdataImportModel model)
    {
        var result = await _masterdataService.ImportMasterdata(model);
        return AcceptedAtAction(nameof(Get), new { id = result?.Id }, result);
    }
    #endregion
}
