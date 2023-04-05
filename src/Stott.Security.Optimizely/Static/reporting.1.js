function sendCspViolationReport(violationData) {

    if (violationData.blockedURI &&
        violationData.blockedURI.includes("stott.security.optimizely") &&
        violationData.violatedDirective &&
        violationData.violatedDirective.includes("connect-src"))
    {
        return;
    }

    let data = JSON.stringify({
        'blockedUri': violationData.blockedURI,
        'documentUri': violationData.documentURI,
        'originalPolicy': violationData.originalPolicy,
        'referrer': violationData.referrer,
        'scriptSample': violationData.sample,
        'sourceFile': violationData.sourceFile,
        'violatedDirective': violationData.violatedDirective,
        'effectiveDirective': violationData.effectiveDirective,
        'disposition': violationData.disposition
    });

    let xhr = new XMLHttpRequest();
    let reportUri = '/stott.security.optimizely/api/cspreporting/report/';

    xhr.open("POST", reportUri, true);
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.send(data);
}

document.addEventListener('securitypolicyviolation', (e) => { sendCspViolationReport(e); });