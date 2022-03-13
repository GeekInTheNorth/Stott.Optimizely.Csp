﻿using System;

namespace Stott.Optimizely.Csp.Features.Reporting
{
    public class ViolationReportSummary
    {
        public int Key { get; set; }

        public string Source { get; set; }

        public string Directive { get; set; }

        public int Violations { get; set; }

        public DateTime LastViolated { get; set; }
    }
}
