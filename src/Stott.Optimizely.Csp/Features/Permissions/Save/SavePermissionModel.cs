﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

using Stott.Optimizely.Csp.Common;

namespace Stott.Optimizely.Csp.Features.Permissions.Save
{
    public class SavePermissionModel : IValidatableObject
    {
        public Guid Id { get; set; }

        public string Source { get; set; }

        public List<string> Directives { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!IsSourceValid())
            {
                yield return new ValidationResult($"{nameof(Source)} is invalid.", new[] { nameof(Source) });
            }

            if (!IsDirectivesValid())
            {
                yield return new ValidationResult($"{nameof(Directives)} contains invalid entries.", new[] { nameof(Directives) });
            }
        }

        private bool IsDirectivesValid()
        {
            if (Directives == null || !Directives.Any())
            {
                return false;
            }

            var allowedDirectives = CspConstants.AllDirectives;
            if (Directives.Any(x => !allowedDirectives.Contains(x)))
            {
                return false;
            }

            return true;
        }

        private bool IsSourceValid()
        {
            if (string.IsNullOrWhiteSpace(Source))
            {
                return false;
            }

            if (CspConstants.AllSources.Contains(this.Source))
            {
                return true;
            }

            return Regex.IsMatch(Source, "^([a-z0-9\\/\\-\\._\\:\\*\\[\\]\\@]{3,}\\.{1}[a-z0-9\\/\\-\\._\\:\\*\\[\\]\\@]{3,})$");
        }
    }
}
