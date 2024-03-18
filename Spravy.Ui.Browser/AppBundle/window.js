export function windowOpen(url) {
    return window.open(url, '_blank').focus();
}

export function getCurrentUrl() {
    return window.location.href;
}