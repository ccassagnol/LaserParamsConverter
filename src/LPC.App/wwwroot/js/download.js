// Triggers a browser download from a byte[] passed by C# / Blazor Server.
// Used by Pages/Convert.razor.
window.lpcDownload = function (filename, contentType, bytesBase64) {
    const binary = atob(bytesBase64);
    const len = binary.length;
    const buf = new Uint8Array(len);
    for (let i = 0; i < len; i++) {
        buf[i] = binary.charCodeAt(i);
    }
    const blob = new Blob([buf], { type: contentType });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    setTimeout(() => URL.revokeObjectURL(url), 1000);
};
