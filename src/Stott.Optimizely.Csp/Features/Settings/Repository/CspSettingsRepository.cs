﻿using System.Linq;
using System.Threading.Tasks;

using Stott.Optimizely.Csp.Entities;

namespace Stott.Optimizely.Csp.Features.Settings.Repository
{
    public class CspSettingsRepository : ICspSettingsRepository
    {
        private readonly CspDataContext _context;

        public CspSettingsRepository(CspDataContext context)
        {
            _context = context;
        }

        public async Task<CspSettings> GetAsync()
        {
            var settings = await _context.CspSettings.FirstOrDefaultAsync();

            return settings ?? new CspSettings();
        }

        public async Task SaveAsync(bool isEnabled, bool isReportOnly)
        {
            var recordToSave = await _context.CspSettings.FirstOrDefaultAsync();
            if (recordToSave == null)
            {
                recordToSave = new CspSettings();
                _context.CspSettings.Add(recordToSave);
            }

            recordToSave.IsEnabled = isEnabled;
            recordToSave.IsReportOnly = isReportOnly;

            await _context.SaveChangesAsync();
        }
    }
}
