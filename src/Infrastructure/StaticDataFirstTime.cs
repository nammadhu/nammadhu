﻿using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;
using CleanArchitecture.Blazor.Domain.Enums;

namespace CleanArchitecture.Blazor.Infrastructure;
public class StaticData//only for first time
{
    public static TenantDto DefaultTenant;
    private static List<ApplicationRoleDto> _roles ;
    private static List<TenantDto> _tenants ;
    readonly IIdentityService _identityService;
    readonly ITenantService _tenantService;
    public StaticData(IIdentityService identityService, ITenantService tenantService)
    {
        _identityService = identityService;
        _tenantService = tenantService;
    }
    public async Task<List<ApplicationRoleDto>> LoadUserBaseRoles(bool forceLoad = false)
    {
        if (forceLoad || _roles == null || _roles.Count == 0)
            _roles = (await _identityService.GetAllRoles());
        return _roles ?? throw new Exception("Base roles not defined");
    }

    public static List<ApplicationRoleDto> Roles()//All
    {
        return _roles;
    }
    public static List<ApplicationRoleDto>? RolesOfTenantType(byte tenantType = (byte)TenantTypeEnum.Default)
    {
        return tenantType == 0 ? (List<ApplicationRoleDto>?)null : (_roles?.Where(x => x.TenantType == tenantType).ToList());
    }

    public async Task<List<TenantDto>> LoadAllTenants(bool forceLoad = false)
    {
        if (forceLoad || _tenants == null || _tenants.Count == 0)
            _tenants = _tenantService.DataSource;
        if (_tenants != null && _tenants.Count > 0 && DefaultTenant == null)
            DefaultTenant = _tenants.FirstOrDefault(x => x.Type == (byte)TenantTypeEnum.Default) ?? throw new Exception("No default tenant defined");
        return _tenants ?? throw new Exception("Tenants are not defined");
    }
    public static List<TenantDto> AllTenants()
    {
        return _tenants ?? throw new Exception("Tenants are not defined");
    }
    public static TenantDto Tenant(string id)
    {
        return _tenants?.FirstOrDefault(x => x.Id == id) ?? throw new Exception("Tenants are not defined");
    }
    //todo voter
    //here we can add state,constituencies, even summary also with 1 hr once after reload logic
}