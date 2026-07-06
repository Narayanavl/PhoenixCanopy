//window.radzenExportPdf = function (tableId) {
//    const element = document.querySelector("table");
//    html2pdf().from(element).save();
//};

window.printThis = function (selector) {
    var html = document.querySelector(selector).outerHTML;
    var wnd = window.open("", "PrintGrid");
    wnd.document.write(html);
    wnd.print();
    wnd.close();
};

window.downloadFileFromBase64 = (filename, base64) => {
    const link = document.createElement('a');
    link.href = "data:text/csv;base64," + base64;
    link.download = filename;
    link.click();
};

window.radzenExportPdf = (element, filename) => {
    Radzen.exportPdf(element, { fileName: filename });
};

window.radzenExportExcel = (element, filename) => {
    Radzen.exportExcel(element, { fileName: filename });
};

window.openInNewTab = function (url) {
    window.open(url, '_blank', 'noopener,noreferrer');
};
window.downloadExcelFileFromBase64 = (fileName, base64) => {
    const link = document.createElement("a");
    link.download = fileName;
    link.href = "data:application/octet-stream;base64," + base64;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};
window.downloadPDFFileFromBase64 = (fileName, contentType, base64Data) => {
    const link = document.createElement("a");
    link.download = fileName;
    link.href = `data:${contentType};base64,${base64Data}`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};
window.copyTextToClipboard = (text) => {
    try {
        if (navigator.clipboard && window.isSecureContext) {
            // Modern API — executes immediately
            navigator.clipboard.writeText(text);
        } else {
            // Fallback for older browsers
            const textarea = document.createElement("textarea");
            textarea.value = text;
            textarea.style.position = "fixed";
            textarea.style.opacity = "0";
            document.body.appendChild(textarea);
            textarea.focus();
            textarea.select();
            document.execCommand("copy");
            document.body.removeChild(textarea);
        }
        // Optional: feedback
        console.log("Copied to clipboard!");
    } catch (err) {
        console.error("Clipboard copy failed:", err);
        alert("Unable to copy. Make sure the page is focused and try again.");
    }
};
window.showToast = (message) => {
    alert(message); // or implement your custom toast
};

window.fileManagerInstance = function (dotnetRef) {
    console.log("File manager initialized");
    window.dotnetRef = dotnetRef;
};

window.bindUploadMenuEvents = function () {
    console.log("Binding upload menu events");
};
window.openExcelViewer = function (url, provider) {
    if (!url) return;

    let viewerUrl = url;

    if (provider === "office") {
        viewerUrl = "https://view.officeapps.live.com/op/view.aspx?src="
            + encodeURIComponent(url);
    } else if (provider === "google") {
        viewerUrl = "https://docs.google.com/gview?url="
            + encodeURIComponent(url) + "&embedded=true";
    }

    window.open(viewerUrl, '_blank', 'noopener,noreferrer');
};
window.openDocumentViewer = function (url, provider) {
    if (!url) return;

    let viewerUrl = url;

    if (provider === "office") {
        // Works for Excel (.xlsx) AND Word (.docx)
        viewerUrl = "https://view.officeapps.live.com/op/view.aspx?src="
            + encodeURIComponent(url);
    }
    else if (provider === "google") {
        // Works for both as well
        viewerUrl = "https://docs.google.com/gview?url="
            + encodeURIComponent(url) + "&embedded=true";
    }

    window.open(viewerUrl, '_blank', 'noopener,noreferrer');
};
window.openFileTab = function (url) {
    if (!url) return;

    window.open(url, "_blank", "noopener,noreferrer");
};