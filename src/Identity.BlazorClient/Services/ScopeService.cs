using Company.Services.Identity.Shared.Models;
using Company.Services.Identity.Shared.Request;
using Company.Services.Identity.Shared.Responses;
using Company.Services.Identity.Shared.ViewModels;
using Microsoft.AspNetCore.WebUtilities;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Company.WebApps.Identity.BlazorClient.Services
{
    /// <summary>
    /// Service contract for consuming scopes api to manage sccopes
    /// </summary>
    public interface IScopeService
    {
        /// <summary>
        /// Get all the available scopes based on request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<PagedList<ScopeViewModel>?> GetScopesAsync(GetScopesRequest request);

        /// <summary>
        /// Get scope details given scope id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ScopeViewModel?> GetByIdAsync(string id);

        /// <summary>
        /// Add a new scope
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        Task<OperationResult> AddScopeAsync(ScopeViewModel scope);
       
        /// <summary>
        /// Update details of an existing scope
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        Task<OperationResult> UpdateScopeAsync(ScopeViewModel scope);


        /// <summary>
        /// Update details of an existing scope
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        Task<OperationResult> DeleteScopeAsync(ScopeViewModel scope);
    }

    public class ScopeService : IScopeService
    {
        private readonly HttpClient httpClient;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="httpClient"></param>
        public ScopeService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <inheritdoc/>
        public async Task<PagedList<ScopeViewModel>?> GetScopesAsync(GetScopesRequest request)
        {
            var queryStringParam = new Dictionary<string, string>
            {
                ["currentPage"] = request.CurrentPage.ToString(),
                ["pageSize"] = request.PageSize.ToString()
            };
            if (!string.IsNullOrEmpty(request.ScopesFilter))
            {
                queryStringParam.Add("scopesFilter", request.ScopesFilter);
            }
            return await httpClient.GetFromJsonAsync<PagedList<ScopeViewModel>>(QueryHelpers.AddQueryString("api/scopes", queryStringParam));          
        }

        /// <inheritdoc/>
        public async Task<ScopeViewModel?> GetByIdAsync(string id)
        {
            return await httpClient.GetFromJsonAsync<ScopeViewModel>($"api/scopes/id/{id}");          
        }

        /// <inheritdoc/>
        public async Task<OperationResult> AddScopeAsync(ScopeViewModel scope)
        {
            var result = await httpClient.PostAsJsonAsync<ScopeViewModel>("api/scopes", scope);
            return await OperationResult.FromResponseAsync(result);
        }

        /// <inheritdoc/>
        public async Task<OperationResult> UpdateScopeAsync(ScopeViewModel scope)
        {
            var result = await httpClient.PutAsJsonAsync<ScopeViewModel>("api/scopes", scope);
            return await OperationResult.FromResponseAsync(result);
        }

        /// <inheritdoc/>
        public async Task<OperationResult> DeleteScopeAsync(ScopeViewModel scope)
        {
            var result = await httpClient.DeleteAsync($"api/scopes/{scope.Id}");
            return await OperationResult.FromResponseAsync(result);
        }
    }
}
