﻿namespace Stott.Security.Optimizely.Features.Sandbox.Repository;

using System;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Entities;

internal sealed class CspSandboxRepository : ICspSandboxRepository
{
    private readonly ICspDataContext _context;

    public CspSandboxRepository(ICspDataContext context)
    {
        _context = context;
    }

    public async Task<SandboxModel> GetAsync()
    {
        var sandboxSettings = await _context.CspSandboxes.FirstOrDefaultAsync();

        return CspSandboxMapper.ToModel(sandboxSettings);
    }

    public async Task SaveAsync(SandboxModel model, string modifiedBy)
    {
        var recordToSave = await _context.CspSandboxes.FirstOrDefaultAsync();

        if (recordToSave == null)
        {
            recordToSave = new CspSandbox();
            _context.CspSandboxes.Add(recordToSave);
        }

        CspSandboxMapper.ToEntity(model, recordToSave);

        recordToSave.Modified = DateTime.UtcNow;
        recordToSave.ModifiedBy = modifiedBy;

        await _context.SaveChangesAsync();
    }
}