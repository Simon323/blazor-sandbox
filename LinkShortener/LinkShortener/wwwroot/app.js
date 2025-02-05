window.copyToClipboard = async (textToCopy) => {
    try {
        await navigator.clipboard.writeText(textToCopy);
        console.log('Text copied to clipboard:', textToCopy);
    } catch (err) {
        console.error('Failed to copy, error:', err);
    }
}